using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DispatchEntryManager : DisposeTrackableBase, IDispatchEntryManager
	{
		public DispatchEntryManager(SyncLogSession syncLogSession, ISyncHealthLog syncHealthLog, ISyncManagerConfiguration configuration)
		{
			this.syncLogSession = syncLogSession;
			this.expirationCheckFrequency = configuration.DispatchEntryExpirationCheckFrequency;
			this.timeToEntryExpiration = configuration.DispatchEntryExpirationTime;
			this.attemptingDispatch = new Dictionary<Guid, DispatchEntry>();
			this.dispatched = new Dictionary<Guid, DispatchEntry>();
			this.entriesPerDatabase = new Dictionary<Guid, int>();
			this.workTypeBudgetManager = new WorkTypeBudgetManager(this.syncLogSession, syncHealthLog, configuration);
			this.expirationTimer = new GuardedTimer(new TimerCallback(this.CleanExpiredItemsCallback), null, (int)this.expirationCheckFrequency.TotalMilliseconds, -1);
		}

		public event EventHandler<DispatchEntry> EntryExpiredEvent
		{
			add
			{
				this.InternalEntryExpiredEvent += value;
			}
			remove
			{
				this.InternalEntryExpiredEvent -= value;
			}
		}

		private event EventHandler<DispatchEntry> InternalEntryExpiredEvent;

		public bool ContainsSubscription(Guid subscriptionGuid)
		{
			this.collectionLock.EnterReadLock();
			bool result;
			try
			{
				result = (this.attemptingDispatch.ContainsKey(subscriptionGuid) || this.dispatched.ContainsKey(subscriptionGuid));
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		public int GetNumberOfEntriesForDatabase(Guid databaseGuid)
		{
			this.collectionLock.EnterReadLock();
			int result;
			try
			{
				if (!this.entriesPerDatabase.ContainsKey(databaseGuid))
				{
					result = 0;
				}
				else
				{
					result = this.entriesPerDatabase[databaseGuid];
				}
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		public bool HasBudget(WorkType workType)
		{
			return this.workTypeBudgetManager.HasBudget(workType);
		}

		public void Add(DispatchEntry dispatchEntry)
		{
			this.collectionLock.EnterWriteLock();
			try
			{
				if (!this.entriesPerDatabase.ContainsKey(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid))
				{
					this.entriesPerDatabase.Add(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, 1);
				}
				else
				{
					Dictionary<Guid, int> dictionary;
					Guid databaseGuid;
					(dictionary = this.entriesPerDatabase)[databaseGuid = dispatchEntry.MiniSubscriptionInformation.DatabaseGuid] = dictionary[databaseGuid] + 1;
				}
				this.attemptingDispatch.Add(dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid, dispatchEntry);
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
		}

		public DispatchEntry RemoveDispatchAttempt(Guid databaseGuid, Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfArgumentNull("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			this.collectionLock.EnterWriteLock();
			DispatchEntry result;
			try
			{
				if (!this.attemptingDispatch.ContainsKey(subscriptionGuid))
				{
					throw new InvalidOperationException();
				}
				Dictionary<Guid, int> dictionary;
				(dictionary = this.entriesPerDatabase)[databaseGuid] = dictionary[databaseGuid] - 1;
				DispatchEntry dispatchEntry = this.attemptingDispatch[subscriptionGuid];
				this.attemptingDispatch.Remove(subscriptionGuid);
				result = dispatchEntry;
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
			return result;
		}

		public void MarkDispatchSuccess(Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			this.collectionLock.EnterWriteLock();
			try
			{
				DispatchEntry dispatchEntry = this.attemptingDispatch[subscriptionGuid];
				this.attemptingDispatch.Remove(subscriptionGuid);
				this.dispatched.Add(subscriptionGuid, dispatchEntry);
				this.workTypeBudgetManager.Increment(dispatchEntry.WorkType);
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
		}

		public bool TryRemoveDispatchedEntry(Guid subscriptionGuid, out DispatchEntry dispatchEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			dispatchEntry = null;
			this.collectionLock.EnterWriteLock();
			bool result;
			try
			{
				if (this.dispatched.ContainsKey(subscriptionGuid))
				{
					dispatchEntry = this.RemoveDispatchedEntry(subscriptionGuid);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
			return result;
		}

		public XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("DispatchEntryManager");
			this.collectionLock.EnterReadLock();
			try
			{
				xelement.Add(new XElement("attemptingDispatchCount", this.attemptingDispatch.Count));
				xelement.Add(new XElement("dispatchedCount", this.dispatched.Count));
				XElement xelement2 = new XElement("EntriesPerDatabase");
				foreach (Guid guid in this.entriesPerDatabase.Keys)
				{
					XElement xelement3 = new XElement("Database");
					xelement3.Add(new XElement("databaseId", guid));
					xelement3.Add(new XElement("countOfEntries", this.entriesPerDatabase[guid]));
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
				xelement.Add(this.workTypeBudgetManager.GetDiagnosticInfo());
				if (mode == SyncDiagnosticMode.Verbose)
				{
					XElement xelement4 = new XElement("DispatchAttemptEntries");
					foreach (DispatchEntry dispatchEntry in this.attemptingDispatch.Values)
					{
						xelement4.Add(dispatchEntry.GetDiagnosticInfo());
					}
					xelement.Add(xelement4);
					XElement xelement5 = new XElement("DispatchedEntries");
					foreach (DispatchEntry dispatchEntry2 in this.dispatched.Values)
					{
						xelement5.Add(dispatchEntry2.GetDiagnosticInfo());
					}
					xelement.Add(xelement5);
				}
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return xelement;
		}

		public void DisabledExpiration()
		{
			this.expirationTimer.Change(-1, -1);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.expirationTimer != null)
				{
					this.expirationTimer.Dispose(true);
					this.expirationTimer = null;
				}
				if (this.workTypeBudgetManager != null)
				{
					this.workTypeBudgetManager.Dispose();
					this.workTypeBudgetManager = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DispatchEntryManager>(this);
		}

		private DispatchEntry RemoveDispatchedEntry(Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			DispatchEntry dispatchEntry = this.dispatched[subscriptionGuid];
			this.dispatched.Remove(subscriptionGuid);
			if (dispatchEntry != null)
			{
				this.workTypeBudgetManager.Decrement(dispatchEntry.WorkType);
				Dictionary<Guid, int> dictionary;
				Guid databaseGuid;
				(dictionary = this.entriesPerDatabase)[databaseGuid = dispatchEntry.MiniSubscriptionInformation.DatabaseGuid] = dictionary[databaseGuid] - 1;
			}
			return dispatchEntry;
		}

		private void CleanExpiredItemsCallback(object state)
		{
			this.syncLogSession.LogDebugging((TSLID)1294UL, "CleanExpiredItemsCallback", new object[0]);
			ExDateTime utcNow = ExDateTime.UtcNow;
			List<DispatchEntry> list = new List<DispatchEntry>();
			this.collectionLock.EnterWriteLock();
			try
			{
				foreach (DispatchEntry dispatchEntry in this.dispatched.Values)
				{
					TimeSpan t = utcNow - dispatchEntry.DispatchAttemptTime;
					if (t > this.timeToEntryExpiration)
					{
						list.Add(dispatchEntry);
					}
				}
				foreach (DispatchEntry dispatchEntry2 in list)
				{
					this.RemoveDispatchedEntry(dispatchEntry2.MiniSubscriptionInformation.SubscriptionGuid);
					if (this.InternalEntryExpiredEvent != null)
					{
						this.InternalEntryExpiredEvent(this, dispatchEntry2);
					}
				}
			}
			finally
			{
				this.expirationTimer.Change((int)this.expirationCheckFrequency.TotalMilliseconds, -1);
				this.collectionLock.ExitWriteLock();
			}
		}

		private readonly Dictionary<Guid, DispatchEntry> attemptingDispatch;

		private readonly Dictionary<Guid, DispatchEntry> dispatched;

		private readonly Dictionary<Guid, int> entriesPerDatabase;

		private WorkTypeBudgetManager workTypeBudgetManager;

		private ReaderWriterLockSlim collectionLock = new ReaderWriterLockSlim();

		private GuardedTimer expirationTimer;

		private TimeSpan expirationCheckFrequency;

		private TimeSpan timeToEntryExpiration;

		private SyncLogSession syncLogSession;
	}
}
