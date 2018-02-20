using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class TaskProducer
    {
        private TaskQueue _taskQueue;

        public TaskProducer(TaskQueue taskQueue) { _taskQueue = taskQueue; }

        /// <summary>
        /// Produces the tasks.
        /// </summary>
        public void ProduceTasks(Action action)
        {
                // Prepare Queue Object (Hold the Test Data)
                var queue = new QueuedObject
                {
                    ProducerThreadID = Thread.CurrentThread.ManagedThreadId,
                    EnqueueDateTime = DateTime.Now,
                };

                // Add Task to Queue with Action
                _taskQueue.Enqueue(action);
                //Console.WriteLine
                //    (
                //    "Enqueued: " + queue.QueueID +
                //    "\t" + "Producer ThreadID :" + queue.ProducerThreadID +
                //    "\t" + queue.EnqueueDateTime.ToLongTimeString() +
                //    "\t" + "RandomString   :" + queue.RandomString
                //    );
        }
    }
}
