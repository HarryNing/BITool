using BIToolUtility.Common;
using BIToolUtility.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Helper
{
    public static class TableDataHelper
    {
        public static List<TableData> SourceTableList { get; set; }

        public static List<TableData> TargetTableList { get; set; }

        public static Dictionary<string, TableData> BatchTableDic
        {
            get
            {
                Dictionary<string, TableData> batchTableDic = new Dictionary<string, TableData>();
                foreach (var config in BIToolConfigHelper.BatchConfigs)
                {
                    foreach (var transform in config.Transforms)
                    {
                        if (!batchTableDic.ContainsKey(transform.SourceTableName))
                        {
                            TableData td = new TableData() { BatchName = config.BatchName, IsDelta = transform.IsDeltaLoad };
                            batchTableDic.Add(transform.SourceTableName, td);
                        }
                    }
                }
                return batchTableDic;
            }
        }

        public static void RefreshData()
        {
            if (SourceTableList != null)
            {
                foreach (TableData sourceTable in SourceTableList)
                {
                    if (TargetTableList != null)
                    {
                        TableData table = TargetTableList.FirstOrDefault(t => t.TargetTable == sourceTable.SourceTable);
                        if (table != null)
                        {
                            sourceTable.TargetTable = table.TargetTable;
                            sourceTable.TargetTableCount = table.TargetTableCount;
                            sourceTable.TargetColumns = table.TargetColumns;
                        }
                        else
                        {
                            sourceTable.TargetTable = string.Empty;
                            sourceTable.TargetTableCount = 0;
                        }
                    }

                    sourceTable.BatchName = string.Empty;
                    sourceTable.IsDelta = false;
                    if (BatchTableDic.ContainsKey(sourceTable.SourceTable))
                    {
                        sourceTable.BatchName = BatchTableDic[sourceTable.SourceTable].BatchName;
                        sourceTable.IsDelta = BatchTableDic[sourceTable.SourceTable].IsDelta;
                    }
                    sourceTable.PRValue = 0;
                }
            }
        }
    }
}
