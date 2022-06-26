using System;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using Azure;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
using Microsoft.Azure.Monitoring.DGrep.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Azure.Data.Tables;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Azure.Data.Tables.Models;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs.Host;

namespace api
{
    public static class hello
    {
        [FunctionName("hello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            Console.WriteLine(name);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data1 = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data1?.name;
            if (!string.IsNullOrEmpty(name))
            {
                JObject data = JObject.Parse(name);
                //  Console.WriteLine("Started Acessing Table");
                string accountName = "dhruv7888";
                var storageUri = "https://dhruv7888.table.core.windows.net/";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";
                string tableName = "UserInputs";
                var serviceClient = new TableServiceClient(new Uri(storageUri), new TableSharedKeyCredential(accountName, storageAccountKey));
                var tableClient = new TableClient(new Uri(storageUri), tableName, new TableSharedKeyCredential(accountName, storageAccountKey));
                //Console.WriteLine("Acessing Table Complete");

                String Rowkey = "";
                String Partitionkey = "";
                char letter;
                Random rnd = new Random();
                for (int i = 0; i < 20; i++)
                {
                    double shift = rnd.Next(25);
                    letter = (char)('A' + shift);
                    Rowkey += letter;
                }
                for (int i = 0; i < 20; i++)
                {
                    double shift = rnd.Next(25);
                    letter = (char)('A' + shift);
                    Partitionkey += letter;
                }
                //log.LogInformation("PartitionKeyCreated");
                var entity = new TableEntity(Partitionkey, Rowkey) {
                    { "DGrepEndpoint",data["DGrepEndpoint"].ToString()},
                    { "MdsEndpoint",data["MdsEndpoint"].ToString()},
                    { "Namespace",data["Namespace"].ToString()},
                    { "EventName",data["EventName"].ToString()},
                    { "ServiceName",data["ServiceName"].ToString()},
                    { "APIName",data["APIName"].ToString()},
                    { "ExternalServiceName",data["ExternalServiceName"].ToString()},
                    { "ExternalAPIName",data["ExternalAPIName"].ToString()},
                    { "ExternalCallType",data["ExternalCallType"].ToString()},
                    { "CertificateName",data["CertificateName"].ToString()}
                };
                tableClient.AddEntity(entity);
            }
            string responseMessage = string.IsNullOrEmpty(name)
                ? "{Not Done}": "{Done}";
            return new OkObjectResult(responseMessage);
        }
    }
}



