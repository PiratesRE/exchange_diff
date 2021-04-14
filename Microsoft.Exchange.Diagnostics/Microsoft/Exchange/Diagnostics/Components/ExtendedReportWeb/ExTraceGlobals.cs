using System;

namespace Microsoft.Exchange.Diagnostics.Components.ExtendedReportWeb
{
	public static class ExTraceGlobals
	{
		public static Trace ExtendedReportWebTracer
		{
			get
			{
				if (ExTraceGlobals.extendedReportWebTracer == null)
				{
					ExTraceGlobals.extendedReportWebTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.extendedReportWebTracer;
			}
		}

		private static Guid componentGuid = new Guid("5d38a9b6-081e-4f9e-992a-4f818692c421");

		private static Trace extendedReportWebTracer = null;
	}
}
