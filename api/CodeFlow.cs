using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using Azure.Storage;

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
            Console.WriteLine(name);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data1 = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data1?.name;
            if (!string.IsNullOrEmpty(name))
            {
                JObject data = JObject.Parse(name);
                string sep = "_______";
                string FileName=data["Namespace"].ToString()+sep+data["EventName"].ToString()+sep+data["ServiceName"].ToString()+ sep + data["APIName"].ToString();
                string containerName = "2022-06-27-05-52";
                string accountName = "dhruv7888";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";

                Azure.Storage.StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, storageAccountKey);
                string blobUri = "https://" + accountName + ".blob.core.windows.net";
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient Blob = container.GetBlobClient(FileName);

            }
            string responseMessage = string.IsNullOrEmpty(name)? "Not Done": "";
            return new OkObjectResult(responseMessage);
        }
    }
}


