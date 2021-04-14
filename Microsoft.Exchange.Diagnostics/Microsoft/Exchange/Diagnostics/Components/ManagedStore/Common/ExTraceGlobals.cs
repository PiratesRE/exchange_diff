using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common
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

		public static Trace StatisticsTracer
		{
			get
			{
				if (ExTraceGlobals.statisticsTracer == null)
				{
					ExTraceGlobals.statisticsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.statisticsTracer;
			}
		}

		public static Trace ResetStatisticsTracer
		{
			get
			{
				if (ExTraceGlobals.resetStatisticsTracer == null)
				{
					ExTraceGlobals.resetStatisticsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.resetStatisticsTracer;
			}
		}

		public static Trace LockWaitTimeTracer
		{
			get
			{
				if (ExTraceGlobals.lockWaitTimeTracer == null)
				{
					ExTraceGlobals.lockWaitTimeTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.lockWaitTimeTracer;
			}
		}

		public static Trace TasksTracer
		{
			get
			{
				if (ExTraceGlobals.tasksTracer == null)
				{
					ExTraceGlobals.tasksTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.tasksTracer;
			}
		}

		public static Trace ExceptionHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.exceptionHandlerTracer == null)
				{
					ExTraceGlobals.exceptionHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.exceptionHandlerTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.configurationTracer;
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

		public static Trace Buddy1Tracer
		{
			get
			{
				if (ExTraceGlobals.buddy1Tracer == null)
				{
					ExTraceGlobals.buddy1Tracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.buddy1Tracer;
			}
		}

		public static Trace Buddy2Tracer
		{
			get
			{
				if (ExTraceGlobals.buddy2Tracer == null)
				{
					ExTraceGlobals.buddy2Tracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.buddy2Tracer;
			}
		}

		public static Trace Buddy3Tracer
		{
			get
			{
				if (ExTraceGlobals.buddy3Tracer == null)
				{
					ExTraceGlobals.buddy3Tracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.buddy3Tracer;
			}
		}

		public static Trace Buddy4Tracer
		{
			get
			{
				if (ExTraceGlobals.buddy4Tracer == null)
				{
					ExTraceGlobals.buddy4Tracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.buddy4Tracer;
			}
		}

		public static Trace Buddy5Tracer
		{
			get
			{
				if (ExTraceGlobals.buddy5Tracer == null)
				{
					ExTraceGlobals.buddy5Tracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.buddy5Tracer;
			}
		}

		private static Guid componentGuid = new Guid("b5c49b06-9eda-4e9d-b5b0-d696691fe1a7");

		private static Trace generalTracer = null;

		private static Trace statisticsTracer = null;

		private static Trace resetStatisticsTracer = null;

		private static Trace lockWaitTimeTracer = null;

		private static Trace tasksTracer = null;

		private static Trace exceptionHandlerTracer = null;

		private static Trace configurationTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace buddy1Tracer = null;

		private static Trace buddy2Tracer = null;

		private static Trace buddy3Tracer = null;

		private static Trace buddy4Tracer = null;

		private static Trace buddy5Tracer = null;
	}
}
