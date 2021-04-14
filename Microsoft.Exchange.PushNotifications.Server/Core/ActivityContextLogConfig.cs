using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal sealed class ActivityContextLogConfig : ILogConfiguration
	{
		public ActivityContextLogConfig()
		{
			this.IsLoggingEnabled = ActivityContextLogConfig.IsActivityContextLogEnabled();
			this.LogPath = AppConfigLoader.GetConfigStringValue("ActivityContextLogPath", ActivityContextLogConfig.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ActivityContextMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(7.0));
			this.MaxLogDirectorySizeInBytes = (long)ActivityContextLogConfig.GetConfigByteQuantifiedSizeValue("ActivityContextMaxDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ActivityContextLogConfig.GetConfigByteQuantifiedSizeValue("ActivityContextMaxLogSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ActivityContextLogConfig.defaultLogPath == null)
				{
					ActivityContextLogConfig.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\PushNotifications");
				}
				return ActivityContextLogConfig.defaultLogPath;
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
				return "PushNotificationLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return "PushNotification";
			}
		}

		public string LogType
		{
			get
			{
				return "PushNotification Server Log";
			}
		}

		internal static bool IsActivityContextLogEnabled()
		{
			return AppConfigLoader.GetConfigBoolValue("ActivityContextLoggingEnabled", false);
		}

		private static ByteQuantifiedSize GetConfigByteQuantifiedSizeValue(string configName, ByteQuantifiedSize defaultValue)
		{
			string expression = null;
			ByteQuantifiedSize result;
			if (AppConfigLoader.TryGetConfigRawValue(configName, out expression) && ByteQuantifiedSize.TryParse(expression, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public const string ActivityContextLoggingEnabled = "ActivityContextLoggingEnabled";

		public const string ActivityContextLogPath = "ActivityContextLogPath";

		public const string ActivityContextMaxLogAge = "ActivityContextMaxLogAge";

		public const string ActivityContextMaxDirectorySize = "ActivityContextMaxDirectorySize";

		public const string ActivityContextMaxLogSize = "ActivityContextMaxLogSize";

		public const string LogPrefixValue = "PushNotification";

		private const string LogTypeValue = "PushNotification Server Log";

		private const string LogComponentValue = "PushNotificationLog";

		private const string DefaultRelativeFilePath = "Logging\\PushNotifications";

		private static string defaultLogPath;
	}
}
