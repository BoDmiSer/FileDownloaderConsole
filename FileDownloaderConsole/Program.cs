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
    public class Program : IFileDownloader
    {
        private static HttpClient _client = new HttpClient();
        private static IEnumerable<string> _urlList;
        private static int count;
        public event Action<string> OnDownloaded;
        public event Action<string> OnFailed;
        public void CheckDownload(string str)
        {
            OnDownloaded?.Invoke(str);
        }

        public void CheckFailed(string str)
        {
            OnFailed?.Invoke(str);
        }
        public Program()
        {
            this.OnDownloaded += Show_StringConsoleOk;
            this.OnFailed += ShowStringConsoleFail;
        }

        public static async Task Main(string[] args)
        {
               count = 0;
            _urlList = File.ReadAllLines("D:\\Download\\urls-list.txt");
            using (var semaphore = new SemaphoreSlim(4,25))
            {
                var task = _urlList.Select(async url =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        using (var result = await _client.GetAsync(url))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                using (var fileStream = await Task.Run(() => File.Create("D:\\Foto\\foto" + count++ + ".jpg")))
                                {
                                    var content = await result.Content.ReadAsStreamAsync();
                                    await content.CopyToAsync(fileStream);
                                }
                            }
                            Show_StringConsoleOk(url + " " + result.StatusCode);
                        }
                    }
                    catch (Exception e)
                    {
                        ShowStringConsoleFail(url + " " + e.Message);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(task);
            }
            Console.ReadKey();
        }

        public void SetDereeOfParallelism(int? degreeOfParallelism)
        {
            throw new NotImplementedException();
        }

        public void AddFileToDownloadingQueue(string fileID, string url, string pathToSave)
        {
            throw new NotImplementedException();
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
