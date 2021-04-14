using System;

namespace Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel
{
	public static class ExTraceGlobals
	{
		public static Trace EventLogTracer
		{
			get
			{
				if (ExTraceGlobals.eventLogTracer == null)
				{
					ExTraceGlobals.eventLogTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.eventLogTracer;
			}
		}

		public static Trace RBACTracer
		{
			get
			{
				if (ExTraceGlobals.rBACTracer == null)
				{
					ExTraceGlobals.rBACTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.rBACTracer;
			}
		}

		public static Trace ProxyTracer
		{
			get
			{
				if (ExTraceGlobals.proxyTracer == null)
				{
					ExTraceGlobals.proxyTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.proxyTracer;
			}
		}

		public static Trace RedirectTracer
		{
			get
			{
				if (ExTraceGlobals.redirectTracer == null)
				{
					ExTraceGlobals.redirectTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.redirectTracer;
			}
		}

		public static Trace WebServiceTracer
		{
			get
			{
				if (ExTraceGlobals.webServiceTracer == null)
				{
					ExTraceGlobals.webServiceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.webServiceTracer;
			}
		}

		public static Trace PerformanceTracer
		{
			get
			{
				if (ExTraceGlobals.performanceTracer == null)
				{
					ExTraceGlobals.performanceTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.performanceTracer;
			}
		}

		public static Trace UserPhotosTracer
		{
			get
			{
				if (ExTraceGlobals.userPhotosTracer == null)
				{
					ExTraceGlobals.userPhotosTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.userPhotosTracer;
			}
		}

		public static Trace DDITracer
		{
			get
			{
				if (ExTraceGlobals.dDITracer == null)
				{
					ExTraceGlobals.dDITracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.dDITracer;
			}
		}

		public static Trace LinkedInTracer
		{
			get
			{
				if (ExTraceGlobals.linkedInTracer == null)
				{
					ExTraceGlobals.linkedInTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.linkedInTracer;
			}
		}

		private static Guid componentGuid = new Guid("EDD5672C-EB31-485A-9880-6E1F3BFCE4EB");

		private static Trace eventLogTracer = null;

		private static Trace rBACTracer = null;

		private static Trace proxyTracer = null;

		private static Trace redirectTracer = null;

		private static Trace webServiceTracer = null;

		private static Trace performanceTracer = null;

		private static Trace userPhotosTracer = null;

		private static Trace dDITracer = null;

		private static Trace linkedInTracer = null;
	}
}
