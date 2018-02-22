using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TaskQueue
    {
        private BlockingCollection<Task> _workTaskQueue;

        public delegate void TaskEventHandler(TaskProcessingArguments e);

        public event TaskEventHandler TaskStatus;

        public TaskQueue(IProducerConsumerCollection<Task> workTaskCollection, int queueSize, int timeout)
        {
            _workTaskQueue = new BlockingCollection<Task>(workTaskCollection, queueSize);
        }

        public void Enqueue(Action action, CancellationToken cs=default)
        {
            var task = new Task(action, cs);

            if(_workTaskQueue.TryAdd(task))
            {
                TaskStatus?.Invoke(
                    new TaskProcessingArguments
                    {
                        IsTaskAdded = true,
                        Message = "Task Added to queue",
                        PendingTaskCount = _workTaskQueue.Count
                    });
            }
            else
            {
                TaskStatus?.Invoke
                    (new TaskProcessingArguments
                    {
                        IsTaskAdded = false,
                        Message = "Timedout while adding Task to Queue",
                        PendingTaskCount = _workTaskQueue.Count,
                    });
            }
        }

        public void DequeueTask()
        {
            if(_workTaskQueue.TryTake(out Task item))
            {
                if (!item.IsCanceled)
                {
                    item.RunSynchronously();
                }
            }
        }

        /// <summary>
        /// CompleteAdding : Will notify Consumer / Queue - Task Addition from Producer is Completed
        /// </summary>
        public void Close()
        {
            _workTaskQueue.CompleteAdding();
        }

        public int Count()
        {
            return _workTaskQueue.Count;
        }
    }
}
