using Microsoft.EntityFrameworkCore;

namespace Wallets.Implemmentation.Context.DbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; } // DbSet for Wallet model

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure any additional settings or relationships here
        }
    }
}
