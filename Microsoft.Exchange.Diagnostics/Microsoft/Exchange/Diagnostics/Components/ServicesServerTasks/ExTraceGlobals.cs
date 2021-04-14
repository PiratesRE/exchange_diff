using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServicesServerTasks
{
	public static class ExTraceGlobals
	{
		public static Trace TaskTracer
		{
			get
			{
				if (ExTraceGlobals.taskTracer == null)
				{
					ExTraceGlobals.taskTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.taskTracer;
			}
		}

		public static Trace NonTaskTracer
		{
			get
			{
				if (ExTraceGlobals.nonTaskTracer == null)
				{
					ExTraceGlobals.nonTaskTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.nonTaskTracer;
			}
		}

		private static Guid componentGuid = new Guid("DD7D3371-4EDD-4645-9BA9-F0532EA8C214");

		private static Trace taskTracer = null;

		private static Trace nonTaskTracer = null;
	}
}
