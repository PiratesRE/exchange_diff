using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Assistants.Logging;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AssistantsLog : ActivityContextLogger
	{
		private AssistantsLog()
		{
		}

		internal static AssistantsLog Instance
		{
			get
			{
				if (AssistantsLog.instance == null)
				{
					AssistantsLog.instance = new AssistantsLog();
				}
				return AssistantsLog.instance;
			}
		}

		internal static HashSet<string> LogDisabledAssistants
		{
			get
			{
				return AssistantsLog.Instance.logDisabledAssistants;
			}
		}

		protected override string LogComponentName
		{
			get
			{
				return "MailboxAssistants";
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "Mailbox Assistants Log";
			}
		}

		protected override string FileNamePrefix
		{
			get
			{
				return "MailboxAssistants";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AssistantBaseTracer;
			}
		}

		protected override string ServerName
		{
			get
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer == null)
				{
					return string.Empty;
				}
				return localServer.Name;
			}
		}

		protected override int TimestampField
		{
			get
			{
				return 0;
			}
		}

		public static void Stop()
		{
			if (AssistantsLog.instance != null)
			{
				AssistantsLog.Instance.Dispose();
				AssistantsLog.instance = null;
			}
		}

		public static void Flush()
		{
			if (AssistantsLog.instance != null)
			{
				AssistantsLog.Instance.FlushLog();
			}
		}

		internal static void LogServiceStartEvent(Guid activityId)
		{
			AssistantsLog.InternalLogRow(activityId, "AssistantsService", null, AssistantsEventType.ServiceStarted, null, Guid.Empty);
		}

		internal static void LogServiceStopEvent(Guid activityId)
		{
			AssistantsLog.InternalLogRow(activityId, "AssistantsService", null, AssistantsEventType.ServiceStopped, null, Guid.Empty);
		}

		internal static void LogStartProcessingMailboxEvent(Guid activityId, AssistantBase assistant, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, TimeBasedDatabaseJob job)
		{
			ArgumentValidator.ThrowIfNull("job", job);
			AssistantsLog.InternalLogAssistantEvent(activityId, assistant, AssistantsEventType.StartProcessingMailbox, null, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, job, MailboxSlaEventType.StartProcessingMailbox, MailboxSlaFilterReasonType.None, null);
		}

		internal static void LogStartProcessingMailboxEvent(Guid activityId, AssistantBase assistant, MapiEvent mapiEvent, Guid mailboxGuid)
		{
			ArgumentValidator.ThrowIfNull("mapiEvent", mapiEvent);
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MapiEvent", AssistantsLog.FormatMapiEvent(mapiEvent))
			};
			AssistantsLog.InternalLogAssistantEvent(activityId, assistant, AssistantsEventType.StartProcessingMailbox, customData, mailboxGuid);
		}

		internal static string GetDiagnosticContext(Exception exception)
		{
			string result = string.Empty;
			while (exception != null)
			{
				MapiPermanentException ex = exception.InnerException as MapiPermanentException;
				if (ex != null)
				{
					result = ex.DiagCtx.ToCompactString();
					break;
				}
				MapiRetryableException ex2 = exception.InnerException as MapiRetryableException;
				if (ex2 != null)
				{
					result = ex2.DiagCtx.ToCompactString();
					break;
				}
				exception = exception.InnerException;
			}
			return result;
		}

		internal static void LogErrorProcessingMailboxEvent(string assistantName, MailboxData mailbox, Exception e, string databaseName = "", string jobId = "", MailboxSlaRequestType requestType = MailboxSlaRequestType.Unknown)
		{
			string value = string.Empty;
			string value2 = "unknown";
			string value3 = "unknown";
			Guid guid = Guid.Empty;
			string value4 = string.Empty;
			string value5 = (e.InnerException != null) ? e.InnerException.GetType().ToString() : "null";
			string diagnosticContext = AssistantsLog.GetDiagnosticContext(e);
			Guid activityId = (ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value : Guid.Empty;
			if (mailbox != null)
			{
				value3 = mailbox.DatabaseGuid.ToString();
				StoreMailboxData storeMailboxData = mailbox as StoreMailboxData;
				if (storeMailboxData != null)
				{
					value2 = "Store";
					guid = storeMailboxData.Guid;
					if (storeMailboxData.OrganizationId != null)
					{
						value = storeMailboxData.OrganizationId.ToString();
					}
				}
				else
				{
					AdminRpcMailboxData adminRpcMailboxData = mailbox as AdminRpcMailboxData;
					if (adminRpcMailboxData != null)
					{
						value2 = "AdminRpc";
						value4 = adminRpcMailboxData.MailboxNumber.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MailboxType", value2),
				new KeyValuePair<string, object>("MailboxGuid", guid),
				new KeyValuePair<string, object>("MailboxId", value4),
				new KeyValuePair<string, object>("TenantId", value),
				new KeyValuePair<string, object>("Database", value3),
				new KeyValuePair<string, object>("ExceptionType", e.GetType().ToString()),
				new KeyValuePair<string, object>("InnerExceptionType", value5),
				new KeyValuePair<string, object>("DiagnosticContext", diagnosticContext)
			};
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.ErrorProcessingMailbox, customData, guid);
			if (!string.IsNullOrEmpty(assistantName))
			{
				MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog logInstance = MailboxAssistantsSlaReportLogFactory.GetLogInstance(assistantName, SlaLogType.MailboxSlaLog);
				if (logInstance != null)
				{
					logInstance.LogMailboxEvent(assistantName, databaseName, jobId, requestType, guid, (mailbox == null) ? string.Empty : mailbox.DisplayName, MailboxSlaEventType.ErrorProcessingMailbox, MailboxSlaFilterReasonType.None, e);
				}
			}
		}

		internal static void LogEndProcessingMailboxEvent(Guid activityId, AssistantBase assistant, List<KeyValuePair<string, object>> customData, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, TimeBasedDatabaseJob job = null)
		{
			AssistantsLog.InternalLogAssistantEvent(activityId, assistant, AssistantsEventType.EndProcessingMailbox, customData, mailboxGuid);
			if (job != null)
			{
				AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, job, MailboxSlaEventType.EndProcessingMailbox, MailboxSlaFilterReasonType.None, null);
			}
		}

		internal static void LogMailboxInterestingEvent(Guid activityId, string assistantName, AssistantBase assistant, List<KeyValuePair<string, object>> customData, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, assistant, AssistantsEventType.MailboxInteresting, customData, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, null, MailboxSlaEventType.MailboxInteresting, MailboxSlaFilterReasonType.None, null);
		}

		internal static void LogMailboxInterestingEvent(Guid activityId, string assistantName, List<KeyValuePair<string, object>> customData, Guid mailboxGuid)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.MailboxInteresting, customData, mailboxGuid);
		}

		internal static void LogMailboxNotInterestingEvent(Guid activityId, string assistantName, AssistantBase assistant, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, assistant, AssistantsEventType.MailboxNotInteresting, null, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, null, MailboxSlaEventType.MailboxNotInteresting, MailboxSlaFilterReasonType.None, null);
		}

		internal static void LogMailboxFilteredEvent(Guid activityId, string assistantName, AssistantBase assistant, string reason, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, MailboxSlaFilterReasonType filterReason)
		{
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("Reason", reason)
			};
			AssistantsLog.InternalLogRow(activityId, assistantName, assistant, AssistantsEventType.FilterMailbox, customData, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, null, MailboxSlaEventType.FilterMailbox, filterReason, null);
		}

		internal static void LogMailboxSucceedToOpenStoreSessionEvent(Guid activityId, string assistantName, AssistantBase assistant, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, TimeBasedDatabaseJob job)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, assistant, AssistantsEventType.SucceedOpenMailboxStoreSession, null, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, job, MailboxSlaEventType.SucceedOpenMailboxStoreSession, MailboxSlaFilterReasonType.None, null);
		}

		internal static void LogMailboxFailedToOpenStoreSessionEvent(Guid activityId, string assistantName, AssistantBase assistant, Exception storeSessionException, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, TimeBasedDatabaseJob job)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			if (storeSessionException != null)
			{
				list.Add(new KeyValuePair<string, object>("ExceptionType", storeSessionException.GetType().ToString()));
				list.Add(new KeyValuePair<string, object>("ExceptionMessage", storeSessionException.Message));
				if (storeSessionException.InnerException != null)
				{
					list.Add(new KeyValuePair<string, object>("InnerExceptionType", storeSessionException.InnerException.GetType().ToString()));
					list.Add(new KeyValuePair<string, object>("InnerExceptionMessage", storeSessionException.InnerException.Message));
				}
			}
			AssistantsLog.InternalLogRow(activityId, assistantName, assistant, AssistantsEventType.FailedOpenMailboxStoreSession, list, mailboxGuid);
			AssistantsLog.LogMailboxSlaEvent(assistant, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, job, MailboxSlaEventType.FailedOpenMailboxStoreSession, MailboxSlaFilterReasonType.None, storeSessionException);
		}

		internal static void LogBeginJob(string assistantName, string databaseName, int startingPendingQueueCount)
		{
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("DatabaseName", databaseName),
				new KeyValuePair<string, object>("PendingQueueCount", startingPendingQueueCount)
			};
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.BeginJob, customData, Guid.Empty);
		}

		internal static void LogEndJobEvent(string assistantName, List<KeyValuePair<string, object>> customData)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.EndJob, customData, Guid.Empty);
		}

		internal static void LogDatabaseStartEvent(AssistantBase assistant)
		{
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.StartDatabase, null);
		}

		internal static void LogDatabaseStopEvent(AssistantBase assistant)
		{
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.StopDatabase, null);
		}

		internal static void LogGetMailboxesQueryEvent(Guid activityId, string assistantName, int numberOfMailboxesInQuery, AssistantBase assistant)
		{
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MailboxesToProcess", numberOfMailboxesInQuery)
			};
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.ReceivedQueriedMailboxes, customData, Guid.Empty);
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.StartMailboxTableQuery, null);
		}

		internal static void LogEndGetMailboxesEvent(Guid activityId, string assistantName, int numberOfMailboxesToProcess, AssistantBase assistant)
		{
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MailboxesToProcess", numberOfMailboxesToProcess)
			};
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.EndGetMailboxes, customData, Guid.Empty);
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.EndMailboxTableQuery, null);
		}

		internal static void LogNoJobsEvent(string assistantName)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.NoJobs, null, Guid.Empty);
		}

		internal static void LogNotStartedEvent(string assistantName, AssistantBase assistant)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.NotStarted, null, Guid.Empty);
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.DatabaseIsStopped, null);
		}

		internal static void LogDriverNotStartedEvent(string assistantName, AssistantBase assistant)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.DriverNotStarted, null, Guid.Empty);
			AssistantsLog.LogDatabaseSlaEvent(assistant, DatabaseSlaEventType.DatabaseIsStopped, null);
		}

		internal static void LogJobAlreadyRunningEvent(string assistantName)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.JobAlreadyRunning, null, Guid.Empty);
		}

		internal static void LogNoMailboxesPendingEvent(string assistantName)
		{
			AssistantsLog.InternalLogRow(Guid.Empty, assistantName, null, AssistantsEventType.NoMailboxes, null, Guid.Empty);
		}

		internal static void LogErrorEnumeratingMailboxes(ITimeBasedAssistant assistant, Guid mailboxGuid, Exception exception, bool isExceptionHandled)
		{
			string value = (exception.InnerException != null) ? exception.InnerException.GetType().ToString() : string.Empty;
			List<KeyValuePair<string, object>> customData = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MailboxGuid", mailboxGuid.ToString()),
				new KeyValuePair<string, object>("ExceptionType", exception.GetType().ToString()),
				new KeyValuePair<string, object>("InnerExceptionType", value),
				new KeyValuePair<string, object>("IsExceptionHandled", isExceptionHandled.ToString()),
				new KeyValuePair<string, object>("ExceptionDetail", exception.ToString())
			};
			AssistantsLog.InternalLogRow(Guid.Empty, (assistant != null) ? assistant.NonLocalizedName : string.Empty, null, AssistantsEventType.ErrorEnumeratingMailbox, customData, Guid.Empty);
			AssistantsLog.LogDatabaseSlaEvent(assistant as AssistantBase, DatabaseSlaEventType.ErrorMailboxTableQuery, exception);
		}

		internal static void LogFolderSyncOperationEvent(Guid activityId, string assistantName, List<KeyValuePair<string, object>> customData)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.FolderSyncOperation, customData, Guid.Empty);
		}

		internal static void LogFolderSyncExceptionEvent(Guid activityId, string assistantName, List<KeyValuePair<string, object>> customData)
		{
			AssistantsLog.InternalLogRow(activityId, assistantName, null, AssistantsEventType.FolderSyncException, customData, Guid.Empty);
		}

		protected override void InternalConfigure(ActivityContextLogFileSettings settings)
		{
			base.InternalConfigure(settings);
			this.logDisabledAssistants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			AssistantsLogFileSettings assistantsLogFileSettings = settings as AssistantsLogFileSettings;
			if (assistantsLogFileSettings == null)
			{
				return;
			}
			foreach (string text in assistantsLogFileSettings.LogDisabledAssistants)
			{
				string text2 = text.Trim();
				if (text2 != string.Empty)
				{
					this.logDisabledAssistants.Add(text2);
				}
			}
		}

		protected override void InternalLogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			object userState = activityScope.UserState;
			AssistantBase assistantBase = null;
			string assistantShortName;
			if (activityScope.ActivityType == ActivityType.Global)
			{
				assistantShortName = "GlobalActivity";
			}
			else
			{
				SystemTaskBase systemTaskBase = userState as SystemTaskBase;
				if (systemTaskBase != null)
				{
					assistantShortName = systemTaskBase.Workload.Id;
				}
				else
				{
					AssistantBase assistantBase2 = userState as AssistantBase;
					if (assistantBase2 == null)
					{
						return;
					}
					assistantBase = assistantBase2;
					assistantShortName = assistantBase.NonLocalizedName;
				}
			}
			AssistantsEventType eventType2;
			switch (eventType)
			{
			case ActivityEventType.SuspendActivity:
				eventType2 = AssistantsEventType.SuspendActivity;
				goto IL_92;
			case ActivityEventType.EndActivity:
				eventType2 = AssistantsEventType.EndActivity;
				goto IL_92;
			}
			base.SafeTraceDebug(0L, "Skip logging ActivityEvent '{0}'.", new object[]
			{
				eventType
			});
			return;
			IL_92:
			List<KeyValuePair<string, object>> customData = WorkloadManagementLogger.FormatWlmActivity(activityScope, true);
			AssistantsLog.InternalLogRow(activityScope.ActivityId, assistantShortName, assistantBase, eventType2, customData, Guid.Empty);
		}

		protected override string[] GetLogFields()
		{
			return Enum.GetNames(typeof(AssistantsLogField));
		}

		protected override ActivityContextLogFileSettings GetLogFileSettings()
		{
			return AssistantsLogFileSettings.Load();
		}

		private static void InternalLogAssistantEvent(Guid activityId, AssistantBase assistant, AssistantsEventType eventType, List<KeyValuePair<string, object>> customData, Guid mailboxGuid)
		{
			string assistantShortName = (assistant == null) ? "Unknown" : assistant.NonLocalizedName;
			AssistantsLog.InternalLogRow(activityId, assistantShortName, assistant, eventType, customData, mailboxGuid);
		}

		private static void InternalLogRow(Guid activityId, string assistantShortName, AssistantBase assistant, AssistantsEventType eventType, List<KeyValuePair<string, object>> customData, Guid mailboxGuid)
		{
			if (!AssistantsLog.Instance.Enabled)
			{
				AssistantsLog.Instance.SafeTraceDebug(0L, "Mailbox assistant log is disabled, skip writing to the log file.", new object[0]);
				return;
			}
			if (AssistantsLog.LogDisabledAssistants.Contains(assistantShortName))
			{
				AssistantsLog.Instance.SafeTraceDebug(0L, "Mailbox assistant '{0}' is disabled for logging, skip writing to the log file.", new object[]
				{
					assistantShortName
				});
				return;
			}
			string text = string.Empty;
			if (assistant != null && assistant.DatabaseInfo != null)
			{
				text = assistant.DatabaseInfo.DatabaseName;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(AssistantsLog.Instance.LogSchema);
			if (AssistantsLog.Instance.IsDebugTraceEnabled)
			{
				string text2 = string.Empty;
				if (customData != null)
				{
					bool flag;
					text2 = LogRowFormatter.FormatCollection(customData, out flag);
				}
				AssistantsLog.Instance.SafeTraceDebug(0L, "Start writing row to mailbox assistant log: ServerName='{0}', Location='{1}', AssistantName='{2}', ActivityId='{3}', TargetObject='{4}', Event='{5}', CustomData='{6}'", new object[]
				{
					AssistantsLog.Instance.ServerName,
					text,
					assistantShortName,
					activityId,
					mailboxGuid,
					AssistantsLog.stringDictionary[eventType],
					text2
				});
			}
			logRowFormatter[1] = AssistantsLog.Instance.ServerName;
			logRowFormatter[3] = assistantShortName;
			logRowFormatter[6] = AssistantsLog.stringDictionary[eventType];
			logRowFormatter[2] = text;
			logRowFormatter[7] = customData;
			logRowFormatter[5] = ((mailboxGuid == Guid.Empty) ? string.Empty : mailboxGuid.ToString("D"));
			logRowFormatter[4] = ((activityId == Guid.Empty) ? string.Empty : activityId.ToString("D"));
			AssistantsLog.Append(logRowFormatter);
			AssistantsLog.Instance.SafeTraceDebug(0L, "The above row is written to mailbox assistant log successfully.", new object[0]);
		}

		private static void Append(LogRowFormatter row)
		{
			AssistantsLog.Instance.AppendLog(row);
		}

		private static Dictionary<AssistantsEventType, string> CreateTypeDictionary()
		{
			Dictionary<AssistantsEventType, string> dictionary = new Dictionary<AssistantsEventType, string>();
			string[] names = Enum.GetNames(typeof(AssistantsEventType));
			Array values = Enum.GetValues(typeof(AssistantsEventType));
			int num = names.Length;
			for (int i = 0; i < num; i++)
			{
				dictionary.Add((AssistantsEventType)values.GetValue(i), names[i]);
			}
			return dictionary;
		}

		private static string FormatMapiEvent(MapiEvent mapiEvent)
		{
			string text = string.Format("Counter: 0x{0,0:X}, MailboxGUID: {1}, Mask: {2}, Flags: {3}, ExtendedFlags: {4}, Object Class: {5}, Created Time: {6}, Item Type: {7}, Item EntryId: {8}, Parent entryId: {9}, Old Item entryId: {10}, Old parent entryId: {11}, SID: {12}, Client Type: {13}, Document ID: {14}", new object[]
			{
				mapiEvent.EventCounter,
				mapiEvent.MailboxGuid,
				mapiEvent.EventMask,
				mapiEvent.EventFlags,
				mapiEvent.ExtendedEventFlags,
				mapiEvent.ObjectClass,
				mapiEvent.CreateTime,
				mapiEvent.ItemType,
				AssistantsLog.FormatEntryId(mapiEvent.ItemEntryId),
				AssistantsLog.FormatEntryId(mapiEvent.ParentEntryId),
				AssistantsLog.FormatEntryId(mapiEvent.OldItemEntryId),
				AssistantsLog.FormatEntryId(mapiEvent.OldParentEntryId),
				(null != mapiEvent.Sid) ? mapiEvent.Sid.ToString() : "<null>",
				mapiEvent.ClientType,
				mapiEvent.DocumentId
			});
			if (ObjectType.MAPI_FOLDER == mapiEvent.ItemType)
			{
				text += string.Format(", Item Count: {0}, Unread Item Count: {1}", mapiEvent.ItemCount, mapiEvent.UnreadItemCount);
			}
			return text;
		}

		private static string FormatEntryId(byte[] entryId)
		{
			if (entryId != null)
			{
				return BitConverter.ToString(entryId);
			}
			return "<null>";
		}

		private static void LogMailboxSlaEvent(AssistantBase assistant, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, TimeBasedDatabaseJob job, MailboxSlaEventType eventType, MailboxSlaFilterReasonType reason = MailboxSlaFilterReasonType.None, Exception exception = null)
		{
			string text = "Unknown";
			string databaseName = "Unknown";
			string jobId = string.Empty;
			MailboxSlaRequestType requestType = MailboxSlaRequestType.Unknown;
			if (assistant != null)
			{
				databaseName = ((assistant.DatabaseInfo == null) ? "Unknown" : assistant.DatabaseInfo.DatabaseName);
				text = assistant.NonLocalizedName;
			}
			if (job != null)
			{
				jobId = job.StartTime.ToString("O");
				requestType = ((job is TimeBasedDatabaseWindowJob) ? MailboxSlaRequestType.Scheduled : MailboxSlaRequestType.OnDemand);
			}
			MailboxAssistantsSlaReportLogFactory.MailboxAssistantsSlaReportLog logInstance = MailboxAssistantsSlaReportLogFactory.GetLogInstance(text, SlaLogType.MailboxSlaLog);
			if (logInstance != null)
			{
				logInstance.LogMailboxEvent(text, databaseName, jobId, requestType, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, eventType, reason, exception);
			}
		}

		private static void LogDatabaseSlaEvent(AssistantBase assistant, DatabaseSlaEventType eventType, Exception exception = null)
		{
			string text = "Unknown";
			string databaseName = "Unknown";
			if (assistant != null)
			{
				databaseName = ((assistant.DatabaseInfo == null) ? "Unknown" : assistant.DatabaseInfo.DatabaseName);
				text = assistant.NonLocalizedName;
			}
			MailboxAssistantsSlaReportLogFactory.MailboxAssistantsDatabaseSlaLog mailboxAssistantsDatabaseSlaLog = MailboxAssistantsSlaReportLogFactory.GetLogInstance(text, SlaLogType.DatabaseSlaLog) as MailboxAssistantsSlaReportLogFactory.MailboxAssistantsDatabaseSlaLog;
			if (mailboxAssistantsDatabaseSlaLog != null)
			{
				mailboxAssistantsDatabaseSlaLog.LogDatabaseEvent(text, databaseName, eventType, exception);
			}
		}

		internal const string AssistantsServiceName = "AssistantsService";

		internal const string AssistantsLogFileNamePrefix = "MailboxAssistants";

		internal const string AssistantsLogComponentName = "MailboxAssistants";

		internal const string AssistantsLogTypeName = "Mailbox Assistants Log";

		internal const string Unknown = "Unknown";

		private static readonly Dictionary<AssistantsEventType, string> stringDictionary = AssistantsLog.CreateTypeDictionary();

		private static AssistantsLog instance;

		private HashSet<string> logDisabledAssistants;
	}
}
