using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    public class RabbitMqDirectConsumer :IDisposable
    {
        private static IModel _channel;
        private static IConnection _connection;

        public RabbitMqDirectConsumer()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
          
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
           
            var replyQueueName = _channel.QueueDeclare("req_Queue", true, false, false, null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (o, args) => { OnReceived(args); };
            _channel.BasicConsume(replyQueueName, true, consumer);
        }
     
        private void OnReceived(BasicDeliverEventArgs args)
        {
            Console.WriteLine($"Producer: {Encoding.UTF8.GetString(args.Body)}");
            Console.Write("Consumer:");
            var input = Console.ReadLine();
            var replyProps = _channel.CreateBasicProperties();
         
            _channel.BasicPublish("", args.BasicProperties.ReplyTo, replyProps, Encoding.UTF8.GetBytes(input));
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
