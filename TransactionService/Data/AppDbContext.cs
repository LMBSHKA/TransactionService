using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Transaction> TransactionsService { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
