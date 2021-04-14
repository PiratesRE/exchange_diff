using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService
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

		public static Trace ClientTracer
		{
			get
			{
				if (ExTraceGlobals.clientTracer == null)
				{
					ExTraceGlobals.clientTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.clientTracer;
			}
		}

		public static Trace WCFServiceEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.wCFServiceEndpointTracer == null)
				{
					ExTraceGlobals.wCFServiceEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.wCFServiceEndpointTracer;
			}
		}

		public static Trace WCFClientEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.wCFClientEndpointTracer == null)
				{
					ExTraceGlobals.wCFClientEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.wCFClientEndpointTracer;
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

		public static Trace SuitabilityVerifierTracer
		{
			get
			{
				if (ExTraceGlobals.suitabilityVerifierTracer == null)
				{
					ExTraceGlobals.suitabilityVerifierTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.suitabilityVerifierTracer;
			}
		}

		public static Trace DiscoveryTracer
		{
			get
			{
				if (ExTraceGlobals.discoveryTracer == null)
				{
					ExTraceGlobals.discoveryTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.discoveryTracer;
			}
		}

		public static Trace DnsTroubleshooterTracer
		{
			get
			{
				if (ExTraceGlobals.dnsTroubleshooterTracer == null)
				{
					ExTraceGlobals.dnsTroubleshooterTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.dnsTroubleshooterTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("23c20436-ba78-481d-99c3-5c523ff23024");

		private static Trace serviceTracer = null;

		private static Trace clientTracer = null;

		private static Trace wCFServiceEndpointTracer = null;

		private static Trace wCFClientEndpointTracer = null;

		private static Trace topologyTracer = null;

		private static Trace suitabilityVerifierTracer = null;

		private static Trace discoveryTracer = null;

		private static Trace dnsTroubleshooterTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
