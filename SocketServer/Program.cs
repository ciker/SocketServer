using System;
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

        static IPEndPoint LocalEndPoint
        {
            get
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                //return new IPEndPoint(iPHostEntry.AddressList[0], serverPort);
                return new IPEndPoint(iPHostEntry.AddressList[1], serverPort);
            }
        }

        public static async Task Main(string[] args)
        {
            var listener = new TcpListener(LocalEndPoint);
            listener.Start();

            Console.WriteLine($"Listening on {LocalEndPoint}");
            while (true)
            {
                var socket = await listener.AcceptTcpClientAsync();
                await ProcessClient(socket);
            }
        }

        private static async Task ProcessClient(TcpClient client)
        {
            Clients.Add(client);
            var stream = client.GetStream();
            var buffer = new byte[4 * 1_024];

            string message = "";
            //while (client.Connected)
            {
                try
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        message = message + Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine(message);

            var result = Clients.Remove(client);
        }

        private static List<TcpClient> Clients { get; } = new List<TcpClient>();
    }
}
