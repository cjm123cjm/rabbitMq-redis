using rabbitMq.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbitMq.Consumer.Helloword
{
    public class ReceiveHelloword
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMqClient.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("Hwqueue", false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("已经处理消息：" + message);

                //BasicReject 拒收消息
                //channel.BasicReject(e.DeliveryTag, requeue: false);
            };

            channel.BasicConsume(queue: "Hwqueue", autoAck: false, consumer: consumer); //autoAck:false 自动亲手
        }
    }
}
