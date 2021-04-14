using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.OutlookService.Service
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

		public static Trace FrameworkTracer
		{
			get
			{
				if (ExTraceGlobals.frameworkTracer == null)
				{
					ExTraceGlobals.frameworkTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.frameworkTracer;
			}
		}

		public static Trace FeaturesTracer
		{
			get
			{
				if (ExTraceGlobals.featuresTracer == null)
				{
					ExTraceGlobals.featuresTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.featuresTracer;
			}
		}

		public static Trace StorageNotificationSubscriptionTracer
		{
			get
			{
				if (ExTraceGlobals.storageNotificationSubscriptionTracer == null)
				{
					ExTraceGlobals.storageNotificationSubscriptionTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.storageNotificationSubscriptionTracer;
			}
		}

		private static Guid componentGuid = new Guid("33858cdd-8b16-4201-8490-dc180f17036e");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace frameworkTracer = null;

		private static Trace featuresTracer = null;

		private static Trace storageNotificationSubscriptionTracer = null;
	}
}
