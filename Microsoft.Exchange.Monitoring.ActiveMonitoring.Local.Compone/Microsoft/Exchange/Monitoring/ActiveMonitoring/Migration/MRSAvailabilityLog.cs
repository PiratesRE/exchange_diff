using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Migration
{
	internal class MRSAvailabilityLog : ObjectLog<MRSAvailabilityData>
	{
		private MRSAvailabilityLog() : base(new MRSAvailabilityLog.MRSAvailabilityLogSchema(), new MRSAvailabilityLog.MRSAvailabilityLogConfiguration())
		{
		}

		private static MRSAvailabilityLog Instance
		{
			get
			{
				if (MRSAvailabilityLog.instance == null)
				{
					MRSAvailabilityLog.instance = new MRSAvailabilityLog();
				}
				return MRSAvailabilityLog.instance;
			}
		}

		public static void Write(string context, string data)
		{
			MRSAvailabilityData objectToLog = default(MRSAvailabilityData);
			objectToLog.EventContext = context;
			objectToLog.EventData = data;
			MRSAvailabilityLog.Instance.LogObject(objectToLog);
		}

		public static void SetLogEnabledStatus(bool isEnabled)
		{
			MRSAvailabilityLog.MRSAvailabilityLogConfiguration.LoggingEnabled = isEnabled;
		}

		private static MRSAvailabilityLog instance;

		private class MRSAvailabilityLogSchema : ObjectLogSchema
		{
			public override string LogType
			{
				get
				{
					return "MRSAvailability Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<MRSAvailabilityData> EntryType = new ObjectLogSimplePropertyDefinition<MRSAvailabilityData>("Server", (MRSAvailabilityData d) => d.Server);

			public static readonly ObjectLogSimplePropertyDefinition<MRSAvailabilityData> ExecutionGuid = new ObjectLogSimplePropertyDefinition<MRSAvailabilityData>("Version", (MRSAvailabilityData d) => d.Version.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<MRSAvailabilityData> Failure = new ObjectLogSimplePropertyDefinition<MRSAvailabilityData>("EventContext", (MRSAvailabilityData d) => d.EventContext);

			public static readonly ObjectLogSimplePropertyDefinition<MRSAvailabilityData> ExchangeVersion = new ObjectLogSimplePropertyDefinition<MRSAvailabilityData>("EventData", (MRSAvailabilityData d) => d.EventData);
		}

		private class MRSAvailabilityLogConfiguration : ObjectLogConfiguration
		{
			public static bool LoggingEnabled
			{
				get
				{
					return MRSAvailabilityLog.MRSAvailabilityLogConfiguration.loggingEnabled;
				}
				set
				{
					MRSAvailabilityLog.MRSAvailabilityLogConfiguration.loggingEnabled = value;
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return "MRSAvailability_";
				}
			}

			public override bool IsEnabled
			{
				get
				{
					return MRSAvailabilityLog.MRSAvailabilityLogConfiguration.LoggingEnabled;
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "MRSAvailability";
				}
			}

			public override string LoggingFolder
			{
				get
				{
					return MRSAvailabilityLog.MRSAvailabilityLogConfiguration.DefaultLoggingPath;
				}
			}

			public override TimeSpan MaxLogAge
			{
				get
				{
					return TimeSpan.FromDays(30.0);
				}
			}

			public override long MaxLogDirSize
			{
				get
				{
					return 50000000L;
				}
			}

			public override long MaxLogFileSize
			{
				get
				{
					return 500000L;
				}
			}

			private const string LogFilePrefix = "MRSAvailability_";

			private const string MRSAvailabilityLogName = "MRSAvailability";

			private static readonly string DefaultLoggingPath = Path.Combine(ConfigurationContext.Setup.LoggingPath, "MailboxReplicationService", "MRSAvailability");

			public static bool loggingEnabled = true;
		}
	}
}
