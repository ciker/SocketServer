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

        public delegate Task TaskEventHandler(TaskProcessingArguments e);

        public event TaskEventHandler TaskStatus;

        public TaskQueue(IProducerConsumerCollection<Task> workTaskCollection, int queueSize, int timeout)
        {
            _workTaskQueue = new BlockingCollection<Task>(workTaskCollection, queueSize);
        }

        public void Enqueue(Action action, CancellationToken cs=default(CancellationToken))
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
        }

        public async Task DequeueTask()
        {
            //foreach (var task in _workTaskQueue.GetConsumingEnumerable())
            //    try
            //    {
            //        if (!task.IsCanceled) task.RunSynchronously();
            //        // if (!task.IsCanceled) task.Start();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            await Task.Delay(10000);
            if(_workTaskQueue.TryTake(out Task item))
            {
                if (!item.IsCanceled)
                {
                    //item.RunSynchronously();
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
