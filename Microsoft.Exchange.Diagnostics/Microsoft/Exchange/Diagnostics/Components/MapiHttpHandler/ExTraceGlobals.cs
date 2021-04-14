using System;

namespace Microsoft.Exchange.Diagnostics.Components.MapiHttpHandler
{
	public static class ExTraceGlobals
	{
		public static Trace AsyncOperationTracer
		{
			get
			{
				if (ExTraceGlobals.asyncOperationTracer == null)
				{
					ExTraceGlobals.asyncOperationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.asyncOperationTracer;
			}
		}

		public static Trace SessionContextTracer
		{
			get
			{
				if (ExTraceGlobals.sessionContextTracer == null)
				{
					ExTraceGlobals.sessionContextTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.sessionContextTracer;
			}
		}

		private static Guid componentGuid = new Guid("F998D96B-10B7-4D4F-94FA-D1A019D62326");

		private static Trace asyncOperationTracer = null;

		private static Trace sessionContextTracer = null;
	}
}
