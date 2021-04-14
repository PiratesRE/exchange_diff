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
	internal class DatabaseQueueManager
	{
		public DatabaseQueueManager(Guid databaseGuid, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
			this.databaseGuid = databaseGuid;
			this.pollingIntervalSyncQueue = new PollingIntervalSyncQueue<Guid>(1000, this.syncLogSession);
			this.subscriptionList = new Dictionary<Guid, DatabaseQueueManager.SubscriptionQueueEntry>(15000);
			this.count = 0;
		}

		public event EventHandler<SyncQueueEventArgs> SubscriptionAddedOrRemovedEvent
		{
			add
			{
				lock (this.syncRoot)
				{
					this.InternalSubscriptionAddedOrRemovedEvent += value;
				}
			}
			remove
			{
				lock (this.syncRoot)
				{
					this.InternalSubscriptionAddedOrRemovedEvent -= value;
				}
			}
		}

		public event EventHandler<SyncQueueEventArgs> SubscriptionEnqueuedOrDequeuedEvent
		{
			add
			{
				lock (this.syncRoot)
				{
					this.InternalSubscriptionEnqueuedOrDequeuedEvent += value;
				}
			}
			remove
			{
				lock (this.syncRoot)
				{
					this.InternalSubscriptionEnqueuedOrDequeuedEvent -= value;
				}
			}
		}

		private event EventHandler<SyncQueueEventArgs> InternalSubscriptionAddedOrRemovedEvent;

		private event EventHandler<SyncQueueEventArgs> InternalSubscriptionEnqueuedOrDequeuedEvent;

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public int SubscriptionCount
		{
			get
			{
				return this.subscriptionList.Count;
			}
		}

		public bool IsEmpty()
		{
			return this.pollingIntervalSyncQueue.IsEmpty();
		}

		public void Clear()
		{
			lock (this.syncRoot)
			{
				this.pollingIntervalSyncQueue.Clear();
				this.subscriptionList.Clear();
				this.count = 0;
			}
		}

		public bool Add(ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid);
			bool result;
			lock (this.syncRoot)
			{
				DatabaseQueueManager.SubscriptionQueueEntry value = new DatabaseQueueManager.SubscriptionQueueEntry(new MiniSubscriptionInformation(subscriptionInformation.ExternalDirectoryOrgId, subscriptionInformation.DatabaseGuid, subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid, subscriptionInformation.SubscriptionType, ExDateTime.UtcNow));
				DatabaseQueueManager.SubscriptionQueueEntry subscriptionQueueEntry;
				if (!this.subscriptionList.TryGetValue(subscriptionInformation.SubscriptionGuid, out subscriptionQueueEntry))
				{
					this.subscriptionList.Add(subscriptionInformation.SubscriptionGuid, value);
					this.RaiseSubscriptionAddedEvent();
					syncLogSession.LogDebugging((TSLID)461UL, "DQM.Add added subscription", new object[0]);
					result = true;
				}
				else
				{
					syncLogSession.LogDebugging((TSLID)463UL, "DQM.Add subscription already added, not adding now", new object[0]);
					result = false;
				}
			}
			return result;
		}

		public void Remove(Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			lock (this.syncRoot)
			{
				DatabaseQueueManager.SubscriptionQueueEntry subscriptionQueueEntry = null;
				if (this.subscriptionList.TryGetValue(subscriptionGuid, out subscriptionQueueEntry))
				{
					this.subscriptionList.Remove(subscriptionGuid);
					this.RaiseSubscriptionRemovedEvent();
					SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionGuid);
					syncLogSession.LogDebugging((TSLID)484UL, "DQM.Remove removed subscription", new object[0]);
				}
			}
		}

		public bool TryFindSubscription(Guid subscriptionGuid, out HashSet<WorkType> queuedWorkTypes)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			queuedWorkTypes = null;
			lock (this.syncRoot)
			{
				DatabaseQueueManager.SubscriptionQueueEntry subscriptionQueueEntry = null;
				if (this.subscriptionList.TryGetValue(subscriptionGuid, out subscriptionQueueEntry))
				{
					queuedWorkTypes = subscriptionQueueEntry.GetCopyOfQueuedWorkTypes();
					return true;
				}
			}
			return false;
		}

		public bool EnqueueAtFront(WorkType workType, Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionGuid);
			syncLogSession.LogDebugging((TSLID)493UL, "DQM.EnqueueAtFront", new object[0]);
			return this.InternalEnqueue(workType, subscriptionGuid, null, true);
		}

		public bool Enqueue(WorkType workType, Guid subscriptionGuid, ExDateTime nextDispatchTime)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionGuid);
			syncLogSession.LogDebugging((TSLID)494UL, "DQM.Enqueue in DB {0} WorkType {1} with nextDispatchTime {2}", new object[]
			{
				this.databaseGuid,
				workType,
				nextDispatchTime
			});
			return this.InternalEnqueue(workType, subscriptionGuid, new ExDateTime?(nextDispatchTime), false);
		}

		public ExDateTime? NextPollingTime(ExDateTime currentTime)
		{
			ExDateTime? result;
			lock (this.syncRoot)
			{
				if (!this.pollingIntervalSyncQueue.IsEmpty())
				{
					result = new ExDateTime?(this.pollingIntervalSyncQueue.NextPollingTime(currentTime));
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public IList<WorkType> GetDueWorkTypesByNextPollingTime(ExDateTime currentTime)
		{
			IList<WorkType> result;
			lock (this.syncRoot)
			{
				if (!this.pollingIntervalSyncQueue.IsEmpty())
				{
					result = this.pollingIntervalSyncQueue.GetDueWorkTypesByNextPollingTime(currentTime);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public bool TryDequeue(ExDateTime currentTime, WorkType workType, out MiniSubscriptionInformation miniSubscriptionInformation, out ExDateTime dueTimeOfNextSubscription)
		{
			miniSubscriptionInformation = null;
			dueTimeOfNextSubscription = ExDateTime.UtcNow;
			bool result;
			lock (this.syncRoot)
			{
				if (this.pollingIntervalSyncQueue.IsWorkDue(currentTime, workType))
				{
					this.syncLogSession.LogDebugging((TSLID)495UL, "DQM.TryDequeue Dequeuing from polling interval queue", new object[0]);
					dueTimeOfNextSubscription = this.pollingIntervalSyncQueue.NextPollingTime(currentTime);
					Guid guid = this.pollingIntervalSyncQueue.Dequeue(workType);
					TimeSpan timeSpan = currentTime - dueTimeOfNextSubscription;
					SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(guid);
					syncLogSession.LogDebugging((TSLID)1285UL, "DQM.TryDequeue Time Since Due:{0}", new object[]
					{
						timeSpan
					});
					this.count--;
					WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
					this.RaiseSubscriptionDequeuedEvent(workTypeDefinition.TimeTillSyncDue);
					DatabaseQueueManager.SubscriptionQueueEntry subscriptionQueueEntry = null;
					if (this.subscriptionList.TryGetValue(guid, out subscriptionQueueEntry))
					{
						subscriptionQueueEntry.UnMarkWorkTypeQueued(workType);
						miniSubscriptionInformation = subscriptionQueueEntry.MiniSubscriptionInformation;
						result = true;
					}
					else
					{
						syncLogSession.LogDebugging((TSLID)1286UL, "DQM.TryDequeue Subscription no longer in DQM, failing dequeue", new object[0]);
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("DatabaseQueueManager");
			lock (this.syncRoot)
			{
				xelement.Add(new XElement("databaseId", this.databaseGuid));
				xelement.Add(new XElement("nextPollingTime", (!this.pollingIntervalSyncQueue.IsEmpty()) ? this.pollingIntervalSyncQueue.NextPollingTime(ExDateTime.UtcNow).ToString("o") : string.Empty));
				XElement xelement2 = new XElement("Counts");
				xelement2.Add(new XElement("subscriptionList", this.subscriptionList.Count));
				xelement2.Add(new XElement("subscriptionInstancesInQueues", this.Count));
				xelement.Add(xelement2);
				this.pollingIntervalSyncQueue.AddDatabaseDiagnosticInfoTo(xelement, mode);
				this.pollingIntervalSyncQueue.AddSubscriptionDiagnosticInfoTo(xelement, mode);
			}
			return xelement;
		}

		private bool InternalEnqueue(WorkType workType, Guid subscriptionGuid, ExDateTime? nextDispatchTime, bool enqueueAtFront)
		{
			if ((!enqueueAtFront && nextDispatchTime == null) || (enqueueAtFront && nextDispatchTime != null))
			{
				throw new ArgumentException("If we aren't enqueueing at the front, we must have a next dispatch time");
			}
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionGuid);
			lock (this.syncRoot)
			{
				DatabaseQueueManager.SubscriptionQueueEntry subscriptionQueueEntry;
				if (!this.subscriptionList.TryGetValue(subscriptionGuid, out subscriptionQueueEntry))
				{
					syncLogSession.LogDebugging((TSLID)430UL, "DQM.Enqueue: Subscription is not managed by DQM, will not be enqueued.", new object[0]);
					return false;
				}
				WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
				if (!subscriptionQueueEntry.MarkWorkTypeQueued(workType))
				{
					syncLogSession.LogDebugging((TSLID)1292UL, "DQM.InternalEnqueue: {0} already enqueued.", new object[]
					{
						workType
					});
					return false;
				}
				if (enqueueAtFront)
				{
					this.pollingIntervalSyncQueue.EnqueueAtFront(subscriptionGuid, workType);
				}
				else
				{
					this.pollingIntervalSyncQueue.Enqueue(subscriptionGuid, workType, nextDispatchTime.Value);
				}
				this.count++;
				this.RaiseSubscriptionEnqueuedEvent(workTypeDefinition.TimeTillSyncDue);
			}
			return true;
		}

		private void ThrowIfQueueEmpty()
		{
			if (this.IsEmpty())
			{
				throw new InvalidOperationException("Database Queue Manager is empty.");
			}
		}

		private void RaiseSubscriptionAddedEvent()
		{
			if (this.InternalSubscriptionAddedOrRemovedEvent != null)
			{
				this.InternalSubscriptionAddedOrRemovedEvent(this, SyncQueueEventArgs.CreateSubscriptionAddedEventArgs(this.databaseGuid));
			}
		}

		private void RaiseSubscriptionRemovedEvent()
		{
			if (this.InternalSubscriptionAddedOrRemovedEvent != null)
			{
				this.InternalSubscriptionAddedOrRemovedEvent(this, SyncQueueEventArgs.CreateSubscriptionRemovedEventArgs(this.databaseGuid));
			}
		}

		private void RaiseSubscriptionEnqueuedEvent(TimeSpan syncInterval)
		{
			if (this.InternalSubscriptionEnqueuedOrDequeuedEvent != null)
			{
				this.InternalSubscriptionEnqueuedOrDequeuedEvent(this, SyncQueueEventArgs.CreateSubscriptionEnqueuedEventArgs(this.databaseGuid, syncInterval));
			}
		}

		private void RaiseSubscriptionDequeuedEvent(TimeSpan syncInterval)
		{
			if (this.InternalSubscriptionEnqueuedOrDequeuedEvent != null)
			{
				this.InternalSubscriptionEnqueuedOrDequeuedEvent(this, SyncQueueEventArgs.CreateSubscriptionDequeuedEventArgs(this.databaseGuid, syncInterval));
			}
		}

		private const int DefaultNumberOfSubscriptionsPerMailboxServer = 15000;

		private const int DefaultNumberOfSyncsScheduledPerDatabase = 1000;

		private readonly object syncRoot = new object();

		private readonly Guid databaseGuid;

		private readonly PollingIntervalSyncQueue<Guid> pollingIntervalSyncQueue;

		private readonly Dictionary<Guid, DatabaseQueueManager.SubscriptionQueueEntry> subscriptionList;

		private volatile int count;

		private SyncLogSession syncLogSession;

		private sealed class SubscriptionQueueEntry
		{
			internal SubscriptionQueueEntry(MiniSubscriptionInformation miniSubscriptionInformation)
			{
				SyncUtilities.ThrowIfArgumentNull("miniSubscriptionInformation", miniSubscriptionInformation);
				this.miniSubscriptionInformation = miniSubscriptionInformation;
				this.queuedWorkTypes = new HashSet<WorkType>();
			}

			internal MiniSubscriptionInformation MiniSubscriptionInformation
			{
				get
				{
					return this.miniSubscriptionInformation;
				}
			}

			internal HashSet<WorkType> GetCopyOfQueuedWorkTypes()
			{
				WorkType[] array = new WorkType[this.queuedWorkTypes.Count];
				this.queuedWorkTypes.CopyTo(array);
				return new HashSet<WorkType>(array);
			}

			internal bool MarkWorkTypeQueued(WorkType workType)
			{
				if (this.queuedWorkTypes.Contains(workType))
				{
					return false;
				}
				this.queuedWorkTypes.Add(workType);
				return true;
			}

			internal void UnMarkWorkTypeQueued(WorkType workType)
			{
				this.queuedWorkTypes.Remove(workType);
			}

			private readonly MiniSubscriptionInformation miniSubscriptionInformation;

			private HashSet<WorkType> queuedWorkTypes;
		}
	}
}
