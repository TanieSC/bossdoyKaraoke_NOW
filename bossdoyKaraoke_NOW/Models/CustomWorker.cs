using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bossdoyKaraoke_NOW.Models
{
    public class CustomWorker : BackgroundWorker
    {
        public CustomWorker(object sender)
        {
            this.Sender = sender;
        }

        public object Sender { get; private set; }

        public static void QueueWorker(
                            Queue<CustomWorker> queue,
                            object item,
                            Action<object, DoWorkEventArgs> action,
                            Action<object, RunWorkerCompletedEventArgs> actionComplete,
                            Action<RunWorkerCompletedEventArgs> displayError,
                            Action<object, ProgressChangedEventArgs> progressChange)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            using (var worker = new CustomWorker(item))
            {
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                worker.ProgressChanged += (sender, args) =>
                {
                    progressChange.Invoke(sender, args);
                };

                worker.DoWork += (sender, args) =>
                {
                    action.Invoke(sender, args);
                };

                worker.RunWorkerCompleted += (sender, args) =>
                {
                    actionComplete.Invoke(sender, args);
                    queue.Dequeue();
                    if (queue.Count > 0)
                    {
                        var next = queue.Peek();
                        next.ReportProgress(0, "Performing operation...");
                        next.RunWorkerAsync(next.Sender);
                    }
                    else
                        displayError.Invoke(args);
                };

                queue.Enqueue(worker);
                if (queue.Count == 1)
                {
                    var next = queue.Peek();
                    next.ReportProgress(0, "Performing operation...");
                    next.RunWorkerAsync(next.Sender);
                }
            }
        }
    }
}
