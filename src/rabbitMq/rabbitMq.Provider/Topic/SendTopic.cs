﻿using rabbitMq.Common;
using RabbitMQ.Client;
using System.Text;

namespace rabbitMq.Provider.Topic
{
    public class SendTopic
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
                    channel.ExchangeDeclare("topicExchange", "topic");

                    //定义队列
                    channel.QueueDeclare("topicqueue1", false, false, false, null);
                    channel.QueueDeclare("topicqueue2", false, false, false, null);

                    //将队列绑定到交换机上
                    channel.QueueBind("topicqueue1", "topicExchange", "Routingkey.*");
                    channel.QueueBind("topicqueue2", "topicExchange", "Routingkey.#");

                    int i = 0;
                    while (i < 10)
                    {
                        string content = $"这是第{i}条数据";
                        byte[] body = Encoding.UTF8.GetBytes(content);

                        //发送消息，向路由key为Routingkey3的队列发消息
                        channel.BasicPublish(exchange: "topicExchange", routingKey: "Routingkey.abc", null, body);

                        Console.WriteLine($"第{i}条数据发送完毕");
                        i++;
                    }
                }
            }
        }
    }
}
