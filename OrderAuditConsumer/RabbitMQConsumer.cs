using System;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace OrderAuditConsumer
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private const string ExchangeName = "Topic_Exchange";
        private const string AllQueueName = "AllOrdersTopic_Queue";
        private const string OrderTopicName = "order.*";
        
        public RabbitMQConsumer()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void ProcessMessages()
        {
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    Console.WriteLine($"Listening for Topic <{OrderTopicName}>");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(AllQueueName,
                        true, false, false, null);

                    channel.QueueBind(AllQueueName, ExchangeName,OrderTopicName);

                    var subscription = new Subscription(channel,AllQueueName, false);

                    while (true)
                    {
                        var deliveryArguments = subscription.Next();

                        var order = deliveryArguments.Body.Deserialize<Order>();
    
                        Console.WriteLine($"Order placed. {order.ProductName}:{order.Amount} ");

                        subscription.Ack(deliveryArguments);
                    }
                }
            }
        }
    }
}

