using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using laundry.Models;
using Microsoft.AspNetCore.Identity;
using laundry.Areas.Identity.Data;

namespace laundry.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly UserManager<laundryUser> _userManager;
        
        public TableStorageController(UserManager<laundryUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
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
            CloudTable tables = tableClient.GetTableReference("TestTable");

            return tables;
        }

        //create table using the code if not exists just create, if exist will not create one more time
        public ActionResult CreateTable()
        {
            CloudTable tableObject = getTableInformation();
            //check whether exist or not if not then create
            ViewBag.result = tableObject.CreateIfNotExistsAsync().Result;
            ViewBag.tablename = tableObject.Name;
            return View();
        }



        
        public ActionResult addTestimonials(string msgInput)
        {
      
            var partKey = "UserTestinomials";
            var userId = _userManager.GetUserAsync(User).Result.Id;
            CloudTable tableclient = getTableInformation();
            User user = new User(partKey, userId);
            user.Testimonial = msgInput;

            try
            {
                TableOperation addTestomonial = TableOperation.Insert(user);
                TableResult result = tableclient.ExecuteAsync(addTestomonial).Result;
                if (result.HttpStatusCode == 204)
                {
                    ViewBag.tablename = tableclient.Name;
                    ViewBag.msg = "Success!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.tablename = tableclient.Name;
                ViewBag.msg = ex.ToString();
            }
            return View();
        }

        public ActionResult getDataTableStorage()
        {
            return View();
        }
        
    }
}
