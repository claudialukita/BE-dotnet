//using Confluent.Kafka;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace BLL.Messaging
//{
//    class KafkaProducer<TKey, TValue> : IDisposable, IKafkaProducer<TKey, TValue> where TValue : class
//    {
//        //private readonly IProducer<TKey, TValue> _producer;

//        private readonly IConfiguration _configuration;
//        private readonly ProducerConfig _producerConfig;
//        private readonly int _timeoutMs = 30000;
//        private readonly ILogger _logger;
//        private readonly IProducer<string, string> _producer;

//        /// <summary>  
//        /// Initializes the producer  
//        /// </summary>  
//        /// <param name="config"></param>  
//        public KafkaProducer(IConfiguration configuration, ILogger<KafkaSender> logger)
//        {
//            //_producer = new ProducerBuilder<TKey, TValue>(config).SetValueSerializer(new KafkaSerializer<TValue>()).Build();

//            _configuration = configuration;
//            _logger = logger;

//            var bootstrapServers = _configuration.GetValue<string>("Kafka");

//            _producerConfig = new ProducerConfig
//            {
//                Acks = Acks.All,
//                EnableIdempotence = true,
//                BootstrapServers = bootstrapServers,
//                MessageTimeoutMs = _timeoutMs,
//                Partitioner = Partitioner.ConsistentRandom
//            };

//            _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
//        }

//        /// <summary>  
//        /// Triggered when the service creates Kafka topic.  
//        /// </summary>  
//        /// <param name="topic">Indicates topic name</param>  
//        /// <param name="key">Indicates message's key in Kafka topic</param>  
//        /// <param name="value">Indicates message's value in Kafka topic</param>  
//        /// <returns></returns>  
//        public async Task ProduceAsync(string topic, TKey key, TValue value)
//        {
//            await _producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value });
//        }

//        /// <summary>  
//        ///   
//        /// </summary>  
//        public void Dispose()
//        {
//            _producer.Flush();
//            _producer.Dispose();
//        }
//    }
//}
