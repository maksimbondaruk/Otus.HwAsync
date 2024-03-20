
namespace Otus.HwAsync
{
    internal class ImageDownloader
    {
        public event Action<string> DonwloadStarted;
        public event Action<string, long> DonwloadCompleted;
        private string remoteUrl;
        private string fileName;
        private string path = @"C:\Users\bondaruk\source\repos\Otus.HwAsync\";
       public ImageDownloader(string remoteUrl, string fileName)
        {
            this.remoteUrl = remoteUrl;
            this.fileName = fileName;
            this.path += fileName;
        }

        HttpClient client = new HttpClient();
        public async Task GetPicture(CancellationToken token)
        {
            //Console.WriteLine($"Качаю {fileName} из {remoteUrl}");
            var _response = await client.GetAsync(remoteUrl);
            if (_response.IsSuccessStatusCode)
            {
                this.DonwloadStarted?.Invoke(this.fileName);
                var _bytes = await _response.Content.ReadAsByteArrayAsync();
                //await File.WriteAllBytesAsync(path, _bytes);
                _ = Task.Run(() => File.WriteAllBytesAsync(path, _bytes),token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested(); // генерируем исключение

                FileInfo fileInfo = new FileInfo(path);
                this.DonwloadCompleted?.Invoke(this.fileName, fileInfo.Length);
                //Console.WriteLine($"Успешно скачал {fileName} из {remoteUrl}");
                Console.WriteLine("WriteAllBytesAsync ID" + Thread.CurrentThread.ManagedThreadId.ToString());
            }
        }
    }
}
