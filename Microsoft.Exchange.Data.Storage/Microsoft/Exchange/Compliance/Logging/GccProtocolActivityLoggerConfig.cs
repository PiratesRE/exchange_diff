using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Win32;

namespace Microsoft.Exchange.Compliance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GccProtocolActivityLoggerConfig
	{
		public GccProtocolActivityLoggerConfig() : this(GccProtocolActivityLoggerConfig.ParseConfigValue(ConfigurationManager.AppSettings, "GccDisabled", true), null)
		{
		}

		internal GccProtocolActivityLoggerConfig(bool disabled, string logRoot)
		{
			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			this.disabled = disabled;
			this.maxAge = GccProtocolActivityLoggerConfig.ParseConfigValue(appSettings, "GccLogMaxAge", GccProtocolActivityLoggerConfig.DefaultMaxAge);
			this.maxDirectorySize = GccProtocolActivityLoggerConfig.ParseConfigValue(appSettings, "GccLogMaxDirectorySize", long.MaxValue);
			this.maxLogfileSize = GccProtocolActivityLoggerConfig.ParseConfigValue(appSettings, "GccLogMaxLogfileSize", long.MaxValue);
			this.reportIntervalMilliseconds = GccProtocolActivityLoggerConfig.ParseConfigValue(appSettings, "GccReportIntervalMilliseconds", 60000.0);
			if (logRoot == null)
			{
				try
				{
					this.logRoot = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15", "GccLogRoot", null) ?? string.Empty);
					goto IL_B5;
				}
				catch
				{
					this.logRoot = string.Empty;
					goto IL_B5;
				}
			}
			this.logRoot = logRoot;
			IL_B5:
			if (!this.disabled && this.logRoot.Length == 0)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "No GccLogRoot regkey was found - Criminal Compliance logging disabled");
				this.disabled = true;
			}
			try
			{
				if (!this.disabled && !Path.IsPathRooted(this.logRoot))
				{
					ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "GccLogRoot contains a relative path, must be absolute - Criminal Compliance logging disabled");
					this.disabled = true;
				}
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "GccLogRoot contains invalid path characters - Criminal Compliance logging disabled");
				this.disabled = true;
			}
		}

		public string LogRoot
		{
			get
			{
				return this.logRoot;
			}
		}

		public TimeSpan MaxAge
		{
			get
			{
				return this.maxAge;
			}
		}

		public long MaxDirectorySize
		{
			get
			{
				return this.maxDirectorySize;
			}
		}

		public long MaxLogfileSize
		{
			get
			{
				return this.maxLogfileSize;
			}
		}

		public double ReportIntervalMilliseconds
		{
			get
			{
				return this.reportIntervalMilliseconds;
			}
		}

		public bool Disabled
		{
			get
			{
				return this.disabled;
			}
		}

		private static bool ParseConfigValue(NameValueCollection values, string name, bool defaultValue)
		{
			string value = values[name];
			bool result = defaultValue;
			if (!string.IsNullOrEmpty(value))
			{
				bool.TryParse(value, out result);
			}
			return result;
		}

		private static long ParseConfigValue(NameValueCollection values, string name, long defaultValue)
		{
			string text = values[name];
			long result = defaultValue;
			if (!string.IsNullOrEmpty(text))
			{
				long.TryParse(text, out result);
			}
			return result;
		}

		private static double ParseConfigValue(NameValueCollection values, string name, double defaultValue)
		{
			string text = values[name];
			double result = defaultValue;
			if (!string.IsNullOrEmpty(text))
			{
				double.TryParse(text, out result);
			}
			return result;
		}

		private static TimeSpan ParseConfigValue(NameValueCollection values, string name, TimeSpan defaultValue)
		{
			string text = values[name];
			TimeSpan result = defaultValue;
			if (!string.IsNullOrEmpty(text))
			{
				TimeSpan.TryParse(text, out result);
			}
			return result;
		}

		private const string Exchange14RootRegKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string GccLogRootRegValue = "GccLogRoot";

		private const string GccDisabledSetting = "GccDisabled";

		private const string GccMaxAgeSetting = "GccLogMaxAge";

		private const string MaxDirectorySizeSetting = "GccLogMaxDirectorySize";

		private const string MaxLogfileSizeSetting = "GccLogMaxLogfileSize";

		private const string ReportIntervalMillisecondsSetting = "GccReportIntervalMilliseconds";

		private const bool DefaultDisabled = true;

		private const long DefaultMaxDirectorySize = 9223372036854775807L;

		private const long DefaultMaxLogfileSize = 9223372036854775807L;

		private const double DefaultReportIntervalMilliseconds = 60000.0;

		private static readonly TimeSpan DefaultMaxAge = TimeSpan.MaxValue;

		private bool disabled;

		private string logRoot;

		private TimeSpan maxAge;

		private long maxDirectorySize;

		private long maxLogfileSize;

		private double reportIntervalMilliseconds;
	}
}
