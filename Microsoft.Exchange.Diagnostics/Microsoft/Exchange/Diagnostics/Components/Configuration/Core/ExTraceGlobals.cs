using System;

namespace Microsoft.Exchange.Diagnostics.Components.Configuration.Core
{
	public static class ExTraceGlobals
	{
		public static Trace HttpModuleTracer
		{
			get
			{
				if (ExTraceGlobals.httpModuleTracer == null)
				{
					ExTraceGlobals.httpModuleTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.httpModuleTracer;
			}
		}

		public static Trace InstrumentationTracer
		{
			get
			{
				if (ExTraceGlobals.instrumentationTracer == null)
				{
					ExTraceGlobals.instrumentationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.instrumentationTracer;
			}
		}

		public static Trace UserTokenTracer
		{
			get
			{
				if (ExTraceGlobals.userTokenTracer == null)
				{
					ExTraceGlobals.userTokenTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.userTokenTracer;
			}
		}

		private static Guid componentGuid = new Guid("0377C8B8-BA48-4B10-9D07-F1F3E5636ED4");

		private static Trace httpModuleTracer = null;

		private static Trace instrumentationTracer = null;

		private static Trace userTokenTracer = null;
	}
}
