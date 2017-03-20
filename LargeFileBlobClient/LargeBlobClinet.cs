using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileBlobClient
{

    public class LargeBlobHandler
    {
        const int blobsize = 50000; //5 mb 


        CloudBlobContainer filesContainer;
        async Task<bool> InitStorage()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            filesContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("Container"));
            await filesContainer.CreateIfNotExistsAsync();

            Console.WriteLine("Storage Init Completed");

            return true;
        }

        private IEnumerable<FileBlock> GetFileBlocks(byte[] fileContent)
        {
            HashSet<FileBlock> hashSet = new HashSet<FileBlock>();
            if (fileContent.Length == 0)
                return new HashSet<FileBlock>();

            int blockId = 0;
            int ix = 0;

            int currentBlockSize = blobsize;

            while (currentBlockSize == blobsize)
            {
                if ((ix + currentBlockSize) > fileContent.Length)
                    currentBlockSize = fileContent.Length - ix;

                byte[] chunk = new byte[currentBlockSize];
                Array.Copy(fileContent, ix, chunk, 0, currentBlockSize);

                hashSet.Add(
                    new FileBlock()
                    {
                        Content = chunk,
                        Id = Convert.ToBase64String(System.BitConverter.GetBytes(blockId))
                    });

                ix += currentBlockSize;
                blockId++;
            }

            return hashSet;
        }

        public async Task UploadLargeFileToAzure(string filepath, string blobname)
        {
            //read file contents in byte array
            byte[] fileContent = File.ReadAllBytes(filepath);


            CloudBlockBlob blob = filesContainer.GetBlockBlobReference(blobname);
            blob.StreamWriteSizeInBytes = 10048;
            blob.StreamMinimumReadSizeInBytes = 10048;

            
            BlobRequestOptions options = new BlobRequestOptions();
            options.ServerTimeout = new TimeSpan(2, 0, 0);
            
            HashSet<string> blocklist = new HashSet<string>();
            List<FileBlock> bloksT = GetFileBlocks(fileContent).ToList();
            foreach (FileBlock block in GetFileBlocks(fileContent))
            {
                await blob.PutBlockAsync(
                    block.Id,
                    new MemoryStream(block.Content, true), null,
                    null, options, null
                    );

                blocklist.Add(block.Id);

            }
            //commit the blocks that are uploaded in above loop
            await blob.PutBlockListAsync(blocklist, null, options, null);
        }

        internal class FileBlock
        {
            public string Id
            {
                get;
                set;
            }

            public byte[] Content
            {
                get;
                set;
            }
        }
    }
}
