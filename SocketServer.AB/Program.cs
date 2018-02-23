using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SocketServer.AB
{
    class Program
    {
        static int serverPort = 8082;

        static IPEndPoint LocalEndPoint
        {
            get
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                return new IPEndPoint(iPHostEntry.AddressList[1], serverPort);
            }
        }

        static ActionBlock<Tuple<byte[], int>> block = new ActionBlock<Tuple<byte[], int>>(_=> ProcessAsync(_),
            new ExecutionDataflowBlockOptions {
                BoundedCapacity = 10,
                CancellationToken = default,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

        static async void ProcessAsync(Tuple<byte[], int> input)
        {
            //some long running task
            await Task.Delay(1000);

            string message = Encoding.UTF8.GetString(input.Item1, 0, input.Item2);

            Console.WriteLine($"Processed:{message}");
        }

        public static async Task Main(string[] args)
        {
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
                    block.Post(new Tuple<byte[], int>(buffer, bytesRead));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(message);
        }
    }
}
