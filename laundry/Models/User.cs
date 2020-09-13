using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Security.Claims;

namespace laundry.Models
{
    public class User : TableEntity
    {
       
        public User(string userId, string email)
        {
            this.PartitionKey = userId;
            this.RowKey = email;
        }

        public User() { }
        public string Testimonial { get; set; }


    }
}
