using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Monitoring
{
	public static class ExTraceGlobals
	{
		public static Trace MonitoringServiceTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringServiceTracer == null)
				{
					ExTraceGlobals.monitoringServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.monitoringServiceTracer;
			}
		}

		public static Trace MonitoringRpcServerTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringRpcServerTracer == null)
				{
					ExTraceGlobals.monitoringRpcServerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.monitoringRpcServerTracer;
			}
		}

		public static Trace MonitoringTasksTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringTasksTracer == null)
				{
					ExTraceGlobals.monitoringTasksTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.monitoringTasksTracer;
			}
		}

		public static Trace MonitoringHelperTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringHelperTracer == null)
				{
					ExTraceGlobals.monitoringHelperTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.monitoringHelperTracer;
			}
		}

		public static Trace MonitoringDataTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringDataTracer == null)
				{
					ExTraceGlobals.monitoringDataTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.monitoringDataTracer;
			}
		}

		public static Trace CorrelationEngineTracer
		{
			get
			{
				if (ExTraceGlobals.correlationEngineTracer == null)
				{
					ExTraceGlobals.correlationEngineTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.correlationEngineTracer;
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

		private static Guid componentGuid = new Guid("170506F7-64BA-4C74-A2A3-0A5CC247DB58");

		private static Trace monitoringServiceTracer = null;

		private static Trace monitoringRpcServerTracer = null;

		private static Trace monitoringTasksTracer = null;

		private static Trace monitoringHelperTracer = null;

		private static Trace monitoringDataTracer = null;

		private static Trace correlationEngineTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
