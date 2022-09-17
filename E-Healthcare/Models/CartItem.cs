namespace E_Healthcare.Models
{
    public class CartItem : BaseEntity
    {
        public int CartID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public virtual Cart Cart { get; set; }

        public virtual Product Product { get; set; }
    }
}
