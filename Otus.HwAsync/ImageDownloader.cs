
namespace Otus.HwAsync
{
    internal class ImageDownloader: IDisposable
    {
        public event Action<string>? DonwloadStarted;
        public event Action<string, long>? DonwloadCompleted;
        private readonly string remoteUrl;
        public readonly string fileName;
        private readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        private bool disposed = false;

        public ImageDownloader(string remoteUrl, string fileName)
        {
            this.remoteUrl = remoteUrl;
            this.fileName = fileName;
            this.path += fileName;
        }

        readonly HttpClient client = new HttpClient();
        public async Task GetPicture(CancellationToken token)
        {
            var _response = await client.GetAsync(remoteUrl, token);
            if (_response.IsSuccessStatusCode)
            {
                this.DonwloadStarted?.Invoke(this.fileName);
                var _bytes = await _response.Content.ReadAsByteArrayAsync(token);
                await File.WriteAllBytesAsync(path, _bytes);
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested(); // генерируем исключение

                FileInfo fileInfo = new FileInfo(path);
                this.DonwloadCompleted?.Invoke(this.fileName, fileInfo.Length);
            }
        }
        
        public void Dispose()
        {
            // освобождаем неуправляемые ресурсы
            client.Dispose();
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }
    }
}
