using AdhocQuery.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AdhocQuery.Services
{
    public class JobManager
    {
        public static readonly JobManager Instance = new JobManager();

        public JobManager()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        }
        ConcurrentDictionary<string, Job> _runningJobs = new ConcurrentDictionary<string, Job>();
        private IHubContext _hubContext;

        public Job DoJobAsync(Action<Job> action, Job job)
        {
            // this will (should!) never fail, because job.Id is globally unique
            _runningJobs.TryAdd(job.Id, job);

            Task.Factory.StartNew(() =>
            {
                action(job);
                if (job.IsExceptioned)
                {
                    _hubContext.Clients.Group(job.Id).exception(job.ExceptionString);
                }
                else
                {
                    job.ReportComplete();
                }
                if (!String.IsNullOrEmpty(job.TempFilePath))
                {
                    System.IO.File.Delete(job.TempFilePath);
                }
                _runningJobs.TryRemove(job.Id, out job);
            },
            TaskCreationOptions.LongRunning);
            BroadcastJobStatus(job);
            return job;
        }

        private void BroadcastJobStatus(Job job)
        {
            job.ProgressChanged += HandleJobProgressChanged;
            job.Completed += HandleJobCompleted;
        }

        private void HandleJobCompleted(object sender, EventArgs e)
        {
            var job = (Job)sender;

            _hubContext.Clients.Group(job.Id).jobCompleted(job.Id);

            job.ProgressChanged -= HandleJobProgressChanged;
            job.Completed -= HandleJobCompleted;
        }

        private void HandleJobProgressChanged(object sender, EventArgs e)
        {
            var job = (Job)sender;
            _hubContext.Clients.Group(job.Id).progressChanged(job.Progress);
        }

        public Job GetJob(string id)
        {
            Job result;
            return _runningJobs.TryGetValue(id, out result) ? result : null;
        }
    }
}