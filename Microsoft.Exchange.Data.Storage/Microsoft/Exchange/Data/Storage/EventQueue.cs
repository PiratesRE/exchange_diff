using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EventQueue : EventSink, IRecoveryEventSink, IDisposable
	{
		private EventQueue(Guid mailboxGuid, bool isPublicFolderDatabase, EventCondition condition, int maxQueueSize, EventWatermark firstMissedEventWatermark) : base(mailboxGuid, isPublicFolderDatabase, condition)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (maxQueueSize < 10 || maxQueueSize > 1000)
				{
					throw new ArgumentOutOfRangeException("maxQueueSize", ServerStrings.ExInvalidMaxQueueSize);
				}
				this.maxQueueSize = maxQueueSize;
				this.mainQueue = new Queue<Event>(10);
				this.recoveryQueue = new Queue<Event>(10);
				if (firstMissedEventWatermark != null)
				{
					this.firstMissedEventWatermark = firstMissedEventWatermark;
					this.startInRecovery = true;
					this.areThereMissedEvents = true;
				}
				disposeGuard.Success();
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EventQueue>(this);
		}

		internal override void HandleException(Exception exception)
		{
			lock (this.thisLock)
			{
				base.HandleException(exception);
				this.SetEventAvailable();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.eventAvailableEvent != null)
			{
				lock (this.lockEventAvailableEvent)
				{
					this.eventAvailableEvent.Dispose();
					this.eventAvailableEvent = null;
				}
			}
			base.InternalDispose(disposing);
		}

		protected override void InternalConsume(MapiEvent mapiEvent)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				if (this.startInRecovery)
				{
					this.lastMissedEventWatermark = mapiEvent.Watermark.EventCounter;
					this.startInRecovery = false;
					flag = true;
					this.state = EventQueue.State.NeedRecovery;
				}
				this.currentEventsCount++;
				if (this.mainQueue.Count == this.maxQueueSize || this.areThereMissedEvents)
				{
					if (!this.areThereMissedEvents)
					{
						this.firstMissedEventWatermark = new EventWatermark(base.MdbGuid, mapiEvent.Watermark.EventCounter, false);
						this.areThereMissedEvents = true;
					}
					this.lastMissedEventWatermark = mapiEvent.Watermark.EventCounter;
				}
				else
				{
					this.mainQueue.Enqueue(new Event(base.MdbGuid, mapiEvent));
					if (!this.areThereRecoveryMissedEvents)
					{
						this.SetEventAvailable();
					}
				}
				this.lastKnownWatermark = mapiEvent.Watermark.EventCounter;
			}
			if (flag)
			{
				base.RequestRecovery();
			}
		}

		internal override IRecoveryEventSink StartRecovery()
		{
			this.CheckDisposed(null);
			lock (this.thisLock)
			{
				if (!this.areThereRecoveryMissedEvents)
				{
					this.recoveryFirstMissedEventWatermark = this.firstMissedEventWatermark;
					this.recoveryLastMissedEventWatermark = this.lastMissedEventWatermark;
					this.areThereRecoveryMissedEvents = this.areThereMissedEvents;
					this.firstMissedEventWatermark = null;
					this.lastMissedEventWatermark = 0L;
					this.areThereMissedEvents = false;
				}
				this.state = EventQueue.State.Recovery;
			}
			return this;
		}

		internal override EventWatermark GetCurrentEventWatermark()
		{
			EventWatermark result;
			lock (this.thisLock)
			{
				if (this.recoveryQueue.Count != 0)
				{
					result = new EventWatermark(base.MdbGuid, this.recoveryQueue.Peek().MapiWatermark, false);
				}
				else if (this.areThereRecoveryMissedEvents)
				{
					result = this.recoveryFirstMissedEventWatermark;
				}
				else if (this.mainQueue.Count != 0)
				{
					result = new EventWatermark(base.MdbGuid, this.mainQueue.Peek().MapiWatermark, false);
				}
				else if (this.areThereMissedEvents)
				{
					result = this.firstMissedEventWatermark;
				}
				else if (base.FirstEventToConsumeWatermark > this.lastKnownWatermark)
				{
					result = new EventWatermark(base.MdbGuid, base.FirstEventToConsumeWatermark, false);
				}
				else
				{
					result = new EventWatermark(base.MdbGuid, this.lastKnownWatermark, true);
				}
			}
			return result;
		}

		internal override void SetLastKnownWatermark(long mapiWatermark, bool mayInitiateRecovery)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				this.lastKnownWatermark = mapiWatermark;
				if (this.startInRecovery && mayInitiateRecovery)
				{
					this.lastMissedEventWatermark = mapiWatermark;
					this.startInRecovery = false;
					flag = true;
					this.state = EventQueue.State.NeedRecovery;
				}
			}
			if (flag)
			{
				base.RequestRecovery();
			}
		}

		internal override void SetFirstEventToConsumeOnSink(long firstEventToConsumeWatermark)
		{
			lock (this.thisLock)
			{
				base.FirstEventToConsumeWatermark = firstEventToConsumeWatermark;
			}
		}

		bool IRecoveryEventSink.RecoveryConsume(MapiEvent mapiEvent)
		{
			this.CheckDisposed(null);
			base.CheckForFinalEvents(mapiEvent);
			if (base.IsEventRelevant(mapiEvent))
			{
				Event item = new Event(base.MdbGuid, mapiEvent);
				lock (this.thisLock)
				{
					this.recoveryFirstMissedEventWatermark = new EventWatermark(base.MdbGuid, mapiEvent.Watermark.EventCounter, true);
					this.recoveryQueue.Enqueue(item);
					this.SetEventAvailable();
					if (this.recoveryQueue.Count == this.maxQueueSize)
					{
						this.state = EventQueue.State.Normal;
						return false;
					}
				}
				return true;
			}
			return true;
		}

		void IRecoveryEventSink.EndRecovery()
		{
			lock (this.thisLock)
			{
				this.state = EventQueue.State.Normal;
				this.areThereRecoveryMissedEvents = false;
			}
		}

		EventWatermark IRecoveryEventSink.FirstMissedEventWatermark
		{
			get
			{
				this.CheckDisposed(null);
				return this.recoveryFirstMissedEventWatermark;
			}
		}

		long IRecoveryEventSink.LastMissedEventWatermark
		{
			get
			{
				this.CheckDisposed(null);
				return this.recoveryLastMissedEventWatermark;
			}
		}

		public static TimeSpan PollingInterval
		{
			get
			{
				return EventPump.PollingTimeSpan;
			}
			set
			{
				EventPump.PollingTimeSpan = value;
			}
		}

		public static EventQueue Create(StoreSession session, EventCondition condition, int maxQueueSize)
		{
			return EventSink.InternalCreateEventSink<EventQueue>(session, null, () => new EventQueue(session.MailboxGuid, session is PublicFolderSession, condition, maxQueueSize, null));
		}

		public static EventQueue Create(StoreSession session, EventCondition condition, int maxQueueSize, EventWatermark watermark)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			return EventSink.InternalCreateEventSink<EventQueue>(session, watermark, () => new EventQueue(session.MailboxGuid, session is PublicFolderSession, condition, maxQueueSize, watermark));
		}

		public bool HasMissedEvents
		{
			get
			{
				this.CheckDisposed(null);
				bool result;
				lock (this.thisLock)
				{
					result = (this.areThereMissedEvents || this.areThereRecoveryMissedEvents);
				}
				return result;
			}
		}

		public int CurrentEventsCount
		{
			get
			{
				this.CheckDisposed(null);
				return this.currentEventsCount;
			}
		}

		public Event GetEvent()
		{
			this.CheckDisposed(null);
			Event result = null;
			bool flag = false;
			lock (this.thisLock)
			{
				try
				{
					base.CheckException();
				}
				finally
				{
					if (base.IsExceptionPresent)
					{
						this.ResetEventAvailable();
					}
				}
				if (this.recoveryQueue.Count != 0)
				{
					result = this.recoveryQueue.Dequeue();
					if (this.recoveryQueue.Count == 0)
					{
						if (this.areThereRecoveryMissedEvents || this.mainQueue.Count == 0)
						{
							this.ResetEventAvailable();
						}
						if (this.NeedToRecover())
						{
							this.state = EventQueue.State.NeedRecovery;
							flag = true;
						}
					}
				}
				else if (this.mainQueue.Count != 0 && !this.areThereRecoveryMissedEvents)
				{
					result = this.mainQueue.Dequeue();
					if (this.mainQueue.Count == 0)
					{
						this.ResetEventAvailable();
						if (this.NeedToRecover())
						{
							this.state = EventQueue.State.NeedRecovery;
							flag = true;
						}
					}
				}
				this.currentEventsCount--;
			}
			if (flag)
			{
				base.RequestRecovery();
			}
			return result;
		}

		public WaitHandle EventAvailableWaitHandle
		{
			get
			{
				this.CheckDisposed(null);
				if (this.eventAvailableEvent == null)
				{
					this.eventAvailableEvent = new ManualResetEvent(false);
				}
				return this.eventAvailableEvent;
			}
		}

		public void RegisterEventAvailableHandler(EventQueue.EventAvailableHandler handler)
		{
			this.CheckDisposed(null);
			this.eventAvailable = handler;
		}

		public bool IsQueueEmptyAndUpToDate()
		{
			this.CheckDisposed(null);
			bool result;
			lock (this.thisLock)
			{
				result = (this.mainQueue.Count == 0 && this.recoveryQueue.Count == 0 && !this.areThereMissedEvents && !this.areThereRecoveryMissedEvents);
			}
			return result;
		}

		public void ResetQueue()
		{
			this.CheckDisposed(null);
			lock (this.thisLock)
			{
				this.mainQueue.Clear();
				this.recoveryQueue.Clear();
				this.areThereMissedEvents = false;
				this.areThereRecoveryMissedEvents = false;
				this.lastMissedEventWatermark = 0L;
				this.recoveryFirstMissedEventWatermark = null;
				this.recoveryLastMissedEventWatermark = 0L;
				this.startInRecovery = false;
				this.lastKnownWatermark = 0L;
				this.currentEventsCount = 0;
				this.state = EventQueue.State.Normal;
				this.ResetEventAvailable();
			}
		}

		private void SetEventAvailable()
		{
			if (Interlocked.Exchange(ref this.eventAvailableCount, 1) == 0 && this.eventAvailable != null)
			{
				this.eventAvailable();
			}
			if (this.eventAvailableEvent != null)
			{
				lock (this.lockEventAvailableEvent)
				{
					if (this.eventAvailableEvent != null)
					{
						this.eventAvailableEvent.Set();
					}
				}
			}
		}

		private void ResetEventAvailable()
		{
			Interlocked.Exchange(ref this.eventAvailableCount, 0);
			if (this.eventAvailableEvent != null)
			{
				lock (this.lockEventAvailableEvent)
				{
					if (this.eventAvailableEvent != null)
					{
						this.eventAvailableEvent.Reset();
					}
				}
			}
		}

		private bool NeedToRecover()
		{
			return this.state == EventQueue.State.Normal && (this.areThereRecoveryMissedEvents || (this.mainQueue.Count == 0 && this.areThereMissedEvents));
		}

		public const int DefaultEventQueueSize = 10;

		private readonly Queue<Event> mainQueue;

		private long lastMissedEventWatermark;

		private bool areThereMissedEvents;

		private readonly Queue<Event> recoveryQueue;

		private EventWatermark recoveryFirstMissedEventWatermark;

		private long recoveryLastMissedEventWatermark;

		private bool areThereRecoveryMissedEvents;

		private bool startInRecovery;

		private EventQueue.State state;

		private ManualResetEvent eventAvailableEvent;

		private readonly object lockEventAvailableEvent = new object();

		private readonly int maxQueueSize;

		private readonly object thisLock = new object();

		private long lastKnownWatermark;

		private EventQueue.EventAvailableHandler eventAvailable;

		private int eventAvailableCount;

		private int currentEventsCount;

		public delegate void EventAvailableHandler();

		private enum State
		{
			Normal,
			NeedRecovery,
			Recovery
		}
	}
}
