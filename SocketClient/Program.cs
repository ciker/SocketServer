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
        static int serverPort = 2017;

        static IPEndPoint LocalEndPoint
        {
            get
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                return new IPEndPoint(iPHostEntry.AddressList[0], serverPort);
            }
        }

        static async Task Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync("localhost", 8082);

                    if(client.Connected)
                    {
                        string reqNo = $"request no: {i}";
                        Console.WriteLine(reqNo);

                        var stream = client.GetStream();

                        var data = Encoding.ASCII.GetBytes(reqNo);

                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }

                await Task.Delay(1);

                i++;
            }
        }
    }
}
