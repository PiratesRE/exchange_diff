using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal sealed class MapiTimedEventHandler : ITimedEventHandler
	{
		public void Invoke(Context context, TimedEventEntry timedEvent)
		{
			ITimedEventHandler timedEventHandler = null;
			if (MapiTimedEventHandler.eventHandlerMap.TryGetValue((MapiTimedEvents.EventType)timedEvent.EventType, out timedEventHandler))
			{
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug(45384L, string.Concat(new object[]
					{
						"Event type=",
						timedEvent.EventType,
						" handler=",
						timedEventHandler.GetType().Name
					}));
				}
				timedEventHandler.Invoke(context, timedEvent);
				return;
			}
			MapiTimedEventHandler.defaultHandler.Invoke(context, timedEvent);
		}

		private static readonly Dictionary<MapiTimedEvents.EventType, ITimedEventHandler> eventHandlerMap = new Dictionary<MapiTimedEvents.EventType, ITimedEventHandler>
		{
			{
				MapiTimedEvents.EventType.DeferredSend,
				new DeferredSendHandler()
			}
		};

		private static readonly ITimedEventHandler defaultHandler = new MapiTimedEventHandler.TimedEventDefaultHandler();

		private sealed class TimedEventDefaultHandler : ITimedEventHandler
		{
			public void Invoke(Context context, TimedEventEntry timedEvent)
			{
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug<string>(51528L, "Mapi Timed event {0} does not have registered handler", timedEvent.ToString());
				}
			}
		}
	}
}
