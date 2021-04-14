using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		public static Trace DataAccessTracer
		{
			get
			{
				if (ExTraceGlobals.dataAccessTracer == null)
				{
					ExTraceGlobals.dataAccessTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.dataAccessTracer;
			}
		}

		public static Trace WorkItemTracer
		{
			get
			{
				if (ExTraceGlobals.workItemTracer == null)
				{
					ExTraceGlobals.workItemTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.workItemTracer;
			}
		}

		public static Trace ManagedAvailabilityTracer
		{
			get
			{
				if (ExTraceGlobals.managedAvailabilityTracer == null)
				{
					ExTraceGlobals.managedAvailabilityTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.managedAvailabilityTracer;
			}
		}

		private static Guid componentGuid = new Guid("EAF36C57-87B9-4D84-B551-3537A14A62B8");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace coreTracer = null;

		private static Trace dataAccessTracer = null;

		private static Trace workItemTracer = null;

		private static Trace managedAvailabilityTracer = null;
	}
}
