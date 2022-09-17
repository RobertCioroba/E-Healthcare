using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class Account : BaseEntity
    {
        [Display(Name = "Account number")]
        public int AccNumber { get; set; }

        public double Amount { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
