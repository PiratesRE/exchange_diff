using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GroupMessageEscalationLogConfiguration : ILogConfiguration
	{
		private GroupMessageEscalationLogConfiguration()
		{
			this.prefix = "GroupMessageEscalation_" + ApplicationName.Current.UniqueId + "_";
			this.directoryPath = (GroupMessageEscalationLogConfiguration.DirectoryPath.Value ?? Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\GroupMessageEscalationLogs\\"));
		}

		public static GroupMessageEscalationLogConfiguration Default
		{
			get
			{
				if (GroupMessageEscalationLogConfiguration.defaultInstance == null)
				{
					GroupMessageEscalationLogConfiguration.defaultInstance = new GroupMessageEscalationLogConfiguration();
				}
				return GroupMessageEscalationLogConfiguration.defaultInstance;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return GroupMessageEscalationLogConfiguration.Enabled.Value;
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
				return "GroupMessageEscalationLog";
			}
		}

		public string LogType
		{
			get
			{
				return "Group Message Escalation Log";
			}
		}

		public long MaxLogDirectorySizeInBytes
		{
			get
			{
				return (long)GroupMessageEscalationLogConfiguration.MaxDirectorySize.Value.ToBytes();
			}
		}

		public long MaxLogFileSizeInBytes
		{
			get
			{
				return (long)GroupMessageEscalationLogConfiguration.MaxFileSize.Value.ToBytes();
			}
		}

		public TimeSpan MaxLogAge
		{
			get
			{
				return GroupMessageEscalationLogConfiguration.MaxAge.Value;
			}
		}

		private const string Type = "Group Message Escalation Log";

		private const string Component = "GroupMessageEscalationLog";

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly BoolAppSettingsEntry Enabled = new BoolAppSettingsEntry("GroupMessageEscalationLogEnabled", true, GroupMessageEscalationLogConfiguration.Tracer);

		private static readonly StringAppSettingsEntry DirectoryPath = new StringAppSettingsEntry("GroupMessageEscalationLogPath", null, GroupMessageEscalationLogConfiguration.Tracer);

		private static readonly TimeSpanAppSettingsEntry MaxAge = new TimeSpanAppSettingsEntry("GroupMessageEscalationLogLogMaxAge", TimeSpanUnit.Minutes, TimeSpan.FromDays(7.0), GroupMessageEscalationLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxDirectorySize = new ByteQuantifiedSizeAppSettingsEntry("MailboxAssociationLogMaxDirectorySize", ByteQuantifiedSize.FromMB(250UL), GroupMessageEscalationLogConfiguration.Tracer);

		private static readonly ByteQuantifiedSizeAppSettingsEntry MaxFileSize = new ByteQuantifiedSizeAppSettingsEntry("MailboxAssociationLogMaxFileSize", ByteQuantifiedSize.FromMB(10UL), GroupMessageEscalationLogConfiguration.Tracer);

		private static GroupMessageEscalationLogConfiguration defaultInstance;

		private readonly string prefix;

		private readonly string directoryPath;
	}
}
