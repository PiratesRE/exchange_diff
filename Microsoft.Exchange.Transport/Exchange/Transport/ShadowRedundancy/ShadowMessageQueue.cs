using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowMessageQueue : IQueueVisitor
	{
		public ShadowMessageQueue(RoutedQueueBase queueStorage, NextHopSolutionKey key, ShadowMessageQueue.ItemExpiredHandler itemExpiredHandler, IShadowRedundancyConfigurationSource configuration, ShouldSuppressResubmission shouldSuppressResubmission, ShadowRedundancyEventLogger shadowRedundancyEventLogger, FindRelatedBridgeHeads findRelatedBridgeHeads, GetRoutedMessageQueueStatus getRoutedMessageQueueStatus)
		{
			if (itemExpiredHandler == null)
			{
				throw new ArgumentNullException("itemExpiredHandler");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (shouldSuppressResubmission == null)
			{
				throw new ArgumentNullException("shouldSuppressResubmission");
			}
			if (shadowRedundancyEventLogger == null)
			{
				throw new ArgumentNullException("shadowRedundancyEventLogger");
			}
			if (!string.Equals(queueStorage.NextHopDomain, key.NextHopDomain, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "queueStorage.NextHopDomain = '{0}' and key.NextHopDomain = '{1}' where they should be consistent.", new object[]
				{
					queueStorage.NextHopDomain,
					key.NextHopDomain
				}));
			}
			if (findRelatedBridgeHeads == null)
			{
				this.findRelatedBridgeHeads = new FindRelatedBridgeHeads(ShadowMessageQueue.GetRelatedBridgeHeads);
			}
			else
			{
				this.findRelatedBridgeHeads = findRelatedBridgeHeads;
			}
			if (getRoutedMessageQueueStatus == null)
			{
				this.getRoutedMessageQueueStatus = new GetRoutedMessageQueueStatus(ShadowMessageQueue.GetRoutedMessageQueueStatus);
			}
			else
			{
				this.getRoutedMessageQueueStatus = getRoutedMessageQueueStatus;
			}
			this.queueStorage = queueStorage;
			this.itemExpiredHandler = itemExpiredHandler;
			this.key = key;
			this.configuration = configuration;
			this.shouldSuppressResubmission = shouldSuppressResubmission;
			this.shadowRedundancyEventLogger = shadowRedundancyEventLogger;
			if (ShadowMessageQueue.heartbeatMonitoringInterval > this.configuration.HeartbeatFrequency)
			{
				ShadowMessageQueue.heartbeatMonitoringInterval = this.configuration.HeartbeatFrequency;
			}
			this.heartbeatHelper = new ShadowRedundancyHeartbeatHelper(key, configuration, shadowRedundancyEventLogger);
		}

		public long Id
		{
			get
			{
				return this.queueStorage.Id;
			}
		}

		public NextHopSolutionKey Key
		{
			get
			{
				return this.key;
			}
		}

		public string NextHopDomain
		{
			get
			{
				return this.key.NextHopDomain;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public bool Suspended
		{
			get
			{
				return this.queueStorage.Suspended;
			}
			set
			{
				this.queueStorage.Suspended = value;
				this.queueStorage.Commit();
			}
		}

		public bool HasHeartbeatFailure
		{
			get
			{
				return this.heartbeatHelper.HasHeartbeatFailure;
			}
		}

		public bool IsResubmissionSuppressed
		{
			get
			{
				return this.suppressed;
			}
		}

		public int Count
		{
			get
			{
				return this.shadowMailItems.Count;
			}
		}

		public DateTime LastHeartbeatTime
		{
			get
			{
				return this.heartbeatHelper.LastHeartbeatTime;
			}
		}

		public DateTime LastExpiryCheck
		{
			get
			{
				return this.lastExpiryCheck;
			}
		}

		public long IgnoredDiscardIdCount
		{
			get
			{
				return this.ignoredDiscardIdCount;
			}
		}

		public long ValidDiscardIdCount
		{
			get
			{
				return this.validDiscardIdCount;
			}
		}

		public static ShadowMessageQueue NewQueue(NextHopSolutionKey key, ShadowMessageQueue.ItemExpiredHandler itemExpiredHandler, IShadowRedundancyConfigurationSource configuration, ShouldSuppressResubmission shouldSuppressResubmission, ShadowRedundancyEventLogger shadowRedundancyEventLogger, FindRelatedBridgeHeads findRelatedBridgeHeads, GetRoutedMessageQueueStatus getRoutedMessageQueueStatus)
		{
			RoutedQueueBase routedQueueBase = Components.MessagingDatabase.CreateQueue(key, false);
			return new ShadowMessageQueue(routedQueueBase, key, itemExpiredHandler, configuration, shouldSuppressResubmission, shadowRedundancyEventLogger, findRelatedBridgeHeads, getRoutedMessageQueueStatus);
		}

		public static void EnsureValidResubmitReason(ResubmitReason resubmitReason)
		{
			if (resubmitReason != ResubmitReason.Admin && resubmitReason != ResubmitReason.ShadowHeartbeatFailure && resubmitReason != ResubmitReason.ShadowStateChange)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Resubmit reason '{0}' is not applicable to Shadow Queues.", new object[]
				{
					resubmitReason
				}));
			}
		}

		public void Enqueue(ShadowMailItem shadowMailItem)
		{
			if (shadowMailItem == null)
			{
				throw new ArgumentNullException("shadowMailItem");
			}
			lock (this.shadowMailItems)
			{
				this.shadowMailItems[shadowMailItem.TransportMailItem.ShadowMessageId] = shadowMailItem;
			}
			ShadowRedundancyManager.PerfCounters.UpdateShadowQueueLength(this.key.NextHopDomain, 1);
			this.lastActivityTime = DateTime.UtcNow;
		}

		public ShadowMailItem FindShadowMailItem(Guid shadowMessageId)
		{
			if (shadowMessageId == Guid.Empty)
			{
				throw new ArgumentException("shadowMessageId can't be Guid.Empty.");
			}
			ShadowMailItem shadowMailItem = null;
			ShadowMailItem result;
			lock (this.shadowMailItems)
			{
				if (this.shadowMailItems.TryGetValue(shadowMessageId, out shadowMailItem))
				{
					result = shadowMailItem;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public void ForEach(Action<IQueueItem> action, bool includeDeferred)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (this.shadowMailItems)
			{
				foreach (KeyValuePair<Guid, ShadowMailItem> keyValuePair in this.shadowMailItems)
				{
					action(keyValuePair.Value.TransportMailItem);
				}
			}
		}

		public bool Discard(Guid shadowMessageId, DiscardReason discardReason)
		{
			if (shadowMessageId == Guid.Empty)
			{
				throw new ArgumentException("shadowMessageId can't be Guid.Empty.");
			}
			ShadowMailItem shadowMailItem = null;
			lock (this.shadowMailItems)
			{
				if (this.shadowMailItems.TryGetValue(shadowMessageId, out shadowMailItem))
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, Guid, DiscardReason>((long)this.GetHashCode(), "ShadowMessageQueue.Discard: queue {0} removing shadowMessageId={1} for reason={2}", this.key, shadowMessageId, discardReason);
					this.shadowMailItems.Remove(shadowMessageId);
					this.validDiscardIdCount += 1L;
				}
			}
			if (shadowMailItem == null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, Guid>((long)this.GetHashCode(), "ShadowMessageQueue.Discard queue {0} could not find shadowMessageId={1}", this.key, shadowMessageId);
				this.ignoredDiscardIdCount += 1L;
				return false;
			}
			shadowMailItem.Discard(discardReason);
			this.itemExpiredHandler(this, shadowMailItem);
			ShadowRedundancyManager.PerfCounters.UpdateShadowQueueLength(this.key.NextHopDomain, -1);
			this.lastActivityTime = DateTime.UtcNow;
			return true;
		}

		public void DiscardAll()
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Shadow queue '{0}' is being discarded", this.key);
			List<ShadowMailItem> list;
			lock (this.shadowMailItems)
			{
				list = new List<ShadowMailItem>(this.shadowMailItems.Values);
				this.shadowMailItems.Clear();
				this.lastActivityTime = DateTime.MinValue;
			}
			if (list.Count > 0)
			{
				foreach (ShadowMailItem shadowMailItem in list)
				{
					shadowMailItem.Discard(DiscardReason.DiscardAll);
				}
				this.NotifyExpiryHandlerAsync(list);
			}
		}

		public bool CanBeDeleted(TimeSpan idleTime)
		{
			return this.referenceCount == 0 && this.IsEmpty && !this.Suspended && this.lastActivityTime + idleTime < DateTime.UtcNow;
		}

		public void Delete()
		{
			this.queueStorage.MarkToDelete();
			this.queueStorage.Commit();
		}

		public void AddReference()
		{
			Interlocked.Increment(ref this.referenceCount);
		}

		public void ReleaseReference()
		{
			Interlocked.Decrement(ref this.referenceCount);
		}

		public void UpdateQueue(bool heartbeatEnabled, bool shadowRedundancyPaused)
		{
			DateTime utcNow = DateTime.UtcNow;
			List<ShadowMailItem> list = null;
			bool flag = false;
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, bool>((long)this.GetHashCode(), "ShadowMessageQueue.UpdateQueue for queue {0} heartbeatEnabled={1}", this.key, heartbeatEnabled);
			lock (this.shadowMailItems)
			{
				if (utcNow - this.lastExpiryCheck >= this.configuration.ShadowQueueCheckExpiryInterval)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, DateTime>((long)this.GetHashCode(), "ShadowMessageQueue.UpdateQueue for queue {0} needs to check for expired items lastExpiryCheck={1}", this.key, this.lastExpiryCheck);
					foreach (ShadowMailItem shadowMailItem in this.shadowMailItems.Values)
					{
						if (utcNow >= ((IQueueItem)shadowMailItem).Expiry)
						{
							if (list == null)
							{
								list = new List<ShadowMailItem>();
							}
							if (shadowMailItem.DiscardReason == null)
							{
								shadowMailItem.Discard(DiscardReason.Expired);
							}
							list.Add(shadowMailItem);
						}
					}
					this.RemoveShadowMailItems(list);
					this.lastExpiryCheck = utcNow;
				}
				if (heartbeatEnabled)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, DateTime, DateTime>((long)this.GetHashCode(), "ShadowMessageQueue.UpdateQueue for queue {0} checking time now {1} against last heartbeat check at {2}", this.key, utcNow, this.lastHeartbeatCheck);
					if (utcNow - this.lastHeartbeatCheck >= ShadowMessageQueue.heartbeatMonitoringInterval)
					{
						this.lastHeartbeatCheck = utcNow;
						flag = true;
					}
				}
				else
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "ShadowMessageQueue.UpdateQueue for queue {0} skipping heartbeat because it was told to", this.key);
					this.suppressed = false;
					this.lastHeartbeatCheck = utcNow;
					this.heartbeatHelper.ResetHeartbeat();
				}
			}
			this.NotifyExpiryHandlerAsync(list);
			if (flag)
			{
				if (!this.ResubmitIfNecessary(shadowRedundancyPaused))
				{
					this.CreateHeartbeatIfNecessary();
					return;
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "ShadowMessageQueue.UpdateQueue for queue {0} skipping heartbeat because ResubmitIfNecessary returned true", this.key);
			}
		}

		public int Resubmit(ResubmitReason resubmitReason)
		{
			this.suppressed = false;
			ShadowMessageQueue.EnsureValidResubmitReason(resubmitReason);
			QueueIdentity queueIdentity = new QueueIdentity(QueueType.Shadow, this.Id, this.NextHopDomain);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<QueueIdentity, ResubmitReason>((long)this.GetHashCode(), "Resubmit request for the queue '{0}' due to reason '{1}'.", queueIdentity, resubmitReason);
			int result;
			try
			{
				if (Interlocked.Increment(ref this.resubmitGuard) != 1)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<QueueIdentity>((long)this.GetHashCode(), "Another thread is currently resubmitting for queue '{0}'.", queueIdentity);
					result = 0;
				}
				else if (this.Suspended)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<QueueIdentity, ResubmitReason>((long)this.GetHashCode(), "Resubmit request for the queue {0} due to reason '{1}' was not performed because the queue is frozen.", queueIdentity, resubmitReason);
					result = 0;
				}
				else
				{
					List<ShadowMailItem> list = null;
					lock (this.shadowMailItems)
					{
						foreach (ShadowMailItem shadowMailItem in this.shadowMailItems.Values)
						{
							if (shadowMailItem.NextHopSolution.AdminActionStatus != AdminActionStatus.Suspended)
							{
								if (list == null)
								{
									list = new List<ShadowMailItem>();
								}
								list.Add(shadowMailItem);
							}
						}
						this.RemoveShadowMailItems(list);
					}
					int num = 0;
					if (list != null)
					{
						List<TransportMailItem> list2 = new List<TransportMailItem>(list.Count);
						foreach (ShadowMailItem shadowMailItem2 in list)
						{
							list2.Add(shadowMailItem2.TransportMailItem);
						}
						ShadowRedundancyResubmitHelper shadowRedundancyResubmitHelper = new ShadowRedundancyResubmitHelper(resubmitReason, this.Key);
						shadowRedundancyResubmitHelper.Resubmit(list2);
						ShadowRedundancyManager.PerfCounters.SubmitMessagesFromShadowQueue(this.key.NextHopDomain, list2.Count);
						foreach (ShadowMailItem shadowMailItem3 in list)
						{
							shadowMailItem3.Discard(DiscardReason.Resubmitted);
						}
						this.NotifyExpiryHandler(list);
						num = list.Count;
					}
					this.lastActivityTime = DateTime.UtcNow;
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int, QueueIdentity>((long)this.GetHashCode(), "Resubmitted {0} items from queue '{1}'.", num, queueIdentity);
					if (num > 0)
					{
						this.shadowRedundancyEventLogger.LogShadowRedundancyMessagesResubmitted(num, this.NextHopDomain, resubmitReason);
					}
					result = num;
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.resubmitGuard);
			}
			return result;
		}

		public void ScheduleImmediateHeartbeat()
		{
			QueueIdentity arg = new QueueIdentity(QueueType.Shadow, this.Id, this.NextHopDomain);
			ExTraceGlobals.QueuingTracer.TraceDebug<QueueIdentity>(0L, "Immediate heartbeat scheduled for queue {0}", arg);
			this.heartbeatHelper.ScheduleImmediateHeartbeat();
		}

		public void CreateHeartbeatIfNecessary()
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, bool, bool>((long)this.GetHashCode(), "ShadowMessageQueue.CreateHeartbeatIfNecessary for queue {0}: IsEmpty={1} Suspended={2}", this.key, this.IsEmpty, this.Suspended);
			if (this.IsEmpty || this.Suspended)
			{
				this.suppressed = false;
				this.heartbeatHelper.ResetHeartbeat();
				return;
			}
			this.heartbeatHelper.CreateHeartbeatIfNecessary();
		}

		public bool CanResubmit()
		{
			return this.heartbeatHelper.CanResubmit();
		}

		public void UpdateHeartbeat(DateTime heartbeatTime, NextHopSolutionKey key, bool successfulHeartbeat)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowMessageQueue.UpdateHeartbeat for queue {0} heartbeatTime={1} key={2} successfulHeartbeat={3}", new object[]
			{
				this.key,
				heartbeatTime,
				key,
				successfulHeartbeat
			});
			if (this.IsEmpty || this.Suspended)
			{
				this.suppressed = false;
				this.heartbeatHelper.ResetHeartbeat();
				return;
			}
			this.heartbeatHelper.UpdateHeartbeat(heartbeatTime, key, successfulHeartbeat);
			if (successfulHeartbeat)
			{
				this.suppressed = false;
				return;
			}
			this.shadowRedundancyEventLogger.LogPrimaryServerHeartbeatFailed(this.NextHopDomain);
			ShadowRedundancyManager.PerfCounters.HeartbeatFailure(this.NextHopDomain);
		}

		public void EvaluateHeartbeatAttempt(out bool sendHeartbeat, out bool abortHeartbeat)
		{
			sendHeartbeat = false;
			abortHeartbeat = (this.IsEmpty || this.Suspended);
			if (abortHeartbeat)
			{
				this.suppressed = false;
				this.heartbeatHelper.ResetHeartbeat();
				return;
			}
			this.heartbeatHelper.EvaluateHeartbeatAttempt(out sendHeartbeat, out abortHeartbeat);
		}

		public void NotifyHeartbeatConfigChanged(NextHopSolutionKey key, out bool abortHeartbeat)
		{
			abortHeartbeat = false;
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat queue corresponding to shadow queue '{0}' has config change", this.key);
			abortHeartbeat = (this.IsEmpty || this.Suspended);
			if (abortHeartbeat)
			{
				this.suppressed = false;
				this.heartbeatHelper.ResetHeartbeat();
			}
			else
			{
				this.heartbeatHelper.UpdateHeartbeat(DateTime.UtcNow, key, false);
			}
			if (this.heartbeatHelper.CanResubmit())
			{
				this.DiscardAll();
				abortHeartbeat = true;
				this.suppressed = false;
			}
		}

		public void NotifyConfigUpdated(IShadowRedundancyConfigurationSource oldConfiguration)
		{
			if (oldConfiguration == null)
			{
				throw new ArgumentNullException("ShadowMessageConfigChange: oldConfiguration");
			}
			this.heartbeatHelper.NotifyConfigUpdated(oldConfiguration);
		}

		private static IEnumerable<INextHopServer> GetRelatedBridgeHeads(NextHopSolutionKey nextHopSolutionKey)
		{
			IEnumerable<INextHopServer> result;
			if (Components.RoutingComponent.MailRouter.TryGetRelatedServersForShadowQueue(nextHopSolutionKey, out result))
			{
				return result;
			}
			return null;
		}

		private static QueueStatus GetRoutedMessageQueueStatus(NextHopSolutionKey key)
		{
			SmtpSendConnectorConfig smtpSendConnectorConfig;
			if (!Components.RoutingComponent.MailRouter.TryGetLocalSendConnector<SmtpSendConnectorConfig>(key.NextHopConnector, out smtpSendConnectorConfig) || smtpSendConnectorConfig.DNSRoutingEnabled)
			{
				return QueueStatus.None;
			}
			NextHopSolutionKey nextHopSolutionKey = new NextHopSolutionKey(DeliveryType.SmartHostConnectorDelivery, smtpSendConnectorConfig.SmartHostsString, key.NextHopConnector);
			RoutedMessageQueue queue = Components.RemoteDeliveryComponent.GetQueue(nextHopSolutionKey);
			if (queue == null)
			{
				return QueueStatus.None;
			}
			if (queue.Suspended)
			{
				return QueueStatus.Suspended;
			}
			if (queue.ActiveConnections > 0)
			{
				return QueueStatus.Active;
			}
			if (queue.AttemptingConnections > 0)
			{
				return QueueStatus.Connecting;
			}
			DateTime dateTime;
			if (queue.GetRetryConnectionSchedule(out dateTime))
			{
				return QueueStatus.Retry;
			}
			return QueueStatus.Ready;
		}

		private void RemoveShadowMailItems(IEnumerable<ShadowMailItem> shadowMailItemsToRemove)
		{
			if (shadowMailItemsToRemove != null)
			{
				foreach (ShadowMailItem shadowMailItem in shadowMailItemsToRemove)
				{
					this.shadowMailItems.Remove(shadowMailItem.TransportMailItem.ShadowMessageId);
				}
			}
		}

		private bool ResubmitIfNecessary(bool shadowRedundancyPaused)
		{
			bool result = false;
			if (this.Suspended)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} was not resubmitted because it is frozen.", this.key);
			}
			else if (Components.RemoteDeliveryComponent.IsPaused)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} was not resubmitted because Remote Delivery is paused.", this.key);
			}
			else if (shadowRedundancyPaused)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Queue {0} was not resubmitted because Shadow Redundancy is paused.", this.key);
			}
			else if (this.CanResubmit())
			{
				QueueStatus queueStatus = this.getRoutedMessageQueueStatus(this.key);
				bool flag;
				if (queueStatus == QueueStatus.Ready || queueStatus == QueueStatus.Active)
				{
					flag = false;
				}
				else
				{
					IEnumerable<INextHopServer> relatedBridgeheads = this.findRelatedBridgeHeads(this.key);
					flag = this.shouldSuppressResubmission(relatedBridgeheads);
				}
				if (!flag)
				{
					this.Resubmit(ResubmitReason.ShadowHeartbeatFailure);
					result = true;
				}
				else if (!this.suppressed)
				{
					this.suppressed = true;
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, string>((long)this.GetHashCode(), "Queue {0} was not resubmitted because resubmission for the {1} is suppressed.", this.key, this.key.NextHopDomain);
					this.shadowRedundancyEventLogger.LogShadowRedundancyMessageResubmitSuppressed(this.shadowMailItems.Count, this.NextHopDomain, Strings.ShadowRedundancyNoActiveServerInNexthopSolution);
				}
				this.heartbeatHelper.ResetHeartbeat();
			}
			return result;
		}

		private void NotifyExpiryHandlerCallback(object state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			IEnumerable<ShadowMailItem> enumerable = state as IEnumerable<ShadowMailItem>;
			if (enumerable == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "State is of type '{0}' rather than 'IEnumerable<ShadowMailItem>'.", new object[]
				{
					state.GetType()
				}));
			}
			this.NotifyExpiryHandler(enumerable);
		}

		private void NotifyExpiryHandlerAsync(ICollection<ShadowMailItem> expiredItems)
		{
			if (expiredItems != null && expiredItems.Count > 0)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int>((long)this.GetHashCode(), "Scheduling a call to NotifyExpiryHandlerCallback() to expire '{0}' items.", expiredItems.Count);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.NotifyExpiryHandlerCallback), expiredItems);
			}
		}

		private void NotifyExpiryHandler(IEnumerable<ShadowMailItem> expiredItems)
		{
			foreach (ShadowMailItem shadowMailItem in expiredItems)
			{
				this.itemExpiredHandler(this, shadowMailItem);
				ShadowRedundancyManager.PerfCounters.UpdateShadowQueueLength(this.key.NextHopDomain, -1);
			}
		}

		private static TimeSpan heartbeatMonitoringInterval = TimeSpan.FromSeconds(1.0);

		private Dictionary<Guid, ShadowMailItem> shadowMailItems = new Dictionary<Guid, ShadowMailItem>();

		private NextHopSolutionKey key;

		private DateTime lastActivityTime = DateTime.UtcNow;

		private DateTime lastExpiryCheck = DateTime.MinValue;

		private ShadowRedundancyHeartbeatHelper heartbeatHelper;

		private DateTime lastHeartbeatCheck = DateTime.UtcNow;

		private int referenceCount;

		private ShadowMessageQueue.ItemExpiredHandler itemExpiredHandler;

		private IShadowRedundancyConfigurationSource configuration;

		private RoutedQueueBase queueStorage;

		private int resubmitGuard;

		private ShouldSuppressResubmission shouldSuppressResubmission;

		private ShadowRedundancyEventLogger shadowRedundancyEventLogger;

		private FindRelatedBridgeHeads findRelatedBridgeHeads;

		private GetRoutedMessageQueueStatus getRoutedMessageQueueStatus;

		private bool suppressed;

		private long ignoredDiscardIdCount;

		private long validDiscardIdCount;

		internal delegate void ItemExpiredHandler(ShadowMessageQueue shadowMessageQueue, ShadowMailItem shadowMailItem);
	}
}
