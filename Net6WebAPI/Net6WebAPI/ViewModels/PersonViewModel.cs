namespace Net6WebAPI.ViewModels
{
    public class PersonViewModel
    {
        public Guid Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string ImageURI { get; set; } = string.Empty;
        public char Gender { get; set; }
        public decimal Grade { get; set; }
        public decimal CashOnHand { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
