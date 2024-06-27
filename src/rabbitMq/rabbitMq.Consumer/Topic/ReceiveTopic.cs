using RabbitMQ.Client.Events;
using rabbitMq.Common;
using System.Text;
using RabbitMQ.Client;

namespace rabbitMq.Consumer.Topic
{
    public class ReceiveTopic
    {
        public static void ReceiveMessage()
        {
            //创建连接
            var connection = RabbitMqClient.GetConnection();

            //创建信道
            var channel = connection.CreateModel();

            //定义交换机
            channel.ExchangeDeclare("topicExchange", "topic");

            //定义队列
            channel.QueueDeclare("topicqueue1", false, false, false, null);
            channel.QueueDeclare("topicqueue2", false, false, false, null);

            //将队列绑定到交换机上
            channel.QueueBind("topicqueue1", "topicExchange", "Routingkey.*");
            channel.QueueBind("topicqueue2", "topicExchange", "Routingkey.#");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, e) =>
            {
                string content = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("消费topicqueue1队列消息：" + content);

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "topicqueue1", false, consumer);
        }

    }
}
