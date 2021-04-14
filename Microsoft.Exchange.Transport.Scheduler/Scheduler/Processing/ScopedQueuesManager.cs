using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class ScopedQueuesManager : IScopedQueuesManager, IQueueLogProvider
	{
		public ScopedQueuesManager(TimeSpan scopedQueueTtl, TimeSpan updateInterval, IQueueFactory queueFactory, ISchedulerThrottler throttler, ISchedulerDiagnostics schedulerDiagnostics, Func<DateTime> timeProvider = null)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("scopedQueueTtl", scopedQueueTtl, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("updateInterval", updateInterval, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfNull("queueFactory", queueFactory);
			ArgumentValidator.ThrowIfNull("throttler", throttler);
			ArgumentValidator.ThrowIfNull("schedulerDiagnostics", schedulerDiagnostics);
			this.ScopedQueueTtl = scopedQueueTtl;
			this.UpdateInterval = updateInterval;
			this.queueFactory = queueFactory;
			this.throttler = throttler;
			this.schedulerDiagnostics = schedulerDiagnostics;
			this.timeProvider = timeProvider;
			this.LastUpdated = this.GetCurrentTime();
			this.schedulerDiagnostics.RegisterQueueLogging(this);
		}

		public IDictionary<IMessageScope, ScopedQueue> ScopedQueue
		{
			get
			{
				return this.scopedQueues;
			}
		}

		public DateTime LastUpdated { get; private set; }

		private TimeSpan UpdateInterval { get; set; }

		private TimeSpan ScopedQueueTtl { get; set; }

		public void Add(ISchedulableMessage message, IMessageScope throttledScope)
		{
			if (!this.scopedQueues.ContainsKey(throttledScope))
			{
				this.scopedQueues.Add(throttledScope, new ScopedQueue(throttledScope, this.queueFactory.CreateNewQueueInstance(), this.timeProvider));
				this.schedulerDiagnostics.ScopedQueueCreated(1);
			}
			this.scopedQueues[throttledScope].Enqueue(message);
		}

		public bool IsAlreadyScoped(IEnumerable<IMessageScope> scopes, out IMessageScope throttledScope)
		{
			foreach (IMessageScope messageScope in scopes)
			{
				if (this.scopedQueues.ContainsKey(messageScope) && !this.scopedQueues[messageScope].IsEmpty)
				{
					throttledScope = messageScope;
					return true;
				}
			}
			throttledScope = null;
			return false;
		}

		public bool TryGetNext(out ISchedulableMessage message)
		{
			while (this.nextQueueToSelect != null)
			{
				ScopedQueue value = this.nextQueueToSelect.Value;
				if (!value.Locked && !value.IsEmpty)
				{
					if (!this.ShouldBeLocked(value))
					{
						this.AdvanceUnlockedQueuesIterator();
						return value.TryDequeue(out message);
					}
					value.Lock();
				}
				LinkedListNode<ScopedQueue> node = this.nextQueueToSelect;
				this.AdvanceUnlockedQueuesIterator();
				this.unlockedQueues.Remove(node);
				if (this.unlockedQueues.Count == 0)
				{
					this.nextQueueToSelect = null;
				}
			}
			message = null;
			return false;
		}

		public void TimedUpdate()
		{
			DateTime currentTime = this.GetCurrentTime();
			if (this.LastUpdated.Add(this.UpdateInterval) < currentTime)
			{
				this.EvictUnusedQueues(currentTime);
				this.UpdateUnlockedQueuesIterator(currentTime);
				this.schedulerDiagnostics.VisitCurrentScopedQueues(this.scopedQueues);
				this.LastUpdated = this.GetCurrentTime();
			}
		}

		public IEnumerable<QueueLogInfo> FlushLogs(DateTime checkpoint, ISchedulerMetering metering)
		{
			List<QueueLogInfo> list = new List<QueueLogInfo>();
			foreach (KeyValuePair<IMessageScope, ScopedQueue> keyValuePair in this.scopedQueues)
			{
				IMessageScope key = keyValuePair.Key;
				ScopedQueue value = keyValuePair.Value;
				QueueLogInfo queueLogInfo = new QueueLogInfo(key.Display, checkpoint);
				value.Flush(checkpoint, queueLogInfo);
				UsageData usageData;
				if (metering.TryGetUsage(key, out usageData))
				{
					queueLogInfo.UsageData = usageData;
				}
				list.Add(queueLogInfo);
			}
			return list;
		}

		private void AdvanceUnlockedQueuesIterator()
		{
			if (this.nextQueueToSelect.Next != null)
			{
				this.nextQueueToSelect = this.nextQueueToSelect.Next;
				return;
			}
			this.nextQueueToSelect = this.nextQueueToSelect.List.First;
		}

		private bool ShouldBeLocked(ScopedQueue queue)
		{
			return this.throttler.ShouldThrottle(queue.Scope);
		}

		private void EvictUnusedQueues(DateTime nowTimestamp)
		{
			List<IMessageScope> list = new List<IMessageScope>();
			DateTime t = nowTimestamp - this.ScopedQueueTtl;
			foreach (KeyValuePair<IMessageScope, ScopedQueue> keyValuePair in this.scopedQueues)
			{
				ScopedQueue value = keyValuePair.Value;
				if (value.IsEmpty && value.LastActivity < t)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (IMessageScope key in list)
			{
				this.scopedQueues.Remove(key);
			}
			this.schedulerDiagnostics.ScopedQueueDestroyed(list.Count);
		}

		private void UpdateUnlockedQueuesIterator(DateTime nowTimeStamp)
		{
			List<ScopedQueue> list = new List<ScopedQueue>(this.scopedQueues.Count);
			foreach (KeyValuePair<IMessageScope, ScopedQueue> keyValuePair in this.scopedQueues)
			{
				ScopedQueue value = keyValuePair.Value;
				if (this.ShouldBeLocked(value))
				{
					value.Lock();
				}
				else
				{
					value.Unlock();
				}
				if (!value.IsEmpty && !value.Locked)
				{
					list.Add(value);
				}
			}
			list.Sort(new Comparison<ScopedQueue>(this.CompareScopedQueue));
			this.unlockedQueues = new LinkedList<ScopedQueue>(list);
			this.nextQueueToSelect = this.unlockedQueues.First;
		}

		private int CompareScopedQueue(ScopedQueue queue1, ScopedQueue queue2)
		{
			ISchedulableMessage schedulableMessage;
			queue1.TryPeek(out schedulableMessage);
			ISchedulableMessage schedulableMessage2;
			queue2.TryPeek(out schedulableMessage2);
			return schedulableMessage.SubmitTime.CompareTo(schedulableMessage2.SubmitTime);
		}

		private DateTime GetCurrentTime()
		{
			if (this.timeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.timeProvider();
		}

		private readonly IQueueFactory queueFactory;

		private readonly Func<DateTime> timeProvider;

		private readonly ISchedulerThrottler throttler;

		private readonly ISchedulerDiagnostics schedulerDiagnostics;

		private IDictionary<IMessageScope, ScopedQueue> scopedQueues = new Dictionary<IMessageScope, ScopedQueue>();

		private LinkedList<ScopedQueue> unlockedQueues = new LinkedList<ScopedQueue>();

		private LinkedListNode<ScopedQueue> nextQueueToSelect;
	}
}
