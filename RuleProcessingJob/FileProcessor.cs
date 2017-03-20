using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using RulesCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessingJob
{
    public class FileProcessor : IFileProcessor
    {
        List<Task> tasks = new List<Task>();
        //string rkey;
        //string rvalue;
        //JobDataTable dt;
        
        public FileProcessor()
        {
           
        }

        //Both contract name and type must match!
        [ImportMany(typeof(IExportRule))]
        public IEnumerable<IExportRule> exportRules { get; set; }

        async Task<bool> InitStorage()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            filesContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("Container"));

            Console.WriteLine("Storage Init Completed");

            return true;
        }

        CompositionContainer container;

        async Task<bool> InitRuleHandlers()
        {
            DirectoryCatalog catalog = new DirectoryCatalog(CloudConfigurationManager.GetSetting("RuleCatalogPath"), "*.dll");
            container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            Console.WriteLine("Rules Init Completed");
            return true;
        }

        CloudBlobContainer filesContainer;

        public async Task Process()
        {
            try
            {
                await InitStorage();

                await InitRuleHandlers();
            }
            catch(Exception)
            {
                throw;
            }

            ITableProcessor tbproc = new TableProcessor();
            var tableentries = tbproc.QueryTableData();

            foreach (var entry in tableentries)
            {
                var filename = entry.PartitionKey;
                CloudBlockBlob blob = filesContainer.GetBlockBlobReference(filename);
                if(blob != null)
                {
                    await Task.Delay(1000);
                    tasks.Add(DownloadBlob(blob, entry));
                }
            }

            //foreach (IListBlobItem item in filesContainer.ListBlobs(null, false))
            //{

            //}
            await Task.WhenAll(tasks);
        }

        async Task DownloadBlob(CloudBlockBlob blob, JobDataTable dt)
        {
            var specificBlobRef = filesContainer.GetBlockBlobReference(blob.Name);

            if (await blob.ExistsAsync() && blob.Name.Contains(".txt"))
            {
                // Save blob contents to a file.
                using (var fileStream = File.OpenWrite(@blob.Name))
                {
                    await blob.DownloadToStreamAsync(fileStream);
                }
                
                await ProcessFile(@blob.Name, dt);
            }
        }

        KeyValuePair<string, string> rules;
        ITableProcessor tbproc = new TableProcessor();
        async Task ProcessFile(string filename, JobDataTable dt)
        {
            string[] splitstr = filename.Split("-".ToCharArray());

            if (splitstr.Length > 0)
            {
                var filerulename = splitstr[0];

                if (dt.RuleKey == filerulename)
                {
                    //rule key matches

                    try
                    {
                        var rule = exportRules.FirstOrDefault(i => i.RuleText == dt.RuleText);
                        if(rule != null)
                        {
                            if (!Directory.Exists("./Outputfiles/"))
                                Directory.CreateDirectory("./Outputfiles/");

                            var outputdir = "./Outputfiles/";
                            var processedfilename = filename.Replace(dt.RuleKey, "Result" + dt.RuleKey);
                            var outputfilename = outputdir + processedfilename;

                            Console.WriteLine($"{filename} processing started...");

                            await rule.ApplyRule(filename, 1000, outputfilename);

                            CloudBlockBlob blockBlob = filesContainer.GetBlockBlobReference(@"Ouputfiles/" + processedfilename);

                            if (File.Exists(outputfilename))
                            {
                                dt.Status = "Running";
                                tbproc.UpdateTable(dt);

                                using (var fileStream = File.OpenRead(outputfilename))
                                {
                                    await blockBlob.UploadFromStreamAsync(fileStream);
                                    Console.WriteLine($"{outputfilename} uploaded...");
                                }

                                await Task.Delay(500);

                                File.Delete(outputfilename);

                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                    Console.WriteLine($"Temp {filename} deleted...");
                                }

                                dt.Status = "Completed";
                                tbproc.UpdateTable(dt);

                                Console.WriteLine($"Local {outputfilename} deleted...");
                            }

                            Console.WriteLine($"{filename} processing completed...");
                        }
                        else
                            Console.WriteLine($"No Rulehandler found with specified ruletext {dt.RuleText}");
                    }
                    catch (IOException ie)
                    {
                        Console.WriteLine(ie.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
                        
        }

    }
}
