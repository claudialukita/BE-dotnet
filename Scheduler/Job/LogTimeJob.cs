using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class LogTimeJob : IJob
    {

        private readonly ILogger _logger;

        public LogTimeJob(ILogger<LogTimeJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Current Date : {DateTime.UtcNow}");
        }
    }
}
