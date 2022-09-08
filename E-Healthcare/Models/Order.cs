using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class Order : BaseEntity
    {
        public int UserID { get; set; }

        [Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Placed on")]
        public DateTime PlacedOn { get; set; }

        public virtual User User { get; set; }
    }
}
