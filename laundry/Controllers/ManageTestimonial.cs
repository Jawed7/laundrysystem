using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using laundry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace laundry.Controllers
{
    public class ManageTestimonial : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Get(int id=0)
        {
            //link to appsettings.json file and read the access key
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //create link to connect to your storage account
            CloudStorageAccount storageaccount =
                CloudStorageAccount.Parse(configure["ConnectionStrings:tablestorageconnection"]);

            //create agent object client to communicate between ur app and ur storage account
            CloudTableClient tableClient = storageaccount.CreateCloudTableClient();

            //refer to table that related to your app
            CloudTable tables = tableClient.GetTableReference("TestTable");

            TableQuery<User> query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UserTestinomials"));

            List<User> users = new List<User>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<User> segment = tables.ExecuteQuerySegmentedAsync(query, token).Result;
                token = segment.ContinuationToken;

                foreach (User user in segment.Results)
                {
                    users.Add(user);
                }
            }
            while (token != null);
            return View(users);
        }


        public ActionResult deleteTableData(string partitionkey, string rowkey)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //create link to connect to your storage account
            CloudStorageAccount storageaccount =
                CloudStorageAccount.Parse(configure["ConnectionStrings:tablestorageconnection"]);

            //create agent object client to communicate between ur app and ur storage account
            CloudTableClient tableClient = storageaccount.CreateCloudTableClient();

            //refer to table that related to your app
            CloudTable tables = tableClient.GetTableReference("TestTable");

            TableOperation delOperation = TableOperation.Delete(new User(partitionkey, rowkey) { ETag = "*" });

            TableResult result = tables.ExecuteAsync(delOperation).Result;

            return RedirectToAction("Get", "ManageTestimonial"); 
        }
    }
}
