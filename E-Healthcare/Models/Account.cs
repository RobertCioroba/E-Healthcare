namespace E_Healthcare.Models
{
    public class Account : BaseEntity
    {
        public int AccNumber { get; set; }

        public int Amount { get; set; }

        public string Email { get; set; }
    }
}
