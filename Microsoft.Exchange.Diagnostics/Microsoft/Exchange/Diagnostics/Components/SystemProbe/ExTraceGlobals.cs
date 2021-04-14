using System;

namespace Microsoft.Exchange.Diagnostics.Components.SystemProbe
{
	public static class ExTraceGlobals
	{
		public static Trace ProbeTracer
		{
			get
			{
				if (ExTraceGlobals.probeTracer == null)
				{
					ExTraceGlobals.probeTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.probeTracer;
			}
		}

		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		private static Guid componentGuid = new Guid("89ff0675-2089-453d-8c5a-21d9466a6eed");

		private static Trace probeTracer = null;

		private static Trace agentTracer = null;
	}
}
