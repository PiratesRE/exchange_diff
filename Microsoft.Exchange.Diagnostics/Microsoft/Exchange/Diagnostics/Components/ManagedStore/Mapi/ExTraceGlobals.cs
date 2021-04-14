using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace SchemaMapEntryAddedTracer
		{
			get
			{
				if (ExTraceGlobals.schemaMapEntryAddedTracer == null)
				{
					ExTraceGlobals.schemaMapEntryAddedTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.schemaMapEntryAddedTracer;
			}
		}

		public static Trace SchemaMapEntryUpdatedTracer
		{
			get
			{
				if (ExTraceGlobals.schemaMapEntryUpdatedTracer == null)
				{
					ExTraceGlobals.schemaMapEntryUpdatedTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.schemaMapEntryUpdatedTracer;
			}
		}

		public static Trace PropertyMappingTracer
		{
			get
			{
				if (ExTraceGlobals.propertyMappingTracer == null)
				{
					ExTraceGlobals.propertyMappingTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.propertyMappingTracer;
			}
		}

		public static Trace GetPropsPropertiesTracer
		{
			get
			{
				if (ExTraceGlobals.getPropsPropertiesTracer == null)
				{
					ExTraceGlobals.getPropsPropertiesTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.getPropsPropertiesTracer;
			}
		}

		public static Trace SetPropsPropertiesTracer
		{
			get
			{
				if (ExTraceGlobals.setPropsPropertiesTracer == null)
				{
					ExTraceGlobals.setPropsPropertiesTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.setPropsPropertiesTracer;
			}
		}

		public static Trace DeletePropsPropertiesTracer
		{
			get
			{
				if (ExTraceGlobals.deletePropsPropertiesTracer == null)
				{
					ExTraceGlobals.deletePropsPropertiesTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.deletePropsPropertiesTracer;
			}
		}

		public static Trace CopyOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.copyOperationsTracer == null)
				{
					ExTraceGlobals.copyOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.copyOperationsTracer;
			}
		}

		public static Trace StreamOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.streamOperationsTracer == null)
				{
					ExTraceGlobals.streamOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.streamOperationsTracer;
			}
		}

		public static Trace AttachmentOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.attachmentOperationsTracer == null)
				{
					ExTraceGlobals.attachmentOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.attachmentOperationsTracer;
			}
		}

		public static Trace NotificationTracer
		{
			get
			{
				if (ExTraceGlobals.notificationTracer == null)
				{
					ExTraceGlobals.notificationTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.notificationTracer;
			}
		}

		public static Trace CreateLogonTracer
		{
			get
			{
				if (ExTraceGlobals.createLogonTracer == null)
				{
					ExTraceGlobals.createLogonTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.createLogonTracer;
			}
		}

		public static Trace CreateSessionTracer
		{
			get
			{
				if (ExTraceGlobals.createSessionTracer == null)
				{
					ExTraceGlobals.createSessionTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.createSessionTracer;
			}
		}

		public static Trace SubmitMessageTracer
		{
			get
			{
				if (ExTraceGlobals.submitMessageTracer == null)
				{
					ExTraceGlobals.submitMessageTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.submitMessageTracer;
			}
		}

		public static Trace AccessCheckTracer
		{
			get
			{
				if (ExTraceGlobals.accessCheckTracer == null)
				{
					ExTraceGlobals.accessCheckTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.accessCheckTracer;
			}
		}

		public static Trace TimedEventsTracer
		{
			get
			{
				if (ExTraceGlobals.timedEventsTracer == null)
				{
					ExTraceGlobals.timedEventsTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.timedEventsTracer;
			}
		}

		public static Trace DeferredSendTracer
		{
			get
			{
				if (ExTraceGlobals.deferredSendTracer == null)
				{
					ExTraceGlobals.deferredSendTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.deferredSendTracer;
			}
		}

		public static Trace MailboxSignatureTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxSignatureTracer == null)
				{
					ExTraceGlobals.mailboxSignatureTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.mailboxSignatureTracer;
			}
		}

		public static Trace QuotaTracer
		{
			get
			{
				if (ExTraceGlobals.quotaTracer == null)
				{
					ExTraceGlobals.quotaTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.quotaTracer;
			}
		}

		public static Trace FillRowTracer
		{
			get
			{
				if (ExTraceGlobals.fillRowTracer == null)
				{
					ExTraceGlobals.fillRowTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.fillRowTracer;
			}
		}

		public static Trace SecurityContextManagerTracer
		{
			get
			{
				if (ExTraceGlobals.securityContextManagerTracer == null)
				{
					ExTraceGlobals.securityContextManagerTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.securityContextManagerTracer;
			}
		}

		public static Trace InTransitTransitionsTracer
		{
			get
			{
				if (ExTraceGlobals.inTransitTransitionsTracer == null)
				{
					ExTraceGlobals.inTransitTransitionsTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.inTransitTransitionsTracer;
			}
		}

		public static Trace RestrictTracer
		{
			get
			{
				if (ExTraceGlobals.restrictTracer == null)
				{
					ExTraceGlobals.restrictTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.restrictTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace EnableBadItemInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.enableBadItemInjectionTracer == null)
				{
					ExTraceGlobals.enableBadItemInjectionTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.enableBadItemInjectionTracer;
			}
		}

		public static Trace CreateMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.createMailboxTracer == null)
				{
					ExTraceGlobals.createMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.createMailboxTracer;
			}
		}

		private static Guid componentGuid = new Guid("7927e3f9-b2bc-461f-96e7-c78d73ed4f04");

		private static Trace generalTracer = null;

		private static Trace schemaMapEntryAddedTracer = null;

		private static Trace schemaMapEntryUpdatedTracer = null;

		private static Trace propertyMappingTracer = null;

		private static Trace getPropsPropertiesTracer = null;

		private static Trace setPropsPropertiesTracer = null;

		private static Trace deletePropsPropertiesTracer = null;

		private static Trace copyOperationsTracer = null;

		private static Trace streamOperationsTracer = null;

		private static Trace attachmentOperationsTracer = null;

		private static Trace notificationTracer = null;

		private static Trace createLogonTracer = null;

		private static Trace createSessionTracer = null;

		private static Trace submitMessageTracer = null;

		private static Trace accessCheckTracer = null;

		private static Trace timedEventsTracer = null;

		private static Trace deferredSendTracer = null;

		private static Trace mailboxSignatureTracer = null;

		private static Trace quotaTracer = null;

		private static Trace fillRowTracer = null;

		private static Trace securityContextManagerTracer = null;

		private static Trace inTransitTransitionsTracer = null;

		private static Trace restrictTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace enableBadItemInjectionTracer = null;

		private static Trace createMailboxTracer = null;
	}
}
