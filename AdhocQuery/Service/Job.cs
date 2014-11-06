using AdhocQuery.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading;

namespace AdhocQuery.Services
{
    public class Job
    {
        public event EventHandler<EventArgs> ProgressChanged;
        public event EventHandler<EventArgs> Completed;

        private volatile int _progress;
        private CancellationTokenSource _cancellationTokenSource;
        protected IHubContext _hubContext;

        public Job(string id)
        {
            Id = id;
            _cancellationTokenSource = new CancellationTokenSource();
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        }

        public string Id { get; private set; }

        public int Progress
        {
            get { return _progress; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        public string TempFilePath { get; set; }

        public bool IsExceptioned { get; set; }

        public string ExceptionString { get; set; }

        public void ReportProgress(int progress)
        {
            if (_progress != progress)
            {
                _progress = progress;
                OnProgressChanged();
            }
        }

        public void ReportComplete()
        {
            OnCompleted();
        }

        protected virtual void OnCompleted()
        {
            var handler = Completed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnProgressChanged()
        {
            var handler = ProgressChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}