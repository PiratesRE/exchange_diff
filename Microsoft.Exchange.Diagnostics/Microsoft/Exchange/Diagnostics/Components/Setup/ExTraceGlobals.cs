using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Setup
{
	public static class ExTraceGlobals
	{
		public static Trace TraceTracer
		{
			get
			{
				if (ExTraceGlobals.traceTracer == null)
				{
					ExTraceGlobals.traceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.traceTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("0868076d-75ca-47bf-8d73-487edd017b4d");

		private static Trace traceTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
