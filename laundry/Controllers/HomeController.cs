using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using laundry.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace laundry.Controllers
{   [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("HomePage");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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


        public IActionResult HomePage(string _userId, string eemail)
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
            tables.CreateIfNotExistsAsync();

            //TableOperation retrieveOperation = TableOperation.Retrieve<User>(_userId, eemail);
            //TableResult result = await tables.ExecuteAsync(retrieveOperation);
            //User users = result.Result as User;

            string htmlstr = "";

            TableQuery<User> query = new TableQuery<User>();
            foreach (User entity in tables.ExecuteQuerySegmentedAsync(query, null).Result)
            {
                //string hi = "lolzzzzz";
                htmlstr += "<div>" +
                        "<div class=\"container\">" +
                            "<div class=\"row\">" +
                                "<div class=\"carousel-blog-box\">" +
                                    "<div class=\"col-xs-3\">" +
                                        "<a href=\"#\" class=\"img\"></a>" +
                                    "</div>" +
                                    "<div class=\"col-xs-9\">" +
                                        "<div class=\"center-y\">" +
                                            "<h4 class=\"title\"><a href=\"#\">" + entity.Testimonial + "</a></h4><br>" +
                                            "<div class=\"description\">" +
                                                
                                            "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>";

            }
            ViewBag.html = htmlstr;
            return View();
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult AddRequest()
        {
            return View();
        }

        public IActionResult ContactPage()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult ManageRequest()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Prices()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }
    }
}
