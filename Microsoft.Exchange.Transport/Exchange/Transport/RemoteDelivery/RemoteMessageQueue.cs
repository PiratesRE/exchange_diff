using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal abstract class RemoteMessageQueue : TransportMessageQueue
	{
		protected RemoteMessageQueue(RoutedQueueBase queueStorage, PriorityBehaviour behaviour, MultiQueueWaitConditionManager conditionManager) : base(queueStorage, behaviour)
		{
			this.conditionManager = conditionManager;
			this.activeMessageCountsCollection = new ReadOnlyCollection<int>(this.activeMessageCounts);
			this.deferredMessageCountsCollection = new ReadOnlyCollection<int>(this.deferredMessageCounts);
		}

		protected RemoteMessageQueue(PriorityBehaviour behaviour) : base(behaviour)
		{
			this.activeMessageCountsCollection = new ReadOnlyCollection<int>(this.activeMessageCounts);
			this.deferredMessageCountsCollection = new ReadOnlyCollection<int>(this.deferredMessageCounts);
		}

		public event Action<RoutedMailItem> OnAcquire;

		public event Action<RoutedMailItem> OnRelease;

		public abstract NextHopSolutionKey Key { get; }

		public int CountNotDeleted
		{
			get
			{
				return Components.QueueManager.GetTotalFromInstance(this.ActiveMessageCounts) + Components.QueueManager.GetTotalFromInstance(this.DeferredMessageCounts);
			}
		}

		public long LastResubmitTime
		{
			get
			{
				return Interlocked.Read(ref this.lastResubmitTime);
			}
			set
			{
				Interlocked.Exchange(ref this.lastResubmitTime, value);
			}
		}

		public LastError LastTransientError
		{
			get
			{
				return this.lastTransientError;
			}
			set
			{
				this.lastTransientError = value;
			}
		}

		protected MultiQueueWaitConditionManager ConditionManager
		{
			get
			{
				return this.conditionManager;
			}
		}

		protected virtual LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyComponent.None;
			}
		}

		protected ReadOnlyCollection<int> ActiveMessageCounts
		{
			get
			{
				return this.activeMessageCountsCollection;
			}
		}

		protected ReadOnlyCollection<int> DeferredMessageCounts
		{
			get
			{
				return this.deferredMessageCountsCollection;
			}
		}

		protected bool IsResubmitting
		{
			get
			{
				return this.resubmitReason != null;
			}
		}

		public override IQueueItem Dequeue(DeliveryPriority priority)
		{
			IQueueItem queueItem = base.Dequeue(priority);
			RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
			if (routedMailItem != null)
			{
				this.OnReleaseInternal(routedMailItem);
			}
			return queueItem;
		}

		public IQueueItem DequeueItem(DequeueMatch match)
		{
			return base.DequeueItem(match, false);
		}

		public void Lock(ILockableItem item, WaitCondition condition, string lockReason, int dehydrateThreshold)
		{
			item.LockExpirationTime = DateTimeOffset.UtcNow + Components.TransportAppConfig.ThrottlingConfig.LockExpirationInterval;
			base.Lock(item, condition, lockReason, dehydrateThreshold);
		}

		public virtual int Resubmit(ResubmitReason resubmitReason, Action<TransportMailItem> updateBeforeResubmit = null)
		{
			int num = 0;
			long num2 = 0L;
			lock (this)
			{
				num2 = DateTime.UtcNow.Ticks;
				this.LastResubmitTime = num2;
				if (this.IsResubmitting)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<ResubmitReason, ResubmitReason?>((long)this.GetHashCode(), "Resubmit for reason '{0}' not performed; Resubmission is currently in progress for reason '{1}'.", resubmitReason, this.resubmitReason);
					this.nextResubmitReason = new ResubmitReason?(resubmitReason);
					this.nextUpdateBeforeResubmit = updateBeforeResubmit;
					return 0;
				}
				this.resubmitReason = new ResubmitReason?(resubmitReason);
				goto IL_200;
			}
			IL_86:
			ICollection<IQueueItem> collection = base.DequeueAll(new Predicate<IQueueItem>(this.ShouldDequeueForResubmit));
			int num3 = 0;
			bool routeForHighAvailability = this.resubmitReason != ResubmitReason.UnreachableSameVersionHubs;
			foreach (IQueueItem queueItem in collection)
			{
				RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
				if (this.resubmitReason == ResubmitReason.UnreachableSameVersionHubs)
				{
					MessageTrackingLog.TrackHighAvailabilityRedirect(MessageTrackingSource.QUEUE, routedMailItem, "ResubmitForDirectDelivery");
				}
				DeferReason resubmitDeferReason = DeferReason.None;
				TimeSpan value = TimeSpan.Zero;
				TimeSpan configUpdateResubmitDeferInterval = Components.TransportAppConfig.RemoteDelivery.ConfigUpdateResubmitDeferInterval;
				if (resubmitReason == ResubmitReason.ConfigUpdate && configUpdateResubmitDeferInterval > TimeSpan.Zero)
				{
					resubmitDeferReason = DeferReason.ConfigUpdate;
					value = configUpdateResubmitDeferInterval;
				}
				if (!routedMailItem.Resubmit(resubmitDeferReason, new TimeSpan?(value), routeForHighAvailability, updateBeforeResubmit))
				{
					num3++;
				}
			}
			num += collection.Count - num3;
			lock (this)
			{
				if (this.LastResubmitTime != num2)
				{
					num2 = this.LastResubmitTime;
					if (this.nextResubmitReason == null)
					{
						throw new InvalidOperationException("Invalid: a queued resubmit without a reason");
					}
					this.resubmitReason = this.nextResubmitReason;
					updateBeforeResubmit = this.nextUpdateBeforeResubmit;
					this.nextResubmitReason = null;
				}
				else
				{
					this.resubmitReason = null;
					this.nextResubmitReason = null;
					this.nextUpdateBeforeResubmit = null;
				}
			}
			IL_200:
			if (!this.IsResubmitting)
			{
				if (Components.QueueManager.PerfCountersTotal != null)
				{
					Components.QueueManager.PerfCountersTotal.ItemsResubmittedTotal.IncrementBy((long)num);
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<int>((long)this.GetHashCode(), "{0} items were resubmitted", num);
				return num;
			}
			goto IL_86;
		}

		public void AttemptToGenerateDelayDSNAndDehydrateAll()
		{
			List<IQueueItem> dsnList = new List<IQueueItem>();
			DateTime now = DateTime.UtcNow;
			base.ForEach(delegate(IQueueItem item)
			{
				RoutedMailItem routedMailItem = (RoutedMailItem)item;
				if (routedMailItem.IsWorkNeeded(now))
				{
					routedMailItem.LastQueueLevelError = this.lastTransientError;
					dsnList.Add(item);
				}
			});
			dsnList.ForEach(delegate(IQueueItem item)
			{
				item.Update();
			});
		}

		protected override void DataAvailable()
		{
		}

		protected virtual SmtpResponse GetItemExpiredResponse(RoutedMailItem routedMailItem)
		{
			return AckReason.MessageExpired;
		}

		protected override void ItemExpired(IQueueItem item, bool wasEnqueued)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			ExTraceGlobals.QueuingTracer.TraceDebug<long, NextHopSolutionKey>((long)this.GetHashCode(), "Message with ID {0} has expired in queue '{1}'.", routedMailItem.RecordId, this.Key);
			if (wasEnqueued)
			{
				this.ItemRemoved(item);
			}
			else
			{
				this.RemoveFromConditionManager(item);
			}
			SmtpResponse itemExpiredResponse = this.GetItemExpiredResponse(routedMailItem);
			routedMailItem.LastQueueLevelError = this.CreatePermanentError(itemExpiredResponse, this.lastTransientError);
			bool flag;
			RiskLevel riskLevel;
			DeliveryPriority priority;
			routedMailItem.Ack(AckStatus.Fail, itemExpiredResponse, MessageTrackingSource.QUEUE, "Queue=" + this.queueStorage.Id.ToString(), null, true, out flag, out riskLevel, out priority);
			if (!flag)
			{
				Components.QueueManager.UpateInstanceCounter(riskLevel, priority, delegate(QueuingPerfCountersInstance c)
				{
					c.ItemsQueuedForDeliveryExpiredTotal.Increment();
				});
			}
		}

		protected override void ItemLockExpired(IQueueItem item)
		{
			this.ItemRemoved(item);
			Components.QueueManager.UpdatePerfCountersOnLockExpiredInDeliveryQueue();
		}

		protected override bool ItemDeferred(IQueueItem item)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			if (routedMailItem.Defer())
			{
				this.OnAcquireInternal(routedMailItem);
				this.RemoveFromConditionManager(item);
				return true;
			}
			return false;
		}

		protected override void ItemEnqueued(IQueueItem item)
		{
			this.OnAcquireInternal((RoutedMailItem)item);
		}

		protected override bool ItemActivated(IQueueItem item)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			this.OnReleaseInternal(routedMailItem);
			return routedMailItem.Activate();
		}

		protected override bool ItemLocked(IQueueItem item, WaitCondition condition, string lockReason)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			LatencyTracker.BeginTrackLatency(LatencyComponent.DeliveryQueueLocking, routedMailItem.LatencyTracker);
			routedMailItem.Lock(lockReason);
			RemoteMessageQueue.ReturnTokenIfPresent(routedMailItem);
			this.OnAcquireInternal(routedMailItem);
			return true;
		}

		protected override bool ItemUnlocked(IQueueItem item, AccessToken token)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			routedMailItem.CurrentCondition = null;
			routedMailItem.LockReason = null;
			routedMailItem.LockExpirationTime = DateTimeOffset.MinValue;
			LatencyTracker.EndTrackLatency(LatencyComponent.DeliveryQueueLocking, routedMailItem.LatencyTracker);
			if (routedMailItem.IsSuspendedByAdmin && routedMailItem.PrepareForSuspension())
			{
				this.OnReleaseInternal(routedMailItem);
				this.Enqueue(routedMailItem);
				return false;
			}
			routedMailItem.AccessToken = token;
			foreach (MailRecipient mailRecipient in routedMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Locked)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
			return true;
		}

		protected override void ItemRelocked(IQueueItem item, string lockReason, out WaitCondition condition)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			LatencyTracker.BeginTrackLatency(LatencyComponent.DeliveryQueueLocking, routedMailItem.LatencyTracker);
			condition = routedMailItem.AccessToken.Condition;
			routedMailItem.LockExpirationTime = DateTimeOffset.UtcNow + Components.TransportAppConfig.ThrottlingConfig.LockExpirationInterval;
			RemoteMessageQueue.ReturnTokenIfPresent(routedMailItem);
			this.ConditionManager.AddToLocked(condition, this.Key);
			routedMailItem.Lock(lockReason);
		}

		protected override void ItemRemoved(IQueueItem item)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			this.OnReleaseInternal(routedMailItem);
			this.RemoveFromConditionManager(routedMailItem);
		}

		protected override void ItemDehydrated(IQueueItem item)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			routedMailItem.Dehydrate(Breadcrumb.DehydrateOnMailItemLocked);
		}

		protected virtual bool ShouldDequeueForResubmit(IQueueItem queueItem)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)queueItem;
			return routedMailItem.ShouldDequeueForResubmit();
		}

		private void OnAcquireInternal(RoutedMailItem mailItem)
		{
			this.IncrementInstanceCount(mailItem);
			Interlocked.Increment(ref this.lastIncomingMessageCount);
			if (this.OnAcquire != null)
			{
				this.OnAcquire(mailItem);
			}
		}

		private void OnReleaseInternal(RoutedMailItem mailItem)
		{
			this.DecrementInstanceCount(mailItem);
			Interlocked.Increment(ref this.lastOutgoingMessageCount);
			if (this.OnRelease != null)
			{
				this.OnRelease(mailItem);
			}
		}

		private static void ReturnTokenIfPresent(RoutedMailItem mailItem)
		{
			if (mailItem.AccessToken != null)
			{
				mailItem.AccessToken.Return(true);
				mailItem.AccessToken = null;
			}
		}

		private void RemoveFromConditionManager(IQueueItem item)
		{
			RoutedMailItem routedMailItem = (RoutedMailItem)item;
			if (routedMailItem.AccessToken != null)
			{
				RemoteMessageQueue.ReturnTokenIfPresent(routedMailItem);
			}
			else if (routedMailItem.CurrentCondition != null)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.DeliveryQueueLocking, routedMailItem.LatencyTracker);
				this.ConditionManager.CleanupItem(routedMailItem.CurrentCondition, this.Key);
			}
			foreach (MailRecipient mailRecipient in routedMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Locked)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
			routedMailItem.CurrentCondition = null;
		}

		private void DecrementInstanceCount(RoutedMailItem routedMailItem)
		{
			this.DecrementInstanceCount(routedMailItem.RiskLevel, routedMailItem.Priority, routedMailItem.Deferred);
		}

		private void DecrementInstanceCount(RiskLevel riskLevel, DeliveryPriority priority, bool deferred)
		{
			IEnumerable<int> instanceCounterIndex = QueueManager.GetInstanceCounterIndex(riskLevel, priority);
			List<int> list = new List<int>();
			if (deferred)
			{
				using (IEnumerator<int> enumerator = instanceCounterIndex.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						list.Add(Interlocked.Decrement(ref this.deferredMessageCounts[num]));
					}
					goto IL_91;
				}
			}
			foreach (int num2 in instanceCounterIndex)
			{
				list.Add(Interlocked.Decrement(ref this.activeMessageCounts[num2]));
			}
			IL_91:
			foreach (int num3 in list)
			{
				if (num3 < 0)
				{
					throw new InvalidOperationException(string.Format("Cannot decrement message count for {0}deferred messages below zero", deferred ? string.Empty : "non "));
				}
			}
		}

		private void IncrementInstanceCount(RoutedMailItem item)
		{
			this.IncrementInstanceCount(item.RiskLevel, item.Priority, item.Deferred);
		}

		private void IncrementInstanceCount(RiskLevel riskLevel, DeliveryPriority priority, bool deferred)
		{
			IEnumerable<int> instanceCounterIndex = QueueManager.GetInstanceCounterIndex(riskLevel, priority);
			if (deferred)
			{
				using (IEnumerator<int> enumerator = instanceCounterIndex.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						Interlocked.Increment(ref this.deferredMessageCounts[num]);
					}
					return;
				}
			}
			foreach (int num2 in instanceCounterIndex)
			{
				Interlocked.Increment(ref this.activeMessageCounts[num2]);
			}
		}

		private LastError CreatePermanentError(SmtpResponse smtpResponse, LastError transientError)
		{
			if (transientError != null && smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
			{
				return new LastError(transientError.LastAttemptFqdn, transientError.LastAttemptEndpoint, new DateTime?(DateTime.UtcNow), smtpResponse, transientError);
			}
			return null;
		}

		protected ResubmitReason? resubmitReason;

		private readonly MultiQueueWaitConditionManager conditionManager;

		private long lastResubmitTime = DateTime.MinValue.Ticks;

		private ResubmitReason? nextResubmitReason;

		private Action<TransportMailItem> nextUpdateBeforeResubmit;

		private int[] activeMessageCounts = new int[QueueManager.InstanceCountersLength];

		private ReadOnlyCollection<int> activeMessageCountsCollection;

		private int[] deferredMessageCounts = new int[QueueManager.InstanceCountersLength];

		private ReadOnlyCollection<int> deferredMessageCountsCollection;

		protected LastError lastTransientError;
	}
}
