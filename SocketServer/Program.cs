using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    class Program
    {
        static int serverPort = 8082;

        static TaskQueue TaskQueue = new TaskQueue(new ConcurrentQueue<Task>(), 10000, 10);

        static IPEndPoint LocalEndPoint
        {
            get
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                return new IPEndPoint(iPHostEntry.AddressList[1], serverPort);
            }
        }

        public static async Task Main(string[] args)
        {
            // Subscrive to Queue Processing Events
            TaskQueue.TaskStatus += WorkQueue_TaskStatus;

            List<Task> tasks = new List<Task>(20);

            var listener = new TcpListener(LocalEndPoint);
            listener.Start();

            Console.WriteLine($"Listening on {LocalEndPoint}");

            while (true)
            {
                var socket = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                ProcessClient(socket);
            }

        }

        private static async void ProcessClient(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4 * 1_024];

            string message = "";

            try
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    message = message + Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }

                TaskQueue.Enqueue(async () =>
                {
                    //some long running task
                    await Task.Delay(1000);

                    Console.WriteLine($"Task Dequeued: {message}");
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(message);
        }

        private static void WorkQueue_TaskStatus(TaskProcessingArguments e)
        {
            TaskQueue.DequeueTask();
        }
    }
}
