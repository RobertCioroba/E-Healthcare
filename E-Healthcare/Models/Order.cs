using E_Healthcare.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_Healthcare.Models
{
    public class Order : BaseEntity
    {
        public int UserID { get; set; }

        [Display(Name = "Total amount")]
        public double TotalAmount { get; set; }

        [Display(Name = "Placed on")]
        public DateTime PlacedOn { get; set; }

        public OrderStatus Status { get; set; }

        public virtual User User { get; set; }
    }
}
