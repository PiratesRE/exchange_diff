using System;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.ClientAccess;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaApplication : OwaApplicationBase
	{
		internal override OWAVDirType OwaVDirType
		{
			get
			{
				return OWAVDirType.OWA;
			}
		}

		protected override void ExecuteApplicationSpecificStart()
		{
			ExWatson.Register("E12IIS");
			StoreSession.UseRPCContextPool = true;
			UMClientCommonBase.InitializePerformanceCounters(false);
			OwaEventRegistry.RegisterEnum(typeof(Importance));
			OwaEventRegistry.RegisterEnum(typeof(Sensitivity));
			OwaEventRegistry.RegisterEnum(typeof(AddressOrigin));
			OwaEventRegistry.RegisterEnum(typeof(FlagAction));
			OwaEventRegistry.RegisterEnum(typeof(TaskStatus));
			OwaEventRegistry.RegisterEnum(typeof(BusyType));
			OwaEventRegistry.RegisterEnum(typeof(ResponseType));
			OwaEventRegistry.RegisterEnum(typeof(StoreObjectType));
			OwaEventRegistry.RegisterEnum(typeof(EmailAddressIndex));
			OwaEventRegistry.RegisterEnum(typeof(NavigationNodeGroupSection));
			OwaEventRegistry.RegisterEnum(typeof(InstantMessagingTypeOptions));
			OwaEventRegistry.RegisterEnum(typeof(NavigationModule));
			OwaEventRegistry.RegisterEnum(typeof(DefaultFolderType));
			OwaEventRegistry.RegisterEnum(typeof(RecipientBlockType));
			OwaEventRegistry.RegisterEnum(typeof(RecipientJunkEmailContextMenuType));
			OwaEventRegistry.RegisterEnum(typeof(SharingLevel));
			OwaEventRegistry.RegisterEnum(typeof(DenyResponseType));
			OwaEventRegistry.RegisterEnum(typeof(AddressBookItemEventHandler.ItemTypeToPeople));
			OwaEventRegistry.RegisterStruct(typeof(RecipientInfo));
			OwaEventRegistry.RegisterStruct(typeof(DeleteItemInfo));
			OwaEventRegistry.RegisterStruct(typeof(ReminderInfo));
			OwaEventRegistry.RegisterHandler(typeof(ProxyEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(PendingRequestEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(ProxyToEwsEventHandler));
			NotificationEventHandler.Register();
			ClientCacheEventHandler.Register();
			DocumentLibraryEventHandler.Register();
			DocumentEventHandler.Register();
			DatePickerEventHandler.Register();
			ReadADOrgPersonEventHandler.Register();
			ListViewEventHandler.Register();
			TreeEventHandler.Register();
			NavigationEventHandler.Register();
			RecipientWellEventHandler.Register();
			AttachmentEventHandler.Register();
			ReadMessageEventHandler.Register();
			ReadConversationEventHandler.Register();
			ReadVoiceMessageEventHandler.Register();
			OptionsEventHandler.Register();
			AddressBookItemEventHandler.Register();
			OwaEventRegistry.RegisterHandler(typeof(EditMessageEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(EditSmsEventHandler));
			EditCalendarItemEventHandler.Register();
			CalendarViewEventHandler.Register();
			OwaEventRegistry.RegisterHandler(typeof(EditMeetingInviteEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(EditMeetingResponseEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(ErrorEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(FlagEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(CategoryEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(InstantMessageEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(MonitoringEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(MailTipsEventHandler));
			RemindersEventHandler.Register();
			EditContactItemEventHandler.Register();
			EditDistributionListEventHandler.Register();
			JunkEmailEventHandler.Register();
			EditTaskEventHandler.Register();
			ComplianceEventHandler.Register();
			WebReadyFileEventHandler.Register();
			EditPostEventHandler.Register();
			ReadPostEventHandler.Register();
			SMimeEventHandler.Register();
			NavigationNodeEventHandler.Register();
			MessageVirtualListViewEventHandler2.Register();
			TaskVirtualListViewEventHandler.Register();
			PersonalAutoAttendantOptionsEventHandler.Register();
			EditPAAEventHandler.Register();
			PerformanceConsoleEventHandler.Register();
			DirectoryVirtualListViewEventHandler.Register();
			ContactVirtualListViewEventHandler.Register();
			DumpsterVirtualListViewEventHandler.Register();
			SharingMessageEventHandler.Register();
			DeletePolicyEventHandler.Register();
			MovePolicyEventHandler.Register();
			PrintCalendarEventHandler.Register();
			MessageAnnotationEventHandler.Register();
		}
	}
}
