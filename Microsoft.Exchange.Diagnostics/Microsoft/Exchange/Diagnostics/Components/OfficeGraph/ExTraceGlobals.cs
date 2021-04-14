using System;

namespace Microsoft.Exchange.Diagnostics.Components.OfficeGraph
{
	public static class ExTraceGlobals
	{
		public static Trace OfficeGraphAgentTracer
		{
			get
			{
				if (ExTraceGlobals.officeGraphAgentTracer == null)
				{
					ExTraceGlobals.officeGraphAgentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.officeGraphAgentTracer;
			}
		}

		public static Trace OfficeGraphWriterTracer
		{
			get
			{
				if (ExTraceGlobals.officeGraphWriterTracer == null)
				{
					ExTraceGlobals.officeGraphWriterTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.officeGraphWriterTracer;
			}
		}

		private static Guid componentGuid = new Guid("2F0CCE12-0EF1-4AA2-808D-7827E3E21306");

		private static Trace officeGraphAgentTracer = null;

		private static Trace officeGraphWriterTracer = null;
	}
}
