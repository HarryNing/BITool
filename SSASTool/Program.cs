using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS = Microsoft.AnalysisServices;

namespace SSASTool
{
    public struct PartitionInfo
    {
        public string CubeName;
        public string MeasureGroup;
        public string PartitionName;
        public long EstimatedSize;
    }

    class Program
    {
        static void Main(string[] args)
        {
            IList<PartitionInfo> plist = GetListOfPartitions("Initial Catalog=Odin DW;Data Source=10.43.45.23", "Odin DW");
            SaveToCsv(plist, "C:\\Excel\\Reports.xls");
        }

        public static IList<PartitionInfo> GetListOfPartitions(string ConnectionString, string DatabaseName)
        {
            List<PartitionInfo> LPI = new List<PartitionInfo>();
            using (AS.Server Server = new AS.Server())
            {
                Server.Connect(ConnectionString);
                AS.Database Database = Server.Databases.FindByName(DatabaseName);

                foreach (AS.Cube Cube in Database.Cubes)
                {
                    foreach (AS.MeasureGroup MG in Cube.MeasureGroups)
                    {
                        foreach (AS.Partition P in MG.Partitions)
                        {
                            PartitionInfo PI = new PartitionInfo();
                            PI.CubeName = Cube.Name;
                            PI.MeasureGroup = MG.Name;
                            PI.PartitionName = P.Name;
                            PI.EstimatedSize = P.EstimatedSize;
                            LPI.Add(PI);
                        }
                    }
                }
            }
            return LPI;
        }


        public static void SaveToCsv(IList<PartitionInfo> LPI, string Filename)
        {
            using (System.IO.StreamWriter SW = new System.IO.StreamWriter(Filename))
            {
                SW.WriteLine("CubeName,MeasureGroup,PartitionName,EstimatedSizeInBytes");
                foreach (PartitionInfo PI in LPI)
                {
                    string Row = string.Format("{0},{1},{2},{3}", PI.CubeName, PI.MeasureGroup, PI.PartitionName, PI.EstimatedSize);
                    SW.WriteLine(Row);
                }
            }
        }
    }
}
