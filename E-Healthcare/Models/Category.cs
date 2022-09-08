using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [Display(Name = "Category name")]
        [StringLength(20, ErrorMessage = "Category name length can't be more than 20.")]
        public string Name { get; set; }
    }
}
