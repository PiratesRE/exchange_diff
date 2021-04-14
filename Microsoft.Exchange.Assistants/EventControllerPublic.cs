using System;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventControllerPublic : EventController
	{
		public EventControllerPublic(DatabaseInfo databaseInfo, EventBasedAssistantCollection assistants, PoisonEventControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters, ThrottleGovernor serverGovernor) : base(databaseInfo, assistants, poisonControl, databaseCounters, serverGovernor, (MapiEventTypeFlags)0)
		{
			this.dispatcher = new EventDispatcherPublic(assistants.GetAssistantForPublicFolder(), this, base.Governor);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dispatcher.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void WaitUntilStoppedInternal()
		{
			this.dispatcher.WaitForShutdown();
		}

		protected override void ProcessPolledEvent(MapiEvent mapiEvent)
		{
			this.dispatcher.ProcessPolledEvent(mapiEvent);
		}

		protected override void UpdateWatermarksForAssistant(Guid assistantId)
		{
			long watermark = this.dispatcher.GetWatermark(base.HighestEventPolled);
			if (watermark != base.DatabaseBookmark[assistantId])
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventControllerPublic, long>((long)this.GetHashCode(), "{0}: Saving database watermark at {1}", this, watermark);
				base.EventAccess.SaveWatermarks(assistantId, new Watermark[]
				{
					Watermark.GetDatabaseWatermark(watermark)
				});
				base.DatabaseBookmark[assistantId] = watermark;
			}
		}

		private EventDispatcherPublic dispatcher;
	}
}
