namespace E_Healthcare.Models
{
    public class Order : BaseEntity
    {
        public int UserID { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime PlacedOn { get; set; }

        public virtual User User { get; set; }
    }
}
