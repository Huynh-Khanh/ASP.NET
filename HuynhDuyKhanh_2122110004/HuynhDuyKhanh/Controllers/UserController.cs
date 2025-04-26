using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HuynhDuyKhanh.Data;
using HuynhDuyKhanh.DTO;
using HuynhDuyKhanh.Model;
using AutoMapper;
using BCrypt.Net;
using System.Linq;

namespace HuynhDuyKhanh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
        }

        // Đăng ký người dùng
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput.Password) || userInput.Password.Length < 6)
            {
                return BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            if (_context.Users.Any(u => u.Username == userInput.Username))
            {
                return BadRequest("Người dùng đã tồn tại.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userInput.Password);
            var user = _mapper.Map<User>(userInput);
            user.Password = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        // Đăng nhập người dùng
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == login.Username);

            if (user == null)
            {
                return BadRequest(new { message = "Username not found" });
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    return BadRequest(new { message = "Incorrect password" });
                }

                // Nếu mật khẩu cần rehash, cập nhật lại mật khẩu
                if (BCrypt.Net.BCrypt.PasswordNeedsRehash(user.Password, 12)) // Sử dụng 12 làm work factor
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(login.Password, 12); // Băm mật khẩu với work factor 12
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi mã hóa mật khẩu. Vui lòng thử lại sau." });
            }

            var token = GenerateJwtToken(user);
            var userDto = _mapper.Map<UserDTO>(user);

            return Ok(new
            {
                token,
                user = userDto
            });
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList(); // Lấy tất cả người dùng từ cơ sở dữ liệu

            if (users == null || !users.Any())
            {
                return NotFound("Không có người dùng nào.");
            }

            var userDtos = _mapper.Map<List<UserDTO>>(users); // Chuyển đổi danh sách người dùng sang DTO
            return Ok(userDtos); // Trả về danh sách người dùng
        }

        // Xóa người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id); // Tìm người dùng theo ID

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            _context.Users.Remove(user); // Xóa người dùng khỏi cơ sở dữ liệu
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return Ok(new { message = "Xóa người dùng thành công." }); // Trả về thông báo thành công
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userInput)
        {
            // Kiểm tra xem người dùng có tồn tại trong cơ sở dữ liệu không
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Kiểm tra các trường dữ liệu hợp lệ
            if (string.IsNullOrWhiteSpace(userInput.Username))
            {
                return BadRequest("Tên người dùng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(userInput.Email))
            {
                return BadRequest("Email không được để trống.");
            }

            // Nếu mật khẩu được thay đổi, mã hóa lại mật khẩu
            if (!string.IsNullOrWhiteSpace(userInput.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userInput.Password);
            }

            // Cập nhật thông tin người dùng
            user.Username = userInput.Username;
            user.Email = userInput.Email;
            // Cập nhật tên người dùng nếu có thay đổi
            if (!string.IsNullOrWhiteSpace(userInput.Name))
            {
                user.Name = userInput.Name;
            }

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi cập nhật thông tin người dùng: " + ex.Message);
            }

            var updatedUserDto = _mapper.Map<UserDTO>(user); // Chuyển đổi thông tin người dùng sau khi cập nhật sang DTO
            return Ok(updatedUserDto); // Trả về thông tin người dùng đã cập nhật
        }

        // Hàm tạo JWT token
        private string GenerateJwtToken(User user)
        {
            var keyString = _config["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32BytesLong1234"; // fallback key
            var key32 = keyString.PadRight(32, 'x');
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key32));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
