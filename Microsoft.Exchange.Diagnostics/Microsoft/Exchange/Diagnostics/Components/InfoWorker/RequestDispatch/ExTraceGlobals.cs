using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch
{
	public static class ExTraceGlobals
	{
		public static Trace RequestRoutingTracer
		{
			get
			{
				if (ExTraceGlobals.requestRoutingTracer == null)
				{
					ExTraceGlobals.requestRoutingTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.requestRoutingTracer;
			}
		}

		public static Trace DistributionListHandlingTracer
		{
			get
			{
				if (ExTraceGlobals.distributionListHandlingTracer == null)
				{
					ExTraceGlobals.distributionListHandlingTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.distributionListHandlingTracer;
			}
		}

		public static Trace ProxyWebRequestTracer
		{
			get
			{
				if (ExTraceGlobals.proxyWebRequestTracer == null)
				{
					ExTraceGlobals.proxyWebRequestTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.proxyWebRequestTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace GetFolderRequestTracer
		{
			get
			{
				if (ExTraceGlobals.getFolderRequestTracer == null)
				{
					ExTraceGlobals.getFolderRequestTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.getFolderRequestTracer;
			}
		}

		private static Guid componentGuid = new Guid("92915F00-6982-4d61-818A-6931EBA87182");

		private static Trace requestRoutingTracer = null;

		private static Trace distributionListHandlingTracer = null;

		private static Trace proxyWebRequestTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace getFolderRequestTracer = null;
	}
}
