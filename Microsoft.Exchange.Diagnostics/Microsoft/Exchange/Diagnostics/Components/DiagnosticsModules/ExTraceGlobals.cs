using System;

namespace Microsoft.Exchange.Diagnostics.Components.DiagnosticsModules
{
	public static class ExTraceGlobals
	{
		public static Trace ErrorLoggingModuleTracer
		{
			get
			{
				if (ExTraceGlobals.errorLoggingModuleTracer == null)
				{
					ExTraceGlobals.errorLoggingModuleTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.errorLoggingModuleTracer;
			}
		}

		public static Trace ClientDiagnosticsModuleTracer
		{
			get
			{
				if (ExTraceGlobals.clientDiagnosticsModuleTracer == null)
				{
					ExTraceGlobals.clientDiagnosticsModuleTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.clientDiagnosticsModuleTracer;
			}
		}

		private static Guid componentGuid = new Guid("B79CCE07-AFC0-40CA-A6AD-4FB725D5770A");

		private static Trace errorLoggingModuleTracer = null;

		private static Trace clientDiagnosticsModuleTracer = null;
	}
}
