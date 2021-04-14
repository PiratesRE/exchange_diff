using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.ServiceAccount
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

		private static Guid componentGuid = new Guid("76986af5-10f0-40d2-aac4-62e85132c65a");

		private static Trace generalTracer = null;
	}
}
