using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.ELC
{
	public static class ExTraceGlobals
	{
		public static Trace ELCAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.eLCAssistantTracer == null)
				{
					ExTraceGlobals.eLCAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.eLCAssistantTracer;
			}
		}

		public static Trace FolderProvisionerTracer
		{
			get
			{
				if (ExTraceGlobals.folderProvisionerTracer == null)
				{
					ExTraceGlobals.folderProvisionerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.folderProvisionerTracer;
			}
		}

		public static Trace CommonEnforcerOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.commonEnforcerOperationsTracer == null)
				{
					ExTraceGlobals.commonEnforcerOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.commonEnforcerOperationsTracer;
			}
		}

		public static Trace ExpirationEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.expirationEnforcerTracer == null)
				{
					ExTraceGlobals.expirationEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.expirationEnforcerTracer;
			}
		}

		public static Trace AutoCopyEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.autoCopyEnforcerTracer == null)
				{
					ExTraceGlobals.autoCopyEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.autoCopyEnforcerTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace TagProvisionerTracer
		{
			get
			{
				if (ExTraceGlobals.tagProvisionerTracer == null)
				{
					ExTraceGlobals.tagProvisionerTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.tagProvisionerTracer;
			}
		}

		public static Trace CommonTagEnforcerOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.commonTagEnforcerOperationsTracer == null)
				{
					ExTraceGlobals.commonTagEnforcerOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.commonTagEnforcerOperationsTracer;
			}
		}

		public static Trace ExpirationTagEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.expirationTagEnforcerTracer == null)
				{
					ExTraceGlobals.expirationTagEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.expirationTagEnforcerTracer;
			}
		}

		public static Trace AutocopyTagEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.autocopyTagEnforcerTracer == null)
				{
					ExTraceGlobals.autocopyTagEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.autocopyTagEnforcerTracer;
			}
		}

		public static Trace EventBasedAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.eventBasedAssistantTracer == null)
				{
					ExTraceGlobals.eventBasedAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.eventBasedAssistantTracer;
			}
		}

		public static Trace DeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.deliveryAgentTracer == null)
				{
					ExTraceGlobals.deliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.deliveryAgentTracer;
			}
		}

		public static Trace TagExpirationExecutorTracer
		{
			get
			{
				if (ExTraceGlobals.tagExpirationExecutorTracer == null)
				{
					ExTraceGlobals.tagExpirationExecutorTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.tagExpirationExecutorTracer;
			}
		}

		public static Trace CommonCleanupEnforcerOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.commonCleanupEnforcerOperationsTracer == null)
				{
					ExTraceGlobals.commonCleanupEnforcerOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.commonCleanupEnforcerOperationsTracer;
			}
		}

		public static Trace DumpsterExpirationEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.dumpsterExpirationEnforcerTracer == null)
				{
					ExTraceGlobals.dumpsterExpirationEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.dumpsterExpirationEnforcerTracer;
			}
		}

		public static Trace AuditExpirationEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.auditExpirationEnforcerTracer == null)
				{
					ExTraceGlobals.auditExpirationEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.auditExpirationEnforcerTracer;
			}
		}

		public static Trace CalendarLogExpirationEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.calendarLogExpirationEnforcerTracer == null)
				{
					ExTraceGlobals.calendarLogExpirationEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.calendarLogExpirationEnforcerTracer;
			}
		}

		public static Trace DumpsterQuotaEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.dumpsterQuotaEnforcerTracer == null)
				{
					ExTraceGlobals.dumpsterQuotaEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.dumpsterQuotaEnforcerTracer;
			}
		}

		public static Trace SupplementExpirationEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.supplementExpirationEnforcerTracer == null)
				{
					ExTraceGlobals.supplementExpirationEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.supplementExpirationEnforcerTracer;
			}
		}

		public static Trace DiscoveryHoldEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.discoveryHoldEnforcerTracer == null)
				{
					ExTraceGlobals.discoveryHoldEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.discoveryHoldEnforcerTracer;
			}
		}

		public static Trace ElcReportingTracer
		{
			get
			{
				if (ExTraceGlobals.elcReportingTracer == null)
				{
					ExTraceGlobals.elcReportingTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.elcReportingTracer;
			}
		}

		public static Trace HoldCleanupEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.holdCleanupEnforcerTracer == null)
				{
					ExTraceGlobals.holdCleanupEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.holdCleanupEnforcerTracer;
			}
		}

		public static Trace EHAHiddenFolderCleanupEnforcerTracer
		{
			get
			{
				if (ExTraceGlobals.eHAHiddenFolderCleanupEnforcerTracer == null)
				{
					ExTraceGlobals.eHAHiddenFolderCleanupEnforcerTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.eHAHiddenFolderCleanupEnforcerTracer;
			}
		}

		private static Guid componentGuid = new Guid("75989588-FD78-490c-B0DC-EC9E6F6E148B");

		private static Trace eLCAssistantTracer = null;

		private static Trace folderProvisionerTracer = null;

		private static Trace commonEnforcerOperationsTracer = null;

		private static Trace expirationEnforcerTracer = null;

		private static Trace autoCopyEnforcerTracer = null;

		private static Trace pFDTracer = null;

		private static Trace tagProvisionerTracer = null;

		private static Trace commonTagEnforcerOperationsTracer = null;

		private static Trace expirationTagEnforcerTracer = null;

		private static Trace autocopyTagEnforcerTracer = null;

		private static Trace eventBasedAssistantTracer = null;

		private static Trace deliveryAgentTracer = null;

		private static Trace tagExpirationExecutorTracer = null;

		private static Trace commonCleanupEnforcerOperationsTracer = null;

		private static Trace dumpsterExpirationEnforcerTracer = null;

		private static Trace auditExpirationEnforcerTracer = null;

		private static Trace calendarLogExpirationEnforcerTracer = null;

		private static Trace dumpsterQuotaEnforcerTracer = null;

		private static Trace supplementExpirationEnforcerTracer = null;

		private static Trace discoveryHoldEnforcerTracer = null;

		private static Trace elcReportingTracer = null;

		private static Trace holdCleanupEnforcerTracer = null;

		private static Trace eHAHiddenFolderCleanupEnforcerTracer = null;
	}
}
