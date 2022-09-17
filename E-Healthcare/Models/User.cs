using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class User : BaseEntity
    {
        [Required]
        [Display(Name = "First name")]
        [StringLength(20, ErrorMessage = "First name length can't be more than 20.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(20, ErrorMessage = "Last name length can't be more than 20.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string AdminPassword { get; set; }
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public bool IsAdmin { get; set; } = false;

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Address length can't be more than 50.")]
        public string Address { get; set; }
    }
}
