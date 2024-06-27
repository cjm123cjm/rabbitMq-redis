using RabbitMQ.Client.Events;
using rabbitMq.Common;
using System.Text;
using RabbitMQ.Client;

namespace rabbitMq.Consumer.PublishSubscribe
{
    public class ReceivePublishSubscribe
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMqClient.GetConnection();
            var channel = connection.CreateModel();
            //定义交换机
            channel.ExchangeDeclare("PublishSubscribe", "fanout");

            //定义队列
            channel.QueueDeclare("Publish1", false, false, false, null);
            channel.QueueDeclare("Publish2", false, false, false, null);
            channel.QueueDeclare("Publish3", false, false, false, null);

            //将队列绑定到交换机上
            channel.QueueBind("Publish1", "PublishSubscribe", "");
            channel.QueueBind("Publish2", "PublishSubscribe", "");
            channel.QueueBind("Publish3", "PublishSubscribe", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("已经处理消息：" + message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "Publish1", autoAck: false, consumer: consumer); //autoAck:false 自动亲手
        }
    }
}
