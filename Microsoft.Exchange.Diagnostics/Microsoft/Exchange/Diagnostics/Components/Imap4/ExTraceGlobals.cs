using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Imap4
{
	public static class ExTraceGlobals
	{
		public static Trace ServerTracer
		{
			get
			{
				if (ExTraceGlobals.serverTracer == null)
				{
					ExTraceGlobals.serverTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serverTracer;
			}
		}

		public static Trace SessionTracer
		{
			get
			{
				if (ExTraceGlobals.sessionTracer == null)
				{
					ExTraceGlobals.sessionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.sessionTracer;
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

		private static Guid componentGuid = new Guid("B338D7C6-58F5-4523-B459-E387B7C956BA");

		private static Trace serverTracer = null;

		private static Trace sessionTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
