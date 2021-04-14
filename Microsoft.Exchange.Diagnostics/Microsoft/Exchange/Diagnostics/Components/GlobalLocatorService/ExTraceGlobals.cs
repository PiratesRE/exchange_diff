using System;

namespace Microsoft.Exchange.Diagnostics.Components.GlobalLocatorService
{
	public static class ExTraceGlobals
	{
		public static Trace APITracer
		{
			get
			{
				if (ExTraceGlobals.aPITracer == null)
				{
					ExTraceGlobals.aPITracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aPITracer;
			}
		}

		private static Guid componentGuid = new Guid("11E1750A-9A85-4d08-BFDB-4BCFD5BA8645");

		private static Trace aPITracer = null;
	}
}
