using System;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.Assistants.TopN
{
	public static class ExTraceGlobals
	{
		public static Trace TopNAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.topNAssistantTracer == null)
				{
					ExTraceGlobals.topNAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.topNAssistantTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		private static Guid componentGuid = new Guid("E97F40AF-7C47-4d35-86A0-0725D7B8019D");

		private static Trace topNAssistantTracer = null;

		private static Trace pFDTracer = null;
	}
}
