using Newtonsoft.Json;
using SocketServer.Protocols;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        static int serverPort = 8082;
        static BlockingCollection<QueueObject> blockingCollection
    = new BlockingCollection<QueueObject>(new ConcurrentQueue<QueueObject>(), 10000);

        static async Task Main(string[] args)
        {
            int i = 0;

            Task.Run(ProcessResponse);

            while (true)
            {
                var client = new TcpClient();

                await client.ConnectAsync("localhost", serverPort);

                if (client.Connected)
                {
                    var protocol = new Protocol
                    {
                        Id = Guid.NewGuid().ToString()
                    };

                    string serialized = JsonConvert.SerializeObject(protocol);

                    //string reqNo = $"request no: {Guid.NewGuid()}";
                    Console.WriteLine(serialized);

                    var stream = client.GetStream();

                    var data = Encoding.ASCII.GetBytes(serialized);

                    await stream.WriteAsync(data, 0, data.Length);

                    blockingCollection.Add(new QueueObject
                    {
                        Client = client
                    });

                    await Task.Delay(1);

                    i++;
                }
            }
        }

        static async Task ProcessResponse()
        {
            while(true)
            {
                if (blockingCollection.TryTake(out QueueObject output))
                {
                    using (TcpClient client = output.Client)
                    {
                        var stream = client.GetStream();

                        if (output.Client.ReceiveBufferSize > 0)
                        {
                            var bytes = new byte[client.ReceiveBufferSize];

                            await stream.ReadAsync(bytes, 0, client.ReceiveBufferSize);

                            string res = Encoding.UTF8.GetString(bytes);

                            var protocol = JsonConvert.DeserializeObject<Protocol>(res);

                            Console.WriteLine($"response: {protocol.Id}");

                        }
                    }
                }
            }
        }
    }
}
