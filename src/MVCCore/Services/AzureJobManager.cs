using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MVCCore.Common;
using MVCCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MVCCore.Services
{
    public class AzureJobManager : IAzureJobManager
    {
        public AzureJobManager(IConfigurationRoot config)
        {
            connstr = config.GetValue<string>("JobStorageConn");
            storjAcc = CloudStorageAccount.Parse(connstr);

            //App Service Publish Profile Credentials 
            //string userName = "$webjobavi"; //userName 
            //string userPassword = "iraz5eGwa9NqkpPvK9C7WY5hbeA8Xacm5k8HfS4DP9TgQRLlwWEzNkc9Qq5o"; //userPWD 

            //change webJobName to your WebJob name 
            //webJobName = "RuleEngine";

            //var unEncodedString = String.Format($"{userName}:{userPassword}");
            //encodedString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(unEncodedString));
        }

        string connstr;
        CloudStorageAccount storjAcc;

        //public async Task InvokeWebJob(string parameter)
        //{
        //    try
        //    {
        //        //Change this URL to your WebApp hosting the  
        //        url = "https://webjobavi.scm.azurewebsites.net/api/triggeredwebjobs/" + webJobName + "/run?arguments=" + parameter;

        //        WebRequest request = WebRequest.Create(url);
        //        request.Method = "POST";
        //        request.Headers["Authorization"] = "Basic " + encodedString;

        //        System.Net.WebResponse response = await request.GetResponseAsync();

        //        //System.IO.Stream dataStream = response.GetResponseStream();
        //        //System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
        //        //string responseFromServer = reader.ReadToEnd();
        //    }
        //    catch (Exception )
        //    {
        //        throw;
        //    }
        //}

        public async Task CreateJobInfo(Dictionary<string, KeyValuePair<string, string>> jobInfo)
        {
            var tableClient = storjAcc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("jobs");
            await table.CreateIfNotExistsAsync();

            foreach (var item in jobInfo)
            {
                var insertOperation = TableOperation.Insert(new JobDataTable(item.Key, item.Value) { RuleKey = item.Value.Key, RuleText=item.Value.Value, Status = "Created" });
                await table.ExecuteAsync(insertOperation);
            }
        }
    }
}
