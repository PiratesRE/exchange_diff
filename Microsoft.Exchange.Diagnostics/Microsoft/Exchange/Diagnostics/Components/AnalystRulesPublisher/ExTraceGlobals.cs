using System;

namespace Microsoft.Exchange.Diagnostics.Components.AnalystRulesPublisher
{
	public static class ExTraceGlobals
	{
		public static Trace AnalystRulesPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.analystRulesPublisherTracer == null)
				{
					ExTraceGlobals.analystRulesPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.analystRulesPublisherTracer;
			}
		}

		private static Guid componentGuid = new Guid("A6B672FA-759B-4090-B7B8-83F450061F2B");

		private static Trace analystRulesPublisherTracer = null;
	}
}
