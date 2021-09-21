﻿//using Confluent.Kafka;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace BLL.Messaging
//{
//    //public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue> where TValue : class
//    public class KafkaConsumer : BackgroundService
//    {
//        private readonly ConsumerConfig _consumerConfig;
//        private readonly IConfiguration _config;

//        private IKafkaHandler<TKey, TValue> _handler;
//        private IConsumer<TKey, TValue> _consumer;
//        private string _topic;

//        private readonly IServiceScopeFactory _serviceScopeFactory;

//        /// <summary>  
//        /// Indicates constructor to initialize the serviceScopeFactory and ConsumerConfig  
//        /// </summary>  
//        /// <param name="config">Indicates the consumer configuration</param>  
//        /// <param name="serviceScopeFactory">Indicates the instance for serviceScopeFactory</param>  
//        public KafkaConsumer(IConfiguration config, IServiceScopeFactory serviceScopeFactory)
//        {
//            //_serviceScopeFactory = serviceScopeFactory;
//            //_config = config;

//            _config = config;
//            _consumerConfig = new ConsumerConfig
//            {
//                BootstrapServers = config.GetValue<string>("Kafka"),
//                //GroupId = config.GetValue<string>("Kafka:GroupId"),
//                EnableAutoCommit = false,
//                AutoOffsetReset = AutoOffsetReset.Earliest,
//                AllowAutoCreateTopics = true,
//                IsolationLevel = IsolationLevel.ReadCommitted
//            };
//            _logger = logger;
//            _processSumService = processNameService;
//        }

//        /// <summary>  
//        /// Triggered when the service is ready to consume the Kafka topic.  
//        /// </summary>  
//        /// <param name="topic">Indicates Kafka Topic</param>  
//        /// <param name="stoppingToken">Indicates stopping token</param>  
//        /// <returns></returns>  
//        public async Task Consume(string topic, CancellationToken stoppingToken)
//        {
//            using var scope = _serviceScopeFactory.CreateScope();

//            _handler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TKey, TValue>>();
//            _consumer = new ConsumerBuilder<TKey, TValue>(_config).SetValueDeserializer(new KafkaDeserializer<TValue>()).Build();
//            _topic = topic;

//            await Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
//        }

//        /// <summary>  
//        /// This will close the consumer, commit offsets and leave the group cleanly.  
//        /// </summary>  
//        public void Close()
//        {
//            _consumer.Close();
//        }

//        /// <summary>  
//        /// Releases all resources used by the current instance of the consumer  
//        /// </summary>  
//        public void Dispose()
//        {
//            _consumer.Dispose();
//        }

//        private async Task StartConsumerLoop(CancellationToken cancellationToken)
//        {
//            _consumer.Subscribe(_topic);

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                try
//                {
//                    var result = _consumer.Consume(cancellationToken);

//                    if (result != null)
//                    {
//                        await _handler.HandleAsync(result.Message.Key, result.Message.Value);
//                    }
//                }
//                catch (OperationCanceledException)
//                {
//                    break;
//                }
//                catch (ConsumeException e)
//                {
//                    // Consumer errors should generally be ignored (or logged) unless fatal.  
//                    Console.WriteLine($"Consume error: {e.Error.Reason}");

//                    if (e.Error.IsFatal)
//                    {
//                        break;
//                    }
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine($"Unexpected error: {e}");
//                    break;
//                }
//            }
//        }
//    }
//}
