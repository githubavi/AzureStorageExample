using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVCCore.Common
{
    public class RuleParser
    {
        public static async Task<KeyValuePair<string,string>> ParseRule(IConfigurationRoot config, string inputruleparam)
        {
            string[] keywords = Regex.Split(inputruleparam.Trim(), @"\s{1,}");

            try
            {
                if (keywords.Length == 2 && string.Compare(keywords[0], "Apply", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var rulename = keywords[1];

                    var rulenum = Regex.Match(rulename, @"\d+").Value;

                    int ruleIndex;
                    if (int.TryParse(rulenum, out ruleIndex))
                    {
                        if (ruleIndex <= 0)
                            return default(KeyValuePair<string,string>);
                        else
                        {
                            var rule = config.GetValue<string>("RuleBook:Rules:" + (ruleIndex - 1) + ":" + rulename);
                            return new KeyValuePair<string, string>(rulename.ToUpper(), rule);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return default(KeyValuePair<string, string>);
        }
    }
}