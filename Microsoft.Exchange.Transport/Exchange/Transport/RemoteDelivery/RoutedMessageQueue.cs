using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class RoutedMessageQueue : RemoteMessageQueue, ILockableQueue, IRoutedMessageQueue, IDisposeTrackable, IDisposable
	{
		public virtual int ActiveQueueLength
		{
			get
			{
				return base.ActiveCount + ((base.ConditionManager != null && base.ConditionManager.MapStateChanged) ? base.LockedCount : 0);
			}
		}

		protected override LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyTracker.GetDeliveryQueueLatencyComponent(this.key.NextHopType.DeliveryType);
			}
		}

		public virtual int GetActiveQueueLength(DeliveryPriority priority)
		{
			if (base.SupportsFixedPriority)
			{
				return this.subQueues[(int)priority].ActiveCount + ((base.ConditionManager != null && base.ConditionManager.MapStateChanged) ? this.subQueues[(int)priority].LockedCount : 0);
			}
			return this.ActiveQueueLength;
		}

		public virtual int TotalQueueLength
		{
			get
			{
				return base.TotalCount;
			}
		}

		public int ActiveConnections
		{
			get
			{
				int result;
				lock (this.syncObject)
				{
					result = this.activeConnections[0] + this.activeConnections[1] + this.activeConnections[2];
				}
				return result;
			}
		}

		public int AttemptingConnections
		{
			get
			{
				int result;
				lock (this.syncObject)
				{
					result = this.attemptingConnections[0].Count + this.attemptingConnections[1].Count + this.attemptingConnections[2].Count;
				}
				return result;
			}
		}

		public int TotalConnections
		{
			get
			{
				int result;
				lock (this.syncObject)
				{
					result = this.ActiveConnections + this.AttemptingConnections;
				}
				return result;
			}
		}

		public int InFlightMessages
		{
			get
			{
				return this.inFlightMessages;
			}
		}

		public override NextHopSolutionKey Key
		{
			get
			{
				return this.key;
			}
		}

		public bool RetryConnectionScheduled
		{
			get
			{
				return this.retryTimerInfo != null;
			}
		}

		public DateTime LastRetryTime
		{
			get
			{
				return new DateTime(Interlocked.Read(ref this.lastRetryTime), DateTimeKind.Utc);
			}
			internal set
			{
				Interlocked.Exchange(ref this.lastRetryTime, value.Ticks);
			}
		}

		public DateTime FirstRetryTime
		{
			get
			{
				return this.firstRetryTime;
			}
		}

		public long LastDeliveryTime
		{
			get
			{
				return Interlocked.Read(ref this.lastDeliveryTime);
			}
			set
			{
				Interlocked.Exchange(ref this.lastDeliveryTime, value);
			}
		}

		public int ConnectionRetryCount
		{
			get
			{
				return this.connectionRetryCount;
			}
		}

		public long Id
		{
			get
			{
				return this.queueStorage.Id;
			}
		}

		public string NextHopDomain
		{
			get
			{
				return this.queueStorage.NextHopDomain;
			}
		}

		public bool GetRetryConnectionSchedule(out DateTime nextRetryTime)
		{
			RoutedMessageQueue.RetryTimerInfo retryTimerInfo = (RoutedMessageQueue.RetryTimerInfo)this.retryTimerInfo;
			if (retryTimerInfo != null)
			{
				nextRetryTime = retryTimerInfo.NextRetryTime;
				return true;
			}
			nextRetryTime = DateTime.MaxValue;
			return false;
		}

		public SmtpResponse LastError
		{
			get
			{
				return this.lastError;
			}
			set
			{
				this.lastError = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.inFlightMessages == 0 && base.TotalCount == 0;
			}
		}

		public bool IsAdminVisible
		{
			get
			{
				return this.Key.NextHopType != NextHopType.Heartbeat;
			}
		}

		public override bool Suspended
		{
			set
			{
				base.Suspended = value;
				if (this.Suspended)
				{
					this.RelockAllItems("Queue suspended");
				}
			}
		}

		public bool CanBeDeleted(TimeSpan idleTime)
		{
			long num = Math.Max(base.LastDequeueTime, base.LastResubmitTime);
			return this.referenceCount == 0 && this.IsEmpty && !this.Suspended && num + idleTime.Ticks < DateTime.UtcNow.Ticks;
		}

		public bool CanResubmit(TimeSpan idleTime)
		{
			if (this.key.NextHopType.IsConnectorDeliveryType || this.key.NextHopType == NextHopType.Heartbeat)
			{
				return false;
			}
			if (!this.RetryConnectionScheduled)
			{
				return false;
			}
			long ticks = DateTime.UtcNow.Ticks;
			long val = ticks - this.LastDeliveryTime;
			long val2 = ticks - base.LastResubmitTime;
			long num = Math.Min(val, val2);
			return num > idleTime.Ticks;
		}

		public bool EvaluateConnectionAttempt(DeliveryPriority priority)
		{
			if (this.RetryConnectionScheduled)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} is in retry, no connection will be created.", this.key);
				return false;
			}
			if (this.Suspended)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} is frozen, no connection will be created.", this.key);
				return false;
			}
			if (this.GetActiveQueueLength(priority) == 0)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} has no active messages, no connection will be created.", this.key);
				return false;
			}
			if (this.key.NextHopType != NextHopType.Heartbeat)
			{
				return true;
			}
			bool flag;
			bool flag2;
			Components.ShadowRedundancyComponent.ShadowRedundancyManager.EvaluateHeartbeatAttempt(this.key, out flag, out flag2);
			if (flag2)
			{
				this.AbortHeartbeat();
			}
			if (!flag)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat connection not created for queue {0}.", this.key);
			}
			return flag;
		}

		public void NDRAllMessages(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, SmtpResponse smtpResponse, AckDetails details)
		{
			if (this.key.NextHopType == NextHopType.Heartbeat)
			{
				throw new InvalidOperationException("Should never attempt to NDR heartbeat messages");
			}
			if (base.DeferredCount > 0)
			{
				return;
			}
			if (this.TotalConnections != 0)
			{
				return;
			}
			ICollection<IQueueItem> collection = base.DequeueAll((IQueueItem item) => true, false);
			foreach (IQueueItem queueItem in collection)
			{
				RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
				bool flag;
				RiskLevel riskLevel;
				DeliveryPriority priority;
				routedMailItem.Ack(AckStatus.Fail, smtpResponse, messageTrackingSource, messageTrackingSourceContext, details, false, out flag, out riskLevel, out priority);
				if (!flag)
				{
					Components.QueueManager.UpateInstanceCounter(riskLevel, priority, delegate(QueuingPerfCountersInstance c)
					{
						c.ItemsCompletedDeliveryTotal.Increment();
					});
				}
			}
		}

		public override void Enqueue(IQueueItem item)
		{
			RoutedMailItem routedMailItem = item as RoutedMailItem;
			if (routedMailItem == null)
			{
				throw new InvalidOperationException("Attempt to enqueue a non-routedmailitem (or null) to the RoutedMessageQueue");
			}
			routedMailItem.EnqueuedTime = DateTime.UtcNow;
			if (this.orgId == null && !string.IsNullOrEmpty(this.key.OverrideSource))
			{
				this.orgId = routedMailItem.OrganizationId;
			}
			base.Enqueue(item);
		}

		public void UpdateQueue()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > this.lastDeferEventLogTime.Add(Components.TransportAppConfig.QueueConfiguration.MessageDeferEventCheckInterval))
			{
				this.HandleDeferEventLogging();
				this.lastDeferEventLogTime = utcNow;
			}
			base.UpdateQueueRates();
			base.TimedUpdate();
		}

		private void HandleDeferEventLogging()
		{
			bool retryConnectionScheduled = this.RetryConnectionScheduled;
			AckDetails ackDetails = new AckDetails(new IPEndPoint(IPAddress.None, 0), this.Key.NextHopDomain, null, this.Key.NextHopConnector.ToString(), IPAddress.None);
			RoutedMessageQueue.DeferLoggingState state = new RoutedMessageQueue.DeferLoggingState(DateTime.UtcNow, retryConnectionScheduled, this.Suspended, ackDetails);
			if (RoutedMessageQueue.RetryDeferLoggingEnabled)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "HandleDeferEventLogging: ForEach message call HandleLogDeferRetryOrSuspended. Start.");
				base.ForEach<RoutedMessageQueue.DeferLoggingState>(new Action<IQueueItem, RoutedMessageQueue.DeferLoggingState>(this.HandleLogDeferRetryOrSuspended), state, true);
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "HandleDeferEventLogging: ForEach message call HandleLogDeferRetryOrSuspended. Done.");
			}
			if (RoutedMessageQueue.DelayDeferLoggingEnabled && !this.Suspended && !retryConnectionScheduled)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "HandleDeferEventLogging: ForEach message (except deferred queue) call HandleLogDeferDelay. Start.");
				base.ForEach<RoutedMessageQueue.DeferLoggingState>(new Action<IQueueItem, RoutedMessageQueue.DeferLoggingState>(this.HandleLogDeferDelay), state, false);
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "HandleDeferEventLogging: ForEach message (except deferred queue) call HandleLogDeferDelay. Done.");
			}
		}

		public void HandleLogDeferRetryOrSuspended(IQueueItem item, RoutedMessageQueue.DeferLoggingState state)
		{
			RoutedMailItem routedMailItem = item as RoutedMailItem;
			if (!routedMailItem.IsDeletedByAdmin && !routedMailItem.RetryDeferLogged && state.ScanStartTime.Ticks > routedMailItem.OriginalEnqueuedTime.Add(RoutedMessageQueue.RetryDeferLoggingInterval).Ticks)
			{
				if (state.QueueSuspended || routedMailItem.IsSuspendedByAdmin)
				{
					routedMailItem.TrackDeferRetryOrSuspended(SmtpResponse.QueueSuspended, state.AckDetails);
					return;
				}
				if (state.QueueInRetry)
				{
					routedMailItem.TrackDeferRetryOrSuspended(this.LastError, state.AckDetails);
					return;
				}
				routedMailItem.TrackDeferIfRecipientsInRetry(this.LastError, state.AckDetails);
			}
		}

		public void HandleLogDeferDelay(IQueueItem item, RoutedMessageQueue.DeferLoggingState state)
		{
			RoutedMailItem routedMailItem = item as RoutedMailItem;
			if (!routedMailItem.IsDeletedByAdmin && !routedMailItem.RetryDeferLogged && !routedMailItem.DelayDeferLogged && !routedMailItem.IsInactive && state.ScanStartTime.Ticks > routedMailItem.EnqueuedTime.Add(RoutedMessageQueue.DelayDeferLoggingInterval).Ticks)
			{
				routedMailItem.TrackDeferDelay(SmtpResponse.QueueLarge, state.AckDetails, base.ActiveCount.ToString());
			}
		}

		public RoutedMailItem GetNextMailItem(DeliveryPriority priority)
		{
			if (this.Suspended)
			{
				return null;
			}
			RoutedMailItem routedMailItem;
			if (base.ConditionManager != null)
			{
				routedMailItem = (RoutedMailItem)base.ConditionManager.DequeueNext(this, priority);
				if (routedMailItem != null)
				{
					routedMailItem.ThrottlingContext.AddMemoryCost(ByteQuantifiedSize.FromBytes((ulong)routedMailItem.GetCurrentMimeSize()));
				}
			}
			else
			{
				routedMailItem = (RoutedMailItem)((ILockableQueue)this).DequeueInternal(priority);
			}
			if (routedMailItem != null)
			{
				routedMailItem.InitializeDeliveryLatencyTracking();
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2865114429U, routedMailItem.Subject);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3938856253U, routedMailItem.Subject);
			}
			return routedMailItem;
		}

		ILockableItem ILockableQueue.DequeueInternal(DeliveryPriority priority)
		{
			while (!this.Suspended && !base.IsResubmitting)
			{
				Interlocked.Increment(ref this.inFlightMessages);
				RoutedMailItem routedMailItem = (RoutedMailItem)this.Dequeue(priority);
				if (routedMailItem == null)
				{
					Interlocked.Decrement(ref this.inFlightMessages);
					return null;
				}
				PoisonMessage.Context = ((IQueueItem)routedMailItem).GetMessageContext(MessageProcessingSource.Queue);
				switch (routedMailItem.PrepareForDelivery())
				{
				case RoutedMailItem.PrepareForDeliveryResult.Deliver:
					return routedMailItem;
				case RoutedMailItem.PrepareForDeliveryResult.IgnoreDeleted:
					Interlocked.Decrement(ref this.inFlightMessages);
					break;
				case RoutedMailItem.PrepareForDeliveryResult.Requeue:
					this.Enqueue(routedMailItem);
					Interlocked.Decrement(ref this.inFlightMessages);
					break;
				default:
					throw new InvalidOperationException("Unexpected PrepareForDeliveryResult");
				}
			}
			return null;
		}

		ILockableItem ILockableQueue.DequeueInternal()
		{
			throw new InvalidOperationException("RoutedMessageQueue does not support dequeue without priority");
		}

		void ILockableQueue.Lock(ILockableItem item, WaitCondition condition, string lockReason, int dehydrateThreshold)
		{
			base.Lock(item, condition, lockReason, dehydrateThreshold);
			Interlocked.Decrement(ref this.inFlightMessages);
		}

		internal void AckMessage(RoutedMailItem routedMailItem, Queue<AckStatusAndResponse> recipientResponses, AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails ackDetails, DeferReason resubmitDeferReason, TimeSpan? resubmitDeferInterval, TimeSpan? retryInterval, MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, IEnumerable<MailRecipient> readyRecipients, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			routedMailItem.FinalizeDeliveryLatencyTracking(deliveryComponent);
			DeliveryPriority priority = routedMailItem.Priority;
			RiskLevel riskLevel = routedMailItem.RiskLevel;
			WaitCondition currentCondition = routedMailItem.CurrentCondition;
			bool flag;
			routedMailItem.Ack(ackStatus, smtpResponse, recipientResponses, readyRecipients, messageTrackingSource, messageTrackingSourceContext, ackDetails, reportEndToEndLatencies, resubmitDeferReason, resubmitDeferInterval, retryInterval, remoteMta, shadowed, primaryServer, false, out flag);
			if (flag)
			{
				LatencyTracker.BeginTrackLatency(LatencyTracker.GetDeliveryQueueLatencyComponent(routedMailItem.DeliveryType), routedMailItem.LatencyTracker);
				this.Enqueue(routedMailItem);
			}
			else
			{
				Components.QueueManager.UpateInstanceCounter(riskLevel, priority, delegate(QueuingPerfCountersInstance c)
				{
					c.ItemsCompletedDeliveryTotal.Increment();
				});
			}
			if (ackStatus == AckStatus.Success)
			{
				this.LastDeliveryTime = DateTime.UtcNow.Ticks;
				this.ResetConnectionRetryCount();
				base.LastTransientError = null;
				this.firstRetryTime = DateTime.MinValue;
			}
			Interlocked.Decrement(ref this.inFlightMessages);
			if (base.ConditionManager != null && currentCondition != null)
			{
				base.ConditionManager.MessageCompleted(currentCondition, this.Key);
				routedMailItem.CurrentCondition = null;
			}
		}

		public void Retry(TimerCallback callBackDelegate, TimeSpan? interval, SmtpResponse response, AckDetails ackDetails)
		{
			lock (this.syncObject)
			{
				if (this.ActiveConnections == 0 && !this.Suspended)
				{
					if (this.key.NextHopType == NextHopType.Heartbeat)
					{
						bool flag2;
						Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyHeartbeatRetry(this.key, out flag2);
						if (flag2)
						{
							this.AbortHeartbeat();
							return;
						}
					}
					if (this.firstRetryTime == DateTime.MinValue)
					{
						this.firstRetryTime = DateTime.UtcNow;
					}
					if (this.ShouldResubmitQueueDueToOutboundConnectorChange())
					{
						this.Resubmit(ResubmitReason.OutboundConnectorChange, null);
					}
					else
					{
						this.RelockAllItems("Queue in retry");
						this.SetScheduledCallback(callBackDelegate, interval);
						this.LastError = response;
						if (response.SmtpResponseType == SmtpResponseType.TransientError && ackDetails != null)
						{
							base.LastTransientError = new LastError(ackDetails.RemoteHostName, ackDetails.RemoteEndPoint, new DateTime?(this.LastRetryTime), response);
						}
					}
				}
			}
		}

		private bool ShouldResubmitQueueDueToOutboundConnectorChange()
		{
			if (this.orgId != null && !string.IsNullOrEmpty(this.key.OverrideSource) && this.key.OverrideSource.StartsWith("Connector"))
			{
				long num = Math.Max(this.firstRetryTime.Ticks, base.LastResubmitTime);
				if (DateTime.UtcNow.Ticks - num > Components.Configuration.AppConfig.RemoteDelivery.ResubmitDueToOutboundConnectorChangeInterval.Ticks)
				{
					try
					{
						PerTenantOutboundConnectors perTenantOutboundConnectors;
						if (Components.Configuration.TryGetTenantOutboundConnectors(this.orgId, out perTenantOutboundConnectors))
						{
							TimeSpan timeSpan = Components.Configuration.AppConfig.PerTenantCache.OutboundConnectorsCacheExpirationInterval + Components.Configuration.AppConfig.RemoteDelivery.OutboundConnectorLookbackBufferInterval;
							foreach (TenantOutboundConnector tenantOutboundConnector in perTenantOutboundConnectors.TenantOutboundConnectors)
							{
								if (tenantOutboundConnector.WhenChangedUTC != null && tenantOutboundConnector.WhenChangedUTC.Value.Ticks > num - timeSpan.Ticks)
								{
									return true;
								}
							}
							return perTenantOutboundConnectors.TenantOutboundConnectors.Length == 0;
						}
					}
					catch (TenantOutboundConnectorsRetrievalException ex)
					{
						Exception exception = ex.Result.Exception;
						QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RetryQueueOutboundConnectorLookupFailed, this.key.OverrideSource, new object[]
						{
							this.GetQueueName(),
							(exception != null) ? exception.Message : "<NULL>",
							(exception != null) ? exception.StackTrace : "<NULL>"
						});
						return false;
					}
					return false;
				}
			}
			return false;
		}

		public bool EvaluateResubmitDueToConfigUpdate(TimerCallback callBackDelegate)
		{
			if (this.key.NextHopType == NextHopType.Heartbeat)
			{
				bool flag;
				Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyHeartbeatConfigChanged(this.key, out flag);
				if (flag)
				{
					this.AbortHeartbeat();
				}
				else
				{
					this.SetScheduledCallback(callBackDelegate, null);
				}
				return false;
			}
			return true;
		}

		public override int Resubmit(ResubmitReason resubmitReason, Action<TransportMailItem> updateBeforeResubmit = null)
		{
			return this.ResubmitAsync(resubmitReason, updateBeforeResubmit).Result;
		}

		public async Task<int> ResubmitAsync(ResubmitReason resubmitReason, Action<TransportMailItem> updateBeforeResubmit = null)
		{
			DateTime endTime = DateTime.UtcNow;
			bool firstResubmit = true;
			int resubmitCount = 0;
			int result;
			if (!this.resubmittingLock.Wait(0))
			{
				result = 0;
			}
			else
			{
				try
				{
					await Task.Run(async delegate()
					{
						while (!this.Suspended)
						{
							resubmitCount += this.<>n__FabricatedMethode(resubmitReason, updateBeforeResubmit);
							if (firstResubmit)
							{
								firstResubmit = false;
								endTime = DateTime.UtcNow + RoutedMessageQueue.QueueResubmitRetryTimeout;
							}
							if (!(DateTime.UtcNow < endTime) || this.InFlightMessages <= 0)
							{
								return;
							}
							await Task.Delay(RoutedMessageQueue.QueueResubmitRetryInterval);
						}
						ExTraceGlobals.QueuingTracer.TraceDebug<long, ResubmitReason>((long)this.GetHashCode(), "A resubmit request for the queue {0} due to reason '{1}' was not performed because the queue is frozen.", this.Id, resubmitReason);
					});
				}
				finally
				{
					this.resubmittingLock.Release();
				}
				this.LogResubmitEvent(resubmitReason, resubmitCount);
				result = resubmitCount;
			}
			return result;
		}

		public int CreateAttemptingConnection(DeliveryPriority priority, out long connectionId)
		{
			connectionId = Interlocked.Increment(ref this.nextConnectionId);
			int totalConnectionsCount;
			lock (this.syncObject)
			{
				this.attemptingConnections[(int)priority].Add(connectionId);
				totalConnectionsCount = this.GetTotalConnectionsCount(priority);
			}
			return totalConnectionsCount;
		}

		public void ConnectionAttemptSucceeded(DeliveryPriority priority, long connectionId)
		{
			lock (this.syncObject)
			{
				if (!this.attemptingConnections[(int)priority].Contains(connectionId))
				{
					throw new InvalidOperationException("Next Hop Connection Id: {0} with priority: {1} does not exist in attempting connections collection.");
				}
				this.attemptingConnections[(int)priority].Remove(connectionId);
				this.activeConnections[(int)priority]++;
			}
		}

		public int CloseConnection(DeliveryPriority priority, long connectionId)
		{
			int totalConnectionsCount;
			lock (this.syncObject)
			{
				if (this.attemptingConnections[(int)priority].Contains(connectionId))
				{
					this.attemptingConnections[(int)priority].Remove(connectionId);
				}
				else
				{
					if (this.activeConnections[(int)priority] <= 0)
					{
						throw new InvalidOperationException("The active connection count is zero, while trying to close the connection.");
					}
					this.activeConnections[(int)priority]--;
				}
				totalConnectionsCount = this.GetTotalConnectionsCount(priority);
			}
			return totalConnectionsCount;
		}

		private int GetTotalConnectionsCount(DeliveryPriority priority)
		{
			return this.activeConnections[(int)priority] + this.attemptingConnections[(int)priority].Count;
		}

		public void ResetScheduledCallback()
		{
			RoutedMessageQueue.RetryTimerInfo retryTimerInfo = (RoutedMessageQueue.RetryTimerInfo)Interlocked.Exchange(ref this.retryTimerInfo, null);
			if (retryTimerInfo != null)
			{
				retryTimerInfo.RetryTimer.Dispose();
			}
		}

		public void SetScheduledCallback(TimerCallback callBackDelegate, TimeSpan? interval)
		{
			if (this.retryTimerInfo != null)
			{
				return;
			}
			int num = this.connectionRetryCount + 1;
			bool flag = false;
			TimeSpan timeSpan;
			if (interval != null)
			{
				timeSpan = interval.Value;
				if (num > Components.Configuration.LocalServer.TransportServer.TransientFailureRetryCount)
				{
					flag = true;
				}
			}
			else if (this.key.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				timeSpan = Components.Configuration.AppConfig.RemoteDelivery.MailboxDeliveryQueueRetryInterval;
				if (num > Components.Configuration.LocalServer.TransportServer.TransientFailureRetryCount)
				{
					flag = true;
				}
			}
			else if (this.key.NextHopType == NextHopType.Heartbeat)
			{
				timeSpan = Components.ShadowRedundancyComponent.ShadowRedundancyManager.Configuration.HeartbeatFrequency;
			}
			else if (num <= Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryCount)
			{
				timeSpan = Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryInterval;
			}
			else if (num <= Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryCount + Components.Configuration.LocalServer.TransportServer.TransientFailureRetryCount)
			{
				timeSpan = Components.Configuration.LocalServer.TransportServer.TransientFailureRetryInterval;
			}
			else
			{
				timeSpan = Components.Configuration.LocalServer.TransportServer.OutboundConnectionFailureRetryInterval;
				flag = true;
			}
			RoutedMessageQueue.RetryTimerInfo retryTimerInfo = new RoutedMessageQueue.RetryTimerInfo(new Timer(callBackDelegate, this, timeSpan, TimeSpan.Zero), DateTime.UtcNow + timeSpan);
			if (Interlocked.CompareExchange(ref this.retryTimerInfo, retryTimerInfo, null) != null)
			{
				retryTimerInfo.RetryTimer.Dispose();
				return;
			}
			if (flag)
			{
				base.AttemptToGenerateDelayDSNAndDehydrateAll();
			}
		}

		public int IncrementConnectionRetryCount()
		{
			return Interlocked.Increment(ref this.connectionRetryCount);
		}

		public void ResetConnectionRetryCount()
		{
			Interlocked.Exchange(ref this.connectionRetryCount, 0);
		}

		internal void GetQueueCounts(out int[] activeCount, out int[] retryCount)
		{
			lock (this.syncObject)
			{
				IEnumerable<int> instanceCounterIndex = QueueManager.GetInstanceCounterIndex(RiskLevel.Normal, DeliveryPriority.Normal);
				if (this.RetryConnectionScheduled || (this.ActiveConnections == 0 && this.AttemptingConnections != 0))
				{
					activeCount = new int[QueueManager.InstanceCountersLength];
					retryCount = base.ActiveMessageCounts.Zip(base.DeferredMessageCounts, (int a, int b) => a + b).ToArray<int>();
					using (IEnumerator<int> enumerator = instanceCounterIndex.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int num = enumerator.Current;
							retryCount[num] += this.inFlightMessages;
						}
						goto IL_119;
					}
				}
				activeCount = base.ActiveMessageCounts.ToArray<int>();
				foreach (int num2 in instanceCounterIndex)
				{
					activeCount[num2] += this.inFlightMessages;
				}
				retryCount = base.DeferredMessageCounts.ToArray<int>();
				IL_119:;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.resubmittingLock != null)
			{
				this.resubmittingLock.Dispose();
				this.resubmittingLock = null;
			}
			this.disposed = true;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RoutedMessageQueue>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal void GetQueueCountsOnlyForIndividualPriorities(out int[] activeCount, out int[] retryCount)
		{
			this.GetQueueCounts(out activeCount, out retryCount);
			activeCount = activeCount.Take(QueueManager.PriorityToInstanceIndexMap.Count).ToArray<int>();
			retryCount = retryCount.Take(QueueManager.PriorityToInstanceIndexMap.Count).ToArray<int>();
		}

		private RoutedMessageQueue(RoutedQueueBase queueStorage, PriorityBehaviour priorityBehaviour) : this(queueStorage, priorityBehaviour, null, null)
		{
		}

		private RoutedMessageQueue(RoutedQueueBase queueStorage, PriorityBehaviour priorityBehaviour, MultiQueueWaitConditionManager conditionManager, OrganizationId orgId)
		{
			this.activeConnections = new int[3];
			this.attemptingConnections = new HashSet<long>[]
			{
				new HashSet<long>(),
				new HashSet<long>(),
				new HashSet<long>()
			};
			this.syncObject = new object();
			this.lastError = SmtpResponse.Empty;
			this.lastRetryTime = DateTime.MinValue.Ticks;
			this.lastDeliveryTime = DateTime.MinValue.Ticks;
			this.lastDeferEventLogTime = DateTime.MinValue;
			this.firstRetryTime = DateTime.MinValue;
			this.resubmittingLock = new SemaphoreSlim(1);
			base..ctor(queueStorage, priorityBehaviour, conditionManager);
			this.orgId = orgId;
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected RoutedMessageQueue(PriorityBehaviour priorityBehavior)
		{
			this.activeConnections = new int[3];
			this.attemptingConnections = new HashSet<long>[]
			{
				new HashSet<long>(),
				new HashSet<long>(),
				new HashSet<long>()
			};
			this.syncObject = new object();
			this.lastError = SmtpResponse.Empty;
			this.lastRetryTime = DateTime.MinValue.Ticks;
			this.lastDeliveryTime = DateTime.MinValue.Ticks;
			this.lastDeferEventLogTime = DateTime.MinValue;
			this.firstRetryTime = DateTime.MinValue;
			this.resubmittingLock = new SemaphoreSlim(1);
			base..ctor(priorityBehavior);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private static PriorityBehaviour NewQueueBehaviour(DeliveryType deliveryType)
		{
			PriorityBehaviour result = PriorityBehaviour.IgnorePriority;
			if (Components.IsBridgehead)
			{
				if (Components.Configuration.AppConfig.RemoteDelivery.LocalDeliveryPriorityQueuingEnabled && deliveryType == DeliveryType.MapiDelivery)
				{
					result = PriorityBehaviour.QueuePriority;
				}
				else if (deliveryType != DeliveryType.MapiDelivery && deliveryType != DeliveryType.NonSmtpGatewayDelivery && deliveryType != DeliveryType.DeliveryAgent)
				{
					if (Components.Configuration.AppConfig.RemoteDelivery.PriorityQueuingEnabled)
					{
						result = PriorityBehaviour.Fixed;
					}
					else if (Components.Configuration.AppConfig.RemoteDelivery.RemoteDeliveryPriorityQueuingEnabled)
					{
						result = PriorityBehaviour.QueuePriority;
					}
				}
			}
			return result;
		}

		public static RoutedMessageQueue NewQueue(NextHopSolutionKey key)
		{
			return RoutedMessageQueue.NewQueue(key, null, null);
		}

		public static RoutedMessageQueue NewQueue(NextHopSolutionKey key, MultiQueueWaitConditionManager conditionManager, OrganizationId orgId)
		{
			RoutedQueueBase queueStorage = Components.MessagingDatabase.CreateQueue(key, false);
			PriorityBehaviour priorityBehaviour = RoutedMessageQueue.NewQueueBehaviour(key.NextHopType.DeliveryType);
			return new RoutedMessageQueue(queueStorage, priorityBehaviour, conditionManager, orgId)
			{
				key = key
			};
		}

		public static RoutedMessageQueue LoadQueue(RoutedQueueBase queueStorage, MultiQueueWaitConditionManager conditionManager)
		{
			if (queueStorage.NextHopType == NextHopType.Heartbeat)
			{
				RoutedMessageQueue.Delete(queueStorage);
				return null;
			}
			PriorityBehaviour priorityBehaviour = RoutedMessageQueue.NewQueueBehaviour(queueStorage.NextHopType.DeliveryType);
			RoutedMessageQueue routedMessageQueue = new RoutedMessageQueue(queueStorage, priorityBehaviour, conditionManager, null);
			routedMessageQueue.ComputeKey();
			return routedMessageQueue;
		}

		public override void Delete()
		{
			RoutedMessageQueue.Delete(this.queueStorage);
			base.Delete();
		}

		public void AddReference()
		{
			Interlocked.Increment(ref this.referenceCount);
		}

		public void ReleaseReference()
		{
			Interlocked.Decrement(ref this.referenceCount);
		}

		private static void Delete(RoutedQueueBase queueStorage)
		{
			queueStorage.MarkToDelete();
			queueStorage.Commit();
		}

		protected override bool ItemDeferred(IQueueItem item)
		{
			RoutedMailItem mailItem = (RoutedMailItem)item;
			Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemDeferred(mailItem, this, item.DeferUntil);
			return base.ItemDeferred(item);
		}

		private void AbortHeartbeat()
		{
			ICollection<IQueueItem> collection = base.DequeueAll((IQueueItem item) => true, false);
			foreach (IQueueItem queueItem in collection)
			{
				RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
				routedMailItem.AbortHeartbeat();
				Components.QueueManager.UpateInstanceCounter(routedMailItem.RiskLevel, routedMailItem.Priority, delegate(QueuingPerfCountersInstance c)
				{
					c.ItemsCompletedDeliveryTotal.Increment();
				});
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat aborted for queue {0}.", this.key);
		}

		private void ComputeKey()
		{
			this.key = new NextHopSolutionKey(this.queueStorage.NextHopType, this.queueStorage.NextHopDomain, this.queueStorage.NextHopConnector, this.queueStorage.NextHopTlsDomain);
		}

		private void RelockAllItems(string lockReason)
		{
			base.RelockAll(lockReason, delegate(IQueueItem item)
			{
				RoutedMailItem routedMailItem = (RoutedMailItem)item;
				return routedMailItem.AccessToken != null;
			});
		}

		private string GetQueueName()
		{
			return string.Format("'{0}':'{1}':'{2}'", this.key.NextHopType, this.key.NextHopDomain, this.key.NextHopConnector);
		}

		private void LogResubmitEvent(ResubmitReason reason, int resubmitCount)
		{
			if (resubmitCount <= 0 || reason == ResubmitReason.Admin)
			{
				return;
			}
			string queueName = this.GetQueueName();
			if (reason == ResubmitReason.ConfigUpdate)
			{
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitDueToConfigUpdate, null, new object[]
				{
					resubmitCount,
					queueName
				});
				return;
			}
			if (reason == ResubmitReason.UnreachableSameVersionHubs)
			{
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitDueToUnavailabilityOfSameVersionHubs, null, new object[]
				{
					resubmitCount,
					queueName
				});
				return;
			}
			if (reason != ResubmitReason.OutboundConnectorChange)
			{
				QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitDueToInactivityTimeout, null, new object[]
				{
					resubmitCount,
					queueName,
					Components.Configuration.AppConfig.RemoteDelivery.MaxIdleTimeBeforeResubmit
				});
				return;
			}
			QueueManager.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResubmitDueToOutboundConnectorChange, null, new object[]
			{
				resubmitCount,
				queueName
			});
		}

		private static readonly TimeSpan RetryDeferLoggingInterval = Components.TransportAppConfig.QueueConfiguration.MinLargeQueueDeferEventInterval;

		private static readonly bool RetryDeferLoggingEnabled = RoutedMessageQueue.RetryDeferLoggingInterval.CompareTo(TimeSpan.Zero) > 0;

		private static readonly TimeSpan DelayDeferLoggingInterval = Components.TransportAppConfig.QueueConfiguration.MinQueueRetryOrSuspendDeferEventInterval;

		private static readonly bool DelayDeferLoggingEnabled = RoutedMessageQueue.DelayDeferLoggingInterval.CompareTo(TimeSpan.Zero) > 0;

		private static readonly TimeSpan QueueResubmitRetryTimeout = Components.TransportAppConfig.QueueConfiguration.QueueResubmitRetryTimeout;

		private static readonly TimeSpan QueueResubmitRetryInterval = Components.TransportAppConfig.QueueConfiguration.QueueResubmitRetryInterval;

		private NextHopSolutionKey key;

		private long nextConnectionId;

		private int[] activeConnections;

		private HashSet<long>[] attemptingConnections;

		private object syncObject;

		private int referenceCount;

		private int inFlightMessages;

		private object retryTimerInfo;

		private int connectionRetryCount;

		private SmtpResponse lastError;

		private long lastRetryTime;

		private long lastDeliveryTime;

		private DateTime lastDeferEventLogTime;

		private DateTime firstRetryTime;

		private OrganizationId orgId;

		private SemaphoreSlim resubmittingLock;

		private bool disposed;

		private DisposeTracker disposeTracker;

		private class RetryTimerInfo
		{
			public RetryTimerInfo(Timer retryTimer, DateTime nextRetryTime)
			{
				this.RetryTimer = retryTimer;
				this.NextRetryTime = nextRetryTime;
			}

			public Timer RetryTimer;

			public DateTime NextRetryTime;
		}

		internal struct DeferLoggingState
		{
			public DeferLoggingState(DateTime scanStartTime, bool queueInRetry, bool queueSuspended, AckDetails ackDetails)
			{
				this.ScanStartTime = scanStartTime;
				this.QueueInRetry = queueInRetry;
				this.QueueSuspended = queueSuspended;
				this.AckDetails = ackDetails;
			}

			public readonly DateTime ScanStartTime;

			public readonly bool QueueInRetry;

			public readonly bool QueueSuspended;

			public readonly AckDetails AckDetails;
		}
	}
}
