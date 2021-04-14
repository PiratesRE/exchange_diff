using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IXSOFactory
	{
		IContact BindToContact(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IContact CreateContact(IStoreSession session, StoreId contactFolderId);

		IFolder BindToFolder(IStoreSession session, StoreObjectId folderId);

		IFolder BindToFolder(IMailboxSession session, DefaultFolderType defaultFolderType);

		IFolder BindToFolder(IMailboxSession session, DefaultFolderType defaultFolderType, params PropertyDefinition[] propsToReturn);

		ISearchFolder BindToSearchFolder(IMailboxSession session, DefaultFolderType defaultFolderType);

		ISearchFolder BindToSearchFolder(IStoreSession session, StoreId folderId);

		IDistributionList BindToDistributionList(IStoreSession session, StoreId storeId);

		IDistributionList BindToDistributionList(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IDistributionList CreateDistributionList(IStoreSession session, StoreId contactFolderId);

		IMailboxAssociationGroup BindToMailboxAssociationGroup(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IMailboxAssociationGroup CreateMailboxAssociationGroup(IStoreSession session, StoreId folderId);

		IMailboxAssociationUser BindToMailboxAssociationUser(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IMailboxAssociationUser CreateMailboxAssociationUser(IStoreSession session, StoreId folderId);

		IMessageItem BindToMessage(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null);

		IMessageItem Create(IStoreSession session, StoreId destFolderId);

		IMessageItem CreateMessageAssociated(IStoreSession session, StoreId destFolderId);

		ICalendarFolder BindToCalendarFolder(IStoreSession session, StoreId id);

		IItem BindToItem(IStoreSession session, StoreId id, params PropertyDefinition[] propsToReturn);

		ICalendarFolder CreateCalendarFolder(IStoreSession session, StoreId parentFolderId);

		ICalendarGroupEntry BindToCalendarGroupEntry(IMailboxSession session, StoreId id);

		ICalendarGroupEntry CreateCalendarGroupEntry(IMailboxSession session, string legacyDistinguishedName, ICalendarGroup parentGroup);

		ICalendarGroupEntry CreateCalendarGroupEntry(IMailboxSession session, StoreObjectId calendarFolderId, ICalendarGroup parentGroup);

		CalendarGroupInfoList GetCalendarGroupsView(IMailboxSession session);

		ICalendarItemBase BindToCalendarItemBase(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null);

		ICalendarItem CreateCalendarItem(IStoreSession session, StoreId parentFolderId);

		IMeetingMessage BindToMeetingMessage(IStoreSession session, StoreId storeId);

		IMeetingRequest BindToMeetingRequestMessage(IStoreSession session, StoreId storeId);

		ICalendarItemBase CreateCalendarItemSeries(IStoreSession session, StoreId parentFolderId);

		ICalendarGroup CreateCalendarGroup(IMailboxSession session);

		ICalendarGroup BindToCalendarGroup(IMailboxSession session, CalendarGroupType defaultGroupType);

		ICalendarGroup BindToCalendarGroup(IMailboxSession session, Guid groupClassId);

		ICalendarGroup BindToCalendarGroup(IMailboxSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null);

		IHierarchySyncMetadataItem GetHierarchySyncMetadataItem(IStoreSession session, IFolder folder, ICollection<PropertyDefinition> propsToReturn);

		IAttachment CloneAttachment(IAttachment attachment, IItem draftItem);

		IAttachment CreateAttachment(IItem item, AttachmentType type);

		IMailboxSession ConfigurableOpenMailboxSession(ExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit);

		T RunQueryOnAllItemsFolder<T>(IMailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, object seekToValue, T defaultValue, AllItemsFolderHelper.DoQueryProcessing<T> queryProcessor, ICollection<PropertyDefinition> properties);

		T RunQueryOnAllItemsFolder<T>(IMailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, AllItemsFolderHelper.DoQueryProcessing<T> queryProcessor, ICollection<PropertyDefinition> properties);

		IPushNotificationStorage CreatePushNotificationStorage(IMailboxSession session);

		IPushNotificationStorage FindPushNotificationStorage(IMailboxSession session);

		IPushNotificationSubscriptionItem BindToPushNotificationSubscriptionItem(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IPushNotificationSubscriptionItem CreatePushNotificationSubscriptionItem(IStoreSession session, StoreId destFolderId);

		IOutlookServiceSubscriptionItem BindToOutlookServiceSubscriptionItem(IStoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn);

		IOutlookServiceSubscriptionItem CreateOutlookServiceSubscriptionItem(IStoreSession session, StoreId destFolderId);
	}
}
