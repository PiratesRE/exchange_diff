using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class MrsAndProxyActivityLogger : ActivityContextLogger
	{
		private MrsAndProxyActivityLogger()
		{
		}

		protected override string LogComponentName
		{
			get
			{
				return WorkloadType.MailboxReplicationService.ToString();
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "Mailbox Replication Log";
			}
		}

		protected override string FileNamePrefix
		{
			get
			{
				return "MRS";
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
				return ExTraceGlobals.MailboxReplicationResourceHealthTracer;
			}
		}

		public static MrsAndProxyActivityLogger Start()
		{
			MrsAndProxyActivityLogger.instance = new MrsAndProxyActivityLogger();
			MrsAndProxyActivityLogger.InternalLogRow(MrsAndProxyActivityLogger.instance.id, "ServiceStart", null, null, null, null);
			return MrsAndProxyActivityLogger.instance;
		}

		public static void Stop()
		{
			if (MrsAndProxyActivityLogger.instance != null)
			{
				MrsAndProxyActivityLogger.InternalLogRow(MrsAndProxyActivityLogger.instance.id, "ServiceStop", null, null, null, null);
				MrsAndProxyActivityLogger.instance.Dispose();
				MrsAndProxyActivityLogger.instance = null;
			}
		}

		public static void Flush()
		{
			if (MrsAndProxyActivityLogger.instance != null)
			{
				MrsAndProxyActivityLogger.instance.FlushLog();
			}
		}

		protected override void InternalLogActivityEvent(IActivityScope scope, ActivityEventType eventType)
		{
			List<KeyValuePair<string, object>> customData = null;
			switch (eventType)
			{
			case ActivityEventType.StartActivity:
			case ActivityEventType.ResumeActivity:
				break;
			case ActivityEventType.SuspendActivity:
			case ActivityEventType.EndActivity:
				customData = WorkloadManagementLogger.FormatWlmActivity(scope, true);
				break;
			default:
				MrsAndProxyActivityLogger.instance.SafeTraceDebug(0L, "Skip logging ActivityEvent '{0}'.", new object[]
				{
					eventType
				});
				return;
			}
			MrsAndProxyActivityLogger.InternalLogRow(scope.ActivityId, ActivityContextLogger.ActivityEventTypeDictionary[eventType], scope.Action, scope.UserId, scope.ClientInfo, customData);
		}

		protected override string[] GetLogFields()
		{
			return Enum.GetNames(typeof(MrsAndProxyActivityContextLogFields));
		}

		protected override ActivityContextLogFileSettings GetLogFileSettings()
		{
			return MrsAndProxyLoggerSettings.Load();
		}

		private static void InternalLogRow(Guid activityId, string eventType, string action, string mailboxId, string mrsServerName, List<KeyValuePair<string, object>> customData)
		{
			if (MrsAndProxyActivityLogger.instance == null)
			{
				return;
			}
			if (!MrsAndProxyActivityLogger.instance.Enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(MrsAndProxyActivityLogger.instance.LogSchema);
			if (MrsAndProxyActivityLogger.instance.IsDebugTraceEnabled)
			{
				string text = string.Empty;
				if (customData != null)
				{
					bool flag;
					text = LogRowFormatter.FormatCollection(customData, out flag);
				}
				MrsAndProxyActivityLogger.instance.SafeTraceDebug(0L, "Adding row to MRS log: ServerName='{0}', ActivityId='{1}', Event='{2}', Action= '{3}', Mailbox='{4}', CustomData='{5}'", new object[]
				{
					MrsAndProxyActivityLogger.instance.ServerName,
					activityId,
					eventType,
					action,
					mailboxId,
					text
				});
			}
			logRowFormatter[2] = activityId.ToString("D");
			logRowFormatter[1] = mrsServerName;
			logRowFormatter[3] = eventType;
			logRowFormatter[4] = action;
			logRowFormatter[5] = mailboxId;
			logRowFormatter[6] = customData;
			MrsAndProxyActivityLogger.instance.AppendLog(logRowFormatter);
		}

		internal const string MrsProxyClassName = "MailboxReplicationProxyService";

		internal const string LoggerFileNamePrefix = "MRS";

		internal const string LoggerTypeName = "Mailbox Replication Log";

		private const string ServiceStart = "ServiceStart";

		private const string ServiceStop = "ServiceStop";

		private static MrsAndProxyActivityLogger instance;

		private readonly Guid id = Guid.NewGuid();
	}
}
