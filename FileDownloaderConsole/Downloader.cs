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
    public class Downloader : IFileDownloader
    {
        private int? _maxcountDownload;
        private static List<PhotoFile> _urlList;
        public event Action<string> OnDownloaded;
        public event Action<string> OnFailed;
        private static HttpClient _client = new HttpClient();
        private static SemaphoreSlim _semaphore;
        public Downloader()
        {
            _urlList = new List<PhotoFile>();
        }

        public async Task Downloads()
        {
            using (var semaphore = _semaphore)
            {
                var task = _urlList.Select(async photo =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        using (var result = await _client.GetAsync(photo.Url))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                using (var fileStream = await Task.Run(() => File.Create(photo.PathToSave + photo.Id + ".jpg")))
                                {
                                    var content = await result.Content.ReadAsStreamAsync();
                                    await content.CopyToAsync(fileStream);
                                    CheckDownload(photo.Url+" "+photo.Id.ToString()+" "+result.StatusCode.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        CheckFailed(photo.Url + " "+photo.Id.ToString() + " " + e.Message.ToString());
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });
                await Task.WhenAll(task);
            }
        }

        public void AddFileToDownloadingQueue(int fileID, string url, string pathToSave)
        {
            _urlList.Add(new PhotoFile() { Id = fileID, Url = url, PathToSave = pathToSave });
        }

        public void SetDereeOfParallelism(int? degreeOfParallelism)
        {
            _maxcountDownload = degreeOfParallelism;
            if (_maxcountDownload != null)
            {
                _semaphore = new SemaphoreSlim(4, (int)_maxcountDownload);
            }
            else
            {
                _semaphore = new SemaphoreSlim(4);
            }
        }
        public void CheckDownload(string str)
        {
            OnDownloaded?.Invoke(str);
        }
        public void CheckFailed(string str)
        {
            OnFailed?.Invoke(str);
        }
    }
}
