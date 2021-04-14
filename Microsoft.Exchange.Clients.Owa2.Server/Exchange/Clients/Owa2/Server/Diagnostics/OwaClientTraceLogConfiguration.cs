using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaClientTraceLogConfiguration : ILogConfiguration
	{
		public OwaClientTraceLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("OWAIsClientTraceLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("OWAClientTraceLogPath", OwaClientTraceLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("OWAClientTraceMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAClientTraceMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWAClientTraceMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (OwaClientTraceLogConfiguration.defaultLogPath == null)
				{
					OwaClientTraceLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\OWA\\ClientTrace");
				}
				return OwaClientTraceLogConfiguration.defaultLogPath;
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
				return "OWAClientTraceLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return OwaClientTraceLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "OWA Client Trace Log";
			}
		}

		private const string LogTypeValue = "OWA Client Trace Log";

		private const string LogComponentValue = "OWAClientTraceLog";

		private const string DefaultRelativeFilePath = "Logging\\OWA\\ClientTrace";

		public static readonly string LogPrefixValue = "OWAClientTrace";

		private static string defaultLogPath;
	}
}
