using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQProducer
{
    public class RabbitMqDirectProducer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;

        public RabbitMqDirectProducer()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _replyQueueName = _channel.QueueDeclare("resp_Queue", true, false, false, null);           

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceived;
            _channel.BasicConsume(_replyQueueName, true, consumer);
        }
      
        public void SendMessage(string message)
        {
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            _channel.BasicPublish("", "req_Queue", props, Encoding.ASCII.GetBytes(message));
        }
      
        private  void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"Consumer: {Encoding.UTF8.GetString(e.Body)}");
            Console.Write("Producer:");
            var input = Console.ReadLine();
            SendMessage(input);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
