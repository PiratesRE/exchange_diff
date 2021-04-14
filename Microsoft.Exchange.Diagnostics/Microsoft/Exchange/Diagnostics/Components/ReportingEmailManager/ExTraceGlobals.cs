using System;

namespace Microsoft.Exchange.Diagnostics.Components.ReportingEmailManager
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		private static Guid componentGuid = new Guid("be2f13aa-6b0e-40b0-8612-74f560f2a53c");

		private static Trace commonTracer = null;
	}
}
