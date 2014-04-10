using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Controller
{
    public static class WorkQueue
    {
        private static readonly ConcurrentQueue<Task> _queue;
        private static Task _latestTask;

        public static int QueueSize
        {
            get { return _queue.Count; }
        }

        static WorkQueue()
        {
            _queue = new ConcurrentQueue<Task>();
        }

        public static void Enqueue(Task task)
        {
            if (_latestTask == null)
            {
                _latestTask = task;
                _latestTask.ContinueWith(TaskCompleted);
                _latestTask.Start();
            }
            else
            {
                _queue.Enqueue(task);
            }
        }

        private static void TaskCompleted(Task task)
        {
            if (_queue.IsEmpty)
            {
                _latestTask.Dispose();
                _latestTask = null;
                return;
            }

            Task nextTask;
            if (_queue.TryDequeue(out nextTask) == false)
            {
                return;
            }

            var oldTask = _latestTask;

            nextTask.ContinueWith(TaskCompleted);
            _latestTask = nextTask;
            _latestTask.Start();

            oldTask.Dispose();
        }
    }
}
