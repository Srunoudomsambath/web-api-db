using System.ComponentModel.DataAnnotations;

namespace WebAPIDB.Models
{
    public class PostProductDao
    {
        [Required]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01,double.MaxValue,ErrorMessage ="Price must be greater than 0.")]
        public double Price { get; set; }

        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Quantity must be at least 1.")]
        public int Qty{ get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
