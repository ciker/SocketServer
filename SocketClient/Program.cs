using Newtonsoft.Json;
using SocketServer.Protocols;
using System;
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

        static async Task Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync("localhost", serverPort);

                    if(client.Connected)
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

                        if(client.ReceiveBufferSize>0)
                        {
                            var bytes = new byte[client.ReceiveBufferSize];

                            await stream.ReadAsync(bytes, 0, client.ReceiveBufferSize);

                            string res = Encoding.UTF8.GetString(bytes);

                            Console.WriteLine(res);
                        }
                    }
                }

                await Task.Delay(1);

                i++;
            }
        }
    }
}
