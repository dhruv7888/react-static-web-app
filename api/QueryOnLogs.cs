//using Azure;
//using Azure.Data.Tables;
//using Azure.Identity;
//using Azure.Security.KeyVault.Certificates;
//using Azure.Security.KeyVault.Secrets;
//using Azure.Storage;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Specialized;
//using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
//using Microsoft.Azure.Monitoring.DGrep.SDK;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace api
//{
//    public static class QueryOnLogs
//    {
//        private static DateTimeOffset StartTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);

//        public static async void AppendToBlob(TraceWriter log, BlobContainerClient containerClient, MemoryStream logEntryStream, string LogBlobName)
//        {
//            log.Info("Starting Appending to Blob");
//            AppendBlobClient appendBlobClient = containerClient.GetAppendBlobClient(LogBlobName);
//            appendBlobClient.CreateIfNotExists();
//            var maxBlockSize = appendBlobClient.AppendBlobMaxAppendBlockBytes;
//            var buffer = new byte[maxBlockSize];
//            if (logEntryStream.Length <= maxBlockSize)
//            {
//                appendBlobClient.AppendBlock(logEntryStream);
//            }
//            else
//            {
//                var bytesLeft = (logEntryStream.Length - logEntryStream.Position);
//                while (bytesLeft > 0)
//                {
//                    if (bytesLeft >= maxBlockSize)
//                    {
//                        buffer = new byte[maxBlockSize];
//                        await logEntryStream.ReadAsync
//                            (buffer, 0, maxBlockSize);
//                    }
//                    else
//                    {
//                        buffer = new byte[bytesLeft];
//                        await logEntryStream.ReadAsync
//                            (buffer, 0, Convert.ToInt32(bytesLeft));
//                    }
//                    appendBlobClient.AppendBlock(new MemoryStream(buffer));
//                    bytesLeft = (logEntryStream.Length - logEntryStream.Position);
//                }
//            }
//            log.Info("Appended to Blob");
//        }

//        public static string GetDGrep(string DGrep)
//        {
//            if (DGrep == "PublicCloud")
//                return "https://dgrepv2-frontend-prod.trafficmanager.net";
//            else if (DGrep == "Mooncake")
//                return "https://dgrepv2-frontend-prod.trafficmanager.cn";
//            else if (DGrep == "Blackforest")
//                return "https://dgrepv2-frontend-prod.azuretrafficmanager.de";
//            else if (DGrep == "Fairfax")
//                return "https://dgrepv2-frontend-prod.usgovtrafficmanager.net";
//            else if (DGrep == "USSec")
//                return "https://dgrepv2-frontend-prod.trafficmanager.microsoft.scloud";
//            else if (DGrep == "USNat")
//                return "https://dgrepv2-frontend-prod.trafficmanager.eaglex.ic.gov";
//            return "";
//        }

//        public static string GetMds(string Mds)
//        {
//            if (Mds == "Test")
//                return "https://test1.diagnostics.monitoring.core.windows.net";
//            else if (Mds == "Stage")
//                return "https://stage.diagnostics.monitoring.core.windows.net";
//            else if (Mds == "FirstPartyPROD")
//                return "https://firstparty.monitoring.windows.net";
//            else if (Mds == "ExternalPROD")
//                return "https://monitoring.windows.net";
//            else if (Mds == "DiagnosticsPROD")
//                return "https://production.diagnostics.monitoring.core.windows.net";
//            else if (Mds == "CAMooncake")
//                return "https://monitoring.core.chinacloudapi.cn";
//            else if (Mds == "CAFairfax")
//                return "https://monitoring.core.usgovcloudapi.net";
//            else if (Mds == "BlackForest")
//                return "https://monitoring.core.cloudapi.de";
//            return "";
//        }
//        [FunctionName("QueryOnLogs")]
//        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, TraceWriter log)
//        {
//            log.Info($"QueryOnLogs Timer trigger function started execution at : {DateTime.Now}");

//            string accountName, storageAccountKey;
//            Pageable<TableEntity> tableEntities;
//            AccessingTable(log, out accountName, out storageAccountKey, out tableEntities);

