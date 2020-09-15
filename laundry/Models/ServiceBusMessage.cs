using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace laundry.Models
{
    public class ServiceBusMessage
    {

        public Guid Id { get; }
        public string Title { get; set; }
        public string Message { get; set; }

        public ServiceBusMessage()
        {
            Id = Guid.NewGuid();
        }
    }
}
