using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessingJob
{
    interface IFileProcessor
    {
        Task Process();
    }
}
