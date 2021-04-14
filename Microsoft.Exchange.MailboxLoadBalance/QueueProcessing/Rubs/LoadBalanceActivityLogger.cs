using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MigrationWorkflowService;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.MailboxLoadBalance.Config;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing.Rubs
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceActivityLogger : ActivityContextLogger
	{
		protected override string FileNamePrefix
		{
			get
			{
				return this.logConfiguration.Value.FilenamePrefix;
			}
		}

		protected override string LogComponentName
		{
			get
			{
				return this.logConfiguration.Value.LogComponentName;
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "Mailbox Load Balance Activities";
			}
		}

		protected override int TimestampField
		{
			get
			{
				return 0;
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MigrationWorkflowServiceTracer;
			}
		}

		protected override string[] GetLogFields()
		{
			return Enum.GetNames(typeof(LoadBalanceActivityLogger.LogFields));
		}

		protected override ActivityContextLogFileSettings GetLogFileSettings()
		{
			return this.logConfiguration.Value;
		}

		protected override void InternalLogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(base.LogSchema);
			logRowFormatter[1] = activityScope.ActivityType;
			logRowFormatter[2] = ActivityContextLogger.ActivityEventTypeDictionary[eventType];
			logRowFormatter[3] = activityScope.Status;
			logRowFormatter[4] = activityScope.StartTime;
			logRowFormatter[5] = activityScope.EndTime;
			logRowFormatter[6] = WorkloadManagementLogger.FormatWlmActivity(activityScope, true);
			base.AppendLog(logRowFormatter);
		}

		private const string LoadBalanceActivityLogTypeName = "Mailbox Load Balance Activities";

		private readonly Lazy<LoadBalanceActivityLogger.ContextLogSettings> logConfiguration = new Lazy<LoadBalanceActivityLogger.ContextLogSettings>(() => new LoadBalanceActivityLogger.ContextLogSettings());

		private enum LogFields
		{
			Timestamp,
			Activity,
			EventType,
			Status,
			StartTime,
			EndTime,
			CustomData
		}

		private class ContextLogSettings : ActivityContextLogFileSettings
		{
			public string FilenamePrefix
			{
				get
				{
					return this.logConfig.Value.FilenamePrefix;
				}
			}

			public string LogComponentName
			{
				get
				{
					return this.logConfig.Value.LogComponentName;
				}
			}

			protected override string LogSubFolderName
			{
				get
				{
					return "Activity";
				}
			}

			protected override string LogTypeName
			{
				get
				{
					return "Mailbox Load Balance Activities";
				}
			}

			protected override Trace Tracer
			{
				get
				{
					return ExTraceGlobals.MailboxLoadBalanceTracer;
				}
			}

			protected override void LoadCustomSettings()
			{
				base.DirectoryPath = this.logConfig.Value.LoggingFolder;
				base.MaxDirectorySize = ByteQuantifiedSize.FromBytes((ulong)this.logConfig.Value.MaxLogDirSize);
				base.MaxFileSize = ByteQuantifiedSize.FromBytes((ulong)this.logConfig.Value.MaxLogFileSize);
				base.MaxAge = this.logConfig.Value.MaxLogAge;
			}

			private readonly Lazy<LoadBalanceLoggingConfig> logConfig = new Lazy<LoadBalanceLoggingConfig>(() => new LoadBalanceLoggingConfig("MailboxLoadBalanceActivity"));
		}
	}
}
