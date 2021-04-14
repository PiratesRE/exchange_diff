using System;

namespace Microsoft.Exchange.Diagnostics.Components.AntispamCommon
{
	public static class ExTraceGlobals
	{
		public static Trace UtilitiesTracer
		{
			get
			{
				if (ExTraceGlobals.utilitiesTracer == null)
				{
					ExTraceGlobals.utilitiesTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.utilitiesTracer;
			}
		}

		private static Guid componentGuid = new Guid("99F80BFB-F09F-41C9-8D3E-89A1C6CD9CD1");

		private static Trace utilitiesTracer = null;
	}
}
