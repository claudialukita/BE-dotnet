using System.Threading.Tasks;

namespace BLL.Kafka
{
    public interface IKafkaSender
    {
        Task SendAsync(string topic, object message);
    }
}