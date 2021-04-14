using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class LogDatapoint : ServiceCommand<bool>
	{
		public LogDatapoint(CallContext callContext, Datapoint[] datapoints, Action<IEnumerable<ILogEvent>> analyticsLogger, Action<IEnumerable<ILogEvent>> diagnosticsLogger, Action<string, Type> registerType, bool isOwa, string owaVersion) : this(callContext, datapoints, analyticsLogger, diagnosticsLogger, new ClientWatsonDatapointHandler(callContext.ProtocolLog, owaVersion), registerType, isOwa)
		{
		}

		public LogDatapoint(CallContext callContext, Datapoint[] datapoints, Action<IEnumerable<ILogEvent>> analyticsLogger, Action<IEnumerable<ILogEvent>> diagnosticsLogger, ClientWatsonDatapointHandler clientWatsonHandler, Action<string, Type> registerType, bool isOwa) : base(callContext)
		{
			this.datapoints = datapoints;
			this.analyticsLogger = analyticsLogger;
			this.diagnosticsLogger = diagnosticsLogger;
			this.clientWatsonHandler = clientWatsonHandler;
			this.isOwa = isOwa;
			registerType(LogDatapoint.LogDatapointActionName, typeof(LogDatapointMetadata));
			string value = callContext.HttpContext.Request.Headers["X-OWA-Test-PassThruProxy"];
			if (!string.IsNullOrEmpty(value))
			{
				this.isFromPassThroughProxy = true;
			}
			this.serverVersion = this.GetServerVersion();
			this.clientVersion = (callContext.HttpContext.Request.Headers["X-OWA-ClientBuildVersion"] ?? this.serverVersion);
		}

		protected override bool InternalExecute()
		{
			if (this.datapoints == null || this.datapoints.Length == 0)
			{
				return true;
			}
			InstrumentationSettings instrumentationSettings = this.GetInstrumentationSettings();
			if (!instrumentationSettings.IsInstrumentationEnabled())
			{
				return true;
			}
			UserContext userContext = this.GetUserContext();
			Stopwatch stopwatch = Stopwatch.StartNew();
			Datapoint chunkHeaderDatapoint = this.GetChunkHeaderDatapoint(this.datapoints[0].Time);
			int num;
			IDictionary<DatapointConsumer, LogDatapoint.ClientLogEventList> dictionary = this.TriageAndConvertDatapoints(userContext, chunkHeaderDatapoint, instrumentationSettings, out num);
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			IList<ClientLogEvent> events = dictionary[DatapointConsumer.Watson].Events;
			if (events.Count > 0 && instrumentationSettings.IsClientWatsonEnabled)
			{
				this.clientWatsonHandler.ReportWatsonEvents(userContext, events, chunkHeaderDatapoint, this.datapoints);
			}
			FaultInjection.GenerateFault((FaultInjection.LIDs)3804638525U);
			stopwatch.Restart();
			this.analyticsLogger(dictionary[DatapointConsumer.Analytics].Events);
			this.diagnosticsLogger(dictionary[DatapointConsumer.Diagnostics].Events);
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			long num2 = 0L;
			long num3 = 0L;
			int num4 = 0;
			if (this.isOwa && dictionary[DatapointConsumer.Inference].Events.Count > 0)
			{
				stopwatch.Restart();
				IActivityLogger activityLogger = this.GetActivityLogger();
				if (activityLogger != null)
				{
					IList<Activity> list = this.CreateInferenceActivities(dictionary[DatapointConsumer.Inference].Events);
					num2 = stopwatch.ElapsedMilliseconds;
					num3 = 0L;
					if (list.Count > 0)
					{
						num4 = list.Count;
						stopwatch.Restart();
						this.WriteInferenceActivities(list, activityLogger);
						num3 = stopwatch.ElapsedMilliseconds;
					}
				}
			}
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.CreateDatapointEventsElapsed, elapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.TotalDatapointSize, num);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.AnalyticsDatapointCount, dictionary[DatapointConsumer.Analytics].Events.Count);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.AnalyticsDatapointSize, dictionary[DatapointConsumer.Analytics].DatapointTotalSize);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.InferenceDatapointCount, dictionary[DatapointConsumer.Inference].Events.Count);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.InferenceDatapointSize, dictionary[DatapointConsumer.Inference].DatapointTotalSize);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.DiagnosticsDatapointCount, dictionary[DatapointConsumer.Diagnostics].Events.Count);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.DiagnosticsDatapointSize, dictionary[DatapointConsumer.Diagnostics].DatapointTotalSize);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.WatsonDatapointCount, dictionary[DatapointConsumer.Watson].Events.Count);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.WatsonDatapointSize, dictionary[DatapointConsumer.Watson].DatapointTotalSize);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.DatapointsToLoggerElapsed, elapsedMilliseconds2);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.InferenceActivitiesToMailboxCount, num4);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.CreateInferenceActivitiesElapsed, num2);
			base.CallContext.ProtocolLog.Set(LogDatapointMetadata.InferenceActivitiesToMailboxElapsed, num3);
			return true;
		}

		protected virtual IActivityLogger GetActivityLogger()
		{
			MailboxSession mailboxSession = this.GetMailboxSession() as MailboxSession;
			return ActivityLogger.Create(mailboxSession);
		}

		protected virtual string GetClientAddress()
		{
			string text = HttpContext.Current.Request.Headers["X-Forwarded-For"];
			if (string.IsNullOrEmpty(text))
			{
				return HttpContext.Current.Request.UserHostAddress;
			}
			return text;
		}

		protected virtual string GetServerVersion()
		{
			if (this.isOwa)
			{
				return Globals.ApplicationVersion ?? OwaRegistryKeys.InstalledOwaVersion;
			}
			return string.Empty;
		}

		protected virtual InstrumentationSettings GetInstrumentationSettings()
		{
			return InstrumentationSettings.Instance;
		}

		protected virtual IMailboxSession GetMailboxSession()
		{
			if (this.mailboxSession == null)
			{
				this.mailboxSession = base.MailboxIdentityMailboxSession;
			}
			return this.mailboxSession;
		}

		protected virtual UserContext GetUserContext()
		{
			if (this.isOwa)
			{
				return UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			}
			return null;
		}

		private static bool ShouldProcessForConsumer(Datapoint datapoint, DatapointConsumer consumer, DatapointConsumer enabledConsumers)
		{
			return datapoint.IsForConsumer(consumer) && (consumer & enabledConsumers) != DatapointConsumer.None;
		}

		private IDictionary<DatapointConsumer, LogDatapoint.ClientLogEventList> TriageAndConvertDatapoints(UserContext userContext, Datapoint header, InstrumentationSettings settings, out int totalDatapointSize)
		{
			string userContextId = this.GetUserContextId();
			string clientAddress = this.GetClientAddress();
			string userName = this.isOwa ? base.CallContext.GetEffectiveAccessingSmtpAddress() : "ExternalUser";
			string cookieValueAndSetIfNull = ClientIdCookie.GetCookieValueAndSetIfNull(HttpContext.Current);
			ClientLogEvent header2 = new ClientLogEvent(header, userContextId, clientAddress, userName, this.serverVersion, base.CallContext.IsMowa, cookieValueAndSetIfNull);
			DatapointConsumer enabledConsumers = this.GetEnabledConsumers(settings);
			totalDatapointSize = 0;
			LogDatapoint.ClientLogEventList clientLogEventList = new LogDatapoint.ClientLogEventList(DatapointConsumer.Analytics, enabledConsumers, header2);
			LogDatapoint.ClientLogEventList clientLogEventList2 = new LogDatapoint.ClientLogEventList(DatapointConsumer.Diagnostics, enabledConsumers, header2);
			LogDatapoint.ClientLogEventList clientLogEventList3 = new LogDatapoint.ClientLogEventList(DatapointConsumer.Inference, enabledConsumers, null);
			LogDatapoint.ClientLogEventList clientLogEventList4 = new LogDatapoint.ClientLogEventList(DatapointConsumer.Watson, enabledConsumers, null);
			int num = 0;
			if (userContext != null && this.datapoints[0].Id == "SessionInfo")
			{
				num = 1;
				Datapoint datapoint = this.datapoints[0];
				ClientLogEvent clientLogEvent = new ClientLogEvent(datapoint, userContextId, clientAddress, userName, this.serverVersion, base.CallContext.IsMowa, cookieValueAndSetIfNull);
				userContext.LogEventCommonData.UpdateClientData(clientLogEvent.DatapointProperties);
				this.clientVersion = userContext.LogEventCommonData.ClientBuild;
				clientLogEvent.UpdateTenantInfo(userContext);
				clientLogEvent.UpdateNetid(userContext);
				clientLogEvent.UpdateMailboxGuid(userContext.ExchangePrincipal);
				clientLogEvent.UpdateDatabaseInfo(userContext);
				clientLogEvent.UpdateFlightInfo(userContext.LogEventCommonData);
				clientLogEvent.UpdatePassThroughProxyInfo(this.isFromPassThroughProxy);
				clientLogEvent.UpdateUserAgent(userContext.UserAgent);
				clientLogEventList.CheckAndAdd(clientLogEvent);
				clientLogEventList3.CheckAndAdd(clientLogEvent);
				clientLogEventList2.CheckAndAdd(clientLogEvent);
				clientLogEventList4.CheckAndAdd(clientLogEvent);
			}
			for (int i = num; i < this.datapoints.Length; i++)
			{
				Datapoint datapoint2 = this.datapoints[i];
				totalDatapointSize += datapoint2.Size;
				if ((enabledConsumers & datapoint2.Consumers) != DatapointConsumer.None)
				{
					ClientLogEvent clientLogEvent2 = new ClientLogEvent(datapoint2, userContextId, clientAddress, userName, this.serverVersion, base.CallContext.IsMowa, cookieValueAndSetIfNull);
					if (userContext != null)
					{
						clientLogEvent2.UpdateClientBuildVersion(userContext.LogEventCommonData);
						string id;
						if (clientLogEventList.CheckAndAdd(clientLogEvent2) && (id = datapoint2.Id) != null)
						{
							if (<PrivateImplementationDetails>{38B691CF-9E72-4F22-A560-A7F126C51047}.$$method0x6001a88-1 == null)
							{
								<PrivateImplementationDetails>{38B691CF-9E72-4F22-A560-A7F126C51047}.$$method0x6001a88-1 = new Dictionary<string, int>(11)
								{
									{
										"PerfTraceCTQ",
										0
									},
									{
										"InlineFeedback",
										1
									},
									{
										"PerfNavTime",
										2
									},
									{
										"PerfTrace",
										3
									},
									{
										"Performance",
										4
									},
									{
										"Core.Models.Sync.ModuleStats",
										5
									},
									{
										"Core.Models.Sync.MailSyncReachedLimit",
										6
									},
									{
										"PageDataPayload",
										7
									},
									{
										"Core.Models.Sync.FolderActiveSyncingAgain",
										8
									},
									{
										"Action",
										9
									},
									{
										"ActionRecordDatapoint",
										10
									}
								};
							}
							int num2;
							if (<PrivateImplementationDetails>{38B691CF-9E72-4F22-A560-A7F126C51047}.$$method0x6001a88-1.TryGetValue(id, out num2))
							{
								switch (num2)
								{
								case 0:
									clientLogEvent2.UpdateDeviceInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateTenantInfo(userContext);
									clientLogEvent2.UpdateNetid(userContext);
									clientLogEvent2.UpdateMailboxGuid(userContext.ExchangePrincipal);
									clientLogEvent2.UpdateDatabaseInfo(userContext);
									clientLogEvent2.UpdateFlightInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateClientInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateUserAgent(userContext.UserAgent);
									clientLogEvent2.UpdateTenantGuid(userContext.ExchangePrincipal);
									break;
								case 1:
									clientLogEvent2.UpdateDeviceInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateTenantInfo(userContext);
									clientLogEvent2.UpdateNetid(userContext);
									clientLogEvent2.UpdateMailboxGuid(userContext.ExchangePrincipal);
									clientLogEvent2.UpdateDatabaseInfo(userContext);
									clientLogEvent2.UpdateFlightInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateClientInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateUserAgent(userContext.UserAgent);
									break;
								case 2:
									clientLogEvent2.UpdatePerformanceNavigationDatapoint(userContext);
									break;
								case 3:
									clientLogEvent2.UpdatePerfTraceDatapoint(userContext);
									break;
								case 4:
									clientLogEvent2.UpdateDeviceInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateTenantGuid(userContext.ExchangePrincipal);
									clientLogEvent2.UpdateMailboxGuid(userContext.ExchangePrincipal);
									clientLogEvent2.UpdateUserAgent(userContext.UserAgent);
									break;
								case 5:
								case 6:
									clientLogEvent2.UpdateDeviceInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateTenantInfo(userContext);
									clientLogEvent2.UpdateNetid(userContext);
									clientLogEvent2.UpdateMailboxGuid(userContext.ExchangePrincipal);
									clientLogEvent2.UpdateDatabaseInfo(userContext);
									clientLogEvent2.UpdateClientLocaleInfo(userContext.LogEventCommonData);
									break;
								case 7:
								case 8:
									clientLogEvent2.UpdateDeviceInfo(userContext.LogEventCommonData);
									clientLogEvent2.UpdateClientLocaleInfo(userContext.LogEventCommonData);
									break;
								case 9:
								case 10:
									clientLogEvent2.UpdateActionRecordDataPoint(userContext);
									break;
								}
							}
						}
					}
					clientLogEvent2.UpdatePassThroughProxyInfo(this.isFromPassThroughProxy);
					clientLogEventList3.CheckAndAdd(clientLogEvent2);
					clientLogEventList2.CheckAndAdd(clientLogEvent2);
					clientLogEventList4.CheckAndAdd(clientLogEvent2);
				}
			}
			if (clientLogEventList.Count == 1)
			{
				clientLogEventList.Clear();
			}
			if (clientLogEventList2.Count == 1)
			{
				clientLogEventList2.Clear();
			}
			return new Dictionary<DatapointConsumer, LogDatapoint.ClientLogEventList>
			{
				{
					DatapointConsumer.Analytics,
					clientLogEventList
				},
				{
					DatapointConsumer.Inference,
					clientLogEventList3
				},
				{
					DatapointConsumer.Diagnostics,
					clientLogEventList2
				},
				{
					DatapointConsumer.Watson,
					clientLogEventList4
				}
			};
		}

		private Datapoint GetChunkHeaderDatapoint(string time)
		{
			HttpContext httpContext = base.CallContext.HttpContext;
			if (httpContext != null)
			{
				string[] keys = new string[]
				{
					"UA"
				};
				string[] values = new string[]
				{
					httpContext.Request.UserAgent
				};
				return new Datapoint(DatapointConsumer.Analytics, "DatapointChunkHeader", time, keys, values);
			}
			return new Datapoint(DatapointConsumer.Analytics, "DatapointChunkHeader", time, null, null);
		}

		private DatapointConsumer GetEnabledConsumers(InstrumentationSettings settings)
		{
			DatapointConsumer datapointConsumer = DatapointConsumer.None;
			if (settings.CoreAnalyticsProbability > 0f || settings.AnalyticsProbability > 0f)
			{
				datapointConsumer |= DatapointConsumer.Analytics;
			}
			if (settings.IsInferenceEnabled)
			{
				datapointConsumer |= DatapointConsumer.Inference;
			}
			if (settings.IsClientWatsonEnabled)
			{
				datapointConsumer |= DatapointConsumer.Watson;
			}
			if (settings.DefaultTraceLevel != TraceLevel.Off)
			{
				datapointConsumer |= DatapointConsumer.Diagnostics;
			}
			return datapointConsumer;
		}

		private IList<Activity> CreateInferenceActivities(IList<ClientLogEvent> clientLogEvents)
		{
			UserContext userContext = this.GetUserContext();
			IList<Activity> activities = null;
			this.LogStoreExceptions("CreateInferenceActivities", delegate
			{
				IMailboxSession mailboxSession = this.GetMailboxSession();
				string clientAddress = this.GetClientAddress();
				string userAgent = this.CallContext.UserAgent;
				ActivityConverter activityConverter = new ActivityConverter(userContext, mailboxSession, clientAddress, userAgent, this.clientVersion);
				activities = activityConverter.GetActivities(clientLogEvents);
			});
			return activities ?? new List<Activity>();
		}

		private void WriteInferenceActivities(IList<Activity> activities, IActivityLogger inferenceLogger)
		{
			this.LogStoreExceptions("WriteInferenceActivities", delegate
			{
				inferenceLogger.Log(activities);
			});
		}

		private string GetUserContextId()
		{
			if (base.CallContext.HttpContext == null)
			{
				return string.Empty;
			}
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(base.CallContext.HttpContext);
			if (userContextCookie != null)
			{
				return userContextCookie.CookieValue;
			}
			return string.Empty;
		}

		private void LogStoreExceptions(string operation, Action action)
		{
			try
			{
				action();
			}
			catch (StoragePermanentException ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "LogDatapoint_LogStoreExceptions_" + operation);
			}
			catch (StorageTransientException ex2)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex2, "LogDatapoint_LogStoreExceptions_" + operation);
			}
		}

		private const string PerformanceNavigationTimingDatapointId = "PerfNavTime";

		private const string SessionInfoDatapointId = "SessionInfo";

		private const string PerfTraceCTQDatapointId = "PerfTraceCTQ";

		private const string PerfTraceDatapointId = "PerfTrace";

		private const string ActionRecordDatapointId = "Action";

		private const string ActionRecordOldDataPointId = "ActionRecordDatapoint";

		private const string InlineFeedbackDatapointId = "InlineFeedback";

		private const string PerformanceDatapointId = "Performance";

		private const string SyncModuleStatsId = "Core.Models.Sync.ModuleStats";

		private const string SyncMailSyncReachedLimitId = "Core.Models.Sync.MailSyncReachedLimit";

		private const string SyncFolderActiveSyncingAgainId = "Core.Models.Sync.FolderActiveSyncingAgain";

		private const string PageDataPayloadId = "PageDataPayload";

		private const string ClientPerfCounterId = "PC";

		internal const string LogNamespace = "LD";

		private static readonly string LogDatapointActionName = typeof(LogDatapoint).Name;

		private readonly Datapoint[] datapoints;

		private readonly Action<IEnumerable<ILogEvent>> analyticsLogger;

		private readonly Action<IEnumerable<ILogEvent>> diagnosticsLogger;

		private readonly ClientWatsonDatapointHandler clientWatsonHandler;

		private readonly bool isOwa;

		private IMailboxSession mailboxSession;

		private readonly bool isFromPassThroughProxy;

		private string clientVersion;

		private readonly string serverVersion;

		private sealed class ClientLogEventList
		{
			public ClientLogEventList(DatapointConsumer consumerFilter, DatapointConsumer enabledConsumers, ClientLogEvent header = null)
			{
				this.Events = ((header == null) ? new List<ClientLogEvent>() : new List<ClientLogEvent>
				{
					header
				});
				this.DatapointTotalSize = 0;
				this.consumerFilter = consumerFilter;
				this.consumerEnabled = ((consumerFilter & enabledConsumers) != DatapointConsumer.None);
			}

			public IList<ClientLogEvent> Events { get; private set; }

			public int DatapointTotalSize { get; private set; }

			public int Count
			{
				get
				{
					return this.Events.Count;
				}
			}

			public bool CheckAndAdd(ClientLogEvent clientLogEvent)
			{
				if (this.consumerEnabled && clientLogEvent.IsForConsumer(this.consumerFilter))
				{
					this.Events.Add(clientLogEvent);
					this.DatapointTotalSize += clientLogEvent.InnerDatapoint.Size;
					return true;
				}
				return false;
			}

			public void Clear()
			{
				this.Events.Clear();
			}

			private readonly DatapointConsumer consumerFilter;

			private readonly bool consumerEnabled;
		}
	}
}
