using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.WorkloadManagement
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace ExecutionTracer
		{
			get
			{
				if (ExTraceGlobals.executionTracer == null)
				{
					ExTraceGlobals.executionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.executionTracer;
			}
		}

		public static Trace SchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.schedulerTracer == null)
				{
					ExTraceGlobals.schedulerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.schedulerTracer;
			}
		}

		public static Trace PoliciesTracer
		{
			get
			{
				if (ExTraceGlobals.policiesTracer == null)
				{
					ExTraceGlobals.policiesTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.policiesTracer;
			}
		}

		public static Trace ActivityContextTracer
		{
			get
			{
				if (ExTraceGlobals.activityContextTracer == null)
				{
					ExTraceGlobals.activityContextTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.activityContextTracer;
			}
		}

		public static Trace UserWorkloadManagerTracer
		{
			get
			{
				if (ExTraceGlobals.userWorkloadManagerTracer == null)
				{
					ExTraceGlobals.userWorkloadManagerTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.userWorkloadManagerTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace AdmissionControlTracer
		{
			get
			{
				if (ExTraceGlobals.admissionControlTracer == null)
				{
					ExTraceGlobals.admissionControlTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.admissionControlTracer;
			}
		}

		private static Guid componentGuid = new Guid("488b469c-d752-4650-8655-28590e044606");

		private static Trace commonTracer = null;

		private static Trace executionTracer = null;

		private static Trace schedulerTracer = null;

		private static Trace policiesTracer = null;

		private static Trace activityContextTracer = null;

		private static Trace userWorkloadManagerTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace admissionControlTracer = null;
	}
}
