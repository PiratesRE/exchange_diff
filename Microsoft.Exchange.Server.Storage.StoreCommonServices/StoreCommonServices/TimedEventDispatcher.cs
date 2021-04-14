using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class TimedEventDispatcher : ITimedEventHandler
	{
		internal static IDictionary<Guid, ITimedEventHandler> DispatchMap
		{
			get
			{
				return TimedEventDispatcher.dispatchMap;
			}
		}

		public static void RegisterHandler(Guid eventSource, ITimedEventHandler handler)
		{
			if (!TimedEventDispatcher.dispatchMap.ContainsKey(eventSource))
			{
				TimedEventDispatcher.dispatchMap.Add(eventSource, handler);
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug<string, string>(37192L, "Handler registered, source={0}, handler={1}", eventSource.ToString(), handler.GetType().ToString());
				}
			}
		}

		public static void UnregisterHandler(Guid eventSource)
		{
			if (TimedEventDispatcher.dispatchMap.ContainsKey(eventSource))
			{
				TimedEventDispatcher.dispatchMap.Remove(eventSource);
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug<string>(53576L, "Handler unregistered, source={0}", eventSource.ToString());
				}
			}
		}

		public void Invoke(Context context, TimedEventEntry timedEvent)
		{
			ITimedEventHandler timedEventHandler = null;
			if (!TimedEventDispatcher.dispatchMap.TryGetValue(timedEvent.EventSource, out timedEventHandler))
			{
				TimedEventDispatcher.defaultHandler.Invoke(context, timedEvent);
				return;
			}
			if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.TimedEventsTracer.TraceDebug(41288L, "Event=[" + timedEvent.ToString() + "] handler=" + timedEventHandler.GetType().ToString());
			}
			timedEventHandler.Invoke(context, timedEvent);
		}

		private static readonly ITimedEventHandler defaultHandler = new TimedEventDispatcher.TimedEventDefaultHandler();

		private static IDictionary<Guid, ITimedEventHandler> dispatchMap = new LockFreeDictionary<Guid, ITimedEventHandler>();

		private sealed class TimedEventDefaultHandler : ITimedEventHandler
		{
			public void Invoke(Context context, TimedEventEntry timedEvent)
			{
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug<string>(61768L, "TimedEvent {0} has no registered handler", timedEvent.ToString());
				}
			}
		}
	}
}
