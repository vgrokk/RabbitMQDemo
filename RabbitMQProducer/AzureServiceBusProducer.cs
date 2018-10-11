using System;
using System.Threading.Tasks;
using Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace RabbitMQProducer
{
    public class AzureServiceBusProducer
    {
        private const string BambaTopicName = "order.bamba";
        private readonly Random _random = new Random();
        private readonly TopicClient _bambaTopicClient;
     
        public AzureServiceBusProducer(string connectionString)
        {
            _bambaTopicClient =  new TopicClient(connectionString, BambaTopicName);
        }

        public void PlaceOrder()
        {
            var number = _random.Next(1, 100);
            SendMessage(new Order {Amount = number, ProductName = "Bamba"}, _bambaTopicClient).Wait();
        }

        private async Task SendMessage(Order order, ISenderClient topicClient)
        {
            var message = new Message(order.Serialize());
           
            await topicClient.SendAsync(message);
           
            Console.WriteLine($"Order placed. {order.ProductName}:{order.Amount} ");
        }
    }
}
