using System;

namespace Microsoft.Exchange.Diagnostics.Components.ADRecipientExpansion
{
	public static class ExTraceGlobals
	{
		public static Trace ADExpansionTracer
		{
			get
			{
				if (ExTraceGlobals.aDExpansionTracer == null)
				{
					ExTraceGlobals.aDExpansionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aDExpansionTracer;
			}
		}

		private static Guid componentGuid = new Guid("9e0cc833-7761-49ac-80cc-e2b9cf4d5b94");

		private static Trace aDExpansionTracer = null;
	}
}
