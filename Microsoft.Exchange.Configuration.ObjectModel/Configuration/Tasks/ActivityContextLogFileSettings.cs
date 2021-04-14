using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal abstract class ActivityContextLogFileSettings
	{
		internal bool Enabled { get; set; }

		internal string DirectoryPath { get; set; }

		internal TimeSpan MaxAge { get; set; }

		internal ByteQuantifiedSize MaxDirectorySize { get; set; }

		internal ByteQuantifiedSize MaxFileSize { get; set; }

		internal ByteQuantifiedSize CacheSize { get; set; }

		internal TimeSpan FlushInterval { get; set; }

		internal bool FlushToDisk { get; set; }

		protected abstract Trace Tracer { get; }

		protected abstract string LogTypeName { get; }

		protected abstract string LogSubFolderName { get; }

		protected virtual bool DefaultEnabled
		{
			get
			{
				return true;
			}
		}

		protected ActivityContextLogFileSettings()
		{
			this.LoadSettings();
		}

		protected virtual void LoadCustomSettings()
		{
		}

		protected virtual void LoadSettings()
		{
			this.Tracer.TraceDebug<string>(0L, "Start loading {0} settings.", this.LogTypeName);
			BoolAppSettingsEntry boolAppSettingsEntry = new BoolAppSettingsEntry("LogEnabled", this.DefaultEnabled, this.Tracer);
			this.Enabled = boolAppSettingsEntry.Value;
			StringAppSettingsEntry stringAppSettingsEntry = new StringAppSettingsEntry("LogDirectoryPath", Path.Combine(ExchangeSetupContext.LoggingPath, this.LogSubFolderName), this.Tracer);
			this.DirectoryPath = stringAppSettingsEntry.Value;
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry("LogFileAgeInDays", TimeSpanUnit.Days, TimeSpan.FromDays(30.0), this.Tracer);
			this.MaxAge = timeSpanAppSettingsEntry.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry = new ByteQuantifiedSizeAppSettingsEntry("LogDirectorySizeLimit", ByteQuantifiedSize.Parse("100MB"), this.Tracer);
			this.MaxDirectorySize = byteQuantifiedSizeAppSettingsEntry.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry2 = new ByteQuantifiedSizeAppSettingsEntry("LogFileSizeLimit", ByteQuantifiedSize.Parse("10MB"), this.Tracer);
			this.MaxFileSize = byteQuantifiedSizeAppSettingsEntry2.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry3 = new ByteQuantifiedSizeAppSettingsEntry("LogCacheSizeLimit", ByteQuantifiedSize.Parse("2MB"), this.Tracer);
			this.CacheSize = byteQuantifiedSizeAppSettingsEntry3.Value;
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry2 = new TimeSpanAppSettingsEntry("LogFlushIntervalInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(60.0), this.Tracer);
			this.FlushInterval = timeSpanAppSettingsEntry2.Value;
			this.FlushToDisk = true;
			this.LoadCustomSettings();
			this.Tracer.TraceDebug<string>(0L, "{0} settings are loaded successfully.", this.LogTypeName);
		}
	}
}
