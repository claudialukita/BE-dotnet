using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BLL.Kafka
{
    public class ConsumerService : BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConfiguration _config;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger _logger;
        private readonly ProcessSumService _processSumService;

        public ConsumerService(IConfiguration config, ILogger<ConsumerService> logger, ProcessSumService processNameService)
        {
            _config = config;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.GetValue<string>("Kafka"),
                //GroupId = config.GetValue<string>("Kafka:GroupId"),
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true,
                IsolationLevel=IsolationLevel.ReadCommitted
            };
            _logger = logger;
            _processSumService = processNameService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            List<Task> tasks = new List<Task>
            {
                RegisterTaskProcessName(stoppingToken),
                RegisterTaskCategoryName(stoppingToken)
            };

            await Task.WhenAll(tasks.ToArray());
        }

        private Task RegisterTaskProcessName(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                var topic = _config.GetValue<string>("Topic:ProcessName");
                do
                {
                    try
                    {
                        await ConsumeAsync(topic, "sell_name", _processSumService.ConsumeAsync, false, _cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, $"Error when consuming topic \"{topic}\".");
                    }
                } while (!_cancellationTokenSource.IsCancellationRequested);
            }, stoppingToken);
        }

        private Task RegisterTaskCategoryName(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                var topic = _config.GetValue<string>("Topic:ProcessCategory");
                do
                {
                    try
                    {
                        await ConsumeAsync(topic, "sell_category", _processSumService.ConsumeAsync, false, _cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, $"Error when consuming topic \"{topic}\".");
                    }
                } while (!_cancellationTokenSource.IsCancellationRequested);
            }, stoppingToken);
        }

        private async Task ConsumeAsync(string topic, string redisSumKey, Func<ConsumeResult<Ignore, string>, string, CancellationToken, Task> consumeTask, bool commitOnError, CancellationToken cancellationToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<Ignore, string> result = null;

                        try
                        {
                            result = consumer.Consume(cancellationToken);
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogInformation(e, e.Message);
                        }

                        if (result == null)
                        {
                            continue;
                        }

                        consumer.Commit(result);

                        try
                        {
                            _logger.LogInformation(JsonConvert.SerializeObject(result));
                            //await consumeTask(result, redisSumKey, cancellationToken);
                        }
                        catch (OperationCanceledException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            if (!commitOnError)
                            {
                                throw e;
                            }
                        }

                    }
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogWarning(e, $"Stopped consuming topic \"{topic}\".");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}