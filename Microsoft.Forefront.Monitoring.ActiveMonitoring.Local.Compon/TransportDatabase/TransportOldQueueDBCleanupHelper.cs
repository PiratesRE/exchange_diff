using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.TransportDatabase
{
	public class TransportOldQueueDBCleanupHelper
	{
		internal static bool TryGetQueueDatabaseFolderPath(out string queueDatabasePath)
		{
			bool result = false;
			string exeConfigFilename = Path.Combine(ExchangeSetupContext.BinPath, "EdgeTransport.exe.config");
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = exeConfigFilename
			}, ConfigurationUserLevel.None);
			queueDatabasePath = configuration.AppSettings.Settings["QueueDatabasePath"].Value;
			if (!string.IsNullOrEmpty(queueDatabasePath) && Path.IsPathRooted(queueDatabasePath) && Directory.Exists(queueDatabasePath))
			{
				result = true;
			}
			return result;
		}

		internal static List<DirectoryInfo> GetExpiredOldQueueDatabases(string queueDatabasePath, TimeSpan oldDatabaseRetentionPeriod)
		{
			List<DirectoryInfo> list = new List<DirectoryInfo>();
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(queueDatabasePath);
				DirectoryInfo[] directories = directoryInfo.GetDirectories("Messaging.old*", SearchOption.AllDirectories);
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string[] array2 = directoryInfo2.Name.Split(new char[]
					{
						'-'
					});
					string s = (array2.Length > 1) ? array2[1] : string.Empty;
					DateTime t;
					if (DateTime.TryParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t) && t < DateTime.UtcNow - oldDatabaseRetentionPeriod)
					{
						list.Add(directoryInfo2);
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, new TracingContext(), string.Format("TransportDatabaseCleanupHelper-GetExpiredDatabases-Error:{0}", ex.Message), null, "GetExpiredOldQueueDatabases", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\TransportOldQueueDBCleanupHelper.cs", 111);
			}
			return list;
		}

		public const string OldMailDatabaseRetentionPeriodKey = "OldMailDatabaseRetentionPeriod";

		private const string TransportQueueDatabasePathConfigKey = "QueueDatabasePath";

		private const string AppConfigName = "EdgeTransport.exe.config";

		private const string OldDatabaseNamePattern = "Messaging.old*";

		private const string OldDatabaseDateTimeFormat = "yyyyMMddHHmmss";
	}
}
