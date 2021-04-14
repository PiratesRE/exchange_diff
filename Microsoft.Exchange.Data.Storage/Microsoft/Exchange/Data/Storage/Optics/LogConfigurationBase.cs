using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class LogConfigurationBase : ILogConfiguration
	{
		public LogConfigurationBase()
		{
			this.enabled = new BoolAppSettingsEntry(this.Component + "Enabled", true, this.Tracer);
			StringAppSettingsEntry stringAppSettingsEntry = new StringAppSettingsEntry(this.Component + "Path", null, this.Tracer);
			this.logPath = (stringAppSettingsEntry.Value ?? Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\" + this.Component + "\\"));
			this.prefix = this.Component + "_" + ApplicationName.Current.UniqueId + "_";
			this.maxDirectorySize = new ByteQuantifiedSizeAppSettingsEntry(this.Component + "MaxDirectorySize", ByteQuantifiedSize.FromMB(250UL), this.Tracer);
			this.maxFileSize = new ByteQuantifiedSizeAppSettingsEntry(this.Component + "MaxFileSize", ByteQuantifiedSize.FromMB(10UL), this.Tracer);
			this.maxAge = new TimeSpanAppSettingsEntry(this.Component + "MaxAge", TimeSpanUnit.Minutes, TimeSpan.FromDays(30.0), this.Tracer);
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return this.enabled.Value;
			}
		}

		public bool IsActivityEventHandler
		{
			get
			{
				return false;
			}
		}

		public string LogPath
		{
			get
			{
				return this.logPath;
			}
		}

		public string LogPrefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string LogComponent
		{
			get
			{
				return this.Component;
			}
		}

		public string LogType
		{
			get
			{
				return this.Type;
			}
		}

		public long MaxLogDirectorySizeInBytes
		{
			get
			{
				return (long)this.maxDirectorySize.Value.ToBytes();
			}
		}

		public long MaxLogFileSizeInBytes
		{
			get
			{
				return (long)this.maxFileSize.Value.ToBytes();
			}
		}

		public TimeSpan MaxLogAge
		{
			get
			{
				return this.maxAge.Value;
			}
		}

		protected abstract string Component { get; }

		protected abstract string Type { get; }

		protected abstract Trace Tracer { get; }

		private readonly BoolAppSettingsEntry enabled;

		private readonly TimeSpanAppSettingsEntry maxAge;

		private readonly ByteQuantifiedSizeAppSettingsEntry maxDirectorySize;

		private readonly ByteQuantifiedSizeAppSettingsEntry maxFileSize;

		private readonly string prefix;

		private readonly string logPath;
	}
}
