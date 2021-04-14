using System;

namespace Microsoft.Exchange.Diagnostics.Components.Reconciliation
{
	public static class ExTraceGlobals
	{
		public static PerfTrace StartProcessingMessagePerfTracer
		{
			get
			{
				if (ExTraceGlobals.startProcessingMessagePerfTracer == null)
				{
					ExTraceGlobals.startProcessingMessagePerfTracer = new PerfTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.startProcessingMessagePerfTracer;
			}
		}

		public static PerfTrace EndProcessingMessagePerfTracer
		{
			get
			{
				if (ExTraceGlobals.endProcessingMessagePerfTracer == null)
				{
					ExTraceGlobals.endProcessingMessagePerfTracer = new PerfTrace(ExTraceGlobals.componentGuid, 255);
				}
				return ExTraceGlobals.endProcessingMessagePerfTracer;
			}
		}

		private static Guid componentGuid = new Guid("E06E0123-1B5C-4f61-959D-8258BF6C689A");

		private static PerfTrace startProcessingMessagePerfTracer = null;

		private static PerfTrace endProcessingMessagePerfTracer = null;
	}
}
