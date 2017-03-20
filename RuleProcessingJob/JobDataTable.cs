using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuleProcessingJob
{
    public class JobDataTable : TableEntity
    {
        public JobDataTable(string id, KeyValuePair<string,string> kv)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString() + kv.Key;
            this.RuleKey = kv.Key;
            this.RuleText = kv.Value;
        }

        public string RuleKey { get; set; }
        public string RuleText { get; set; }
        public string Status { get; set; }

        public JobDataTable() { }
    }
}
