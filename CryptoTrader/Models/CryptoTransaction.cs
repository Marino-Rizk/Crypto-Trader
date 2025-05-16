namespace CryptoTrader.Models
{
    public class CryptoTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CoinId { get; set; }
        public string Type { get; set; } 
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }
}