using rabbitMq.Common;
using RabbitMQ.Client;
using System.Text;

namespace rabbitMq.Provider.PublishSubscribe
{
    public class SendPublishSubscribe
    {
        public static void SendMessage()
        {
            //创建连接
            using (var connection = RabbitMqClient.GetConnection())
            {
                //创建信道
                using (var channel = connection.CreateModel())
                {
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

                    int i = 0;
                    while (i < 10)
                    {
                        string content = $"这是第{i}条数据";
                        byte[] body = Encoding.UTF8.GetBytes(content);

                        //给PublishSubscribe交换机下的所有队列都发送消息
                        channel.BasicPublish(exchange: "PublishSubscribe", routingKey: "", null, body);

                        Console.WriteLine($"第{i}条数据发送完毕");
                        i++;
                    }
                }
            }
        }
    }
}
