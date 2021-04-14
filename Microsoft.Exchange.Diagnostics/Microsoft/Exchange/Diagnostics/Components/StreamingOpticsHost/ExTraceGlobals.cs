using System;

namespace Microsoft.Exchange.Diagnostics.Components.StreamingOpticsHost
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

		private static Guid componentGuid = new Guid("05238676-6E91-4730-B8B6-2D891A4A0E85");

		private static Trace generalTracer = null;
	}
}
