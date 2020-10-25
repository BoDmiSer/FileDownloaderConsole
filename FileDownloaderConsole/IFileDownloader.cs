using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderConsole
{
    interface IFileDownloader
    {
        void SetDereeOfParallelism(int? degreeOfParallelism);
        void AddFileToDownloadingQueue(int fileID, string url, string pathToSave);
        event Action<string> OnDownloaded;
        event Action<string> OnFailed;
    }
}
