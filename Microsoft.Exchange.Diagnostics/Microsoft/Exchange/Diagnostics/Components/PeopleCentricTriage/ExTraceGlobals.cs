using System;

namespace Microsoft.Exchange.Diagnostics.Components.PeopleCentricTriage
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantTracer
		{
			get
			{
				if (ExTraceGlobals.assistantTracer == null)
				{
					ExTraceGlobals.assistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("4D9E2E62-6125-4C65-9C1D-ACDF3EE2E42E");

		private static Trace assistantTracer = null;
	}
}
