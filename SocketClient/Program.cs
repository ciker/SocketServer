using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            await Task.Delay(1000);
            int i = 0;
            while (true)
            {
                string req = await SendRequest("127.0.0.1", 50000, "none", "data" + i);

                await Task.Delay(1000);

                Console.WriteLine(req);

                i++;
            }
        }

        public static async Task<string> SendRequest(string server,
  int port, string method, string data)
        {
            try
            {
                IPAddress ipAddress = LocalEndPoint.Address;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(server);
                for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
                {
                    if (ipHostInfo.AddressList[i].AddressFamily ==
                      AddressFamily.InterNetwork)
                    {
                        ipAddress = ipHostInfo.AddressList[i];
                        break;
                    }
                }
                if (ipAddress == null)
                    throw new Exception("No IPv4 address for server");
                //TcpClient client = new TcpClient();

                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, port); // Connect
                    NetworkStream networkStream = client.GetStream();
                    StreamWriter writer = new StreamWriter(networkStream);
                    StreamReader reader = new StreamReader(networkStream);
                    writer.AutoFlush = true;
                    string requestData = "method=" + method + "&" + "data=" +
                      data + "&eor"; // 'End-of-request'
                    await writer.WriteLineAsync(requestData);
                    //string response = await reader.ReadLineAsync();

                    return requestData;
                }


                //return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
