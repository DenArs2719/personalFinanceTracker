using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Data;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-many relationship explicitly (optional, but good practice)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)  // Each transaction has one user
            .WithMany(u => u.Transactions)  // Each user can have multiple transactions
            .HasForeignKey(t => t.UserId);  // Foreign key is UserId
        
        
        // Transaction → Category (foreign key: CategoryId + cascade delete)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}