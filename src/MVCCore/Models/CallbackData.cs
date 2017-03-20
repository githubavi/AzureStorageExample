using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCore.Models
{
    public class CallbackData
    {
        public string CallbackUrl { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Method { get; } = "GET";
    }
}
