using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesCommon
{
    public interface IExportRule
    {
        string RuleText { get; set; }

        Task ApplyRule(string filename, int linesize, string outputfile);
    }
}
