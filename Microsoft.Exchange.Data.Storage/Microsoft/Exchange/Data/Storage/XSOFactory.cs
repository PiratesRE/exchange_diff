using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XSOFactory : IXSOFactory
	{
		public IContact BindToContact(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return Contact.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public IContact CreateContact(IStoreSession session, StoreId contactFolderId)
		{
			return Contact.Create((StoreSession)session, contactFolderId);
		}

		public IFolder BindToFolder(IStoreSession session, StoreObjectId folderId)
		{
			return Folder.Bind((StoreSession)session, folderId);
		}

		public IFolder BindToFolder(IMailboxSession session, DefaultFolderType defaultFolderType)
		{
			return Folder.Bind((MailboxSession)session, defaultFolderType);
		}

		public IFolder BindToFolder(IMailboxSession session, DefaultFolderType defaultFolderType, params PropertyDefinition[] propsToReturn)
		{
			return Folder.Bind((MailboxSession)session, defaultFolderType, propsToReturn);
		}

		public ISearchFolder BindToSearchFolder(IMailboxSession session, DefaultFolderType defaultFolderType)
		{
			return SearchFolder.Bind((MailboxSession)session, defaultFolderType);
		}

		public ISearchFolder BindToSearchFolder(IStoreSession session, StoreId folderId)
		{
			return SearchFolder.Bind((StoreSession)session, folderId);
		}

		public IDistributionList BindToDistributionList(IStoreSession session, StoreId storeId)
		{
			return DistributionList.Bind((StoreSession)session, storeId);
		}

		public IDistributionList BindToDistributionList(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return DistributionList.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public IDistributionList CreateDistributionList(IStoreSession session, StoreId contactFolderId)
		{
			return DistributionList.Create((StoreSession)session, contactFolderId);
		}

		public IMailboxAssociationGroup BindToMailboxAssociationGroup(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return MailboxAssociationGroup.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public IMailboxAssociationGroup CreateMailboxAssociationGroup(IStoreSession session, StoreId folderId)
		{
			return MailboxAssociationGroup.Create((StoreSession)session, folderId);
		}

		public IMailboxAssociationUser BindToMailboxAssociationUser(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return MailboxAssociationUser.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public IMailboxAssociationUser CreateMailboxAssociationUser(IStoreSession session, StoreId folderId)
		{
			return MailboxAssociationUser.Create((StoreSession)session, folderId);
		}

		public IMessageItem BindToMessage(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null)
		{
			return MessageItem.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public IMessageItem Create(IStoreSession session, StoreId destFolderId)
		{
			return MessageItem.Create((StoreSession)session, destFolderId);
		}

		public IMessageItem CreateMessageAssociated(IStoreSession session, StoreId destFolderId)
		{
			return MessageItem.CreateAssociated((StoreSession)session, destFolderId);
		}

		public ICalendarFolder BindToCalendarFolder(IStoreSession session, StoreId id)
		{
			return CalendarFolder.Bind((StoreSession)session, id);
		}

		public IItem BindToItem(IStoreSession session, StoreId id, params PropertyDefinition[] propsToReturn)
		{
			return Item.Bind((StoreSession)session, id, propsToReturn);
		}

		public ICalendarFolder CreateCalendarFolder(IStoreSession session, StoreId parentFolderId)
		{
			return CalendarFolder.Create((StoreSession)session, parentFolderId);
		}

		public ICalendarGroupEntry BindToCalendarGroupEntry(IMailboxSession session, StoreId id)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			if (!storeObjectId.IsFolderId)
			{
				return CalendarGroupEntry.Bind((MailboxSession)session, id, null);
			}
			return CalendarGroupEntry.BindFromCalendarFolder((MailboxSession)session, storeObjectId);
		}

		public ICalendarGroupEntry CreateCalendarGroupEntry(IMailboxSession session, string legacyDistinguishedName, ICalendarGroup parentGroup)
		{
			return CalendarGroupEntry.Create((MailboxSession)session, legacyDistinguishedName, (CalendarGroup)parentGroup);
		}

		public ICalendarGroupEntry CreateCalendarGroupEntry(IMailboxSession session, StoreObjectId calendarFolderId, ICalendarGroup parentGroup)
		{
			return CalendarGroupEntry.Create((MailboxSession)session, calendarFolderId, (CalendarGroup)parentGroup);
		}

		public CalendarGroupInfoList GetCalendarGroupsView(IMailboxSession session)
		{
			return CalendarGroup.GetCalendarGroupsView((MailboxSession)session);
		}

		public ICalendarItemBase BindToCalendarItemBase(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null)
		{
			return CalendarItemBase.Bind((StoreSession)session, storeId, propsToReturn);
		}

		public ICalendarItem CreateCalendarItem(IStoreSession session, StoreId parentFolderId)
		{
			return CalendarItem.Create((StoreSession)session, parentFolderId);
		}

		public IMeetingMessage BindToMeetingMessage(IStoreSession session, StoreId storeId)
		{
			return MeetingMessage.Bind((StoreSession)session, storeId);
		}

		public IMeetingRequest BindToMeetingRequestMessage(IStoreSession session, StoreId storeId)
		{
			return MeetingRequest.Bind((StoreSession)session, storeId);
		}

		public ICalendarItemBase CreateCalendarItemSeries(IStoreSession session, StoreId parentFolderId)
		{
			return CalendarItemSeries.CreateSeries((StoreSession)session, parentFolderId, true);
		}

		public ICalendarGroup CreateCalendarGroup(IMailboxSession session)
		{
			return CalendarGroup.Create((MailboxSession)session);
		}

		public ICalendarGroup BindToCalendarGroup(IMailboxSession session, CalendarGroupType defaultGroupType)
		{
			return CalendarGroup.Bind((MailboxSession)session, defaultGroupType);
		}

		public ICalendarGroup BindToCalendarGroup(IMailboxSession session, Guid groupClassId)
		{
			return CalendarGroup.Bind((MailboxSession)session, groupClassId);
		}

		public ICalendarGroup BindToCalendarGroup(IMailboxSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null)
		{
			return CalendarGroup.Bind((MailboxSession)session, storeId, propsToReturn);
		}

		public IHierarchySyncMetadataItem GetHierarchySyncMetadataItem(IStoreSession session, IFolder folder, ICollection<PropertyDefinition> propsToReturn)
		{
			return new HierarchySyncMetadataItemHandler().GetItem(session, folder, propsToReturn);
		}

		public IAttachment CloneAttachment(IAttachment attachment, IItem draftItem)
		{
			AttachmentType type = AttachmentType.Stream;
			if (attachment is IReferenceAttachment)
			{
				type = AttachmentType.Reference;
			}
			return draftItem.IAttachmentCollection.CreateIAttachment(type, attachment);
		}

		public IAttachment CreateAttachment(IItem item, AttachmentType type)
		{
			return item.IAttachmentCollection.CreateIAttachment(type);
		}

		public IMailboxSession ConfigurableOpenMailboxSession(ExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit)
		{
			return MailboxSession.ConfigurableOpen(mailbox, accessInfo, cultureInfo, clientInfoString, logonType, mailboxProperties, initFlags, foldersToInit);
		}

		public T RunQueryOnAllItemsFolder<T>(IMailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, object seekToValue, T defaultValue, AllItemsFolderHelper.DoQueryProcessing<T> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			return AllItemsFolderHelper.RunQueryOnAllItemsFolder<T>((MailboxSession)session, supportedSortBy, seekToValue, defaultValue, queryProcessor, properties);
		}

		public G RunQueryOnAllItemsFolder<G>(IMailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, AllItemsFolderHelper.DoQueryProcessing<G> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			return AllItemsFolderHelper.RunQueryOnAllItemsFolder<G>((MailboxSession)session, supportedSortBy, queryProcessor, properties);
		}

		public IPushNotificationStorage CreatePushNotificationStorage(IMailboxSession session)
		{
			return PushNotificationStorage.Create(session, this);
		}

		public IPushNotificationStorage FindPushNotificationStorage(IMailboxSession session)
		{
			return PushNotificationStorage.Find(session, this);
		}

		public IPushNotificationSubscriptionItem BindToPushNotificationSubscriptionItem(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<PushNotificationSubscriptionItem>((StoreSession)session, storeId, PushNotificationSubscriptionItemSchema.Instance, propsToReturn);
		}

		public IPushNotificationSubscriptionItem CreatePushNotificationSubscriptionItem(IStoreSession session, StoreId destFolderId)
		{
			return ItemBuilder.CreateNewItem<PushNotificationSubscriptionItem>((StoreSession)session, destFolderId, ItemCreateInfo.PushNotificationSubscriptionItemInfo);
		}

		public IOutlookServiceSubscriptionItem BindToOutlookServiceSubscriptionItem(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<OutlookServiceSubscriptionItem>((StoreSession)session, storeId, OutlookServiceSubscriptionItemSchema.Instance, propsToReturn);
		}

		public IOutlookServiceSubscriptionItem CreateOutlookServiceSubscriptionItem(IStoreSession session, StoreId destFolderId)
		{
			return ItemBuilder.CreateNewItem<OutlookServiceSubscriptionItem>((StoreSession)session, destFolderId, ItemCreateInfo.OutlookServiceSubscriptionItemInfo);
		}

		public static readonly XSOFactory Default = new XSOFactory();
	}
}
