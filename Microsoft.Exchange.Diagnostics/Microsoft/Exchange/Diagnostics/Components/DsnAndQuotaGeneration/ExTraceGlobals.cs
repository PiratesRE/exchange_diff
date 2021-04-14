using System;

namespace Microsoft.Exchange.Diagnostics.Components.DsnAndQuotaGeneration
{
	public static class ExTraceGlobals
	{
		public static Trace DsnTracer
		{
			get
			{
				if (ExTraceGlobals.dsnTracer == null)
				{
					ExTraceGlobals.dsnTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dsnTracer;
			}
		}

		private static Guid componentGuid = new Guid("D15942F5-51BD-41f5-956D-1E47626ADFB6");

		private static Trace dsnTracer = null;
	}
}
