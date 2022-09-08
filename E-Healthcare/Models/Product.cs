using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class Product : BaseEntity
    {
        [Required]
        [Display(Name = "Product name")]
        [StringLength(30, ErrorMessage = "Product name length can't be more than 30.")]
        public string Name { get; set; }

        [Display(Name = "Company name")]
        [StringLength(20, ErrorMessage = "Company name length can't be more than 20.")]
        public string CompanyName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string ImageUrl { get; set; }

        public string Uses { get; set; }
        [Required]
        [Display(Name = "Expire date")]
        public string ExpireDate { get; set; }
    }
}
