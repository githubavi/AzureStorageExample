using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCore.Common
{
    public interface IAzureStorageManager
    {
        Task<string> UploadtoAzureStorage(Stream body, string contenttype, string ruleName);

        Task<string> DownloadfromAzureStorage(string id);
    }
}
