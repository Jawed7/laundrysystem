using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using laundry.Areas.Identity.Data;
using laundry.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace laundry.Controllers
{
    public class ManageTestimonial : Controller
    {
        private readonly UserManager<laundryUser> _userManager;
        public ManageTestimonial(UserManager<laundryUser> userManager)
        {
            _userManager = userManager;
        }
        private CloudTable getTableInformation()
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
            CloudTable tables = tableClient.GetTableReference("tablestorage");

            return tables;
        }
        public IActionResult Index()
        {
            return View();
        }



        //public ActionResult search(string msg)
        //{
        //    if (msg != null)
        //        ViewBag.msg = msg;
        //    return View();
        //}

        [HttpGet]
        public ActionResult Get()
        {
            CloudTable tableclient = getTableInformation();

            //create agent object client to communicate between ur app and ur storage account
            //CloudTableClient tableClient = storageaccount.CreateCloudTableClient();

            //refer to table that related to your app
            //CloudTable tables = tableClient.GetTableReference("TestTable");

            try
            {
                TableQuery<User> query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UserTestimonials"));

                //var userId = _userManager.GetUserAsync(User).Result.Id;

                //TableOperation getdata = TableOperation.Retrieve<User>("UserTestinomials");
                //TableResult result = tableclient.ExecuteAsync(getdata).Result;
                List<User> users = new List<User>();
                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<User> segment = tableclient.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = segment.ContinuationToken;

                    foreach (User user in segment.Results)
                    {
                        users.Add(user);
                    }
                }
                while (token != null);
                return View(users);

            }
            catch(Exception ex)
            {
                //ViewBag.msg = "Error" + ex.ToString();
            }

            return View();
        }

        
        public ActionResult deleteTableData(string partitionkey, string rowkey)
        {
            CloudTable tableclient = getTableInformation();
            TableOperation delOperation = TableOperation.Delete(new User(partitionkey, rowkey) { ETag = "*" });

            TableResult result = tableclient.ExecuteAsync(delOperation).Result;

            var msg = "";
            if (result.HttpStatusCode == 204)
                msg = "Delete Successfully";
            else
                msg = "unable to delete";

            return RedirectToAction(nameof(Get));
        }
    }
}
