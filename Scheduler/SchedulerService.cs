using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Scheduler.Jobs;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    public class SchedulerService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IScheduler _scheduler;


        public SchedulerService(IConfiguration configuration, IJobFactory quartzJobFactory)
        {
            _configuration = configuration;
            StdSchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            _scheduler.JobFactory = quartzJobFactory;
        }

        public void Initialize()
        {
            string logTime = _configuration.GetValue<string>("Schedule:LogTimeJob");
            AddJob<LogTimeJob>(logTime);
        }

        private void AddJob<T>(string cronExpression) where T : IJob
        {
            string jobName = typeof(T).Name;
            string groupName = jobName + "Group";
            string triggerName = jobName + "Trigger";

            IJobDetail jobDetail = JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .StartNow()
                .WithCronSchedule(cronExpression)
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Initialize();
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_scheduler.IsStarted)
            {
                await _scheduler.Standby();
            }

            System.Collections.Generic.IReadOnlyCollection<JobKey> jobKeys = _scheduler
                .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .GetAwaiter()
                .GetResult();

            foreach (JobKey jobKey in jobKeys)
            {
                _ = _scheduler.Interrupt(jobKey);
            }

            _scheduler
                .Shutdown(true)
                .GetAwaiter()
                .GetResult();
        }
    }
}
