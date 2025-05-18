using CryptoTrader.Models;

namespace CryptoTrader.Models
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string CoinId { get; set; } 
        public string CoinName { get; set; } 

        public decimal NetAmount { get; set; }  
    }
}
