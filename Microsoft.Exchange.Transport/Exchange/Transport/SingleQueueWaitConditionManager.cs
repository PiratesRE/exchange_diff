using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class SingleQueueWaitConditionManager : WaitConditionManager
	{
		public SingleQueueWaitConditionManager(ILockableQueue queue, NextHopSolutionKey queueName, int maxExecutingThreadsLimit, IWaitConditionManagerConfig config, ICostFactory factory, IProcessingQuotaComponent processingQuotaComponent, Func<DateTime> timeProvider, Trace tracer) : base(maxExecutingThreadsLimit, config, factory, processingQuotaComponent, timeProvider, tracer)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			this.queue = queue;
			this.queueName = queueName;
			this.timeProvider = timeProvider;
		}

		public ILockableItem DequeueNext()
		{
			bool flag = false;
			ILockableItem lockableItem;
			Cost cost;
			for (;;)
			{
				lockableItem = this.queue.DequeueInternal();
				if (lockableItem != null)
				{
					WaitCondition condition = lockableItem.GetCondition();
					if (condition == null)
					{
						break;
					}
					if (base.AllowMessageOrLock(condition, this.queueName, lockableItem, lockableItem.AccessToken, out cost))
					{
						goto Block_3;
					}
				}
				else
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(2869308733U);
					if (flag || !base.TryActivate(this.queueName))
					{
						goto IL_96;
					}
					flag = true;
					ExTraceGlobals.FaultInjectionTracer.TraceTest(4211486013U);
				}
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "No condition could be determined for mail item");
			return lockableItem;
			Block_3:
			lockableItem.ThrottlingContext = new ThrottlingContext(this.GetCurrentTime(), cost);
			return lockableItem;
			IL_96:
			this.mapStateChanged = false;
			return lockableItem;
		}

		public void MessageCompleted(DateTime startTime, WaitCondition condition)
		{
			base.MessageCompleted(condition, this.queueName, startTime);
		}

		public void AddToWaitlist(WaitCondition condition)
		{
			base.AddToLocked(condition, this.queueName);
		}

		public void RevokeToken(WaitCondition condition)
		{
			base.RevokeToken(condition, this.queueName);
		}

		public void CleanupQueue(WaitCondition condition)
		{
			base.CleanupQueue(condition, this.queueName);
		}

		public void CleanupItem(WaitCondition condition)
		{
			base.CleanupItem(condition, this.queueName);
		}

		protected override bool Activate(WaitCondition condition, NextHopSolutionKey queue)
		{
			AccessToken token = null;
			bool undoWaiting = false;
			QueueWaitList queueWaitList;
			if (!this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				base.ActivateFailed(condition, this.queueName, token, undoWaiting);
				return false;
			}
			if (queueWaitList.GetNextItem(this.queueName))
			{
				undoWaiting = true;
				token = new AccessToken(condition, this.queueName, this);
				if (this.queue.ActivateOne(condition, DeliveryPriority.Normal, token))
				{
					base.ActivateSucceeded(condition, this.queueName, queueWaitList);
					return true;
				}
			}
			base.ActivateFailed(condition, this.queueName, token, undoWaiting);
			return false;
		}

		protected override void Lock(ILockableItem item, WaitCondition condition, NextHopSolutionKey queueName, string lockReason)
		{
			if (!this.queueName.Equals(queueName))
			{
				throw new InvalidOperationException(string.Format("Lock called on SingleQueueWaitConditionManager and the queue passed '{0}' did not match expected '{1}'", queueName, this.queueName));
			}
			this.queue.Lock(item, condition, lockReason, this.config.LockedMessageDehydrationThreshold);
		}

		private DateTime GetCurrentTime()
		{
			DateTime result;
			if (this.timeProvider != null)
			{
				result = this.timeProvider();
			}
			else
			{
				result = DateTime.UtcNow;
			}
			return result;
		}

		private readonly ILockableQueue queue;

		private readonly NextHopSolutionKey queueName;

		private Func<DateTime> timeProvider;
	}
}
