using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.MultiMailboxSearch
{
	public static class ExTraceGlobals
	{
		public static Trace FullTextIndexTracer
		{
			get
			{
				if (ExTraceGlobals.fullTextIndexTracer == null)
				{
					ExTraceGlobals.fullTextIndexTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.fullTextIndexTracer;
			}
		}

		public static Trace SearchTracer
		{
			get
			{
				if (ExTraceGlobals.searchTracer == null)
				{
					ExTraceGlobals.searchTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.searchTracer;
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

		private static Guid componentGuid = new Guid("A1D513D5-5D1D-4c2d-A5D4-44005EA7DB83");

		private static Trace fullTextIndexTracer = null;

		private static Trace searchTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
