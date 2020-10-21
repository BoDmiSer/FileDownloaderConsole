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
        void AddFileToDownloadingQueue(string fileID, string url, string pathToSave);
    }
}
