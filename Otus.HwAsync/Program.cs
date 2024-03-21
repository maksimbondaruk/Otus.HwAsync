

using System.Threading.Tasks;

namespace Otus.HwAsync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int Qty = 10;
            var notRunStatusTask = 0;

            List<ImageDownloader> imageDownloaderList = new List<ImageDownloader>(Qty);
            List<EventHandler> eventHandlerList = new List<EventHandler>(Qty);
            List<Task> doownloaserTaskList = new List<Task>(Qty);

            for (int i = 0; i < Qty; i++) 
            {
                imageDownloaderList.Add(new ImageDownloader("https://speedtest.selectel.ru/100MB", "100Mb_testfile" + i + ".txt"));
                eventHandlerList.Add(new EventHandler());
                eventHandlerList[i].Subscribe(imageDownloaderList[i]);
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            try
            {
                for (int i = 0; i < Qty; i++)
                {
                    doownloaserTaskList.Add(imageDownloaderList[i].GetPicture(token));
                }
                Console.WriteLine("Нажмите клавишу A для выхода или любую другую клавишу для проверки статуса скачивания");
                while (Console.ReadLine() != "A")
                {
                    for(int i = 0; i < Qty; i++)
                    {
                        Console.WriteLine($"Статус скачивания {imageDownloaderList[i].fileName}  {doownloaserTaskList[i].Status}");
                    }
                    for (int i = 0; i< Qty; i++)
                    {
                        if ((doownloaserTaskList[i].Status == TaskStatus.RanToCompletion) || 
                            (doownloaserTaskList[i].Status == TaskStatus.Faulted) || 
                            (doownloaserTaskList[i].Status == TaskStatus.Canceled))
                        {
                            notRunStatusTask++;
                        }
                    }
                   if (notRunStatusTask == Qty) 
                    {
                        break; 
                    }
                    Console.WriteLine("Нажмите клавишу A для выхода или любую другую клавишу для проверки статуса скачивания");
                }
                cts.Cancel();
                for (int i = 0; i < Qty; i++)
                {
                    doownloaserTaskList[i].Wait(); // ожидаем завершения задачи
                }
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions) 
                {
                    if (ex is TaskCanceledException)
                        Console.WriteLine("Операция прервана");
                    else
                        Console.WriteLine(ex.Message);
                }
            }
            finally
            {
                cts.Dispose();
            }
        }
    }   
}
