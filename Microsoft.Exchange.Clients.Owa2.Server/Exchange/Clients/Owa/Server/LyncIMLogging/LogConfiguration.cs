using System;
using System.Configuration;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging
{
	internal class LogConfiguration
	{
		internal static SyncLogConfiguration CreateSyncLogConfiguration()
		{
			return new SyncLogConfiguration
			{
				Enabled = LogConfiguration.GetAppSetting<bool>("LyncIMSyncLogEnabled", true),
				AgeQuotaInHours = LogConfiguration.GetAppSetting<long>("LyncIMSyncLogAgeQuota", 168L),
				DirectorySizeQuota = LogConfiguration.GetAppSetting<long>("LyncIMSyncLogDirectorySizeQuota", 256000L),
				PerFileSizeQuota = LogConfiguration.GetAppSetting<long>("LyncIMSyncLogFileSizeQuota", 10240L),
				LogFilePath = LogConfiguration.GetAppSetting<string>("LyncIMSyncLogFilePath", "Logging\\OWA\\InstantMessaging"),
				SyncLoggingLevel = LogConfiguration.GetAppSetting<SyncLoggingLevel>("LyncIMSyncLogLevel", SyncLoggingLevel.Debugging)
			};
		}

		internal static T GetAppSetting<T>(string key, T defaultValue)
		{
			try
			{
				string value = ConfigurationManager.AppSettings.Get(key);
				if (!string.IsNullOrWhiteSpace(value))
				{
					if (typeof(T).IsEnum)
					{
						return (T)((object)Enum.Parse(typeof(T), value, true));
					}
					return (T)((object)Convert.ChangeType(value, typeof(T)));
				}
			}
			catch (Exception)
			{
				return defaultValue;
			}
			return defaultValue;
		}

		private const int DefaultLogFileAgeInHours = 168;

		private const int DefaultLogDirectoryQuotaInKB = 256000;

		private const int DefaultPerFileQuotaInKB = 10240;
	}
}
