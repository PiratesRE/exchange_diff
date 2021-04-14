using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ConversationShape : Shape
	{
		static ConversationShape()
		{
			ConversationShape.defaultProperties.Add(ConversationSchema.ConversationId);
			ConversationShape.defaultProperties.Add(ConversationSchema.ConversationTopic);
			ConversationShape.defaultProperties.Add(ConversationSchema.UniqueRecipients);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalUniqueRecipients);
			ConversationShape.defaultProperties.Add(ConversationSchema.UniqueUnreadSenders);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalUniqueUnreadSenders);
			ConversationShape.defaultProperties.Add(ConversationSchema.UniqueSenders);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalUniqueSenders);
			ConversationShape.defaultProperties.Add(ConversationSchema.LastDeliveryTime);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalLastDeliveryTime);
			ConversationShape.defaultProperties.Add(ConversationSchema.Categories);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalCategories);
			ConversationShape.defaultProperties.Add(ConversationSchema.FlagStatus);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalFlagStatus);
			ConversationShape.defaultProperties.Add(ConversationSchema.HasAttachments);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalHasAttachments);
			ConversationShape.defaultProperties.Add(ConversationSchema.HasIrm);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalHasIrm);
			ConversationShape.defaultProperties.Add(ConversationSchema.MessageCount);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalMessageCount);
			ConversationShape.defaultProperties.Add(ConversationSchema.UnreadCount);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalUnreadCount);
			ConversationShape.defaultProperties.Add(ConversationSchema.Size);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalSize);
			ConversationShape.defaultProperties.Add(ConversationSchema.ItemClasses);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalItemClasses);
			ConversationShape.defaultProperties.Add(ConversationSchema.Importance);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalImportance);
			ConversationShape.defaultProperties.Add(ConversationSchema.ItemIds);
			ConversationShape.defaultProperties.Add(ConversationSchema.GlobalItemIds);
			ConversationShape.defaultProperties.Add(ConversationSchema.LastModifiedTime);
			ConversationShape.defaultProperties.Add(ConversationSchema.InstanceKey);
			ConversationShape.defaultProperties.Add(ConversationSchema.MailboxGuid);
		}

		private ConversationShape(List<PropertyInformation> defaultProperties) : base(Schema.Conversation, ConversationSchema.GetSchema(), null, defaultProperties)
		{
		}

		internal static ConversationShape CreateShape()
		{
			return new ConversationShape(ConversationShape.defaultProperties);
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
