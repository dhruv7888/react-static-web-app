using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace api
{
    public static class CodeFlow
    {
        [FunctionName("CodeFlow")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["name"];
            //Console.WriteLine(name);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data1 = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data1?.name;
            string content = "[{WrongContent : NotDone}]";
            if (!string.IsNullOrEmpty(name))
            {
                JObject data = JObject.Parse(name);
                string sep = "~";
                string FileName= data["Namespace"].ToString()+sep+ data["EventName"].ToString() +sep+ data["ServiceName"].ToString() + sep + data["APIName"].ToString()+".json"; 
                string containerName = "2022-06-27-20-00";
                string accountName = "dhruv7888";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";

                Azure.Storage.StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, storageAccountKey);
                string blobUri = "https://" + accountName + ".blob.core.windows.net";
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient Blob = container.GetBlobClient(FileName);
                MemoryStream Memstrm = new MemoryStream();
                Blob.DownloadTo(Memstrm);
                Memstrm.Position = 0;
                content = new StreamReader(Memstrm).ReadToEnd();
                //Console.WriteLine(content);
            }
            return new OkObjectResult(content);
        }
    }
}


