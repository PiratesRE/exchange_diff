using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck
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

		public static Trace OnlineIsintegTracer
		{
			get
			{
				if (ExTraceGlobals.onlineIsintegTracer == null)
				{
					ExTraceGlobals.onlineIsintegTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.onlineIsintegTracer;
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

		private static Guid componentGuid = new Guid("856DA9F3-E7F6-4565-84F6-71A96AF18D92");

		private static Trace startupShutdownTracer = null;

		private static Trace onlineIsintegTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
