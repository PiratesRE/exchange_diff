using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent
{
	public static class ExTraceGlobals
	{
		public static Trace RusTracer
		{
			get
			{
				if (ExTraceGlobals.rusTracer == null)
				{
					ExTraceGlobals.rusTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rusTracer;
			}
		}

		public static Trace AdminAuditLogTracer
		{
			get
			{
				if (ExTraceGlobals.adminAuditLogTracer == null)
				{
					ExTraceGlobals.adminAuditLogTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.adminAuditLogTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("f0fd0248-ef90-4fad-8a53-a6a21ac5528c");

		private static Trace rusTracer = null;

		private static Trace adminAuditLogTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
