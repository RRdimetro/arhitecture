using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}