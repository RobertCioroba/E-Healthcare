namespace E_Healthcare.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; }

        public string Uses { get; set; }

        public string ExpireDate { get; set; }
    }
}
