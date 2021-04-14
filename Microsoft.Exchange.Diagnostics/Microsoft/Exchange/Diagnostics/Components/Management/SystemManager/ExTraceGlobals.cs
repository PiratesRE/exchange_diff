using System;

namespace Microsoft.Exchange.Diagnostics.Components.Management.SystemManager
{
	public static class ExTraceGlobals
	{
		public static Trace ProgramFlowTracer
		{
			get
			{
				if (ExTraceGlobals.programFlowTracer == null)
				{
					ExTraceGlobals.programFlowTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.programFlowTracer;
			}
		}

		public static Trace DataFlowTracer
		{
			get
			{
				if (ExTraceGlobals.dataFlowTracer == null)
				{
					ExTraceGlobals.dataFlowTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dataFlowTracer;
			}
		}

		public static Trace LayoutTracer
		{
			get
			{
				if (ExTraceGlobals.layoutTracer == null)
				{
					ExTraceGlobals.layoutTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.layoutTracer;
			}
		}

		private static Guid componentGuid = new Guid("D92207E8-DBF2-4a93-B4F9-4931434E5F96");

		private static Trace programFlowTracer = null;

		private static Trace dataFlowTracer = null;

		private static Trace layoutTracer = null;
	}
}
