using System;

namespace Microsoft.Exchange.Diagnostics.Components.EdgeSync
{
	public static class ExTraceGlobals
	{
		public static Trace ProcessTracer
		{
			get
			{
				if (ExTraceGlobals.processTracer == null)
				{
					ExTraceGlobals.processTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.processTracer;
			}
		}

		public static Trace ConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.connectionTracer == null)
				{
					ExTraceGlobals.connectionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.connectionTracer;
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

		public static Trace SyncNowTracer
		{
			get
			{
				if (ExTraceGlobals.syncNowTracer == null)
				{
					ExTraceGlobals.syncNowTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.syncNowTracer;
			}
		}

		public static Trace TopologyTracer
		{
			get
			{
				if (ExTraceGlobals.topologyTracer == null)
				{
					ExTraceGlobals.topologyTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.topologyTracer;
			}
		}

		public static Trace SynchronizationJobTracer
		{
			get
			{
				if (ExTraceGlobals.synchronizationJobTracer == null)
				{
					ExTraceGlobals.synchronizationJobTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.synchronizationJobTracer;
			}
		}

		public static Trace SubscriptionTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionTracer == null)
				{
					ExTraceGlobals.subscriptionTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.subscriptionTracer;
			}
		}

		private static Guid componentGuid = new Guid("AB9C28FE-50E0-4907-BB41-8F82D8E0C068");

		private static Trace processTracer = null;

		private static Trace connectionTracer = null;

		private static Trace schedulerTracer = null;

		private static Trace syncNowTracer = null;

		private static Trace topologyTracer = null;

		private static Trace synchronizationJobTracer = null;

		private static Trace subscriptionTracer = null;
	}
}
