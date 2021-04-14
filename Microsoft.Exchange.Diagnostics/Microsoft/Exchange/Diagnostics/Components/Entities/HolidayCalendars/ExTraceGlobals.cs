using System;

namespace Microsoft.Exchange.Diagnostics.Components.Entities.HolidayCalendars
{
	public static class ExTraceGlobals
	{
		public static Trace ConfigurationCacheTracer
		{
			get
			{
				if (ExTraceGlobals.configurationCacheTracer == null)
				{
					ExTraceGlobals.configurationCacheTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.configurationCacheTracer;
			}
		}

		public static Trace EndpointConfigurationRetrieverTracer
		{
			get
			{
				if (ExTraceGlobals.endpointConfigurationRetrieverTracer == null)
				{
					ExTraceGlobals.endpointConfigurationRetrieverTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.endpointConfigurationRetrieverTracer;
			}
		}

		private static Guid componentGuid = new Guid("B8764FA6-79B0-42B6-8209-17E80F43CA43");

		private static Trace configurationCacheTracer = null;

		private static Trace endpointConfigurationRetrieverTracer = null;
	}
}
