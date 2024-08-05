namespace SPMA.Dtos.Sales
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int? Discount { get; set; }
        public string Comment { get; set; }
    }
}
