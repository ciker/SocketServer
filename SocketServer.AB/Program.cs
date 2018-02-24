using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using SocketServer.Protocols;

namespace SocketServer.AB
{
    class Program
    {
        static int serverPort = 8082;

        static BlockingCollection<QueueObject> blockingCollection 
            = new BlockingCollection<QueueObject>(new ConcurrentQueue<QueueObject>(), 10000);

        static IPEndPoint LocalEndPoint
        {
            get
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                return new IPEndPoint(iPHostEntry.AddressList[1], serverPort);
            }
        }

        static ActionBlock<TcpClient> block = new ActionBlock<TcpClient>(_=>ProcessAsync(_),
    new ExecutionDataflowBlockOptions
    {
        BoundedCapacity = 10,
        CancellationToken = default,
        MaxDegreeOfParallelism = Environment.ProcessorCount
    });

        static async void ProcessAsync(TcpClient client)
        {
            //some long running task
            await Task.Delay(1000);

            var stream = client.GetStream();
            var buffer = new byte[4 * 1_024];

            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                //var bytes = Encoding.UTF8.GetBytes($"response: {counter}");
                //await stream.WriteAsync(bytes, 0, bytes.Length);

                if (bytesRead > 0)
                {
                    string content = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Protocol protocol = JsonConvert.DeserializeObject<Protocol>(content);

                    blockingCollection.Add(new QueueObject { Client=client, Protocol=protocol });

                    Console.WriteLine($"Processed Client: {protocol.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Console.WriteLine(message);

            //string message = Encoding.UTF8.GetString(input.Item1, 0, input.Item2);

        }

        public static async Task Main(string[] args)
        {
            var listener = new TcpListener(LocalEndPoint);
            listener.Start();

            Console.WriteLine($"Listening on {LocalEndPoint}");

            int counter = 0;


            Task.Run(DeQueueAsync);

            while (true)
            {
                var socket = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                block.Post(socket);

                //ProcessClient(socket);

                counter++;
            }
        }

        private static async void ProcessClient(TcpClient client)
        {
            //var stream = client.GetStream();
            //var buffer = new byte[4 * 1_024];

            //string message = "";

            //try
            //{

            //    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            //    //var bytes = Encoding.UTF8.GetBytes($"response: {counter}");
            //    //await stream.WriteAsync(bytes, 0, bytes.Length);

            //    if (bytesRead > 0)
            //    {
            //        block.Post(new Tuple<byte[], int>(buffer, bytesRead));
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //Console.WriteLine(message);
        }

        private static async Task DeQueueAsync()
        {
            while(true)
            {
                if(blockingCollection.TryTake(out QueueObject obj))
                {
                    NetworkStream stream = obj.Client.GetStream();

                    var serilized = JsonConvert.SerializeObject(obj.Protocol);

                    var bytes = Encoding.UTF8.GetBytes(serilized);

                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
