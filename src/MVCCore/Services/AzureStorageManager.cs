using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using MVCCore.Common;

namespace MVCCore.Services
{
    public class AzureStorageManager : IAzureStorageManager
    {
        string accountName;
        string accountKey;
        string container;

        public AzureStorageManager(IConfigurationRoot config)
        {
            accountName = config.GetValue<string>("RuleFileStorage:AzStorjAccName");
            accountKey = config.GetValue<string>("RuleFileStorage:AzStorjAccKey");
            container = config.GetValue<string>("RuleFileStorage:Container");
        }

        public async Task<string> UploadtoAzureStorage(Stream body, string contenttype, string ruleName)
        {
            string filename = string.Empty;
            string id;

            try
            {
                var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer filesContainer = blobClient.GetContainerReference(container);
                await filesContainer.CreateIfNotExistsAsync();

                id = ruleName + "-" + Guid.NewGuid().ToString();
                filename = id + ".txt";

                CloudBlockBlob blob = filesContainer.GetBlockBlobReference(filename);
                blob.Properties.ContentType = contenttype;

                Task uploadtask = Task.Run(async () => { await blob.UploadFromStreamAsync(body); }).ContinueWith(t=>
                {
                    if (t.Exception != null)
                        throw t.Exception;
                });
            }
            catch(Exception e)
            {
                throw;
            }

            return id;
        }

        public async Task<string> DownloadfromAzureStorage(string id)
        {
            string data = string.Empty;
            string filename = id + ".txt";

            string[] splitstr = filename.Split("-".ToCharArray());
            if (splitstr.Length > 0)
            {
                var rkey = splitstr[0];

                var outputfilename = filename.Replace(rkey, "Result" + rkey);

                var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer filesContainer = blobClient.GetContainerReference(container);
                CloudBlockBlob blob = filesContainer.GetBlockBlobReference(@"Ouputfiles/" + outputfilename);

                if (await blob.ExistsAsync() && blob.Name.Contains(".txt"))
                {
                    // Save blob contents to a file.
                    return await blob.DownloadTextAsync();
                }
            }

            return string.Empty;
        }
    }
}
