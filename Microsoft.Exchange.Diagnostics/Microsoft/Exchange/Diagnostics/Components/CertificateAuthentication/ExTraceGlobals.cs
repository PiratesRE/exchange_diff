using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.CertificateAuthentication
{
	public static class ExTraceGlobals
	{
		public static Trace CertAuthTracer
		{
			get
			{
				if (ExTraceGlobals.certAuthTracer == null)
				{
					ExTraceGlobals.certAuthTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.certAuthTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("39942ef4-b83c-426d-b492-ba040437091a");

		private static Trace certAuthTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
