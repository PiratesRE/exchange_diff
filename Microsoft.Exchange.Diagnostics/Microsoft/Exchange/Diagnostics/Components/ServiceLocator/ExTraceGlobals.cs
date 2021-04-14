using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceLocator
{
	public static class ExTraceGlobals
	{
		public static Trace FfoDnsServerCommonTracer
		{
			get
			{
				if (ExTraceGlobals.ffoDnsServerCommonTracer == null)
				{
					ExTraceGlobals.ffoDnsServerCommonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.ffoDnsServerCommonTracer;
			}
		}

		public static Trace FfoDnsServerTracer
		{
			get
			{
				if (ExTraceGlobals.ffoDnsServerTracer == null)
				{
					ExTraceGlobals.ffoDnsServerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.ffoDnsServerTracer;
			}
		}

		public static Trace FfoDnsServerDBPlugInTracer
		{
			get
			{
				if (ExTraceGlobals.ffoDnsServerDBPlugInTracer == null)
				{
					ExTraceGlobals.ffoDnsServerDBPlugInTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.ffoDnsServerDBPlugInTracer;
			}
		}

		private static Guid componentGuid = new Guid("9CCAE37E-338A-403B-9EBB-2636514DEE9C");

		private static Trace ffoDnsServerCommonTracer = null;

		private static Trace ffoDnsServerTracer = null;

		private static Trace ffoDnsServerDBPlugInTracer = null;
	}
}
