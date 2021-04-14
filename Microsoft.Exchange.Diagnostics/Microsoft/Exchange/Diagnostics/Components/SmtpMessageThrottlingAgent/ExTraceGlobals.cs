using System;

namespace Microsoft.Exchange.Diagnostics.Components.SmtpMessageThrottlingAgent
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

		private static Guid componentGuid = new Guid("B9416E03-DF78-4A00-87D7-48AEE982B9FE");

		private static Trace agentTracer = null;
	}
}
