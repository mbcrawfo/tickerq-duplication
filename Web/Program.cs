using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models.Ticker;
using Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TickerQDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddTickerQ(options =>
{
    options.SetMaxConcurrency(1);
    options.AddOperationalStore<TickerQDbContext>();
    options.AddDashboard("/dashboard");
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// TickerQ tries to hit the db as soon as it starts, even in manual mode, so migrations must be applied before that.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TickerQDbContext>();
    await context.Database.MigrateAsync();

    var idleTickers = await context.TimeTickers.CountAsync(tt => tt.Status == TickerStatus.Idle);
    if (idleTickers < 1000)
    {
        var timeTickerManager = scope.ServiceProvider.GetRequiredService<ITimeTickerManager<TimeTicker>>();
        foreach (var _ in Enumerable.Range(start: 0, 1000 - idleTickers))
        {
            await timeTickerManager.AddAsync(
                new TimeTicker
                {
                    Function = nameof(TestTicker),
                    ExecutionTime = DateTime.UtcNow,
                    Retries = 0,
                }
            );
        }
    }
}

app.UseHealthChecks("/health");
app.UseTickerQ(TickerQStartMode.Manual);

app.MapGet(
    "/duplicates",
    async (TickerQDbContext context, CancellationToken cancellationToken) =>
    {
        var duplicatedExecutions = context
            .TickerExecutions.GroupBy(je => je.TickerId)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(100);

        var result = await context
            .TickerExecutions.AsNoTracking()
            .Where(je => duplicatedExecutions.Contains(je.TickerId))
            .ToListAsync(cancellationToken);

        return result
            .GroupBy(je => je.TickerId)
            .OrderByDescending(g => g.Count())
            .ToImmutableSortedDictionary(
                g => g.Key,
                g => g.Select(je => new { je.ExecutedBy, je.ExecutedAt }).OrderBy(x => x.ExecutedAt).ToImmutableArray()
            );
    }
);

await app.RunAsync();
