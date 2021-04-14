using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaServerLogConfiguration : ILogConfiguration
	{
		public OwaServerLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("OWAIsLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("OWALogPath", OwaServerLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("OWAMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (OwaServerLogConfiguration.defaultLogPath == null)
				{
					OwaServerLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\OWA\\Server");
				}
				return OwaServerLogConfiguration.defaultLogPath;
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
				return "OWAServerLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return OwaServerLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "OWA Server Log";
			}
		}

		private const string LogTypeValue = "OWA Server Log";

		private const string LogComponentValue = "OWAServerLog";

		private const string DefaultRelativeFilePath = "Logging\\OWA\\Server";

		public static readonly string LogPrefixValue = "OWAServer";

		private static string defaultLogPath;
	}
}
