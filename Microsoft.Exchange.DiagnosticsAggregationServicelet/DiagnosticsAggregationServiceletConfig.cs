using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal class DiagnosticsAggregationServiceletConfig
	{
		public DiagnosticsAggregationServiceletConfig()
		{
			this.Enabled = AppConfigLoader.GetConfigBoolValue("DiagnosticsAggregationServiceletEnabled", true);
			this.TimeSpanForQueueDataBeingCurrent = AppConfigLoader.GetConfigTimeSpanValue("TimeSpanForQueueDataBeingCurrent", TimeSpan.FromMinutes(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(11.0));
			this.TimeSpanForQueueDataBeingStale = AppConfigLoader.GetConfigTimeSpanValue("TimeSpanForQueueDataBeingStale", TimeSpan.FromMinutes(1.0), TimeSpan.FromHours(10.0), TimeSpan.FromHours(1.0));
			this.LoggingEnabled = AppConfigLoader.GetConfigBoolValue("DiagnosticsAggregationLoggingEnabled", false);
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string path = Path.Combine(Directory.GetParent(Path.GetDirectoryName(executingAssembly.Location)).FullName, "TransportRoles\\Logs\\DiagnosticsAggregation\\");
			this.LogFileDirectoryPath = AppConfigLoader.GetConfigStringValue("DiagnosticsAggregationLogFileDirectoryPath", Path.GetFullPath(path));
			int configIntValue = AppConfigLoader.GetConfigIntValue("DiagnosticsAggregationLogFileMaxSizeInMB", 1, 10000, 2);
			this.LogFileMaxSize = ByteQuantifiedSize.FromMB((ulong)((long)configIntValue));
			int configIntValue2 = AppConfigLoader.GetConfigIntValue("DiagnosticsAggregationLogFileMaxDirectorySizeInMB", 1, 10000, 10);
			this.LogFileMaxDirectorySize = ByteQuantifiedSize.FromMB((ulong)((long)configIntValue2));
			this.LogFileMaxAge = AppConfigLoader.GetConfigTimeSpanValue("DiagnosticsAggregationLogFileMaxAge", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(365.0), TimeSpan.FromDays(15.0));
		}

		public bool Enabled { get; private set; }

		public TimeSpan TimeSpanForQueueDataBeingCurrent { get; private set; }

		public TimeSpan TimeSpanForQueueDataBeingStale { get; private set; }

		public bool LoggingEnabled { get; private set; }

		public string LogFileDirectoryPath { get; private set; }

		public ByteQuantifiedSize LogFileMaxSize { get; private set; }

		public ByteQuantifiedSize LogFileMaxDirectorySize { get; private set; }

		public TimeSpan LogFileMaxAge { get; private set; }

		private const string DiagnosticsAggregationServiceletEnabledString = "DiagnosticsAggregationServiceletEnabled";

		private const string TimeSpanForQueueDataBeingCurrentString = "TimeSpanForQueueDataBeingCurrent";

		private const string TimeSpanForQueueDataBeingStaleString = "TimeSpanForQueueDataBeingStale";

		private const string LoggingEnabledString = "DiagnosticsAggregationLoggingEnabled";

		private const string LogFileDirectoryPathString = "DiagnosticsAggregationLogFileDirectoryPath";

		private const string LogFileMaxSizeString = "DiagnosticsAggregationLogFileMaxSizeInMB";

		private const string LogFileMaxDirectorySizeString = "DiagnosticsAggregationLogFileMaxDirectorySizeInMB";

		private const string LogFileMaxAgeString = "DiagnosticsAggregationLogFileMaxAge";
	}
}
