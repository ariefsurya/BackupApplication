using Microsoft.AspNetCore.Connections;
using Model;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace RabbitMqProductApi.RabbitMQ
{
    public interface IRabitMQProducer
    {
        public Task SendBackupMessage<T>(T message);
    }
    public class RabitMQProducer : IRabitMQProducer
    {
        public async Task SendBackupMessage<T>(T data)
        {
            //Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it

            var factory = new ConnectionFactory
            {
                //UserName = "guest",
                //Password = "guest",
                //Port = 5672,
                HostName = "rabbitmqbackup", // rabbitmqbackup kalau mau pakai yang di docker rabbitmq:3.13.3-management-alpine
                //VirtualHost = "/"
            };

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    TargetBackup task = data as TargetBackup;
                    var exchangeName = "backup_exchange.company-" + task.CompanyId;
                    var queueName = "backupQueue.company-" + task.CompanyId;
                    var routingKey = "backup.database.company-" + task.CompanyId;
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true, autoDelete: false);
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

                    string message = JsonSerializer.Serialize(data);
                    var body = Encoding.UTF8.GetBytes(message);

                    // Publish to the queue
                    channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                    Console.WriteLine($"[x] Sent {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error producing message to RabbitMQ: {ex.Message}");
            }
        }


        #region test
        public async Task SendBackupMessage2<T>(T message)
        {
            try
            {
                var RoutingKey = "aa";
                var Body = "Message";
                var Factory = new ConnectionFactory
                {
                    UserName = "guest",
                    Password = "guest",
                    Port = 5672,
                    HostName = "localhost", //localhost kalau rabbitmq nya di jadiin services windows
                    VirtualHost = "/"
                };

                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("messageexchange", ExchangeType.Direct);
                    channel.QueueDeclare(RoutingKey, true, false, false, null);
                    channel.QueueBind(RoutingKey, "messageexchange", RoutingKey, null);
                    channel.BasicPublish("messageexchange", RoutingKey, null, Encoding.UTF8.GetBytes(Body));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error producing message to RabbitMQ: {ex.Message}");
            }
        }
        #endregion
    }
}
