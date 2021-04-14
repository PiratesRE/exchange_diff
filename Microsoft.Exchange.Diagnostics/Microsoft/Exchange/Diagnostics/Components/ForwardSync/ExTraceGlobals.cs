using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ForwardSync
{
	public static class ExTraceGlobals
	{
		public static Trace ForwardSyncServiceTracer
		{
			get
			{
				if (ExTraceGlobals.forwardSyncServiceTracer == null)
				{
					ExTraceGlobals.forwardSyncServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.forwardSyncServiceTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace MainStreamTracer
		{
			get
			{
				if (ExTraceGlobals.mainStreamTracer == null)
				{
					ExTraceGlobals.mainStreamTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.mainStreamTracer;
			}
		}

		public static Trace FullSyncStreamTracer
		{
			get
			{
				if (ExTraceGlobals.fullSyncStreamTracer == null)
				{
					ExTraceGlobals.fullSyncStreamTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.fullSyncStreamTracer;
			}
		}

		public static Trace MsoSyncServiceTracer
		{
			get
			{
				if (ExTraceGlobals.msoSyncServiceTracer == null)
				{
					ExTraceGlobals.msoSyncServiceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.msoSyncServiceTracer;
			}
		}

		public static Trace PowerShellTracer
		{
			get
			{
				if (ExTraceGlobals.powerShellTracer == null)
				{
					ExTraceGlobals.powerShellTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.powerShellTracer;
			}
		}

		public static Trace JobProcessorTracer
		{
			get
			{
				if (ExTraceGlobals.jobProcessorTracer == null)
				{
					ExTraceGlobals.jobProcessorTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.jobProcessorTracer;
			}
		}

		public static Trace RecipientWorkflowTracer
		{
			get
			{
				if (ExTraceGlobals.recipientWorkflowTracer == null)
				{
					ExTraceGlobals.recipientWorkflowTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.recipientWorkflowTracer;
			}
		}

		public static Trace OrganizationWorkflowTracer
		{
			get
			{
				if (ExTraceGlobals.organizationWorkflowTracer == null)
				{
					ExTraceGlobals.organizationWorkflowTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.organizationWorkflowTracer;
			}
		}

		public static Trace ProvisioningLicenseTracer
		{
			get
			{
				if (ExTraceGlobals.provisioningLicenseTracer == null)
				{
					ExTraceGlobals.provisioningLicenseTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.provisioningLicenseTracer;
			}
		}

		public static Trace UnifiedGroupTracer
		{
			get
			{
				if (ExTraceGlobals.unifiedGroupTracer == null)
				{
					ExTraceGlobals.unifiedGroupTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.unifiedGroupTracer;
			}
		}

		private static Guid componentGuid = new Guid("8FAC856B-D0D4-4f7d-BBE9-B713EDFCBAAD");

		private static Trace forwardSyncServiceTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace mainStreamTracer = null;

		private static Trace fullSyncStreamTracer = null;

		private static Trace msoSyncServiceTracer = null;

		private static Trace powerShellTracer = null;

		private static Trace jobProcessorTracer = null;

		private static Trace recipientWorkflowTracer = null;

		private static Trace organizationWorkflowTracer = null;

		private static Trace provisioningLicenseTracer = null;

		private static Trace unifiedGroupTracer = null;
	}
}
