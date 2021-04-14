using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("B14FEEA6-0168-427D-AB88-5F88812B99C1");

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
