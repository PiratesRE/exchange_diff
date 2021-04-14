using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy
{
	public static class ExTraceGlobals
	{
		public static Trace ProxyAdminTracer
		{
			get
			{
				if (ExTraceGlobals.proxyAdminTracer == null)
				{
					ExTraceGlobals.proxyAdminTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.proxyAdminTracer;
			}
		}

		public static Trace ProxyMapiTracer
		{
			get
			{
				if (ExTraceGlobals.proxyMapiTracer == null)
				{
					ExTraceGlobals.proxyMapiTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.proxyMapiTracer;
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

		private static Guid componentGuid = new Guid("437834ff-ce93-406a-be6e-4547009136c8");

		private static Trace proxyAdminTracer = null;

		private static Trace proxyMapiTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
