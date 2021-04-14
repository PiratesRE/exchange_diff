using System;

namespace Microsoft.Exchange.Diagnostics.Components.Ehf
{
	public static class ExTraceGlobals
	{
		public static Trace ProviderTracer
		{
			get
			{
				if (ExTraceGlobals.providerTracer == null)
				{
					ExTraceGlobals.providerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.providerTracer;
			}
		}

		public static Trace TargetConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.targetConnectionTracer == null)
				{
					ExTraceGlobals.targetConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.targetConnectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("0b50faa8-d032-4a04-b40e-d774edf00c31");

		private static Trace providerTracer = null;

		private static Trace targetConnectionTracer = null;
	}
}
