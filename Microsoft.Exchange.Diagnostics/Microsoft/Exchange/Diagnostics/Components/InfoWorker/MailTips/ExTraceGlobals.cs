using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips
{
	public static class ExTraceGlobals
	{
		public static Trace GetMailTipsConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.getMailTipsConfigurationTracer == null)
				{
					ExTraceGlobals.getMailTipsConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.getMailTipsConfigurationTracer;
			}
		}

		public static Trace GetMailTipsTracer
		{
			get
			{
				if (ExTraceGlobals.getMailTipsTracer == null)
				{
					ExTraceGlobals.getMailTipsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.getMailTipsTracer;
			}
		}

		public static Trace GroupMetricsTracer
		{
			get
			{
				if (ExTraceGlobals.groupMetricsTracer == null)
				{
					ExTraceGlobals.groupMetricsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.groupMetricsTracer;
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

		private static Guid componentGuid = new Guid("EF265C98-7258-4d64-B449-75B576D9A678");

		private static Trace getMailTipsConfigurationTracer = null;

		private static Trace getMailTipsTracer = null;

		private static Trace groupMetricsTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
