using System;

namespace Microsoft.Exchange.Diagnostics.Components.Approval.Common
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		private static Guid componentGuid = new Guid("DEFD64F3-201F-4cf5-A1A4-B949C647C287");

		private static Trace generalTracer = null;
	}
}
