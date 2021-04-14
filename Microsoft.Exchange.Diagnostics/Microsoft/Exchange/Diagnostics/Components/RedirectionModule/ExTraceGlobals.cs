using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.RedirectionModule
{
	public static class ExTraceGlobals
	{
		public static Trace RedirectionTracer
		{
			get
			{
				if (ExTraceGlobals.redirectionTracer == null)
				{
					ExTraceGlobals.redirectionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.redirectionTracer;
			}
		}

		public static Trace ErrorReportingTracer
		{
			get
			{
				if (ExTraceGlobals.errorReportingTracer == null)
				{
					ExTraceGlobals.errorReportingTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.errorReportingTracer;
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

		public static Trace TenantMonitoringTracer
		{
			get
			{
				if (ExTraceGlobals.tenantMonitoringTracer == null)
				{
					ExTraceGlobals.tenantMonitoringTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.tenantMonitoringTracer;
			}
		}

		private static Guid componentGuid = new Guid("62a46310-1793-40b2-9f61-74bf8637fce6");

		private static Trace redirectionTracer = null;

		private static Trace errorReportingTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace tenantMonitoringTracer = null;
	}
}
