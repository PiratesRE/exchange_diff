using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class RoutingDiag
	{
		public static readonly ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.RoutingTracer.Category, TransportEventLog.GetEventSource());

		public static readonly Trace Tracer = ExTraceGlobals.RoutingTracer;

		public static readonly SystemProbeTrace SystemProbeTracer = new SystemProbeTrace(ExTraceGlobals.RoutingTracer, "Routing");
	}
}
