using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api
{
    public static class CertBuilder
    {
        [FunctionName("CertBuilder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            { 
                log.LogInformation("C# HTTP trigger function started processing CertBuilder request.");
                string content = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Content is " + content);
                string[] separator = { ",","\"" };
                string[] strList = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                X509Certificate2 cert = ImportCertFromBase64(strList[0], "");
                
                log.LogInformation("Trying to access keyvault and Creating Certificate Client");
                string vaultUrl = "https://dkg7888.vault.azure.net/";
                var CertClient = new CertificateClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
                log.LogInformation("Certificate Client created");
                
                var certPassword = "password";
                var certPolicy = new CertificatePolicy(WellKnownIssuerNames.Self, cert.Subject);
                certPolicy.ContentType = CertificateContentType.Pkcs12;
                certPolicy.Exportable = true;
                certPolicy.KeySize = cert.PrivateKey.KeySize;
                string nameOfCert = cert.Subject;

                log.LogInformation("Starting Importing Certificate");
                var Result = CertClient.ImportCertificate(new ImportCertificateOptions(strList[1], cert.Export(X509ContentType.Pfx, certPassword))
                {
                    Password = certPassword,
                    Policy = certPolicy
                });
                log.LogInformation("Certificate Imported");
            
            }
            catch (Exception exception)
            {
                log.LogInformation(exception.ToString());
            }
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
