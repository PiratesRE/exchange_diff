using System;

namespace Microsoft.Exchange.Diagnostics.Components.ThrottlingService
{
	public static class ExTraceGlobals
	{
		public static Trace ThrottlingServiceTracer
		{
			get
			{
				if (ExTraceGlobals.throttlingServiceTracer == null)
				{
					ExTraceGlobals.throttlingServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.throttlingServiceTracer;
			}
		}

		public static Trace ThrottlingClientTracer
		{
			get
			{
				if (ExTraceGlobals.throttlingClientTracer == null)
				{
					ExTraceGlobals.throttlingClientTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.throttlingClientTracer;
			}
		}

		public static Trace ExportTracer
		{
			get
			{
				if (ExTraceGlobals.exportTracer == null)
				{
					ExTraceGlobals.exportTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.exportTracer;
			}
		}

		private static Guid componentGuid = new Guid("2e888ec1-6dd9-48cb-aa14-5bf7cad71a88");

		private static Trace throttlingServiceTracer = null;

		private static Trace throttlingClientTracer = null;

		private static Trace exportTracer = null;
	}
}
