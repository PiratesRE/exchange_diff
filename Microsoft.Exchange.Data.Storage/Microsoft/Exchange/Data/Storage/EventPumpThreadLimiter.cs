using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EventPumpThreadLimiter
	{
		internal EventPumpThreadLimiter(EventPump eventPump)
		{
			this.eventPump = eventPump;
		}

		internal void RequestRecovery(EventSink eventSink)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				this.eventSinks.Enqueue(eventSink);
				if (!this.isThreadActive)
				{
					this.isThreadActive = true;
					flag = true;
				}
			}
			if (flag)
			{
				this.ExecuteRecovery();
			}
		}

		private void ExecuteRecovery()
		{
			bool flag = false;
			while (!flag)
			{
				EventSink eventSink = null;
				lock (this.thisLock)
				{
					eventSink = this.eventSinks.Dequeue();
				}
				try
				{
					this.eventPump.ExecuteRecovery(eventSink);
				}
				catch (StoragePermanentException arg)
				{
					ExTraceGlobals.EventTracer.TraceError<EventPumpThreadLimiter, EventSink, StoragePermanentException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught during recovery. EventSink = {1}. Exception = {2}.", this, eventSink, arg);
				}
				catch (StorageTransientException arg2)
				{
					ExTraceGlobals.EventTracer.TraceError<EventPumpThreadLimiter, EventSink, StorageTransientException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught during recovery. EventSink = {1}. Exception = {2}.", this, eventSink, arg2);
				}
				lock (this.thisLock)
				{
					flag = (this.eventSinks.Count == 0);
					if (flag)
					{
						this.isThreadActive = false;
					}
				}
			}
		}

		private bool isThreadActive;

		private Queue<EventSink> eventSinks = new Queue<EventSink>();

		private readonly EventPump eventPump;

		private readonly object thisLock = new object();
	}
}
