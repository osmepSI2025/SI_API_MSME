using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using SME_API_MSME.Entities;
using SME_API_MSME.Services;

public class ScheduledJobPuller : IJob
{
    private readonly MSMEDBContext _dbContext;
    private readonly ILogger<ScheduledJobPuller> _logger;
    private readonly ProjectService _projectService;
     private readonly ProjectAreaService _projectAreaService;
    private readonly ProjectProductService _projectProductService;
    private readonly ProjectOutcomeService _projectOutcomeService;
    private readonly ExpectedOutcomeService _expectedOutcomeService;
    private readonly BudgetPlanService _budgetPlanService;
    private readonly DisbursementResultService _disbursementResultService;
    private readonly OutcomeResultService _outcomeResultService;
    private readonly PerformanceResultService _performanceResultService;
    private readonly ExpectedOutcomeResultService _expectedOutcomeResultService;
    private readonly ProjectActivityService _projectActivityService;
    private readonly EconomicValueService _economicValueService;
    private readonly ProductResultService _productResultService;
    private readonly IServiceProvider _serviceProvider;

    public ScheduledJobPuller(
        MSMEDBContext dbContext,
        ILogger<ScheduledJobPuller> logger,
        ProjectService projectService,
        ProjectAreaService projectAreaService,
        ProjectProductService projectProductService,
        ProjectOutcomeService projectOutcomeService,
        ExpectedOutcomeService expectedOutcomeService,
        BudgetPlanService budgetPlanService,
        DisbursementResultService disbursementResultService,
        OutcomeResultService outcomeResultService,
        PerformanceResultService performanceResultService,
        ExpectedOutcomeResultService expectedOutcomeResultService,
         ProjectActivityService projectActivityService,
         EconomicValueService economicValueService,
            ProductResultService productResultService,
            IServiceProvider serviceProvider


        )

    {
        _dbContext = dbContext;
        _logger = logger;
        _projectService = projectService;
        _projectAreaService = projectAreaService;
        _projectProductService = projectProductService;
        _projectOutcomeService = projectOutcomeService;
        _expectedOutcomeService = expectedOutcomeService;
        _budgetPlanService = budgetPlanService;
        _disbursementResultService = disbursementResultService;
        _outcomeResultService = outcomeResultService;
        _performanceResultService = performanceResultService;
        _expectedOutcomeResultService = expectedOutcomeResultService;
        _projectActivityService = projectActivityService;
        _economicValueService = economicValueService;
        _productResultService = productResultService;
        _serviceProvider = serviceProvider;

    }

    public async Task Execute(IJobExecutionContext context)
    {
        // สร้าง scope ใหม่สำหรับ Job นี้
        using (var scope = _serviceProvider.CreateScope())
        {
            // ดึงค่า jobName จาก JobDataMap
            var jobName = context.JobDetail.JobDataMap.GetString("JobName");
            _logger.LogInformation($"Executing job: {jobName}");

            try
            {
                var serviceProvider = scope.ServiceProvider;
                switch (jobName)
                {
                    case "project":
                        await _projectService.batchEndOfday();
                        break;
                    case "project-area":
                        await _projectAreaService.batchEndOfday();
                        break;
                    case "project-product":
                        await _projectProductService.batchEndOfday();
                        break;
                    case "project-outcome":
                        await _projectOutcomeService.batchEndOfday();
                        break;
                    case "expected-outcome":
                        await _expectedOutcomeService.batchEndOfday();
                        break;
                    case "budget-plan":
                        await _budgetPlanService.batchEndOfday();
                        break;
                    case "disbursement-result":
                        await _disbursementResultService.batchEndOfday();
                        break;
                    case "outcome-result":
                        await _outcomeResultService.batchEndOfday();
                        break;
                    case "performance-result":
                        await _performanceResultService.batchEndOfday();
                        break;
                    case "expected-outcome-result":
                        await _expectedOutcomeResultService.batchEndOfday();
                        break;
                    case "project-activity":
                        await _projectActivityService.batchEndOfday();
                        break;
                    case "economic-value":
                        await _economicValueService.batchEndOfday();
                        break;
                    case "product-result":
                        await _productResultService.batchEndOfday();
                        break;
                    default:
                        // Optionally log unknown job
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing job {jobName}.");
            }
        }
    }
   

    public class JobSchedulerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobSchedulerService> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public JobSchedulerService(IServiceProvider serviceProvider, ILogger<JobSchedulerService> logger, ISchedulerFactory schedulerFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("JobSchedulerService is starting.");
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MSMEDBContext>();
                var jobs = await dbContext.MScheduledJobs.Where(j => j.IsActive == true).ToListAsync(cancellationToken);

                // Clear all triggers in the "dynamic" group before scheduling
                var allScheduledJobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals("dynamic"));
                foreach (var key in allScheduledJobKeys)
                {
                    var triggers = await scheduler.GetTriggersOfJob(key, cancellationToken);
                    foreach (var trigger in triggers)
                    {
                        await scheduler.UnscheduleJob(trigger.Key, cancellationToken);
                        _logger.LogInformation($"Trigger '{trigger.Key.Name}' for job '{key.Name}' deleted.");
                    }
                }

                foreach (var job in jobs)
                {
                    // แก้ไข: เพิ่มการตรวจสอบค่าว่างเปล่า (whitespace)
                    if (!int.TryParse(job.RunMinute.ToString(), out _) || !int.TryParse(job.RunHour.ToString(), out _))
                    {
                        _logger.LogError($"Job '{job.JobName}' has invalid RunMinute or RunHour. Skipping.");
                        continue;
                    }
                    string cron = $"0 {job.RunMinute} {job.RunHour} * * ?";
                    var jobKey = new JobKey(job.JobName, "dynamic");

                    // ตรวจสอบว่า Job มีอยู่แล้วหรือไม่
                    if (await scheduler.CheckExists(jobKey, cancellationToken))
                    {
                        _logger.LogInformation($"Job '{job.JobName}' already exists. Rescheduling with new trigger.");

                        var trigger = TriggerBuilder.Create()
                            .WithIdentity($"{job.JobName}-trigger", "dynamic")
                            .WithCronSchedule(cron)
                            .Build();

                        await scheduler.RescheduleJob(trigger.Key, trigger, cancellationToken);
                    }
                    else
                    {
                        _logger.LogInformation($"Job '{job.JobName}' does not exist. Creating a new one.");

                        var jobDetail = JobBuilder.Create<ScheduledJobPuller>()
                            .WithIdentity(jobKey)
                            .UsingJobData("JobName", job.JobName)
                            .Build();

                        var trigger = TriggerBuilder.Create()
                            .WithIdentity($"{job.JobName}-trigger", "dynamic")
                            .WithCronSchedule(cron)
                            .Build();

                        await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("JobSchedulerService is stopping.");
            return Task.CompletedTask;
        }
    }
}