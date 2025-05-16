using Microsoft.EntityFrameworkCore;
using CryptoTrader.Models;
using System.Collections.Generic;

namespace CryptoTrader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CryptoTransaction> Transactions { get; set; }
    }
}
