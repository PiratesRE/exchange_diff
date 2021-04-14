using System;

namespace Microsoft.Exchange.Diagnostics.Components.IPFilter
{
	public static class ExTraceGlobals
	{
		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		private static Guid componentGuid = new Guid("5383B207-8388-4459-9124-B229AD7A64F3");

		private static Trace agentTracer = null;
	}
}
