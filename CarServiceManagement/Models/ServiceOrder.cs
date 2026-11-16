
using System.ComponentModel.DataAnnotations;

namespace CarServiceManagement.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public Vehicle? Vehicle { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateCompleted { get; set; }

        [Required]
        public string Status { get; set; } = "Na čekanju";

        public decimal Price { get; set; }
    }
}
