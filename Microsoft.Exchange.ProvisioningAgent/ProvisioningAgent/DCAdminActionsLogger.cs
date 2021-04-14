using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class DCAdminActionsLogger : ActivityContextLogger
	{
		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AdminAuditLogTracer;
			}
		}

		internal static DCAdminActionsLogger Instance
		{
			get
			{
				if (DCAdminActionsLogger.instance == null)
				{
					DCAdminActionsLogger.instance = new DCAdminActionsLogger();
				}
				return DCAdminActionsLogger.instance;
			}
		}

		private DCAdminActionsLogger()
		{
		}

		protected override string LogComponentName
		{
			get
			{
				return "DCAdminActions";
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "Admin Audit Logs for DC Admin Actions";
			}
		}

		protected override string FileNamePrefix
		{
			get
			{
				return "DCAdminActions";
			}
		}

		protected override int TimestampField
		{
			get
			{
				return 0;
			}
		}

		public static void Start()
		{
			if (DCAdminActionsLogger.instance == null)
			{
				DCAdminActionsLogger.instance = new DCAdminActionsLogger();
			}
		}

		public static void Stop()
		{
			if (DCAdminActionsLogger.instance != null)
			{
				DCAdminActionsLogger.instance.Dispose();
				DCAdminActionsLogger.instance = null;
			}
		}

		public static void Flush()
		{
			if (DCAdminActionsLogger.instance != null)
			{
				DCAdminActionsLogger.instance.FlushLog();
			}
		}

		internal static void LogDCAdminAction(Guid activityId, List<KeyValuePair<string, object>> customData)
		{
			DCAdminActionsLogger.InternalLogRow(activityId, customData);
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
				DCAdminActionsLogger.instance.SafeTraceDebug(0L, "Skip logging ActivityEvent '{0}'.", new object[]
				{
					eventType
				});
				return;
			}
			DCAdminActionsLogger.InternalLogRow(scope.ActivityId, customData);
		}

		protected override string[] GetLogFields()
		{
			return Enum.GetNames(typeof(DCAdminActionsLogFields));
		}

		protected override ActivityContextLogFileSettings GetLogFileSettings()
		{
			return DCAdminActionsLoggerSettings.Load();
		}

		private static void InternalLogRow(Guid activityId, List<KeyValuePair<string, object>> customData)
		{
			if (DCAdminActionsLogger.instance == null)
			{
				return;
			}
			if (!DCAdminActionsLogger.instance.Enabled)
			{
				DCAdminActionsLogger.instance.SafeTraceDebug(0L, "DCAdminAtionsLogger log is disabled, skip writing to the log file.", new object[0]);
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(DCAdminActionsLogger.instance.LogSchema);
			if (DCAdminActionsLogger.instance.IsDebugTraceEnabled)
			{
				string text = string.Empty;
				if (customData != null)
				{
					bool flag;
					text = LogRowFormatter.FormatCollection(customData, out flag);
				}
				DCAdminActionsLogger.instance.SafeTraceDebug(0L, "Start writing row to MRS log: ServerName='{0}', ActivityId='{1}', CustomData='{2}'", new object[]
				{
					DCAdminActionsLogger.instance.ServerName,
					activityId,
					text
				});
			}
			logRowFormatter[1] = activityId.ToString("D");
			logRowFormatter[2] = customData;
			DCAdminActionsLogger.instance.AppendLog(logRowFormatter);
			DCAdminActionsLogger.instance.SafeTraceDebug(0L, "The above row is written to MRS log successfully.", new object[0]);
		}

		internal const string LoggerFileNamePrefix = "DCAdminActions";

		internal const string LoggerComponentName = "DCAdminActions";

		internal const string LoggerTypeName = "Admin Audit Logs for DC Admin Actions";

		private static DCAdminActionsLogger instance;
	}
}
