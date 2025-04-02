namespace Ecommerce.Models
{
    public class LoginInfo
    {
        public int Id { get; set; }
        public DateTime Expires { get; set; }
        // Svešā atslēga uz lietotāju
        public int UserId { get; set; }

        // Navigācijas īpašība atpakaļ uz User
        public User User { get; set; }
    }
}
