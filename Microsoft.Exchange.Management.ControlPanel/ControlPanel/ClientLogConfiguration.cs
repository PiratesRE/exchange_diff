using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientLogConfiguration : ILogConfiguration
	{
		public ClientLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("ECPIsClientLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("ECPClientLogPath", ClientLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ECPClientMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ECPClientMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ECPClientMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		internal static ByteQuantifiedSize GetConfigByteQuantifiedSizeValue(string configName, ByteQuantifiedSize defaultValue)
		{
			string expression = null;
			ByteQuantifiedSize result;
			if (AppConfigLoader.TryGetConfigRawValue(configName, out expression) && ByteQuantifiedSize.TryParse(expression, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ClientLogConfiguration.defaultLogPath == null)
				{
					ClientLogConfiguration.defaultLogPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\ECP\\Client");
				}
				return ClientLogConfiguration.defaultLogPath;
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public bool IsActivityEventHandler
		{
			get
			{
				return false;
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
				return "ECPClientLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return ClientLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "ECP Client Log";
			}
		}

		private const string LogTypeValue = "ECP Client Log";

		private const string LogComponentValue = "ECPClientLog";

		private const string DefaultRelativeFilePath = "Logging\\ECP\\Client";

		public static readonly string LogPrefixValue = "ECPClient";

		private static string defaultLogPath;
	}
}
