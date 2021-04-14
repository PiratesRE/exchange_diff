using System;

namespace Microsoft.Exchange.Diagnostics.Components.EnvelopeFilter
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

		private static Guid componentGuid = new Guid("93959C88-FC97-45C2-BCA9-D8A360CC07CF");

		private static Trace agentTracer = null;
	}
}
