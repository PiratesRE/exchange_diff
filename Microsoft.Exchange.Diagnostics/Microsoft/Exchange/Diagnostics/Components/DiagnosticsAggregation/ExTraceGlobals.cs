using System;

namespace Microsoft.Exchange.Diagnostics.Components.DiagnosticsAggregation
{
	public static class ExTraceGlobals
	{
		public static Trace DiagnosticsAggregationTracer
		{
			get
			{
				if (ExTraceGlobals.diagnosticsAggregationTracer == null)
				{
					ExTraceGlobals.diagnosticsAggregationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.diagnosticsAggregationTracer;
			}
		}

		private static Guid componentGuid = new Guid("3e5245bb-9b29-457c-9cbf-83294dcb9a64");

		private static Trace diagnosticsAggregationTracer = null;
	}
}
