using System;

namespace Microsoft.Exchange.Diagnostics.Components.EseRepl
{
	public static class ExTraceGlobals
	{
		public static Trace DagNetEnvironmentTracer
		{
			get
			{
				if (ExTraceGlobals.dagNetEnvironmentTracer == null)
				{
					ExTraceGlobals.dagNetEnvironmentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dagNetEnvironmentTracer;
			}
		}

		public static Trace DagNetChooserTracer
		{
			get
			{
				if (ExTraceGlobals.dagNetChooserTracer == null)
				{
					ExTraceGlobals.dagNetChooserTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dagNetChooserTracer;
			}
		}

		private static Guid componentGuid = new Guid("173045e6-18eb-4bbe-907a-fc8170f0b837");

		private static Trace dagNetEnvironmentTracer = null;

		private static Trace dagNetChooserTracer = null;
	}
}
