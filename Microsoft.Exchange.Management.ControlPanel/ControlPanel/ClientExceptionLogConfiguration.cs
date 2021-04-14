using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientExceptionLogConfiguration : ILogConfiguration
	{
		public ClientExceptionLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("IsClientExceptionLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("ClientExceptionLogPath", ClientExceptionLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ClientExceptionMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ClientExceptionMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ClientLogConfiguration.GetConfigByteQuantifiedSizeValue("ClientExceptionMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (ClientExceptionLogConfiguration.defaultLogPath == null)
				{
					ClientExceptionLogConfiguration.defaultLogPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\ECP\\ClientException");
				}
				return ClientExceptionLogConfiguration.defaultLogPath;
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
				return "ECPClientExceptionLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return ClientExceptionLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "ECP Client Exception Log";
			}
		}

		private const string LogTypeValue = "ECP Client Exception Log";

		private const string LogComponentValue = "ECPClientExceptionLog";

		private const string DefaultRelativeFilePath = "Logging\\ECP\\ClientException";

		public static readonly string LogPrefixValue = "ECPClientException";

		private static string defaultLogPath;
	}
}
