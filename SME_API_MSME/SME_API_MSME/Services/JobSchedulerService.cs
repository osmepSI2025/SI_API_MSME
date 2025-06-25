using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;
using SME_API_MSME.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class JobSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public JobSchedulerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MSMEDBContext>();
                var now = DateTime.Now;
                var jobs = await db.MScheduledJobs
                    .Where(j => j.IsActive == true && j.RunHour == now.Hour && j.RunMinute == now.Minute)
                    .ToListAsync(stoppingToken);

                foreach (var job in jobs)
                {
                    _ = RunJobAsync(job.JobName, scope.ServiceProvider);
                }
            }

            // Check every minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task RunJobAsync(string jobName, IServiceProvider serviceProvider)
    {
        switch (jobName)
        {
            case "project":
                await serviceProvider.GetRequiredService<ProjectService>().batchEndOfday();
                break;
            case "project-area":
                await serviceProvider.GetRequiredService<ProjectAreaService>().batchEndOfday();
                break;
            case "project-product":
                await serviceProvider.GetRequiredService<ProjectProductService>().batchEndOfday();
                break;
            case "project-outcome":
                await serviceProvider.GetRequiredService<ProjectOutcomeService>().batchEndOfday();
                break;
            case "expected-outcome":
                await serviceProvider.GetRequiredService<ExpectedOutcomeService>().batchEndOfday();
                break;
            case "budget-plan":
                await serviceProvider.GetRequiredService<BudgetPlanService>().batchEndOfday();
                break;
            case "disbursement-result":
                await serviceProvider.GetRequiredService<DisbursementResultService>().batchEndOfday();
                break;
            case "outcome-result":
                await serviceProvider.GetRequiredService<OutcomeResultService>().batchEndOfday();
                break;
            case "performance-result":
                await serviceProvider.GetRequiredService<PerformanceResultService>().batchEndOfday();
                break;
            case "expected-outcome-result":
                await serviceProvider.GetRequiredService<ExpectedOutcomeResultService>().batchEndOfday();
                break;
            case "project-activity":
                await serviceProvider.GetRequiredService<ProjectActivityService>().batchEndOfday();
                break;
            case "economic-value":
                await serviceProvider.GetRequiredService<EconomicValueService>().batchEndOfday();
                break;
            case "product-result":
                await serviceProvider.GetRequiredService<ProductResultService>().batchEndOfday();
                break;
            default:
                // Optionally log unknown job
                break;
        }
    }
}