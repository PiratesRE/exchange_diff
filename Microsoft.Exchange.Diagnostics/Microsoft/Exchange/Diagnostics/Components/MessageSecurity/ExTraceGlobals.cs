using System;

namespace Microsoft.Exchange.Diagnostics.Components.MessageSecurity
{
	public static class ExTraceGlobals
	{
		public static Trace EdgeCredentialServiceTracer
		{
			get
			{
				if (ExTraceGlobals.edgeCredentialServiceTracer == null)
				{
					ExTraceGlobals.edgeCredentialServiceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.edgeCredentialServiceTracer;
			}
		}

		public static Trace TasksTracer
		{
			get
			{
				if (ExTraceGlobals.tasksTracer == null)
				{
					ExTraceGlobals.tasksTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.tasksTracer;
			}
		}

		private static Guid componentGuid = new Guid("C03489AA-60B3-4417-ABD0-67A4EA779033");

		private static Trace edgeCredentialServiceTracer = null;

		private static Trace tasksTracer = null;
	}
}
