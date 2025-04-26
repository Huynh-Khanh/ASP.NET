using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HuynhDuyKhanh.Data;
using HuynhDuyKhanh.DTO;
using HuynhDuyKhanh.Model;
using Microsoft.AspNetCore.Authorization;


namespace HuynhDuyKhanh.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; // Khai báo AutoMapper

        public OrderController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper; // Khởi tạo AutoMapper
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> Create(OrderDTO orderdto)
        {
            if (orderdto == null)
                return BadRequest();

            // Dùng AutoMapper để chuyển OrderDTO thành Order entity
            var order = _mapper.Map<Order>(orderdto);

            _context.Orders.Add(order);  // Thêm đối tượng Order vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);  // Trả về thông tin của order vừa tạo
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDTO orderdto)
        {
            if (id != orderdto.OrderId)
                return BadRequest();

            // Dùng AutoMapper để chuyển OrderDTO thành Order entity
            var order = _mapper.Map<Order>(orderdto);

            _context.Entry(order).State = EntityState.Modified;  // Cập nhật trạng thái của Order

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.OrderId == id))
                    return NotFound();

                throw;
            }

            return NoContent();  // Trả về 204 khi cập nhật thành công
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();  // Trả về 204 khi xóa thành công
        }
    }
}
