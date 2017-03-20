using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCore.Common
{
    public interface IAzureJobManager
    {
        Task CreateJobInfo(Dictionary<string,KeyValuePair<string,string>> jobInfo);
    }
}
