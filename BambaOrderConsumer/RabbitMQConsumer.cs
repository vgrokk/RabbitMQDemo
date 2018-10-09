using System;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace BambaOrderConsumer
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private const string ExchangeName = "Topic_Exchange";
        private const string BambaQueueName = "OrderBambaTopic_Queue";
        private const string BambaTopicName = "order.bamba";
        private int _totalCount;
        private int _totalAmount;

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
                    Console.WriteLine($"Listening for Topic <{BambaTopicName}>");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(BambaQueueName,
                        true, false, false, null);

                    channel.QueueBind(BambaQueueName, ExchangeName,BambaTopicName);

                    var subscription = new Subscription(channel,BambaQueueName, false);

                    while (true)
                    {
                        var deliveryArguments = subscription.Next();

                        var order = deliveryArguments.Body.Deserialize<Order>();

                        _totalCount++;
                        _totalAmount += order.Amount;
                        Console.WriteLine($"We sold {_totalCount} {order.ProductName}! Total Amount:{_totalAmount}");
                        subscription.Ack(deliveryArguments);
                    }
                  
                }
            }
        }
    }
}

