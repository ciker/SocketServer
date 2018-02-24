using SocketServer.Protocols;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SocketServer.Protocols
{
    public class QueueObject
    {
        public Protocol Protocol { get; set; }
        public TcpClient Client { get; set; }
    }
}
