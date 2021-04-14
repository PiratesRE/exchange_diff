using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ServerLogConfiguration : ILogConfiguration
	{
		public ServerLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("ECPIsServerLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("ECPServerLogPath", ServerLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ECPServerMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ECPServerMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ECPServerMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ServerLogConfiguration.defaultLogPath == null)
				{
					ServerLogConfiguration.defaultLogPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\ECP\\Server");
				}
				return ServerLogConfiguration.defaultLogPath;
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
				return "ECPServerLog";
			}
		}

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
				return "ECP Server Log";
			}
		}

		private const string LogTypeValue = "ECP Server Log";

		private const string LogComponentValue = "ECPServerLog";

		private const string DefaultRelativeFilePath = "Logging\\ECP\\Server";

		public static readonly string LogPrefixValue = "ECPServer";

		private static string defaultLogPath;
	}
}
