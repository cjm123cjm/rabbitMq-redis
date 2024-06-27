using rabbitMq.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbitMq.Consumer.Routing
{
    public class ReceiveRouting
    {
        public static void ReceiveMessage()
        {
            //创建连接
            var connection = RabbitMqClient.GetConnection();

            //创建信道
            var channel = connection.CreateModel();

            //定义交换机
            channel.ExchangeDeclare("Routing", "direct");

            //定义队列
            channel.QueueDeclare("Routingqueue1", false, false, false, null);
            channel.QueueDeclare("Routingqueue2", false, false, false, null);
            channel.QueueDeclare("Routingqueue3", false, false, false, null);

            //将队列绑定到交换机上
            channel.QueueBind("Routingqueue1", "Routing", "Routingkey1");
            channel.QueueBind("Routingqueue2", "Routing", "Routingkey2");
            channel.QueueBind("Routingqueue3", "Routing", "Routingkey3");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string content = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("消费Routingkey3消息：" + content);

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "Routingqueue3", false, consumer);


        }
    }
}
