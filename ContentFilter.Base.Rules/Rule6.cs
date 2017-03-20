using RulesCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentFilter.Base.Rules
{
    [Export(typeof(IExportRule))]
    public class Rule6 : IExportRule
    {
        public string RuleText { get; set; } = "OR R4 R5";
        StringBuilder sb = new StringBuilder();

        /// <summary>
        /// returns output file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="linesize"></param>
        /// <param name="outputfile"></param>
        /// <returns></returns>
        public async Task ApplyRule(string filename, int linesize, string outputfile)
        {
            foreach (string line in File.ReadLines(filename))
            {
                if ((line.StartsWith("hello") && line.EndsWith("bye")) || !line.Contains("world"))
                {
                    sb.Append(line);
                    sb.AppendLine();
                }

                //flush data to outputfile in chunk
                if (sb.Length == linesize)
                {
                    WriteLines(outputfile);
                    sb.Clear();
                }
            }

            if (sb.Length > 0)
            {
                WriteLines(outputfile);
                sb.Clear();
            }

            return;
        }

        void WriteLines(string outputfile)
        {
            if (sb.Length > 0)
                File.AppendAllText(outputfile, sb.ToString());
        }
    }
}