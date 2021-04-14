using System;

namespace Microsoft.Exchange.Diagnostics.Components.EngineUpdate
{
	public static class ExTraceGlobals
	{
		public static Trace LoggingLibTracer
		{
			get
			{
				if (ExTraceGlobals.loggingLibTracer == null)
				{
					ExTraceGlobals.loggingLibTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.loggingLibTracer;
			}
		}

		public static Trace LoggerTracer
		{
			get
			{
				if (ExTraceGlobals.loggerTracer == null)
				{
					ExTraceGlobals.loggerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.loggerTracer;
			}
		}

		private static Guid componentGuid = new Guid("4A97B20E-92F6-4960-8273-E069E7785615");

		private static Trace loggingLibTracer = null;

		private static Trace loggerTracer = null;
	}
}
