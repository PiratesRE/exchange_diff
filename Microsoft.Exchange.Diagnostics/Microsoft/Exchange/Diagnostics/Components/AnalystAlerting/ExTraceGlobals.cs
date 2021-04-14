using System;

namespace Microsoft.Exchange.Diagnostics.Components.AnalystAlerting
{
	public static class ExTraceGlobals
	{
		public static Trace AnalystAlertingTracer
		{
			get
			{
				if (ExTraceGlobals.analystAlertingTracer == null)
				{
					ExTraceGlobals.analystAlertingTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.analystAlertingTracer;
			}
		}

		private static Guid componentGuid = new Guid("DB7CA5BC-B68B-46DC-89AC-85572FBD89DD");

		private static Trace analystAlertingTracer = null;
	}
}
