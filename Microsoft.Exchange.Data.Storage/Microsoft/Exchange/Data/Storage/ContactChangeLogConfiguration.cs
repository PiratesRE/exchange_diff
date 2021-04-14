using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactChangeLogConfiguration : ILogConfiguration
	{
		public static ContactChangeLogConfiguration Default
		{
			get
			{
				if (ContactChangeLogConfiguration.defaultInstance == null)
				{
					ContactChangeLogConfiguration.defaultInstance = new ContactChangeLogConfiguration();
				}
				return ContactChangeLogConfiguration.defaultInstance;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return ContactChangeLogConfiguration.Enabled.Value;
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
				return this.directoryPath;
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
				return "ContactChangeLogging";
			}
		}

		public string LogType
		{
			get
			{
				return "Contact Change Log";
			}
		}

		public long MaxLogDirectorySizeInBytes
		{
			get
			{
				return (long)ContactChangeLogConfiguration.MaxDirectorySize.Value.ToBytes();
			}
		}

		public long MaxLogFileSizeInBytes
		{
			get
			{
				return (long)ContactChangeLogConfiguration.MaxFileSize.Value.ToBytes();
			}
		}

		public TimeSpan MaxLogAge
		{
			get
			{
				return ContactChangeLogConfiguration.MaxAge.Value;
			}
		}

		private ContactChangeLogConfiguration()
		{
			this.prefix = "ContactChange_" + ApplicationName.Current.UniqueId + "_";
			this.directoryPath = ((ContactChangeLogConfiguration.DirectoryPath.Value != null) ? ContactChangeLogConfiguration.DirectoryPath.Value : Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\ContactChangeLogging\\"));
		}

		private const string Type = "Contact Change Log";

		private const string Component = "ContactChangeLogging";

		private static readonly Trace Tracer = ExTraceGlobals.ContactChangeLoggingTracer;

		private static readonly BoolAppSettingsEntry Enabled = new BoolAppSettingsEntry("ContactChangeLoggingEnabled", true, ContactChangeLogConfiguration.Tracer);

		private static readonly StringAppSettingsEntry DirectoryPath = new StringAppSettingsEntry("ContactChangeLoggingPath", null, ContactChangeLogConfiguration.Tracer);

		private static readonly TimeSpanAppSettingsEntry MaxAge = new TimeSpanAppSettingsEntry("ContactChangeLoggingMaxAge", TimeSpanUnit.Minutes, TimeSpan.FromDays(30.0), ContactChangeLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxDirectorySize = new ByteQuantifiedSizeAppSettingsEntry("ContactChangeLoggingMaxDirectorySize", ByteQuantifiedSize.FromGB(1UL), ContactChangeLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxFileSize = new ByteQuantifiedSizeAppSettingsEntry("ContactChangeLoggingMaxFileSize", ByteQuantifiedSize.FromMB(10UL), ContactChangeLogConfiguration.Tracer);

		private readonly string prefix;

		private readonly string directoryPath;

		private static ContactChangeLogConfiguration defaultInstance;
	}
}
