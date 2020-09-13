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
            //link to appsettings.json file and read the access key
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //create link to connect to your storage account
            CloudStorageAccount storageaccount =
                CloudStorageAccount.Parse(configure["ConnectionStrings:blobstorageconnection"]);

            //create agent object client to communicate between ur app and ur storage account
            CloudBlobClient tableClient = storageaccount.CreateCloudBlobClient();

            //refer to table that related to your app
            CloudBlobContainer containerRef = tableClient.GetContainerReference("blobstorage");

            return containerRef;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        //OPTION A: Disables Asp.Net Core's default upload size limit
        [DisableRequestSizeLimit]
        //OPTION B: Uncomment to set a specified upload file limit
        //[RequestSizeLimit(40000000)] 

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

                // NOTE: uncomment either OPTION A or OPTION B to use one approach over another

                // OPTION A: convert to byte array before upload
                //using (var ms = new MemoryStream())
                //{
                //    formFile.CopyTo(ms);
                //    var fileBytes = ms.ToArray();
                //    uploadSuccess = await UploadToBlob(formFile.FileName, fileBytes, null);

                //}

                // OPTION B: read directly from stream for blob upload      
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
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = _configuration["storageconnectionstring"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("blobstorage");


                    CloudBlobContainer tableObject = cloudBlobContainer;
                    bool var = tableObject.CreateIfNotExistsAsync().Result;

                    //await  cloudBlobContainer.CreateAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
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
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
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

            using (var output = System.IO.File.OpenWrite(@"D:\Folder"))
            {
                blob.DownloadToStreamAsync(output).Wait();
            }

        }
    }
}
