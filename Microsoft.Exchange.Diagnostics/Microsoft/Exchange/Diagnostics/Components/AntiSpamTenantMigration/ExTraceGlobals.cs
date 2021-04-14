using System;

namespace Microsoft.Exchange.Diagnostics.Components.AntiSpamTenantMigration
{
	public static class ExTraceGlobals
	{
		public static Trace AntiSpamTenantMigrationTracer
		{
			get
			{
				if (ExTraceGlobals.antiSpamTenantMigrationTracer == null)
				{
					ExTraceGlobals.antiSpamTenantMigrationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.antiSpamTenantMigrationTracer;
			}
		}

		private static Guid componentGuid = new Guid("8B9CF4C9-EFC6-44E5-B0C0-795992D39DB9");

		private static Trace antiSpamTenantMigrationTracer = null;
	}
}
