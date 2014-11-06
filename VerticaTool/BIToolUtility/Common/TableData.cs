using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Common
{
    public class TableData
    {
        public string SourceTable { get; set; }

        public int SourceTableCount { get; set; }

        public string TargetTable { get; set; }

        public int TargetTableCount { get; set; }

        public string BatchName { get; set; }

        public bool IsDelta { get; set; }

        public bool NotExist { get { return TargetTable == string.Empty ? true : false; } }

        public string CreateTableText { get; set; }

        public int PRValue { get; set; }

        public string TargetColumns { get; set; }

    }
}
