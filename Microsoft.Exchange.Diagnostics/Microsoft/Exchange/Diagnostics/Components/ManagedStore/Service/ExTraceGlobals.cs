using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service
{
	public static class ExTraceGlobals
	{
		public static Trace StartupShutdownTracer
		{
			get
			{
				if (ExTraceGlobals.startupShutdownTracer == null)
				{
					ExTraceGlobals.startupShutdownTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.startupShutdownTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("2e177940-9c28-43b0-9f7a-b92bf03227a6");

		private static Trace startupShutdownTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
