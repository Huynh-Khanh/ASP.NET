namespace HuynhDuyKhanh_2122110004.Model
{
    public class User
    {
        public int Id { get; set; }                      // ID người dùng
        public string Username { get; set; }             // Tên đăng nhập
        public string Password { get; set; }             // Mật khẩu
        public string Image { get; set; }                // Đường dẫn ảnh đại diện
        public string Email { get; set; }                // Email người dùng
        public string Phone { get; set; }                // Số điện thoại
        public string Address { get; set; }              // Địa chỉ
        public DateTime CreatedAt { get; set; }          // Ngày tạo tài khoản
        public DateTime UpdatedAt { get; set; }          // Ngày cập nhật thông tin
        public string Role { get; set; } = "Customer";   // Vai trò: "Admin" hoặc "Customer"
    }
}
