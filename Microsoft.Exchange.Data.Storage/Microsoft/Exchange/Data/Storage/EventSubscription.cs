using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EventSubscription : IDisposeTrackable, IDisposable
	{
		internal EventSubscription(EventQueue eventQueue, IEventHandler eventHandler)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.eventQueue = eventQueue;
			this.eventQueue.RegisterEventAvailableHandler(new EventQueue.EventAvailableHandler(this.EventAvailableHandler));
			this.eventHandler = eventHandler;
			ExTraceGlobals.EventTracer.TraceDebug<EventSubscription>((long)this.GetHashCode(), "EventSubscription::Constructor. {0}", this);
		}

		public static EventSubscription Create(StoreSession session, EventCondition condition, IEventHandler eventHandler)
		{
			return EventSubscription.InternalCreate(session, condition, eventHandler, null);
		}

		public static EventSubscription Create(StoreSession session, EventCondition condition, IEventHandler eventHandler, EventWatermark watermark)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			return EventSubscription.InternalCreate(session, condition, eventHandler, watermark);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EventSubscription>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static EventSubscription InternalCreate(StoreSession session, EventCondition condition, IEventHandler eventHandler, EventWatermark watermark)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			if (eventHandler == null)
			{
				throw new ArgumentNullException("eventHandler");
			}
			EventQueue eventQueue = null;
			if (watermark != null)
			{
				eventQueue = EventQueue.Create(session, condition, 10, watermark);
			}
			else
			{
				eventQueue = EventQueue.Create(session, condition, 10);
			}
			bool flag = false;
			EventSubscription result;
			try
			{
				EventSubscription eventSubscription = new EventSubscription(eventQueue, eventHandler);
				flag = true;
				result = eventSubscription;
			}
			finally
			{
				if (!flag && eventQueue != null)
				{
					eventQueue.Dispose();
					eventQueue = null;
				}
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
				ExTraceGlobals.EventTracer.TraceDebug<EventSubscription>((long)this.GetHashCode(), "EventSubscription::Dispose. {0}", this);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.thisLock)
				{
					if (this.eventQueue != null)
					{
						this.eventQueue.Dispose();
					}
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		protected bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public override string ToString()
		{
			return string.Format("Internal Event Queue = {0}.", this.eventQueue);
		}

		private void CallBack(object state)
		{
			if (Interlocked.Increment(ref this.countWorkerThreads) > 1)
			{
				return;
			}
			LocalizedException ex;
			for (;;)
			{
				Event @event = null;
				ex = null;
				do
				{
					lock (this.thisLock)
					{
						if (this.IsDisposed)
						{
							ExTraceGlobals.EventTracer.TraceDebug<EventSubscription>((long)this.GetHashCode(), "EventSubscription::CallBack. {0}. Aborted notification delivery because EventSubscription was disposed.", this);
							return;
						}
						try
						{
							@event = this.eventQueue.GetEvent();
						}
						catch (StoragePermanentException ex2)
						{
							ex = ex2;
						}
						catch (StorageTransientException ex3)
						{
							ex = ex3;
						}
					}
					if (@event != null)
					{
						this.eventHandler.Consume(@event);
					}
					else if (ex != null)
					{
						goto Block_4;
					}
				}
				while (@event != null);
				if (Interlocked.Decrement(ref this.countWorkerThreads) <= 0)
				{
					return;
				}
			}
			Block_4:
			this.eventHandler.HandleException(ex);
		}

		private void EventAvailableHandler()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.CallBack));
		}

		private readonly EventQueue eventQueue;

		private readonly IEventHandler eventHandler;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;

		private readonly object thisLock = new object();

		private int countWorkerThreads;
	}
}
