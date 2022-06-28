using Azure.Data.Tables;
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
    public static class TableData
    {
        [FunctionName("TableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("TableData HTTP trigger function started processing.");

            string details = req.Query["details"];
            Console.WriteLine(details);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic dynamicBody = JsonConvert.DeserializeObject(requestBody);
            details = details ?? dynamicBody?.details;
            if (!string.IsNullOrEmpty(details))
            {
                JObject data = JObject.Parse(details);

                log.LogInformation("Started Acessing Table");
                string accountName = "dhruv7888";
                var storageUri = "https://dhruv7888.table.core.windows.net/";
                string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";
                string tableName = "UserInputs";
                var serviceClient = new TableServiceClient(new Uri(storageUri), new TableSharedKeyCredential(accountName, storageAccountKey));
                var tableClient = new TableClient(new Uri(storageUri), tableName, new TableSharedKeyCredential(accountName, storageAccountKey));
                log.LogInformation("Acessing Table Complete");

                string partitionKey = GetRandomString();
                string rowKey = GetRandomString();
                TableEntity entity = GetEntity(data, partitionKey, rowKey);
                tableClient.AddEntity(entity);
                log.LogInformation("Data Entered to Table");

            }
            string responseMessage = string.IsNullOrEmpty(details)
                ? "{emptyDetails : notDone}}" : "{correctDetailsFormat : Done}";
            return new OkObjectResult(responseMessage);
        }

        private static TableEntity GetEntity(JObject data, string partitionKey, string rowKey)
        {
            return new TableEntity(partitionKey, rowKey) {
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
        }

        private static string GetRandomString()
        {
            string randomString = "";
            Random rnd = new Random();
            char letter;
            for (int i = 0; i < 20; i++)
            {
                double randomNumber = rnd.Next(25);
                letter = (char)('A' + randomNumber);
                randomString += letter;
            }
            return randomString;
        }
    }
}



