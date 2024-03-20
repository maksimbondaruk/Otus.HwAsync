

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

            //var imageDownloader = new ImageDownloader("https://speedtest.selectel.ru/100MB", "100Mb.txt");
            //var eventHandler = new EventHandler();
            //eventHandler.Subscribe(imageDownloader);
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            try
            {
                //Task t = imageDownloader.GetPicture();
                //Task task = new(async () => await imageDownloader.GetPicture(token));
                for (int i = 0; i < Qty; i++)
                {
                    doownloaserTaskList[i] = imageDownloaderList[i].GetPicture(token);
                }
                //Task task = imageDownloader.GetPicture(token);
                Console.WriteLine("Нажмите клавишу A для выхода или любую другую клавишу для проверки статуса скачивания");
                while (Console.ReadLine() != "A")
                {
                    for(int i = 0; i < Qty; i++)
                    {
                        Console.WriteLine($"Статус скачивания {doownloaserTaskList[i].Status}");
                    }
                    //Console.WriteLine($"Статус скачивания {task.Status}");
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
                    //if ((task.Status == TaskStatus.RanToCompletion)||(task.Status == TaskStatus.Faulted)||(task.Status == TaskStatus.Canceled))
                    //    break;
                    Console.WriteLine("Нажмите клавишу A для выхода или любую другую клавишу для проверки статуса скачивания");
                    Console.WriteLine("while ID" + Thread.CurrentThread.ManagedThreadId.ToString());
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
