using System;

namespace Microsoft.Exchange.Diagnostics.Components.BackSync
{
	public static class ExTraceGlobals
	{
		public static Trace BackSyncTracer
		{
			get
			{
				if (ExTraceGlobals.backSyncTracer == null)
				{
					ExTraceGlobals.backSyncTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.backSyncTracer;
			}
		}

		public static Trace ActiveDirectoryTracer
		{
			get
			{
				if (ExTraceGlobals.activeDirectoryTracer == null)
				{
					ExTraceGlobals.activeDirectoryTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.activeDirectoryTracer;
			}
		}

		public static Trace TenantFullSyncTracer
		{
			get
			{
				if (ExTraceGlobals.tenantFullSyncTracer == null)
				{
					ExTraceGlobals.tenantFullSyncTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.tenantFullSyncTracer;
			}
		}

		public static Trace MergeTracer
		{
			get
			{
				if (ExTraceGlobals.mergeTracer == null)
				{
					ExTraceGlobals.mergeTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.mergeTracer;
			}
		}

		private static Guid componentGuid = new Guid("3C237538-546C-4659-AED9-F445236DFB91");

		private static Trace backSyncTracer = null;

		private static Trace activeDirectoryTracer = null;

		private static Trace tenantFullSyncTracer = null;

		private static Trace mergeTracer = null;
	}
}
