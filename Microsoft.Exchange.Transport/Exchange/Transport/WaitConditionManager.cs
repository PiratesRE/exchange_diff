using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal abstract class WaitConditionManager
	{
		public WaitConditionManager(int maxExecutingThreadsLimit, IWaitConditionManagerConfig config, ICostFactory factory, IProcessingQuotaComponent processingQuotaComponent, Func<DateTime> timeProvider, Trace tracer)
		{
			if (maxExecutingThreadsLimit <= 0)
			{
				throw new ArgumentOutOfRangeException("maxExecutingThreadsLimit");
			}
			this.config = config;
			CostConfiguration costConfig = new CostConfiguration(this.config, false, maxExecutingThreadsLimit, (long)((double)maxExecutingThreadsLimit * this.config.ThrottlingHistoryInterval.TotalMilliseconds), processingQuotaComponent, timeProvider);
			this.costMap = factory.CreateMap(costConfig, new IsLocked(this.IsLocked), new IsLockedOnQueue(this.IsLockedOnQueue), tracer);
			this.tracer = tracer;
		}

		public bool MapStateChanged
		{
			get
			{
				return this.mapStateChanged;
			}
		}

		public void AddToLocked(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.WaitlistNewItem);
			this.AddToWaitList(condition, queue);
			this.mapStateChanged = true;
		}

		public bool TryActivate(NextHopSolutionKey queue)
		{
			WaitCondition[] array = this.costMap.Unlock(queue);
			if (array == null || array.Length == 0)
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ItemNotFoundToActivate);
				return false;
			}
			foreach (WaitCondition condition in array)
			{
				this.ActivateAndUpdate(condition, queue);
			}
			this.mapStateChanged = true;
			return true;
		}

		public void RevokeToken(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ReturnTokenUnused);
			this.CompleteItem(condition, queue);
			this.costMap.FailToken(condition);
			this.mapStateChanged = true;
		}

		public void MoveLockedToDisabled(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.AddDisabled);
			QueueWaitList queueWaitList;
			if (!this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				queueWaitList = this.AddToWaitList(condition, queue);
			}
			lock (queueWaitList)
			{
				if (!queueWaitList.MoveToDisabled(queue))
				{
					queueWaitList.Reset();
					QueueWaitList orAdd = this.WaitMap.GetOrAdd(condition, queueWaitList);
					orAdd.Add(queue);
					orAdd.MoveToDisabled(queue);
				}
			}
		}

		public void CleanupQueue(NextHopSolutionKey queue)
		{
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.CleanupQueue);
			List<KeyValuePair<WaitCondition, QueueWaitList>> list = new List<KeyValuePair<WaitCondition, QueueWaitList>>();
			foreach (KeyValuePair<WaitCondition, QueueWaitList> item in this.WaitMap)
			{
				if (item.Value.Cleanup(queue))
				{
					this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.RemoveEmptyWaitlist);
					list.Add(item);
				}
			}
			foreach (KeyValuePair<WaitCondition, QueueWaitList> keyValuePair in list)
			{
				lock (keyValuePair.Value)
				{
					if (keyValuePair.Value.Cleanup(queue))
					{
						this.RemoveEmptyList(keyValuePair.Key);
					}
				}
				this.mapStateChanged = true;
			}
		}

		public void CleanupQueue(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			QueueWaitList queueWaitList;
			if (this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.CleanupQueue);
				this.tracer.TraceDebug<NextHopSolutionKey, WaitCondition>((long)this.GetHashCode(), "Removing queue '{0}' from condition '{1}'", queue, condition);
				lock (queueWaitList)
				{
					if (queueWaitList.Cleanup(queue))
					{
						this.RemoveEmptyList(condition);
					}
				}
				this.mapStateChanged = true;
			}
		}

		public void CleanupItem(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			QueueWaitList queueWaitList;
			if (this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.CleanupItem);
				this.tracer.TraceDebug<NextHopSolutionKey, WaitCondition>((long)this.GetHashCode(), "Removing message in queue '{0}' from condition '{1}'", queue, condition);
				lock (queueWaitList)
				{
					if (queueWaitList.CleanupItem(queue))
					{
						this.RemoveEmptyList(condition);
					}
				}
				this.mapStateChanged = true;
			}
		}

		internal NextHopSolutionKey[] ActivateAll(WaitCondition condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			QueueWaitList queueWaitList;
			if (this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				lock (queueWaitList)
				{
					this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ActivateAll);
					this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Activating all queues for condition '{0}'", condition);
					NextHopSolutionKey[] result = queueWaitList.Clear();
					this.RemoveEmptyList(condition);
					this.mapStateChanged = true;
					return result;
				}
			}
			return null;
		}

		internal virtual XElement GetDiagnosticInfo(bool showVerbose)
		{
			XElement xelement = new XElement("conditionManager");
			xelement.Add(new XElement("MapStateChanged", this.mapStateChanged));
			xelement.Add(this.costMap.GetDiagnosticInfo(showVerbose));
			XElement xelement2 = new XElement("LockedConditions");
			if (!showVerbose)
			{
				xelement2.Add(new XElement("count", this.WaitMap.Count));
			}
			else
			{
				foreach (KeyValuePair<WaitCondition, QueueWaitList> keyValuePair in this.WaitMap)
				{
					XElement xelement3 = new XElement("condition");
					xelement3.Add(new XElement("name", keyValuePair.Key.ToString()));
					xelement3.Add(new XElement("lockedMessages", keyValuePair.Value.MessageCount));
					xelement3.Add(new XElement("tokens", keyValuePair.Value.PendingMessageCount));
					xelement2.Add(xelement3);
				}
			}
			xelement.Add(xelement2);
			return xelement;
		}

		internal virtual void TimedUpdate()
		{
			if (this.config.ProcessingTimeThrottlingEnabled || this.config.ThrottlingMemoryMaxThreshold.ToBytes() > 0UL)
			{
				this.mapStateChanged = true;
			}
			this.costMap.TimedUpdate();
		}

		protected abstract bool Activate(WaitCondition condition, NextHopSolutionKey queue);

		protected abstract void Lock(ILockableItem item, WaitCondition condition, NextHopSolutionKey queue, string lockReason);

		protected bool AllowMessageOrLock(WaitCondition condition, NextHopSolutionKey queue, ILockableItem item, out Cost cost)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			QueueWaitList queueWaitList;
			if (this.WaitMap.TryGetValue(condition, out queueWaitList) && (queueWaitList.GetPendingMessageCount(queue) > 0 || queueWaitList.GetMessageCount(queue) > 0))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.OlderItemFound);
				string lockReason = WaitConditionManager.GetLockReason(queueWaitList, queue);
				this.AddToWaitList(condition, queueWaitList, queue);
				this.tracer.TraceDebug<WaitCondition, NextHopSolutionKey>((long)this.GetHashCode(), "Older locked items or tokens found for condition '{0}' on queue '{1}'", condition, queue);
				this.Lock(item, condition, queue, lockReason);
				cost = null;
				return false;
			}
			if (this.costMap.Allow(condition, out cost))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.IncrementInUse);
				return true;
			}
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ConditionExceedsQuota);
			this.AddToWaitList(condition, queue);
			string diagnosticString = this.costMap.GetDiagnosticString(condition);
			this.Lock(item, condition, queue, diagnosticString);
			cost = null;
			return false;
		}

		protected bool AllowMessageOrLock(WaitCondition condition, NextHopSolutionKey queue, ILockableItem item, AccessToken token, out Cost cost)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (token != null && token.Validate(condition) && token.Return(false))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.NewItemTokenUsed);
				this.costMap.ReturnToken(condition, out cost);
				this.CompleteItem(condition, queue);
				return true;
			}
			return this.AllowMessageOrLock(condition, queue, item, out cost);
		}

		protected void MessageCompleted(WaitCondition condition, NextHopSolutionKey queue, DateTime startTime)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.DecrementInUse);
			this.costMap.CompleteProcessing(condition, startTime);
			this.costMap.RemoveThread(condition);
			this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Condition '{0}' completed processing", condition);
			this.mapStateChanged = true;
		}

		protected void ActivateSucceeded(WaitCondition condition, NextHopSolutionKey queue, QueueWaitList waitList)
		{
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.UpdatePriority);
			lock (waitList)
			{
				if (waitList.ConfirmRemove(queue))
				{
					this.RemoveEmptyList(condition);
				}
			}
			this.tracer.TraceDebug<WaitCondition, NextHopSolutionKey>((long)this.GetHashCode(), "Updating condition '{0}' that queue '{1}' activated an item", condition, queue);
		}

		protected void ActivateFailed(WaitCondition condition, NextHopSolutionKey queue, AccessToken token, bool undoWaiting)
		{
			this.tracer.TraceDebug<NextHopSolutionKey, WaitCondition>((long)this.GetHashCode(), "could not activate message in queue '{0}' for condition '{1}'.", queue, condition);
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ItemNotFoundToActivate);
			this.costMap.FailToken(condition);
			if (undoWaiting)
			{
				QueueWaitList queueWaitList;
				if (this.WaitMap.TryGetValue(condition, out queueWaitList))
				{
					lock (queueWaitList)
					{
						if (queueWaitList.RemoveWaitingAndOneOutstanding(queue))
						{
							this.RemoveEmptyList(condition);
						}
						goto IL_79;
					}
				}
				throw new InvalidOperationException("Can't find waitList for a condition we were trying to unlock");
			}
			IL_79:
			if (token != null)
			{
				token.Return(false);
			}
		}

		protected void DisabledMessagesCleared(WaitCondition condition, NextHopSolutionKey queue, QueueWaitList waitList)
		{
			lock (waitList)
			{
				if (waitList.DisabledMessagesCleared(queue))
				{
					this.RemoveEmptyList(condition);
				}
			}
			this.tracer.TraceDebug<WaitCondition, NextHopSolutionKey>((long)this.GetHashCode(), "Updating condition '{0}' on queue '{1}' has cleared all disabled messages", condition, queue);
		}

		private static string GetLockReason(QueueWaitList waitList, NextHopSolutionKey queue)
		{
			return string.Format("W/Tk {0}/{1}", waitList.GetMessageCount(queue), waitList.GetPendingMessageCount(queue));
		}

		private QueueWaitList AddToWaitList(WaitCondition condition, NextHopSolutionKey queue)
		{
			this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Adding item to waitlist for condition '{0}'", condition);
			QueueWaitList orAdd = this.WaitMap.GetOrAdd(condition, new QueueWaitList(this.tracer));
			this.AddToWaitList(condition, orAdd, queue);
			return orAdd;
		}

		private void AddToWaitList(WaitCondition condition, QueueWaitList waitList, NextHopSolutionKey queue)
		{
			if (waitList == null)
			{
				throw new ArgumentNullException("waitList");
			}
			lock (waitList)
			{
				int messageCount;
				if (waitList.Add(queue))
				{
					messageCount = waitList.MessageCount;
				}
				else
				{
					waitList.Reset();
					QueueWaitList orAdd = this.WaitMap.GetOrAdd(condition, waitList);
					orAdd.Add(queue);
					messageCount = orAdd.MessageCount;
				}
				if (messageCount == 1)
				{
					this.costMap.AddToIndex(condition);
				}
			}
		}

		private bool ActivateAndUpdate(WaitCondition condition, NextHopSolutionKey queue)
		{
			if (this.Activate(condition, queue))
			{
				this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.ActivateWaitingItemFound);
				this.tracer.TraceDebug<WaitCondition, NextHopSolutionKey>((long)this.GetHashCode(), "Activation successful for condition '{0}' on queue '{1}'", condition, queue);
				return true;
			}
			this.tracer.TraceDebug<WaitCondition, NextHopSolutionKey>((long)this.GetHashCode(), "Activation failed for condition '{0}' on queue '{1}' ", condition, queue);
			return false;
		}

		private void CompleteItem(WaitCondition condition, NextHopSolutionKey queue)
		{
			QueueWaitList queueWaitList;
			if (this.WaitMap.TryGetValue(condition, out queueWaitList))
			{
				lock (queueWaitList)
				{
					if (queueWaitList.CompleteItem(queue))
					{
						this.RemoveEmptyList(condition);
					}
				}
			}
		}

		private void RemoveEmptyList(WaitCondition condition)
		{
			this.breadcrumbs.Drop(WaitConditionManagerBreadcrumbs.RemoveEmptyWaitlist);
			QueueWaitList queueWaitList;
			this.WaitMap.TryRemove(condition, out queueWaitList);
		}

		private bool IsLockedOnQueue(WaitCondition condition, NextHopSolutionKey queue)
		{
			QueueWaitList queueWaitList;
			return this.WaitMap.TryGetValue(condition, out queueWaitList) && queueWaitList.GetMessageCount(queue) > 0;
		}

		private bool IsLocked(WaitCondition condition)
		{
			QueueWaitList queueWaitList;
			return this.WaitMap.TryGetValue(condition, out queueWaitList) && queueWaitList.MessageCount > 0;
		}

		private const int NumberOfBreadcrumbs = 32;

		protected readonly CostMap costMap;

		protected readonly ConcurrentDictionary<WaitCondition, QueueWaitList> WaitMap = new ConcurrentDictionary<WaitCondition, QueueWaitList>();

		protected readonly object SyncRoot = new object();

		protected readonly IWaitConditionManagerConfig config;

		protected bool mapStateChanged;

		protected Breadcrumbs<WaitConditionManagerBreadcrumbs> breadcrumbs = new Breadcrumbs<WaitConditionManagerBreadcrumbs>(32);

		protected Trace tracer;
	}
}
