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
                return new IPEndPoint(iPHostEntry.AddressList[1], serverPort);
            }
        }

        public static async Task Main(string[] args)
        {
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(LocalEndPoint);
            socketServer.Listen((int)SocketOptionName.MaxConnections);

            Console.WriteLine($"Listening on {LocalEndPoint}");
            while (true)
            {
                var socket = await Task<Socket>.Factory.FromAsync(socketServer.BeginAccept, socketServer.EndAccept, null);

                await ProcessClient(socket);
            }
        }

        private static async Task ProcessClient(Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            {

                string message = "";
                while (socket.Connected)
                {
                    var buffer = new byte[4 * 1_024];

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
            }
        }

    }
}
