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
        public static void Main(string[] args)
        {
            Downloader downloader = new Downloader();
            downloader.OnDownloaded += Show_StringConsoleOk;
            downloader.OnFailed += ShowStringConsoleFail;
            downloader.SetDereeOfParallelism(4);
            foreach (var url in File.ReadAllLines("D:\\Download\\urls-list.txt"))
            {
                downloader.AddFileToDownloadingQueue((url.GetHashCode() & 0xfffffff), url, "D:\\Foto\\");
            }
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
