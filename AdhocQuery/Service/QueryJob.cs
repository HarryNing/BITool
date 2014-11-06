using AdhocQuery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdhocQuery.Services
{
    public class QueryJob : Job
    {
        public QueryJob(string jobId)
            : base(jobId)
        {
        }

        public void SendTop5(string htmlString)
        {
            _hubContext.Clients.Group(Id).top5Returned(htmlString);
        }

        public void ReportQueryComplete()
        {
            _hubContext.Clients.Group(Id).queryCompleted();
        }
    }
}