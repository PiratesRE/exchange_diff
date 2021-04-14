using System;

namespace Microsoft.Exchange.Diagnostics.Components.Data.Directory.ABProviderFramework
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

		public static Trace ActiveDirectoryTracer
		{
			get
			{
				if (ExTraceGlobals.activeDirectoryTracer == null)
				{
					ExTraceGlobals.activeDirectoryTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.activeDirectoryTracer;
			}
		}

		public static Trace ExchangeWebServicesTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeWebServicesTracer == null)
				{
					ExTraceGlobals.exchangeWebServicesTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.exchangeWebServicesTracer;
			}
		}

		public static Trace OwaUrlsTracer
		{
			get
			{
				if (ExTraceGlobals.owaUrlsTracer == null)
				{
					ExTraceGlobals.owaUrlsTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.owaUrlsTracer;
			}
		}

		private static Guid componentGuid = new Guid("9E009811-D5D4-434b-B1BC-85C64CE57046");

		private static Trace frameworkTracer = null;

		private static Trace activeDirectoryTracer = null;

		private static Trace exchangeWebServicesTracer = null;

		private static Trace owaUrlsTracer = null;
	}
}
