using laundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace laundry.Service
{
    public class ServiceBusQueueService : IServiceBusQueueService
    {
        private const string _serviceBusConnectionString = "Endpoint=sb://customerrequest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZC+QQdCJZtptXmOJ/xWLb4ZXAtCq17+bSOvTIJOERbE=";
        private const string _queueName = "requestqueue";

        private readonly IQueueClient _queueClient;
        private readonly ILogger<IServiceBusQueueService> _logger;

        public ServiceBusQueueService(ILogger<IServiceBusQueueService> logger)
        {
            _queueClient = new QueueClient(_serviceBusConnectionString, _queueName);
            _logger = logger;
        }

        public async Task SendMessageAsync(ServiceBusMessage serviceBusMessage)
        {
            try
            {
                // Serialize message and send to the queue.
                string messageBody = JsonSerializer.Serialize(serviceBusMessage);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                // Log the body of the message to the console.
                _logger.LogInformation($"Sending message: {messageBody}");

                // Send the message to the queue.
                await _queueClient.SendAsync(message);

            }
            catch (Exception exception)
            {
                // Log errors
                _logger.LogError($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }


    }
}
