using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	internal static class AppConfigReader
	{
		static AppConfigReader()
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			AppConfigReader.LogConfigurationSection = (LogConfiguration)configuration.GetSection("LogUploaderConfiguration");
			if (AppConfigReader.LogConfigurationSection == null)
			{
				throw new ConfigurationErrorsException("LogUploaderConfiguration section does not exist in app.config.");
			}
			AppConfigReader.DalStubSettings = (DalStubConfig)configuration.GetSection("DalStubConfig");
			if (AppConfigReader.DalStubSettings != null)
			{
				AppConfigReader.DalStubSettings.Partitions.BuildSearchList();
			}
			TimeSpan nonExistentDirectoryCheckInterval;
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("NonExistentDirectoryCheckInterval"), out nonExistentDirectoryCheckInterval))
			{
				AppConfigReader.NonExistentDirectoryCheckInterval = nonExistentDirectoryCheckInterval;
			}
			else
			{
				AppConfigReader.NonExistentDirectoryCheckInterval = TimeSpan.FromMinutes(5.0);
			}
			bool skipLinesFromSprodMsit;
			if (bool.TryParse(AppConfigReader.GetAppSettingStringValue("SkipLinesFromSprodMSIT"), out skipLinesFromSprodMsit))
			{
				AppConfigReader.SkipLinesFromSprodMsit = skipLinesFromSprodMsit;
			}
			else
			{
				AppConfigReader.SkipLinesFromSprodMsit = false;
			}
			bool useDefaultRegionTag;
			if (bool.TryParse(AppConfigReader.GetAppSettingStringValue("UseDefaultRegionTag"), out useDefaultRegionTag))
			{
				AppConfigReader.UseDefaultRegionTag = useDefaultRegionTag;
			}
			else
			{
				AppConfigReader.UseDefaultRegionTag = false;
			}
			TimeSpan persistentStoreDetailsRecheckTimerInterval;
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("PersistentStoreDetailsRecheckTimerInterval"), out persistentStoreDetailsRecheckTimerInterval))
			{
				AppConfigReader.PersistentStoreDetailsRecheckTimerInterval = persistentStoreDetailsRecheckTimerInterval;
			}
			else
			{
				AppConfigReader.PersistentStoreDetailsRecheckTimerInterval = TimeSpan.FromHours(1.0);
			}
			int num;
			if (int.TryParse(AppConfigReader.GetAppSettingStringValue("LogProcessingCutOffDays"), out num) && num > 0)
			{
				AppConfigReader.LogProcessingCutOffDays = TimeSpan.FromDays((double)num);
			}
			else
			{
				AppConfigReader.LogProcessingCutOffDays = TimeSpan.FromDays(7.0);
			}
			TimeSpan newerLogCutOffTimeSpan;
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("NewerLogCutOffTimeSpan"), out newerLogCutOffTimeSpan))
			{
				AppConfigReader.NewerLogCutOffTimeSpan = newerLogCutOffTimeSpan;
			}
			else
			{
				AppConfigReader.NewerLogCutOffTimeSpan = TimeSpan.FromHours(2.0);
			}
			int num2;
			if (int.TryParse(AppConfigReader.GetAppSettingStringValue("MaxNumberOfMessagesInBatch"), out num2) && num2 > 0)
			{
				AppConfigReader.MaxNumberOfMessagesInBatch = num2;
			}
			else
			{
				AppConfigReader.MaxNumberOfMessagesInBatch = 200;
			}
			if (!int.TryParse(AppConfigReader.GetAppSettingStringValue("MaxNumberOfEventsInBatch"), out AppConfigReader.MaxNumberOfEventsInBatch) || AppConfigReader.MaxNumberOfEventsInBatch <= 0)
			{
				AppConfigReader.MaxNumberOfEventsInBatch = 1000;
			}
			if (!int.TryParse(AppConfigReader.GetAppSettingStringValue("MaxNumberOfRecipientsInBatch"), out AppConfigReader.MaxNumberOfRecipientsInBatch) || AppConfigReader.MaxNumberOfRecipientsInBatch <= 0)
			{
				AppConfigReader.MaxNumberOfRecipientsInBatch = 1000;
			}
			TimeSpan maxWaitTimeBeforeAlertOnBackLog;
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("MaxWaitTimeBeforeAlertOnBackLog"), out maxWaitTimeBeforeAlertOnBackLog))
			{
				AppConfigReader.MaxWaitTimeBeforeAlertOnBackLog = maxWaitTimeBeforeAlertOnBackLog;
			}
			else
			{
				AppConfigReader.MaxWaitTimeBeforeAlertOnBackLog = TimeSpan.FromMinutes(5.0);
			}
			TimeSpan timeSpan;
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("WriterFailureSampleExpirationTime"), out timeSpan))
			{
				AppConfigReader.WriterFailureSampleExpirationTimeSpan = timeSpan;
			}
			else
			{
				AppConfigReader.WriterFailureSampleExpirationTimeSpan = TimeSpan.FromMinutes(15.0);
			}
			if (TimeSpan.TryParse(AppConfigReader.GetAppSettingStringValue("WriterLongLatencyFailureThreshold"), out timeSpan))
			{
				AppConfigReader.WriterLongLatencyFailureThresholdTimeSpan = timeSpan;
			}
			else
			{
				AppConfigReader.WriterLongLatencyFailureThresholdTimeSpan = TimeSpan.FromSeconds(120.0);
			}
			int num3;
			if (int.TryParse(AppConfigReader.GetAppSettingStringValue("WriterConsecutiveFailureHealthyThreshold"), out num3) && num3 > 0)
			{
				AppConfigReader.WriterConsecutiveFailureHealthyThresholdCount = num3;
				return;
			}
			AppConfigReader.WriterConsecutiveFailureHealthyThresholdCount = 10;
		}

		internal static IEnumerable<LogTypeInstance> GetLogTypeInstancesInEnvironment(string envName, string region = "")
		{
			ProcessingEnvironmentCollection environments = AppConfigReader.LogConfigurationSection.Environments;
			List<LogTypeInstance> list = new List<LogTypeInstance>();
			foreach (object obj in environments)
			{
				ProcessingEnvironment processingEnvironment = (ProcessingEnvironment)obj;
				if (string.Compare(processingEnvironment.EnvironmentName, envName, true) == 0 && (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(processingEnvironment.Region) || string.Compare(processingEnvironment.Region, region, true) == 0))
				{
					foreach (object obj2 in processingEnvironment.Logs)
					{
						LogTypeInstance item = (LogTypeInstance)obj2;
						list.Add(item);
					}
				}
			}
			return list;
		}

		internal static ConfigInstance GetConfigurationByName(string name)
		{
			return AppConfigReader.LogConfigurationSection.ConfigSettings.Get(name);
		}

		internal static ConfigInstance GetDefaultConfiguration()
		{
			return AppConfigReader.LogConfigurationSection.ConfigSettings.Get("Default");
		}

		internal static string GetAppSettingStringValue(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		private const string DefaultConfigSettingName = "Default";

		private const string SkipLinesFromSprodMsitKey = "SkipLinesFromSprodMSIT";

		private const string PersistentStoreDetailsRecheckTimerIntervalKey = "PersistentStoreDetailsRecheckTimerInterval";

		private const string LogProcessingCutOffDaysKey = "LogProcessingCutOffDays";

		private const string NewerLogCutOffTimeSpanKey = "NewerLogCutOffTimeSpan";

		private const string NonExistentDirectoryCheckIntervalKey = "NonExistentDirectoryCheckInterval";

		private const string UseDefaultRegionTagKey = "UseDefaultRegionTag";

		private const string MaxNumberOfMessagesInBatchKey = "MaxNumberOfMessagesInBatch";

		private const string MaxWaitTimeBeforeAlertOnBackLogKey = "MaxWaitTimeBeforeAlertOnBackLog";

		internal static readonly LogConfiguration LogConfigurationSection;

		internal static readonly DalStubConfig DalStubSettings;

		internal static readonly bool SkipLinesFromSprodMsit;

		internal static readonly TimeSpan PersistentStoreDetailsRecheckTimerInterval;

		internal static readonly TimeSpan LogProcessingCutOffDays;

		internal static readonly TimeSpan NewerLogCutOffTimeSpan;

		internal static readonly TimeSpan NonExistentDirectoryCheckInterval;

		internal static readonly bool UseDefaultRegionTag;

		internal static readonly int MaxNumberOfMessagesInBatch;

		internal static readonly int MaxNumberOfEventsInBatch;

		internal static readonly int MaxNumberOfRecipientsInBatch;

		internal static readonly TimeSpan MaxWaitTimeBeforeAlertOnBackLog;

		internal static readonly TimeSpan WriterFailureSampleExpirationTimeSpan;

		internal static readonly TimeSpan WriterLongLatencyFailureThresholdTimeSpan;

		internal static readonly int WriterConsecutiveFailureHealthyThresholdCount;
	}
}
