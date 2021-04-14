using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.LazyIndexing
{
	public static class ExTraceGlobals
	{
		public static Trace PseudoIndexTracer
		{
			get
			{
				if (ExTraceGlobals.pseudoIndexTracer == null)
				{
					ExTraceGlobals.pseudoIndexTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.pseudoIndexTracer;
			}
		}

		public static Trace CategoryHeaderViewPopulationTracer
		{
			get
			{
				if (ExTraceGlobals.categoryHeaderViewPopulationTracer == null)
				{
					ExTraceGlobals.categoryHeaderViewPopulationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.categoryHeaderViewPopulationTracer;
			}
		}

		public static Trace CategoryHeaderViewMaintenanceTracer
		{
			get
			{
				if (ExTraceGlobals.categoryHeaderViewMaintenanceTracer == null)
				{
					ExTraceGlobals.categoryHeaderViewMaintenanceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.categoryHeaderViewMaintenanceTracer;
			}
		}

		public static Trace CategorizedViewsTracer
		{
			get
			{
				if (ExTraceGlobals.categorizedViewsTracer == null)
				{
					ExTraceGlobals.categorizedViewsTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.categorizedViewsTracer;
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

		private static Guid componentGuid = new Guid("0e12474e-7e64-471f-93f5-901f795c4ae0");

		private static Trace pseudoIndexTracer = null;

		private static Trace categoryHeaderViewPopulationTracer = null;

		private static Trace categoryHeaderViewMaintenanceTracer = null;

		private static Trace categorizedViewsTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
