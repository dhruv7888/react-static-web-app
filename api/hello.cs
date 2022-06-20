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
            Console.WriteLine(name);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data1 = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data1?.name;
            if (!string.IsNullOrEmpty(name))
            {
                JObject data = JObject.Parse(name);
                
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
                    {"DGrepEndpoint",data["DGrepEndpoint"].ToString()},
                    {"Namespace",data["Namespace"].ToString()},
                    {"MdsEndpoint",data["MdsEndpoint"].ToString()},
                    {"TimeStampWindow",data["TimeStampWindow"].ToString()},
                    {"RepititiveTaskId",data["RepititiveTaskId"].ToString()},
                    { "EventName",data["EventName"].ToString()},
                    { "ColumnName",data["ColumnName"].ToString()},
                    { "ServiceFormat",data["ServiceFormat"].ToString()},
                    { "SystemIdColumnName",data["SystemIdColumnName"].ToString()},
                    {"Namespace",data["Namespace"].ToString()},
                    {"ExternalIdColumnName",data["ExternalIdColumnName"].ToString()},
                    {"CertificateName",data["CertificateName"].ToString()},
                };
                Console.WriteLine(entity);
                tableClient.AddEntity(entity);
                try
                {
                    //Console.WriteLine(data["Certificate"].GetType());
                    //Console.WriteLine(data["CertficateRawData"]);/*
                    var p = data["CertficateRawData"].ToString();
                    byte[] arr = Encoding.UTF8.GetBytes(p);
                    var cert = new X509Certificate2(arr, "", X509KeyStorageFlags.Exportable);
                    /*
                    string vaultUrl = "https://dkg7888.vault.azure.net/";
                    var client = new CertificateClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
                    var cert = new X509Certificate2(@"C:\Users\dhruv7888\Downloads\vliprovisioningkeys-int-clientauth-geneva-keyvault-vli-commerce-microsoft-int-20220612.pfx", "", X509KeyStorageFlags.Exportable);
                    //var cert = new X509Certificate2(@"C:\Users\dhruv7888\Downloads\vliprovisioningkeys-int-clientauth-geneva-keyvault-vli-commerce-microsoft-int-20220612.pfx", "", X509KeyStorageFlags.Exportable);
                    var tempPw = "password";
                    var tmpPolicy = new CertificatePolicy(WellKnownIssuerNames.Self, cert.Subject);
                    tmpPolicy.ContentType = CertificateContentType.Pkcs12;
                    tmpPolicy.Exportable = true;
                    tmpPolicy.KeySize = cert.PrivateKey.KeySize;
                    //Certificate name should be jsut characters , Its better to generate a random name 
                    var result = client.ImportCertificate(new ImportCertificateOptions(data["CertificateName"].ToString(), cert.Export(X509ContentType.Pfx, tempPw))
                    {
                        Password = tempPw,
                        Policy = tmpPolicy
                    });*/
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function evs codexecuted successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}















