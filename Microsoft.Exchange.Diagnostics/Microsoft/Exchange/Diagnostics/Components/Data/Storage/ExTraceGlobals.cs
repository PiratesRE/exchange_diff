using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Data.Storage
{
	public static class ExTraceGlobals
	{
		public static Trace StorageTracer
		{
			get
			{
				if (ExTraceGlobals.storageTracer == null)
				{
					ExTraceGlobals.storageTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.storageTracer;
			}
		}

		public static Trace InteropTracer
		{
			get
			{
				if (ExTraceGlobals.interopTracer == null)
				{
					ExTraceGlobals.interopTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.interopTracer;
			}
		}

		public static Trace MeetingMessageTracer
		{
			get
			{
				if (ExTraceGlobals.meetingMessageTracer == null)
				{
					ExTraceGlobals.meetingMessageTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.meetingMessageTracer;
			}
		}

		public static Trace EventTracer
		{
			get
			{
				if (ExTraceGlobals.eventTracer == null)
				{
					ExTraceGlobals.eventTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.eventTracer;
			}
		}

		public static Trace DisposeTracer
		{
			get
			{
				if (ExTraceGlobals.disposeTracer == null)
				{
					ExTraceGlobals.disposeTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.disposeTracer;
			}
		}

		public static Trace ServiceDiscoveryTracer
		{
			get
			{
				if (ExTraceGlobals.serviceDiscoveryTracer == null)
				{
					ExTraceGlobals.serviceDiscoveryTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.serviceDiscoveryTracer;
			}
		}

		public static Trace ContextTracer
		{
			get
			{
				if (ExTraceGlobals.contextTracer == null)
				{
					ExTraceGlobals.contextTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.contextTracer;
			}
		}

		public static Trace ContextShadowTracer
		{
			get
			{
				if (ExTraceGlobals.contextShadowTracer == null)
				{
					ExTraceGlobals.contextShadowTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.contextShadowTracer;
			}
		}

		public static Trace CcGenericTracer
		{
			get
			{
				if (ExTraceGlobals.ccGenericTracer == null)
				{
					ExTraceGlobals.ccGenericTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.ccGenericTracer;
			}
		}

		public static Trace CcOleTracer
		{
			get
			{
				if (ExTraceGlobals.ccOleTracer == null)
				{
					ExTraceGlobals.ccOleTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.ccOleTracer;
			}
		}

		public static Trace CcBodyTracer
		{
			get
			{
				if (ExTraceGlobals.ccBodyTracer == null)
				{
					ExTraceGlobals.ccBodyTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.ccBodyTracer;
			}
		}

		public static Trace CcInboundGenericTracer
		{
			get
			{
				if (ExTraceGlobals.ccInboundGenericTracer == null)
				{
					ExTraceGlobals.ccInboundGenericTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.ccInboundGenericTracer;
			}
		}

		public static Trace CcInboundMimeTracer
		{
			get
			{
				if (ExTraceGlobals.ccInboundMimeTracer == null)
				{
					ExTraceGlobals.ccInboundMimeTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.ccInboundMimeTracer;
			}
		}

		public static Trace CcInboundTnefTracer
		{
			get
			{
				if (ExTraceGlobals.ccInboundTnefTracer == null)
				{
					ExTraceGlobals.ccInboundTnefTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.ccInboundTnefTracer;
			}
		}

		public static Trace CcOutboundGenericTracer
		{
			get
			{
				if (ExTraceGlobals.ccOutboundGenericTracer == null)
				{
					ExTraceGlobals.ccOutboundGenericTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.ccOutboundGenericTracer;
			}
		}

		public static Trace CcOutboundMimeTracer
		{
			get
			{
				if (ExTraceGlobals.ccOutboundMimeTracer == null)
				{
					ExTraceGlobals.ccOutboundMimeTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.ccOutboundMimeTracer;
			}
		}

		public static Trace CcOutboundTnefTracer
		{
			get
			{
				if (ExTraceGlobals.ccOutboundTnefTracer == null)
				{
					ExTraceGlobals.ccOutboundTnefTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.ccOutboundTnefTracer;
			}
		}

		public static Trace CcPFDTracer
		{
			get
			{
				if (ExTraceGlobals.ccPFDTracer == null)
				{
					ExTraceGlobals.ccPFDTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.ccPFDTracer;
			}
		}

		public static Trace SessionTracer
		{
			get
			{
				if (ExTraceGlobals.sessionTracer == null)
				{
					ExTraceGlobals.sessionTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.sessionTracer;
			}
		}

		public static Trace DefaultFoldersTracer
		{
			get
			{
				if (ExTraceGlobals.defaultFoldersTracer == null)
				{
					ExTraceGlobals.defaultFoldersTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.defaultFoldersTracer;
			}
		}

		public static Trace UserConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.userConfigurationTracer == null)
				{
					ExTraceGlobals.userConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.userConfigurationTracer;
			}
		}

		public static Trace PropertyBagTracer
		{
			get
			{
				if (ExTraceGlobals.propertyBagTracer == null)
				{
					ExTraceGlobals.propertyBagTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.propertyBagTracer;
			}
		}

		public static Trace TaskTracer
		{
			get
			{
				if (ExTraceGlobals.taskTracer == null)
				{
					ExTraceGlobals.taskTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.taskTracer;
			}
		}

		public static Trace RecurrenceTracer
		{
			get
			{
				if (ExTraceGlobals.recurrenceTracer == null)
				{
					ExTraceGlobals.recurrenceTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.recurrenceTracer;
			}
		}

		public static Trace WorkHoursTracer
		{
			get
			{
				if (ExTraceGlobals.workHoursTracer == null)
				{
					ExTraceGlobals.workHoursTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.workHoursTracer;
			}
		}

		public static Trace SyncTracer
		{
			get
			{
				if (ExTraceGlobals.syncTracer == null)
				{
					ExTraceGlobals.syncTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.syncTracer;
			}
		}

		public static Trace ICalTracer
		{
			get
			{
				if (ExTraceGlobals.iCalTracer == null)
				{
					ExTraceGlobals.iCalTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.iCalTracer;
			}
		}

		public static Trace ActiveManagerClientTracer
		{
			get
			{
				if (ExTraceGlobals.activeManagerClientTracer == null)
				{
					ExTraceGlobals.activeManagerClientTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.activeManagerClientTracer;
			}
		}

		public static Trace CcOutboundVCardTracer
		{
			get
			{
				if (ExTraceGlobals.ccOutboundVCardTracer == null)
				{
					ExTraceGlobals.ccOutboundVCardTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.ccOutboundVCardTracer;
			}
		}

		public static Trace CcInboundVCardTracer
		{
			get
			{
				if (ExTraceGlobals.ccInboundVCardTracer == null)
				{
					ExTraceGlobals.ccInboundVCardTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.ccInboundVCardTracer;
			}
		}

		public static Trace SharingTracer
		{
			get
			{
				if (ExTraceGlobals.sharingTracer == null)
				{
					ExTraceGlobals.sharingTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.sharingTracer;
			}
		}

		public static Trace RightsManagementTracer
		{
			get
			{
				if (ExTraceGlobals.rightsManagementTracer == null)
				{
					ExTraceGlobals.rightsManagementTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.rightsManagementTracer;
			}
		}

		public static Trace DatabaseAvailabilityGroupTracer
		{
			get
			{
				if (ExTraceGlobals.databaseAvailabilityGroupTracer == null)
				{
					ExTraceGlobals.databaseAvailabilityGroupTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.databaseAvailabilityGroupTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace SmtpServiceTracer
		{
			get
			{
				if (ExTraceGlobals.smtpServiceTracer == null)
				{
					ExTraceGlobals.smtpServiceTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.smtpServiceTracer;
			}
		}

		public static Trace MapiConnectivityTracer
		{
			get
			{
				if (ExTraceGlobals.mapiConnectivityTracer == null)
				{
					ExTraceGlobals.mapiConnectivityTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.mapiConnectivityTracer;
			}
		}

		public static Trace XtcTracer
		{
			get
			{
				if (ExTraceGlobals.xtcTracer == null)
				{
					ExTraceGlobals.xtcTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.xtcTracer;
			}
		}

		public static Trace CalendarLoggingTracer
		{
			get
			{
				if (ExTraceGlobals.calendarLoggingTracer == null)
				{
					ExTraceGlobals.calendarLoggingTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.calendarLoggingTracer;
			}
		}

		public static Trace CalendarSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.calendarSeriesTracer == null)
				{
					ExTraceGlobals.calendarSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.calendarSeriesTracer;
			}
		}

		public static Trace BirthdayCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayCalendarTracer == null)
				{
					ExTraceGlobals.birthdayCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.birthdayCalendarTracer;
			}
		}

		public static Trace PropertyMappingTracer
		{
			get
			{
				if (ExTraceGlobals.propertyMappingTracer == null)
				{
					ExTraceGlobals.propertyMappingTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.propertyMappingTracer;
			}
		}

		public static Trace MdbResourceHealthTracer
		{
			get
			{
				if (ExTraceGlobals.mdbResourceHealthTracer == null)
				{
					ExTraceGlobals.mdbResourceHealthTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.mdbResourceHealthTracer;
			}
		}

		public static Trace ContactLinkingTracer
		{
			get
			{
				if (ExTraceGlobals.contactLinkingTracer == null)
				{
					ExTraceGlobals.contactLinkingTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.contactLinkingTracer;
			}
		}

		public static Trace UserPhotosTracer
		{
			get
			{
				if (ExTraceGlobals.userPhotosTracer == null)
				{
					ExTraceGlobals.userPhotosTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.userPhotosTracer;
			}
		}

		public static Trace ContactFoldersEnumeratorTracer
		{
			get
			{
				if (ExTraceGlobals.contactFoldersEnumeratorTracer == null)
				{
					ExTraceGlobals.contactFoldersEnumeratorTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.contactFoldersEnumeratorTracer;
			}
		}

		public static Trace MyContactsFolderTracer
		{
			get
			{
				if (ExTraceGlobals.myContactsFolderTracer == null)
				{
					ExTraceGlobals.myContactsFolderTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.myContactsFolderTracer;
			}
		}

		public static Trace AggregationTracer
		{
			get
			{
				if (ExTraceGlobals.aggregationTracer == null)
				{
					ExTraceGlobals.aggregationTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.aggregationTracer;
			}
		}

		public static Trace OutlookSocialConnectorInteropTracer
		{
			get
			{
				if (ExTraceGlobals.outlookSocialConnectorInteropTracer == null)
				{
					ExTraceGlobals.outlookSocialConnectorInteropTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.outlookSocialConnectorInteropTracer;
			}
		}

		public static Trace PersonTracer
		{
			get
			{
				if (ExTraceGlobals.personTracer == null)
				{
					ExTraceGlobals.personTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.personTracer;
			}
		}

		public static Trace DatabasePingerTracer
		{
			get
			{
				if (ExTraceGlobals.databasePingerTracer == null)
				{
					ExTraceGlobals.databasePingerTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.databasePingerTracer;
			}
		}

		public static Trace ContactsEnumeratorTracer
		{
			get
			{
				if (ExTraceGlobals.contactsEnumeratorTracer == null)
				{
					ExTraceGlobals.contactsEnumeratorTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.contactsEnumeratorTracer;
			}
		}

		public static Trace ContactChangeLoggingTracer
		{
			get
			{
				if (ExTraceGlobals.contactChangeLoggingTracer == null)
				{
					ExTraceGlobals.contactChangeLoggingTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.contactChangeLoggingTracer;
			}
		}

		public static Trace ContactExporterTracer
		{
			get
			{
				if (ExTraceGlobals.contactExporterTracer == null)
				{
					ExTraceGlobals.contactExporterTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.contactExporterTracer;
			}
		}

		public static Trace SiteMailboxPermissionCheckTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxPermissionCheckTracer == null)
				{
					ExTraceGlobals.siteMailboxPermissionCheckTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.siteMailboxPermissionCheckTracer;
			}
		}

		public static Trace SiteMailboxDocumentSyncTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxDocumentSyncTracer == null)
				{
					ExTraceGlobals.siteMailboxDocumentSyncTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.siteMailboxDocumentSyncTracer;
			}
		}

		public static Trace SiteMailboxMembershipSyncTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxMembershipSyncTracer == null)
				{
					ExTraceGlobals.siteMailboxMembershipSyncTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.siteMailboxMembershipSyncTracer;
			}
		}

		public static Trace SiteMailboxClientOperationTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxClientOperationTracer == null)
				{
					ExTraceGlobals.siteMailboxClientOperationTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.siteMailboxClientOperationTracer;
			}
		}

		public static Trace SiteMailboxMessageDedupTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxMessageDedupTracer == null)
				{
					ExTraceGlobals.siteMailboxMessageDedupTracer = new Trace(ExTraceGlobals.componentGuid, 74);
				}
				return ExTraceGlobals.siteMailboxMessageDedupTracer;
			}
		}

		public static Trace RemindersTracer
		{
			get
			{
				if (ExTraceGlobals.remindersTracer == null)
				{
					ExTraceGlobals.remindersTracer = new Trace(ExTraceGlobals.componentGuid, 81);
				}
				return ExTraceGlobals.remindersTracer;
			}
		}

		public static Trace PeopleIKnowTracer
		{
			get
			{
				if (ExTraceGlobals.peopleIKnowTracer == null)
				{
					ExTraceGlobals.peopleIKnowTracer = new Trace(ExTraceGlobals.componentGuid, 82);
				}
				return ExTraceGlobals.peopleIKnowTracer;
			}
		}

		public static Trace AggregatedConversationsTracer
		{
			get
			{
				if (ExTraceGlobals.aggregatedConversationsTracer == null)
				{
					ExTraceGlobals.aggregatedConversationsTracer = new Trace(ExTraceGlobals.componentGuid, 83);
				}
				return ExTraceGlobals.aggregatedConversationsTracer;
			}
		}

		public static Trace DelegateTracer
		{
			get
			{
				if (ExTraceGlobals.delegateTracer == null)
				{
					ExTraceGlobals.delegateTracer = new Trace(ExTraceGlobals.componentGuid, 84);
				}
				return ExTraceGlobals.delegateTracer;
			}
		}

		public static Trace GroupMailboxSessionTracer
		{
			get
			{
				if (ExTraceGlobals.groupMailboxSessionTracer == null)
				{
					ExTraceGlobals.groupMailboxSessionTracer = new Trace(ExTraceGlobals.componentGuid, 85);
				}
				return ExTraceGlobals.groupMailboxSessionTracer;
			}
		}

		public static Trace SyncProcessTracer
		{
			get
			{
				if (ExTraceGlobals.syncProcessTracer == null)
				{
					ExTraceGlobals.syncProcessTracer = new Trace(ExTraceGlobals.componentGuid, 86);
				}
				return ExTraceGlobals.syncProcessTracer;
			}
		}

		public static Trace ConversationTracer
		{
			get
			{
				if (ExTraceGlobals.conversationTracer == null)
				{
					ExTraceGlobals.conversationTracer = new Trace(ExTraceGlobals.componentGuid, 87);
				}
				return ExTraceGlobals.conversationTracer;
			}
		}

		public static Trace ReliableTimerTracer
		{
			get
			{
				if (ExTraceGlobals.reliableTimerTracer == null)
				{
					ExTraceGlobals.reliableTimerTracer = new Trace(ExTraceGlobals.componentGuid, 88);
				}
				return ExTraceGlobals.reliableTimerTracer;
			}
		}

		public static Trace FavoritePublicFoldersTracer
		{
			get
			{
				if (ExTraceGlobals.favoritePublicFoldersTracer == null)
				{
					ExTraceGlobals.favoritePublicFoldersTracer = new Trace(ExTraceGlobals.componentGuid, 89);
				}
				return ExTraceGlobals.favoritePublicFoldersTracer;
			}
		}

		public static Trace PublicFoldersTracer
		{
			get
			{
				if (ExTraceGlobals.publicFoldersTracer == null)
				{
					ExTraceGlobals.publicFoldersTracer = new Trace(ExTraceGlobals.componentGuid, 90);
				}
				return ExTraceGlobals.publicFoldersTracer;
			}
		}

		private static Guid componentGuid = new Guid("6d031d1d-5908-457a-a6c4-cdd0f6e74d81");

		private static Trace storageTracer = null;

		private static Trace interopTracer = null;

		private static Trace meetingMessageTracer = null;

		private static Trace eventTracer = null;

		private static Trace disposeTracer = null;

		private static Trace serviceDiscoveryTracer = null;

		private static Trace contextTracer = null;

		private static Trace contextShadowTracer = null;

		private static Trace ccGenericTracer = null;

		private static Trace ccOleTracer = null;

		private static Trace ccBodyTracer = null;

		private static Trace ccInboundGenericTracer = null;

		private static Trace ccInboundMimeTracer = null;

		private static Trace ccInboundTnefTracer = null;

		private static Trace ccOutboundGenericTracer = null;

		private static Trace ccOutboundMimeTracer = null;

		private static Trace ccOutboundTnefTracer = null;

		private static Trace ccPFDTracer = null;

		private static Trace sessionTracer = null;

		private static Trace defaultFoldersTracer = null;

		private static Trace userConfigurationTracer = null;

		private static Trace propertyBagTracer = null;

		private static Trace taskTracer = null;

		private static Trace recurrenceTracer = null;

		private static Trace workHoursTracer = null;

		private static Trace syncTracer = null;

		private static Trace iCalTracer = null;

		private static Trace activeManagerClientTracer = null;

		private static Trace ccOutboundVCardTracer = null;

		private static Trace ccInboundVCardTracer = null;

		private static Trace sharingTracer = null;

		private static Trace rightsManagementTracer = null;

		private static Trace databaseAvailabilityGroupTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace smtpServiceTracer = null;

		private static Trace mapiConnectivityTracer = null;

		private static Trace xtcTracer = null;

		private static Trace calendarLoggingTracer = null;

		private static Trace calendarSeriesTracer = null;

		private static Trace birthdayCalendarTracer = null;

		private static Trace propertyMappingTracer = null;

		private static Trace mdbResourceHealthTracer = null;

		private static Trace contactLinkingTracer = null;

		private static Trace userPhotosTracer = null;

		private static Trace contactFoldersEnumeratorTracer = null;

		private static Trace myContactsFolderTracer = null;

		private static Trace aggregationTracer = null;

		private static Trace outlookSocialConnectorInteropTracer = null;

		private static Trace personTracer = null;

		private static Trace databasePingerTracer = null;

		private static Trace contactsEnumeratorTracer = null;

		private static Trace contactChangeLoggingTracer = null;

		private static Trace contactExporterTracer = null;

		private static Trace siteMailboxPermissionCheckTracer = null;

		private static Trace siteMailboxDocumentSyncTracer = null;

		private static Trace siteMailboxMembershipSyncTracer = null;

		private static Trace siteMailboxClientOperationTracer = null;

		private static Trace siteMailboxMessageDedupTracer = null;

		private static Trace remindersTracer = null;

		private static Trace peopleIKnowTracer = null;

		private static Trace aggregatedConversationsTracer = null;

		private static Trace delegateTracer = null;

		private static Trace groupMailboxSessionTracer = null;

		private static Trace syncProcessTracer = null;

		private static Trace conversationTracer = null;

		private static Trace reliableTimerTracer = null;

		private static Trace favoritePublicFoldersTracer = null;

		private static Trace publicFoldersTracer = null;
	}
}
