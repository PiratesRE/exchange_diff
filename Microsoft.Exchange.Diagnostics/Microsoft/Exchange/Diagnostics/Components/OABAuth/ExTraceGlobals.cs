using System;

namespace Microsoft.Exchange.Diagnostics.Components.OABAuth
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace RPCTracer
		{
			get
			{
				if (ExTraceGlobals.rPCTracer == null)
				{
					ExTraceGlobals.rPCTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.rPCTracer;
			}
		}

		private static Guid componentGuid = new Guid("a38f8e7a-27d6-4fee-a5a6-56c225bbd889");

		private static Trace generalTracer = null;

		private static Trace rPCTracer = null;
	}
}
