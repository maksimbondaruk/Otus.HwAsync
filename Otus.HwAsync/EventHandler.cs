using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus.HwAsync
{
    internal class EventHandler
    {
        public void Subscribe (ImageDownloader Downloader) 
        {
            Downloader.DonwloadStarted += (string fileName) =>
            {
                Console.WriteLine($"Начата загрузка файла {fileName}");
            };

            Downloader.DonwloadCompleted += (string fileName, long fileLength) =>
            {
                Console.WriteLine($"Загрузка файла {fileName} размером {fileLength/1024}кБ завершена");
            };
        }
    }
}
