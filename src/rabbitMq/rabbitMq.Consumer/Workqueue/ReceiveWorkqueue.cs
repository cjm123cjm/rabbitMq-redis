using RabbitMQ.Client.Events;
using rabbitMq.Common;
using System.Text;
using RabbitMQ.Client;

namespace rabbitMq.Consumer.Workqueue
{
    public class ReceiveWorkqueue
    {
        public static void ReceiveMessage()
        {
            var connection = RabbitMqClient.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("Workqueue", false, false, false, null);

            //prefetchSize:消息本身大小 如果设置为0，那么对消息本身大小不限制
            //prefetchCount:告诉rabbitMq不要一次性给消费者推送大于N个消息
            //global:是否将上面的设置应用于整个管道，false：只应用于当前消费者 true：当前通道所有消费者都应用这个限流策略
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(100);

                Console.WriteLine("已经处理消息：" + message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "Workqueue", autoAck: false, consumer: consumer); //autoAck:false 自动亲手
        }
    }
}
