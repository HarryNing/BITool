using BIToolUtility.Common;
using BIToolUtility.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Helper
{
    public static class BIToolConfigHelper
    {
        private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BiToolConfig.xml");
        private static string batchConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "batch\\");

        static BIToolConfigHelper()
        {
            if (!Directory.Exists(batchConfigPath))
            {
                Directory.CreateDirectory(batchConfigPath);
            }
        }
        public static BIToolConfig Config { get; set; }

        public static List<BatchConfig> BatchConfigs { get; set; }

        public static void LoadConfig()
        {
            Config = SerializeHelper.LoadFromXml<BIToolConfig>(configPath);
            Config.SourceConnections = Config.SourceConnections.OrderBy(c => c.Name).ToList();
            Config.TargetConnections = Config.TargetConnections.OrderBy(c => c.Name).ToList();
            BatchConfigs = new List<BatchConfig>();
            foreach (var fileFullName in Directory.EnumerateFiles(batchConfigPath, "*.xml"))
            {
                string fileName = Path.GetFileNameWithoutExtension(fileFullName);
                if (!Config.Batches.Contains(fileName.Trim()))
                {
                    continue;
                }
                try
                {
                    BatchConfig config = SerializeHelper.LoadFromXml<BatchConfig>(fileFullName);
                    config.BatchName = config.BatchName.Trim();
                    BatchConfigs.Add(config);
                }
                catch { }
            }
        }

        public static void SaveConfig()
        {
            SerializeHelper.SaveToXml<BIToolConfig>(configPath, Config);
        }

        public static void SaveBatchConfig(List<BatchConfig> batchConfigs)
        {
            foreach (var fileName in Directory.EnumerateFiles(batchConfigPath, "*.xml"))
            {
                File.Delete(fileName);
            }

            foreach (var config in batchConfigs)
            {
                SerializeHelper.SaveToXml<BatchConfig>(GetBatchFilePath(config.BatchName), config);
            }
        }

        private static string GetBatchFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("batch\\{0}.xml",fileName));
        }

        public static void AddSourceConnection(Connection connection)
        {
            Config.SourceConnections.Add(connection);
            Config.SourceConnections = Config.SourceConnections.OrderBy(c => c.Name).ToList();
            SaveConfig();
        }
        public static void AddTargetConnection(Connection connection)
        {
            Config.TargetConnections.Add(connection);
            Config.TargetConnections = Config.TargetConnections.OrderBy(c => c.Name).ToList();
            SaveConfig();
        }
    }
}
