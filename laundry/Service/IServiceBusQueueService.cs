using laundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laundry.Service
{
    public interface IServiceBusQueueService
    {
        Task SendMessageAsync(ServiceBusMessage serviceBusMessage);
    }
}
