using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Tasks
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

		public static Trace LogTracer
		{
			get
			{
				if (ExTraceGlobals.logTracer == null)
				{
					ExTraceGlobals.logTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.logTracer;
			}
		}

		public static Trace ErrorTracer
		{
			get
			{
				if (ExTraceGlobals.errorTracer == null)
				{
					ExTraceGlobals.errorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.errorTracer;
			}
		}

		public static Trace EventTracer
		{
			get
			{
				if (ExTraceGlobals.eventTracer == null)
				{
					ExTraceGlobals.eventTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.eventTracer;
			}
		}

		public static Trace EnterExitTracer
		{
			get
			{
				if (ExTraceGlobals.enterExitTracer == null)
				{
					ExTraceGlobals.enterExitTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.enterExitTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("1e254b9e-d663-4138-8183-e5e4b077f8d3");

		private static Trace traceTracer = null;

		private static Trace logTracer = null;

		private static Trace errorTracer = null;

		private static Trace eventTracer = null;

		private static Trace enterExitTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
