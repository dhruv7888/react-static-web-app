using Azure.Storage;
using Azure.Storage.Blobs;
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
    public static class DesignFlow
    {
        [FunctionName("DesignFlow")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DesignFlow HttpTrigger Function started execution");
            string details = req.Query["details"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic dynamicBody = JsonConvert.DeserializeObject(requestBody);
            details = details ?? dynamicBody?.details;
            string content = "[{emptyDetails : notDone}]";

            if (!string.IsNullOrEmpty(details))
            {
                JObject data = JObject.Parse(details);
                string separator = "~";
                string FileName = data["Namespace"].ToString() + separator + data["EventName"].ToString() + separator + data["ServiceName"].ToString() + separator + data["APIName"].ToString() + ".json";
                string containerName = "2022-06-28-17-23";
                string accountName = "dhruv7888";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";

                log.LogInformation("Accessing Blob Container and File");
                Azure.Storage.StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, storageAccountKey);
                string blobUri = "https://" + accountName + ".blob.core.windows.net";
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blob = container.GetBlobClient(FileName);
                log.LogInformation("Blob Container and File Accessed");

                MemoryStream memoryStrm = new MemoryStream();
                blob.DownloadTo(memoryStrm);
                memoryStrm.Position = 0;
                content = new StreamReader(memoryStrm).ReadToEnd();
            }
            else
                log.LogInformation("Details are Empty");
            log.LogInformation("DesignFlow HttpTrigger Function execution completed");
            return new OkObjectResult(content);
        }
    }
}


