using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HuynhDuyKhanh.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Mối quan hệ với Order
        public ICollection<Order> Orders { get; set; }
    }
}
