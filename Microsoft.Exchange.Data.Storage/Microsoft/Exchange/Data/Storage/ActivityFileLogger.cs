using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActivityFileLogger
	{
		public static ActivityFileLogger Instance
		{
			get
			{
				return ActivityFileLogger.lazyInstanceWrapper.Value;
			}
		}

		public string FilePrefix
		{
			get
			{
				return this.filePrefix;
			}
		}

		internal ActivityFileLogger()
		{
			int num = 0;
			string text = string.Empty;
			string version = "15.00.1497.012";
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				num = currentProcess.Id;
				text = currentProcess.ProcessName;
			}
			this.logSchema = new LogSchema(text, version, "ActivityLogging", ActivityFileLogger.LogSchemaStrings);
			this.filePrefix = string.Format("{0}_{1}_{2}_", "ActivityLogging", text, num.ToString());
			this.logger = new Log(this.filePrefix, new LogHeaderFormatter(this.logSchema), "ActivityLogging");
			this.logger.Configure(ActivityFileLogger.LogPath, ActivityLoggingConfig.Instance.MaxLogFileAge, (long)ActivityLoggingConfig.Instance.MaxLogDirectorySize.ToBytes(), (long)ActivityLoggingConfig.Instance.MaxLogFileSize.ToBytes());
		}

		internal static void ResetInstance()
		{
			ActivityFileLogger.lazyInstanceWrapper.Value.logger.Flush();
			ActivityFileLogger.lazyInstanceWrapper.Value.logger.Close();
			ActivityFileLogger.lazyInstanceWrapper = new Lazy<ActivityFileLogger>(() => new ActivityFileLogger());
		}

		internal LogRowFormatter GetFileLogRow(Activity activity)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[0] = activity.Id;
			logRowFormatter[1] = activity.TimeStamp.ToUtc().ToISOString();
			logRowFormatter[2] = activity.ClientId;
			logRowFormatter[3] = activity.MailboxGuid;
			logRowFormatter[4] = activity.ClientSessionId;
			logRowFormatter[5] = activity.SequenceNumber;
			StoreObjectId storeObjectId = activity.ItemId;
			logRowFormatter[6] = ((storeObjectId == null) ? null : storeObjectId.ToBase64ProviderLevelItemId());
			logRowFormatter[7] = activity.ClientVersion;
			logRowFormatter[8] = activity.TenantName;
			logRowFormatter[9] = activity.LocaleId;
			logRowFormatter[10] = activity.CustomPropertiesString;
			storeObjectId = activity.PreviousItemId;
			logRowFormatter[11] = ((storeObjectId == null) ? null : storeObjectId.ToBase64ProviderLevelItemId());
			logRowFormatter[12] = activity.ActivityCreationTime.ToUtc().ToISOString();
			logRowFormatter[13] = ((activity.MailboxType == null) ? null : string.Format("0x{0:X16}", activity.MailboxType));
			logRowFormatter[14] = ((activity.NetId == null) ? null : activity.NetId.ToString());
			return logRowFormatter;
		}

		public void Log(IEnumerable<Activity> activities)
		{
			if (activities == null)
			{
				throw new ArgumentNullException("activities");
			}
			int num = 0;
			bool flag = false;
			using (IEnumerator<Activity> enumerator = activities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Activity activity = enumerator.Current;
					flag = ActivityLogHelper.CatchNonFatalExceptions(delegate
					{
						this.Log(activity);
					}, null);
					if (flag)
					{
						break;
					}
					num++;
				}
			}
			if (flag)
			{
				ActivityFileLogger.activityPerfCounters.ActivityLogsFileWriteExceptions.Increment();
			}
			ActivityFileLogger.activityPerfCounters.ActivityLogsFileWriteCount.IncrementBy((long)num);
		}

		public void Flush()
		{
			this.logger.Flush();
		}

		private void Log(Activity activity)
		{
			LogRowFormatter fileLogRow = this.GetFileLogRow(activity);
			this.logger.Append(fileLogRow, -1);
		}

		internal const string ComponentName = "ActivityLogging";

		internal static readonly string LogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\ActivityLogging\\ActivityLogs");

		private static readonly string[] LogSchemaStrings = new string[]
		{
			"ActivityId",
			"Timestamp",
			"ClientId",
			"MailboxGuid",
			"ClientSession",
			"SequenceNumber",
			"ItemId",
			"ClientVersion",
			"TenantName",
			"LocaleId",
			"CustomProperties",
			"PreviousItemId",
			"ActivityCreationTime",
			"MailboxType",
			"NetId"
		};

		private static Lazy<ActivityFileLogger> lazyInstanceWrapper = new Lazy<ActivityFileLogger>(() => new ActivityFileLogger());

		private static MiddleTierStoragePerformanceCountersInstance activityPerfCounters = NamedPropMap.GetPerfCounters();

		private readonly LogSchema logSchema;

		private readonly Log logger;

		private readonly string filePrefix;
	}
}
