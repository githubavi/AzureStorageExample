using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RuleProcessingJob
{
    public class TableProcessor : ITableProcessor
    {
        public TableProcessor() { }

        public IEnumerable<JobDataTable> QueryTableData()
        {
            var storageAccount = CloudStorageAccount.Parse(
                               CloudConfigurationManager.GetSetting("TableConnStr"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("jobs");
            var query = new TableQuery<JobDataTable>().Where(TableQuery.GenerateFilterCondition("Status", QueryComparisons.Equal, "Created"));
            return table.ExecuteQuery(query);
        }

		public void UpdateTable(JobDataTable dt)
        {
            var storageAccount = CloudStorageAccount.Parse(
                               CloudConfigurationManager.GetSetting("TableConnStr"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("jobs");

            var updateop = TableOperation.Replace(dt);
            table.ExecuteAsync(updateop);
        }
    }
}
