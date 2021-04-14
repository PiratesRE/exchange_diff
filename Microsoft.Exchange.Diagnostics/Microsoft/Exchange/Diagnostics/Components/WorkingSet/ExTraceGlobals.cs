using System;

namespace Microsoft.Exchange.Diagnostics.Components.WorkingSet
{
	public static class ExTraceGlobals
	{
		public static Trace WorkingSetAgentTracer
		{
			get
			{
				if (ExTraceGlobals.workingSetAgentTracer == null)
				{
					ExTraceGlobals.workingSetAgentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.workingSetAgentTracer;
			}
		}

		public static Trace WorkingSetPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.workingSetPublisherTracer == null)
				{
					ExTraceGlobals.workingSetPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.workingSetPublisherTracer;
			}
		}

		private static Guid componentGuid = new Guid("144FC985-645D-4889-97CB-841A82F498F3");

		private static Trace workingSetAgentTracer = null;

		private static Trace workingSetPublisherTracer = null;
	}
}
