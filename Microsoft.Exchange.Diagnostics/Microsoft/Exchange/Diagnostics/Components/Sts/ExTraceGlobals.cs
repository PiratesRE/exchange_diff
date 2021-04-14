using System;

namespace Microsoft.Exchange.Diagnostics.Components.Sts
{
	public static class ExTraceGlobals
	{
		public static Trace DatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.databaseTracer == null)
				{
					ExTraceGlobals.databaseTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.databaseTracer;
			}
		}

		private static Guid componentGuid = new Guid("DEB97D6B-83F3-4002-8295-6BD0A2F71F18");

		private static Trace databaseTracer = null;
	}
}
