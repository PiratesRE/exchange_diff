using System;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal class EventDispatcherPublic : EventDispatcher
	{
		public EventDispatcherPublic(AssistantCollectionEntry assistant, EventControllerPublic controller, ThrottleGovernor governor) : base(assistant, new MailboxGovernor(controller.Governor, new Throttle("EventDispatcherPublic", controller.Throttle.OpenThrottleValue, controller.Throttle)), controller)
		{
		}

		public override string MailboxDisplayName
		{
			get
			{
				return "<public folder database>";
			}
		}

		public override string ToString()
		{
			return "Dispatcher for database " + base.Controller.DatabaseInfo.DisplayName;
		}

		public void ProcessPolledEvent(MapiEvent mapiEvent)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPublic, long>((long)this.GetHashCode(), "{0}: ProcessPolledEvent {1}", this, mapiEvent.EventCounter);
			base.EnqueueIfInteresting(new EmergencyKit(mapiEvent));
		}

		public long GetWatermark(long highestEventPolled)
		{
			return Math.Min(highestEventPolled, this.FindLowestQueuedEventCounter() - 1L);
		}

		protected override AIException DangerousProcessItem(EmergencyKit kit, InterestingEvent interestingEvent)
		{
			AIException result = null;
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					this.HandleEvent(kit, interestingEvent, null, null);
				}, (base.Assistant != null) ? base.Assistant.Name : "<null>");
			}
			catch (AIException ex)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcherPublic, AIException>((long)this.GetHashCode(), "{0}: Exception from HandleEvent: {1}", this, ex);
				result = ex;
			}
			return result;
		}

		private long FindLowestQueuedEventCounter()
		{
			long num = long.MaxValue;
			lock (base.Locker)
			{
				foreach (InterestingEvent interestingEvent in base.ActiveQueue)
				{
					num = Math.Min(num, interestingEvent.MapiEvent.EventCounter);
				}
				foreach (InterestingEvent interestingEvent2 in base.PendingQueue)
				{
					num = Math.Min(num, interestingEvent2.MapiEvent.EventCounter);
				}
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcherPublic, long>((long)this.GetHashCode(), "{0}: LowestQueuedEvent is {1}", this, num);
			return num;
		}
	}
}
