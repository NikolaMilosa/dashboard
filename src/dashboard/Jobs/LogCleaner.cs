
using dashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace dashboard.Jobs;

public class LogCleaner : IHostedService
{

    private CancellationTokenSource _localToken;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LogCleaner> _logger;

    public LogCleaner(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope().ServiceProvider;
        _dbContext = scope.GetRequiredService<ApplicationDbContext>();
        _logger = scope.GetRequiredService<ILogger<LogCleaner>>();
        _localToken = new CancellationTokenSource(); 
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(DoWork);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shutdown completed");
        return Task.CompletedTask;
    }

    private async Task DoWork() {
        _logger.LogInformation("Waiting 15s to be sure migrations are applied");
        await Task.Delay(new TimeSpan(0, 0, 15), _localToken.Token);
        _logger.LogInformation("Running loop");

        while (true) {
            _logger.LogInformation("Checking what should be deleted");
            var threshold = DateTime.Now.AddYears(-1);
            var logsToDelete = await _dbContext.EventLogs.Where(x => x.Timestamp < threshold.ToUniversalTime()).ToListAsync();
            
            _logger.LogInformation("Found {0} logs older than {1}. Deleting them...", logsToDelete.Count, threshold.ToString("dd/MM/yyyy"));
            _dbContext.EventLogs.RemoveRange(logsToDelete);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Sleeping for a day");
            await Task.Delay(new TimeSpan(1, 0, 0, 0));
        }
    }
}
