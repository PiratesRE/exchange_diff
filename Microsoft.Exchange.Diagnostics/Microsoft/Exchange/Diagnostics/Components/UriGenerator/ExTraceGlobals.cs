using System;

namespace Microsoft.Exchange.Diagnostics.Components.UriGenerator
{
	public static class ExTraceGlobals
	{
		public static Trace UriGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.uriGeneratorTracer == null)
				{
					ExTraceGlobals.uriGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.uriGeneratorTracer;
			}
		}

		private static Guid componentGuid = new Guid("FAE9B52D-2EC7-4168-9668-5A3B88DCAACA");

		private static Trace uriGeneratorTracer = null;
	}
}
