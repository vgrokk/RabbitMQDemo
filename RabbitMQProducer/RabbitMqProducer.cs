﻿using System;
using Core;
using RabbitMQ.Client;

namespace RabbitMQProducer
{
    public class RabbitMqProducer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private const string ExchangeName = "Topic_Exchange";
        private const string BambaTopicName = "order.bamba";
        private const string BissliTopicName = "order.bissli";
        private readonly Random _random = new Random();

        public RabbitMqProducer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeName, "topic");

         
        }

        public void PlaceOrder()
        {
            var number = _random.Next(1, 100);
            if (number % 2 == 0)
                SendMessage(new Order { Amount = number, ProductName = "Bamba" }, BambaTopicName);
            else
                SendMessage(new Order { Amount = number, ProductName = "Bissli" }, BissliTopicName);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _model?.Dispose();
        }

        private void SendMessage(Order order, string topicName)
        {
           _model.BasicPublish(ExchangeName, topicName, null, order.Serialize());
            Console.WriteLine($"Order placed. {order.ProductName}:{order.Amount} ");
        }
    }
}
