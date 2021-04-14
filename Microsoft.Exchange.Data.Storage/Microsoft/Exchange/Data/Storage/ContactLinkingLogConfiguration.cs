using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactLinkingLogConfiguration : ILogConfiguration
	{
		private ContactLinkingLogConfiguration()
		{
			this.prefix = "ContactLinking_" + ApplicationName.Current.UniqueId + "_";
			this.directoryPath = ((ContactLinkingLogConfiguration.DirectoryPath.Value != null) ? ContactLinkingLogConfiguration.DirectoryPath.Value : Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\ContactLinkingLogs\\"));
		}

		public static ContactLinkingLogConfiguration Default
		{
			get
			{
				if (ContactLinkingLogConfiguration.defaultInstance == null)
				{
					ContactLinkingLogConfiguration.defaultInstance = new ContactLinkingLogConfiguration();
				}
				return ContactLinkingLogConfiguration.defaultInstance;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return ContactLinkingLogConfiguration.Enabled.Value;
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
				return "ContactLinkingLog";
			}
		}

		public string LogType
		{
			get
			{
				return "Contact Linking Log";
			}
		}

		public long MaxLogDirectorySizeInBytes
		{
			get
			{
				return (long)ContactLinkingLogConfiguration.MaxDirectorySize.Value.ToBytes();
			}
		}

		public long MaxLogFileSizeInBytes
		{
			get
			{
				return (long)ContactLinkingLogConfiguration.MaxFileSize.Value.ToBytes();
			}
		}

		public TimeSpan MaxLogAge
		{
			get
			{
				return ContactLinkingLogConfiguration.MaxAge.Value;
			}
		}

		private const string Type = "Contact Linking Log";

		private const string Component = "ContactLinkingLog";

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private static readonly BoolAppSettingsEntry Enabled = new BoolAppSettingsEntry("ContactLinkingLogEnabled", true, ContactLinkingLogConfiguration.Tracer);

		private static readonly StringAppSettingsEntry DirectoryPath = new StringAppSettingsEntry("ContactLinkingLogPath", null, ContactLinkingLogConfiguration.Tracer);

		private static readonly TimeSpanAppSettingsEntry MaxAge = new TimeSpanAppSettingsEntry("ContactLinkingLogMaxAge", TimeSpanUnit.Minutes, TimeSpan.FromDays(30.0), ContactLinkingLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxDirectorySize = new ByteQuantifiedSizeAppSettingsEntry("ContactLinkingLogMaxDirectorySize", ByteQuantifiedSize.FromMB(250UL), ContactLinkingLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxFileSize = new ByteQuantifiedSizeAppSettingsEntry("ContactLinkingLogMaxFileSize", ByteQuantifiedSize.FromMB(10UL), ContactLinkingLogConfiguration.Tracer);

		private readonly string prefix;

		private readonly string directoryPath;

		private static ContactLinkingLogConfiguration defaultInstance;
	}
}
