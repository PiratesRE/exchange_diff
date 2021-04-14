using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp
{
	public static class ExTraceGlobals
	{
		public static Trace RpcBufferTracer
		{
			get
			{
				if (ExTraceGlobals.rpcBufferTracer == null)
				{
					ExTraceGlobals.rpcBufferTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rpcBufferTracer;
			}
		}

		public static Trace RpcOperationTracer
		{
			get
			{
				if (ExTraceGlobals.rpcOperationTracer == null)
				{
					ExTraceGlobals.rpcOperationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.rpcOperationTracer;
			}
		}

		public static Trace RpcDetailTracer
		{
			get
			{
				if (ExTraceGlobals.rpcDetailTracer == null)
				{
					ExTraceGlobals.rpcDetailTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.rpcDetailTracer;
			}
		}

		public static Trace RopTimingTracer
		{
			get
			{
				if (ExTraceGlobals.ropTimingTracer == null)
				{
					ExTraceGlobals.ropTimingTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.ropTimingTracer;
			}
		}

		public static Trace RpcContextPoolTracer
		{
			get
			{
				if (ExTraceGlobals.rpcContextPoolTracer == null)
				{
					ExTraceGlobals.rpcContextPoolTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.rpcContextPoolTracer;
			}
		}

		public static Trace SyncMailboxWithDSTracer
		{
			get
			{
				if (ExTraceGlobals.syncMailboxWithDSTracer == null)
				{
					ExTraceGlobals.syncMailboxWithDSTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.syncMailboxWithDSTracer;
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

		private static Guid componentGuid = new Guid("0df8b91e-45ef-41d3-bb91-b60a4446bb35");

		private static Trace rpcBufferTracer = null;

		private static Trace rpcOperationTracer = null;

		private static Trace rpcDetailTracer = null;

		private static Trace ropTimingTracer = null;

		private static Trace rpcContextPoolTracer = null;

		private static Trace syncMailboxWithDSTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
