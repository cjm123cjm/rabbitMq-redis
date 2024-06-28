using rabbitMq.Common;
using RabbitMQ.Client;
using System.Text;

namespace rabbitMq.Provider.Dead
{
    public class SendDead
    {
        public static void SendMessage()
        {
            var connection = RabbitMqClient.GetConnection();
            var channel = connection.CreateModel();

            //正常交换机、正常队列、正常路由
            string normalExchange = "normalExchange";
            string normalQueue = "normalQueue";
            string normalKey = "normalKey";
            //死信交换机、死信队列、死信路由
            string deadExchange = "deadExchange";
            string deadQueue = "deadQueue";
            string deadKey = "deadKey";

            //声明正常
            channel.ExchangeDeclare(normalExchange, type: "direct", durable: true);
            Dictionary<string, object> arg = new Dictionary<string, object>();
            arg.Add("x-dead-letter-exchange", deadExchange);//死信交换机
            arg.Add("x-dead-letter-routing-key", deadKey);//死信路由
            arg.Add("x-max-length", 10);//最大容量
            channel.QueueDeclare(normalQueue, durable: true, false, false, arg);
            channel.QueueBind(normalQueue, normalExchange, normalKey);

            //声明死信
            channel.ExchangeDeclare(deadExchange, type: "direct", durable: true);
            channel.QueueDeclare(deadQueue, durable: true, false, false, null);
            channel.QueueBind(deadQueue, deadExchange, deadKey);

            //向正常队列发送消息
            var prop = channel.CreateBasicProperties();
            prop.Expiration = "10000"; //过期事件 毫秒 10s过期
            int i = 0;
            while (i < 15)
            {
                string content = $"死信消息，第{i}条消息";
                byte[] body = Encoding.UTF8.GetBytes(content);

                channel.BasicPublish(normalExchange, normalKey, prop, body);

                i++;
            }
        }
    }
}
