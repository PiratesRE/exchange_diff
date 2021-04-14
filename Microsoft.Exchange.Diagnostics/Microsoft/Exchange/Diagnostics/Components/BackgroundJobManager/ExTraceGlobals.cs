using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.BackgroundJobManager
{
	public static class ExTraceGlobals
	{
		public static Trace SDKTracer
		{
			get
			{
				if (ExTraceGlobals.sDKTracer == null)
				{
					ExTraceGlobals.sDKTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.sDKTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serviceTracer;
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

		private static Guid componentGuid = new Guid("790DC26A-9CC8-400D-84B0-1CAA155054BE");

		private static Trace sDKTracer = null;

		private static Trace serviceTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
