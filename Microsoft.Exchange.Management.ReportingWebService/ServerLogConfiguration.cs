using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal sealed class ServerLogConfiguration : ILogConfiguration
	{
		public ServerLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("RWSIsServerLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("RWSServerLogPath", ServerLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("RWSServerMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ServerLogConfiguration.GetConfigByteQuantifiedSizeValue("RWSServerMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ServerLogConfiguration.GetConfigByteQuantifiedSizeValue("RWSServerMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ServerLogConfiguration.defaultLogPath == null)
				{
					ServerLogConfiguration.defaultLogPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\RWSServer");
				}
				return ServerLogConfiguration.defaultLogPath;
			}
		}

		public bool IsActivityEventHandler
		{
			get
			{
				return false;
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public string LogComponent
		{
			get
			{
				return "RWSServerLog";
			}
		}

		public string LogPath { get; private set; }

		public string LogPrefix
		{
			get
			{
				return ServerLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "RWS Server Log";
			}
		}

		public TimeSpan MaxLogAge { get; private set; }

		public long MaxLogDirectorySizeInBytes { get; private set; }

		public long MaxLogFileSizeInBytes { get; private set; }

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

		private const string LogTypeValue = "RWS Server Log";

		private const string LogComponentValue = "RWSServerLog";

		private const string DefaultRelativeFilePath = "Logging\\RWSServer";

		public static readonly string LogPrefixValue = "RWSServer";

		private static string defaultLogPath;
	}
}
