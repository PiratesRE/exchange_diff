using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventDispatcherPrivate : EventDispatcher
	{
		internal static void SetTestHookForAfterEventQueueCountDecrementedForRetry(Action testhook)
		{
			EventDispatcherPrivate.syncWithTestCodeAfterEventQueueCountDecrementedForRetry = testhook;
		}

		public EventDispatcherPrivate(MailboxDispatcher parentMailboxDispatcher, AssistantCollectionEntry assistant, EventControllerPrivate controller, long watermark) : base(assistant, new MailboxGovernor(controller.Governor, new Throttle("EventDispatcherPrivate", 1, controller.Throttle)), controller)
		{
			this.parentMailboxDispatcher = parentMailboxDispatcher;
			this.committedWatermark = watermark;
			this.highestEventQueued = watermark;
		}

		public long CommittedWatermark
		{
			get
			{
				return this.committedWatermark;
			}
			set
			{
				this.committedWatermark = value;
			}
		}

		public bool IsMailboxDead
		{
			get
			{
				return this.parentMailboxDispatcher.IsMailboxDead;
			}
		}

		public bool IsIdle
		{
			get
			{
				return !this.IsInRetry && base.ActiveQueue.Count == 0 && base.PendingQueue.Count == 0;
			}
		}

		public long RecoveryEventCounter
		{
			get
			{
				return this.recoveryEventCounter;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.parentMailboxDispatcher.MailboxGuid;
			}
		}

		public EventControllerPrivate ControllerPrivate
		{
			get
			{
				return (EventControllerPrivate)base.Controller;
			}
		}

		public override string MailboxDisplayName
		{
			get
			{
				return this.parentMailboxDispatcher.MailboxDisplayName;
			}
		}

		public Guid AssistantIdentity
		{
			get
			{
				return base.Assistant.Identity;
			}
		}

		protected override bool Shutdown
		{
			get
			{
				return base.Shutdown || this.IsMailboxDead || this.parentMailboxDispatcher.Shutdown;
			}
		}

		protected override bool CountEvents
		{
			get
			{
				return !this.IsInRetry;
			}
		}

		private bool IsInRetry
		{
			get
			{
				return this.isInRetry;
			}
			set
			{
				this.isInRetry = value;
			}
		}

		public void Initialize(EventAccess eventAccess, MapiEvent[] eventTable, long databaseCounter)
		{
			try
			{
				this.InitFromWatermark(eventAccess, eventTable, databaseCounter);
			}
			catch (MapiExceptionUnknownMailbox arg)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPrivate, MapiExceptionUnknownMailbox>((long)this.GetHashCode(), "{0}: Unable to InitFromWatermark: {1}", this, arg);
				long lastEventCounter = this.ControllerPrivate.GetLastEventCounter();
				try
				{
					this.InitFromWatermark(eventAccess, eventTable, databaseCounter);
				}
				catch (MapiExceptionUnknownMailbox arg2)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPrivate, MapiExceptionUnknownMailbox>((long)this.GetHashCode(), "{0}: Still unable to InitFromWatermark: {1}", this, arg2);
					this.SetAsDeadMailbox(databaseCounter, lastEventCounter);
				}
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Constructed", this);
		}

		public override string ToString()
		{
			return "Dispatcher for " + this.MailboxDisplayName + " and " + base.Assistant.Instance.Name;
		}

		public bool ProcessPolledEvent(EmergencyKit emergencyKit)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: ProcessPolledEvent {1}", this, emergencyKit.MapiEvent.EventCounter);
			if (this.IsInRetry)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: In Retry; discarding event {1}", this, emergencyKit.MapiEvent.EventCounter);
				return false;
			}
			if (emergencyKit.MapiEvent.EventCounter <= this.committedWatermark)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, long>((long)this.GetHashCode(), "{0}: Ignoring event {1} because it is below the watermark {2}", this, emergencyKit.MapiEvent.EventCounter, this.committedWatermark);
				return false;
			}
			if (emergencyKit.MapiEvent.EventCounter <= this.highestEventQueued)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, long>((long)this.GetHashCode(), "{0}: Ignoring event {1} because it was already queued {2}", this, emergencyKit.MapiEvent.EventCounter, this.committedWatermark);
				return false;
			}
			return base.EnqueueIfInteresting(emergencyKit);
		}

		public Watermark GetCurrentWatermark(ref long databaseWatermark)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: GetCurrentWatermark", this);
			Watermark result;
			lock (base.Locker)
			{
				long num = this.committedWatermark;
				if (this.IsMailboxDead)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Not writing watermark for dead mailbox", this);
					result = null;
				}
				else
				{
					InterestingEvent interestingEvent;
					if (base.ActiveQueue.Count > 0)
					{
						interestingEvent = base.ActiveQueue[0];
					}
					else if (base.PendingQueue.Count > 0)
					{
						interestingEvent = base.PendingQueue[0];
					}
					else
					{
						interestingEvent = null;
					}
					if (this.IsInRetry)
					{
						if (interestingEvent == null)
						{
							ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Not writing watermark for retry mailbox with empty queue", this);
						}
						else
						{
							num = interestingEvent.MapiEvent.EventCounter - 1L;
						}
					}
					else
					{
						if (interestingEvent == null)
						{
							num = Math.Max(databaseWatermark, this.committedWatermark);
						}
						else
						{
							num = interestingEvent.MapiEvent.EventCounter - 1L;
						}
						if (num < databaseWatermark)
						{
							ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: lowering databaseWatermark to {1}", this, num);
							databaseWatermark = num;
						}
					}
					if (this.committedWatermark != num)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, long>((long)this.GetHashCode(), "{0}: Will update watermark for mailbox from {1} to {2}", this, this.committedWatermark, num);
						result = Watermark.GetMailboxWatermark(this.MailboxGuid, num);
					}
					else
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Watermark has not changed from {1}.", this, num);
						result = null;
					}
				}
			}
			return result;
		}

		public void Recover()
		{
			base.Assistant.PerformanceCounters.FailedDispatchers.Decrement();
			lock (base.Locker)
			{
				this.IsInRetry = false;
				this.highestEventQueued = this.recoveryEventCounter - 1L;
			}
		}

		public void ClearPendingQueue()
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, int>((long)this.GetHashCode(), "{0}: Clearing PendingQueue, queue count:{1}", this, base.PendingQueue.Count);
			lock (base.Locker)
			{
				if (base.PendingQueue.Count != 0)
				{
					base.Assistant.PerformanceCounters.EventsInQueueCurrent.IncrementBy((long)(-(long)base.PendingQueue.Count));
					if (this.CountEvents)
					{
						base.Controller.DecrementEventQueueCount((long)base.PendingQueue.Count);
					}
					base.PendingQueue.Clear();
				}
			}
		}

		public bool IsAssistantInterestedInMailbox(ExchangePrincipal mailboxOwner)
		{
			bool flag = this.ShouldProcessMailbox(mailboxOwner);
			base.Assistant.PerformanceCounters.EventsDiscardedByMailboxFilterBase.Increment();
			if (!flag)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: This type of mailbox is not interesting to this assistant.", this);
				base.Assistant.PerformanceCounters.EventsDiscardedByMailboxFilter.Increment();
			}
			return flag;
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableEventDispatcher queryableEventDispatcher = queryableObject as QueryableEventDispatcher;
			if (queryableEventDispatcher != null)
			{
				queryableEventDispatcher.AssistantName = base.Assistant.Name;
				queryableEventDispatcher.AssistantGuid = base.Assistant.Identity;
				queryableEventDispatcher.CommittedWatermark = this.committedWatermark;
				queryableEventDispatcher.HighestEventQueued = this.highestEventQueued;
				queryableEventDispatcher.RecoveryEventCounter = this.recoveryEventCounter;
				queryableEventDispatcher.IsInRetry = this.isInRetry;
				queryableEventDispatcher.ActiveQueueLength = base.ActiveQueue.Count;
				queryableEventDispatcher.PendingQueueLength = base.PendingQueue.Count;
				queryableEventDispatcher.PendingWorkers = base.PendingWorkers;
				queryableEventDispatcher.ActiveWorkers = base.ActiveWorkers;
			}
		}

		protected override void EnqueueEvent(InterestingEvent interestingEvent)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: queueing event {1}...", this, interestingEvent.MapiEvent.EventCounter);
			lock (base.Locker)
			{
				if (base.Shutdown)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Not queueing event {1} for shutdown mailbox", this, interestingEvent.MapiEvent.EventCounter);
				}
				else if (this.IsMailboxDead)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Not queueing event {1} for dead mailbox", this, interestingEvent.MapiEvent.EventCounter);
				}
				else if (this.IsInRetry)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Not queueing event {1} for retry mailbox", this, interestingEvent.MapiEvent.EventCounter);
				}
				else
				{
					base.EnqueueEvent(interestingEvent);
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, long>((long)this.GetHashCode(), "{0}: Queued event {1}, old highestEventQueued: {2}", this, interestingEvent.MapiEvent.EventCounter, this.highestEventQueued);
					this.highestEventQueued = Math.Max(this.highestEventQueued, interestingEvent.MapiEvent.EventCounter);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.isInRetry)
				{
					base.Assistant.PerformanceCounters.FailedDispatchers.Decrement();
					this.isInRetry = false;
				}
				base.Assistant.PerformanceCounters.EventDispatchers.Decrement();
			}
		}

		protected override void OnWorkersStarted()
		{
			this.parentMailboxDispatcher.OnWorkersStarted();
		}

		protected override void OnWorkersClear()
		{
			this.parentMailboxDispatcher.OnWorkersClear();
		}

		protected override AIException DangerousProcessItem(EmergencyKit kit, InterestingEvent interestingEvent)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Dequeued event {1}", this, interestingEvent.MapiEvent.EventCounter);
			long lastEventCounter = 0L;
			AIException ex = null;
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					try
					{
						this.DispatchOneEvent(kit, interestingEvent);
					}
					catch (DeadMailboxException ex3)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPrivate, DeadMailboxException>((long)this.GetHashCode(), "{0}: Encountered DeadMailboxException #1: {1}", this, ex3);
						lastEventCounter = this.ControllerPrivate.GetLastEventCounter();
						ExTraceGlobals.EventDispatcherTracer.TraceError((long)this.GetHashCode(), "{0}: Read lastEventCounter {1} after DeadMailboxExcetion.  Retrying eventDispatch {2}.  Previous exception: {3}", new object[]
						{
							this,
							lastEventCounter,
							interestingEvent.MapiEvent.EventCounter,
							ex3
						});
						this.DispatchOneEvent(kit, interestingEvent);
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: DispatchOneEvent returned successfully for mapiEvent {1}", this, interestingEvent.MapiEvent.EventCounter);
					}
				}, (base.Assistant != null) ? base.Assistant.Name : "<null>");
			}
			catch (AIException ex2)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPrivate, AIException>((long)this.GetHashCode(), "{0}: Exception from DispatchOneEvent: {1}", this, ex2);
				ex = ex2;
			}
			if (ex is DeadMailboxException)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError((long)this.GetHashCode(), "{0}: Encountered DeadMailboxException #2.  mapiEvent: {1}, lastEventCounter: {2}, exception: {3}", new object[]
				{
					this,
					interestingEvent.MapiEvent.EventCounter,
					lastEventCounter,
					ex
				});
				this.SetAsDeadMailbox(interestingEvent.MapiEvent.EventCounter, lastEventCounter);
			}
			return ex;
		}

		protected override void OnCompletedItem(InterestingEvent interestingEvent, AIException exception)
		{
			base.OnCompletedItem(interestingEvent, exception);
			if (this.IsInRetry && !this.IsMailboxDead)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Recovering from retry state, mapiEvent {1}", this, interestingEvent.MapiEvent.EventCounter);
				this.recoveryEventCounter = interestingEvent.MapiEvent.EventCounter + 1L;
				this.ControllerPrivate.RequestRecovery(this);
			}
		}

		protected override void OnTransientFailure(InterestingEvent interestingEvent, AIException exception)
		{
			if (exception is TransientMailboxException)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPrivate, long, AIException>((long)this.GetHashCode(), "{0}: retryable mailbox exception, mapiEvent {1}: {2}", this, interestingEvent.MapiEvent.EventCounter, exception);
				lock (base.Locker)
				{
					this.ClearPendingQueue();
					if (!this.IsInRetry)
					{
						if (this.IsMailboxDead && ExTraceGlobals.EventDispatcherTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Mailbox had been on TransientFailure, but was found dead", this);
						}
						this.isInRetry = true;
						base.Controller.DecrementEventQueueCount();
						base.Assistant.PerformanceCounters.FailedDispatchers.Increment();
						if (EventDispatcherPrivate.syncWithTestCodeAfterEventQueueCountDecrementedForRetry != null)
						{
							EventDispatcherPrivate.syncWithTestCodeAfterEventQueueCountDecrementedForRetry();
						}
					}
				}
			}
		}

		private MapiEvent FindNextEventForMailbox(EventAccess eventAccess, long beginCounter)
		{
			Restriction restriction = Restriction.EQ(PropTag.EventMailboxGuid, this.MailboxGuid.ToByteArray());
			Restriction filter = (base.Controller.Filter == null) ? restriction : Restriction.And(new Restriction[]
			{
				restriction,
				base.Controller.Filter
			});
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Finding next for this mailbox from {1}...", this, beginCounter);
			long num;
			MapiEvent[] array = eventAccess.ReadEvents(beginCounter, 1, int.MaxValue, filter, out num);
			if (array.Length > 0)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, MapiEvent>((long)this.GetHashCode(), "{0}: Found next event for this mailbox from {1}: {2}", this, beginCounter, array[0]);
				return array[0];
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Found no events for mailbox after {1}", this, beginCounter);
			return null;
		}

		private MapiEvent FindNextEventForMailboxFromEventTable(MapiEvent[] eventTable, long beginCounter)
		{
			for (long num = 0L; num < (long)eventTable.Length; num += 1L)
			{
				checked
				{
					if (eventTable[(int)((IntPtr)num)].EventCounter >= beginCounter && eventTable[(int)((IntPtr)num)].MailboxGuid.Equals(this.MailboxGuid))
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long, MapiEvent>(unchecked((long)this.GetHashCode()), "{0}: Found next event from the mailbox table from {1}: {2}", this, beginCounter, eventTable[(int)((IntPtr)num)]);
						return eventTable[(int)((IntPtr)num)];
					}
				}
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: Found no events on the mailbox table after {1}", this, beginCounter);
			return null;
		}

		private void InitFromWatermark(EventAccess eventAccess, MapiEvent[] eventTable, long databaseCounter)
		{
			if (this.committedWatermark != 0L && this.committedWatermark < databaseCounter)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Searching for retryEvent...", this);
				MapiEvent mapiEvent;
				if (eventTable != null)
				{
					mapiEvent = this.FindNextEventForMailboxFromEventTable(eventTable, this.committedWatermark + 1L);
				}
				else
				{
					mapiEvent = this.FindNextEventForMailbox(eventAccess, this.committedWatermark + 1L);
				}
				if (mapiEvent != null && mapiEvent.EventCounter < databaseCounter)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug((long)this.GetHashCode(), "{0}: Starting in retry.  mailboxWatermark: {1}, databaseCounter: {2}, retryEvent: {3}", new object[]
					{
						this,
						this.committedWatermark,
						databaseCounter,
						mapiEvent.EventCounter
					});
					this.IsInRetry = true;
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Starting in retry.", this);
					base.Assistant.PerformanceCounters.FailedDispatchers.Increment();
					base.EnqueueItem(InterestingEvent.CreateUnprocessed(mapiEvent));
				}
			}
		}

		private void SetAsDeadMailbox(long databaseCounter, long lastEventCounter)
		{
			this.parentMailboxDispatcher.SetAsDeadMailbox(databaseCounter, lastEventCounter);
		}

		private bool ShouldProcessMailbox(ExchangePrincipal mailboxOwner)
		{
			if (!base.Assistants.NeedsMailboxSession)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: No assistant is interested in opening mailbox sessions. Mailbox Filter not applicable. Resume.", this);
				return true;
			}
			if (mailboxOwner == null)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: The mailbox owner is not known at this time. Resume.", this);
				return true;
			}
			if (mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox && !base.Assistant.Type.ProcessesPublicDatabases)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant is not interested in public folder mailboxes.", this);
				return false;
			}
			IMailboxFilter mailboxFilter = base.Assistant.Type.MailboxFilter;
			if (mailboxFilter == null)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant does not implement a mailbox filter. Assume it is interested in all types of mailboxes.", this);
				return true;
			}
			if (mailboxOwner.MailboxInfo.IsArchive)
			{
				if (mailboxFilter.MailboxType.Contains(MailboxType.Archive))
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant is interested in Archive mailboxes.", this);
					return true;
				}
			}
			else if (mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox)
			{
				if (mailboxFilter.MailboxType.Contains(MailboxType.Arbitration))
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant is interested in Arbitration mailboxes.", this);
					return true;
				}
			}
			else
			{
				if (mailboxFilter.MailboxType.Contains(MailboxType.User) && mailboxOwner.RecipientType == RecipientType.UserMailbox)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant is interested in User mailboxes.", this);
					return true;
				}
				if (mailboxFilter.MailboxType.Contains(MailboxType.System) && mailboxOwner.RecipientType == RecipientType.SystemMailbox)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Assistant is interested in System mailboxes.", this);
					return true;
				}
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug((long)this.GetHashCode(), "{0}: Assistant is not interested in this mailbox {1}, type {2}, details {3}, archive: {4}. Its mailbox filter is {5}", new object[]
			{
				this,
				mailboxOwner,
				mailboxOwner.RecipientType,
				mailboxOwner.RecipientTypeDetails,
				mailboxOwner.MailboxInfo.IsArchive,
				mailboxFilter.MailboxType
			});
			return false;
		}

		private void DispatchOneEvent(EmergencyKit kit, InterestingEvent interestingEvent)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, long>((long)this.GetHashCode(), "{0}: DispatchOneEvent {1}", this, kit.MapiEvent.EventCounter);
			if (!base.Assistants.NeedsMailboxSession)
			{
				this.HandleEventWithoutSession(kit, interestingEvent);
				return;
			}
			this.parentMailboxDispatcher.DispatchEvent(interestingEvent, delegate(ExchangePrincipal mailboxOwner)
			{
				bool flag = this.ShouldProcessMailbox(mailboxOwner);
				this.Assistant.PerformanceCounters.QueuedEventsDiscardedByMailboxFilterBase.Increment();
				if (!flag)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, MapiEvent>((long)this.GetHashCode(), "{0}: A previously queued event has been discarded by the mailbox filter. Event: {1}", this, kit.MapiEvent);
					this.Assistant.PerformanceCounters.QueuedEventsDiscardedByMailboxFilter.Increment();
				}
				return flag;
			}, delegate(MailboxSession mailboxSession)
			{
				using (Item item = this.TryOpenItem(mailboxSession, kit.MapiEvent))
				{
					this.HandleEvent(kit, interestingEvent, mailboxSession, item);
				}
			}, (base.Assistant != null) ? base.Assistant.Name : "<null>");
		}

		private Item TryOpenItem(MailboxSession mailboxSession, MapiEvent mapiEvent)
		{
			Item item = null;
			if (mapiEvent.ItemEntryId != null && mapiEvent.ItemType == ObjectType.MAPI_MESSAGE && (mapiEvent.EventMask & MapiEventTypeFlags.ObjectDeleted) == (MapiEventTypeFlags)0)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Getting itemId...", this);
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(mapiEvent.ItemEntryId);
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate, StoreObjectId>((long)this.GetHashCode(), "{0}: Binding to item: {1}", this, storeObjectId);
				Exception ex = null;
				bool flag = false;
				try
				{
					item = Item.Bind(mailboxSession, storeObjectId, base.Assistants.PreloadItemProperties);
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Bound item. Opening item...", this);
					item.OpenAsReadWrite();
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPrivate>((long)this.GetHashCode(), "{0}: Opened item.", this);
					flag = true;
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (ConversionFailedException ex3)
				{
					ex = ex3;
				}
				catch (VirusMessageDeletedException ex4)
				{
					ex = ex4;
				}
				finally
				{
					if (!flag && item != null)
					{
						item.Dispose();
						item = null;
					}
				}
				if (ex != null)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceError((long)this.GetHashCode(), "{0}: failed to open itemId {1} for eventId {2}.  Exception: {3}", new object[]
					{
						this,
						storeObjectId,
						mapiEvent.EventCounter,
						ex
					});
				}
			}
			return item;
		}

		private void HandleEventWithoutSession(EmergencyKit kit, InterestingEvent interestingEvent)
		{
			base.HandleEvent(kit, interestingEvent, null, null);
		}

		private static Action syncWithTestCodeAfterEventQueueCountDecrementedForRetry;

		private long committedWatermark;

		private long highestEventQueued;

		private long recoveryEventCounter;

		private bool isInRetry;

		private MailboxDispatcher parentMailboxDispatcher;
	}
}
