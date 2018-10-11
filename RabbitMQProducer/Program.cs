using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RabbitMQProducer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region 
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            PositionWindow();
            #endregion
            
            Console.WriteLine("Press to start");
            Console.ReadLine();
            
            using (var rabbitClient = new RabbitMqProducer())
            {
                while (true)
                {
                    rabbitClient.PlaceOrder();
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }

            /*  while (true)
            {
                var azureClient = new AzureServiceBusProducer(configuration["ConnectionString"]);
           
                azureClient.PlaceOrder();
            }*/

        }


        #region
        private static void PositionWindow()
        {
            Console.SetWindowSize(45, 20);
            Console.SetBufferSize(45, 20);
            MoveWindow(25, 100);
        }

        private static void MoveWindow(int left, int top)
        {
            var handle = GetConsoleWindow();
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            MoveWindow(handle, left, top, rect.right - rect.left, rect.bottom - rect.top, true);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion
    }
}

