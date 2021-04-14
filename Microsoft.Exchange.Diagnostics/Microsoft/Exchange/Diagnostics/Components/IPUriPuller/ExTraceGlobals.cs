using System;

namespace Microsoft.Exchange.Diagnostics.Components.IPUriPuller
{
	public static class ExTraceGlobals
	{
		public static Trace PullerTracer
		{
			get
			{
				if (ExTraceGlobals.pullerTracer == null)
				{
					ExTraceGlobals.pullerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.pullerTracer;
			}
		}

		private static Guid componentGuid = new Guid("D51325F9-4448-46B6-A151-148E015B1831");

		private static Trace pullerTracer = null;
	}
}
