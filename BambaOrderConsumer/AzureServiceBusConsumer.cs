using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.Azure.ServiceBus;

namespace BambaOrderConsumer
{
    public class AzureServiceBusConsumer
    {
        private const string BambaTopicName = "order.bamba";
        const string SubscriptionName = "bambSub";
        static ISubscriptionClient _subscriptionClient;
        private int _totalCount;
        private int _totalAmount;
        public AzureServiceBusConsumer(string connectionString)
        {
            _subscriptionClient = new SubscriptionClient(connectionString, BambaTopicName, SubscriptionName);
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(OnException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            Console.WriteLine($"Listening for Topic <{BambaTopicName}>");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine();
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken arg2)
        {
            var order = message.Body.Deserialize<Order>();
            _totalCount++;
            _totalAmount += order.Amount;

            Console.WriteLine($"We sold {_totalCount} {order.ProductName}! Total Amount:{_totalAmount}");
            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task OnException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
