using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventControllerPrivate : EventController
	{
		public EventControllerPrivate(DatabaseInfo databaseInfo, EventBasedAssistantCollection assistants, PoisonEventControl poisonControl, PerformanceCountersPerDatabaseInstance performanceCounters, ThrottleGovernor serverGovernor) : base(databaseInfo, assistants, poisonControl, performanceCounters, serverGovernor, MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxMoveSucceeded)
		{
		}

		internal List<MailboxDispatcher> MailboxDispatchers
		{
			get
			{
				List<MailboxDispatcher> result = new List<MailboxDispatcher>(this.dispatchers.Count);
				lock (this.dispatchers)
				{
					result = this.dispatchers.Values.ToList<MailboxDispatcher>();
				}
				return result;
			}
		}

		public void RequestRecovery(EventDispatcherPrivate dispatcher)
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Adding dispatcher to recovery queue: {1}", this, dispatcher);
			lock (this.recoveryQueue)
			{
				this.recoveryQueue.Enqueue(dispatcher);
			}
		}

		public void DeadDispatcher(MailboxDispatcher dispatcher)
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, MailboxDispatcher>((long)this.GetHashCode(), "{0}: Adding dispatcher to dead queue: {1}", this, dispatcher);
			lock (this.deadQueue)
			{
				this.deadQueue.Enqueue(dispatcher);
			}
			lock (this.upToDateDispatchers)
			{
				this.upToDateDispatchers.Remove(dispatcher.MailboxGuid);
			}
		}

		public long GetLastEventCounter()
		{
			MapiEvent mapiEvent = base.EventAccess.ReadLastEvent();
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, MapiEvent>((long)this.GetHashCode(), "{0}: last event on database: {1}", this, mapiEvent);
			return mapiEvent.EventCounter;
		}

		public IList<MailboxDispatcher> GetMailboxDispatcher(Guid? mailboxGuid)
		{
			List<MailboxDispatcher> list = new List<MailboxDispatcher>();
			lock (this.dispatchers)
			{
				foreach (KeyValuePair<Guid, MailboxDispatcher> keyValuePair in this.dispatchers)
				{
					if (mailboxGuid == null || keyValuePair.Key == mailboxGuid.Value)
					{
						list.Add(keyValuePair.Value);
					}
				}
			}
			return list;
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableEventController queryableEventController = queryableObject as QueryableEventController;
			if (queryableEventController != null)
			{
				queryableEventController.TimeToUpdateIdleWatermarks = this.timeToUpdateIdleWatermarks;
				lock (this.dispatchers)
				{
					queryableEventController.ActiveMailboxes = new MultiValuedProperty<Guid>(false, QueryableEventControllerObjectSchema.ActiveMailboxes, (from x in this.dispatchers
					select x.Key).ToArray<Guid>());
				}
				lock (this.upToDateDispatchers)
				{
					queryableEventController.UpToDateMailboxes = new MultiValuedProperty<Guid>(false, QueryableEventControllerObjectSchema.UpToDateMailboxes, this.upToDateDispatchers.ToArray<Guid>());
				}
				lock (this.recoveryQueue)
				{
					queryableEventController.RecoveryEventDispatcheres = new MultiValuedProperty<Guid>(false, QueryableEventControllerObjectSchema.RecoveryEventDispatcheres, this.recoveryQueue.ToArray());
				}
				lock (this.deadQueue)
				{
					queryableEventController.DeadMailboxes = new MultiValuedProperty<Guid>(false, QueryableEventControllerObjectSchema.DeadMailboxes, this.deadQueue.ToArray());
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.dispatchers)
				{
					foreach (MailboxDispatcher mailboxDispatcher in this.dispatchers.Values)
					{
						mailboxDispatcher.Dispose();
					}
					this.dispatchers.Clear();
				}
				this.UpdateNumberOfDispatchers();
			}
			base.Dispose(disposing);
		}

		protected override void InitializeEventDispatchers(Btree<Guid, Bookmark> allBookmarks)
		{
			this.initialBookmarks = allBookmarks;
			this.UpdateNumberOfDispatchers();
		}

		protected override void WaitUntilStoppedInternal()
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate>((long)this.GetHashCode(), "{0}: Waiting for dispatchers to stop...", this);
			foreach (MailboxDispatcher mailboxDispatcher in this.MailboxDispatchers)
			{
				mailboxDispatcher.WaitForShutdown();
			}
			base.TracePfd("PFD AIS {0} {1}: All dispatchers have stopped", new object[]
			{
				31831,
				this
			});
		}

		protected override void ProcessPolledEvent(MapiEvent mapiEvent)
		{
			if (mapiEvent.MailboxGuid == Guid.Empty)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: Ignoring event {1} with empty mailbox Guid", this, mapiEvent.EventCounter);
				return;
			}
			if (base.DatabaseInfo.IsSystemAttendantMailbox(mapiEvent.MailboxGuid))
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: Ignoring event {1} for SA mailbox", this, mapiEvent.EventCounter);
				return;
			}
			base.TracePfd("PFD AIS {0} {1}: Processing event {2}", new object[]
			{
				18519,
				this,
				mapiEvent.EventCounter
			});
			MailboxDispatcher dispatcherForMapiEvent = this.GetDispatcherForMapiEvent(mapiEvent);
			dispatcherForMapiEvent.ProcessPolledEvent(mapiEvent);
		}

		protected override void DisposeOfIdleDispatchers()
		{
			List<MailboxDispatcher> list = new List<MailboxDispatcher>(this.dispatchers.Count);
			foreach (MailboxDispatcher mailboxDispatcher in this.MailboxDispatchers)
			{
				if (mailboxDispatcher.IsIdle)
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, MailboxDispatcher>((long)this.GetHashCode(), "{0}: Moving dispatcher to the idle list: {1}.", this, mailboxDispatcher);
					list.Add(mailboxDispatcher);
					lock (this.upToDateDispatchers)
					{
						this.upToDateDispatchers.Add(mailboxDispatcher.MailboxGuid);
					}
				}
			}
			if (list.Count > 0)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int>((long)this.GetHashCode(), "{0}: Disposing of {1} idle dispatchers.", this, list.Count);
				foreach (MailboxDispatcher mailboxDispatcher2 in list)
				{
					this.dispatchers.Remove(mailboxDispatcher2.MailboxGuid);
					mailboxDispatcher2.RequestShutdown();
				}
				foreach (MailboxDispatcher mailboxDispatcher3 in list)
				{
					mailboxDispatcher3.WaitForShutdown();
					mailboxDispatcher3.Dispose();
				}
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int>((long)this.GetHashCode(), "{0}: {1} active dispatchers in memory.", this, this.dispatchers.Count);
				this.UpdateNumberOfDispatchers();
			}
			if (this.timeToUpdateIdleWatermarks < DateTime.UtcNow)
			{
				if (this.initialBookmarks != null)
				{
					foreach (Bookmark bookmark in this.initialBookmarks)
					{
						lock (this.upToDateDispatchers)
						{
							if (!this.upToDateDispatchers.Contains(bookmark.Identity))
							{
								this.upToDateDispatchers.Add(bookmark.Identity);
							}
						}
					}
					this.initialBookmarks = null;
				}
				List<Guid> list2 = new List<Guid>(this.upToDateDispatchers.Count);
				foreach (Guid guid in this.upToDateDispatchers)
				{
					if (!this.dispatchers.ContainsKey(guid))
					{
						list2.Add(guid);
					}
				}
				if (list2.Count == 0)
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate>((long)this.GetHashCode(), "{0}: There are no idle mailboxes at this time.", this);
				}
				else
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int, int>((long)this.GetHashCode(), "{0}: Time to update watermarks for idle mailboxes. There are {1} up-to-date dispatchers and {2} idle mailboxes.", this, this.upToDateDispatchers.Count, list2.Count);
					Guid[] idleMailboxes = list2.ToArray();
					foreach (AssistantCollectionEntry assistantCollectionEntry in base.Assistants)
					{
						this.UpdateIdleWatermarksForAssistant(idleMailboxes, assistantCollectionEntry.Identity);
					}
				}
				this.timeToUpdateIdleWatermarks = DateTime.UtcNow + Configuration.IdleWatermarksSaveInterval;
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, DateTime>((long)this.GetHashCode(), "{0}: Next update for idle watermarks: {1}", this, this.timeToUpdateIdleWatermarks);
			}
		}

		protected override void UpdateWatermarksForAssistant(Guid assistantId)
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate>((long)this.GetHashCode(), "{0}: BuildingWatermarkArray...", this);
			Watermark[] array = this.BuildWatermarkArray(assistantId);
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int>((long)this.GetHashCode(), "{0}: Built Watermark array.  {1} watermarks to save", this, array.Length);
			bool experiencedPartialCompletion = false;
			if (array.Length <= 0)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate>((long)this.GetHashCode(), "{0}: No watermarks to save at this time.", this);
				return;
			}
			try
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int>((long)this.GetHashCode(), "{0}: Saving {1} watermarks...", this, array.Length);
				base.EventAccess.SaveWatermarks(assistantId, array);
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, int>((long)this.GetHashCode(), "{0}: Saved {1} watermarks.", this, array.Length);
			}
			catch (MapiExceptionPartialCompletion arg)
			{
				ExTraceGlobals.EventControllerTracer.TraceError<EventControllerPrivate, int, MapiExceptionPartialCompletion>((long)this.GetHashCode(), "{0}: Tried to save {1} watermarks.\t Some mailboxes no longer exist on the MDB: {2}", this, array.Length, arg);
				experiencedPartialCompletion = true;
			}
			if (Test.NotifyOnPostWatermarkCommit != null)
			{
				Test.NotifyOnPostWatermarkCommit(array, experiencedPartialCompletion);
			}
			foreach (Watermark watermark in array)
			{
				if (watermark.MailboxGuid == Guid.Empty)
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: Committed DatabaseWatermark: {1}", this, watermark.EventCounter);
					base.DatabaseBookmark[assistantId] = watermark.EventCounter;
				}
				else
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Updating bookmark for mailbox: {1}, assistant: {2}, counter: {3}", new object[]
					{
						this,
						watermark.MailboxGuid,
						assistantId,
						watermark.EventCounter
					});
					this.dispatchers[watermark.MailboxGuid].UpdateWatermark(assistantId, watermark.EventCounter);
				}
			}
			if (Test.NotifyOnPostWatermarkCommit != null)
			{
				Test.NotifyOnPostWatermarkCommit(array, experiencedPartialCompletion);
			}
		}

		protected override void PeriodicMaintenance()
		{
			long num = this.RecoverDispatchers();
			if (num < 9223372036854775807L)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: Current recoveryEventCounter: {1}", this, num);
				base.HighestEventPolled = num - 1L;
			}
			this.DisposeDecayedDispatchers();
		}

		private MailboxDispatcher GetDispatcherForMailbox(Guid mailboxGuid, long eventCounter)
		{
			MailboxDispatcher mailboxDispatcher = null;
			bool flag2;
			lock (this.dispatchers)
			{
				flag2 = this.dispatchers.TryGetValue(mailboxGuid, out mailboxDispatcher);
			}
			if (!flag2)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, Guid>((long)this.GetHashCode(), "{0}: creating dispatcher for mailbox {1}", this, mailboxGuid);
				bool dispatcherIsUpToDate = false;
				lock (this.upToDateDispatchers)
				{
					dispatcherIsUpToDate = this.upToDateDispatchers.Contains(mailboxGuid);
				}
				Bookmark bookmark = null;
				if (this.initialBookmarks != null && this.initialBookmarks.Remove(mailboxGuid, out bookmark) && bookmark != null && bookmark.Identity != Guid.Empty)
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, Bookmark>((long)this.GetHashCode(), "{0}: Initializing mailbox dispatcher from {1}", this, bookmark);
					mailboxDispatcher = MailboxDispatcher.CreateFromBookmark(this, base.EventAccess, null, bookmark, base.DatabaseBookmark);
				}
				if (mailboxDispatcher == null)
				{
					mailboxDispatcher = MailboxDispatcher.CreateWithoutBookmark(this, base.EventAccess, mailboxGuid, base.DatabaseBookmark, dispatcherIsUpToDate);
				}
				lock (this.dispatchers)
				{
					this.dispatchers[mailboxGuid] = mailboxDispatcher;
				}
				this.UpdateNumberOfDispatchers();
			}
			return mailboxDispatcher;
		}

		private MailboxDispatcher GetDispatcherForMapiEvent(MapiEvent mapiEvent)
		{
			MailboxDispatcher dispatcherForMailbox = this.GetDispatcherForMailbox(mapiEvent.MailboxGuid, mapiEvent.EventCounter);
			if (dispatcherForMailbox.IsMailboxDead && mapiEvent.EventCounter > dispatcherForMailbox.DecayedEventCounter)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, MailboxDispatcher>((long)this.GetHashCode(), "{0}: Dispatcher has decayed.  Removing it from table: {1}", this, dispatcherForMailbox);
				lock (this.dispatchers)
				{
					this.dispatchers.Remove(dispatcherForMailbox.MailboxGuid);
				}
				this.UpdateNumberOfDispatchers();
				return this.GetDispatcherForMailbox(mapiEvent.MailboxGuid, mapiEvent.EventCounter);
			}
			return dispatcherForMailbox;
		}

		private Watermark[] BuildWatermarkArray(Guid assistantId)
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, Guid, int>((long)this.GetHashCode(), "{0}: Building Watermark Array for {1}. Dispatcher count: {2}", this, assistantId, this.dispatchers.Count);
			List<Watermark> list = new List<Watermark>(this.dispatchers.Count + 1);
			long highestEventPolled = base.HighestEventPolled;
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: HighestWatermarkPolled: {1}", this, highestEventPolled);
			foreach (MailboxDispatcher mailboxDispatcher in this.MailboxDispatchers)
			{
				EventDispatcherPrivate assistantDispatcher = mailboxDispatcher.GetAssistantDispatcher(assistantId);
				Watermark currentWatermark = assistantDispatcher.GetCurrentWatermark(ref highestEventPolled);
				if (currentWatermark != null)
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, Guid, long>((long)this.GetHashCode(), "{0}: Saving watermark: mailbox: {1}, counter: {2}", this, currentWatermark.MailboxGuid, currentWatermark.EventCounter);
					list.Add(currentWatermark);
				}
				else
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, Guid>((long)this.GetHashCode(), "{0}: Not updating watermark for mailbox: {1}", this, assistantDispatcher.MailboxGuid);
				}
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: DatabaseWatermark is {1}", this, highestEventPolled);
			if (highestEventPolled != base.DatabaseBookmark[assistantId])
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, long>((long)this.GetHashCode(), "{0}: Saving DatabaseWatermark: {1}", this, highestEventPolled);
				list.Add(Watermark.GetDatabaseWatermark(highestEventPolled));
			}
			else
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate>((long)this.GetHashCode(), "{0}: DatabaseWatermark has not changed -- not saving", this);
			}
			return list.ToArray();
		}

		private void UpdateIdleWatermarksForAssistant(Guid[] idleMailboxes, Guid assistantId)
		{
			long num = base.DatabaseBookmark[assistantId];
			Watermark[] array = new Watermark[idleMailboxes.Length];
			for (int i = 0; i < idleMailboxes.Length; i++)
			{
				array[i] = Watermark.GetMailboxWatermark(idleMailboxes[i], num);
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Updating {1} watermarks for idle mailboxes to value {2} for consumer {3}", new object[]
			{
				this,
				idleMailboxes.Length,
				num,
				assistantId
			});
			try
			{
				base.EventAccess.SaveWatermarks(assistantId, array);
			}
			catch (MapiExceptionPartialCompletion arg)
			{
				ExTraceGlobals.EventControllerTracer.TraceError<EventControllerPrivate, int, MapiExceptionPartialCompletion>((long)this.GetHashCode(), "{0}: Tried to save {1} watermarks. Some mailboxes no longer exist on this database: {2}", this, array.Length, arg);
			}
		}

		private void UpdateNumberOfDispatchers()
		{
			base.DatabaseCounters.MailboxDispatchers.RawValue = (long)this.dispatchers.Count;
		}

		private long RecoverDispatchers()
		{
			long num = long.MaxValue;
			lock (this.recoveryQueue)
			{
				while (this.recoveryQueue.Count > 0)
				{
					EventDispatcherPrivate eventDispatcherPrivate = this.recoveryQueue.Dequeue();
					num = Math.Min(num, eventDispatcherPrivate.RecoveryEventCounter);
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPrivate, EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Recovering dispatcher {1} with recovery counter {2}", this, eventDispatcherPrivate, eventDispatcherPrivate.RecoveryEventCounter);
					eventDispatcherPrivate.Recover();
				}
			}
			return num;
		}

		private void DisposeDecayedDispatchers()
		{
			bool flag = false;
			try
			{
				Monitor.Enter(this.deadQueue);
				flag = true;
				while (this.deadQueue.Count > 0)
				{
					MailboxDispatcher mailboxDispatcher = this.deadQueue.Peek();
					if (base.HighestEventPolled <= mailboxDispatcher.DecayedEventCounter)
					{
						ExTraceGlobals.EventControllerTracer.TraceDebug((long)this.GetHashCode(), "{0}: finished disposingDecayedDispatchers, highestEventPolled {1}, dispatcher.DecayedEventCounter: {2}, dispatcher: {3}", new object[]
						{
							this,
							base.HighestEventPolled,
							mailboxDispatcher.DecayedEventCounter,
							mailboxDispatcher
						});
						break;
					}
					ExTraceGlobals.EventControllerTracer.TraceDebug((long)this.GetHashCode(), "{0}: dead dispatcher has decayed, highestEventPolled {1}, dispatcher.DecayedEventCounter: {2}, dispatcher: {3}", new object[]
					{
						this,
						base.HighestEventPolled,
						mailboxDispatcher.DecayedEventCounter,
						mailboxDispatcher
					});
					this.deadQueue.Dequeue();
					Monitor.Exit(this.deadQueue);
					flag = false;
					if (this.dispatchers.ContainsKey(mailboxDispatcher.MailboxGuid) && this.dispatchers[mailboxDispatcher.MailboxGuid] == mailboxDispatcher)
					{
						this.dispatchers.Remove(mailboxDispatcher.MailboxGuid);
						this.UpdateNumberOfDispatchers();
					}
					mailboxDispatcher.WaitForShutdown();
					mailboxDispatcher.Dispose();
					Monitor.Enter(this.deadQueue);
					flag = true;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.deadQueue);
				}
			}
		}

		private const MapiEventTypeFlags MoreEvents = MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxMoveSucceeded;

		private HashSet<Guid> upToDateDispatchers = new HashSet<Guid>();

		private Dictionary<Guid, MailboxDispatcher> dispatchers = new Dictionary<Guid, MailboxDispatcher>();

		private Queue<EventDispatcherPrivate> recoveryQueue = new Queue<EventDispatcherPrivate>();

		private Queue<MailboxDispatcher> deadQueue = new Queue<MailboxDispatcher>();

		private Btree<Guid, Bookmark> initialBookmarks;

		private DateTime timeToUpdateIdleWatermarks = DateTime.UtcNow + Configuration.IdleWatermarksSaveInterval;
	}
}
