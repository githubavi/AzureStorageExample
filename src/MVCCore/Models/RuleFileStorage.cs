using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCore.Models
{
    public class RuleFileStorage
    {
        public string AzStorjAccName { get; set; } = string.Empty;
        public string AzStorjAccKey { get; set; } = string.Empty;

        public string Container { get; set; } = string.Empty;
    }
}