//            CertificateClient client;
//            SecretClient secretClient;
//            AccessingKeyVault(log, out client, out secretClient);

//            BlobContainerClient container = await CreatingBlobContainer(log, accountName, storageAccountKey);

//            foreach (TableEntity entity in tableEntities)
//            {
//                log.Info($"Started processing Table Entity with Row Key {entity.GetString("RowKey")}");
//                X509Certificate2 cert = await CreatingX509Cert(log, client, secretClient, entity);

//                Uri DGrepEndpoint = new Uri(GetDGrep(entity.GetString("DGrepEndpoint")));

//                string MdsEndpointUri = GetMds(entity.GetString("MdsEndpoint"));
//                string regexQueryNamespace = "^" + entity.GetString("Namespace") + "$";
//                string regexQueryEvent = "^" + entity.GetString("EventName") + "$";

//                using (var clientCert = new DGrepClient(DGrepEndpoint, cert))
//                {
//                    QueryInput Query = GetQuery(MdsEndpointUri, regexQueryNamespace, regexQueryEvent);

//                    var result = clientCert.GetRowSetResultAsync(Query, CancellationToken.None).GetAwaiter().GetResult();
//                    log.Info("Queried on Logs");

//                    Dictionary<string, JArray> resultMap = ProcessingQueryResult(log, entity, result);

//                    CreatingBlob(log, container, entity, resultMap);
//                }
//                log.Info($"Completed processing Table Entity with Row Key {entity.GetString("RowKey")}");
//            }
//            log.Info($"QueryOnLogs Timer trigger function completed execution at : {DateTime.Now}");
//        }

//        private static void CreatingBlob(TraceWriter log, BlobContainerClient container, TableEntity entity, Dictionary<string, JArray> resultMap)
//        {
//            string Namespace = entity.GetString("Namespace");
//            string EventName = entity.GetString("EventName");
//            string name = Namespace + "~" + EventName;
//            log.Info("Files Created are : ");

//            foreach (KeyValuePair<string, JArray> pair in resultMap)
//            {
//                string fileName = name + "~" + pair.Key + ".json";
//                string toStore = pair.Value.ToString();
//                MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(toStore));
//                log.Info($"{fileName}");
//                AppendToBlob(log, container, mStrm, fileName);
//            }
//        }

//        private static Dictionary<string, JArray> ProcessingQueryResult(TraceWriter log, TableEntity entity, RowSetResult result)
//        {
//            log.Info("Started Porcessing Query Result");
//            string data = JsonConvert.SerializeObject(result);
//            string ExternalServiceName = entity.GetString("ExternalServiceName");
//            string ExternalAPIName = entity.GetString("ExternalAPIName");
//            string ExternalCallType = entity.GetString("ExternalCallType");
//            string ServiceName = entity.GetString("ServiceName");
//            string APIName = entity.GetString("APIName");

//            JObject dataJson = JObject.Parse(data);
//            string rows = dataJson["RowSet"]["Rows"].ToString();
//            JArray jArray = JArray.Parse(rows);

//            var hashMap = new Dictionary<string, HashSet<string>>();
//            var resultMap = new Dictionary<string, JArray>();
//            foreach (JObject Obj in jArray)
//            {
//                JObject toAddObj = new JObject();
//                toAddObj.Add("ExternalServiceName", Obj[ExternalServiceName]);
//                toAddObj.Add("ExternalAPINametea", Obj[ExternalAPIName]);
//                toAddObj.Add("ExternalCallType", Obj[ExternalCallType]);
//                string key = (Obj[ServiceName].ToString()) + "~" + (Obj[APIName].ToString());
//                string toAddObjString = Obj[ExternalCallType] + "~" + Obj[ExternalAPIName] + "~" + Obj[ExternalServiceName];
//                if (hashMap.ContainsKey(key))
//                {
//                    if (!(hashMap[key].Contains(toAddObjString)))
//                    {
//                        hashMap[key].Add(toAddObjString);
//                        resultMap[key].Add(toAddObj);
//                    }
//                }
//                else
//                {
//                    hashMap.Add(key, new HashSet<string>());
//                    resultMap.Add(key, new JArray());
//                    hashMap[key].Add(toAddObjString);
//                    resultMap[key].Add(toAddObj);
//                }
//            }
//            return resultMap;
//            log.Info("Query Result Processed");
//        }

