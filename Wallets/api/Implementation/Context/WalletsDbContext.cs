using Microsoft.EntityFrameworkCore;
using Wallets.Types.Models;

namespace Wallets.Implementation.Context
{
    public class WalletsDbContext : DbContext
    {
        public WalletsDbContext(DbContextOptions<WalletsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<CurrencyRates> CurrencyRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CurrencyRates>()
            .HasKey(cr => new { cr.Currency, cr.Date });
        }
    }
}
