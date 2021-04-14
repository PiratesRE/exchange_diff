using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.FfoAuthorization
{
	public static class ExTraceGlobals
	{
		public static Trace FfoRunspaceTracer
		{
			get
			{
				if (ExTraceGlobals.ffoRunspaceTracer == null)
				{
					ExTraceGlobals.ffoRunspaceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.ffoRunspaceTracer;
			}
		}

		public static Trace PartnerConfigTracer
		{
			get
			{
				if (ExTraceGlobals.partnerConfigTracer == null)
				{
					ExTraceGlobals.partnerConfigTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.partnerConfigTracer;
			}
		}

		public static Trace FfoRpsTracer
		{
			get
			{
				if (ExTraceGlobals.ffoRpsTracer == null)
				{
					ExTraceGlobals.ffoRpsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.ffoRpsTracer;
			}
		}

		public static Trace FfoRpsBudgetTracer
		{
			get
			{
				if (ExTraceGlobals.ffoRpsBudgetTracer == null)
				{
					ExTraceGlobals.ffoRpsBudgetTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.ffoRpsBudgetTracer;
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

		public static Trace FfoServicePlansTracer
		{
			get
			{
				if (ExTraceGlobals.ffoServicePlansTracer == null)
				{
					ExTraceGlobals.ffoServicePlansTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.ffoServicePlansTracer;
			}
		}

		private static Guid componentGuid = new Guid("2AEBD40A-8FA5-4159-A644-54F41B37D965");

		private static Trace ffoRunspaceTracer = null;

		private static Trace partnerConfigTracer = null;

		private static Trace ffoRpsTracer = null;

		private static Trace ffoRpsBudgetTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace ffoServicePlansTracer = null;
	}
}
