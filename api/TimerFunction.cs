using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
using Microsoft.Azure.Monitoring.DGrep.SDK;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Azure;
using Azure.Data.Tables;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;
using Newtonsoft.Json.Linq;

namespace api
{
    public static class TimerFunction
    {
        private static JsonSerializer JsonSerializer = new JsonSerializer();
        private static DateTimeOffset StartTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);

        public static async void AppendToBlob(BlobContainerClient containerClient, MemoryStream logEntryStream, string LogBlobName)
        {
            AppendBlobClient appendBlobClient = containerClient.GetAppendBlobClient(LogBlobName);
            appendBlobClient.CreateIfNotExists();
            var maxBlockSize = appendBlobClient.AppendBlobMaxAppendBlockBytes;
            var buffer = new byte[maxBlockSize];
            if (logEntryStream.Length <= maxBlockSize)
            {
                appendBlobClient.AppendBlock(logEntryStream);
            }
            else
            {
                var bytesLeft = (logEntryStream.Length - logEntryStream.Position);
                while (bytesLeft > 0)
                { 
                    if (bytesLeft >= maxBlockSize)
                    {
                        buffer = new byte[maxBlockSize];
                        await logEntryStream.ReadAsync
                            (buffer, 0, maxBlockSize);
                    }
                    else
                    {
                        buffer = new byte[bytesLeft];
                        await logEntryStream.ReadAsync
                            (buffer, 0, Convert.ToInt32(bytesLeft));
                    }
                    appendBlobClient.AppendBlock(new MemoryStream(buffer));
                    bytesLeft = (logEntryStream.Length - logEntryStream.Position);
                }
            }
        }

        public static string GetDGrep(string DGrep)
        {
            if (DGrep == "PublicCloud")
                return "https://dgrepv2-frontend-prod.trafficmanager.net";
            else if (DGrep == "Mooncake")
                return "https://dgrepv2-frontend-prod.trafficmanager.cn";
            else if (DGrep == "Blackforest")
                return "https://dgrepv2-frontend-prod.azuretrafficmanager.de";
            else if (DGrep == "Fairfax")
                return "https://dgrepv2-frontend-prod.usgovtrafficmanager.net";
            else if (DGrep == "USSec")
                return "https://dgrepv2-frontend-prod.trafficmanager.microsoft.scloud";
            else if (DGrep == "USNat")
                return "https://dgrepv2-frontend-prod.trafficmanager.eaglex.ic.gov";
            return "";
        }

