using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal static class MailboxAssistantsSlaReportLogFactory
	{
		public static MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog GetLogInstance(string logName, SlaLogType logType)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("logName", logName);
			logName = Regex.Replace(logName, "\\s+", string.Empty);
			switch (logType)
			{
			case SlaLogType.MailboxSlaLog:
				return MailboxAssistantsSlaReportLogFactory.GetLogInstance(logName, logType, MailboxAssistantsSlaReportLogFactory.mailboxSlaLogs);
			case SlaLogType.DatabaseSlaLog:
				return MailboxAssistantsSlaReportLogFactory.GetLogInstance(logName, logType, MailboxAssistantsSlaReportLogFactory.databaseSlaLogs);
			default:
				return null;
			}
		}

		public static void StopAll()
		{
			MailboxAssistantsSlaReportLogFactory.Stop(MailboxAssistantsSlaReportLogFactory.mailboxSlaLogs);
			MailboxAssistantsSlaReportLogFactory.Stop(MailboxAssistantsSlaReportLogFactory.databaseSlaLogs);
		}

		private static MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog GetLogInstance(string logName, SlaLogType logType, Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog> logs)
		{
			MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog result;
			lock (logs)
			{
				if (!logs.ContainsKey(logName))
				{
					switch (logType)
					{
					case SlaLogType.MailboxSlaLog:
						logs[logName] = new MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog(logName);
						break;
					case SlaLogType.DatabaseSlaLog:
						logs[logName] = new MailboxAssistantsSlaReportLogFactory.MailboxAssistantsDatabaseSlaLog(logName);
						break;
					}
				}
				result = logs[logName];
			}
			return result;
		}

		private static void Stop(Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog> logs)
		{
			lock (logs)
			{
				foreach (KeyValuePair<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog> keyValuePair in logs)
				{
					if (keyValuePair.Value != null)
					{
						keyValuePair.Value.FlushAndDispose();
					}
				}
				logs.Clear();
			}
		}

		private static readonly Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog> mailboxSlaLogs = new Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog>();

		private static readonly Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog> databaseSlaLogs = new Dictionary<string, MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog>();

		internal class MailboxAssistantsSlaReportLog : ActivityContextLogger
		{
			public MailboxAssistantsSlaReportLog(string logName) : base(logName)
			{
				ArgumentValidator.ThrowIfNullOrWhiteSpace("logName", logName);
				this.logName = logName;
			}

			protected override string FileNamePrefix
			{
				get
				{
					return this.logName;
				}
			}

			protected override string LogComponentName
			{
				get
				{
					return "MailboxAssistantsSlaReportLog";
				}
			}

			protected override string LogTypeName
			{
				get
				{
					return "MailboxAssistantsSlaReportLog";
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
					return ExTraceGlobals.AssistantBaseTracer;
				}
			}

			public void FlushAndDispose()
			{
				base.FlushLog();
				this.Dispose();
			}

			internal void LogMailboxEvent(string assistantName, string databaseName, string jobId, MailboxSlaRequestType requestType, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, MailboxSlaEventType eventType, MailboxSlaFilterReasonType reason = MailboxSlaFilterReasonType.None, Exception exception = null)
			{
				if (!this.ShouldLog())
				{
					return;
				}
				LogRowFormatter logRowFormatter = new LogRowFormatter(base.LogSchema);
				logRowFormatter[1] = this.ServerName;
				logRowFormatter[2] = (assistantName ?? string.Empty);
				logRowFormatter[3] = (databaseName ?? string.Empty);
				logRowFormatter[4] = (jobId ?? string.Empty);
				logRowFormatter[5] = ((requestType == MailboxSlaRequestType.Unknown) ? string.Empty : requestType.ToString());
				logRowFormatter[6] = ((mailboxGuid == Guid.Empty) ? string.Empty : mailboxGuid.ToString("D"));
				logRowFormatter[7] = eventType.ToString();
				logRowFormatter[8] = ((reason == MailboxSlaFilterReasonType.None) ? string.Empty : reason.ToString());
				logRowFormatter[9] = ((exception != null) ? exception.GetType().ToString() : string.Empty);
				logRowFormatter[10] = ((exception != null && exception.InnerException != null) ? exception.InnerException.GetType().ToString() : string.Empty);
				base.AppendLog(logRowFormatter);
				if (base.IsDebugTraceEnabled)
				{
					string message = string.Format("Assistant: {0}, Server: {1}, Database: {2}, WindowJob: {3}, Request: {4}, Mailbox: {5}, Event: {6}, Reason: {7}, Exception: {8}", new object[]
					{
						assistantName,
						this.ServerName,
						databaseName ?? string.Empty,
						jobId ?? string.Empty,
						requestType,
						string.IsNullOrEmpty(mailboxDisplayNameTracingOnlyUsage) ? mailboxGuid.ToString("D") : mailboxDisplayNameTracingOnlyUsage,
						eventType,
						(reason == MailboxSlaFilterReasonType.None) ? string.Empty : reason.ToString(),
						(exception != null) ? exception.Message : string.Empty
					});
					base.SafeTraceDebug((long)this.GetHashCode(), message, new object[0]);
				}
			}

			protected override string[] GetLogFields()
			{
				return Enum.GetNames(typeof(MailboxSlaReportLogFields));
			}

			protected override ActivityContextLogFileSettings GetLogFileSettings()
			{
				return new MailboxAssistantsSlaReportLogFileSettings();
			}

			protected override void InternalLogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
			{
			}

			protected bool ShouldLog()
			{
				if (base.Enabled)
				{
					return true;
				}
				base.SafeTraceDebug((long)this.GetHashCode(), "MailboxAssistantsSlaReportLog is disabled, skip writing to the log file.", new object[0]);
				return false;
			}

			private readonly string logName;
		}

		internal class MailboxAssistantsDatabaseSlaLog : MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog
		{
			public MailboxAssistantsDatabaseSlaLog(string logName) : base(logName)
			{
			}

			protected override string LogComponentName
			{
				get
				{
					return "MailboxAssistantsDatabaseSlaLog";
				}
			}

			protected override string LogTypeName
			{
				get
				{
					return "MailboxAssistantsDatabaseSlaLog";
				}
			}

			protected override int TimestampField
			{
				get
				{
					return 0;
				}
			}

			internal void LogDatabaseEvent(string assistantName, string databaseName, DatabaseSlaEventType eventType, Exception exception = null)
			{
				if (!base.ShouldLog())
				{
					return;
				}
				LogRowFormatter logRowFormatter = new LogRowFormatter(base.LogSchema);
				logRowFormatter[1] = this.ServerName;
				logRowFormatter[2] = (assistantName ?? string.Empty);
				logRowFormatter[3] = (databaseName ?? string.Empty);
				logRowFormatter[4] = eventType.ToString();
				logRowFormatter[5] = ((exception != null) ? exception.GetType().ToString() : string.Empty);
				logRowFormatter[6] = ((exception != null && exception.InnerException != null) ? exception.InnerException.GetType().ToString() : string.Empty);
				base.AppendLog(logRowFormatter);
				if (base.IsDebugTraceEnabled)
				{
					string message = string.Format("Assistant: {0}, Server: {1}, Database: {2}, Event: {3}, Exception: {4}", new object[]
					{
						assistantName,
						this.ServerName,
						databaseName ?? string.Empty,
						eventType,
						(exception != null) ? exception.Message : string.Empty
					});
					base.SafeTraceDebug((long)this.GetHashCode(), message, new object[0]);
				}
			}

			protected override string[] GetLogFields()
			{
				return Enum.GetNames(typeof(DatabaseSlaLogFields));
			}

			protected override ActivityContextLogFileSettings GetLogFileSettings()
			{
				return new MailboxAssistantsDatabaseSlaLogFileSettings();
			}
		}
	}
}
