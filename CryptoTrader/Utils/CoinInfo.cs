namespace CryptoTrader.Utils
{
    public static class CoinInfo
    {
        public static readonly Dictionary<string, string> CoinNames = new()
        {
            { "bitcoin", "Bitcoin (BTC)" },
            { "ethereum", "Ethereum (ETH)" },
            { "tether", "Tether (USDT)" },
            { "binancecoin", "BNB (BNB)" },
            { "usd-coin", "USD Coin (USDC)" },
            { "ripple", "XRP (XRP)" },
            { "cardano", "Cardano (ADA)" },
            { "dogecoin", "Dogecoin (DOGE)" },
            { "polygon", "Polygon (MATIC)" },
            { "solana", "Solana (SOL)" },
            { "polkadot", "Polkadot (DOT)" },
            { "litecoin", "Litecoin (LTC)" },
            { "shiba-inu", "Shiba Inu (SHIB)" },
            { "chainlink", "Chainlink (LINK)" },
            { "uniswap", "Uniswap (UNI)" }
        };
    }
}
