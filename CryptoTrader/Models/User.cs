namespace CryptoTrader.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
}