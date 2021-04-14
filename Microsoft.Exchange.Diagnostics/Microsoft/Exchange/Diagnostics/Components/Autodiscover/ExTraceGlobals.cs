using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Autodiscover
{
	public static class ExTraceGlobals
	{
		public static Trace FrameworkTracer
		{
			get
			{
				if (ExTraceGlobals.frameworkTracer == null)
				{
					ExTraceGlobals.frameworkTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.frameworkTracer;
			}
		}

		public static Trace OutlookProviderTracer
		{
			get
			{
				if (ExTraceGlobals.outlookProviderTracer == null)
				{
					ExTraceGlobals.outlookProviderTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.outlookProviderTracer;
			}
		}

		public static Trace MobileSyncProviderTracer
		{
			get
			{
				if (ExTraceGlobals.mobileSyncProviderTracer == null)
				{
					ExTraceGlobals.mobileSyncProviderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.mobileSyncProviderTracer;
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

		public static Trace AuthMetadataTracer
		{
			get
			{
				if (ExTraceGlobals.authMetadataTracer == null)
				{
					ExTraceGlobals.authMetadataTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.authMetadataTracer;
			}
		}

		private static Guid componentGuid = new Guid("B3E33516-3A9E-4fba-8469-A88ECCCCDCD1");

		private static Trace frameworkTracer = null;

		private static Trace outlookProviderTracer = null;

		private static Trace mobileSyncProviderTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace authMetadataTracer = null;
	}
}
