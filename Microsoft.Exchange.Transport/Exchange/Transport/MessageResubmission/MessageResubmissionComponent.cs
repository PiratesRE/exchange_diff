using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Rpc.MailSubmission;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal class MessageResubmissionComponent : IStartableTransportComponent, ITransportComponent, IMessageResubmissionRpcServer, IDiagnosable
	{
		internal static List<MailRecipient> GetSuppressedRecipients(TransportMailItem mailItem, out bool isFromProbeResubmitRequest)
		{
			List<MailRecipient> list = new List<MailRecipient>();
			isFromProbeResubmitRequest = false;
			Destination destination;
			Guid activityId;
			bool flag = ResubmitRequestFacade.TryParseResubmittedHeader(mailItem, out destination, out activityId);
			if (!flag || destination.Type == Destination.DestinationType.Conditional || destination.Type == Destination.DestinationType.Shadow)
			{
				return list;
			}
			if (mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-TestResubmission") != null)
			{
				isFromProbeResubmitRequest = true;
			}
			foreach (MailRecipient mailRecipient in mailItem.Recipients.AllUnprocessed)
			{
				bool flag2 = false;
				Guid? mdbguid = mailRecipient.GetMDBGuid();
				if (mdbguid != null && NextHopType.IsMailboxDeliveryType(mailRecipient.NextHop.NextHopType.DeliveryType) && destination.ToGuid() == mdbguid.Value)
				{
					flag2 = true;
					if (mailItem.SystemProbeId != Guid.Empty)
					{
						MessageResubmissionComponent.PublishEventNotification(mailItem, "Operation", "SafetyNetDeliver");
					}
				}
				else
				{
					list.Add(mailRecipient);
				}
				if (isFromProbeResubmitRequest)
				{
					SystemProbeHelper.MessageResubmissionTracer.TracePass<long, Guid, bool>(activityId, 0L, "Skipping submission of message for probe resubmit request. Message Id = {0}, Recipient Mdb = {1}, Would be delivered = {2}", mailRecipient.MsgId, mdbguid ?? Guid.Empty, flag2);
					ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "Not delivering this message because it was resubmitted using a probe resubmission request");
					if (flag2)
					{
						list.Add(mailRecipient);
					}
				}
			}
			return list;
		}

		public string CurrentState
		{
			get
			{
				return null;
			}
		}

		public void SetLoadTimeDependencies(TransportAppConfig.IMessageResubmissionConfig config, IMessagingDatabase database)
		{
			this.config = config;
			this.messagingDatabase = database;
		}

		public void SetRunTimeDependencies(IPrimaryServerInfoMap primaryServerInfoMap)
		{
			this.primaryServerInfoMap = primaryServerInfoMap;
		}

		public void Load()
		{
			ExTraceGlobals.MessageResubmissionTracer.Information((long)this.GetHashCode(), "Loading MessageResubmit Component.");
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			MessageResubmissionPerfCounters.Instance.ResetCounters();
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.LoadResubmitRequests();
			this.ExpireCompletedResubmitRequests();
			ExTraceGlobals.MessageResubmissionTracer.TracePerformance<long>((long)this.GetHashCode(), "Loading Component took {0}ms", stopwatch.ElapsedMilliseconds);
		}

		public void Unload()
		{
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			ExTraceGlobals.MessageResubmissionTracer.Information((long)this.GetHashCode(), "Unloading MessageResubmit Component.");
			using (this.syncLock.AcquireWriteLock())
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				foreach (ResubmitRequestFacade resubmitRequestFacade in this.resubmitRequests)
				{
					resubmitRequestFacade.Dispose();
				}
				this.resubmitRequests.Clear();
				ExTraceGlobals.MessageResubmissionTracer.TracePerformance<long>((long)this.GetHashCode(), "Unloading Component took {0}ms", stopwatch.ElapsedMilliseconds);
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			if (!this.MessageResubmissionEnabled)
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "MessageResubmit Component start skipped because its not enabled.");
				return;
			}
			this.targetRunningState = targetRunningState;
			this.paused = (initiallyPaused || !this.ShouldExecute());
			this.resubmitMessageTimer = new GuardedTimer(new TimerCallback(this.ResubmitMessageTimerCallback), null, this.Config.ResubmissionInitialDelay, this.Config.ResubmissionInterval);
			this.resubmitRequestCleanupTimer = new GuardedTimer(delegate(object obj)
			{
				this.ExpireCompletedResubmitRequests();
			}, null, this.GetResubmitRequestExpiryTimerPeriod());
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "MessageResubmit Component Started");
		}

		public void Stop()
		{
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			this.resubmitMessageTimer.Dispose(true);
			this.resubmitRequestCleanupTimer.Dispose(true);
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "MessageResubmit Component Stopped");
		}

		public void Pause()
		{
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			this.paused = true;
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "MessageResubmit Component Paused");
		}

		public void Continue()
		{
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			if (this.ShouldExecute())
			{
				this.paused = false;
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "MessageResubmit Component Unpaused (Resumed)");
				return;
			}
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug<ServiceState>(0L, "MessageResubmit Component unpause skipped due to target state {0}", this.targetRunningState);
		}

		public void StoreRecipient(MailRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.DeliveredDestination != null)
			{
				recipient.DeliveryTime = new DateTime?(DateTime.UtcNow);
				recipient.AddToSafetyNet();
			}
		}

		public AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTimeInTicks, long endTimeInTicks, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return this.AddResubmitRequest(requestGuid, mdbGuid, startTimeInTicks, endTimeInTicks, unresponsivePrimaryServers, reservedBytes, null);
		}

		public AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTimeInTicks, long endTimeInTicks, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return this.AddResubmitRequest(requestGuid, Guid.Empty, startTimeInTicks, endTimeInTicks, unresponsivePrimaryServers, reservedBytes, conditions);
		}

		private AddResubmitRequestStatus AddResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTimeInTicks, long endTimeInTicks, string[] unresponsivePrimaryServers, byte[] reservedBytes, string conditions)
		{
			AddResubmitRequestStatus result;
			try
			{
				DateTime startTime = new DateTime(startTimeInTicks);
				DateTime endTime = new DateTime(endTimeInTicks);
				Destination destination = null;
				if (Guid.Empty.Equals(mdbGuid) && string.IsNullOrEmpty(conditions))
				{
					ExTraceGlobals.MessageResubmissionTracer.TraceError((long)this.GetHashCode(), "AddResubmitRequest Error: destination and conditions were both null");
					result = AddResubmitRequestStatus.InvalidOperation;
				}
				else
				{
					if (!Guid.Empty.Equals(mdbGuid))
					{
						ExTraceGlobals.MessageResubmissionTracer.TraceDebug<Guid, DateTime, DateTime>((long)this.GetHashCode(), "Creating Resubmit Request ({0},{1},{2})", mdbGuid, startTime, endTime);
						destination = new Destination(Destination.DestinationType.Mdb, mdbGuid);
					}
					else if (!string.IsNullOrEmpty(conditions))
					{
						ResubmitFilter resubmitFilter;
						if (!ResubmitFilter.TryBuild(conditions, out resubmitFilter))
						{
							ExTraceGlobals.MessageResubmissionTracer.TraceError((long)this.GetHashCode(), "AddConditionalResubmitRequest: Unable to parse the resquest.");
							return AddResubmitRequestStatus.InvalidOperation;
						}
						ExTraceGlobals.MessageResubmissionTracer.TraceDebug<string, DateTime, DateTime>((long)this.GetHashCode(), "Creating Conditional Resubmit Request ({0},{1},{2})", conditions, new DateTime(startTimeInTicks), new DateTime(endTimeInTicks));
						destination = new Destination(Destination.DestinationType.Conditional, conditions);
					}
					if (startTime >= endTime)
					{
						ExTraceGlobals.MessageResubmissionTracer.TraceError((long)this.GetHashCode(), "AddMdbResubmitRequest: startTime > endTime returning error");
						result = AddResubmitRequestStatus.InvalidOperation;
					}
					else if (!this.MessageResubmissionEnabled)
					{
						ExTraceGlobals.MessageResubmissionTracer.TraceError((long)this.GetHashCode(), "AddMdbResubmitRequest: MessageResubmission component is disabled.");
						result = AddResubmitRequestStatus.Disabled;
					}
					else
					{
						IEnumerable<PrimaryServerInfo> primaryServers = this.SelectPrimaryServersForShadowResubmission(unresponsivePrimaryServers, startTime, endTime);
						Guid correlationId = (requestGuid != Guid.Empty) ? requestGuid : Guid.NewGuid();
						using (this.syncLock.AcquireReadLock())
						{
							if (this.resubmitRequests.Any((ResubmitRequestFacade r) => !r.IsCompleted && r.IsSimilar(destination, startTime, endTime, primaryServers)))
							{
								ExTraceGlobals.MessageResubmissionTracer.TraceError<Destination, DateTime, DateTime>((long)this.GetHashCode(), "AddMdbResubmitRequest: Resubmit requests for the same parameters is already running ({0},{1},{2})", destination, startTime, endTime);
								MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DuplicateResubmitRequest, this.GetDiagnosticComponentName(), new object[]
								{
									destination,
									startTime,
									endTime
								});
								return AddResubmitRequestStatus.DuplicateRequest;
							}
							int num = this.resubmitRequests.Count((ResubmitRequestFacade request) => request.IsRunning());
							if (num >= this.Config.MaxResubmissionRequests)
							{
								ExTraceGlobals.MessageResubmissionTracer.TraceError<int, int>((long)this.GetHashCode(), "AddMdbResubmitRequest: Running resubmit requests count ({0}) exceeded the configured limit ({1})", num, this.Config.MaxResubmissionRequests);
								MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_MaxRunningResubmitRequest, this.GetDiagnosticComponentName(), new object[]
								{
									num,
									this.Config.MaxResubmissionRequests
								});
								return AddResubmitRequestStatus.MaxRunningRequestsExceeded;
							}
							int num2 = this.resubmitRequests.Count((ResubmitRequestFacade r) => r.CreationTime > DateTime.UtcNow.Subtract(this.Config.RecentResubmitRequestPeriod));
							if (num2 > this.Config.MaxRecentResubmissionRequests)
							{
								ExTraceGlobals.MessageResubmissionTracer.TraceError<TimeSpan, int, int>((long)this.GetHashCode(), "AddMdbResubmitRequest: Resubmit requests created in the last {0} is ({1}) and exceeded the configured limit ({2})", this.Config.RecentResubmitRequestPeriod, num, this.Config.MaxRecentResubmissionRequests);
								MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_MaxRecentResubmitRequest, this.GetDiagnosticComponentName(), new object[]
								{
									this.Config.RecentResubmitRequestPeriod,
									num,
									this.Config.MaxRecentResubmissionRequests
								});
								return AddResubmitRequestStatus.MaxRecentRequestsExceeded;
							}
						}
						bool flag = unresponsivePrimaryServers != null && unresponsivePrimaryServers.Length > 0;
						bool flag2;
						if (flag)
						{
							flag2 = unresponsivePrimaryServers.Any((string unresponseServer) => string.Equals(unresponseServer, Components.Configuration.LocalServer.TransportServer.Fqdn, StringComparison.InvariantCultureIgnoreCase));
						}
						else
						{
							flag2 = false;
						}
						bool wasCurrentServerUnresponsive = flag2;
						ResubmitRequestFacade resubmitRequestFacade = new ResubmitRequestFacade(correlationId, primaryServers, this.MessagingDatabase, destination, startTime, endTime, flag, wasCurrentServerUnresponsive, MessageResubmissionComponent.IsTestResubmitRequest(reservedBytes));
						if (!resubmitRequestFacade.IsRunning())
						{
							ExTraceGlobals.MessageResubmissionTracer.Information<DateTime, DateTime>((long)this.GetHashCode(), "AddMdbResubmitRequest: There are no valid primary servers that are being shadowed for the time range {0} to {1}", startTime, endTime);
						}
						else
						{
							using (this.syncLock.AcquireWriteLock())
							{
								this.resubmitRequests.Add(resubmitRequestFacade);
							}
							MessageResubmissionPerfCounters.Instance.IncrementRecentRequestCount(flag);
							MessageResubmissionPerfCounters.Instance.RecordResubmitRequestTimeSpan(endTime.Subtract(startTime));
						}
						result = AddResubmitRequestStatus.Success;
					}
				}
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = AddResubmitRequestStatus.Error;
			}
			return result;
		}

		public byte[] GetResubmitRequest(byte[] rawRequest)
		{
			byte[] result;
			try
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "GetResubmitRequest called");
				byte[] array;
				Dictionary<string, object> dictionary;
				ResubmitRequestId resubmitRequestId;
				if (rawRequest == null)
				{
					result = MessageResubmissionComponent.GetUnexpectedError("Expecting a non-empty request.");
				}
				else if (!MessageResubmissionComponent.TryGetResubmitParameterAndIdentity(rawRequest, out array, out resubmitRequestId, out dictionary))
				{
					result = array;
				}
				else
				{
					using (this.syncLock.AcquireReadLock())
					{
						result = Serialization.ObjectToBytes((from resubmitRequestFacade in this.resubmitRequests
						where resubmitRequestId == null || resubmitRequestFacade.RequestId == resubmitRequestId.ResubmitRequestRowId
						select resubmitRequestFacade.GetResubmitRequest() into resubmitRequestFacade
						where resubmitRequestFacade != null
						select resubmitRequestFacade).ToArray<ResubmitRequest>());
					}
				}
			}
			catch (Exception ex)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(ex);
				result = MessageResubmissionComponent.GetUnexpectedError(ex.Message);
			}
			return result;
		}

		public byte[] SetResubmitRequest(byte[] requestRaw)
		{
			byte[] result;
			try
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "SetResubmitRequest called");
				byte[] array;
				ResubmitRequestId resubmitRequestId;
				Dictionary<string, object> requestParameters;
				if (!MessageResubmissionComponent.TryGetResubmitParameterAndIdentity(requestRaw, out array, out resubmitRequestId, out requestParameters))
				{
					result = array;
				}
				else if (resubmitRequestId == null)
				{
					result = MessageResubmissionComponent.GetIdentityMissingError();
				}
				else
				{
					ResubmitRequestFacade resubmitRequest = this.GetResubmitRequest(resubmitRequestId, out array);
					ResubmitRequestState newState;
					if (resubmitRequest == null)
					{
						result = array;
					}
					else if (!MessageResubmissionComponent.TryGetNewStateToSet(requestParameters, out array, out newState))
					{
						result = array;
					}
					else if (!resubmitRequest.SetState(newState, out array))
					{
						result = array;
					}
					else
					{
						MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ModifiedMessageRepositoryRequest, null, new object[]
						{
							resubmitRequestId,
							resubmitRequest.GetDiagnosticInformationString()
						});
						result = Serialization.ObjectToBytes(ResubmitRequestResponse.SuccessResponse);
					}
				}
			}
			catch (Exception ex)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(ex);
				result = MessageResubmissionComponent.GetUnexpectedError(ex.Message);
			}
			return result;
		}

		public byte[] RemoveResubmitRequest(byte[] requestRaw)
		{
			byte[] result;
			try
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug(0L, "RemoveResubmitRequest called");
				byte[] array;
				ResubmitRequestId resubmitRequestId;
				Dictionary<string, object> dictionary;
				if (!MessageResubmissionComponent.TryGetResubmitParameterAndIdentity(requestRaw, out array, out resubmitRequestId, out dictionary))
				{
					result = array;
				}
				else if (resubmitRequestId == null)
				{
					result = MessageResubmissionComponent.GetIdentityMissingError();
				}
				else
				{
					ResubmitRequestFacade resubmitRequest = this.GetResubmitRequest(resubmitRequestId, out array);
					if (resubmitRequest == null)
					{
						result = array;
					}
					else
					{
						string diagnosticInformationString = resubmitRequest.GetDiagnosticInformationString();
						if (!resubmitRequest.Remove(out array))
						{
							result = array;
						}
						else
						{
							using (this.syncLock.AcquireWriteLock())
							{
								this.resubmitRequests.Remove(resubmitRequest);
							}
							MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_RemovedMessageRepositoryRequest, null, new object[]
							{
								resubmitRequestId.ResubmitRequestRowId,
								diagnosticInformationString
							});
							result = Serialization.ObjectToBytes(ResubmitRequestResponse.SuccessResponse);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(ex);
				result = MessageResubmissionComponent.GetUnexpectedError(ex.Message);
			}
			return result;
		}

		public TransportAppConfig.IMessageResubmissionConfig Config
		{
			get
			{
				return this.config;
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "SafetyNet";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			if (parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1)
			{
				xelement.Add(this.GetConfigurationInfo());
				xelement.Add(this.GetComponentStateInfo());
			}
			else if (parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1)
			{
				xelement.Add(this.GetConfigurationInfo());
			}
			else
			{
				xelement.Add(new XElement("help", "Supported arguments: config, verbose, help."));
			}
			return xelement;
		}

		private bool MessageResubmissionEnabled
		{
			get
			{
				return this.Config.MessageResubmissionEnabled;
			}
		}

		private IPrimaryServerInfoMap PrimaryServerInfoMap
		{
			get
			{
				return this.primaryServerInfoMap;
			}
		}

		private IMessagingDatabase MessagingDatabase
		{
			get
			{
				if (this.messagingDatabase == null)
				{
					throw new InvalidOperationException("Attempt to retrieve messagingDatabase instance before MessageResubmissionComponent is loaded.");
				}
				return this.messagingDatabase;
			}
		}

		private static byte[] GetIdentityMissingError()
		{
			return Serialization.ObjectToBytes(new ResubmitRequestResponse(ResubmitRequestResponseCode.UnexpectedError, Strings.IdentityParameterNotFound));
		}

		private static bool TryGetResubmitParameterAndIdentity(byte[] requestRaw, out byte[] response, out ResubmitRequestId resubmitRequestId, out Dictionary<string, object> requestParameters)
		{
			resubmitRequestId = null;
			response = null;
			Exception ex;
			if (!MessageResubmissionComponent.TryGetRequestParameters(requestRaw, out requestParameters, out ex))
			{
				response = MessageResubmissionComponent.GetUnexpectedError(ex.ToString());
				return false;
			}
			object obj;
			if (!requestParameters.TryGetValue("ResubmitRequestIdentity", out obj))
			{
				response = MessageResubmissionComponent.GetIdentityMissingError();
				return false;
			}
			if (obj == null)
			{
				return true;
			}
			string text = obj as string;
			if (text == null)
			{
				response = MessageResubmissionComponent.GetUnexpectedError("Unexpected Identity type " + obj.GetType());
				return false;
			}
			ResubmitRequestId resubmitRequestId2;
			if (!ResubmitRequestId.TryParse(text, out resubmitRequestId2))
			{
				response = MessageResubmissionComponent.GetUnexpectedError("Unexpected Identity Value " + text);
				return false;
			}
			resubmitRequestId = resubmitRequestId2;
			return true;
		}

		private static bool TryGetNewStateToSet(Dictionary<string, object> requestParameters, out byte[] errorResponse, out ResubmitRequestState newState)
		{
			errorResponse = null;
			newState = ResubmitRequestState.None;
			object obj;
			if (!requestParameters.TryGetValue("DumpsterRequestEnabled", out obj))
			{
				errorResponse = MessageResubmissionComponent.GetUnexpectedError("Enabled parameter is missing.");
				return false;
			}
			if (obj == null || obj.GetType() != typeof(bool))
			{
				errorResponse = MessageResubmissionComponent.GetUnexpectedError("Enabled parameter null or of unexpecte type.");
				return false;
			}
			newState = (((bool)obj) ? ResubmitRequestState.Running : ResubmitRequestState.Paused);
			return true;
		}

		private static bool TryGetRequestParameters(byte[] requestRaw, out Dictionary<string, object> requestParameters, out Exception exception)
		{
			exception = null;
			requestParameters = null;
			try
			{
				object obj = Serialization.BytesToObject(requestRaw);
				requestParameters = (obj as Dictionary<string, object>);
				if (requestParameters == null)
				{
					exception = new Exception("Unexpected Type " + obj);
				}
			}
			catch (SerializationException ex)
			{
				exception = ex;
			}
			catch (SecurityException ex2)
			{
				exception = ex2;
			}
			return requestParameters != null;
		}

		private static byte[] GetUnexpectedError(string error)
		{
			return Serialization.ObjectToBytes(new ResubmitRequestResponse(ResubmitRequestResponseCode.UnexpectedError, error));
		}

		private static bool IsTestResubmitRequest(byte[] reservedBytes)
		{
			if (reservedBytes == null || reservedBytes.Length == 0)
			{
				return false;
			}
			bool result;
			try
			{
				MdbefPropertyCollection mdbefPropertyCollection = MdbefPropertyCollection.Create(reservedBytes, 0, reservedBytes.Length);
				result = mdbefPropertyCollection.ContainsKey(65547U);
			}
			catch (MdbefException ex)
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceWarning<string>(0L, "Failed to parse the reserved bytes as a MdbefPropertyCollection. Error = {0}", ex.ToString());
				result = false;
			}
			return result;
		}

		private static void PublishEventNotification(TransportMailItem mailItem, string key, string value)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Transport.Name, Components.MessageResubmissionComponent.GetDiagnosticComponentName(), null, ResultSeverityLevel.Verbose);
			eventNotificationItem.AddCustomProperty("Key", key);
			eventNotificationItem.AddCustomProperty("Value", value);
			eventNotificationItem.StateAttribute1 = mailItem.SystemProbeId.ToString();
			eventNotificationItem.Publish(false);
		}

		private void LoadResubmitRequests()
		{
			using (this.syncLock.AcquireWriteLock())
			{
				Dictionary<long, ResubmitRequestFacade> dictionary = new Dictionary<long, ResubmitRequestFacade>();
				foreach (IReplayRequest replayRequest in this.MessagingDatabase.GetAllReplayRequests())
				{
					ResubmitRequestFacade resubmitRequestFacade;
					if (!dictionary.TryGetValue(replayRequest.PrimaryRequestId, out resubmitRequestFacade))
					{
						resubmitRequestFacade = new ResubmitRequestFacade(this.MessagingDatabase, replayRequest.PrimaryRequestId, replayRequest.CorrelationId, replayRequest.IsTestRequest);
						this.resubmitRequests.Add(resubmitRequestFacade);
						dictionary.Add(replayRequest.PrimaryRequestId, resubmitRequestFacade);
					}
					resubmitRequestFacade.Add(replayRequest);
					MessageResubmissionPerfCounters.Instance.UpdateResubmitRequestCount(replayRequest.State, 1);
				}
			}
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug<int>((long)this.GetHashCode(), "Loaded {0} Resubmit Requests from the DB", this.resubmitRequests.Count);
		}

		private ResubmitRequestFacade GetResubmitRequest(ResubmitRequestId resubmitRequestId, out byte[] response)
		{
			response = null;
			ResubmitRequestFacade resubmitRequestFacade2;
			using (this.syncLock.AcquireReadLock())
			{
				resubmitRequestFacade2 = this.resubmitRequests.FirstOrDefault((ResubmitRequestFacade resubmitRequestFacade) => resubmitRequestFacade.RequestId == resubmitRequestId.ResubmitRequestRowId);
			}
			if (resubmitRequestFacade2 == null)
			{
				response = Serialization.ObjectToBytes(new ResubmitRequestResponse(ResubmitRequestResponseCode.ResubmitRequestDoesNotExist, Strings.ResubmitRequestDoesNotExist(resubmitRequestId.ResubmitRequestRowId).ToString()));
			}
			return resubmitRequestFacade2;
		}

		private void ExpireCompletedResubmitRequests()
		{
			if (this.paused)
			{
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug((long)this.GetHashCode(), "ExpireCompletedResubmitRequests: Skipping processing because message resubmission is paused");
				return;
			}
			IEnumerable<ResubmitRequestFacade> enumerable;
			using (this.syncLock.AcquireReadLock())
			{
				enumerable = (from request in this.resubmitRequests
				where request.IsCompleted && DateTime.UtcNow - request.CreationTime >= (request.IsTestRequest ? this.Config.TestResubmitRequestExpiryPeriod : this.Config.ResubmitRequestExpiryPeriod)
				select request).ToArray<ResubmitRequestFacade>();
			}
			foreach (ResubmitRequestFacade resubmitRequestFacade in enumerable)
			{
				byte[] array;
				if (resubmitRequestFacade.Remove(out array))
				{
					using (this.syncLock.AcquireWriteLock())
					{
						if (this.resubmitRequests.Contains(resubmitRequestFacade))
						{
							this.resubmitRequests.Remove(resubmitRequestFacade);
						}
					}
					if (!resubmitRequestFacade.IsTestRequest)
					{
						MessageResubmissionComponent.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitRequestExpired, null, new object[]
						{
							resubmitRequestFacade.GetDiagnosticInformationString()
						});
					}
				}
				else
				{
					ExTraceGlobals.MessageResubmissionTracer.TraceDebug<long>((long)this.GetHashCode(), "Resubmit request facade remove failed for request id = {0}", resubmitRequestFacade.RequestId);
				}
			}
		}

		private void ResubmitMessageTimerCallback(object obj)
		{
			if (!this.MessageResubmissionEnabled)
			{
				return;
			}
			ExTraceGlobals.MessageResubmissionTracer.TraceDebug<bool>((long)this.GetHashCode(), "ResubmitMessageTimerCallback: Processing any pending resubmit requests. Paused = {0}", this.paused);
			if (this.paused || this.resubmitRequests.Count == 0)
			{
				return;
			}
			if (this.Config.MaxOutstandingResubmissionMessages <= 0)
			{
				throw new ArgumentException("MaxOutstandingResubmittedMessages cannot be less than or equal to 0");
			}
			if (this.Config.ResubmissionPageSize <= 0)
			{
				throw new ArgumentException("ResubmitRequestPageSize cannot be less than or equal to 0");
			}
			IEnumerable<ResubmitRequestFacade> enumerable;
			using (this.syncLock.AcquireReadLock())
			{
				enumerable = this.resubmitRequests.ToArray();
			}
			int num = this.Config.MaxOutstandingResubmissionMessages - enumerable.Sum((ResubmitRequestFacade r) => r.OutstaindingMailItemCount);
			foreach (ResubmitRequestFacade resubmitRequestFacade in enumerable)
			{
				if (num < this.Config.ResubmissionPageSize)
				{
					break;
				}
				num -= resubmitRequestFacade.Resubmit(num);
			}
		}

		private XElement GetComponentStateInfo()
		{
			XElement xelement = new XElement("State");
			using (this.syncLock.AcquireReadLock())
			{
				xelement.Add(from resubmitRequest in this.resubmitRequests
				select resubmitRequest.GetDiagnosticInformation());
			}
			return xelement;
		}

		private XElement GetConfigurationInfo()
		{
			if (this.config == null)
			{
				return new XElement("Configuration", new XElement("MessageRepositoryEnabled", "False"));
			}
			return new XElement("Configuration", new object[]
			{
				new XElement("MaxOutstandingResubmittedMessages", this.config.MaxOutstandingResubmissionMessages),
				new XElement("MessageRepositoryEnabled", this.config.MessageResubmissionEnabled),
				new XElement("ResubmitInterval", this.config.ResubmissionInterval),
				new XElement("ResubmissionInitialDelay", this.config.ResubmissionInitialDelay),
				new XElement("ResubmitRequestPageSize", this.config.ResubmissionPageSize),
				new XElement("ResubmitRequestExpiryPeriod", this.config.ResubmitRequestExpiryPeriod),
				new XElement("TestResubmitRequestExpiryPeriod", this.config.TestResubmitRequestExpiryPeriod)
			});
		}

		private bool ShouldExecute()
		{
			return this.targetRunningState == ServiceState.Active;
		}

		private IEnumerable<PrimaryServerInfo> SelectPrimaryServersForShadowResubmission(string[] unresponsivePrimaryServers, DateTime startTime, DateTime endTime)
		{
			IEnumerable<PrimaryServerInfo> source = this.PrimaryServerInfoMap.GetAll();
			bool resubmitFromShadow = false;
			if (unresponsivePrimaryServers != null && unresponsivePrimaryServers.Length > 0)
			{
				resubmitFromShadow = true;
				ExTraceGlobals.MessageResubmissionTracer.TraceDebug<string>((long)this.GetHashCode(), "Unresponsive Servers : {0}", string.Join(", ", unresponsivePrimaryServers));
				HashSet<string> unresponsiveServersTable = new HashSet<string>(unresponsivePrimaryServers, StringComparer.InvariantCultureIgnoreCase);
				source = from primaryServer in source
				where unresponsiveServersTable.Contains(primaryServer.ServerFqdn)
				select primaryServer;
			}
			return from primaryServer in source
			where startTime <= primaryServer.EndTime && endTime >= primaryServer.StartTime && (resubmitFromShadow || !primaryServer.IsActive)
			select primaryServer;
		}

		private TimeSpan GetResubmitRequestExpiryTimerPeriod()
		{
			TimeSpan timeSpan = MessageResubmissionComponent.maxResubmitRequestCleanupTimeSpan;
			if (this.Config.ResubmitRequestExpiryPeriod < timeSpan)
			{
				timeSpan = this.Config.ResubmitRequestExpiryPeriod;
			}
			if (this.Config.TestResubmitRequestExpiryPeriod < timeSpan)
			{
				timeSpan = this.Config.TestResubmitRequestExpiryPeriod;
			}
			return timeSpan;
		}

		private const string ComponentName = "SafetyNet";

		public const uint ReservedBytesTestModeKey = 65547U;

		private static readonly TimeSpan maxResubmitRequestCleanupTimeSpan = TimeSpan.FromHours(1.0);

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.MessageResubmissionTracer.Category, TransportEventLog.GetEventSource());

		private readonly List<ResubmitRequestFacade> resubmitRequests = new List<ResubmitRequestFacade>();

		private readonly ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

		private TransportAppConfig.IMessageResubmissionConfig config;

		private GuardedTimer resubmitMessageTimer;

		private GuardedTimer resubmitRequestCleanupTimer;

		private bool paused;

		private IMessagingDatabase messagingDatabase;

		private IPrimaryServerInfoMap primaryServerInfoMap;

		private ServiceState targetRunningState;
	}
}
