using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Models;
namespace MyPlanner.Data
{
    public class FinanceContext : IdentityDbContext
    {
        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Moneybox> Moneyboxes { get; set; }
    }
}