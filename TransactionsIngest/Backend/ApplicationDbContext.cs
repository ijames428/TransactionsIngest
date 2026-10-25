using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<HistoryEntry> History { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define the composite key using both values
        modelBuilder.Entity<Transaction>()
            .HasKey(t => new { t.sourceSystem, t.transactionId });
        modelBuilder.Entity<HistoryEntry>()
            .HasKey(t => new { t.sourceSystem, t.transactionId });
    }
}