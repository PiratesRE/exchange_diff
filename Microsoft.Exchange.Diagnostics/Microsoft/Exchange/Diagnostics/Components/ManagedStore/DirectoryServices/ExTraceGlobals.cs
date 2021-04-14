using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.DirectoryServices
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace ADCallsTracer
		{
			get
			{
				if (ExTraceGlobals.aDCallsTracer == null)
				{
					ExTraceGlobals.aDCallsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.aDCallsTracer;
			}
		}

		public static Trace CallStackTracer
		{
			get
			{
				if (ExTraceGlobals.callStackTracer == null)
				{
					ExTraceGlobals.callStackTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.callStackTracer;
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

		private static Guid componentGuid = new Guid("2d756daa-9cee-497d-b5a1-dc2f994b99ca");

		private static Trace generalTracer = null;

		private static Trace aDCallsTracer = null;

		private static Trace callStackTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
