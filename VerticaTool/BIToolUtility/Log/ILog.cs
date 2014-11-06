using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace BIToolUtility.Log
{
    public interface ILogEx
    {
        void LogRejected(IList<long> rejectedRows, string fileName, string logFileName);
        void Log(string message, LogType type);
    }
}
