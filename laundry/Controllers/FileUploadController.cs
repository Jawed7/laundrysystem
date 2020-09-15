using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using laundry.Models;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Storage.Table;

namespace laundry.Controllers
{
    public class FileUploadController : Controller
    {
        private IConfiguration _configuration;

        public FileUploadController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        private CloudBlobContainer getContainerRef()
        {
          
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            CloudStorageAccount storageaccount =
                CloudStorageAccount.Parse(configure["ConnectionStrings:blobstorageconnection"]);
           
            CloudBlobClient tableClient = storageaccount.CreateCloudBlobClient();

            CloudBlobContainer containerRef = tableClient.GetContainerReference("blobstorage");

            return containerRef;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        [DisableRequestSizeLimit]
        

        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var uploadSuccess = false;
            string uploadedUri = null;

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }
     
                using (var stream = formFile.OpenReadStream())
                {
                    (uploadSuccess, uploadedUri) = await UploadToBlob(formFile.FileName, null, stream);
                    TempData["uploadedUri"] = uploadedUri;
                }

            }

            if (uploadSuccess)
                return View("UploadSuccess");
            else
                return View("UploadError");
        }

        private async Task<(bool, string)> UploadToBlob(string filename, byte[] imageBuffer = null, Stream stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = getContainerRef();
            string storageConnectionString = _configuration["storageconnectionstring"];

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer tableObject = cloudBlobContainer;
                    bool var = tableObject.CreateIfNotExistsAsync().Result;

                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return (false, null);
                    }

                    return (true, cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
                }
                catch (StorageException ex)
                {
                    return (false, null);
                }
            }
            else
            {
                return (false, null);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult ManageUpload()
        {
            CloudBlobContainer container = getContainerRef();
            List<string> blobs = new List<string>();
            BlobResultSegment result = container.ListBlobsSegmentedAsync(null).Result;

            foreach(IListBlobItem item in result.Results)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    blobs.Add(blob.Name + "#" + blob.Uri.ToString());
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory blob = (CloudBlobDirectory)item;
                    blobs.Add(blob.Uri.ToString());
                }
            }
            return View(blobs);
        }

        public ActionResult DeleteBlobs(string area)
        {
            CloudBlobContainer container = getContainerRef();
            CloudBlockBlob blob = container.GetBlockBlobReference(area);

            string name = blob.Name;
            var result = blob.DeleteIfExistsAsync().Result;

            return RedirectToAction(nameof(ManageUpload));
        }

        public void DownloadBlobs(string area)
        {
            CloudBlobContainer container = getContainerRef();
            CloudBlockBlob blob = container.GetBlockBlobReference(area);

            using (var output = System.IO.File.OpenWrite(@"E:\\download images"))
            {
                blob.DownloadToStreamAsync(output).Wait();
            }

        }
    }
}
