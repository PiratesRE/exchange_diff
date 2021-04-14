using System;

namespace Microsoft.Exchange.Diagnostics.Components.VisualizationFramework
{
	public static class ExTraceGlobals
	{
		public static Trace VisualizationFrameworkTracer
		{
			get
			{
				if (ExTraceGlobals.visualizationFrameworkTracer == null)
				{
					ExTraceGlobals.visualizationFrameworkTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.visualizationFrameworkTracer;
			}
		}

		private static Guid componentGuid = new Guid("1896490C-BFB1-473E-A9b8-40E1b86A880C");

		private static Trace visualizationFrameworkTracer = null;
	}
}
