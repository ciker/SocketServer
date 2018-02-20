using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    public class TaskProcessingArguments
    {
        public bool IsTaskAdded { get; set; }
        public int PendingTaskCount { get; set; }
        public string Message { get; set; }
    }
}
