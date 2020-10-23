using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloaderConsole
{
    public class Program
    {
        private static IEnumerable<string> _urlList;
        public static async Task Main(string[] args)
        {
            Downloader downloader = new Downloader();
            downloader.OnDownloaded += Show_StringConsoleOk;
            downloader.OnFailed += ShowStringConsoleFail;
            _urlList = File.ReadAllLines("D:\\Download\\urls-list.txt");
            downloader.SetDereeOfParallelism(8);
            foreach (var url in _urlList)
            {
                downloader.AddFileToDownloadingQueue((url.GetHashCode() & 0xfffffff), url, "D:\\Foto\\");
            }
            await downloader.Downloads();
            Console.ReadKey();
        }
        private static void Show_StringConsoleOk(string str)
        {
            Console.WriteLine(str);
        }
        private static void ShowStringConsoleFail(string str)
        {
            Console.WriteLine(str);
        }

    }
}
