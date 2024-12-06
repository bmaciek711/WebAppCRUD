namespace WebAppCRUD.Models
{
    public class BusinessContact : Contact
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}