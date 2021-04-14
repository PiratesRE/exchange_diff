using System;

namespace Microsoft.Exchange.Diagnostics.Components.Cluster.DagManagement
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace DatabaseHealthTrackerTracer
		{
			get
			{
				if (ExTraceGlobals.databaseHealthTrackerTracer == null)
				{
					ExTraceGlobals.databaseHealthTrackerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.databaseHealthTrackerTracer;
			}
		}

		public static Trace MonitoringWcfServiceTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringWcfServiceTracer == null)
				{
					ExTraceGlobals.monitoringWcfServiceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.monitoringWcfServiceTracer;
			}
		}

		public static Trace MonitoringWcfClientTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringWcfClientTracer == null)
				{
					ExTraceGlobals.monitoringWcfClientTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.monitoringWcfClientTracer;
			}
		}

		private static Guid componentGuid = new Guid("3ce77de7-c264-4d85-96f6-d0c3b66d4a4b");

		private static Trace serviceTracer = null;

		private static Trace databaseHealthTrackerTracer = null;

		private static Trace monitoringWcfServiceTracer = null;

		private static Trace monitoringWcfClientTracer = null;
	}
}
