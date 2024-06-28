using rabbitMq.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbitMq.Consumer.Dead
{
    public class ReceiveDead
    {
        public static void ReceiveMessage()
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string content = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine(content);

                //requeue: false 不返回原来的队列
                channel.BasicReject(e.DeliveryTag,requeue: false);
            };

            //消费正常队列消息
            channel.BasicConsume(normalQueue, false, consumer);

            //消费死信队列消息
            //channel.BasicConsume(deadQueue, false, consumer);
        }
    }
}
