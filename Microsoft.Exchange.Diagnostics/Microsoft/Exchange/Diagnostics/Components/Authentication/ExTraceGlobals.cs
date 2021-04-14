using System;

namespace Microsoft.Exchange.Diagnostics.Components.Authentication
{
	public static class ExTraceGlobals
	{
		public static Trace DefaultTracer
		{
			get
			{
				if (ExTraceGlobals.defaultTracer == null)
				{
					ExTraceGlobals.defaultTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.defaultTracer;
			}
		}

		private static Guid componentGuid = new Guid("d7ac17d0-dee6-44c6-96e5-bdb65dd8efa3");

		private static Trace defaultTracer = null;
	}
}
