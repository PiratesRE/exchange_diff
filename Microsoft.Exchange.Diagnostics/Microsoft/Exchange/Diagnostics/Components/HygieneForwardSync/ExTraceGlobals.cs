using System;

namespace Microsoft.Exchange.Diagnostics.Components.HygieneForwardSync
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceInstanceSyncTracer
		{
			get
			{
				if (ExTraceGlobals.serviceInstanceSyncTracer == null)
				{
					ExTraceGlobals.serviceInstanceSyncTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceInstanceSyncTracer;
			}
		}

		public static Trace FullTenantSyncTracer
		{
			get
			{
				if (ExTraceGlobals.fullTenantSyncTracer == null)
				{
					ExTraceGlobals.fullTenantSyncTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.fullTenantSyncTracer;
			}
		}

		public static Trace PersistenceTracer
		{
			get
			{
				if (ExTraceGlobals.persistenceTracer == null)
				{
					ExTraceGlobals.persistenceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.persistenceTracer;
			}
		}

		public static Trace ProvisioningTracer
		{
			get
			{
				if (ExTraceGlobals.provisioningTracer == null)
				{
					ExTraceGlobals.provisioningTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.provisioningTracer;
			}
		}

		public static Trace MsoServicesTracer
		{
			get
			{
				if (ExTraceGlobals.msoServicesTracer == null)
				{
					ExTraceGlobals.msoServicesTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.msoServicesTracer;
			}
		}

		public static Trace GlsServicesTracer
		{
			get
			{
				if (ExTraceGlobals.glsServicesTracer == null)
				{
					ExTraceGlobals.glsServicesTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.glsServicesTracer;
			}
		}

		public static Trace DNSServicesTracer
		{
			get
			{
				if (ExTraceGlobals.dNSServicesTracer == null)
				{
					ExTraceGlobals.dNSServicesTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.dNSServicesTracer;
			}
		}

		private static Guid componentGuid = new Guid("952887AB-4E9A-4CF8-867F-3C5BD5BB67A3");

		private static Trace serviceInstanceSyncTracer = null;

		private static Trace fullTenantSyncTracer = null;

		private static Trace persistenceTracer = null;

		private static Trace provisioningTracer = null;

		private static Trace msoServicesTracer = null;

		private static Trace glsServicesTracer = null;

		private static Trace dNSServicesTracer = null;
	}
}
