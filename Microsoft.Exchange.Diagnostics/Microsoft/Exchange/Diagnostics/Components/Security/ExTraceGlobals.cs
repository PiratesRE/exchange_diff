using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Security
{
	public static class ExTraceGlobals
	{
		public static Trace AuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.authenticationTracer == null)
				{
					ExTraceGlobals.authenticationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.authenticationTracer;
			}
		}

		public static Trace PartnerTokenTracer
		{
			get
			{
				if (ExTraceGlobals.partnerTokenTracer == null)
				{
					ExTraceGlobals.partnerTokenTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.partnerTokenTracer;
			}
		}

		public static Trace X509CertAuthTracer
		{
			get
			{
				if (ExTraceGlobals.x509CertAuthTracer == null)
				{
					ExTraceGlobals.x509CertAuthTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.x509CertAuthTracer;
			}
		}

		public static Trace OAuthTracer
		{
			get
			{
				if (ExTraceGlobals.oAuthTracer == null)
				{
					ExTraceGlobals.oAuthTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.oAuthTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace BackendRehydrationTracer
		{
			get
			{
				if (ExTraceGlobals.backendRehydrationTracer == null)
				{
					ExTraceGlobals.backendRehydrationTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.backendRehydrationTracer;
			}
		}

		private static Guid componentGuid = new Guid("5ce0dc7e-6229-4bd9-9464-c92d7813bc3b");

		private static Trace authenticationTracer = null;

		private static Trace partnerTokenTracer = null;

		private static Trace x509CertAuthTracer = null;

		private static Trace oAuthTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace backendRehydrationTracer = null;
	}
}
