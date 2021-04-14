using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class StoreDriverSubmission : IStoreDriverSubmission, IStartableTransportComponent, ITransportComponent, ISubmissionProvider, IDiagnosable
	{
		public StoreDriverSubmission(IStoreDriverTracer storeDriverTracer)
		{
			ArgumentValidator.ThrowIfNull("storeDriverTracer", storeDriverTracer);
			this.storeDriverTracer = storeDriverTracer;
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(StoreDriverSubmission.FaultInjectionCallback));
			ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.Transport);
		}

		public static IPHostEntry LocalIP
		{
			get
			{
				return StoreDriverSubmission.localIp;
			}
		}

		public static IPAddress LocalIPAddress
		{
			get
			{
				return StoreDriverSubmission.localIp.AddressList[0];
			}
		}

		public static string ReceivedHeaderTcpInfo
		{
			get
			{
				return StoreDriverSubmission.receivedHeaderTcpInfo;
			}
		}

		public virtual bool PoisonMessageDectionEnabled
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.PoisonMessageDetectionEnabled;
			}
		}

		public bool IsSubmissionPaused
		{
			get
			{
				return this.paused;
			}
		}

		public bool Retired
		{
			get
			{
				return this.stopped;
			}
		}

		public IStoreDriverTracer StoreDriverTracer
		{
			get
			{
				return this.storeDriverTracer;
			}
		}

		public string CurrentState
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				stringBuilder.Append("Submission thread count=");
				stringBuilder.AppendLine(SubmissionThreadLimiter.ConcurrentSubmissions.ToString());
				return stringBuilder.ToString();
			}
		}

		private SubmissionsInProgress SubmissionsInProgress
		{
			get
			{
				if (this.submissionsInProgress == null)
				{
					this.submissionsInProgress = new SubmissionsInProgress(Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions);
				}
				return this.submissionsInProgress;
			}
		}

		public static void InitializePerformanceCounterMaintenance()
		{
			StoreDriverSubmission.performanceCounterMaintenanceTimer = new GuardedTimer(new TimerCallback(StoreDriverSubmission.RefreshPerformanceCounters), null, TimeSpan.FromSeconds(60.0));
		}

		public static void ShutdownPerformanceCounterMaintenance()
		{
			if (StoreDriverSubmission.performanceCounterMaintenanceTimer != null)
			{
				StoreDriverSubmission.performanceCounterMaintenanceTimer.Dispose(true);
				StoreDriverSubmission.performanceCounterMaintenanceTimer = null;
			}
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			StoreDriverSubmission.eventLogger.LogEvent(tuple, periodicKey, messageArgs);
		}

		public static string FormatIPAddress(IPAddress address)
		{
			return "[" + address.ToString() + "]";
		}

		public void Continue()
		{
			lock (this.syncObject)
			{
				this.paused = false;
			}
		}

		public void ExpireOldSubmissionConnections()
		{
			SubmissionConnectionPool.ExpireOldConnections();
		}

		public void Pause()
		{
			lock (this.syncObject)
			{
				this.paused = true;
			}
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			lock (this.syncObject)
			{
				this.paused = initiallyPaused;
				ADNotificationAdapter.RunADOperation(delegate()
				{
					try
					{
						StoreDriverSubmission.localIp = Dns.GetHostEntry(Dns.GetHostName());
					}
					catch (SocketException ex)
					{
						this.storeDriverTracer.StoreDriverSubmissionTracer.TraceFail<string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Start failed: {0}", ex.ToString());
						StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_StoreDriverSubmissionGetLocalIPFailure, null, new object[]
						{
							ex
						});
						throw new TransportComponentLoadFailedException(ex.Message, ex);
					}
					StoreDriverSubmission.receivedHeaderTcpInfo = StoreDriverSubmission.FormatIPAddress(StoreDriverSubmission.localIp.AddressList[0]);
					this.storeDriverTracer.StoreDriverSubmissionTracer.TracePass(this.storeDriverTracer.MessageProbeActivityId, 0L, "Start submission");
					this.StartSubmission();
				}, 1);
			}
		}

		public void Stop()
		{
			this.storeDriverTracer.StoreDriverSubmissionTracer.TracePass(this.storeDriverTracer.MessageProbeActivityId, 0L, "Stop StoreDriverSubmission");
			if (!this.stopped)
			{
				this.stopped = true;
			}
			lock (this.syncObject)
			{
				this.StopSubmission();
				ProcessAccessManager.UnregisterComponent(this);
			}
		}

		public void Retire()
		{
			this.Stop();
		}

		public void Load()
		{
			lock (this.syncObject)
			{
				ProcessAccessManager.RegisterComponent(this);
			}
			try
			{
				MExEvents.Initialize(Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config"), ProcessTransportRole.MailboxSubmission, LatencyAgentGroup.MailboxTransportSubmissionStoreDriverSubmission, "Microsoft.Exchange.Data.Transport.StoreDriver.StoreDriverAgent");
				StoreDriverSubmission.InitializePerformanceCounterMaintenance();
			}
			catch (ExchangeConfigurationException ex)
			{
				this.storeDriverTracer.StoreDriverSubmissionTracer.TraceFail(this.storeDriverTracer.MessageProbeActivityId, (long)this.GetHashCode(), "StoreDriversubmission.Load threw ExchangeConfigurationException: shutting down service.");
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_CannotStartAgents, null, new object[]
				{
					ex.LocalizedString,
					ex
				});
				this.Stop();
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Unload()
		{
			lock (this.syncObject)
			{
				ProcessAccessManager.UnregisterComponent(this);
			}
			MExEvents.Shutdown();
			StoreDriverSubmission.ShutdownPerformanceCounterMaintenance();
		}

		public MailSubmissionResult SubmitMessage(string serverDN, Guid mailboxGuid, Guid mdbGuid, string databaseName, long eventCounter, byte[] entryId, byte[] parentEntryId, string serverFqdn, IPAddress networkAddressBytes, DateTime originalCreateTime, bool isPublicFolder, TenantPartitionHint tenantHint, string mailboxHopLatency, QuarantineHandler quarantineHandler, SubmissionPoisonHandler submissionPoisonHandler, LatencyTracker latencyTracker)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serverDN", serverDN);
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfEmpty("mdbGuid", mdbGuid);
			ArgumentValidator.ThrowIfNullOrEmpty("databaseName", databaseName);
			ArgumentValidator.ThrowIfNull("entryId", entryId);
			ArgumentValidator.ThrowIfInvalidValue<int>("entryId", entryId.Length, (int value) => value > 0);
			ArgumentValidator.ThrowIfNull("parentEntryId", parentEntryId);
			ArgumentValidator.ThrowIfInvalidValue<int>("parentEntryId", parentEntryId.Length, (int value) => value > 0);
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			ArgumentValidator.ThrowIfNull("networkAddressBytes", networkAddressBytes);
			ArgumentValidator.ThrowIfNullOrEmpty("mailboxHopLatency", mailboxHopLatency);
			ArgumentValidator.ThrowIfNull("quarantineHandler", quarantineHandler);
			ArgumentValidator.ThrowIfNull("submissionPoisonHandler", submissionPoisonHandler);
			bool shouldDeprioritize = false;
			if (SubmissionConfiguration.Instance.App.SenderRateDeprioritizationEnabled)
			{
				long num = this.rateTrackerForDeprioritization.IncrementSenderRate(mailboxGuid, originalCreateTime);
				if (num > (long)SubmissionConfiguration.Instance.App.SenderRateDeprioritizationThreshold)
				{
					shouldDeprioritize = true;
				}
			}
			bool shouldThrottle = false;
			if (SubmissionConfiguration.Instance.App.SenderRateThrottlingEnabled)
			{
				long num2 = this.rateTrackerForThrottling.IncrementSenderRate(mailboxGuid, originalCreateTime);
				if (num2 > (long)SubmissionConfiguration.Instance.App.SenderRateThrottlingThreshold)
				{
					shouldThrottle = true;
					this.rateTrackerForThrottling.ResetSenderRate(mailboxGuid, originalCreateTime);
				}
			}
			MapiSubmissionInfo submissionInfo = new MapiSubmissionInfo(serverDN, mailboxGuid, entryId, parentEntryId, eventCounter, serverFqdn, networkAddressBytes, mdbGuid, databaseName, originalCreateTime, isPublicFolder, tenantHint, mailboxHopLatency, latencyTracker, shouldDeprioritize, shouldThrottle, this.storeDriverTracer);
			return this.SubmitMessageImpl(submissionInfo, submissionPoisonHandler, quarantineHandler);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "StoreDriverSubmission";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("exceptions", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("callstacks", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = flag3 || parameters.Argument.IndexOf("basic", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag5 = parameters.Argument.IndexOf("currentThreads", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag6 = flag4 || parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag7 = !flag6 || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			if (flag7)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, basic, verbose, exceptions, callstacks, currentThreads, help."));
			}
			if (flag6)
			{
				SubmissionConfiguration.Instance.App.AddDiagnosticInfo(xelement);
			}
			if (flag4)
			{
				xelement.Add(new XElement("submittingThreads", SubmissionThreadLimiter.ConcurrentSubmissions));
				xelement.Add(SubmissionThreadLimiter.DatabaseThreadMap.GetDiagnosticInfo(new XElement("SubmissionDatabaseThreadMap")));
			}
			if (flag5 && this.SubmissionsInProgress != null)
			{
				xelement.Add(this.SubmissionsInProgress.GetDiagnosticInfo());
			}
			if (flag3)
			{
				xelement.Add(MailItemSubmitter.GetDiagnosticInfo());
			}
			if (flag)
			{
				StoreDriverSubmission.DumpExceptionStatistics(xelement);
			}
			if (flag2)
			{
				StoreDriverSubmission.DumpExceptionCallstacks(xelement);
			}
			return xelement;
		}

		internal static void DumpExceptionStatistics(XElement storeDriverElement)
		{
			XElement xelement = new XElement("ExceptionStatisticRecords");
			lock (StoreDriverSubmission.submissionExceptionStatisticRecords)
			{
				XElement xelement2 = new XElement("SubmissionExceptionStatistics");
				foreach (KeyValuePair<string, StoreDriverSubmission.ExceptionStatisticRecord<StoreDriverSubmission.SubmissionOccurrenceRecord>> keyValuePair in StoreDriverSubmission.submissionExceptionStatisticRecords)
				{
					XElement xelement3 = new XElement("Exception");
					xelement3.Add(new XAttribute("HitCount", keyValuePair.Value.CountSinceServiceStart));
					xelement3.Add(new XAttribute("Type", keyValuePair.Key));
					foreach (StoreDriverSubmission.SubmissionOccurrenceRecord submissionOccurrenceRecord in keyValuePair.Value.LastOccurrences)
					{
						xelement3.Add(submissionOccurrenceRecord.GetDiagnosticInfo());
					}
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			storeDriverElement.Add(xelement);
		}

		internal static void DumpExceptionCallstacks(XElement storeDriverElement)
		{
			XElement xelement = new XElement("ExceptionCallstackRecords");
			lock (StoreDriverSubmission.exceptionSubmissionCallstackRecords)
			{
				XElement xelement2 = new XElement("SubmissionCallstackRecords");
				foreach (KeyValuePair<MessageAction, Dictionary<string, StoreDriverSubmission.SubmissionOccurrenceRecord>> keyValuePair in StoreDriverSubmission.exceptionSubmissionCallstackRecords)
				{
					XElement xelement3 = new XElement(keyValuePair.Key.ToString());
					foreach (KeyValuePair<string, StoreDriverSubmission.SubmissionOccurrenceRecord> keyValuePair2 in keyValuePair.Value)
					{
						XElement diagnosticInfo = keyValuePair2.Value.GetDiagnosticInfo();
						XElement xelement4 = new XElement("Callstack", keyValuePair2.Key);
						xelement4.Add(new XAttribute("Length", keyValuePair2.Key.Length));
						diagnosticInfo.Add(xelement4);
						xelement3.Add(diagnosticInfo);
					}
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			storeDriverElement.Add(xelement);
		}

		internal static void RecordExceptionForDiagnostics(MessageStatus messageStatus, IMessageConverter messageConverter)
		{
			if (messageStatus.Exception != null && (messageStatus.Action == MessageAction.NDR || messageStatus.Action == MessageAction.Retry || messageStatus.Action == MessageAction.RetryQueue || messageStatus.Action == MessageAction.Reroute || messageStatus.Action == MessageAction.RetryMailboxServer || messageStatus.Action == MessageAction.Skip) && messageConverter.IsOutbound)
			{
				StoreDriverSubmission.UpdateSubmissionExceptionStatisticRecords(messageStatus, Components.TransportAppConfig.MapiSubmission.MaxStoreDriverSubmissionExceptionOccurrenceHistoryPerException, Components.Configuration.AppConfig.MapiSubmission.MaxStoreDriverSubmissionExceptionCallstackHistoryPerBucket, (MailItemSubmitter)messageConverter);
			}
		}

		internal static void ClearExceptionRecords()
		{
			lock (StoreDriverSubmission.exceptionSubmissionCallstackRecords)
			{
				StoreDriverSubmission.exceptionSubmissionCallstackRecords.Clear();
			}
			lock (StoreDriverSubmission.submissionExceptionStatisticRecords)
			{
				StoreDriverSubmission.submissionExceptionStatisticRecords.Clear();
			}
		}

		internal static void UpdateSubmissionExceptionStatisticRecords(MessageStatus messageStatus, int lastOccurrencesPerException, int callstacksPerBucket, MailItemSubmitter mailItemSubmitter)
		{
			MapiSubmissionInfo mapiSubmissionInfo = mailItemSubmitter.SubmissionInfo as MapiSubmissionInfo;
			StoreDriverSubmission.SubmissionOccurrenceRecord submissionOccurrenceRecord = new StoreDriverSubmission.SubmissionOccurrenceRecord(DateTime.UtcNow, mailItemSubmitter.StartTime, mailItemSubmitter.SubmissionInfo.MdbGuid, (mapiSubmissionInfo == null) ? Guid.Empty : mapiSubmissionInfo.MailboxGuid, (mapiSubmissionInfo == null) ? null : mapiSubmissionInfo.EntryId, (mapiSubmissionInfo == null) ? null : mapiSubmissionInfo.ParentEntryId, (mapiSubmissionInfo == null) ? 0L : mapiSubmissionInfo.EventCounter, mailItemSubmitter.OrganizationId, mailItemSubmitter.ErrorCode, mailItemSubmitter.SubmissionInfo.MailboxFqdn, mailItemSubmitter.Item.HasMessageItem ? mailItemSubmitter.Item.Item.InternetMessageId : null, mailItemSubmitter.SubmissionConnectionId, mailItemSubmitter.MessageSize, mailItemSubmitter.RecipientCount, mailItemSubmitter.Result.Sender, mailItemSubmitter.Stage);
			if (lastOccurrencesPerException > 0)
			{
				string key = StoreDriverSubmission.GenerateExceptionKey(messageStatus);
				lock (StoreDriverSubmission.submissionExceptionStatisticRecords)
				{
					StoreDriverSubmission.ExceptionStatisticRecord<StoreDriverSubmission.SubmissionOccurrenceRecord> value;
					if (!StoreDriverSubmission.submissionExceptionStatisticRecords.TryGetValue(key, out value))
					{
						value = default(StoreDriverSubmission.ExceptionStatisticRecord<StoreDriverSubmission.SubmissionOccurrenceRecord>);
						value.LastOccurrences = new Queue<StoreDriverSubmission.SubmissionOccurrenceRecord>(lastOccurrencesPerException);
					}
					if (value.LastOccurrences.Count == lastOccurrencesPerException)
					{
						value.LastOccurrences.Dequeue();
					}
					value.LastOccurrences.Enqueue(submissionOccurrenceRecord);
					value.CountSinceServiceStart++;
					StoreDriverSubmission.submissionExceptionStatisticRecords[key] = value;
				}
			}
			StoreDriverSubmission.UpdateSubmissionExceptionCallstackRecords(messageStatus, callstacksPerBucket, submissionOccurrenceRecord);
		}

		internal void ThrowIfStopped()
		{
			if (this.stopped)
			{
				throw new StoreDriverSubmissionRetiredException();
			}
		}

		private static Exception FaultInjectionCallback(string exceptionType)
		{
			LocalizedString localizedString = new LocalizedString("Fault injection.");
			if (exceptionType.Equals("Microsoft.Exchange.Data.Storage.StoragePermanentException", StringComparison.OrdinalIgnoreCase))
			{
				return new StoragePermanentException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.Data.Storage.StorageTransientException", StringComparison.OrdinalIgnoreCase))
			{
				return new StorageTransientException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Mapi.MapiExceptionSessionLimit", StringComparison.OrdinalIgnoreCase))
			{
				Exception innerException = MapiExceptionHelper.SessionLimitException(localizedString);
				return new TooManyObjectsOpenedException(localizedString, innerException);
			}
			if (exceptionType.Equals("Microsoft.Mapi.MapiExceptionUnconfigured", StringComparison.OrdinalIgnoreCase))
			{
				Exception innerException2 = MapiExceptionHelper.UnconfiguredException(localizedString);
				return new MailboxUnavailableException(localizedString, innerException2);
			}
			if (exceptionType.Equals("CannotGetSiteInfoException", StringComparison.OrdinalIgnoreCase))
			{
				return new StoragePermanentException(localizedString, new CannotGetSiteInfoException(localizedString));
			}
			if (exceptionType.Equals("System.ArgumentException", StringComparison.OrdinalIgnoreCase))
			{
				return new ArgumentException(localizedString);
			}
			if (exceptionType.StartsWith("Microsoft.Mapi.MapiException", StringComparison.OrdinalIgnoreCase))
			{
				return FaultInjectionHelper.CreateXsoExceptionFromMapiException(exceptionType, localizedString);
			}
			return new ApplicationException(localizedString);
		}

		private static string GenerateExceptionKey(MessageStatus messageStatus)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(messageStatus.Action.ToString());
			stringBuilder.Append(":");
			stringBuilder.Append(messageStatus.Exception.GetType().FullName);
			Exception ex = messageStatus.Exception;
			if (ex is SmtpResponseException)
			{
				stringBuilder.Append(";");
				stringBuilder.Append(messageStatus.Response.StatusCode);
				stringBuilder.Append(";");
				stringBuilder.Append(messageStatus.Response.EnhancedStatusCode);
			}
			while (ex.InnerException != null)
			{
				stringBuilder.Append("~");
				stringBuilder.Append(ex.InnerException.GetType().Name);
				ex = ex.InnerException;
			}
			return stringBuilder.ToString();
		}

		private static void UpdateSubmissionExceptionCallstackRecords(MessageStatus messageStatus, int callstacksPerBucket, StoreDriverSubmission.SubmissionOccurrenceRecord occurrenceRecord)
		{
			if (callstacksPerBucket > 0)
			{
				string key = messageStatus.Exception.ToString();
				lock (StoreDriverSubmission.exceptionSubmissionCallstackRecords)
				{
					Dictionary<string, StoreDriverSubmission.SubmissionOccurrenceRecord> dictionary;
					if (!StoreDriverSubmission.exceptionSubmissionCallstackRecords.TryGetValue(messageStatus.Action, out dictionary))
					{
						dictionary = new Dictionary<string, StoreDriverSubmission.SubmissionOccurrenceRecord>(callstacksPerBucket);
						StoreDriverSubmission.exceptionSubmissionCallstackRecords[messageStatus.Action] = dictionary;
					}
					StoreDriverSubmission.SubmissionOccurrenceRecord submissionOccurrenceRecord;
					if (!dictionary.TryGetValue(key, out submissionOccurrenceRecord) && dictionary.Count == callstacksPerBucket)
					{
						DateTime t = DateTime.MaxValue;
						string key2 = null;
						foreach (KeyValuePair<string, StoreDriverSubmission.SubmissionOccurrenceRecord> keyValuePair in dictionary)
						{
							if (keyValuePair.Value.Timestamp < t)
							{
								t = keyValuePair.Value.Timestamp;
								key2 = keyValuePair.Key;
							}
						}
						dictionary.Remove(key2);
					}
					dictionary[key] = occurrenceRecord;
				}
			}
		}

		private static void RefreshPerformanceCounters(object state)
		{
			StoreDriverSubmissionDatabasePerfCounters.RefreshPerformanceCounters();
		}

		private MailSubmissionResult SubmitMessageImpl(MapiSubmissionInfo submissionInfo, SubmissionPoisonHandler submissionPoisonHandler, QuarantineHandler quarantineHandler)
		{
			MailSubmissionResult mailSubmissionResult = new MailSubmissionResult();
			if (this.Retired)
			{
				mailSubmissionResult.ErrorCode = 2214592514U;
				return mailSubmissionResult;
			}
			string mailboxFqdn = submissionInfo.MailboxFqdn;
			string database = submissionInfo.MdbGuid.ToString();
			MailSubmissionResult result;
			using (submissionInfo.GetTraceFilter())
			{
				using (SubmissionConnectionWrapper connection = SubmissionConnectionPool.GetConnection(mailboxFqdn, database))
				{
					using (SubmissionThreadLimiter submissionThreadLimiter = new SubmissionThreadLimiter())
					{
						SubmissionPoisonContext submissionPoisonContext = null;
						try
						{
							submissionThreadLimiter.BeginSubmission(connection.Id, mailboxFqdn, database);
							if (this.Retired)
							{
								mailSubmissionResult.ErrorCode = 2214592514U;
								connection.SubmissionAborted("Retiring.");
								result = mailSubmissionResult;
							}
							else
							{
								this.storeDriverTracer.StoreDriverSubmissionTracer.TracePfdPass<int, MapiSubmissionInfo>(this.storeDriverTracer.MessageProbeActivityId, 0L, "PFD ESD {0} Processing SubmitMessage for {1}", 27547, submissionInfo);
								submissionPoisonContext = submissionInfo.GetPoisonContext();
								MailItemSubmitter mailItemSubmitter2;
								MailItemSubmitter mailItemSubmitter = mailItemSubmitter2 = new MailItemSubmitter(connection.Id, submissionInfo, this.sendAsManager, submissionPoisonHandler, submissionPoisonContext, this);
								try
								{
									QuarantineInfoContext quarantineInfoContext;
									TimeSpan timeSpan;
									if (SubmissionConfiguration.Instance.App.EnableMailboxQuarantine && quarantineHandler.IsResourceQuarantined(submissionPoisonContext.ResourceGuid, out quarantineInfoContext, out timeSpan))
									{
										mailSubmissionResult.ErrorCode = 1140850696U;
										mailSubmissionResult.QuarantineTimeSpan = timeSpan;
										mailSubmissionResult.DiagnosticInfo = string.Format("{0}:{1}, QuarantineRemainingTimeSpan:{2}", "QuarantineStart", quarantineInfoContext.QuarantineStartTime, timeSpan);
										connection.SubmissionFailed(string.Format("Resource {0} is in quarantined state", submissionPoisonContext.ResourceGuid));
										return mailSubmissionResult;
									}
									if (this.PoisonMessageDectionEnabled && submissionPoisonHandler.VerifyPoisonMessage(submissionPoisonContext))
									{
										this.LogPoisonMessageMTL(submissionInfo, submissionPoisonContext);
										if (SubmissionConfiguration.Instance.App.EnableSendNdrForPoisonMessage)
										{
											if (!submissionPoisonHandler.VerifyPoisonNdrSent(submissionPoisonContext))
											{
												mailItemSubmitter.HandlePoisonMessageNdrSubmission();
												mailSubmissionResult.ErrorCode = StoreDriverSubmissionUtils.MapSubmissionStatusErrorCodeToPoisonErrorCode(mailItemSubmitter.Result.ErrorCode);
											}
										}
										else
										{
											mailSubmissionResult.ErrorCode = 3U;
										}
										connection.SubmissionAborted(string.Format("Poison Context Info: Resource = {0};EventCounter = {1}.", submissionPoisonContext.ResourceGuid, submissionPoisonContext.MapiEventCounter));
										return mailSubmissionResult;
									}
									Thread currentThread = Thread.CurrentThread;
									try
									{
										this.SubmissionsInProgress[currentThread] = mailItemSubmitter;
										mailItemSubmitter.Submit();
										mailSubmissionResult.RemoteHostName = mailItemSubmitter.Result.RemoteHostName;
									}
									finally
									{
										this.SubmissionsInProgress.Remove(currentThread);
									}
								}
								finally
								{
									if (mailItemSubmitter2 != null)
									{
										((IDisposable)mailItemSubmitter2).Dispose();
									}
								}
								if (mailItemSubmitter.Result.ErrorCode == 0U)
								{
									connection.SubmissionSuccessful(mailItemSubmitter.MessageSize, mailItemSubmitter.RecipientCount);
								}
								else
								{
									StringBuilder stringBuilder = new StringBuilder();
									stringBuilder.Append("HResult: ");
									stringBuilder.Append(mailItemSubmitter.Result.ErrorCode.ToString());
									stringBuilder.Append("; DiagnosticInfo: ");
									stringBuilder.Append(mailItemSubmitter.Result.DiagnosticInfo);
									connection.SubmissionFailed(stringBuilder.ToString());
								}
								if (mailItemSubmitter.Result.ErrorCode == 3U)
								{
									submissionInfo.LogEvent(SubmissionInfo.Event.StoreDriverSubmissionPoisonMessageInSubmission);
								}
								result = mailItemSubmitter.Result;
							}
						}
						catch (ThreadLimitExceededException ex)
						{
							connection.SubmissionAborted(ex.Message);
							mailSubmissionResult.ErrorCode = 1090519042U;
							mailSubmissionResult.DiagnosticInfo = ex.Message;
							result = mailSubmissionResult;
						}
						catch (Exception exception)
						{
							try
							{
								submissionPoisonHandler.SavePoisonContext(submissionPoisonContext);
								string exceptionDiagnosticInfo = StorageExceptionHandler.GetExceptionDiagnosticInfo(exception);
								connection.SubmissionFailed("Exception: " + exceptionDiagnosticInfo);
								submissionInfo.LogEvent(SubmissionInfo.Event.StoreDriverSubmissionPoisonMessage, exception);
								FailFast.Fail(exception);
							}
							catch (Exception ex2)
							{
								StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_StoreDriverSubmissionFailFastFailure, null, new object[]
								{
									ex2
								});
							}
							result = mailSubmissionResult;
						}
					}
				}
			}
			return result;
		}

		private void LogPoisonMessageMTL(SubmissionInfo submissionInfo, SubmissionPoisonContext submissionPoisonContext)
		{
			this.storeDriverTracer.StoreDriverSubmissionTracer.TracePass<SubmissionInfo>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Found poison message for {0}", submissionInfo);
			MsgTrackPoisonInfo msgTrackingPoisonInfo = new MsgTrackPoisonInfo(submissionInfo.NetworkAddress, submissionInfo.MailboxFqdn, StoreDriverSubmission.LocalIPAddress, submissionPoisonContext.MapiEventCounter.ToString() + ": " + submissionPoisonContext.ResourceGuid.ToString());
			MessageTrackingLog.TrackPoisonMessage(MessageTrackingSource.STOREDRIVER, msgTrackingPoisonInfo);
			submissionInfo.LogEvent(SubmissionInfo.Event.StoreDriverSubmissionPoisonMessageInSubmission);
		}

		private string GetSubmissionContext(MapiSubmissionInfo submissionInfo)
		{
			return string.Format(CultureInfo.InvariantCulture, "MDB:{0}, Mailbox:{1}, Event:{2}, CreationTime:{3}", new object[]
			{
				submissionInfo.MdbGuid,
				submissionInfo.MailboxGuid,
				submissionInfo.EventCounter,
				submissionInfo.OriginalCreateTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo)
			});
		}

		private void StartSubmission()
		{
			try
			{
				ProcessAccessManager.RegisterComponent(this);
			}
			catch (Exception ex)
			{
				this.storeDriverTracer.StoreDriverSubmissionTracer.TraceFail<string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Failed to start Store Driver Submission: {0}", ex.ToString());
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_StoreDriverSubmissionStartFailure, null, new object[]
				{
					ex
				});
				throw new TransportComponentLoadFailedException(ex.Message, ex);
			}
		}

		private void StopSubmission()
		{
			this.storeDriverTracer.StoreDriverSubmissionTracer.TracePass(this.storeDriverTracer.MessageProbeActivityId, 0L, "Stop submission");
			while (SubmissionThreadLimiter.ConcurrentSubmissions > 0)
			{
				Thread.Sleep(500);
			}
		}

		private const string ProcessAccessManagerComponentName = "StoreDriverSubmission";

		private static IPHostEntry localIp;

		private static string receivedHeaderTcpInfo;

		private static ExEventLog eventLogger = new ExEventLog(new Guid("{597e9983-c5b7-425c-b17f-983e354b783a}"), "MSExchange Store Driver Submission");

		private static Dictionary<MessageAction, Dictionary<string, StoreDriverSubmission.SubmissionOccurrenceRecord>> exceptionSubmissionCallstackRecords = new Dictionary<MessageAction, Dictionary<string, StoreDriverSubmission.SubmissionOccurrenceRecord>>(6);

		private static Dictionary<string, StoreDriverSubmission.ExceptionStatisticRecord<StoreDriverSubmission.SubmissionOccurrenceRecord>> submissionExceptionStatisticRecords = new Dictionary<string, StoreDriverSubmission.ExceptionStatisticRecord<StoreDriverSubmission.SubmissionOccurrenceRecord>>(100);

		private static GuardedTimer performanceCounterMaintenanceTimer;

		private readonly object syncObject = new object();

		private readonly SenderRateTracker rateTrackerForDeprioritization = new SenderRateTracker(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(10.0));

		private readonly SenderRateTracker rateTrackerForThrottling = new SenderRateTracker(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(10.0));

		private readonly SendAsManager sendAsManager = new SendAsManager();

		private IStoreDriverTracer storeDriverTracer;

		private bool paused;

		private volatile bool stopped;

		private SubmissionsInProgress submissionsInProgress;

		private struct ExceptionStatisticRecord<T>
		{
			internal Queue<T> LastOccurrences;

			internal int CountSinceServiceStart;
		}

		private struct SubmissionOccurrenceRecord
		{
			internal SubmissionOccurrenceRecord(DateTime timestamp, DateTime submissionStart, Guid mdbGuid, Guid mailboxGuid, byte[] entryID, byte[] parentEntryID, long eventCounter, OrganizationId organizationID, uint errorCode, string mailboxServer, string messageID, ulong sessionID, long messageSize, int recipientCount, string sender, MailItemSubmitter.SubmissionStage stage)
			{
				this.timestamp = timestamp;
				this.submissionStart = submissionStart;
				this.mdbGuid = mdbGuid;
				this.mailboxGuid = mailboxGuid;
				this.entryID = entryID;
				this.parentEntryID = parentEntryID;
				this.eventCounter = eventCounter;
				this.organizationID = organizationID;
				this.errorCode = errorCode;
				this.mailboxServer = mailboxServer;
				this.messageID = messageID;
				this.sessionID = sessionID;
				this.messageSize = messageSize;
				this.recipientCount = recipientCount;
				this.sender = sender;
				this.stage = stage;
			}

			internal DateTime Timestamp
			{
				get
				{
					return this.timestamp;
				}
			}

			internal XElement GetDiagnosticInfo()
			{
				return new XElement("Occurrence", new object[]
				{
					new XElement("HitUtc", this.timestamp.ToString(CultureInfo.InvariantCulture)),
					new XElement("SubmissionStart", this.submissionStart.ToString(CultureInfo.InvariantCulture)),
					new XElement("MdbGuid", this.mdbGuid),
					new XElement("MailboxGuid", this.mailboxGuid),
					new XElement("EntryID", this.entryID),
					new XElement("ParentEntryID", this.parentEntryID),
					new XElement("EventCounter", this.eventCounter),
					new XElement("OrgID", this.organizationID),
					new XElement("Stage", this.stage),
					new XElement("ErrorCode", this.errorCode),
					new XElement("MailboxServer", this.mailboxServer),
					new XElement("MessageID", this.messageID),
					new XElement("SessionID", this.sessionID),
					new XElement("MessageSize", this.messageSize),
					new XElement("RecipientCount", this.recipientCount)
				});
			}

			private DateTime timestamp;

			private DateTime submissionStart;

			private Guid mdbGuid;

			private Guid mailboxGuid;

			private byte[] entryID;

			private byte[] parentEntryID;

			private long eventCounter;

			private OrganizationId organizationID;

			private MailItemSubmitter.SubmissionStage stage;

			private uint errorCode;

			private string mailboxServer;

			private string messageID;

			private ulong sessionID;

			private long messageSize;

			private int recipientCount;

			private string sender;
		}
	}
}
