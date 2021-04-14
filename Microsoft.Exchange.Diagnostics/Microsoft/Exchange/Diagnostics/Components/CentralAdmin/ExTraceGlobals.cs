using System;

namespace Microsoft.Exchange.Diagnostics.Components.CentralAdmin
{
	public static class ExTraceGlobals
	{
		public static Trace CentralAdminServiceTracer
		{
			get
			{
				if (ExTraceGlobals.centralAdminServiceTracer == null)
				{
					ExTraceGlobals.centralAdminServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.centralAdminServiceTracer;
			}
		}

		public static Trace MOMConnectorTracer
		{
			get
			{
				if (ExTraceGlobals.mOMConnectorTracer == null)
				{
					ExTraceGlobals.mOMConnectorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mOMConnectorTracer;
			}
		}

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace UtilitiesTracer
		{
			get
			{
				if (ExTraceGlobals.utilitiesTracer == null)
				{
					ExTraceGlobals.utilitiesTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.utilitiesTracer;
			}
		}

		private static Guid componentGuid = new Guid("e84c009b-68df-4514-a1f4-498aab784af1");

		private static Trace centralAdminServiceTracer = null;

		private static Trace mOMConnectorTracer = null;

		private static Trace commonTracer = null;

		private static Trace utilitiesTracer = null;
	}
}
