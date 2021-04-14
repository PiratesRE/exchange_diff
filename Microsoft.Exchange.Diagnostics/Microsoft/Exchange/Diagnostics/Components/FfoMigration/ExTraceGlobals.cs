using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.FfoMigration
{
	public static class ExTraceGlobals
	{
		public static Trace PowershellProviderTracer
		{
			get
			{
				if (ExTraceGlobals.powershellProviderTracer == null)
				{
					ExTraceGlobals.powershellProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.powershellProviderTracer;
			}
		}

		public static Trace MigrationWorkflowTracer
		{
			get
			{
				if (ExTraceGlobals.migrationWorkflowTracer == null)
				{
					ExTraceGlobals.migrationWorkflowTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.migrationWorkflowTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("C7BDFB80-A905-4da5-B7AF-B36A79AD2182");

		private static Trace powershellProviderTracer = null;

		private static Trace migrationWorkflowTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
