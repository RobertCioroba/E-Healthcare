namespace E_Healthcare.Models
{
    public class Cart : BaseEntity
    {
        public int OwnerID { get; set; }

        public virtual User Owner { get; set; }

        public virtual ICollection<CartItem> Items { get; set; }
    }
}
