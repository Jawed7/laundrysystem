using System;
using System.Threading.Tasks;
using laundry.Models;
using laundry.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace laundry.Controllers
{
    public class ServiceBusMessageController : Controller
    {
        private readonly ILogger<ServiceBusMessageController> _logger;
        private readonly IServiceBusQueueService _queueService;

        const string ServiceBusConnectionString = "Endpoint=sb://customerrequest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZC+QQdCJZtptXmOJ/xWLb4ZXAtCq17+bSOvTIJOERbE=";
        const string QueueName = "requestqueue";
        static IQueueClient queueClient;
        static List<string> items;

        public ServiceBusMessageController(ILogger<ServiceBusMessageController> logger,
            IServiceBusQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var message = new ServiceBusMessage
                {
                    Title = collection["Title"],
                    Message = collection["Message"]
                };

                _logger.LogInformation($"Message created: {message.Id}.");

                await _queueService.SendMessageAsync(message);

                _logger.LogInformation($"Message {message.Id} has been sent.");

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured: {ex.Message}");
                return View();
            }
        }

        private static async Task ExecuteMessageProcessing(Message message, CancellationToken arg2)
        {

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            items.Add("Body: " + Encoding.UTF8.GetString(message.Body));

        }

        public async Task<ActionResult> ProcessMsg()
        {
            //queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            items = new List<string>();
            await Task.Factory.StartNew(() =>
            {
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
                var options = new MessageHandlerOptions(ExceptionMethod)
                {
                    MaxConcurrentCalls = 5,
                    AutoComplete = true
                };
                queueClient.RegisterMessageHandler(ExecuteMessageProcessing, options);
            });

            return RedirectToAction("ProcessMsgResult");
        }

        

        //Part 2: Received Message from the Service Bus
        private static async Task ExceptionMethod(ExceptionReceivedEventArgs arg)
        {
            await Task.Run(() =>
           Console.WriteLine($"Error occured. Error is {arg.Exception.Message}")
           );
        }

        public IActionResult ProcessMsgResult()
        {
            
            return View(items);
        }
    }
}
