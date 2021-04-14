using System;

namespace Microsoft.Exchange.Diagnostics.Components.ContentFilter
{
	public static class ExTraceGlobals
	{
		public static Trace InitializationTracer
		{
			get
			{
				if (ExTraceGlobals.initializationTracer == null)
				{
					ExTraceGlobals.initializationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.initializationTracer;
			}
		}

		public static Trace ScanMessageTracer
		{
			get
			{
				if (ExTraceGlobals.scanMessageTracer == null)
				{
					ExTraceGlobals.scanMessageTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.scanMessageTracer;
			}
		}

		public static Trace BypassedSendersTracer
		{
			get
			{
				if (ExTraceGlobals.bypassedSendersTracer == null)
				{
					ExTraceGlobals.bypassedSendersTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.bypassedSendersTracer;
			}
		}

		public static Trace ComInteropTracer
		{
			get
			{
				if (ExTraceGlobals.comInteropTracer == null)
				{
					ExTraceGlobals.comInteropTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.comInteropTracer;
			}
		}

		private static Guid componentGuid = new Guid("A1FD20D2-933F-4505-A0C4-C1FBFFCB9E62");

		private static Trace initializationTracer = null;

		private static Trace scanMessageTracer = null;

		private static Trace bypassedSendersTracer = null;

		private static Trace comInteropTracer = null;
	}
}
