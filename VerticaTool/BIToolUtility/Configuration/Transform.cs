using BIToolUtility.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Configuration
{
    public class Transform
    {
        public string TargetTableName { get; set; }

        public string SourceTableName { get; set; }

        public string SqlConnection { get; set; }

        public string VerticaConnection { get; set; }

        public string SelectText { get; set; }

        public bool IsDeltaLoad { get; set; }

        public int BatchCount { get; set; }
    }
}
