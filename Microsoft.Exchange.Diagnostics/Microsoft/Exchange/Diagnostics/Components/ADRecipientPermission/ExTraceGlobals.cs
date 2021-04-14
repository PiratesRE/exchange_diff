using System;

namespace Microsoft.Exchange.Diagnostics.Components.ADRecipientPermission
{
	public static class ExTraceGlobals
	{
		public static Trace ADPermissionTracer
		{
			get
			{
				if (ExTraceGlobals.aDPermissionTracer == null)
				{
					ExTraceGlobals.aDPermissionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aDPermissionTracer;
			}
		}

		private static Guid componentGuid = new Guid("CA1F9293-1DBD-4762-895F-8A3DC190B239");

		private static Trace aDPermissionTracer = null;
	}
}
