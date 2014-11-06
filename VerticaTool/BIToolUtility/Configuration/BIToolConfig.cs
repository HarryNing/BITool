using BIToolUtility.Helper;
using BIToolUtility.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Configuration
{
    public class BIToolConfig
    {
        public List<Connection> SourceConnections { get; set; }

        public List<Connection> TargetConnections { get; set; }

        public int ThreadCount { get; set; }

        public List<string> Batches { get; set; }
    }

    public class BatchConfig
    {
        public string BatchName { get; set; }

        public List<Transform> Transforms { get; set; }

    }
}
