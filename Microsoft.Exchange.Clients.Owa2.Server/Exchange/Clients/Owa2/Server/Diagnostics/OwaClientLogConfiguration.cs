using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaClientLogConfiguration : ILogConfiguration
	{
		public OwaClientLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("OWAIsClientLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("OWAClientLogPath", OwaClientLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("OWAClientMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAClientMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAClientMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (OwaClientLogConfiguration.defaultLogPath == null)
				{
					OwaClientLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\OWA\\Client");
				}
				return OwaClientLogConfiguration.defaultLogPath;
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
				return "OWAClientLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return OwaClientLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "OWA Client Log";
			}
		}

		private const string LogTypeValue = "OWA Client Log";

		private const string LogComponentValue = "OWAClientLog";

		private const string DefaultRelativeFilePath = "Logging\\OWA\\Client";

		public static readonly string LogPrefixValue = "OWAClient";

		private static string defaultLogPath;
	}
}
