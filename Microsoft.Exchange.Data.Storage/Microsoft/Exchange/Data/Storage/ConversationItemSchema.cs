using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationItemSchema : Schema
	{
		public new static ConversationItemSchema Instance
		{
			get
			{
				if (ConversationItemSchema.instance == null)
				{
					ConversationItemSchema.instance = new ConversationItemSchema();
				}
				return ConversationItemSchema.instance;
			}
		}

		public static readonly PropertyDefinition ConversationId = new ConversationIdProperty(InternalSchema.MapiConversationId, "ConversationId");

		public static readonly PropertyDefinition FamilyId = new ConversationIdProperty(InternalSchema.InternalFamilyId, "FamilyId");

		public static readonly PropertyDefinition ConversationTopic = InternalSchema.ConversationTopic;

		public static readonly ReadonlySmartProperty ConversationMVFrom = new ReadonlySmartProperty(InternalSchema.InternalConversationMVFrom);

		public static readonly ReadonlySmartProperty ConversationGlobalMVFrom = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMVFrom);

		public static readonly ReadonlySmartProperty ConversationMVUnreadFrom = new ReadonlySmartProperty(InternalSchema.InternalConversationMVUnreadFrom);

		public static readonly ReadonlySmartProperty ConversationGlobalMVUnreadFrom = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMVUnreadFrom);

		public static readonly ReadonlySmartProperty ConversationMVTo = new ReadonlySmartProperty(InternalSchema.InternalConversationMVTo);

		public static readonly ReadonlySmartProperty ConversationGlobalMVTo = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMVTo);

		public static readonly ReadonlySmartProperty ConversationLastDeliveryTime = new ReadonlySmartProperty(InternalSchema.InternalConversationLastDeliveryTime);

		public static readonly ReadonlySmartProperty ConversationGlobalLastDeliveryTime = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalLastDeliveryTime);

		public static readonly ReadonlySmartProperty ConversationLastDeliveryOrRenewTime = new ReadonlySmartProperty(InternalSchema.InternalConversationLastDeliveryOrRenewTime);

		public static readonly ReadonlySmartProperty ConversationGlobalLastDeliveryOrRenewTime = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalLastDeliveryOrRenewTime);

		public static readonly ReadonlySmartProperty MailboxGuid = new ReadonlySmartProperty(InternalSchema.InternalConversationMailboxGuid);

		public static readonly ReadonlySmartProperty ConversationCategories = new ReadonlySmartProperty(InternalSchema.InternalConversationCategories);

		public static readonly ReadonlySmartProperty ConversationGlobalCategories = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalCategories);

		public static readonly ReadonlySmartProperty ConversationFlagStatus = new ReadonlySmartProperty(InternalSchema.InternalConversationFlagStatus);

		public static readonly ReadonlySmartProperty ConversationGlobalFlagStatus = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalFlagStatus);

		public static readonly ReadonlySmartProperty ConversationFlagCompleteTime = new ReadonlySmartProperty(InternalSchema.InternalConversationFlagCompleteTime);

		public static readonly ReadonlySmartProperty ConversationGlobalFlagCompleteTime = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalFlagCompleteTime);

		public static readonly ReadonlySmartProperty ConversationHasAttach = new ReadonlySmartProperty(InternalSchema.InternalConversationHasAttach);

		public static readonly ReadonlySmartProperty ConversationGlobalHasAttach = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalHasAttach);

		public static readonly ReadonlySmartProperty ConversationHasIrm = new ReadonlySmartProperty(InternalSchema.InternalConversationHasIrm);

		public static readonly ReadonlySmartProperty ConversationGlobalHasIrm = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalHasIrm);

		public static readonly ReadonlySmartProperty ConversationMessageCount = new ReadonlySmartProperty(InternalSchema.InternalConversationMessageCount);

		public static readonly ReadonlySmartProperty ConversationGlobalMessageCount = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMessageCount);

		public static readonly ReadonlySmartProperty ConversationUnreadMessageCount = new ReadonlySmartProperty(InternalSchema.InternalConversationUnreadMessageCount);

		public static readonly ReadonlySmartProperty ConversationGlobalUnreadMessageCount = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalUnreadMessageCount);

		public static readonly ReadonlySmartProperty ConversationMessageSize = new ReadonlySmartProperty(InternalSchema.InternalConversationMessageSize);

		public static readonly ReadonlySmartProperty ConversationGlobalMessageSize = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMessageSize);

		public static readonly ReadonlySmartProperty ConversationMessageClasses = new ReadonlySmartProperty(InternalSchema.InternalConversationMessageClasses);

		public static readonly ReadonlySmartProperty ConversationGlobalMessageClasses = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalMessageClasses);

		public static readonly ReadonlySmartProperty ConversationReplyForwardState = new ReadonlySmartProperty(InternalSchema.InternalConversationReplyForwardState);

		public static readonly ReadonlySmartProperty ConversationGlobalReplyForwardState = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalReplyForwardState);

		public static readonly ReadonlySmartProperty ConversationImportance = new ReadonlySmartProperty(InternalSchema.InternalConversationImportance);

		public static readonly ReadonlySmartProperty ConversationGlobalImportance = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalImportance);

		public static readonly StoreObjectIdCollectionProperty ConversationItemIds = new StoreObjectIdCollectionProperty(InternalSchema.InternalConversationMVItemIds, PropertyFlags.ReadOnly, "Conversation Member ItemIds");

		public static readonly StoreObjectIdCollectionProperty ConversationGlobalItemIds = new StoreObjectIdCollectionProperty(InternalSchema.InternalConversationGlobalMVItemIds, PropertyFlags.ReadOnly, "Conversation Member Global ItemIds");

		public static readonly ReadonlySmartProperty ConversationLastMemberDocumentId = new ReadonlySmartProperty(InternalSchema.InternalConversationLastMemberDocumentId);

		public static readonly ReadonlySmartProperty ConversationGlobalLastMemberDocumentId = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalLastMemberDocumentId);

		public static readonly ReadonlySmartProperty ConversationPreview = new ReadonlySmartProperty(InternalSchema.InternalConversationPreview);

		public static readonly ReadonlySmartProperty ConversationGlobalPreview = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalPreview);

		public static readonly ReadonlySmartProperty ConversationHasClutter = new ReadonlySmartProperty(InternalSchema.InternalConversationHasClutter);

		public static readonly ReadonlySmartProperty ConversationGlobalHasClutter = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalHasClutter);

		public static readonly ReadonlySmartProperty ConversationInitialMemberDocumentId = new ReadonlySmartProperty(InternalSchema.InternalConversationInitialMemberDocumentId);

		public static readonly ReadonlySmartProperty ConversationMemberDocumentIds = new ReadonlySmartProperty(InternalSchema.InternalConversationMemberDocumentIds);

		public static readonly ReadonlySmartProperty ConversationGlobalRichContent = new ReadonlySmartProperty(InternalSchema.InternalConversationGlobalRichContent);

		public static readonly ReadonlySmartProperty ConversationWorkingSetSourcePartition = new ReadonlySmartProperty(InternalSchema.InternalConversationWorkingSetSourcePartition);

		private static ConversationItemSchema instance = null;
	}
}
