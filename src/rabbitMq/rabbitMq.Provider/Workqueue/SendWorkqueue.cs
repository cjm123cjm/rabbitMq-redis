using rabbitMq.Common;
using RabbitMQ.Client;
using System.Text;

namespace rabbitMq.Provider.Workqueue
{
    public class SendWorkqueue
    {
        public static void SendMessage()
        {
            //创建连接
            using (var connection = RabbitMqClient.GetConnection())
            {
                //创建信道
                using (var channel = connection.CreateModel())
                {
                    //定义队列
                    channel.QueueDeclare("Workqueue", false, false, false, null);

                    int i = 0;
                    while (i < 500)
                    {
                        string content = $"这是第{i}条数据";
                        byte[] body = Encoding.UTF8.GetBytes(content);

                        //发送消息，exchange为空，会使用默认的交换机((AMQP default))
                        channel.BasicPublish(exchange: "", routingKey: "Workqueue", null, body);

                        Console.WriteLine($"第{i}条数据发送完毕");
                        i++;
                    }
                }
            }
        }
    }
}
