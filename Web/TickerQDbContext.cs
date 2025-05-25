using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.Configurations;
using TickerQ.EntityFrameworkCore.Entities;
using TickerQ.Utilities.Models.Ticker;

namespace Web;

public class TickerQDbContext : DbContext
{
    public TickerQDbContext(DbContextOptions<TickerQDbContext> options)
        : base(options) { }

    public DbSet<TickerExecution> TickerExecutions { get; set; } = null!;

    public DbSet<TimeTicker> TimeTickers { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TimeTickerConfigurations());
        modelBuilder.ApplyConfiguration(new CronTickerConfigurations());
        modelBuilder.ApplyConfiguration(new CronTickerOccurrenceConfigurations());

        modelBuilder.Entity<TickerExecution>(table =>
        {
            table.HasKey(x => new { JobId = x.TickerId, x.ExecutedBy });
        });
    }
}

public class TickerExecution
{
    public Guid TickerId { get; set; }

    public required string ExecutedBy { get; set; }

    public DateTime ExecutedAt { get; set; }
}
