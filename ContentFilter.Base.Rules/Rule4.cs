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
    public class Rule4 : IExportRule
    {
        public string RuleText { get; set; } = "AND R1 R2";
        //List<string> lines = new List<string>();
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
                if (line.StartsWith("hello") && line.EndsWith("bye"))
                {
                    //lines.Add(line);
                    sb.Append(line);
                    sb.AppendLine();
                }

                //flush data to outputfile in chunk
                if (sb.Length == linesize)
                {
                    WriteLines(outputfile);
                    //lines.Clear();
                    sb.Clear();
                }
            }

            if (sb.Length > 0)
            {
                WriteLines(outputfile);
                //lines.Clear();
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