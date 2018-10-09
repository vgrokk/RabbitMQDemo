using System;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace BissliConsumer
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private const string ExchangeName = "Topic_Exchange";
        private const string BissliQueueName = "OrderBissliTopic_Queue";
        private const string BissliTopicName = "order.bissli";
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
                    Console.WriteLine($"Listening for Topic <{BissliTopicName}>");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(BissliQueueName,
                        true, false, false, null);

                    channel.QueueBind(BissliQueueName, ExchangeName,BissliTopicName);

                    var subscription = new Subscription(channel,BissliQueueName, false);

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

