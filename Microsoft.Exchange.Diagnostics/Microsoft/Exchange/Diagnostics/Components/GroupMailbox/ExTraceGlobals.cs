using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.GroupMailbox
{
	public static class ExTraceGlobals
	{
		public static Trace CmdletsTracer
		{
			get
			{
				if (ExTraceGlobals.cmdletsTracer == null)
				{
					ExTraceGlobals.cmdletsTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.cmdletsTracer;
			}
		}

		public static Trace WebServicesTracer
		{
			get
			{
				if (ExTraceGlobals.webServicesTracer == null)
				{
					ExTraceGlobals.webServicesTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.webServicesTracer;
			}
		}

		public static Trace GroupMailboxAccessLayerTracer
		{
			get
			{
				if (ExTraceGlobals.groupMailboxAccessLayerTracer == null)
				{
					ExTraceGlobals.groupMailboxAccessLayerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.groupMailboxAccessLayerTracer;
			}
		}

		public static Trace LocalAssociationStoreTracer
		{
			get
			{
				if (ExTraceGlobals.localAssociationStoreTracer == null)
				{
					ExTraceGlobals.localAssociationStoreTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.localAssociationStoreTracer;
			}
		}

		public static Trace MailboxLocatorTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxLocatorTracer == null)
				{
					ExTraceGlobals.mailboxLocatorTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.mailboxLocatorTracer;
			}
		}

		public static Trace GroupAssociationAdaptorTracer
		{
			get
			{
				if (ExTraceGlobals.groupAssociationAdaptorTracer == null)
				{
					ExTraceGlobals.groupAssociationAdaptorTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.groupAssociationAdaptorTracer;
			}
		}

		public static Trace UserAssociationAdaptorTracer
		{
			get
			{
				if (ExTraceGlobals.userAssociationAdaptorTracer == null)
				{
					ExTraceGlobals.userAssociationAdaptorTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.userAssociationAdaptorTracer;
			}
		}

		public static Trace UpdateAssociationCommandTracer
		{
			get
			{
				if (ExTraceGlobals.updateAssociationCommandTracer == null)
				{
					ExTraceGlobals.updateAssociationCommandTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.updateAssociationCommandTracer;
			}
		}

		public static Trace AssociationReplicationTracer
		{
			get
			{
				if (ExTraceGlobals.associationReplicationTracer == null)
				{
					ExTraceGlobals.associationReplicationTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.associationReplicationTracer;
			}
		}

		public static Trace AssociationReplicationAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.associationReplicationAssistantTracer == null)
				{
					ExTraceGlobals.associationReplicationAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.associationReplicationAssistantTracer;
			}
		}

		public static Trace GroupEmailNotificationHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.groupEmailNotificationHandlerTracer == null)
				{
					ExTraceGlobals.groupEmailNotificationHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.groupEmailNotificationHandlerTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace GroupMailboxAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.groupMailboxAssistantTracer == null)
				{
					ExTraceGlobals.groupMailboxAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.groupMailboxAssistantTracer;
			}
		}

		public static Trace UnseenDataUserAssociationAdaptorTracer
		{
			get
			{
				if (ExTraceGlobals.unseenDataUserAssociationAdaptorTracer == null)
				{
					ExTraceGlobals.unseenDataUserAssociationAdaptorTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.unseenDataUserAssociationAdaptorTracer;
			}
		}

		private static Guid componentGuid = new Guid("902B6BA0-4553-4533-8594-4AD6DA001FB7");

		private static Trace cmdletsTracer = null;

		private static Trace webServicesTracer = null;

		private static Trace groupMailboxAccessLayerTracer = null;

		private static Trace localAssociationStoreTracer = null;

		private static Trace mailboxLocatorTracer = null;

		private static Trace groupAssociationAdaptorTracer = null;

		private static Trace userAssociationAdaptorTracer = null;

		private static Trace updateAssociationCommandTracer = null;

		private static Trace associationReplicationTracer = null;

		private static Trace associationReplicationAssistantTracer = null;

		private static Trace groupEmailNotificationHandlerTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace groupMailboxAssistantTracer = null;

		private static Trace unseenDataUserAssociationAdaptorTracer = null;
	}
}
