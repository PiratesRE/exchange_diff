using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ActivityContextLogConfiguration : ILogConfiguration
	{
		public ActivityContextLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("IsActivityContextLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("ActivityContextLogPath", ActivityContextLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ActivityContextMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ActivityContextMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ActivityContextMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ActivityContextLogConfiguration.defaultLogPath == null)
				{
					ActivityContextLogConfiguration.defaultLogPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\ECP\\Activity");
				}
				return ActivityContextLogConfiguration.defaultLogPath;
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public bool IsActivityEventHandler
		{
			get
			{
				return true;
			}
		}

		public string LogPath { get; private set; }

		public TimeSpan MaxLogAge { get; private set; }

		public long MaxLogDirectorySizeInBytes { get; private set; }

		public long MaxLogFileSizeInBytes { get; private set; }

		public string LogComponent
		{
			get
			{
				return "ECPActivityContextLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return ActivityContextLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "ECP Activity Context Log";
			}
		}

		private const string LogTypeValue = "ECP Activity Context Log";

		private const string LogComponentValue = "ECPActivityContextLog";

		private const string DefaultRelativeFilePath = "Logging\\ECP\\Activity";

		public static readonly string LogPrefixValue = "ECPActivity";

		private static string defaultLogPath;
	}
}
