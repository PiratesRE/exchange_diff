using System;

namespace Microsoft.Exchange.Diagnostics.Components.FFOEmailUtil
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

		private static Guid componentGuid = new Guid("ada8a02a-ed68-4c8f-9269-a96fa2e4654d");

		private static Trace commonTracer = null;
	}
}
