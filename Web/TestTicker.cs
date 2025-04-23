using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;

namespace Web;

public class TestTicker
{
    private readonly TickerQDbContext _context;

    public TestTicker(TickerQDbContext context)
    {
        _context = context;
    }

    [TickerFunction(nameof(TestTicker))]
    public async Task ExecuteAsync(TickerFunctionContext functionContext, CancellationToken cancellationToken)
    {
        var jobExecution = new TickerExecution
        {
            TickerId = functionContext.Id,
            ExecutedBy = Environment.MachineName + ", " + Thread.CurrentThread.Name,
            ExecutedAt = DateTime.UtcNow,
        };

        _context.TickerExecutions.Add(jobExecution);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
