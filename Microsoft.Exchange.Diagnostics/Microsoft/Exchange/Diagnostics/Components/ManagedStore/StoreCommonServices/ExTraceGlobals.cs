using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices
{
	public static class ExTraceGlobals
	{
		public static Trace ContextTracer
		{
			get
			{
				if (ExTraceGlobals.contextTracer == null)
				{
					ExTraceGlobals.contextTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.contextTracer;
			}
		}

		public static Trace MailboxTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxTracer == null)
				{
					ExTraceGlobals.mailboxTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mailboxTracer;
			}
		}

		public static Trace ExtendedPropsTracer
		{
			get
			{
				if (ExTraceGlobals.extendedPropsTracer == null)
				{
					ExTraceGlobals.extendedPropsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.extendedPropsTracer;
			}
		}

		public static Trace QueryPlannerSummaryTracer
		{
			get
			{
				if (ExTraceGlobals.queryPlannerSummaryTracer == null)
				{
					ExTraceGlobals.queryPlannerSummaryTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.queryPlannerSummaryTracer;
			}
		}

		public static Trace QueryPlannerDetailTracer
		{
			get
			{
				if (ExTraceGlobals.queryPlannerDetailTracer == null)
				{
					ExTraceGlobals.queryPlannerDetailTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.queryPlannerDetailTracer;
			}
		}

		public static Trace SecurityMailboxOwnerAccessTracer
		{
			get
			{
				if (ExTraceGlobals.securityMailboxOwnerAccessTracer == null)
				{
					ExTraceGlobals.securityMailboxOwnerAccessTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.securityMailboxOwnerAccessTracer;
			}
		}

		public static Trace SecurityAdminAccessTracer
		{
			get
			{
				if (ExTraceGlobals.securityAdminAccessTracer == null)
				{
					ExTraceGlobals.securityAdminAccessTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.securityAdminAccessTracer;
			}
		}

		public static Trace SecurityServiceAccessTracer
		{
			get
			{
				if (ExTraceGlobals.securityServiceAccessTracer == null)
				{
					ExTraceGlobals.securityServiceAccessTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.securityServiceAccessTracer;
			}
		}

		public static Trace SecuritySendAsAccessTracer
		{
			get
			{
				if (ExTraceGlobals.securitySendAsAccessTracer == null)
				{
					ExTraceGlobals.securitySendAsAccessTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.securitySendAsAccessTracer;
			}
		}

		public static Trace SecurityContextTracer
		{
			get
			{
				if (ExTraceGlobals.securityContextTracer == null)
				{
					ExTraceGlobals.securityContextTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.securityContextTracer;
			}
		}

		public static Trace SecurityDescriptorTracer
		{
			get
			{
				if (ExTraceGlobals.securityDescriptorTracer == null)
				{
					ExTraceGlobals.securityDescriptorTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.securityDescriptorTracer;
			}
		}

		public static Trace FullAccountNameTracer
		{
			get
			{
				if (ExTraceGlobals.fullAccountNameTracer == null)
				{
					ExTraceGlobals.fullAccountNameTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.fullAccountNameTracer;
			}
		}

		public static Trace ExecutionDiagnosticsTracer
		{
			get
			{
				if (ExTraceGlobals.executionDiagnosticsTracer == null)
				{
					ExTraceGlobals.executionDiagnosticsTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.executionDiagnosticsTracer;
			}
		}

		public static Trace FullTextIndexTracer
		{
			get
			{
				if (ExTraceGlobals.fullTextIndexTracer == null)
				{
					ExTraceGlobals.fullTextIndexTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.fullTextIndexTracer;
			}
		}

		public static Trace MailboxQuarantineTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxQuarantineTracer == null)
				{
					ExTraceGlobals.mailboxQuarantineTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.mailboxQuarantineTracer;
			}
		}

		public static Trace MailboxSignatureTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxSignatureTracer == null)
				{
					ExTraceGlobals.mailboxSignatureTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.mailboxSignatureTracer;
			}
		}

		public static Trace TimedEventsTracer
		{
			get
			{
				if (ExTraceGlobals.timedEventsTracer == null)
				{
					ExTraceGlobals.timedEventsTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.timedEventsTracer;
			}
		}

		public static Trace MaintenanceTracer
		{
			get
			{
				if (ExTraceGlobals.maintenanceTracer == null)
				{
					ExTraceGlobals.maintenanceTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.maintenanceTracer;
			}
		}

		public static Trace MailboxLockTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxLockTracer == null)
				{
					ExTraceGlobals.mailboxLockTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.mailboxLockTracer;
			}
		}

		public static Trace CriticalBlockTracer
		{
			get
			{
				if (ExTraceGlobals.criticalBlockTracer == null)
				{
					ExTraceGlobals.criticalBlockTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.criticalBlockTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace NotificationTracer
		{
			get
			{
				if (ExTraceGlobals.notificationTracer == null)
				{
					ExTraceGlobals.notificationTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.notificationTracer;
			}
		}

		public static Trace StoreDatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.storeDatabaseTracer == null)
				{
					ExTraceGlobals.storeDatabaseTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.storeDatabaseTracer;
			}
		}

		public static Trace MailboxCountersTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxCountersTracer == null)
				{
					ExTraceGlobals.mailboxCountersTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.mailboxCountersTracer;
			}
		}

		public static Trace ChunkingTracer
		{
			get
			{
				if (ExTraceGlobals.chunkingTracer == null)
				{
					ExTraceGlobals.chunkingTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.chunkingTracer;
			}
		}

		public static Trace ViewTableFindRowTracer
		{
			get
			{
				if (ExTraceGlobals.viewTableFindRowTracer == null)
				{
					ExTraceGlobals.viewTableFindRowTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.viewTableFindRowTracer;
			}
		}

		public static Trace SchemaUpgradeServiceTracer
		{
			get
			{
				if (ExTraceGlobals.schemaUpgradeServiceTracer == null)
				{
					ExTraceGlobals.schemaUpgradeServiceTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.schemaUpgradeServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("15744371-ee52-4dfc-97f9-940803cf462f");

		private static Trace contextTracer = null;

		private static Trace mailboxTracer = null;

		private static Trace extendedPropsTracer = null;

		private static Trace queryPlannerSummaryTracer = null;

		private static Trace queryPlannerDetailTracer = null;

		private static Trace securityMailboxOwnerAccessTracer = null;

		private static Trace securityAdminAccessTracer = null;

		private static Trace securityServiceAccessTracer = null;

		private static Trace securitySendAsAccessTracer = null;

		private static Trace securityContextTracer = null;

		private static Trace securityDescriptorTracer = null;

		private static Trace fullAccountNameTracer = null;

		private static Trace executionDiagnosticsTracer = null;

		private static Trace fullTextIndexTracer = null;

		private static Trace mailboxQuarantineTracer = null;

		private static Trace mailboxSignatureTracer = null;

		private static Trace timedEventsTracer = null;

		private static Trace maintenanceTracer = null;

		private static Trace mailboxLockTracer = null;

		private static Trace criticalBlockTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace notificationTracer = null;

		private static Trace storeDatabaseTracer = null;

		private static Trace mailboxCountersTracer = null;

		private static Trace chunkingTracer = null;

		private static Trace viewTableFindRowTracer = null;

		private static Trace schemaUpgradeServiceTracer = null;
	}
}
