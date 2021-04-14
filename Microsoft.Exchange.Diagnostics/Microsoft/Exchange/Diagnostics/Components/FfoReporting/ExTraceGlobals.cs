using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.FfoReporting
{
	public static class ExTraceGlobals
	{
		public static Trace CmdletsTracer
		{
			get
			{
				if (ExTraceGlobals.cmdletsTracer == null)
				{
					ExTraceGlobals.cmdletsTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.cmdletsTracer;
			}
		}

		public static Trace HttpModuleTracer
		{
			get
			{
				if (ExTraceGlobals.httpModuleTracer == null)
				{
					ExTraceGlobals.httpModuleTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.httpModuleTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("68B388E3-66FC-486C-BD59-C1738D89D4D7");

		private static Trace cmdletsTracer = null;

		private static Trace httpModuleTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