        public static string GetMds(string Mds)
        {
            if (Mds == "Test")
                return "https://test1.diagnostics.monitoring.core.windows.net";
            else if (Mds == "Stage")
                return "https://stage.diagnostics.monitoring.core.windows.net";
            else if (Mds == "FirstPartyPROD")
                return "https://firstparty.monitoring.windows.net";
            else if (Mds == "ExternalPROD")
                return "https://monitoring.windows.net";
            else if (Mds == "DiagnosticsPROD")
                return "https://production.diagnostics.monitoring.core.windows.net";
            else if (Mds == "CAMooncake")
                return "https://monitoring.core.chinacloudapi.cn";
            else if (Mds == "CAFairfax")
                return "https://monitoring.core.usgovcloudapi.net";
            else if (Mds == "BlackForest")
                return "https://monitoring.core.cloudapi.de";
            return "";
        }
        [FunctionName("TimerFunction")]
        public static async Task Run([TimerTrigger("0 */3 * * * *")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            string accountName = "dhruv7888";
            var storageUri = "https://dhruv7888.table.core.windows.net/";
            string storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";
            string tableName = "UserInputs";
            var serviceClient = new TableServiceClient(new Uri(storageUri),
                new TableSharedKeyCredential(accountName, storageAccountKey));
            var tableClient = new TableClient(new Uri(storageUri), tableName,
                new TableSharedKeyCredential(accountName, storageAccountKey));

            Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>();
            Console.WriteLine(queryResultsFilter);
            string vaultUrl = "https://dkg7888.vault.azure.net/";
            var client = new CertificateClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
            var secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());

            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, storageAccountKey);
            string blobUri = "https://" + accountName + ".blob.core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
            string containerName = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
            Console.WriteLine(containerName);
            try
            {
                BlobContainerClient container2 = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container2.ExistsAsync())
                {
                    Console.WriteLine("Created container {0}", container2.Name);
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            
            foreach (TableEntity qEntity in queryResultsFilter)
            {
                CancellationToken cancellationToken = default;
                string CertificateName = qEntity.GetString("CertificateName");
                
                Response<KeyVaultCertificateWithPolicy> certResponse = await client.GetCertificateAsync(CertificateName);
                KeyVaultSecretIdentifier identifier = new KeyVaultSecretIdentifier(certResponse.Value.SecretId);
                Response<KeyVaultSecret> secretResponse = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
                
                KeyVaultSecret secret = secretResponse.Value;
                byte[] privateKeyBytes = Convert.FromBase64String(secret.Value);

                var cert = new X509Certificate2(privateKeyBytes);
                string DgrepEndpointUri = GetDGrep(qEntity.GetString("DGrepEndpoint"));
                Uri DGrepEndpoint = new Uri(DgrepEndpointUri);
                string MdsEndpointUri = GetMds(qEntity.GetString("MdsEndpoint"));

                string Nmspc = "^" + qEntity.GetString("Namespace") + "$";
                string Evnt = "^" + qEntity.GetString("EventName") + "$";
                using (var clientCert = new DGrepClient(DGrepEndpoint, cert))
                {
                    QueryInput SampleInputWithoutSvrQuery = new QueryInput
                    {
                        MdsEndpoint = new Uri(MdsEndpointUri),
                        EventFilters = new List<EventFilter>
                        {
                             new EventFilter { NamespaceRegex = Nmspc , NameRegex = Evnt },
                        },
                        MaxRowCount = 50000,
                        StartTime = StartTime,
                        EndTime = StartTime + TimeSpan.FromDays(1),
                        ServerQuery = "source",
                        ServerQueryType = QueryType.KQL
                    };
                    var result = clientCert.GetRowSetResultAsync(SampleInputWithoutSvrQuery, CancellationToken.None).GetAwaiter().GetResult();
                    string data = JsonConvert.SerializeObject(result);

                    string Namespace = qEntity.GetString("Namespace");
                    string EventName = qEntity.GetString("EventName");
                    string ServiceName = qEntity.GetString("ServiceName");
                    string APIName = qEntity.GetString("APIName");
                    string ExternalServiceName = qEntity.GetString("ExternalServiceName");
                    string ExternalAPIName = qEntity.GetString("ExternalAPIName");
                    string ExternalCallType = qEntity.GetString("ExternalCallType");

                    JObject json=JObject.Parse(data);
                    string ndata = json["RowSet"]["Rows"].ToString();
                    JArray Arr=JArray.Parse(ndata);
                    JArray JsonArray = new JArray(Arr.OrderBy(obj => (DateTime)obj["PreciseTimeStamp"]));

                    var Map = new Dictionary<string, JArray>();
                    foreach (JObject Obj in JsonArray)
                    {
                        JObject CurObj = new JObject();
                        CurObj.Add(ExternalServiceName, Obj[ExternalServiceName]);
                        CurObj.Add(ExternalAPIName, Obj[ExternalAPIName]);
                        CurObj.Add(ExternalCallType, Obj[ExternalCallType]);
                        string key = (Obj[ServiceName].ToString()) + "_______" + (Obj[APIName].ToString());
                        if(Map.ContainsKey(key))
                            Map[key].Add(CurObj);
                        else
                        {
                            Map.Add(key,new JArray());
                            Map[key].Add(CurObj);
                        }
                    }
                    string nam = Namespace + "_______" + EventName;
                    foreach (KeyValuePair<string, JArray> pair in Map)
                    {
                        string FileName=nam+ "_______"+pair.Key+".json";
                        string ToStore =pair.Value.ToString();
                        MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(ToStore));
                        Console.WriteLine(FileName);
                        AppendToBlob(container, mStrm, FileName);
                    }
                    Console.WriteLine("Done");
                }
            }
        }
    }
}









