using System;

namespace Microsoft.Exchange.Diagnostics.Components.DelegatedAuthentication
{
	public static class ExTraceGlobals
	{
		public static Trace DelegatedAuthTracer
		{
			get
			{
				if (ExTraceGlobals.delegatedAuthTracer == null)
				{
					ExTraceGlobals.delegatedAuthTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.delegatedAuthTracer;
			}
		}

		private static Guid componentGuid = new Guid("d05b1f84-6a69-45ca-887c-60d0e07efb93");

		private static Trace delegatedAuthTracer = null;
	}
}
