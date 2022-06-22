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

    public static class CertBuilder
    {
        [FunctionName("CertBuilder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //try
            //{
            log.LogInformation("C# HTTP trigger function processed a request.");
            string content = await new StreamReader(req.Body).ReadToEndAsync();
            //log.LogInformation("content is " + content);
            String[] separator = { "," };
            String[] strlist = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            strlist[0]=strlist[0].Trim();
            X509Certificate2 cert = ImportCertFromBase64(strlist[0], "");
            // foreach(string s in strlist)
            //    Console.WriteLine(s);
            log.LogInformation("Trying to access keyvault");
            string vaultUrl = "https://dkg7888.vault.azure.net/";
            var client = new CertificateClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
            log.LogInformation("Certificate Client created");
            var tempPw = "password";
            var tmpPolicy = new CertificatePolicy(WellKnownIssuerNames.Self, cert.Subject);
            tmpPolicy.ContentType = CertificateContentType.Pkcs12;
            tmpPolicy.Exportable = true;
            tmpPolicy.KeySize = cert.PrivateKey.KeySize;
            string nameOfCert = cert.Subject;

            log.LogInformation("Staring IMporting ");
            var result = client.ImportCertificate(new ImportCertificateOptions(strlist[1], cert.Export(X509ContentType.Pfx, tempPw))
            {
                Password = tempPw,
                Policy = tmpPolicy
            });

            log.LogInformation("IMporting Completed");
            //log.LogInformation("Name is " + cert.Subject);
            //log.LogInformation("Thumbprint is " + cert.Thumbprint);
            /*}/*
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
            }*/
            return new OkObjectResult("OK");
        }
        public static X509Certificate2 ImportCertFromBase64(string rawCert, string password)
        {
            var pemData = Regex.Replace(Regex.Replace(rawCert, @"\s+", string.Empty), @"-+[^-]+-+", string.Empty);
            var pemBytes = Convert.FromBase64String(pemData);
            X509Certificate2 cert = new X509Certificate2(pemBytes, password, X509KeyStorageFlags.Exportable);
            return cert;
        }
    }
}
