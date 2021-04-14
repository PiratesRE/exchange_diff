using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemCreateInfo
	{
		private ItemCreateInfo(StoreObjectType type, Schema schema, AcrProfile acrProfile, ItemCreateInfo.ItemCreator creator)
		{
			this.Type = type;
			this.Schema = schema;
			this.AcrProfile = acrProfile;
			this.Creator = creator;
		}

		private static CalendarItemOccurrence CalendarItemOccurrenceCreator(ICoreItem coreItem)
		{
			return new CalendarItemOccurrence(coreItem);
		}

		private static ReportMessage ReportMessageCreator(ICoreItem coreItem)
		{
			return new ReportMessage(coreItem);
		}

		private static MessageItem MessageItemCreator(ICoreItem coreItem)
		{
			return new MessageItem(coreItem, false);
		}

		private static PostItem PostItemCreator(ICoreItem coreItem)
		{
			return new PostItem(coreItem);
		}

		private static CalendarItem CalendarItemCreator(ICoreItem coreItem)
		{
			return new CalendarItem(coreItem);
		}

		private static CalendarItemSeries CalendarItemSeriesCreator(ICoreItem coreItem)
		{
			return new CalendarItemSeries(coreItem);
		}

		private static ParkedMeetingMessage ParkedMeetingMessageCreator(ICoreItem coreItem)
		{
			return new ParkedMeetingMessage(coreItem);
		}

		private static MeetingRequest MeetingRequestCreator(ICoreItem coreItem)
		{
			return new MeetingRequest(coreItem);
		}

		private static MeetingResponse MeetingResponseCreator(ICoreItem coreItem)
		{
			return new MeetingResponse(coreItem);
		}

		private static MeetingCancellation MeetingCancellationCreator(ICoreItem coreItem)
		{
			return new MeetingCancellation(coreItem);
		}

		private static MeetingForwardNotification MeetingForwardNotificationCreator(ICoreItem coreItem)
		{
			return new MeetingForwardNotification(coreItem);
		}

		private static MeetingInquiryMessage MeetingInquiryMessageCreator(ICoreItem coreItem)
		{
			return new MeetingInquiryMessage(coreItem);
		}

		private static Contact ContactCreator(ICoreItem coreItem)
		{
			return new Contact(coreItem);
		}

		private static Place PlaceCreator(ICoreItem coreItem)
		{
			return new Place(coreItem);
		}

		private static DistributionList DistributionListCreator(ICoreItem coreItem)
		{
			return new DistributionList(coreItem);
		}

		private static MailboxAssociationGroup MailboxAssociationGroupCreator(ICoreItem coreItem)
		{
			return new MailboxAssociationGroup(coreItem);
		}

		private static MailboxAssociationUser MailboxAssociationUserCreator(ICoreItem coreItem)
		{
			return new MailboxAssociationUser(coreItem);
		}

		private static HierarchySyncMetadataItem HierarchySyncMetadataCreator(ICoreItem coreItem)
		{
			return new HierarchySyncMetadataItem(coreItem);
		}

		private static Task TaskCreator(ICoreItem coreItem)
		{
			return new Task(coreItem);
		}

		private static TaskRequest TaskRequestCreator(ICoreItem coreItem)
		{
			return new TaskRequest(coreItem);
		}

		private static ReminderMessage ReminderMessageCreator(ICoreItem coreItem)
		{
			return new ReminderMessage(coreItem);
		}

		private static ConfigurationItem ConfigurationItemCreator(ICoreItem coreItem)
		{
			return new ConfigurationItem(coreItem);
		}

		private static Item GenericItemCreator(ICoreItem coreItem)
		{
			return new Item(coreItem, false);
		}

		private static ConversationActionItem ConversationActionItemCreator(ICoreItem coreItem)
		{
			return new ConversationActionItem(coreItem);
		}

		private static CalendarGroup CalendarGroupCreator(ICoreItem coreItem)
		{
			return new CalendarGroup(coreItem);
		}

		private static CalendarGroupEntry CalendarGroupEntryCreator(ICoreItem coreItem)
		{
			return new CalendarGroupEntry(coreItem);
		}

		private static FavoriteFolderEntry FavoriteFolderEntryCreator(ICoreItem coreItem)
		{
			return new FavoriteFolderEntry(coreItem);
		}

		private static ShortcutMessage ShortcutMessageCreator(ICoreItem coreItem)
		{
			return new ShortcutMessage(coreItem);
		}

		private static TaskGroup TaskGroupCreator(ICoreItem coreItem)
		{
			return new TaskGroup(coreItem);
		}

		private static TaskGroupEntry TaskGroupEntryCreator(ICoreItem coreItem)
		{
			return new TaskGroupEntry(coreItem);
		}

		private static ItemCreateInfo CreateCalendarItemOccurrenceInfo()
		{
			return new ItemCreateInfo(StoreObjectType.CalendarItemOccurrence, CalendarItemOccurrenceSchema.Instance, AcrProfile.AppointmentProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.CalendarItemOccurrenceCreator));
		}

		private static ItemCreateInfo CreateReportInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Report, ReportMessageSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ReportMessageCreator));
		}

		private static ItemCreateInfo CreateMessageItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Message, MessageItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MessageItemCreator));
		}

		private static ItemCreateInfo CreatePostInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Post, PostItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.PostItemCreator));
		}

		private static ItemCreateInfo CreateCalendarItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.CalendarItem, CalendarItemSchema.Instance, AcrProfile.AppointmentProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.CalendarItemCreator));
		}

		private static ItemCreateInfo CreateCalendarItemSeriesInfo()
		{
			return new ItemCreateInfo(StoreObjectType.CalendarItemSeries, CalendarItemSeriesSchema.Instance, AcrProfile.AppointmentProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.CalendarItemSeriesCreator));
		}

		private static ItemCreateInfo CreateParkedMeetingMessageInfo()
		{
			return new ItemCreateInfo(StoreObjectType.ParkedMeetingMessage, ParkedMeetingMessageSchema.Instance, AcrProfile.BlankProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ParkedMeetingMessageCreator));
		}

		private static ItemCreateInfo CreateMeetingRequestInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingRequest, MeetingRequestSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingRequestCreator));
		}

		private static ItemCreateInfo CreateMeetingRequestSeriesInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingRequestSeries, MeetingRequestSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingRequestCreator));
		}

		private static ItemCreateInfo CreateMeetingResponseInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingResponse, MeetingResponseSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingResponseCreator));
		}

		private static ItemCreateInfo CreateMeetingResponseSeriesInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingResponseSeries, MeetingResponseSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingResponseCreator));
		}

		private static ItemCreateInfo CreateMeetingCancellationInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingCancellation, MeetingMessageInstanceSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingCancellationCreator));
		}

		private static ItemCreateInfo CreateMeetingCancellationSeriesInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingCancellationSeries, MeetingMessageInstanceSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingCancellationCreator));
		}

		private static ItemCreateInfo CreateMeetingForwardNotificationInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingForwardNotification, MeetingForwardNotificationSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingForwardNotificationCreator));
		}

		private static ItemCreateInfo CreateMeetingForwardNotificationSeriesInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingForwardNotificationSeries, MeetingForwardNotificationSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingForwardNotificationCreator));
		}

		private static ItemCreateInfo CreateMeetingInquiryInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MeetingInquiryMessage, MeetingInquiryMessageSchema.Instance, AcrProfile.MeetingMessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MeetingInquiryMessageCreator));
		}

		private static ItemCreateInfo CreateContactInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Contact, ContactSchema.Instance, AcrProfile.ContactProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ContactCreator));
		}

		private static ItemCreateInfo CreatePlaceInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Place, PlaceSchema.Instance, AcrProfile.ContactProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.PlaceCreator));
		}

		private static ItemCreateInfo CreateDistributionListInfo()
		{
			return new ItemCreateInfo(StoreObjectType.DistributionList, DistributionListSchema.Instance, AcrProfile.ContactProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.DistributionListCreator));
		}

		private static ItemCreateInfo CreateMailboxAssociationGroupInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MailboxAssociationGroup, MailboxAssociationGroupSchema.Instance, AcrProfile.MailboxAssociationProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MailboxAssociationGroupCreator));
		}

		private static ItemCreateInfo CreateMailboxAssociationUserInfo()
		{
			return new ItemCreateInfo(StoreObjectType.MailboxAssociationUser, MailboxAssociationUserSchema.Instance, AcrProfile.MailboxAssociationProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MailboxAssociationUserCreator));
		}

		private static ItemCreateInfo CreateHierarchySyncMetadataInfo()
		{
			return new ItemCreateInfo(StoreObjectType.HierarchySyncMetadata, HierarchySyncMetadataItemSchema.Instance, AcrProfile.MailboxAssociationProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.HierarchySyncMetadataCreator));
		}

		private static ItemCreateInfo CreateTaskInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Task, TaskSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.TaskCreator));
		}

		private static ItemCreateInfo CreateTaskRequestInfo()
		{
			return new ItemCreateInfo(StoreObjectType.TaskRequest, TaskRequestSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.TaskRequestCreator));
		}

		private static ItemCreateInfo CreateReminderMessageInfo()
		{
			return new ItemCreateInfo(StoreObjectType.ReminderMessage, ReminderMessageSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ReminderMessageCreator));
		}

		private static ItemCreateInfo CreateGenericItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Unknown, ItemSchema.Instance, AcrProfile.GenericItemProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.GenericItemCreator));
		}

		private static ItemCreateInfo CreateConversationActionInfo()
		{
			return new ItemCreateInfo(StoreObjectType.ConversationActionItem, ConversationActionItemSchema.Instance, AcrProfile.GenericItemProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ConversationActionItemCreator));
		}

		private static ItemCreateInfo CreateOofMessageItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.OofMessage, MessageItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MessageItemCreator));
		}

		private static ItemCreateInfo CreateExternalOofMessageItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.ExternalOofMessage, MessageItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.MessageItemCreator));
		}

		private static ItemCreateInfo CreateCalendarGroupInfo()
		{
			return new ItemCreateInfo(StoreObjectType.CalendarGroup, CalendarGroupSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.CalendarGroupCreator));
		}

		private static ItemCreateInfo CreateCalendarGroupEntryInfo()
		{
			return new ItemCreateInfo(StoreObjectType.CalendarGroupEntry, CalendarGroupEntrySchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.CalendarGroupEntryCreator));
		}

		private static ItemCreateInfo CreateFavoriteFolderEntryInfo()
		{
			return new ItemCreateInfo(StoreObjectType.FavoriteFolderEntry, FavoriteFolderEntrySchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.FavoriteFolderEntryCreator));
		}

		private static ItemCreateInfo CreateShortcutMessageInfo()
		{
			return new ItemCreateInfo(StoreObjectType.ShortcutMessage, ShortcutMessageSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ShortcutMessageCreator));
		}

		private static ItemCreateInfo CreateTaskGroupInfo()
		{
			return new ItemCreateInfo(StoreObjectType.TaskGroup, TaskGroupSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.TaskGroupCreator));
		}

		private static ItemCreateInfo CreateTaskGroupEntryInfo()
		{
			return new ItemCreateInfo(StoreObjectType.TaskGroupEntry, TaskGroupEntrySchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.TaskGroupEntryCreator));
		}

		private static ItemCreateInfo CreateConfigurationItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.Configuration, ConfigurationItemSchema.Instance, AcrProfile.GenericItemProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.ConfigurationItemCreator));
		}

		internal static ItemCreateInfo RightsManagedMessageItemInfo { get; private set; }

		internal static ItemCreateInfo SharingMessageItemInfo { get; private set; }

		internal static ItemCreateInfo PushNotificationSubscriptionItemInfo { get; private set; }

		internal static ItemCreateInfo CalendarItemOccurrenceInfo { get; private set; }

		internal static ItemCreateInfo ReportInfo { get; private set; }

		internal static ItemCreateInfo MessageItemInfo { get; private set; }

		internal static ItemCreateInfo PostInfo { get; private set; }

		internal static ItemCreateInfo CalendarItemInfo { get; private set; }

		internal static ItemCreateInfo CalendarItemSeriesInfo { get; private set; }

		internal static ItemCreateInfo ParkedMeetingMessageInfo { get; private set; }

		internal static ItemCreateInfo MeetingRequestInfo { get; private set; }

		internal static ItemCreateInfo MeetingRequestSeriesInfo { get; private set; }

		internal static ItemCreateInfo MeetingResponseInfo { get; private set; }

		internal static ItemCreateInfo MeetingResponseSeriesInfo { get; private set; }

		internal static ItemCreateInfo MeetingCancellationInfo { get; private set; }

		internal static ItemCreateInfo MeetingCancellationSeriesInfo { get; private set; }

		internal static ItemCreateInfo MeetingForwardNotificationInfo { get; private set; }

		internal static ItemCreateInfo MeetingForwardNotificationSeriesInfo { get; private set; }

		internal static ItemCreateInfo MeetingInquiryInfo { get; private set; }

		internal static ItemCreateInfo ContactInfo { get; private set; }

		internal static ItemCreateInfo PlaceInfo { get; private set; }

		internal static ItemCreateInfo DistributionListInfo { get; private set; }

		internal static ItemCreateInfo MailboxAssociationGroupInfo { get; private set; }

		internal static ItemCreateInfo MailboxAssociationUserInfo { get; private set; }

		internal static ItemCreateInfo HierarchySyncMetadataInfo { get; private set; }

		internal static ItemCreateInfo TaskInfo { get; private set; }

		internal static ItemCreateInfo TaskRequestInfo { get; private set; }

		internal static ItemCreateInfo ReminderMessageInfo { get; private set; }

		internal static ItemCreateInfo GenericItemInfo { get; private set; }

		internal static ItemCreateInfo ConversationActionInfo { get; private set; }

		internal static ItemCreateInfo OofMessageItemInfo { get; private set; }

		internal static ItemCreateInfo ExternalOofMessageItemInfo { get; private set; }

		internal static ItemCreateInfo CalendarGroupInfo { get; private set; }

		internal static ItemCreateInfo CalendarGroupEntryInfo { get; private set; }

		internal static ItemCreateInfo FavoriteFolderEntryInfo { get; private set; }

		internal static ItemCreateInfo ShortcutMessageInfo { get; private set; }

		internal static ItemCreateInfo GroupMailboxJoinRequestMessageInfo { get; private set; }

		internal static ItemCreateInfo TaskGroupInfo { get; private set; }

		internal static ItemCreateInfo TaskGroupEntryInfo { get; private set; }

		internal static ItemCreateInfo OutlookServiceSubscriptionItemInfo { get; private set; }

		internal static ItemCreateInfo ConfigurationItemInfo { get; private set; }

		private static RightsManagedMessageItem RightsManagedMessageItemCreator(ICoreItem coreItem)
		{
			return new RightsManagedMessageItem(coreItem);
		}

		private static SharingMessageItem SharingMessageItemCreator(ICoreItem coreItem)
		{
			return new SharingMessageItem(coreItem);
		}

		private static PushNotificationSubscriptionItem PushNotificationSubscriptionItemCreator(ICoreItem coreItem)
		{
			return new PushNotificationSubscriptionItem(coreItem);
		}

		private static GroupMailboxJoinRequestMessageItem GroupMailboxJoinRequestMessageItemCreator(ICoreItem coreItem)
		{
			return new GroupMailboxJoinRequestMessageItem(coreItem);
		}

		private static OutlookServiceSubscriptionItem OutlookServiceSubscriptionItemCreator(ICoreItem coreItem)
		{
			return new OutlookServiceSubscriptionItem(coreItem);
		}

		private static ItemCreateInfo CreateOutlookServiceSubscriptionItemInfo()
		{
			return new ItemCreateInfo(StoreObjectType.OutlookServiceSubscription, OutlookServiceSubscriptionItemSchema.Instance, AcrProfile.GenericItemProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.OutlookServiceSubscriptionItemCreator));
		}

		internal static ItemCreateInfo GetItemCreateInfo(StoreObjectType storeObjectType)
		{
			ItemCreateInfo result;
			if (!ItemCreateInfo.itemCreateInfoDictionary.TryGetValue(storeObjectType, out result))
			{
				result = ItemCreateInfo.itemCreateInfoDictionary[StoreObjectType.Unknown];
			}
			return result;
		}

		private static Dictionary<StoreObjectType, ItemCreateInfo> CreateDictionary()
		{
			ItemCreateInfo.RightsManagedMessageItemInfo = new ItemCreateInfo(StoreObjectType.RightsManagedMessage, RightsManagedMessageItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.RightsManagedMessageItemCreator));
			ItemCreateInfo.SharingMessageItemInfo = new ItemCreateInfo(StoreObjectType.SharingMessage, SharingMessageItemSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.SharingMessageItemCreator));
			ItemCreateInfo.PushNotificationSubscriptionItemInfo = new ItemCreateInfo(StoreObjectType.PushNotificationSubscription, PushNotificationSubscriptionItemSchema.Instance, AcrProfile.GenericItemProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.PushNotificationSubscriptionItemCreator));
			ItemCreateInfo.GroupMailboxJoinRequestMessageInfo = new ItemCreateInfo(StoreObjectType.GroupMailboxRequestMessage, GroupMailboxJoinRequestMessageSchema.Instance, AcrProfile.MessageProfile, new ItemCreateInfo.ItemCreator(ItemCreateInfo.GroupMailboxJoinRequestMessageItemCreator));
			ItemCreateInfo.CalendarItemOccurrenceInfo = ItemCreateInfo.CreateCalendarItemOccurrenceInfo();
			ItemCreateInfo.ReportInfo = ItemCreateInfo.CreateReportInfo();
			ItemCreateInfo.MessageItemInfo = ItemCreateInfo.CreateMessageItemInfo();
			ItemCreateInfo.PostInfo = ItemCreateInfo.CreatePostInfo();
			ItemCreateInfo.CalendarItemInfo = ItemCreateInfo.CreateCalendarItemInfo();
			ItemCreateInfo.CalendarItemSeriesInfo = ItemCreateInfo.CreateCalendarItemSeriesInfo();
			ItemCreateInfo.ParkedMeetingMessageInfo = ItemCreateInfo.CreateParkedMeetingMessageInfo();
			ItemCreateInfo.MeetingRequestInfo = ItemCreateInfo.CreateMeetingRequestInfo();
			ItemCreateInfo.MeetingRequestSeriesInfo = ItemCreateInfo.CreateMeetingRequestSeriesInfo();
			ItemCreateInfo.MeetingResponseInfo = ItemCreateInfo.CreateMeetingResponseInfo();
			ItemCreateInfo.MeetingResponseSeriesInfo = ItemCreateInfo.CreateMeetingResponseSeriesInfo();
			ItemCreateInfo.MeetingCancellationInfo = ItemCreateInfo.CreateMeetingCancellationInfo();
			ItemCreateInfo.MeetingCancellationSeriesInfo = ItemCreateInfo.CreateMeetingCancellationSeriesInfo();
			ItemCreateInfo.MeetingForwardNotificationInfo = ItemCreateInfo.CreateMeetingForwardNotificationInfo();
			ItemCreateInfo.MeetingForwardNotificationSeriesInfo = ItemCreateInfo.CreateMeetingForwardNotificationSeriesInfo();
			ItemCreateInfo.MeetingInquiryInfo = ItemCreateInfo.CreateMeetingInquiryInfo();
			ItemCreateInfo.ContactInfo = ItemCreateInfo.CreateContactInfo();
			ItemCreateInfo.PlaceInfo = ItemCreateInfo.CreatePlaceInfo();
			ItemCreateInfo.DistributionListInfo = ItemCreateInfo.CreateDistributionListInfo();
			ItemCreateInfo.MailboxAssociationGroupInfo = ItemCreateInfo.CreateMailboxAssociationGroupInfo();
			ItemCreateInfo.MailboxAssociationUserInfo = ItemCreateInfo.CreateMailboxAssociationUserInfo();
			ItemCreateInfo.HierarchySyncMetadataInfo = ItemCreateInfo.CreateHierarchySyncMetadataInfo();
			ItemCreateInfo.TaskInfo = ItemCreateInfo.CreateTaskInfo();
			ItemCreateInfo.TaskRequestInfo = ItemCreateInfo.CreateTaskRequestInfo();
			ItemCreateInfo.ReminderMessageInfo = ItemCreateInfo.CreateReminderMessageInfo();
			ItemCreateInfo.GenericItemInfo = ItemCreateInfo.CreateGenericItemInfo();
			ItemCreateInfo.ConversationActionInfo = ItemCreateInfo.CreateConversationActionInfo();
			ItemCreateInfo.OofMessageItemInfo = ItemCreateInfo.CreateOofMessageItemInfo();
			ItemCreateInfo.ExternalOofMessageItemInfo = ItemCreateInfo.CreateExternalOofMessageItemInfo();
			ItemCreateInfo.CalendarGroupInfo = ItemCreateInfo.CreateCalendarGroupInfo();
			ItemCreateInfo.CalendarGroupEntryInfo = ItemCreateInfo.CreateCalendarGroupEntryInfo();
			ItemCreateInfo.FavoriteFolderEntryInfo = ItemCreateInfo.CreateFavoriteFolderEntryInfo();
			ItemCreateInfo.ShortcutMessageInfo = ItemCreateInfo.CreateShortcutMessageInfo();
			ItemCreateInfo.TaskGroupInfo = ItemCreateInfo.CreateTaskGroupInfo();
			ItemCreateInfo.TaskGroupEntryInfo = ItemCreateInfo.CreateTaskGroupEntryInfo();
			ItemCreateInfo.OutlookServiceSubscriptionItemInfo = ItemCreateInfo.CreateOutlookServiceSubscriptionItemInfo();
			ItemCreateInfo.ConfigurationItemInfo = ItemCreateInfo.CreateConfigurationItemInfo();
			Dictionary<StoreObjectType, ItemCreateInfo> dictionary = new Dictionary<StoreObjectType, ItemCreateInfo>(new StoreObjectTypeComparer());
			ItemCreateInfo[] array = new ItemCreateInfo[]
			{
				ItemCreateInfo.RightsManagedMessageItemInfo,
				ItemCreateInfo.SharingMessageItemInfo,
				ItemCreateInfo.PushNotificationSubscriptionItemInfo,
				ItemCreateInfo.CalendarItemOccurrenceInfo,
				ItemCreateInfo.ReportInfo,
				ItemCreateInfo.MessageItemInfo,
				ItemCreateInfo.PostInfo,
				ItemCreateInfo.CalendarItemInfo,
				ItemCreateInfo.CalendarItemSeriesInfo,
				ItemCreateInfo.ParkedMeetingMessageInfo,
				ItemCreateInfo.MeetingRequestInfo,
				ItemCreateInfo.MeetingRequestSeriesInfo,
				ItemCreateInfo.MeetingResponseInfo,
				ItemCreateInfo.MeetingResponseSeriesInfo,
				ItemCreateInfo.MeetingCancellationInfo,
				ItemCreateInfo.MeetingCancellationSeriesInfo,
				ItemCreateInfo.MeetingForwardNotificationInfo,
				ItemCreateInfo.MeetingForwardNotificationSeriesInfo,
				ItemCreateInfo.MeetingInquiryInfo,
				ItemCreateInfo.ContactInfo,
				ItemCreateInfo.PlaceInfo,
				ItemCreateInfo.DistributionListInfo,
				ItemCreateInfo.MailboxAssociationGroupInfo,
				ItemCreateInfo.MailboxAssociationUserInfo,
				ItemCreateInfo.HierarchySyncMetadataInfo,
				ItemCreateInfo.TaskInfo,
				ItemCreateInfo.TaskRequestInfo,
				ItemCreateInfo.ReminderMessageInfo,
				ItemCreateInfo.GenericItemInfo,
				ItemCreateInfo.ConversationActionInfo,
				ItemCreateInfo.OofMessageItemInfo,
				ItemCreateInfo.ExternalOofMessageItemInfo,
				ItemCreateInfo.CalendarGroupInfo,
				ItemCreateInfo.CalendarGroupEntryInfo,
				ItemCreateInfo.FavoriteFolderEntryInfo,
				ItemCreateInfo.ShortcutMessageInfo,
				ItemCreateInfo.TaskGroupInfo,
				ItemCreateInfo.TaskGroupEntryInfo,
				ItemCreateInfo.GroupMailboxJoinRequestMessageInfo,
				ItemCreateInfo.OutlookServiceSubscriptionItemInfo,
				ItemCreateInfo.ConfigurationItemInfo
			};
			foreach (ItemCreateInfo itemCreateInfo in array)
			{
				dictionary.Add(itemCreateInfo.Type, itemCreateInfo);
			}
			return dictionary;
		}

		internal readonly StoreObjectType Type;

		internal readonly Schema Schema;

		internal readonly AcrProfile AcrProfile;

		internal readonly ItemCreateInfo.ItemCreator Creator;

		private static Dictionary<StoreObjectType, ItemCreateInfo> itemCreateInfoDictionary = ItemCreateInfo.CreateDictionary();

		internal delegate Item ItemCreator(ICoreItem coreItem);
	}
}
