using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class DeliveryAgentConnection
	{
		public DeliveryAgentConnection(NextHopConnection nextHopConnection, DeliveryAgentMExEvents mexEvents) : this(nextHopConnection, mexEvents, null, new DeliveryAgentConnection.Stats())
		{
		}

		public DeliveryAgentConnection(NextHopConnection nextHopConnection, DeliveryAgentMExEvents mexEvents, DeliveryAgentConnector connector, DeliveryAgentConnection.Stats stats)
		{
			if (nextHopConnection == null)
			{
				throw new ArgumentNullException("nextHopConnection");
			}
			if (mexEvents == null)
			{
				throw new ArgumentNullException("mexEvents");
			}
			this.nextHopConnection = nextHopConnection;
			this.mexEvents = mexEvents;
			this.connector = connector;
			this.stats = stats;
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DeliveryAgentsTracer;
			}
		}

		public IAsyncResult BeginConnection(object asyncState, AsyncCallback asyncCallback)
		{
			this.connectionAsyncResult = new DeliveryAgentConnection.AsyncResult(asyncState, asyncCallback);
			SmtpResponse smtpResponse;
			if (this.connector != null || this.TryGetDeliveryAgentConnector(out this.connector, out smtpResponse))
			{
				DeliveryAgentConnection.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Created a delivery agent connection for domain {0} using protocol {1}.", this.nextHopConnection.Key.NextHopDomain, this.connector.DeliveryProtocol);
				this.stats.Initialize(this.connector.DeliveryProtocol);
				this.ExecuteNextState(DeliveryAgentConnection.State.OnOpenConnection);
			}
			else
			{
				DeliveryAgentConnection.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "Could not get the delivery agent connector for connector {0}.", this.nextHopConnection.Key.NextHopConnector);
				this.AckConnection(AckStatus.Resubmit, smtpResponse);
				this.CompleteAsyncOperation();
			}
			return this.connectionAsyncResult;
		}

		public void EndConnection(IAsyncResult ar)
		{
		}

		public void Retire()
		{
			this.retire = true;
		}

		private bool TryGetDeliveryAgentConnector(out DeliveryAgentConnector connector, out SmtpResponse failureResponse)
		{
			if (!Components.RoutingComponent.MailRouter.TryGetLocalSendConnector<DeliveryAgentConnector>(this.nextHopConnection.Key.NextHopConnector, out connector))
			{
				DeliveryAgentConnection.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "Connector {0} could not be found", this.nextHopConnection.Key.NextHopConnector);
				failureResponse = AckReason.DeliveryAgentConnectorDeleted;
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private void ExecuteNextState(DeliveryAgentConnection.State nextState)
		{
			while (nextState != DeliveryAgentConnection.State.Done)
			{
				switch (nextState)
				{
				case DeliveryAgentConnection.State.OnOpenConnection:
					nextState = this.RaiseOnOpenConnectionEvent();
					break;
				case DeliveryAgentConnection.State.OnDeliverMailItem:
					nextState = this.RaiseOnDeliverMailItemEvent();
					break;
				case DeliveryAgentConnection.State.OnCloseConnection:
					nextState = this.RaiseOnCloseConnectionEvent();
					break;
				default:
					throw new InvalidOperationException("Invalid next state: " + nextState);
				}
			}
		}

		private DeliveryAgentConnection.State RaiseOnOpenConnectionEvent()
		{
			DeliveryAgentConnection.State result = DeliveryAgentConnection.State.Done;
			if (this.GetCurrentMailItem() == null)
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "No mail item available, returning without raising OnOpenConnection event.");
				this.CompleteAsyncOperation();
			}
			else
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "Retrieved first mail item for this session.  Raising OnOpenConnection event.");
				this.CreateMexSession(false);
				RoutedMailItemWrapper deliverableMailItem = this.CreateDeliverableMailItem();
				InternalOpenConnectionEventSource internalOpenConnectionEventSource = new InternalOpenConnectionEventSource(this.mexSession, deliverableMailItem, this.sessionId, this.nextHopConnection, this.stats);
				InternalOpenConnectionEventArgs internalOpenConnectionEventArgs = new InternalOpenConnectionEventArgs(deliverableMailItem, this.nextHopConnection.Key.NextHopDomain);
				this.TrackBeginAgentInvocation(this.currentMailItem, LatencyComponent.DeliveryAgentOnOpenConnection);
				IAsyncResult asyncResult = this.mexEvents.RaiseEvent(this.mexSession, "OnOpenConnection", new AsyncCallback(this.OnOpenConnectionEventCompleted), internalOpenConnectionEventSource, new object[]
				{
					internalOpenConnectionEventSource,
					internalOpenConnectionEventArgs
				});
				if (asyncResult.CompletedSynchronously)
				{
					result = this.HandleOpenConnectionEventCompleted(asyncResult);
				}
			}
			return result;
		}

		private void OnOpenConnectionEventCompleted(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				DeliveryAgentConnection.State nextState = this.HandleOpenConnectionEventCompleted(ar);
				this.ExecuteNextState(nextState);
			}
		}

		private DeliveryAgentConnection.State HandleOpenConnectionEventCompleted(IAsyncResult ar)
		{
			DeliveryAgentConnection.State result = DeliveryAgentConnection.State.Done;
			InternalDeliveryAgentEventSource internalEventSource = ((InternalOpenConnectionEventSource)ar.AsyncState).InternalEventSource;
			bool flag = this.EndEvent(ar, internalEventSource);
			if (flag && internalEventSource.ConnectionRegistered)
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "The agent registered a connection.  Moving on to OnDeliverMailItem event.");
				this.remoteHost = internalEventSource.RemoteHost;
				result = DeliveryAgentConnection.State.OnDeliverMailItem;
				this.nextHopConnection.ConnectionAttemptSucceeded();
			}
			else
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "No agent registered a connection.");
				if (!internalEventSource.MessageAcked)
				{
					internalEventSource.AckMailItemPending();
				}
				if (!internalEventSource.ConnectionUnregistered)
				{
					this.AckConnection(AckStatus.Retry, DeliveryAgentConnection.ConnectionNotHandledResponse);
				}
				this.CompleteAsyncOperation();
			}
			this.ReleaseMexSession();
			return result;
		}

		private DeliveryAgentConnection.State RaiseOnDeliverMailItemEvent()
		{
			DeliveryAgentConnection.State result = DeliveryAgentConnection.State.Done;
			if (this.stats.NumMessagesDelivered == this.connector.MaxMessagesPerConnection)
			{
				DeliveryAgentConnection.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Already delivered the max messages allowed per connection, {0}.  Moving to OnCloseConnection.", this.connector.MaxMessagesPerConnection);
				result = DeliveryAgentConnection.State.OnCloseConnection;
			}
			else if (this.GetCurrentMailItem() == null)
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "No more mail items available.  Moving to OnCloseConnection.");
				result = DeliveryAgentConnection.State.OnCloseConnection;
			}
			else
			{
				DeliveryAgentConnection.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Retrieved mail item with message id {0}.  Raising OnDeliverMailItem event.", this.currentMailItem.Message.MessageId);
				this.CreateMexSession(true);
				RoutedMailItemWrapper deliverableMailItem = this.CreateDeliverableMailItem();
				InternalDeliverMailItemEventSource internalDeliverMailItemEventSource = new InternalDeliverMailItemEventSource(this.mexSession, deliverableMailItem, this.sessionId, this.nextHopConnection, this.remoteHost, this.stats);
				InternalDeliverMailItemEventArgs internalDeliverMailItemEventArgs = new InternalDeliverMailItemEventArgs(deliverableMailItem);
				this.TrackBeginAgentInvocation(this.currentMailItem, LatencyComponent.DeliveryAgentOnDeliverMailItem);
				IAsyncResult asyncResult = this.mexEvents.RaiseEvent(this.mexSession, "OnDeliverMailItem", new AsyncCallback(this.OnDeliverMailItemEventCompleted), internalDeliverMailItemEventSource, new object[]
				{
					internalDeliverMailItemEventSource,
					internalDeliverMailItemEventArgs
				});
				if (asyncResult.CompletedSynchronously)
				{
					result = this.HandleDeliverMailItemEventCompleted(asyncResult);
				}
			}
			return result;
		}

		private void OnDeliverMailItemEventCompleted(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				DeliveryAgentConnection.State nextState = this.HandleDeliverMailItemEventCompleted(ar);
				this.ExecuteNextState(nextState);
			}
		}

		private DeliveryAgentConnection.State HandleDeliverMailItemEventCompleted(IAsyncResult ar)
		{
			DeliveryAgentConnection.State result = DeliveryAgentConnection.State.Done;
			InternalDeliveryAgentEventSource internalEventSource = ((InternalDeliverMailItemEventSource)ar.AsyncState).InternalEventSource;
			this.EndEvent(ar, internalEventSource);
			if (internalEventSource.ConnectionUnregistered)
			{
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "The agent unregistered its connection.");
				this.CompleteAsyncOperation();
			}
			else
			{
				if (internalEventSource.AnyRecipientsAcked)
				{
					internalEventSource.AckRemainingRecipientsAndFinalizeMailItem(AckStatus.Retry, DeliveryAgentConnection.RecipientNotHandledResponse);
				}
				if (!internalEventSource.MessageAcked)
				{
					internalEventSource.AckMailItemDefer(DeliveryAgentConnection.MessageNotHandledResponse);
				}
				result = DeliveryAgentConnection.State.OnDeliverMailItem;
			}
			this.ReleaseMexSession();
			this.ReleaseCurrentMailItem();
			return result;
		}

		private DeliveryAgentConnection.State RaiseOnCloseConnectionEvent()
		{
			DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "Raising OnCloseConnection event.");
			DeliveryAgentConnection.State result = DeliveryAgentConnection.State.Done;
			this.CreateMexSession(false);
			InternalCloseConnectionEventSource internalCloseConnectionEventSource = new InternalCloseConnectionEventSource(this.mexSession, this.sessionId, this.remoteHost, this.nextHopConnection, this.stats);
			InternalCloseConnectionEventArgs internalCloseConnectionEventArgs = new InternalCloseConnectionEventArgs();
			this.TrackBeginAgentInvocation(null, LatencyComponent.None);
			IAsyncResult asyncResult = this.mexEvents.RaiseEvent(this.mexSession, "OnCloseConnection", new AsyncCallback(this.OnCloseConnectionEventCompleted), internalCloseConnectionEventSource, new object[]
			{
				internalCloseConnectionEventSource,
				internalCloseConnectionEventArgs
			});
			if (asyncResult.CompletedSynchronously)
			{
				result = this.HandleCloseConnectionEventCompleted(asyncResult);
			}
			return result;
		}

		private void OnCloseConnectionEventCompleted(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				DeliveryAgentConnection.State nextState = this.HandleCloseConnectionEventCompleted(ar);
				this.ExecuteNextState(nextState);
			}
		}

		private DeliveryAgentConnection.State HandleCloseConnectionEventCompleted(IAsyncResult ar)
		{
			this.EndEvent(ar, null);
			InternalCloseConnectionEventSource internalCloseConnectionEventSource = (InternalCloseConnectionEventSource)ar.AsyncState;
			if (!internalCloseConnectionEventSource.ConnectionUnregistered)
			{
				internalCloseConnectionEventSource.UnregisterConnection(DeliveryAgentConnection.DefaultUnregisterConnectionResponse);
			}
			this.ReleaseMexSession();
			this.CompleteAsyncOperation();
			return DeliveryAgentConnection.State.Done;
		}

		private void CompleteAsyncOperation()
		{
			this.ReleaseMexSession();
			this.mexEvents = null;
			this.connectionAsyncResult.Completed();
			this.connectionAsyncResult = null;
			this.ReleaseCurrentMailItem();
			this.nextHopConnection = null;
			this.connector = null;
			this.stats = null;
		}

		private bool EndEvent(IAsyncResult ar, InternalDeliveryAgentEventSource eventSource)
		{
			DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "Event completed.");
			try
			{
				this.connectionAsyncResult.SetAsyncIfNecessary(ar);
				this.mexEvents.EndEvent(this.mexSession, ar);
			}
			catch (Exception ex)
			{
				DeliveryAgentConnection.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "The agent threw an exception: {0}", ex);
				bool flag = false;
				if (ExceptionHelper.HandleLeakedException)
				{
					if (eventSource == null || eventSource.MessageAcked)
					{
						flag = ExceptionHelper.IsHandleableException(ex);
					}
					else if (ExceptionHelper.IsHandleableTransientException(ex))
					{
						string text = string.Format(DeliveryAgentConnection.messageFailedWithTransientException, this.currentMailItem.Message.MessageId, ex.GetType().Name);
						SmtpResponse smtpResponse = new SmtpResponse("450", "4.5.1", new string[]
						{
							text
						});
						if (eventSource.AnyRecipientsAcked)
						{
							eventSource.AckRemainingRecipientsAndFinalizeMailItem(AckStatus.Retry, smtpResponse);
						}
						else
						{
							eventSource.AckMailItemDefer(smtpResponse);
						}
						flag = true;
					}
					else if (ExceptionHelper.IsHandleablePermanentException(ex))
					{
						string text2 = string.Format(DeliveryAgentConnection.messageFailedWithPermanentException, this.currentMailItem.Message.MessageId, ex.GetType().Name);
						SmtpResponse smtpResponse2 = new SmtpResponse("550", "5.6.0", new string[]
						{
							text2
						});
						if (eventSource.AnyRecipientsAcked)
						{
							eventSource.AckRemainingRecipientsAndFinalizeMailItem(AckStatus.Fail, smtpResponse2);
						}
						else
						{
							eventSource.AckMailItemFail(smtpResponse2);
						}
						flag = true;
					}
				}
				if (!flag)
				{
					throw;
				}
				DeliveryAgentConnection.Tracer.TraceDebug((long)this.GetHashCode(), "The exception was handled");
				return false;
			}
			finally
			{
				this.TrackEndAgentInvocation();
			}
			return true;
		}

		private RoutedMailItem GetCurrentMailItem()
		{
			if (!this.retire && this.currentMailItem == null)
			{
				this.GetNextMailItem();
			}
			return this.currentMailItem;
		}

		private void GetNextMailItem()
		{
			if (this.mexSession != null)
			{
				throw new InvalidOperationException("GetNextMailItem() is called with an existing mexSession");
			}
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			for (;;)
			{
				this.currentMailItem = this.nextHopConnection.GetNextRoutedMailItem();
				if (this.currentMailItem == null)
				{
					break;
				}
				if (Components.Configuration.TryGetAcceptedDomainTable(this.currentMailItem.OrganizationId, out perTenantAcceptedDomainTable))
				{
					goto IL_99;
				}
				DeliveryAgentConnection.Tracer.TraceError<long>((long)this.GetHashCode(), "Unable to load accepted domains; deferring mail item {0}", this.currentMailItem.RecordId);
				this.nextHopConnection.AckMailItem(AckStatus.Retry, DeliveryAgentConnection.FailedToLoadAcceptedDomainsResponse, null, new TimeSpan?(InternalDeliveryAgentEventSource.DefaultMailItemRetryInterval), MessageTrackingSource.QUEUE, "DeliveryAgentDequeue", LatencyComponent.DeliveryAgent, true);
				this.currentMailItem = null;
			}
			return;
			IL_99:
			this.currentAcceptedDomains = perTenantAcceptedDomainTable.AcceptedDomainTable;
		}

		private void ReleaseCurrentMailItem()
		{
			this.currentMailItem = null;
			this.currentAcceptedDomains = null;
		}

		private void CreateMexSession(bool mailItemSpecificEvent)
		{
			AcceptedDomainTable firstOrgAcceptedDomainTable;
			IReadOnlyMailItem readOnlyMailItem;
			if (mailItemSpecificEvent)
			{
				if (this.currentMailItem == null)
				{
					throw new InvalidOperationException("this.currentMailItem cannot be null for mail-item-specific event");
				}
				if (this.currentAcceptedDomains == null)
				{
					throw new InvalidOperationException("this.currentAcceptedDomains cannot be null for mail-item-specific event");
				}
				firstOrgAcceptedDomainTable = this.currentAcceptedDomains;
				readOnlyMailItem = this.currentMailItem;
			}
			else
			{
				firstOrgAcceptedDomainTable = Components.Configuration.FirstOrgAcceptedDomainTable;
				readOnlyMailItem = null;
			}
			this.mexSession = this.mexEvents.GetExecutionContext(this.connector.DeliveryProtocol, DeliveryAgentServer.GetInstance(readOnlyMailItem, firstOrgAcceptedDomainTable), delegate
			{
				this.TrackAsyncMessage(this.currentMailItem);
			}, delegate
			{
				this.TrackAsyncMessageCompleted(this.currentMailItem);
			}, () => this.SavePoisonContext(this.currentMailItem));
		}

		private void ReleaseMexSession()
		{
			if (this.mexSession != null)
			{
				this.mexEvents.FreeExecutionContext(this.mexSession);
				this.mexSession = null;
			}
		}

		private RoutedMailItemWrapper CreateDeliverableMailItem()
		{
			return new RoutedMailItemWrapper(this.currentMailItem, this.nextHopConnection.ReadyRecipientsList);
		}

		private void TrackBeginAgentInvocation(RoutedMailItem mailItem, LatencyComponent latencyComponent)
		{
			if (mailItem != null)
			{
				this.SavePoisonContext(mailItem);
				if (this.mexSession.AgentLatencyTracker != null)
				{
					this.mexSession.AgentLatencyTracker.BeginTrackLatency(latencyComponent, mailItem.LatencyTracker);
				}
			}
			this.stats.AgentInvokeStart();
		}

		private void TrackAsyncMessage(RoutedMailItem mailItem)
		{
			TransportMailItem.TrackAsyncMessage(mailItem.InternetMessageId);
		}

		private void TrackAsyncMessageCompleted(RoutedMailItem mailItem)
		{
			TransportMailItem.TrackAsyncMessageCompleted(mailItem.InternetMessageId);
		}

		private bool SavePoisonContext(RoutedMailItem mailItem)
		{
			return TransportMailItem.SetPoisonContext(mailItem.RecordId, mailItem.InternetMessageId, MessageProcessingSource.DeliveryAgent);
		}

		private void TrackEndAgentInvocation()
		{
			if (this.mexSession.AgentLatencyTracker != null && this.currentMailItem != null)
			{
				this.mexSession.AgentLatencyTracker.EndTrackLatency();
			}
			this.stats.AgentInvokeEnd();
		}

		private void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.nextHopConnection.AckConnection(ackStatus, smtpResponse, null);
		}

		private static readonly SmtpResponse ConnectionNotHandledResponse = new SmtpResponse("451", "4.4.0", new string[]
		{
			"No DeliveryAgent handled the queue"
		});

		private static readonly SmtpResponse MessageNotHandledResponse = new SmtpResponse("451", "4.4.0", new string[]
		{
			"DeliveryAgent returned without processing message"
		});

		private static readonly SmtpResponse RecipientNotHandledResponse = new SmtpResponse("451", "4.4.0", new string[]
		{
			"DeliveryAgent returned without processing recipient"
		});

		private static readonly SmtpResponse FailedToLoadAcceptedDomainsResponse = new SmtpResponse("450", "4.5.1", new string[]
		{
			"Failed to load accepted domains"
		});

		private static readonly SmtpResponse DefaultUnregisterConnectionResponse = new SmtpResponse("250", "2.0.0", new string[]
		{
			"DeliveryAgent successfully closed the connection"
		});

		private static string messageFailedWithTransientException = "DeliveryAgent failed to process message with id = {0} with transient exception {1}";

		private static string messageFailedWithPermanentException = "DeliveryAgent failed to process message with id = {0} with permanent exception {1}";

		private ulong sessionId = SessionId.GetNextSessionId();

		private DeliveryAgentMExEvents mexEvents;

		private DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession;

		private DeliveryAgentConnection.AsyncResult connectionAsyncResult;

		private DeliveryAgentConnector connector;

		private NextHopConnection nextHopConnection;

		private string remoteHost;

		private RoutedMailItem currentMailItem;

		private AcceptedDomainTable currentAcceptedDomains;

		private bool retire;

		private DeliveryAgentConnection.Stats stats;

		private enum State
		{
			OnOpenConnection,
			OnDeliverMailItem,
			OnCloseConnection,
			Done
		}

		public class Stats
		{
			public virtual int NumMessagesDelivered
			{
				get
				{
					return this.numMessagesDelivered;
				}
			}

			public virtual long NumBytesDelivered
			{
				get
				{
					return this.numBytesDelivered;
				}
			}

			public virtual bool HasOpenConnection
			{
				get
				{
					return this.hasOpenConnection;
				}
			}

			public virtual void Initialize(string deliveryProtocol)
			{
				this.perfCounters = DeliveryAgentPerfCounters.GetInstance(deliveryProtocol);
			}

			public virtual void MessageDelivered(int numRecipients, long numBytes)
			{
				this.numMessagesDelivered++;
				this.numBytesDelivered += numBytes;
				this.perfCounters.MessagesDeliveredTotal.Increment();
				this.perfCounters.RecipientDeliveriesCompletedTotal.IncrementBy((long)numRecipients);
				this.perfCounters.MessageBytesSentTotal.IncrementBy(numBytes);
			}

			public virtual void ConnectionStarted()
			{
				this.perfCounters.CurrentConnectionCount.Increment();
				this.hasOpenConnection = true;
			}

			public virtual void ConnectionStopped()
			{
				this.perfCounters.ConnectionsCompletedTotal.Increment();
				this.perfCounters.CurrentConnectionCount.Decrement();
				this.hasOpenConnection = false;
			}

			public virtual void ConnectionFailed()
			{
				this.perfCounters.ConnectionsFailedTotal.Increment();
			}

			public virtual void MessageFailed()
			{
				this.perfCounters.MessagesFailedTotal.Increment();
			}

			public virtual void MessageDeferred()
			{
				this.perfCounters.MessagesDeferredTotal.Increment();
			}

			public virtual void AgentInvokeStart()
			{
				this.agentInvocationTime = DateTime.UtcNow;
				this.perfCounters.InvocationTotal.Increment();
			}

			public virtual void AgentInvokeEnd()
			{
				TimeSpan timeSpan = DateTime.UtcNow - this.agentInvocationTime;
				this.perfCounters.InvocationDurationTotal.IncrementBy((long)timeSpan.TotalSeconds);
			}

			private DeliveryAgentPerfCountersInstance perfCounters;

			private DateTime agentInvocationTime;

			private int numMessagesDelivered;

			private long numBytesDelivered;

			private bool hasOpenConnection;
		}

		private class AsyncResult : IAsyncResult
		{
			public AsyncResult(object asyncState, AsyncCallback asyncCallback)
			{
				this.asyncState = asyncState;
				this.asyncCallback = asyncCallback;
			}

			public object AsyncState
			{
				get
				{
					return this.asyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.operationIsCompleted;
				}
			}

			public void SetAsyncIfNecessary(IAsyncResult ar)
			{
			}

			public void Completed()
			{
				this.operationIsCompleted = true;
				this.asyncCallback(this);
				this.asyncCallback = null;
			}

			private object asyncState;

			private AsyncCallback asyncCallback;

			private bool operationIsCompleted;
		}
	}
}
