using System;

namespace Microsoft.Exchange.Diagnostics.Components.FFODataPumper
{
	public static class ExTraceGlobals
	{
		public static Trace FFODataPumperTracer
		{
			get
			{
				if (ExTraceGlobals.fFODataPumperTracer == null)
				{
					ExTraceGlobals.fFODataPumperTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.fFODataPumperTracer;
			}
		}

		private static Guid componentGuid = new Guid("38489AA6-6BD4-448b-9FF9-7BFA6B80FC2B");

		private static Trace fFODataPumperTracer = null;
	}
}
