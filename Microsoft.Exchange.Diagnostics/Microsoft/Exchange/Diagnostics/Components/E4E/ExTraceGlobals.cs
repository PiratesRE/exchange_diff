using System;

namespace Microsoft.Exchange.Diagnostics.Components.E4E
{
	public static class ExTraceGlobals
	{
		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		private static Guid componentGuid = new Guid("13E154E9-F834-4B1C-8FD7-81AA1B37F6AE");

		private static Trace coreTracer = null;
	}
}
