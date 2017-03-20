using System.Collections.Generic;

namespace RuleProcessingJob
{
    public interface ITableProcessor
    {
        IEnumerable<JobDataTable> QueryTableData();
        void UpdateTable(JobDataTable dt);
    }
}