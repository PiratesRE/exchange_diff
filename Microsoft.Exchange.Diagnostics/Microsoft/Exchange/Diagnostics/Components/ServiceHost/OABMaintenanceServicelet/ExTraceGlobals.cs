using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.OABMaintenanceServicelet
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceletFrameworkTracer
		{
			get
			{
				if (ExTraceGlobals.serviceletFrameworkTracer == null)
				{
					ExTraceGlobals.serviceletFrameworkTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceletFrameworkTracer;
			}
		}

		public static Trace RecoverOrphanedOABsTaskTracer
		{
			get
			{
				if (ExTraceGlobals.recoverOrphanedOABsTaskTracer == null)
				{
					ExTraceGlobals.recoverOrphanedOABsTaskTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.recoverOrphanedOABsTaskTracer;
			}
		}

		private static Guid componentGuid = new Guid("26A265F6-54FB-4345-A280-DBD4A1A62EE4");

		private static Trace serviceletFrameworkTracer = null;

		private static Trace recoverOrphanedOABsTaskTracer = null;
	}
}
