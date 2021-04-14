using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaServerTraceLogConfiguration : ILogConfiguration
	{
		public OwaServerTraceLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("OWAIsServerTraceLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("OWATraceLogPath", OwaServerTraceLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("OWATraceMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(5.0));
			this.MaxLogDirectorySizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWATraceMaxLogDirectorySize", ByteQuantifiedSize.FromGB(1UL)).ToBytes();
			this.MaxLogFileSizeInBytes = (long)OwaAppConfigLoader.GetConfigByteQuantifiedSizeValue("OWATraceMaxLogFileSize", ByteQuantifiedSize.FromMB(10UL)).ToBytes();
			this.OwaTraceLoggingThreshold = AppConfigLoader.GetConfigDoubleValue("OwaTraceLoggingThreshold", 0.0, 0.0, 0.0);
		}

		public static string DefaultLogPath
		{
			get
			{
				if (OwaServerTraceLogConfiguration.defaultLogPath == null)
				{
					OwaServerTraceLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Owa\\ServerTrace");
				}
				return OwaServerTraceLogConfiguration.defaultLogPath;
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

		public double OwaTraceLoggingThreshold { get; private set; }

		public string LogComponent
		{
			get
			{
				return "OwaServerTraceLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return OwaServerTraceLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "OWA Server Trace Log";
			}
		}

		private const string LogTypeValue = "OWA Server Trace Log";

		private const string LogComponentValue = "OwaServerTraceLog";

		private const string DefaultRelativeFilePath = "Logging\\Owa\\ServerTrace";

		public static readonly string LogPrefixValue = "OwaServerTrace";

		private static string defaultLogPath;
	}
}
