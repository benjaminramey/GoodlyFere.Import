#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

#endregion

namespace GoodlyFere.Import.Tools
{
    public class ThreadManager
    {
        #region Constants and Fields

        private const int MaxThreads = 20;
        private const int ThreadSleep = 10000;

        private static readonly ILog Log = LogManager.GetLogger<ThreadManager>();

        private List<Task> _tasks;

        #endregion

        #region Constructors and Destructors

        public ThreadManager()
        {
            _tasks = new List<Task>(MaxThreads);
        }

        #endregion

        #region Public Methods

        public void RunWithAction(Action action)
        {
            Log.InfoFormat("Starting thread #{0}", _tasks.Count);

            Task task = new Task(action);
            task.Start();
            _tasks.Add(task);

            WaitForFreeSlot();
        }

        public void WaitForCompletion()
        {
            Log.Info("All thread groups assigned to threads.  Waiting for thread completion.");
            while (_tasks.Count > 0)
            {
                Thread.Sleep(ThreadSleep);
                PruneTasks();
            }
            Log.Info("All threads completed.");
        }

        #endregion

        #region Methods

        private void PruneTasks()
        {
            foreach (Task task in _tasks)
            {
                switch (task.Status)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingToRun:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.RanToCompletion:
                        break;
                    case TaskStatus.Canceled:
                        Log.ErrorFormat("Thread cancelled: {0}", task.Id);
                        break;
                    case TaskStatus.Faulted:
                        if (task.Exception != null)
                        {
                            Log.ErrorFormat("Thread faulted: {0}", task.Exception, task.Exception.Message);
                        }
                        else
                        {
                            Log.ErrorFormat("Thread faulted with no exception: {0}", task.Id);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _tasks = _tasks.Where(t => !(t.IsCanceled || t.IsCompleted || t.IsFaulted)).ToList();
        }

        private void WaitForFreeSlot()
        {
            if (_tasks.Count < MaxThreads)
            {
                return;
            }

            Log.InfoFormat("There are {0} threads.  Waiting.", MaxThreads);
            while (_tasks.Count == MaxThreads)
            {
                Thread.Sleep(ThreadSleep);
                PruneTasks();
            }

            Log.InfoFormat("There are now less than {0} threads--continuing.", MaxThreads);
        }

        #endregion
    }
}