using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ConversationSchema : Schema
	{
		static ConversationSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				ConversationSchema.ConversationId,
				ConversationSchema.ConversationTopic,
				ConversationSchema.UniqueRecipients,
				ConversationSchema.GlobalUniqueRecipients,
				ConversationSchema.UniqueUnreadSenders,
				ConversationSchema.GlobalUniqueUnreadSenders,
				ConversationSchema.UniqueSenders,
				ConversationSchema.GlobalUniqueSenders,
				ConversationSchema.LastDeliveryTime,
				ConversationSchema.GlobalLastDeliveryTime,
				ConversationSchema.Categories,
				ConversationSchema.GlobalCategories,
				ConversationSchema.FlagStatus,
				ConversationSchema.GlobalFlagStatus,
				ConversationSchema.HasAttachments,
				ConversationSchema.GlobalHasAttachments,
				ConversationSchema.HasIrm,
				ConversationSchema.GlobalHasIrm,
				ConversationSchema.MessageCount,
				ConversationSchema.GlobalMessageCount,
				ConversationSchema.UnreadCount,
				ConversationSchema.GlobalUnreadCount,
				ConversationSchema.Size,
				ConversationSchema.GlobalSize,
				ConversationSchema.ItemClasses,
				ConversationSchema.GlobalItemClasses,
				ConversationSchema.Importance,
				ConversationSchema.GlobalImportance,
				ConversationSchema.ItemIds,
				ConversationSchema.GlobalItemIds,
				ConversationSchema.LastModifiedTime,
				ConversationSchema.InstanceKey,
				ConversationSchema.Preview,
				ConversationSchema.IconIndex,
				ConversationSchema.GlobalIconIndex,
				ConversationSchema.DraftItemIds,
				ConversationSchema.HasClutter,
				ConversationSchema.InitialPost,
				ConversationSchema.RecentReplys,
				ConversationSchema.FamilyId,
				ConversationSchema.GlobalLastDeliveryOrRenewTime,
				ConversationSchema.GlobalRichContent,
				ConversationSchema.MailboxGuid,
				ConversationSchema.LastDeliveryOrRenewTime,
				ConversationSchema.WorkingSetSourcePartition
			};
			ConversationSchema.schema = new ConversationSchema(xmlElements, ConversationSchema.ConversationId);
		}

		private ConversationSchema(XmlElementInformation[] xmlElements, PropertyInformation conversationIdPropertyInformation) : base(xmlElements, conversationIdPropertyInformation)
		{
		}

		public static Schema GetSchema()
		{
			return ConversationSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation ConversationId = new PropertyInformation("ConversationId", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationId, new PropertyUri(PropertyUriEnum.ConversationGuidId), new PropertyCommand.CreatePropertyCommand(ConversationIdProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ConversationTopic = new PropertyInformation("ConversationTopic", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationTopic, new PropertyUri(PropertyUriEnum.Topic), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation UniqueRecipients = new ArrayPropertyInformation("UniqueRecipients", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationMVTo, new PropertyUri(PropertyUriEnum.ConversationUniqueRecipients), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation GlobalUniqueRecipients = new ArrayPropertyInformation("GlobalUniqueRecipients", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationGlobalMVTo, new PropertyUri(PropertyUriEnum.ConversationGlobalUniqueRecipients), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation UniqueUnreadSenders = new ArrayPropertyInformation("UniqueUnreadSenders", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationMVUnreadFrom, new PropertyUri(PropertyUriEnum.ConversationUniqueUnreadSenders), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation GlobalUniqueUnreadSenders = new ArrayPropertyInformation("GlobalUniqueUnreadSenders", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationGlobalMVUnreadFrom, new PropertyUri(PropertyUriEnum.ConversationGlobalUniqueUnreadSenders), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation UniqueSenders = new ArrayPropertyInformation("UniqueSenders", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationMVFrom, new PropertyUri(PropertyUriEnum.ConversationUniqueSenders), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation GlobalUniqueSenders = new ArrayPropertyInformation("GlobalUniqueSenders", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationGlobalMVFrom, new PropertyUri(PropertyUriEnum.ConversationGlobalUniqueSenders), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation LastDeliveryTime = new PropertyInformation("LastDeliveryTime", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationLastDeliveryTime, new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalLastDeliveryTime = new PropertyInformation("GlobalLastDeliveryTime", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalLastDeliveryTime, new PropertyUri(PropertyUriEnum.ConversationGlobalLastDeliveryTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalLastDeliveryOrRenewTime = new PropertyInformation("GlobalLastDeliveryOrRenewTime", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationGlobalLastDeliveryOrRenewTime, new PropertyUri(PropertyUriEnum.ConversationGlobalLastDeliveryOrRenewTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalRichContent = new ArrayPropertyInformation("GlobalRichContent", ExchangeVersion.Exchange2012, "RichContent", ConversationItemSchema.ConversationGlobalRichContent, new PropertyUri(PropertyUriEnum.ConversationGlobalRichContent), new PropertyCommand.CreatePropertyCommand(ShortArrayValueProperty.CreateCommand));

		public static readonly PropertyInformation MailboxGuid = new PropertyInformation("MailboxGuid", ExchangeVersion.Exchange2012, ConversationItemSchema.MailboxGuid, new PropertyUri(PropertyUriEnum.ConversationMailboxGuid), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Categories = new ArrayPropertyInformation("Categories", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationCategories, new PropertyUri(PropertyUriEnum.ConversationCategories), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation GlobalCategories = new ArrayPropertyInformation("GlobalCategories", ExchangeVersion.Exchange2010SP1, "String", ConversationItemSchema.ConversationGlobalCategories, new PropertyUri(PropertyUriEnum.ConversationGlobalCategories), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation FlagStatus = new PropertyInformation("FlagStatus", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationFlagStatus, new PropertyUri(PropertyUriEnum.ConversationFlagStatus), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalFlagStatus = new PropertyInformation("GlobalFlagStatus", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalFlagStatus, new PropertyUri(PropertyUriEnum.ConversationGlobalFlagStatus), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation HasAttachments = new PropertyInformation("HasAttachments", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationHasAttach, new PropertyUri(PropertyUriEnum.ConversationHasAttachments), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalHasAttachments = new PropertyInformation("GlobalHasAttachments", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalHasAttach, new PropertyUri(PropertyUriEnum.ConversationGlobalHasAttachments), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation HasIrm = new PropertyInformation("HasIrm", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationHasIrm, new PropertyUri(PropertyUriEnum.ConversationHasIrm), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalHasIrm = new PropertyInformation("GlobalHasIrm", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationGlobalHasIrm, new PropertyUri(PropertyUriEnum.ConversationGlobalHasIrm), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation MessageCount = new PropertyInformation("MessageCount", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationMessageCount, new PropertyUri(PropertyUriEnum.ConversationMessageCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalMessageCount = new PropertyInformation("GlobalMessageCount", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalMessageCount, new PropertyUri(PropertyUriEnum.ConversationGlobalMessageCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation UnreadCount = new PropertyInformation("UnreadCount", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationUnreadMessageCount, new PropertyUri(PropertyUriEnum.ConversationUnreadCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommandForPropertyWithDefaultValue), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalUnreadCount = new PropertyInformation("GlobalUnreadCount", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalUnreadMessageCount, new PropertyUri(PropertyUriEnum.ConversationGlobalUnreadCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommandForPropertyWithDefaultValue), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Size = new PropertyInformation("Size", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationMessageSize, new PropertyUri(PropertyUriEnum.ConversationSize), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalSize = new PropertyInformation("GlobalSize", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalMessageSize, new PropertyUri(PropertyUriEnum.ConversationGlobalSize), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ItemClasses = new ArrayPropertyInformation("ItemClasses", ExchangeVersion.Exchange2010SP1, "ItemClass", ConversationItemSchema.ConversationMessageClasses, new PropertyUri(PropertyUriEnum.ConversationItemClasses), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation GlobalItemClasses = new ArrayPropertyInformation("GlobalItemClasses", ExchangeVersion.Exchange2010SP1, "ItemClass", ConversationItemSchema.ConversationGlobalMessageClasses, new PropertyUri(PropertyUriEnum.ConversationGlobalItemClasses), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Importance = new PropertyInformation("Importance", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationImportance, new PropertyUri(PropertyUriEnum.ConversationImportance), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalImportance = new PropertyInformation("GlobalImportance", ExchangeVersion.Exchange2010SP1, ConversationItemSchema.ConversationGlobalImportance, new PropertyUri(PropertyUriEnum.ConversationGlobalImportance), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ItemIds = new ArrayPropertyInformation("ItemIds", ExchangeVersion.Exchange2010SP1, "ItemId", ConversationItemSchema.ConversationItemIds, new PropertyUri(PropertyUriEnum.ConversationItemIds), new PropertyCommand.CreatePropertyCommand(ItemIdProperty.CreateCommand));

		public static readonly PropertyInformation GlobalItemIds = new ArrayPropertyInformation("GlobalItemIds", ExchangeVersion.Exchange2010SP1, "ItemId", ConversationItemSchema.ConversationGlobalItemIds, new PropertyUri(PropertyUriEnum.ConversationGlobalItemIds), new PropertyCommand.CreatePropertyCommand(ItemIdProperty.CreateCommand));

		public static readonly PropertyInformation LastModifiedTime = new PropertyInformation("LastModifiedTime", ExchangeVersion.Exchange2012, StoreObjectSchema.LastModifiedTime, new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation InstanceKey = new PropertyInformation("InstanceKey", ExchangeVersion.Exchange2012, ItemSchema.InstanceKey, new PropertyUri(PropertyUriEnum.ConversationInstanceKey), new PropertyCommand.CreatePropertyCommand(InstanceKeyProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Preview = new PropertyInformation("Preview", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationPreview, new PropertyUri(PropertyUriEnum.ConversationPreview), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalPreview = new PropertyInformation("GlobalPreview", ExchangeVersion.Exchange2013, ConversationItemSchema.ConversationGlobalPreview, new PropertyUri(PropertyUriEnum.ConversationGlobalPreview), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation InitialPost = new PropertyInformation("InitialPost", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationInitialMemberDocumentId, new PropertyUri(PropertyUriEnum.ConversationInitialPost), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation RecentReplys = new PropertyInformation("RecentReplys", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationMemberDocumentIds, new PropertyUri(PropertyUriEnum.ConversationRecentReplys), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation WorkingSetSourcePartition = new PropertyInformation("WorkingSetSourcePartition", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationWorkingSetSourcePartition, new PropertyUri(PropertyUriEnum.ConversationWorkingSetSourcePartition), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IconIndex = new PropertyInformation("IconIndex", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationReplyForwardState, new PropertyUri(PropertyUriEnum.ConversationIconIndex), new PropertyCommand.CreatePropertyCommand(IconIndexProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GlobalIconIndex = new PropertyInformation("GlobalIconIndex", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationGlobalReplyForwardState, new PropertyUri(PropertyUriEnum.ConversationGlobalIconIndex), new PropertyCommand.CreatePropertyCommand(IconIndexProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DraftItemIds = new ArrayPropertyInformation("DraftItemIds", ExchangeVersion.Exchange2012, "ItemId", ConversationItemSchema.ConversationGlobalItemIds, new PropertyUri(PropertyUriEnum.ConversationDraftItemIds), new PropertyCommand.CreatePropertyCommand(DraftItemIdsProperty.CreateCommand));

		public static readonly PropertyInformation HasClutter = new PropertyInformation("HasClutter", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationHasClutter, new PropertyUri(PropertyUriEnum.ConversationHasClutter), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation FamilyId = new PropertyInformation("FamilyId", ExchangeVersion.Exchange2012, ConversationItemSchema.FamilyId, new PropertyUri(PropertyUriEnum.FamilyId), new PropertyCommand.CreatePropertyCommand(ConversationIdProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation LastDeliveryOrRenewTime = new PropertyInformation("LastDeliveryOrRenewTime", ExchangeVersion.Exchange2012, ConversationItemSchema.ConversationLastDeliveryOrRenewTime, new PropertyUri(PropertyUriEnum.ConversationLastDeliveryOrRenewTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);
	}
}
