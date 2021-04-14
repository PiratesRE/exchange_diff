using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkingSet;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WorkingSetPublisherLogConfiguration : ILogConfiguration
	{
		private WorkingSetPublisherLogConfiguration()
		{
			this.prefix = "WorkingSetPublisher_" + ApplicationName.Current.UniqueId + "_";
			this.directoryPath = (WorkingSetPublisherLogConfiguration.DirectoryPath.Value ?? Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\WorkingSetPublisher\\"));
		}

		public static WorkingSetPublisherLogConfiguration Default
		{
			get
			{
				if (WorkingSetPublisherLogConfiguration.defaultInstance == null)
				{
					WorkingSetPublisherLogConfiguration.defaultInstance = new WorkingSetPublisherLogConfiguration();
				}
				return WorkingSetPublisherLogConfiguration.defaultInstance;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return WorkingSetPublisherLogConfiguration.Enabled.Value;
			}
		}

		public bool IsActivityEventHandler
		{
			get
			{
				return false;
			}
		}

		public string LogPath
		{
			get
			{
				return this.directoryPath;
			}
		}

		public string LogPrefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string LogComponent
		{
			get
			{
				return "WorkingSetPublisherLog";
			}
		}

		public string LogType
		{
			get
			{
				return "Working Set Publisher Log";
			}
		}

		public long MaxLogDirectorySizeInBytes
		{
			get
			{
				return (long)WorkingSetPublisherLogConfiguration.MaxDirectorySize.Value.ToBytes();
			}
		}

		public long MaxLogFileSizeInBytes
		{
			get
			{
				return (long)WorkingSetPublisherLogConfiguration.MaxFileSize.Value.ToBytes();
			}
		}

		public TimeSpan MaxLogAge
		{
			get
			{
				return WorkingSetPublisherLogConfiguration.MaxAge.Value;
			}
		}

		private const string Type = "Working Set Publisher Log";

		private const string Component = "WorkingSetPublisherLog";

		private static readonly Trace Tracer = ExTraceGlobals.WorkingSetPublisherTracer;

		private static readonly BoolAppSettingsEntry Enabled = new BoolAppSettingsEntry("WorkingSetPublisherLogEnabled", true, WorkingSetPublisherLogConfiguration.Tracer);

		private static readonly StringAppSettingsEntry DirectoryPath = new StringAppSettingsEntry("WorkingSetPublisherLogPath", null, WorkingSetPublisherLogConfiguration.Tracer);

		private static readonly TimeSpanAppSettingsEntry MaxAge = new TimeSpanAppSettingsEntry("WorkingSetPublisherLogMaxAge", TimeSpanUnit.Minutes, TimeSpan.FromDays(7.0), WorkingSetPublisherLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxDirectorySize = new ByteQuantifiedSizeAppSettingsEntry("WorkingSetPublisherLogMaxDirectorySize", ByteQuantifiedSize.FromMB(250UL), WorkingSetPublisherLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxFileSize = new ByteQuantifiedSizeAppSettingsEntry("WorkingSetPublisherLogMaxFileSize", ByteQuantifiedSize.FromMB(10UL), WorkingSetPublisherLogConfiguration.Tracer);

		private static WorkingSetPublisherLogConfiguration defaultInstance;

		private readonly string prefix;

		private readonly string directoryPath;
	}
}
