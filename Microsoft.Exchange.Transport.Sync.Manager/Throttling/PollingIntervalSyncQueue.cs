using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PollingIntervalSyncQueue<T> : SyncQueue<T>
	{
		public PollingIntervalSyncQueue(int capacity, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentLessThanZero("capacity", capacity);
			this.syncLogSession = syncLogSession;
			this.pollingSyncQueuesPriorityList = new SortedList<SyncQueueEntry<WorkType>, WorkType>(2, new PollingIntervalSyncQueue<T>.SyncQueueItemComparer());
			this.pollingSyncQueues = new Dictionary<WorkType, SortedQueue<SyncQueueEntry<T>>>(2);
			this.capacity = capacity;
			this.count = 0;
		}

		public override int Count
		{
			get
			{
				return this.count;
			}
		}

		protected SyncLogSession SyncLogSession
		{
			get
			{
				return this.syncLogSession;
			}
		}

		protected SortedList<SyncQueueEntry<WorkType>, WorkType> PollingSyncQueuesPriorityList
		{
			get
			{
				return this.pollingSyncQueuesPriorityList;
			}
		}

		public override void Clear()
		{
			this.pollingSyncQueuesPriorityList.Clear();
			this.pollingSyncQueues.Clear();
			this.count = 0;
		}

		public override void EnqueueAtFront(T item, WorkType workType)
		{
			SortedQueue<SyncQueueEntry<T>> sortedQueueFromWorkType = this.GetSortedQueueFromWorkType(workType);
			ExDateTime exDateTime = ExDateTime.UtcNow;
			if (!sortedQueueFromWorkType.IsEmpty())
			{
				ExDateTime nextPollingTime = sortedQueueFromWorkType.Peek().NextPollingTime;
				if (nextPollingTime <= exDateTime)
				{
					exDateTime = nextPollingTime.AddMilliseconds(-1.0);
				}
			}
			this.Enqueue(item, workType, exDateTime);
		}

		public override void Enqueue(T item, WorkType workType, ExDateTime nextPollingTime)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			SortedQueue<SyncQueueEntry<T>> sortedQueueFromWorkType = this.GetSortedQueueFromWorkType(workType);
			SyncQueueEntry<T> item2 = new SyncQueueEntry<T>(item, nextPollingTime);
			if (sortedQueueFromWorkType.Count == 0)
			{
				sortedQueueFromWorkType.Enqueue(item2);
				this.pollingSyncQueuesPriorityList.Add(new SyncQueueEntry<WorkType>(workType, nextPollingTime), workType);
			}
			else
			{
				SyncQueueEntry<T> syncQueueEntry = sortedQueueFromWorkType.Peek();
				sortedQueueFromWorkType.Enqueue(item2);
				if (syncQueueEntry.CompareTo(sortedQueueFromWorkType.Peek()) != 0)
				{
					this.RebuildPriorityQueue();
				}
			}
			this.count++;
		}

		public override T Dequeue(WorkType workType)
		{
			base.ThrowIfQueueEmpty();
			return this.DequeueFromWorkQueue(workType);
		}

		public override T Peek(ExDateTime currentTime)
		{
			base.ThrowIfQueueEmpty();
			return this.InternalPeek(currentTime).Item;
		}

		public override bool IsEmpty()
		{
			return this.count == 0;
		}

		public override ExDateTime NextPollingTime(ExDateTime currentTime)
		{
			base.ThrowIfQueueEmpty();
			return this.InternalPeek(currentTime).NextPollingTime;
		}

		public IList<WorkType> GetDueWorkTypesByNextPollingTime(ExDateTime currentTime)
		{
			List<WorkType> list = new List<WorkType>(this.PollingSyncQueuesPriorityList.Count);
			for (int i = 0; i < this.pollingSyncQueuesPriorityList.Count; i++)
			{
				WorkType item = this.pollingSyncQueuesPriorityList.Keys[i].Item;
				if (this.IsWorkDue(currentTime, item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public bool IsWorkDue(ExDateTime currentTime, WorkType workType)
		{
			if (!this.pollingSyncQueues.ContainsKey(workType) || this.pollingSyncQueues[workType].IsEmpty())
			{
				return false;
			}
			ExDateTime nextPollingTime = this.pollingSyncQueues[workType].Peek().NextPollingTime;
			if (nextPollingTime <= currentTime)
			{
				return true;
			}
			this.syncLogSession.LogDebugging((TSLID)748UL, "NextPollingTime {0}, CurrentTime {1}.", new object[]
			{
				nextPollingTime,
				currentTime
			});
			return false;
		}

		internal void AddDatabaseDiagnosticInfoTo(XElement parentElement, SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("DatabasePollingQueues");
			foreach (WorkType workType in this.pollingSyncQueues.Keys)
			{
				XElement xelement2 = new XElement("PollingQueue");
				SortedQueue<SyncQueueEntry<T>> sortedQueue = this.pollingSyncQueues[workType];
				ExDateTime? exDateTime = null;
				if (!sortedQueue.IsEmpty())
				{
					exDateTime = new ExDateTime?(sortedQueue.Peek().NextPollingTime);
				}
				xelement2.Add(new XElement("nextPollingTime", (exDateTime != null) ? exDateTime.Value.ToString("o") : string.Empty));
				WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
				workTypeDefinition.AddDiagnosticInfoTo(xelement2);
				xelement.Add(xelement2);
				if (mode == SyncDiagnosticMode.Info)
				{
					this.AddAdditionalWorkTypeDiagnosticInfoTo(xelement2, workType, workTypeDefinition);
				}
				if (mode == SyncDiagnosticMode.Verbose)
				{
					XElement xelement3 = new XElement("SubscriptionPriorityList");
					foreach (SyncQueueEntry<WorkType> syncQueueEntry in this.pollingSyncQueuesPriorityList.Keys)
					{
						xelement3.Add(new XElement("workType", syncQueueEntry.Item.ToString()));
					}
					xelement2.Add(xelement3);
				}
			}
			parentElement.Add(xelement);
		}

		internal void AddSubscriptionDiagnosticInfoTo(XElement parentElement, SyncDiagnosticMode mode)
		{
			if (mode != SyncDiagnosticMode.Verbose)
			{
				return;
			}
			XElement xelement = new XElement("SubscriptionInstancesInQueue");
			foreach (WorkType workType in this.pollingSyncQueues.Keys)
			{
				SortedQueue<SyncQueueEntry<T>> sortedQueue = this.pollingSyncQueues[workType];
				foreach (SyncQueueEntry<T> syncQueueEntry in sortedQueue)
				{
					XElement xelement2 = new XElement("SubscriptionInstance");
					syncQueueEntry.AddDiagnosticInfoTo(xelement2, "subscriptionId");
					xelement2.Add(new XElement("workType", workType.ToString()));
					xelement.Add(xelement2);
				}
				parentElement.Add(xelement);
			}
		}

		protected virtual void AddAdditionalWorkTypeDiagnosticInfoTo(XElement parentElement, WorkType workType, WorkTypeDefinition workTypeDefinition)
		{
			int num = 0;
			ExDateTime utcNow = ExDateTime.UtcNow;
			SortedQueue<SyncQueueEntry<T>> sortedQueue = this.pollingSyncQueues[workType];
			foreach (SyncQueueEntry<T> syncQueueEntry in sortedQueue)
			{
				ExDateTime nextPollingTime = syncQueueEntry.NextPollingTime;
				if (ExDateTime.Compare(utcNow, nextPollingTime, workTypeDefinition.TimeTillSyncDue) == 0)
				{
					break;
				}
				num++;
			}
			parentElement.Add(new XElement("itemsOutOfSla", num));
			parentElement.Add(new XElement("count", sortedQueue.Count));
			int num2 = 0;
			if (num != 0)
			{
				num2 = num / sortedQueue.Count * 100;
			}
			parentElement.Add(new XElement("itemsOutOfSlaPercent", num2));
		}

		protected T DequeueFromWorkQueue(WorkType workType)
		{
			this.pollingSyncQueuesPriorityList.RemoveAt(this.pollingSyncQueuesPriorityList.IndexOfValue(workType));
			SortedQueue<SyncQueueEntry<T>> sortedQueue = this.pollingSyncQueues[workType];
			SyncQueueEntry<T> syncQueueEntry = sortedQueue.Dequeue();
			if (sortedQueue.Count > 0)
			{
				this.pollingSyncQueuesPriorityList.Add(new SyncQueueEntry<WorkType>(workType, sortedQueue.Peek().NextPollingTime), workType);
			}
			this.count--;
			return syncQueueEntry.Item;
		}

		protected virtual SortedQueue<SyncQueueEntry<T>> GetSortedQueueFromWorkType(WorkType workType)
		{
			SortedQueue<SyncQueueEntry<T>> sortedQueue;
			if (!this.pollingSyncQueues.TryGetValue(workType, out sortedQueue))
			{
				sortedQueue = new SortedQueue<SyncQueueEntry<T>>(this.capacity);
				this.pollingSyncQueues.Add(workType, sortedQueue);
			}
			return sortedQueue;
		}

		protected virtual SyncQueueEntry<T> InternalPeek(ExDateTime currentTime)
		{
			return this.pollingSyncQueues[this.pollingSyncQueuesPriorityList.Keys[0].Item].Peek();
		}

		private void RebuildPriorityQueue()
		{
			this.pollingSyncQueuesPriorityList.Clear();
			foreach (WorkType workType in this.pollingSyncQueues.Keys)
			{
				SortedQueue<SyncQueueEntry<T>> sortedQueue = this.pollingSyncQueues[workType];
				if (sortedQueue.Count > 0)
				{
					this.pollingSyncQueuesPriorityList.Add(new SyncQueueEntry<WorkType>(workType, sortedQueue.Peek().NextPollingTime), workType);
				}
			}
		}

		private const int DefaultPollingIntervalCount = 2;

		private readonly int capacity;

		private readonly SortedList<SyncQueueEntry<WorkType>, WorkType> pollingSyncQueuesPriorityList;

		private readonly Dictionary<WorkType, SortedQueue<SyncQueueEntry<T>>> pollingSyncQueues;

		private int count;

		private SyncLogSession syncLogSession;

		private class SyncQueueItemComparer : IComparer<SyncQueueEntry<WorkType>>
		{
			public int Compare(SyncQueueEntry<WorkType> syncQueueEntryOne, SyncQueueEntry<WorkType> syncQueueEntryTwo)
			{
				SyncUtilities.ThrowIfArgumentNull("syncQueueEntryOne", syncQueueEntryOne);
				SyncUtilities.ThrowIfArgumentNull("syncQueueEntryTwo", syncQueueEntryTwo);
				if (syncQueueEntryOne.Item == syncQueueEntryTwo.Item)
				{
					return 0;
				}
				int num = syncQueueEntryOne.CompareTo(syncQueueEntryTwo);
				if (num == 0)
				{
					num = -1;
				}
				return num;
			}
		}
	}
}
