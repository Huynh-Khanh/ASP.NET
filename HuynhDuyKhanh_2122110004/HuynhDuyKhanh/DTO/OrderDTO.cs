using System.ComponentModel.DataAnnotations;

namespace HuynhDuyKhanh.DTO
{
    public class OrderDTO
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // FK to User
        public int UserId { get; set; }

    }
}
