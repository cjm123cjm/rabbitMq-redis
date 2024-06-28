using rabbitMq.Common;
using RabbitMQ.Client;
using System.Text;

namespace rabbitMq.Provider.Delay
{
    public class SendDelay
    {
        public static void SendMessage()
        {
            var connection = RabbitMqClient.GetConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "delayExchange",
                type: "x-delayed-message",
                durable: true,
                autoDelete: false, 
                arguments: new Dictionary<string, object>
                {
                    {"x-delayed","direct" }
                });

            channel.QueueDeclare("delayQueue", true, false, false, null);

            channel.QueueBind("delayQueue", "delayExchange", "delayKey");

            var prop = channel.CreateBasicProperties();
            prop.Headers = new Dictionary<string, object>
            {
                {"x-delay","15000" } //延时15s
            };
            string content = "这是一个延时30s的消息";
            byte[] body = Encoding.UTF8.GetBytes(content);

            channel.BasicPublish("delayExchange", "delayKey", prop, body);
        }
    }
}