//        private static QueryInput GetQuery(string MdsEndpointUri, string regexQueryNamespace, string regexQueryEvent)
//        {
//            return new QueryInput
//            {
//                MdsEndpoint = new Uri(MdsEndpointUri),
//                EventFilters = new List<EventFilter>
//                         {
//                              new EventFilter { NamespaceRegex = regexQueryNamespace , NameRegex = regexQueryEvent },
//                         },
//                MaxRowCount = 50000,
//                StartTime = StartTime,
//                EndTime = StartTime + TimeSpan.FromDays(1),
//                ServerQuery = "source",
//                ServerQueryType = QueryType.KQL
//            };
//        }

//        private static async Task<X509Certificate2> CreatingX509Cert(TraceWriter log, CertificateClient client, SecretClient secretClient, TableEntity entity)
//        {
//            log.Info($"Started creating X509Certificate of Certificate Name {entity.GetString("CertificateName")}");
//            string certificateName = entity.GetString("CertificateName");
//            Response<KeyVaultCertificateWithPolicy> certResponse = await client.GetCertificateAsync(certificateName);
//            KeyVaultSecretIdentifier identifier = new KeyVaultSecretIdentifier(certResponse.Value.SecretId);
//            Response<KeyVaultSecret> secretResponse = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
//            KeyVaultSecret secret = secretResponse.Value;
//            byte[] privateKeyBytes = Convert.FromBase64String(secret.Value);
//            var cert = new X509Certificate2(privateKeyBytes);
//            log.Info($"X509Certificate of Certificate Name {entity.GetString("CertificateName")} Created");
//            return cert;
//        }

//        private static async Task<BlobContainerClient> CreatingBlobContainer(TraceWriter log, string accountName, string storageAccountKey)
//        {
//            log.Info("Started Accessing BlobContainer");
//            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, storageAccountKey);
//            string blobUri = "https://" + accountName + ".blob.core.windows.net";
//            BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobUri), sharedKeyCredential);
//            log.Info("BlobContainer Accessed");
//            string containerName = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
//            try
//            {
//                BlobContainerClient container2 = await blobServiceClient.CreateBlobContainerAsync(containerName);

//                if (await container2.ExistsAsync())
//                {
//                    log.Info($"Created container {container2.Name}");
//                }
//            }
//            catch (RequestFailedException e)
//            {
//                log.Info(e.Message);
//            }
//            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
//            log.Info("Container Accessed");
//            return container;
//        }

//        private static void AccessingTable(TraceWriter log, out string accountName, out string storageAccountKey, out Pageable<TableEntity> tableEntities)
//        {
//            log.Info("Started Acessing Azure Storage Table");
//            accountName = "dhruv7888";
//            var storageUri = "https://dhruv7888.table.core.windows.net/";
//            storageAccountKey = "0RaeLG8wQ/95VTxebCCUK/j1kenM3nXixzCQGaGnbYcrCHK7FR0+ZrhCQ6X4q7N27i4c/Pwi3im0+AStT/FpKg==";
//            string tableName = "UserInputs";
//            var serviceClient = new TableServiceClient(new Uri(storageUri),
//                new TableSharedKeyCredential(accountName, storageAccountKey));
//            var tableClient = new TableClient(new Uri(storageUri), tableName,
//                new TableSharedKeyCredential(accountName, storageAccountKey));
//            tableEntities = tableClient.Query<TableEntity>();
//            log.Info("Azure Storage Table Accessed and Table Entities Extracted");
//        }

//        private static void AccessingKeyVault(TraceWriter log, out CertificateClient client, out SecretClient secretClient)
//        {
//            log.Info("Started Acessing KeyVault");
//            string vaultUrl = "https://dkg7888.vault.azure.net/";
//            client = new CertificateClient(vaultUri: new Uri(vaultUrl), credential: new DefaultAzureCredential());
//            secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
//            log.Info("KeyVault Accessed");
//        }
//    }
//}








