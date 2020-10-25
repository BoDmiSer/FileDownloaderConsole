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
        #region Fields
        private static Queue<PhotoFile> _urlQueue = new Queue<PhotoFile>();
        private static HttpClient _client = new HttpClient();
        private int? _maxcountDownload;
        public event Action<string> OnDownloaded;
        public event Action<string> OnFailed;
        private static SemaphoreSlim _semaphore;
        #endregion

        #region Methods
        public async Task Downloads()
        {
            var photo = _urlQueue.Dequeue();
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
                            CheckDownload(photo.Url + " " + photo.Id.ToString() + " " + result.StatusCode.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CheckFailed(photo.Url + " " + photo.Id.ToString() + " " + e.Message.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void AddFileToDownloadingQueue(int fileID, string url, string pathToSave)
        {
            _urlQueue.Enqueue(new PhotoFile() { Id = fileID, Url = url, PathToSave = pathToSave });
            Task.Run(async () => await Downloads());
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
        #endregion

        #region Events
        public void CheckDownload(string str)
        {
            OnDownloaded?.Invoke(str);
        }
        public void CheckFailed(string str)
        {
            OnFailed?.Invoke(str);
        }
        #endregion
    }
}
