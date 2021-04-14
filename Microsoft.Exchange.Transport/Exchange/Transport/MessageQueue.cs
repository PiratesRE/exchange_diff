using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal class MessageQueue : IQueueVisitor
	{
		static MessageQueue()
		{
			MessageQueue.expiryCheckPeriodForFixedPriorityBehaviour = MessageQueue.GetExpiryCheckPeriod(MessageQueue.GetSmallestExpirationInterval());
			Components.Configuration.LocalServerChanged += MessageQueue.TransportServerConfigUpdate;
		}

		public MessageQueue(PriorityBehaviour behaviour) : this(behaviour, Components.TransportAppConfig.RemoteDelivery.DeliveryPriorityQuotas)
		{
		}

		internal MessageQueue(PriorityBehaviour behaviour, int[] deliveryPriorityQuotas)
		{
			if (behaviour == PriorityBehaviour.QueuePriority)
			{
				if (deliveryPriorityQuotas == null)
				{
					throw new ArgumentNullException("deliveryPriorityQuotas");
				}
				if (deliveryPriorityQuotas.Length < 1)
				{
					throw new ArgumentException("deliveryPriorityQuotas");
				}
				this.subQueues = new FifoQueue[deliveryPriorityQuotas.Length];
				this.deliveryPriorityQuotas = deliveryPriorityQuotas;
				this.remainingDeliveryPriorityQuotas = new int[this.deliveryPriorityQuotas.Length];
				this.remainingActivationPriorityQuotas = new int[this.deliveryPriorityQuotas.Length];
				for (int i = 0; i < this.deliveryPriorityQuotas.Length; i++)
				{
					if (this.deliveryPriorityQuotas[i] < 0)
					{
						throw new ArgumentOutOfRangeException(string.Format("deliveryPriorityQuotas[{0}]", i));
					}
				}
				for (int j = 0; j < deliveryPriorityQuotas.Length; j++)
				{
					this.subQueues[j] = new FifoQueue();
					this.remainingDeliveryPriorityQuotas[j] = this.deliveryPriorityQuotas[j];
					this.remainingActivationPriorityQuotas[j] = this.deliveryPriorityQuotas[j];
				}
			}
			else
			{
				this.subQueues = new FifoQueue[this.numberOfDeliveryPriorities];
				for (int k = 0; k < this.numberOfDeliveryPriorities; k++)
				{
					this.subQueues[k] = new FifoQueue();
				}
				this.remainingActivationPriorityQuotas = new int[]
				{
					40,
					20,
					4,
					1
				};
			}
			this.filledCountsPerPriority = new int[this.numberOfDeliveryPriorities];
			this.behaviour = behaviour;
		}

		public int ActiveCount
		{
			get
			{
				int result;
				lock (this)
				{
					int num = this.filled;
					for (int i = 0; i < this.subQueues.Length; i++)
					{
						num += this.subQueues[i].ActiveCount;
					}
					result = num;
				}
				return result;
			}
		}

		public int ActiveCountExcludingPriorityNone
		{
			get
			{
				int result;
				lock (this)
				{
					int num = this.filled - this.filledCountsPerPriority[this.numberOfDeliveryPriorities - 1];
					for (int i = 0; i < this.subQueues.Length - 1; i++)
					{
						num += this.subQueues[i].ActiveCount;
					}
					result = num;
				}
				return result;
			}
		}

		public int DeferredCount
		{
			get
			{
				return this.deferredQueue.TotalCount;
			}
		}

		public int LockedCount
		{
			get
			{
				int result;
				lock (this)
				{
					int num = 0;
					for (int i = 0; i < this.subQueues.Length; i++)
					{
						num += this.subQueues[i].LockedCount;
					}
					result = num;
				}
				return result;
			}
		}

		public int TotalCount
		{
			get
			{
				int result;
				lock (this)
				{
					int num = this.filled;
					for (int i = 0; i < this.subQueues.Length; i++)
					{
						num += this.subQueues[i].TotalCount;
					}
					result = num + this.deferredQueue.TotalCount;
				}
				return result;
			}
		}

		public bool SupportsFixedPriority
		{
			get
			{
				return this.behaviour == PriorityBehaviour.Fixed;
			}
		}

		public TimeSpan ExpiryDuration
		{
			get
			{
				if (this.expiryCheckPeriod != null)
				{
					return this.expiryCheckPeriod.Value;
				}
				if (this.behaviour != PriorityBehaviour.Fixed)
				{
					return MessageQueue.expiryCheckPeriodForOtherPriorityBehaviours;
				}
				return MessageQueue.expiryCheckPeriodForFixedPriorityBehaviour;
			}
			set
			{
				if (value < MessageQueue.minExpiryCheckPeriod)
				{
					throw new ArgumentOutOfRangeException("ExpiryDuration", string.Format("Values less than '{0}' second(s) are invalid.", MessageQueue.minExpiryCheckPeriod.TotalSeconds));
				}
				this.expiryCheckPeriod = new TimeSpan?(value);
			}
		}

		public long LastDequeueTime
		{
			get
			{
				return Interlocked.Read(ref this.lastDequeue);
			}
		}

		private bool ShouldUseFastArray
		{
			get
			{
				return this.behaviour != PriorityBehaviour.Fixed && this.behaviour != PriorityBehaviour.QueuePriority;
			}
		}

		public virtual void Enqueue(IQueueItem item)
		{
			if (item.Expiry < DateTime.UtcNow)
			{
				this.ItemExpired(item, false);
				return;
			}
			bool flag = false;
			if (item.DeferUntil != DateTime.MinValue)
			{
				if (item.DeferUntil > item.Expiry && item.DeferUntil != DateTime.MaxValue)
				{
					this.ItemExpired(item, false);
				}
				else
				{
					flag = this.ItemDeferred(item);
				}
				if (!flag)
				{
					return;
				}
			}
			lock (this)
			{
				if (flag)
				{
					this.deferredQueue.Enqueue(item);
					return;
				}
				if (this.ShouldUseFastArray && this.filled < this.items.Length && this.filled == this.ActiveCount)
				{
					this.items[this.tailIndex] = item;
					this.tailIndex = this.Advance(this.tailIndex);
					this.IncrementFilled(item.Priority);
				}
				else
				{
					DeliveryPriority itemPriority = this.GetItemPriority(item);
					this.subQueues[(int)itemPriority].Enqueue(item);
					this.extendedData = true;
				}
				this.ItemEnqueued(item);
			}
			this.DataAvailable();
		}

		public IQueueItem Dequeue()
		{
			return this.DequeueInternal(DeliveryPriority.Normal);
		}

		public virtual IQueueItem Dequeue(DeliveryPriority priority)
		{
			return this.DequeueInternal(priority);
		}

		public void ForEach(Action<IQueueItem> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (this)
			{
				this.InternalForEach(action);
			}
		}

		public void ForEach(Action<IQueueItem> action, bool includeDeferred)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (this)
			{
				this.InternalForEach(action);
				if (includeDeferred)
				{
					this.deferredQueue.ForEach(action);
				}
			}
		}

		public void ForEach<T>(Action<IQueueItem, T> action, T state, bool includeDeferred)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (this)
			{
				this.InternalForEach<T>(action, state);
				if (includeDeferred)
				{
					this.deferredQueue.ForEach<T>(action, state);
				}
			}
		}

		public void TimedUpdate()
		{
			DateTime now = DateTime.UtcNow;
			QueueItemList queueItemList = null;
			QueueItemList queueItemList2 = null;
			QueueItemList queueItemList3 = null;
			lock (this)
			{
				if (now.Ticks >= this.deferredQueue.NextActivationTime)
				{
					queueItemList = this.deferredQueue.DequeueAll((IQueueItem item) => now.Ticks >= Math.Min(item.DeferUntil.Ticks, item.Expiry.Ticks));
				}
				if (now - this.lastExpiryCheck >= this.ExpiryDuration || (this.checkForExpiriesOnNextTimedUpdate && now - this.lastExpiryCheck >= MessageQueue.minExpiryCheckPeriod))
				{
					this.checkForExpiriesOnNextTimedUpdate = false;
					queueItemList2 = this.DequeueAllInternal((IQueueItem item) => now.Ticks >= item.Expiry.Ticks, false);
					this.lastExpiryCheck = now;
				}
				if (Components.TransportAppConfig.ThrottlingConfig.LockExpirationInterval > TimeSpan.Zero && now - this.lastLockExpiryCheck > Components.TransportAppConfig.ThrottlingConfig.LockExpirationCheckPeriod)
				{
					queueItemList3 = this.DequeueAllLocked(delegate(IQueueItem item)
					{
						ILockableItem lockableItem = item as ILockableItem;
						if (lockableItem == null)
						{
							return false;
						}
						bool flag2 = now.Ticks >= lockableItem.LockExpirationTime.Ticks;
						if (flag2)
						{
							lockableItem.LockExpirationTime = DateTimeOffset.MinValue;
						}
						return flag2;
					});
					this.lastLockExpiryCheck = now;
				}
			}
			if (queueItemList != null)
			{
				queueItemList.ForEach(new Action<IQueueItem>(this.ReactivateItem));
			}
			if (queueItemList2 != null)
			{
				queueItemList2.ForEach(delegate(IQueueItem item)
				{
					this.ItemExpired(item, true);
				});
			}
			if (queueItemList3 != null)
			{
				queueItemList3.ForEach(delegate(IQueueItem item)
				{
					this.ItemLockExpired(item);
					this.Enqueue(item);
				});
			}
		}

		public QueueItemList DequeueAll(Predicate<IQueueItem> match)
		{
			return this.DequeueAll(match, true);
		}

		public QueueItemList DequeueAll(Predicate<IQueueItem> match, bool checkDeferred)
		{
			QueueItemList queueItemList;
			lock (this)
			{
				queueItemList = this.DequeueAllInternal(match, checkDeferred);
			}
			if (queueItemList != null)
			{
				queueItemList.ForEach(delegate(IQueueItem item)
				{
					this.ItemRemoved(item);
				});
			}
			return queueItemList;
		}

		public void Lock(IQueueItem item, WaitCondition condition, string lockReason, int dehydrateThreshold)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			if (item.Expiry < DateTime.UtcNow)
			{
				this.ItemExpired(item, false);
				return;
			}
			if (!this.ItemLocked(item, condition, lockReason))
			{
				return;
			}
			lock (this)
			{
				DeliveryPriority itemPriority = this.GetItemPriority(item);
				this.subQueues[(int)itemPriority].Lock(item, condition, dehydrateThreshold, new Action<IQueueItem>(this.ItemDehydrated));
			}
		}

		public bool ActivateOne(WaitCondition condition, DeliveryPriority suggestedPriority, AccessToken token)
		{
			bool flag = false;
			lock (this)
			{
				switch (this.behaviour)
				{
				case PriorityBehaviour.IgnorePriority:
				case PriorityBehaviour.RoundRobin:
				case PriorityBehaviour.QueuePriority:
					flag = this.FindItemToActivateByQuotas(condition, token);
					break;
				case PriorityBehaviour.Fixed:
					if (this.subQueues[(int)suggestedPriority].LockedCount > 0)
					{
						if (!this.subQueues[(int)suggestedPriority].ActivateOne(condition, token, new ItemUnlocked(this.ItemUnlocked)))
						{
							return false;
						}
						flag = true;
					}
					break;
				}
			}
			if (flag)
			{
				this.DataAvailable();
				return true;
			}
			return false;
		}

		public void UpdateNextActivationTime(DateTime activationTime)
		{
			lock (this)
			{
				this.deferredQueue.UpdateNextActivationTime(activationTime.Ticks);
			}
		}

		internal static void RunUnderPoisonContext(IQueueItem item, Action<IQueueItem> action)
		{
			PoisonContext context = PoisonMessage.Context;
			PoisonMessage.Context = item.GetMessageContext(MessageProcessingSource.Queue);
			action(item);
			PoisonMessage.Context = context;
		}

		internal static void RunUnderPoisonContext<T>(IQueueItem item, T state, Action<IQueueItem, T> action)
		{
			PoisonContext context = PoisonMessage.Context;
			PoisonMessage.Context = item.GetMessageContext(MessageProcessingSource.Queue);
			action(item, state);
			PoisonMessage.Context = context;
		}

		internal int GetDeferredCount(DeferReason reason)
		{
			return this.deferredQueue.GetDeferredCount(reason);
		}

		internal XElement GetDiagnosticInfo(bool verbose, bool conditionalQueuing)
		{
			XElement result;
			lock (this)
			{
				XElement xelement = new XElement("queue");
				XElement xelement2 = new XElement("counts");
				xelement2.Add(new XElement("TotalCount", this.TotalCount));
				xelement2.Add(new XElement("DeferredCount", this.DeferredCount));
				if (this.ShouldUseFastArray)
				{
					XElement content = new XElement("FastArrayCount", this.filled);
					xelement2.Add(content);
				}
				if (conditionalQueuing)
				{
					xelement2.Add(new XElement("LockedCount", this.LockedCount));
				}
				xelement.Add(xelement2);
				if (verbose)
				{
					for (int i = 0; i < this.subQueues.Length; i++)
					{
						XElement xelement3 = new XElement("priorityQueue");
						xelement3.Add(new XElement("priority", i));
						xelement.Add(this.subQueues[i].GetDiagnosticInfo(xelement3, conditionalQueuing));
					}
				}
				result = xelement;
			}
			return result;
		}

		protected void RelockAll(string lockReason, Predicate<IQueueItem> isUnlocked)
		{
			lock (this)
			{
				List<IQueueItem>[] array = new List<IQueueItem>[this.subQueues.Length];
				if (this.ShouldUseFastArray)
				{
					int num = this.headIndex;
					int i = 0;
					while (i < this.filled)
					{
						if (isUnlocked(this.items[num]))
						{
							IQueueItem queueItem = this.DequeueArrayItem(num);
							if (array[(int)queueItem.Priority] == null)
							{
								array[(int)queueItem.Priority] = new List<IQueueItem>();
							}
							array[(int)queueItem.Priority].Add(queueItem);
						}
						else
						{
							num = this.Advance(num);
							i++;
						}
					}
				}
				for (int j = 0; j < this.subQueues.Length; j++)
				{
					this.subQueues[j].RelockAll(array[j], lockReason, new ItemRelocked(this.ItemRelocked));
				}
			}
		}

		protected IQueueItem DequeueItem(DequeueMatch match, bool deferredQueueFirst)
		{
			IQueueItem queueItem = null;
			bool flag = false;
			lock (this)
			{
				if (deferredQueueFirst)
				{
					queueItem = this.deferredQueue.DequeueItem(match, out flag);
				}
				if (!flag && this.ShouldUseFastArray)
				{
					int num = this.headIndex;
					int num2 = 0;
					while (num2 < this.filled && !flag)
					{
						switch (match(this.items[num]))
						{
						case DequeueMatchResult.Break:
							flag = true;
							break;
						case DequeueMatchResult.DequeueAndBreak:
							queueItem = this.DequeueArrayItem(num);
							flag = true;
							if (this.extendedData && this.filled < 35)
							{
								this.ScheduleItems();
							}
							break;
						case DequeueMatchResult.Continue:
							num = this.Advance(num);
							num2++;
							break;
						default:
							throw new InvalidOperationException("Invalid return value from match()");
						}
					}
				}
				int num3 = 0;
				while (num3 < this.subQueues.Length && !flag)
				{
					queueItem = this.subQueues[num3].DequeueItem(match, out flag);
					num3++;
				}
				if (!deferredQueueFirst && !flag)
				{
					queueItem = this.deferredQueue.DequeueItem(match, out flag);
				}
			}
			if (queueItem != null)
			{
				this.ItemRemoved(queueItem);
			}
			return queueItem;
		}

		protected void ScheduleCheckForExpiredItems()
		{
			this.checkForExpiriesOnNextTimedUpdate = true;
		}

		protected virtual void DataAvailable()
		{
		}

		protected virtual void ItemRemoved(IQueueItem item)
		{
		}

		protected virtual void ItemExpired(IQueueItem item, bool wasEnqueued)
		{
		}

		protected virtual void ItemLockExpired(IQueueItem item)
		{
		}

		protected virtual bool ItemDeferred(IQueueItem item)
		{
			return true;
		}

		protected virtual bool ItemActivated(IQueueItem item)
		{
			return true;
		}

		protected virtual void ItemEnqueued(IQueueItem item)
		{
		}

		protected virtual bool ItemLocked(IQueueItem item, WaitCondition condition, string lockReason)
		{
			return true;
		}

		protected virtual bool ItemUnlocked(IQueueItem item, AccessToken token)
		{
			throw new NotSupportedException();
		}

		protected virtual void ItemRelocked(IQueueItem item, string lockReason, out WaitCondition condition)
		{
			condition = null;
		}

		protected virtual void ItemDehydrated(IQueueItem item)
		{
		}

		protected int Advance(int index)
		{
			if (++index >= this.items.Length)
			{
				return 0;
			}
			return index;
		}

		private static void TransportServerConfigUpdate(TransportServerConfiguration args)
		{
			MessageQueue.expiryCheckPeriodForOtherPriorityBehaviours = MessageQueue.GetExpiryCheckPeriod(Components.Configuration.LocalServer.TransportServer.MessageExpirationTimeout);
		}

		private static TimeSpan GetSmallestExpirationInterval()
		{
			TimeSpan timeSpan = Components.TransportAppConfig.RemoteDelivery.MessageExpirationTimeout(DeliveryPriority.High);
			if (timeSpan > Components.TransportAppConfig.RemoteDelivery.MessageExpirationTimeout(DeliveryPriority.Normal))
			{
				timeSpan = Components.TransportAppConfig.RemoteDelivery.MessageExpirationTimeout(DeliveryPriority.Normal);
			}
			if (timeSpan > Components.TransportAppConfig.RemoteDelivery.MessageExpirationTimeout(DeliveryPriority.Low))
			{
				timeSpan = Components.TransportAppConfig.RemoteDelivery.MessageExpirationTimeout(DeliveryPriority.Low);
			}
			return timeSpan;
		}

		private static TimeSpan GetExpiryCheckPeriod(TimeSpan expirationInterval)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(expirationInterval.TotalSeconds * 0.1);
			if (timeSpan < BackgroundProcessingThread.SlowScanInterval)
			{
				timeSpan = MessageQueue.MinExpiryCheckInterval;
			}
			else if (timeSpan > MessageQueue.MaxExpiryCheckInterval)
			{
				timeSpan = MessageQueue.MaxExpiryCheckInterval;
			}
			return timeSpan;
		}

		private void IncrementFilled(DeliveryPriority priority)
		{
			this.filled++;
			this.filledCountsPerPriority[QueueManager.PriorityToInstanceIndexMap[priority]]++;
		}

		private void DecrementFilled(DeliveryPriority priority)
		{
			this.filled--;
			this.filledCountsPerPriority[QueueManager.PriorityToInstanceIndexMap[priority]]--;
		}

		private bool IsEmpty(DeliveryPriority priority)
		{
			if (this.behaviour != PriorityBehaviour.Fixed)
			{
				return this.ActiveCount == 0;
			}
			return this.subQueues[(int)priority].ActiveCount == 0;
		}

		private IQueueItem DequeueInternal(DeliveryPriority priority)
		{
			IQueueItem queueItem = null;
			DateTime utcNow = DateTime.UtcNow;
			List<IQueueItem> list = null;
			lock (this)
			{
				while (!this.IsEmpty(priority))
				{
					Interlocked.Exchange(ref this.lastDequeue, utcNow.Ticks);
					if (this.behaviour == PriorityBehaviour.QueuePriority)
					{
						queueItem = this.PriorityBasedDequeue();
					}
					else if (this.behaviour != PriorityBehaviour.Fixed)
					{
						if (this.filled < 1)
						{
							throw new InvalidOperationException();
						}
						queueItem = this.items[this.headIndex];
						this.items[this.headIndex] = null;
						this.headIndex = this.Advance(this.headIndex);
						this.DecrementFilled(queueItem.Priority);
						if (this.extendedData && this.filled < 35)
						{
							this.ScheduleItems();
						}
					}
					else
					{
						queueItem = this.subQueues[(int)priority].Dequeue();
					}
					if (utcNow <= queueItem.Expiry)
					{
						break;
					}
					if (list == null)
					{
						list = new List<IQueueItem>();
					}
					list.Add(queueItem);
					queueItem = null;
				}
			}
			if (list != null)
			{
				list.ForEach(delegate(IQueueItem item)
				{
					this.ItemExpired(item, true);
				});
			}
			return queueItem;
		}

		private IQueueItem PriorityBasedDequeue()
		{
			this.RefillDequeueQuotasIfNeccessary();
			long num = long.MaxValue;
			int num2 = -1;
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				if (this.subQueues[i].ActiveCount != 0)
				{
					long oldestItem = this.subQueues[i].OldestItem;
					if (num > oldestItem)
					{
						num = oldestItem;
						num2 = i;
					}
					if (this.remainingDeliveryPriorityQuotas[i] > 0)
					{
						break;
					}
				}
			}
			if (num2 == -1)
			{
				throw new InvalidOperationException("The queue was not empty, but we couldn't figure out the sub queue to dequeue from.");
			}
			if (this.remainingDeliveryPriorityQuotas[num2] > 0)
			{
				this.remainingDeliveryPriorityQuotas[num2]--;
			}
			return this.subQueues[num2].Dequeue();
		}

		private void RefillDequeueQuotasIfNeccessary()
		{
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				if (this.subQueues[i].ActiveCount != 0 && this.remainingDeliveryPriorityQuotas[i] > 0)
				{
					return;
				}
			}
			for (int j = 0; j < this.deliveryPriorityQuotas.Length; j++)
			{
				this.remainingDeliveryPriorityQuotas[j] = this.deliveryPriorityQuotas[j];
			}
		}

		private bool FindItemToActivateByQuotas(WaitCondition condition, AccessToken token)
		{
			bool flag = false;
			this.RefillActivationQuotasIfNeccessary();
			int num = 0;
			while (num < this.subQueues.Length && !flag)
			{
				if (this.subQueues[num].LockedCount > 0 && this.remainingActivationPriorityQuotas[num] > 0 && this.subQueues[num].ActivateOne(condition, token, new ItemUnlocked(this.ItemUnlocked)))
				{
					this.remainingActivationPriorityQuotas[num]--;
					flag = true;
				}
				num++;
			}
			int num2 = 0;
			while (num2 < this.subQueues.Length && !flag)
			{
				if (this.subQueues[num2].LockedCount > 0 && this.remainingActivationPriorityQuotas[num2] <= 0 && this.subQueues[num2].ActivateOne(condition, token, new ItemUnlocked(this.ItemUnlocked)))
				{
					this.remainingActivationPriorityQuotas[num2]--;
					flag = true;
				}
				num2++;
			}
			this.extendedData = true;
			if (this.ShouldUseFastArray && this.filled < 35)
			{
				this.ScheduleItems();
			}
			return flag;
		}

		private void RefillActivationQuotasIfNeccessary()
		{
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				if (this.subQueues[i].LockedCount > 0 && this.remainingActivationPriorityQuotas[i] > 0)
				{
					return;
				}
			}
			if (this.behaviour == PriorityBehaviour.QueuePriority)
			{
				for (int j = 0; j < this.deliveryPriorityQuotas.Length; j++)
				{
					this.remainingActivationPriorityQuotas[j] = this.deliveryPriorityQuotas[j];
				}
				return;
			}
			this.remainingActivationPriorityQuotas[0] = 40;
			this.remainingActivationPriorityQuotas[1] = 20;
			this.remainingActivationPriorityQuotas[2] = 4;
			this.remainingActivationPriorityQuotas[3] = 1;
		}

		private void ReactivateItem(IQueueItem item)
		{
			item.DeferUntil = DateTime.MinValue;
			if (this.ItemActivated(item))
			{
				this.Enqueue(item);
			}
		}

		private IQueueItem DequeueArrayItem(int index)
		{
			IQueueItem queueItem = this.items[index];
			this.DecrementFilled(queueItem.Priority);
			this.tailIndex = this.Rewind(this.tailIndex);
			if (index != this.tailIndex)
			{
				this.items[index] = this.items[this.tailIndex];
				this.items[this.tailIndex] = null;
			}
			else
			{
				this.items[index] = null;
			}
			return queueItem;
		}

		private QueueItemList DequeueAllInternal(Predicate<IQueueItem> match, bool checkDeferred)
		{
			QueueItemList queueItemList = new QueueItemList();
			if (this.ShouldUseFastArray)
			{
				int num = this.headIndex;
				int i = 0;
				while (i < this.filled)
				{
					if (match(this.items[num]))
					{
						queueItemList.Add(this.DequeueArrayItem(num));
					}
					else
					{
						num = this.Advance(num);
						i++;
					}
				}
			}
			for (int j = 0; j < this.subQueues.Length; j++)
			{
				QueueItemList list = this.subQueues[j].DequeueAll(match);
				queueItemList.Concat(list);
			}
			if (this.ShouldUseFastArray && this.extendedData && this.filled < 35)
			{
				this.ScheduleItems();
			}
			if (checkDeferred)
			{
				queueItemList.Concat(this.deferredQueue.DequeueAll(match));
			}
			return queueItemList;
		}

		private QueueItemList DequeueAllLocked(Predicate<IQueueItem> match)
		{
			QueueItemList queueItemList = new QueueItemList();
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				QueueItemList list = this.subQueues[i].DequeueAllLocked(match);
				queueItemList.Concat(list);
			}
			return queueItemList;
		}

		private long ShuffleFrom(DeliveryPriority index)
		{
			long oldestItem = this.subQueues[(int)index].OldestItem;
			IQueueItem queueItem = this.subQueues[(int)index].Dequeue();
			this.items[this.tailIndex] = queueItem;
			this.tailIndex = this.Advance(this.tailIndex);
			this.IncrementFilled(queueItem.Priority);
			return oldestItem;
		}

		private void ScheduleItems()
		{
			int i = 0;
			int j = 40;
			int num = this.subQueues[0].ActiveCount;
			if (j > num)
			{
				i = j - num;
				j = num;
			}
			while (j > 0)
			{
				this.ShuffleFrom(DeliveryPriority.High);
				j--;
			}
			j = 20;
			int num2 = 0;
			while (i > 20)
			{
				num2++;
				j += 20;
				i -= 21;
			}
			j += i;
			i = num2;
			num = this.subQueues[1].ActiveCount;
			if (j > num)
			{
				i += j - num;
				j = num;
			}
			long num3 = DateTime.UtcNow.Ticks;
			while (j > 0)
			{
				num3 = this.ShuffleFrom(DeliveryPriority.Normal);
				j--;
			}
			j = 5 + i;
			long num4 = this.subQueues[2].OldestItem;
			num = this.subQueues[2].ActiveCount + this.subQueues[3].ActiveCount;
			while (j > 0 && num > 0)
			{
				if ((num3 <= this.subQueues[2].OldestItem || num3 <= this.subQueues[3].OldestItem) && 0 < this.subQueues[1].ActiveCount)
				{
					num3 = this.ShuffleFrom(DeliveryPriority.Normal);
				}
				else if (num4 <= this.subQueues[3].OldestItem && 0 < this.subQueues[2].ActiveCount)
				{
					num4 = this.ShuffleFrom(DeliveryPriority.Low);
					num--;
				}
				else
				{
					this.ShuffleFrom(DeliveryPriority.None);
					num--;
				}
				j--;
			}
			for (int k = 1; k >= 0; k--)
			{
				while (j > 0 && this.subQueues[k].ActiveCount > 0)
				{
					this.ShuffleFrom((DeliveryPriority)k);
					j--;
				}
			}
			if (j > 0)
			{
				this.extendedData = false;
			}
		}

		private int Rewind(int index)
		{
			if (--index < 0)
			{
				return this.items.Length - 1;
			}
			return index;
		}

		private void InternalForEach(Action<IQueueItem> action)
		{
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				this.subQueues[i].ForEach(action);
			}
			if (this.ShouldUseFastArray)
			{
				int num = this.headIndex;
				for (int j = 0; j < this.filled; j++)
				{
					MessageQueue.RunUnderPoisonContext(this.items[num], action);
					num = this.Advance(num);
				}
			}
		}

		private void InternalForEach<T>(Action<IQueueItem, T> action, T state)
		{
			for (int i = 0; i < this.subQueues.Length; i++)
			{
				this.subQueues[i].ForEach<T>(action, state);
			}
			if (this.behaviour != PriorityBehaviour.Fixed)
			{
				int num = this.headIndex;
				for (int j = 0; j < this.filled; j++)
				{
					MessageQueue.RunUnderPoisonContext<T>(this.items[num], state, action);
					num = this.Advance(num);
				}
			}
		}

		private DeliveryPriority GetItemPriority(IQueueItem item)
		{
			if (this.behaviour == PriorityBehaviour.IgnorePriority)
			{
				return DeliveryPriority.Normal;
			}
			return item.Priority;
		}

		private const int ItemLength = 100;

		private const int NoneRatio = 1;

		private const int LowRatio = 4;

		private const int PriorityRatioValue = 10;

		private const int HighRatio = 40;

		private const int NormalRatio = 20;

		private const int LowThreshold = 35;

		protected FifoQueue[] subQueues;

		private static readonly TimeSpan MinExpiryCheckInterval = BackgroundProcessingThread.SlowScanInterval;

		private static readonly TimeSpan MaxExpiryCheckInterval = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan expiryCheckPeriodForFixedPriorityBehaviour;

		private static TimeSpan minExpiryCheckPeriod = TimeSpan.FromSeconds(1.0);

		private static TimeSpan expiryCheckPeriodForOtherPriorityBehaviours = TimeSpan.FromMinutes(2.0);

		private readonly PriorityBehaviour behaviour;

		private readonly int numberOfDeliveryPriorities = QueueManager.PriorityToInstanceIndexMap.Count;

		private TimeSpan? expiryCheckPeriod = null;

		private IQueueItem[] items = new IQueueItem[100];

		private int headIndex;

		private int tailIndex;

		private int filled;

		private int[] filledCountsPerPriority;

		private DeferredQueue deferredQueue = new DeferredQueue();

		private bool extendedData;

		private long lastDequeue = DateTime.UtcNow.Ticks;

		private DateTime lastExpiryCheck = DateTime.MinValue;

		private DateTime lastLockExpiryCheck = DateTime.MinValue;

		private bool checkForExpiriesOnNextTimedUpdate;

		private int[] remainingDeliveryPriorityQuotas;

		private int[] remainingActivationPriorityQuotas;

		private int[] deliveryPriorityQuotas;
	}
}
