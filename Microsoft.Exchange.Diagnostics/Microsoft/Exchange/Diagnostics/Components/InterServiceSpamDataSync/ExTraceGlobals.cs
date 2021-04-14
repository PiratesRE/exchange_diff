using System;

namespace Microsoft.Exchange.Diagnostics.Components.InterServiceSpamDataSync
{
	public static class ExTraceGlobals
	{
		public static Trace InterServiceSpamDataSyncTracer
		{
			get
			{
				if (ExTraceGlobals.interServiceSpamDataSyncTracer == null)
				{
					ExTraceGlobals.interServiceSpamDataSyncTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.interServiceSpamDataSyncTracer;
			}
		}

		private static Guid componentGuid = new Guid("D9830C82-1661-41A3-8F68-674D885D055E");

		private static Trace interServiceSpamDataSyncTracer = null;
	}
}
