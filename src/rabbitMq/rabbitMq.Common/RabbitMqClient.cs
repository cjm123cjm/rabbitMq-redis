using RabbitMQ.Client;

namespace rabbitMq.Common
{
    public class RabbitMqClient
    {
        public static IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.159.129",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            return factory.CreateConnection();
        }
    }
}