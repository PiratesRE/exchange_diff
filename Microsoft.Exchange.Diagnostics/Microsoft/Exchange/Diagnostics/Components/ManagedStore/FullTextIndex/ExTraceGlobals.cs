using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.FullTextIndex
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

		public static Trace CriteriaFullTextFlavorTracer
		{
			get
			{
				if (ExTraceGlobals.criteriaFullTextFlavorTracer == null)
				{
					ExTraceGlobals.criteriaFullTextFlavorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.criteriaFullTextFlavorTracer;
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

		private static Guid componentGuid = new Guid("58B1ADEC-A4DF-4762-A6C6-92D8877B408C");

		private static Trace fullTextIndexTracer = null;

		private static Trace criteriaFullTextFlavorTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
