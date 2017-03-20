using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace MVCCore.Models
{
    public class JobDataTable : TableEntity
    {
        public JobDataTable(string id, KeyValuePair<string,string> kv)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString() + kv.Key;
        }

        public string RuleKey { get; set; }
        public string RuleText { get; set; }
        public string Status { get; set; }

        public JobDataTable() { }
    }
}
