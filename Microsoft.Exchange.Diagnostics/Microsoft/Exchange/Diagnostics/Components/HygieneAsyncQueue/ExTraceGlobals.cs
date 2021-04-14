using System;

namespace Microsoft.Exchange.Diagnostics.Components.HygieneAsyncQueue
{
	public static class ExTraceGlobals
	{
		public static Trace AsyncQueueServiceTracer
		{
			get
			{
				if (ExTraceGlobals.asyncQueueServiceTracer == null)
				{
					ExTraceGlobals.asyncQueueServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.asyncQueueServiceTracer;
			}
		}

		public static Trace AsyncQueueExecutorTracer
		{
			get
			{
				if (ExTraceGlobals.asyncQueueExecutorTracer == null)
				{
					ExTraceGlobals.asyncQueueExecutorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.asyncQueueExecutorTracer;
			}
		}

		public static Trace AsyncStepExecutorTracer
		{
			get
			{
				if (ExTraceGlobals.asyncStepExecutorTracer == null)
				{
					ExTraceGlobals.asyncStepExecutorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.asyncStepExecutorTracer;
			}
		}

		private static Guid componentGuid = new Guid("040DF3E7-309C-4531-A762-6136DBD1004A");

		private static Trace asyncQueueServiceTracer = null;

		private static Trace asyncQueueExecutorTracer = null;

		private static Trace asyncStepExecutorTracer = null;
	}
}
