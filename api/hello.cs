using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
using Microsoft.Azure.Monitoring.DGrep.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Azure.Data.Tables;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Azure.Data.Tables.Models;
using Azure;

namespace api
{
    public static class hello
    {
        [FunctionName("hello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            /*if (!string.IsNullOrEmpty(name))
            {
                string accountName = "dhruv7888";
                var storageUri = "https://dhruv7888.table.core.windows.net/";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";
                string tableName = "UserInputs";
                var serviceClient = new TableServiceClient(new Uri(storageUri),
                    new TableSharedKeyCredential(accountName, storageAccountKey));
                var tableClient = new TableClient(new Uri(storageUri), tableName,
                    new TableSharedKeyCredential(accountName, storageAccountKey));
                
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
                var entity = new TableEntity(Partitionkey, Rowkey) {
                    {"DGrepEndpoint",data["DGrepEndpoint"]},
                    {"Namespace",data["Namespace"]},
                    {"MdsEndpoint",data["MdsEndpoint"] },
                    {"TimeStampWindow",data["TimeStampWindow"] },
                    {"RepititiveTaskId",data["RepititiveTaskId"] },
                    { "EventName",data["EventName"]},
                    { "ColumnName",data["ColumnName"]},
                    { "ServiceFormat",data["ServiceFormat"]},
                    { "SystemIdColumnName",data["SystemIdColumnName"]},
                    {"Namespace",data["Namespace"]},
                    {"ExternalIdColumnName",data["ExternalIdColumnName"]},
                    {"CertificateName",data["CertificateName"]},
                };
                tableClient.AddEntity(entity);
            }*/
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
