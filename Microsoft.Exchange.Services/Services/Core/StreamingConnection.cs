using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class StreamingConnection : IDisposeTrackable, IDisposable, ISubscriptionEventHandler
	{
		public static int PeriodicConnectionCheckInterval
		{
			get
			{
				return StreamingConnection.periodicConnectionCheckInterval;
			}
			set
			{
				StreamingConnection.periodicConnectionCheckInterval = value;
			}
		}

		internal static void CreateConnection(CallContext callContext, string[] subscriptionIds, TimeSpan connectionLifetime, CompleteRequestAsyncCallback endRequestCallback)
		{
			StreamingConnection streamingConnection = new StreamingConnection(callContext, endRequestCallback);
			if (subscriptionIds != null)
			{
				streamingConnection.LogSubscriptionInfo("CrteConn", subscriptionIds, new string[]
				{
					RequestDetailsLogger.FormatSubscriptionLogDetails("cnt", subscriptionIds.Count<string>()),
					RequestDetailsLogger.FormatSubscriptionLogDetails("LifeTime", connectionLifetime.TotalSeconds)
				});
			}
			lock (StreamingConnection.openConnections)
			{
				StreamingConnection.openConnections.Add(streamingConnection);
			}
			PerformanceMonitor.UpdateActiveStreamingConnectionsCounter((long)StreamingConnection.openConnections.Count);
			lock (streamingConnection.lockObject)
			{
				streamingConnection.Initialize(subscriptionIds, connectionLifetime);
			}
		}

		internal static List<StreamingConnection> OpenConnections
		{
			get
			{
				List<StreamingConnection> result;
				lock (StreamingConnection.openConnections)
				{
					result = new List<StreamingConnection>(StreamingConnection.openConnections);
				}
				return result;
			}
		}

		internal List<StreamingSubscription> Subscriptions
		{
			get
			{
				List<StreamingSubscription> result;
				lock (this.lockObject)
				{
					result = ((this.subscriptions == null) ? null : new List<StreamingSubscription>(this.subscriptions));
				}
				return result;
			}
		}

		internal string CreatorSmtpAddress
		{
			get
			{
				string result;
				lock (this.lockObject)
				{
					if (this.isDisposed)
					{
						result = null;
					}
					else
					{
						result = ((this.callContext.AccessingPrincipal == null) ? string.Empty : this.callContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
					}
				}
				return result;
			}
		}

		private StreamingConnection(CallContext context, CompleteRequestAsyncCallback endRequestCallback)
		{
			this.callContext = context;
			this.subscriptions = new List<StreamingSubscription>();
			this.subscriptionsToNotify = new Queue<StreamingSubscription>();
			this.subscriptionsInError = new Dictionary<ServiceError, List<string>>();
			this.endRequestCallback = endRequestCallback;
			this.responseWriter = EwsResponseWireWriter.Create(this.callContext);
			this.disposeTracker = this.GetDisposeTracker();
			if (this.callContext != null && this.callContext.AccessingADUser != null && this.callContext.AccessingADUser.OrganizationId != null && this.callContext.AccessingADUser.OrganizationId.OrganizationalUnit != null)
			{
				this.organizationId = this.callContext.AccessingADUser.OrganizationId.OrganizationalUnit.Name;
			}
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				this.correlationGuid = currentActivityScope.GetProperty(EwsMetadata.CorrelationGuid);
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamingConnection>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void Initialize(string[] subscriptionIds, TimeSpan connectionLifetime)
		{
			int i = 0;
			while (i < subscriptionIds.Length)
			{
				string text = subscriptionIds[i];
				StreamingSubscription streamingSubscription;
				try
				{
					SubscriptionId subscriptionId = SubscriptionId.Parse(text);
					if (!subscriptionId.ServerFQDN.Equals(LocalServer.GetServer().Fqdn, StringComparison.OrdinalIgnoreCase) && subscriptionIds.Length > 1)
					{
						ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "StreamingConnection.Initialize. Subscription [{0}] is from another CAS: {1}", text, subscriptionId.ServerFQDN);
						this.QueueError(text, ServiceErrors.GetServiceError(new ProxyRequestNotAllowedException()));
						goto IL_14F;
					}
					streamingSubscription = (Microsoft.Exchange.Services.Core.Subscriptions.Singleton.Get(subscriptionId.ToString()) as StreamingSubscription);
				}
				catch (InvalidSubscriptionException ex)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, InvalidSubscriptionException>((long)this.GetHashCode(), "StreamingConnection::Initialize. Invalid subscription id [{0}] : {1}", text, ex);
					this.QueueError(text, ServiceErrors.GetServiceError(ex));
					goto IL_14F;
				}
				catch (SubscriptionNotFoundException ex2)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, SubscriptionNotFoundException>((long)this.GetHashCode(), "StreamingConnection::Initialize. Subscription [{0}] not found: {1}", text, ex2);
					this.QueueError(text, ServiceErrors.GetServiceError(ex2));
					goto IL_14F;
				}
				goto IL_D7;
				IL_14F:
				i++;
				continue;
				IL_D7:
				if (streamingSubscription == null)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "StreamingConnection::Initialize. Subscription [{0}] is not a StreamingSubscription.", text);
					this.QueueError(text, ServiceErrors.GetServiceError(new InvalidSubscriptionException()));
					goto IL_14F;
				}
				if (!streamingSubscription.CheckCallerHasRights(this.callContext))
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "StreamingConnection::Initialize. Caller does not have rights on subscription [{0}].", text);
					this.QueueError(text, ServiceErrors.GetServiceError(new SubscriptionAccessDeniedException()));
					goto IL_14F;
				}
				this.subscriptions.Add(streamingSubscription);
				streamingSubscription.RegisterConnection(this);
				goto IL_14F;
			}
			if (!this.CheckSubscriptionsLeftToService())
			{
				this.TryEndConnection(true);
				return;
			}
			List<string> list = new List<string>(this.subscriptions.Count);
			foreach (StreamingSubscription streamingSubscription2 in this.subscriptions)
			{
				if (streamingSubscription2.MailboxId != null)
				{
					list.Add(streamingSubscription2.MailboxId.SmtpAddress);
				}
				else
				{
					list.Add("null");
				}
			}
			this.callContext.ProtocolLog.AppendGenericInfo("SubscribedMailboxes", string.Join("/", list.ToArray()));
			this.connectionExpires = ExDateTime.UtcNow.Add(connectionLifetime);
			this.heartbeatTimer = new Timer(delegate(object a)
			{
				this.EnqueueTask(this.CreateConnectionStatusTask());
			}, this, 0, StreamingConnection.PeriodicConnectionCheckInterval);
			this.connectionTimer = new Timer(delegate(object a)
			{
				this.TryEndConnection(true);
			}, this, (int)connectionLifetime.TotalMilliseconds, -1);
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "StreamingConnection::Initialize. Connection established successfully.");
		}

		private bool CheckSubscriptionsLeftToService()
		{
			bool flag = true;
			lock (this.lockObject)
			{
				flag = (this.subscriptions != null && this.subscriptions.Count > 0);
			}
			if (!flag)
			{
				this.LogSubscriptionInfo("NoSubLeft", null, null);
			}
			return flag;
		}

		private void LogSubscriptionInfo(string action, IEnumerable<string> subscriptionIds, params string[] details)
		{
			RequestDetailsLogger.LogSubscriptionInfo(this.correlationGuid, this.organizationId, action, subscriptionIds, details);
		}

		private void LogSubscriptionInfo(string action, string subscriptionIds, params string[] details)
		{
			RequestDetailsLogger.LogSubscriptionInfo(this.correlationGuid, this.organizationId, action, subscriptionIds, details);
		}

		public void EventsAvailable(StreamingSubscription subscription)
		{
			lock (this.lockObject)
			{
				if (!this.isDisposed)
				{
					try
					{
						CallContext.SetCurrent(this.callContext);
						if (subscription.IsDisposed)
						{
							this.LogSubscriptionInfo("EventsAvailableSubDisposed", subscription.SubscriptionId, new string[0]);
						}
						else
						{
							if (ExTraceGlobals.SubscriptionsTracer.IsTraceEnabled(TraceType.DebugTrace) && !this.subscriptions.Contains(subscription))
							{
								ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "StreamingConnection.EventsAvailable. Subscription points to this connection, but this connection does not point back.");
							}
							int count = this.subscriptionsToNotify.Count;
							bool flag2 = count == 0;
							this.subscriptionsToNotify.Enqueue(subscription);
							this.LogSubscriptionInfo("EventsAvailable", subscription.SubscriptionId, new string[]
							{
								RequestDetailsLogger.FormatSubscriptionLogDetails("ntfyCnt", count)
							});
							if (flag2)
							{
								this.EnqueueTask(this.CreateNotificationTask());
							}
						}
					}
					finally
					{
						CallContext.SetCurrent(null);
					}
				}
			}
		}

		public void DisconnectSubscription(StreamingSubscription sub, LocalizedException exception)
		{
			lock (this.lockObject)
			{
				if (this.isDisposed)
				{
					return;
				}
				try
				{
					CallContext.SetCurrent(this.callContext);
					if (sub != null)
					{
						bool flag2 = this.subscriptions.Remove(sub);
						if (flag2)
						{
							this.QueueError(sub.SubscriptionId, ServiceErrors.GetServiceError(exception));
						}
						if (!this.CheckSubscriptionsLeftToService())
						{
							this.TryEndConnection(true);
						}
					}
				}
				finally
				{
					CallContext.SetCurrent(null);
				}
			}
			if (sub != null)
			{
				this.LogSubscriptionInfo("DisCntSub", sub.SubscriptionId, new string[]
				{
					(exception != null) ? exception.Message : string.Empty
				});
			}
		}

		private TaskExecuteResult ExecuteNotificationTask()
		{
			this.processingNotifications = true;
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "StreamingConnection.ExecuteNotificationTask. Executing notification task.");
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
			try
			{
				int i = 0;
				if (this.connectionState != StreamingConnection.ConnectionState.Active)
				{
					foreach (KeyValuePair<string, int> keyValuePair in list)
					{
						this.LogSubscriptionInfo("NtfCnnCls", keyValuePair.Key, new string[]
						{
							string.Format("evtCnt:{0}", keyValuePair.Value)
						});
					}
					return TaskExecuteResult.ProcessingComplete;
				}
				List<EwsNotificationType> list2 = new List<EwsNotificationType>();
				while (i < 100)
				{
					lock (this.lockObject)
					{
						if (this.connectionState != StreamingConnection.ConnectionState.Active)
						{
							return TaskExecuteResult.ProcessingComplete;
						}
						if (this.subscriptionsToNotify.Count == 0)
						{
							break;
						}
						StreamingSubscription streamingSubscription = this.subscriptionsToNotify.Peek();
						try
						{
							int num;
							EwsNotificationType events = streamingSubscription.GetEvents(100 - i, out num);
							if (num != 0)
							{
								list2.Add(events);
								i += num;
							}
							list.Add(new KeyValuePair<string, int>(streamingSubscription.SubscriptionId, num));
							if (!streamingSubscription.CheckForEventsLater())
							{
								this.subscriptionsToNotify.Dequeue();
							}
						}
						catch (LocalizedException ex)
						{
							if (!ex.Data.Contains(StreamingConnection.IsNonFatalSubscriptionExceptionKey) || (string)ex.Data[StreamingConnection.IsNonFatalSubscriptionExceptionKey] != bool.TrueString)
							{
								this.subscriptions.Remove(streamingSubscription);
							}
							if (object.Equals(streamingSubscription, this.subscriptionsToNotify.Peek()))
							{
								this.subscriptionsToNotify.Dequeue();
							}
							this.QueueError(streamingSubscription.SubscriptionId, ServiceErrors.GetServiceError(ex));
							if (!this.CheckSubscriptionsLeftToService())
							{
								this.TryEndConnection(true);
							}
						}
					}
				}
				foreach (KeyValuePair<string, int> keyValuePair2 in list)
				{
					this.LogSubscriptionInfo("SndNtf", keyValuePair2.Key, new string[]
					{
						string.Format("evtCnt:{0}", keyValuePair2.Value)
					});
				}
				if (list2.Count > 0)
				{
					this.SendNotifications(list2);
					PerformanceMonitor.UpdateStreamedEventsCounter((long)i);
				}
			}
			finally
			{
				this.processingNotifications = false;
				if (this.connectionState == StreamingConnection.ConnectionState.Closing)
				{
					this.TryEndConnection(true);
				}
			}
			TaskExecuteResult result;
			lock (this.lockObject)
			{
				if (this.subscriptionsToNotify.Count != 0 && this.connectionState == StreamingConnection.ConnectionState.Active)
				{
					result = TaskExecuteResult.StepComplete;
				}
				else
				{
					result = TaskExecuteResult.ProcessingComplete;
				}
			}
			return result;
		}

		private TaskExecuteResult ExecuteErrorTask()
		{
			this.processingErrors = true;
			bool flag = false;
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "StreamingConnection.ExecuteErrorTask. Executing error task.");
			try
			{
				if (this.connectionState == StreamingConnection.ConnectionState.Closed)
				{
					this.LogSubscriptionInfo("ErrCnnCls", string.Empty, new string[]
					{
						string.Format("ErrSubCnt={0}", this.subscriptionsInError.Count)
					});
					return TaskExecuteResult.ProcessingComplete;
				}
				KeyValuePair<ServiceError, List<string>> keyValuePair;
				lock (this.lockObject)
				{
					if (this.subscriptionsInError.Count == 0)
					{
						return TaskExecuteResult.ProcessingComplete;
					}
					keyValuePair = this.subscriptionsInError.ElementAt(0);
					this.subscriptionsInError.Remove(keyValuePair.Key);
				}
				GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
				getStreamingEventsSoapResponse.Body = StreamingConnection.CreateErrorResponse(keyValuePair.Key, keyValuePair.Value);
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, int>((long)this.GetHashCode(), "StreamingConnection.ExecuteErrorTask. Sending error '{0}' for {1} subscriptions.", keyValuePair.Key.MessageText, keyValuePair.Value.Count);
				this.LogSubscriptionInfo("ErrNtf", keyValuePair.Value, new string[]
				{
					keyValuePair.Key.MessageText
				});
				this.WriteResponseToWire(getStreamingEventsSoapResponse, true);
			}
			finally
			{
				this.processingErrors = false;
				lock (this.lockObject)
				{
					flag = (this.subscriptionsInError.Count > 0);
				}
				if (!flag && this.connectionState == StreamingConnection.ConnectionState.Closing)
				{
					this.TryEndConnection(true);
				}
			}
			if (flag)
			{
				return TaskExecuteResult.StepComplete;
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		private void WriteResponseToWire(GetStreamingEventsSoapResponse soapResponse, bool tryEndConnectionOnFailure)
		{
			Exception ex = null;
			if (this.responseWriter != null)
			{
				try
				{
					this.responseWriter.WriteResponseToWire(soapResponse, !tryEndConnectionOnFailure);
				}
				catch (HttpException ex2)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<HttpException>((long)this.GetHashCode(), "StreamingConnection.WriteResponseToWire. Encountered exception: {0}", ex2);
					if (tryEndConnectionOnFailure)
					{
						this.TryEndConnection(false);
					}
					ex = ex2;
				}
				catch (InvalidOperationException ex3)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<InvalidOperationException>((long)this.GetHashCode(), "StreamingConnection.WriteResponseToWire. Encountered exception: {0}", ex3);
					if (tryEndConnectionOnFailure)
					{
						this.TryEndConnection(false);
					}
					ex = ex3;
				}
				if (ex != null)
				{
					this.LogSubscriptionInfo("WrtRspFailed", null, new string[]
					{
						RequestDetailsLogger.FormatSubscriptionLogDetails("ex", ex),
						RequestDetailsLogger.FormatSubscriptionLogDetails("tryEndCnn", tryEndConnectionOnFailure)
					});
				}
			}
		}

		private TaskExecuteResult ExecuteConnectionStatusTask()
		{
			if (this.connectionState == StreamingConnection.ConnectionState.Active && ExDateTime.Now.AddSeconds(1.0).CompareTo(this.connectionExpires) < 0)
			{
				GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
				getStreamingEventsSoapResponse.Body = StreamingConnection.CreateConnectionResponse(ConnectionStatus.OK);
				this.LogSubscriptionInfo("ConnStatus", null, null);
				this.WriteResponseToWire(getStreamingEventsSoapResponse, true);
			}
			else if (this.connectionState == StreamingConnection.ConnectionState.Closing)
			{
				this.LogSubscriptionInfo("ConnStatusCls", null, null);
				this.TryEndConnection(true);
			}
			else
			{
				string action = "ConnStatusOther";
				string subscriptionIds = null;
				string[] array = new string[1];
				string[] array2 = array;
				int num = 0;
				int num2 = (int)this.connectionState;
				array2[num] = num2.ToString();
				this.LogSubscriptionInfo(action, subscriptionIds, array);
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		private void EnqueueTask(ITask task)
		{
			lock (this.lockObject)
			{
				if (task != null && !this.isDisposed && !UserWorkloadManager.Singleton.TrySubmitNewTask(task))
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), ".StreamingConnection.EnqueueTask. UserWorkloadManager rejected a request to enqueue a task. Shutting down the connection.");
					this.TryEndConnection(false);
				}
			}
		}

		private void QueueError(string currentSubscriptionId, ServiceError serviceError)
		{
			bool flag2;
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<ServiceError, string>((long)this.GetHashCode(), "StreamingConnection.QueueError. Queuing error {0} for subscription {1}.", serviceError, currentSubscriptionId);
				flag2 = (this.subscriptionsInError.Count == 0);
				List<string> list;
				if (!this.subscriptionsInError.TryGetValue(serviceError, out list))
				{
					list = new List<string>();
					this.subscriptionsInError.Add(serviceError, list);
				}
				if (!string.IsNullOrEmpty(currentSubscriptionId))
				{
					list.Add(currentSubscriptionId);
				}
			}
			this.LogSubscriptionInfo("QErr", currentSubscriptionId, new string[]
			{
				serviceError.MessageText
			});
			if (flag2)
			{
				this.EnqueueTask(this.CreateErrorTask());
			}
		}

		private ITask CreateNotificationTask()
		{
			return this.CreateTask(new StreamingConnectionTask.StreamingConnectionExecuteDelegate(this.ExecuteNotificationTask), "Notification");
		}

		private ITask CreateErrorTask()
		{
			return this.CreateTask(new StreamingConnectionTask.StreamingConnectionExecuteDelegate(this.ExecuteErrorTask), "Error");
		}

		private ITask CreateConnectionStatusTask()
		{
			return this.CreateTask(new StreamingConnectionTask.StreamingConnectionExecuteDelegate(this.ExecuteConnectionStatusTask), "ConnectionStatus");
		}

		private ITask CreateTask(StreamingConnectionTask.StreamingConnectionExecuteDelegate executeCallback, string taskType)
		{
			ITask result;
			lock (this.lockObject)
			{
				if (!this.isDisposed && this.callContext.Budget != null)
				{
					try
					{
						return new StreamingConnectionTask(this, this.callContext, executeCallback, taskType)
						{
							State = null
						};
					}
					catch (OverBudgetException ex)
					{
						ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[StreamingConnection::CreateTask] Failed to create task due to OverBudgetException. (Message: {0}) (Snapshot: {1})", ex.Message, ex.Snapshot);
						return null;
					}
				}
				result = null;
			}
			return result;
		}

		private void SendNotifications(List<EwsNotificationType> notifications)
		{
			this.WriteResponseToWire(new GetStreamingEventsSoapResponse
			{
				Body = this.CreateNotificationResponse(notifications)
			}, true);
		}

		private GetStreamingEventsResponse CreateNotificationResponse(List<EwsNotificationType> notifications)
		{
			GetStreamingEventsResponse getStreamingEventsResponse = new GetStreamingEventsResponse();
			GetStreamingEventsResponseMessage getStreamingEventsResponseMessage = new GetStreamingEventsResponseMessage(ServiceResultCode.Success, null);
			getStreamingEventsResponseMessage.AddNotifications(notifications);
			getStreamingEventsResponse.AddResponse(getStreamingEventsResponseMessage);
			return getStreamingEventsResponse;
		}

		internal static GetStreamingEventsResponse CreateConnectionResponse(ConnectionStatus status)
		{
			GetStreamingEventsResponse getStreamingEventsResponse = new GetStreamingEventsResponse();
			GetStreamingEventsResponseMessage getStreamingEventsResponseMessage = new GetStreamingEventsResponseMessage(ServiceResultCode.Success, null);
			getStreamingEventsResponseMessage.SetConnectionStatus(status);
			getStreamingEventsResponse.AddResponse(getStreamingEventsResponseMessage);
			return getStreamingEventsResponse;
		}

		internal static GetStreamingEventsResponse CreateErrorResponse(ServiceError error, IEnumerable<string> idsInError)
		{
			GetStreamingEventsResponse getStreamingEventsResponse = new GetStreamingEventsResponse();
			GetStreamingEventsResponseMessage getStreamingEventsResponseMessage = new GetStreamingEventsResponseMessage(ServiceResultCode.Error, error);
			getStreamingEventsResponseMessage.AddErrorSubscriptionIds(idsInError);
			getStreamingEventsResponse.AddResponse(getStreamingEventsResponseMessage);
			return getStreamingEventsResponse;
		}

		internal void TryEndConnection(LocalizedException ex)
		{
			this.QueueError(null, ServiceErrors.GetServiceError(ex, ExchangeVersion.Current));
			this.TryEndConnection(true);
		}

		internal void TryEndConnection(bool waitForErrors)
		{
			bool flag = false;
			HttpException ex = null;
			lock (this.lockObject)
			{
				if (!this.isDisposed)
				{
					if (this.connectionState == StreamingConnection.ConnectionState.Active)
					{
						this.connectionState = StreamingConnection.ConnectionState.Closing;
					}
					if (this.processingNotifications || this.processingErrors)
					{
						return;
					}
					if (waitForErrors && this.callContext.HttpContext.Response.IsClientConnected && this.subscriptionsInError.Count > 0)
					{
						return;
					}
					if (this.connectionState != StreamingConnection.ConnectionState.Closed)
					{
						GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
						getStreamingEventsSoapResponse.Body = StreamingConnection.CreateConnectionResponse(ConnectionStatus.Closed);
						try
						{
							ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "StreamingConnection.TryEndConnection. Writing disconnect response.");
							this.WriteResponseToWire(getStreamingEventsSoapResponse, false);
							this.responseWriter.FinishWritesAndCompleteResponse(this.endRequestCallback);
							flag = true;
						}
						catch (HttpException ex2)
						{
							ExTraceGlobals.SubscriptionsTracer.TraceDebug<HttpException>((long)this.GetHashCode(), "StreamingConnection.TryEndConnection. Exception occurred while closing connection: {0}", ex2);
							ex = ex2;
							this.endRequestCallback(ex2);
						}
						this.Dispose();
					}
				}
			}
			if (flag)
			{
				this.LogSubscriptionInfo("EndConnSuccess", null, null);
				return;
			}
			if (ex != null)
			{
				this.LogSubscriptionInfo("EndConnFailed", null, new string[]
				{
					RequestDetailsLogger.FormatSubscriptionLogDetails("ex", ex)
				});
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
		}

		private void Dispose(bool suppressFinalize)
		{
			if (suppressFinalize)
			{
				GC.SuppressFinalize(this);
			}
			lock (this.lockObject)
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					lock (StreamingConnection.openConnections)
					{
						StreamingConnection.openConnections.Remove(this);
					}
					PerformanceMonitor.UpdateActiveStreamingConnectionsCounter((long)StreamingConnection.openConnections.Count);
					this.connectionState = StreamingConnection.ConnectionState.Closed;
					if (this.heartbeatTimer != null)
					{
						this.heartbeatTimer.Dispose();
						this.heartbeatTimer = null;
					}
					if (this.connectionTimer != null)
					{
						this.connectionTimer.Dispose();
						this.connectionTimer = null;
					}
					if (this.subscriptions != null)
					{
						foreach (StreamingSubscription streamingSubscription in this.subscriptions)
						{
							streamingSubscription.UnregisterConnection(this);
						}
						this.subscriptions = null;
					}
					if (this.responseWriter != null)
					{
						this.responseWriter.Dispose();
						this.responseWriter = null;
					}
				}
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		private const int MaxEventsPerTask = 100;

		internal const int DefaultPeriodicConnectionCheckInterval = 45000;

		private readonly DisposeTracker disposeTracker;

		internal static string IsNonFatalSubscriptionExceptionKey = "IsNonFatalSubscriptionException";

		private static int periodicConnectionCheckInterval = 45000;

		private static List<StreamingConnection> openConnections = new List<StreamingConnection>();

		private List<StreamingSubscription> subscriptions;

		private CallContext callContext;

		private Queue<StreamingSubscription> subscriptionsToNotify;

		private Dictionary<ServiceError, List<string>> subscriptionsInError;

		private ExDateTime connectionExpires;

		private Timer heartbeatTimer;

		private Timer connectionTimer;

		private EwsResponseWireWriter responseWriter;

		private CompleteRequestAsyncCallback endRequestCallback;

		private StreamingConnection.ConnectionState connectionState;

		private bool processingNotifications;

		private bool processingErrors;

		private object lockObject = new object();

		private string organizationId;

		private string correlationGuid;

		private bool isDisposed;

		private enum ConnectionState
		{
			Active,
			Closing,
			Closed
		}
	}
}
