using System;
using System.Configuration;
using System.Threading;

namespace Microsoft.Exchange.LogUploader
{
	internal class ConfigInstance : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
		}

		[IntegerValidator(MinValue = 1)]
		[ConfigurationProperty("BatchSizeInBytes", IsRequired = false, DefaultValue = 4096)]
		public int BatchSizeInBytes
		{
			get
			{
				return (int)base["BatchSizeInBytes"];
			}
			internal set
			{
				base["BatchSizeInBytes"] = value;
			}
		}

		[ConfigurationProperty("MaxNumOfReaders", IsRequired = false, DefaultValue = 1)]
		[IntegerValidator(MinValue = 1, MaxValue = 10)]
		public int MaxNumOfReaders
		{
			get
			{
				return (int)base["MaxNumOfReaders"];
			}
			internal set
			{
				base["MaxNumOfReaders"] = value;
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 10)]
		[ConfigurationProperty("MaxNumOfWriters", IsRequired = false, DefaultValue = 1)]
		public int MaxNumOfWriters
		{
			get
			{
				return (int)base["MaxNumOfWriters"];
			}
			internal set
			{
				base["MaxNumOfWriters"] = value;
			}
		}

		[ConfigurationProperty("ReaderSleepTime", IsRequired = false, DefaultValue = "00:00:04")]
		[TimeSpanValidator(MinValueString = "00:00:00.100")]
		public TimeSpan ReaderSleepTime
		{
			get
			{
				return (TimeSpan)base["ReaderSleepTime"];
			}
			internal set
			{
				base["ReaderSleepTime"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00.100", MaxValueString = "00:15:00")]
		[ConfigurationProperty("ReaderSleepTimeRandomRange", IsRequired = false, DefaultValue = "00:00:01")]
		public TimeSpan ReaderSleepTimeRandomRange
		{
			get
			{
				return (TimeSpan)base["ReaderSleepTimeRandomRange"];
			}
			internal set
			{
				base["ReaderSleepTimeRandomRange"] = value;
			}
		}

		[ConfigurationProperty("SleepTimeForTransientDBError", IsRequired = false, DefaultValue = "00:00:05")]
		[TimeSpanValidator(MinValueString = "00:00:00.100")]
		public TimeSpan SleepTimeForTransientDBError
		{
			get
			{
				return (TimeSpan)base["SleepTimeForTransientDBError"];
			}
			internal set
			{
				base["SleepTimeForTransientDBError"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01")]
		[ConfigurationProperty("LogDirCheckInterval", IsRequired = false, DefaultValue = "00:00:15")]
		public TimeSpan LogDirCheckInterval
		{
			get
			{
				return (TimeSpan)base["LogDirCheckInterval"];
			}
			internal set
			{
				base["LogDirCheckInterval"] = value;
			}
		}

		[ConfigurationProperty("QueueCapacity", IsRequired = false, DefaultValue = 100)]
		[IntegerValidator(MinValue = 1)]
		public int QueueCapacity
		{
			get
			{
				return (int)base["QueueCapacity"];
			}
			internal set
			{
				base["QueueCapacity"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01")]
		[ConfigurationProperty("ServiceStopWaitTime", IsRequired = false, DefaultValue = "00:02:00")]
		public TimeSpan ServiceStopWaitTime
		{
			get
			{
				return (TimeSpan)base["ServiceStopWaitTime"];
			}
			internal set
			{
				base["ServiceStopWaitTime"] = value;
			}
		}

		[ConfigurationProperty("OpenFileRetryInterval", IsRequired = false, DefaultValue = "00:00:05")]
		[TimeSpanValidator(MinValueString = "00:00:01")]
		public TimeSpan OpenFileRetryInterval
		{
			get
			{
				return (TimeSpan)base["OpenFileRetryInterval"];
			}
			internal set
			{
				base["OpenFileRetryInterval"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01")]
		[ConfigurationProperty("BacklogAlertNonUrgentThreshold", IsRequired = false, DefaultValue = "01:00:00")]
		public TimeSpan BacklogAlertNonUrgentThreshold
		{
			get
			{
				return (TimeSpan)base["BacklogAlertNonUrgentThreshold"];
			}
			internal set
			{
				base["BacklogAlertNonUrgentThreshold"] = value;
			}
		}

		[ConfigurationProperty("BacklogAlertUrgentThreshold", IsRequired = false, DefaultValue = "04:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:01")]
		public TimeSpan BacklogAlertUrgentThreshold
		{
			get
			{
				return (TimeSpan)base["BacklogAlertUrgentThreshold"];
			}
			internal set
			{
				base["BacklogAlertUrgentThreshold"] = value;
			}
		}

		[ConfigurationProperty("BacklogAlwaysAlertThreshold", IsRequired = false, DefaultValue = "12:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:01")]
		public TimeSpan BacklogAlwaysAlertThreshold
		{
			get
			{
				return (TimeSpan)base["BacklogAlwaysAlertThreshold"];
			}
			internal set
			{
				base["BacklogAlwaysAlertThreshold"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01")]
		[ConfigurationProperty("WaitTimeToReprocessActiveFile", IsRequired = false, DefaultValue = "00:00:45")]
		public TimeSpan WaitTimeToReprocessActiveFile
		{
			get
			{
				return (TimeSpan)base["WaitTimeToReprocessActiveFile"];
			}
			internal set
			{
				base["WaitTimeToReprocessActiveFile"] = value;
			}
		}

		[ConfigurationProperty("WaitTimeToReprocessActiveFileRandomRange", IsRequired = false, DefaultValue = "00:00:45")]
		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "00:15:00")]
		public TimeSpan WaitTimeToReprocessActiveFileRandomRange
		{
			get
			{
				return (TimeSpan)base["WaitTimeToReprocessActiveFileRandomRange"];
			}
			internal set
			{
				base["WaitTimeToReprocessActiveFileRandomRange"] = value;
			}
		}

		[ConfigurationProperty("BatchFlushInterval", IsRequired = false, DefaultValue = "00:00:30")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "00:05:00")]
		public TimeSpan BatchFlushInterval
		{
			get
			{
				return (TimeSpan)base["BatchFlushInterval"];
			}
			internal set
			{
				base["BatchFlushInterval"] = value;
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:10", MaxValueString = "12:00:00")]
		[ConfigurationProperty("RegionalFilteringFileRolloverTime", IsRequired = false, DefaultValue = "00:10:00")]
		public TimeSpan RegionalLogFilteringOutputRolloverTime
		{
			get
			{
				return (TimeSpan)base["RegionalFilteringFileRolloverTime"];
			}
		}

		[ConfigurationProperty("RetriesBeforeAlert", IsRequired = false, DefaultValue = "10")]
		[IntegerValidator(MinValue = 1)]
		public int RetriesBeforeAlert
		{
			get
			{
				return (int)base["RetriesBeforeAlert"];
			}
			internal set
			{
				base["RetriesBeforeAlert"] = value;
			}
		}

		[IntegerValidator(MinValue = 1)]
		[ConfigurationProperty("RetryCount", IsRequired = false, DefaultValue = "60")]
		public int RetryCount
		{
			get
			{
				return (int)base["RetryCount"];
			}
			internal set
			{
				base["RetryCount"] = value;
			}
		}

		[ConfigurationProperty("ProcessAllSplitLogPartitionsInParallel", IsRequired = false, DefaultValue = "false")]
		public bool ProcessAllSplitLogPartitionsInParallel
		{
			get
			{
				return (bool)base["ProcessAllSplitLogPartitionsInParallel"];
			}
			internal set
			{
				base["ProcessAllSplitLogPartitionsInParallel"] = value;
			}
		}

		[ConfigurationProperty("InputBufferMaxBatchCount", IsRequired = false, DefaultValue = "0")]
		public int InputBufferMaximumBatchCount
		{
			get
			{
				return (int)base["InputBufferMaxBatchCount"];
			}
			internal set
			{
				base["InputBufferMaxBatchCount"] = value;
			}
		}

		[ConfigurationProperty("ReaderPriority", IsRequired = false, DefaultValue = "Normal")]
		public ThreadPriority ReaderPrioritySetting
		{
			get
			{
				return (ThreadPriority)base["ReaderPriority"];
			}
		}

		[ConfigurationProperty("WriterPriority", IsRequired = false, DefaultValue = "Normal")]
		public ThreadPriority WriterPrioritySetting
		{
			get
			{
				return (ThreadPriority)base["WriterPriority"];
			}
		}

		[ConfigurationProperty("SaveLogForRegion", IsRequired = false, DefaultValue = "Unknown")]
		public Region SaveLogForRegion
		{
			get
			{
				return (Region)base["SaveLogForRegion"];
			}
		}

		[ConfigurationProperty("EnableMultipleWriters", IsRequired = false, DefaultValue = false)]
		public bool EnableMultipleWriters
		{
			get
			{
				return (bool)base["EnableMultipleWriters"];
			}
			internal set
			{
				base["EnableMultipleWriters"] = value;
			}
		}

		[ConfigurationProperty("ActiveLogFileIdleTimeout", IsRequired = false, DefaultValue = "2.00:00:00")]
		public TimeSpan ActiveLogFileIdleTimeout
		{
			get
			{
				return (TimeSpan)base["ActiveLogFileIdleTimeout"];
			}
			internal set
			{
				base["ActiveLogFileIdleTimeout"] = value;
			}
		}

		public const string NameKey = "Name";

		public const string BatchSizeKey = "BatchSizeInBytes";

		public const string MaxNumOfReadersKey = "MaxNumOfReaders";

		public const string ReaderPriority = "ReaderPriority";

		public const string MaxNumOfWritersKey = "MaxNumOfWriters";

		public const string WriterPriority = "WriterPriority";

		public const string ReaderSleepTimeKey = "ReaderSleepTime";

		public const string ReaderSleepTimeRandomRangeKey = "ReaderSleepTimeRandomRange";

		public const string SleepTimeForTransientDBErrorKey = "SleepTimeForTransientDBError";

		public const string LogDirCheckIntervalKey = "LogDirCheckInterval";

		public const string QueueCapacityKey = "QueueCapacity";

		public const string ServiceStopWaitTimeKey = "ServiceStopWaitTime";

		public const string OpenFileRetryIntervalKey = "OpenFileRetryInterval";

		public const string BacklogAlertNonUrgentThresholdKey = "BacklogAlertNonUrgentThreshold";

		public const string BacklogAlertUrgentThresholdKey = "BacklogAlertUrgentThreshold";

		public const string BacklogAlwaysAlertThresholdKey = "BacklogAlwaysAlertThreshold";

		public const string WaitTimeToReprocessActiveFileKey = "WaitTimeToReprocessActiveFile";

		public const string WaitTimeToReprocessActiveFileRandomRangeKey = "WaitTimeToReprocessActiveFileRandomRange";

		public const string BatchFlushIntervalKey = "BatchFlushInterval";

		public const string RetriesBeforeAlertKey = "RetriesBeforeAlert";

		public const string RetryCountKey = "RetryCount";

		public const string ProcessAllSplitLogPartitionsInParallelKey = "ProcessAllSplitLogPartitionsInParallel";

		public const string MaxInputBufferBatchCount = "InputBufferMaxBatchCount";

		public const string EnableMultipleWritersKey = "EnableMultipleWriters";

		public const string ActiveLogFileIdleTimeoutKey = "ActiveLogFileIdleTimeout";
	}
}
