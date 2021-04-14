using System;

namespace Microsoft.Exchange.Diagnostics.Components.AntiSpamDataMigration
{
	public static class ExTraceGlobals
	{
		public static Trace AntiSpamDataMigrationTracer
		{
			get
			{
				if (ExTraceGlobals.antiSpamDataMigrationTracer == null)
				{
					ExTraceGlobals.antiSpamDataMigrationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.antiSpamDataMigrationTracer;
			}
		}

		private static Guid componentGuid = new Guid("6991CBA3-1062-49EE-8AD4-A160A8205EF8");

		private static Trace antiSpamDataMigrationTracer = null;
	}
}
