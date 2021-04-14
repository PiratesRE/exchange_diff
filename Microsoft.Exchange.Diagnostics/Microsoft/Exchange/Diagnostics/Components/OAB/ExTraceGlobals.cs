using System;

namespace Microsoft.Exchange.Diagnostics.Components.OAB
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantTracer
		{
			get
			{
				if (ExTraceGlobals.assistantTracer == null)
				{
					ExTraceGlobals.assistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantTracer;
			}
		}

		public static Trace DataTracer
		{
			get
			{
				if (ExTraceGlobals.dataTracer == null)
				{
					ExTraceGlobals.dataTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dataTracer;
			}
		}

		public static Trace RunNowTracer
		{
			get
			{
				if (ExTraceGlobals.runNowTracer == null)
				{
					ExTraceGlobals.runNowTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.runNowTracer;
			}
		}

		public static Trace HttpHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.httpHandlerTracer == null)
				{
					ExTraceGlobals.httpHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.httpHandlerTracer;
			}
		}

		private static Guid componentGuid = new Guid("3934b4fd-72fb-44e1-b29a-6cac52257a5d");

		private static Trace assistantTracer = null;

		private static Trace dataTracer = null;

		private static Trace runNowTracer = null;

		private static Trace httpHandlerTracer = null;
	}
}
