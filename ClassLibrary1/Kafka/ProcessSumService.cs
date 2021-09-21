using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Kafka
{
    public class ProcessSumService
    {
        internal Task ConsumeAsync(ConsumeResult<Ignore, string> arg1, string arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

    }
}