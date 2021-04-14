using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MessageSchema : Schema
	{
		static MessageSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				MessageSchema.Sender,
				MessageSchema.ToRecipients,
				MessageSchema.CcRecipients,
				MessageSchema.BccRecipients,
				MessageSchema.IsReadReceiptRequested,
				MessageSchema.IsDeliveryReceiptRequested,
				MessageSchema.RelyOnConversationIndex,
				MessageSchema.IsSpecificMessageReplyStamped,
				MessageSchema.IsSpecificMessageReply,
				MessageSchema.ConversationIndex,
				MessageSchema.ConversationTopic,
				MessageSchema.From,
				MessageSchema.InternetMessageId,
				MessageSchema.IsRead,
				MessageSchema.IsResponseRequested,
				MessageSchema.References,
				MessageSchema.ReplyTo,
				MessageSchema.ReceivedBy,
				MessageSchema.ReceivedRepresenting,
				MessageSchema.ApprovalRequestData,
				MessageSchema.VotingInformation,
				MessageSchema.ReminderMessageData,
				MessageSchema.ModernReminders,
				MessageSchema.LikeCount,
				MessageSchema.RecipientCounts,
				MessageSchema.Likers,
				MessageSchema.IsGroupEscalationMessage
			};
			MessageSchema.schema = new MessageSchema(xmlElements);
		}

		private MessageSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(MessageSchema.ApprovalRequestData);
			propertyInformationListByShapeEnum.Remove(MessageSchema.VotingInformation);
			propertyInformationListByShapeEnum.Remove(MessageSchema.ReminderMessageData);
			propertyInformationListByShapeEnum.Remove(MessageSchema.ModernReminders);
			propertyInformationListByShapeEnum.Remove(MessageSchema.RecipientCounts);
		}

		public static Schema GetSchema()
		{
			return MessageSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation BccRecipients = new PropertyInformation("BccRecipients", ExchangeVersion.Exchange2007, null, new PropertyUri(PropertyUriEnum.BccRecipients), new PropertyCommand.CreatePropertyCommand(BccRecipientsProperty.CreateCommand));

		public static readonly PropertyInformation CcRecipients = new PropertyInformation("CcRecipients", ExchangeVersion.Exchange2007, null, new PropertyUri(PropertyUriEnum.CcRecipients), new PropertyCommand.CreatePropertyCommand(CcRecipientsProperty.CreateCommand));

		public static readonly PropertyInformation ConversationIndex = new PropertyInformation("ConversationIndex", ExchangeVersion.Exchange2007, ItemSchema.ConversationIndex, new PropertyUri(PropertyUriEnum.ConversationIndex), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ConversationTopic = new PropertyInformation("ConversationTopic", ExchangeVersion.Exchange2007, ItemSchema.ConversationTopic, new PropertyUri(PropertyUriEnum.ConversationTopic), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation From = new PropertyInformation("From", ServiceXml.GetFullyQualifiedName("From"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, new PropertyDefinition[]
		{
			ItemSchema.SentRepresentingDisplayName,
			ItemSchema.SentRepresentingType,
			ItemSchema.SentRepresentingEmailAddress,
			ItemSchema.From,
			MessageItemSchema.IsDraft,
			MessageItemSchema.SharingInstanceGuid
		}, new PropertyUri(PropertyUriEnum.From), new PropertyCommand.CreatePropertyCommand(FromProperty.CreateCommand));

		public static readonly PropertyInformation InternetMessageId = new PropertyInformation("InternetMessageId", ExchangeVersion.Exchange2007, ItemSchema.InternetMessageId, new PropertyUri(PropertyUriEnum.InternetMessageId), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation IsDeliveryReceiptRequested = new PropertyInformation("IsDeliveryReceiptRequested", ExchangeVersion.Exchange2007, MessageItemSchema.IsDeliveryReceiptRequested, new PropertyUri(PropertyUriEnum.IsDeliveryReceiptRequested), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateIsDeliveryReceiptRequestedCommand));

		public static readonly PropertyInformation IsRead = new PropertyInformation("IsRead", ExchangeVersion.Exchange2007, MessageItemSchema.IsRead, new PropertyUri(PropertyUriEnum.IsRead), new PropertyCommand.CreatePropertyCommand(IsReadProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation IsReadReceiptRequested = new PropertyInformation("IsReadReceiptRequested", ExchangeVersion.Exchange2007, MessageItemSchema.IsReadReceiptRequested, new PropertyUri(PropertyUriEnum.IsReadReceiptRequested), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateIsReadReceiptRequestedCommand));

		public static readonly PropertyInformation IsResponseRequested = new PropertyInformation("IsResponseRequested", ExchangeVersion.Exchange2007, ItemSchema.IsResponseRequested, new PropertyUri(PropertyUriEnum.IsResponseRequested), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation ReceivedBy = new PropertyInformation("ReceivedBy", ServiceXml.GetFullyQualifiedName("ReceivedBy"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, new PropertyDefinition[]
		{
			MessageItemSchema.ReceivedByName,
			MessageItemSchema.ReceivedByAddrType,
			MessageItemSchema.ReceivedByEmailAddress,
			MessageItemSchema.ReceivedBy
		}, new PropertyUri(PropertyUriEnum.ReceivedBy), new PropertyCommand.CreatePropertyCommand(ParticipantProperty.CreateCommandForReceivedBy), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ReceivedRepresenting = new PropertyInformation("ReceivedRepresenting", ServiceXml.GetFullyQualifiedName("ReceivedRepresenting"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, new PropertyDefinition[]
		{
			MessageItemSchema.ReceivedRepresentingDisplayName,
			MessageItemSchema.ReceivedRepresentingAddressType,
			MessageItemSchema.ReceivedRepresentingEmailAddress,
			MessageItemSchema.ReceivedRepresenting
		}, new PropertyUri(PropertyUriEnum.ReceivedRepresenting), new PropertyCommand.CreatePropertyCommand(ParticipantProperty.CreateCommandForReceivedRepresenting), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation References = new PropertyInformation("References", ExchangeVersion.Exchange2007, ItemSchema.InternetReferences, new PropertyUri(PropertyUriEnum.References), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation ReplyTo = new PropertyInformation("ReplyTo", ExchangeVersion.Exchange2007, MessageItemSchema.ReplyToNames, new PropertyUri(PropertyUriEnum.ReplyTo), new PropertyCommand.CreatePropertyCommand(ReplyToProperty.CreateCommand));

		public static readonly PropertyInformation Sender = new PropertyInformation("Sender", ServiceXml.GetFullyQualifiedName("Sender"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, new PropertyDefinition[]
		{
			MessageItemSchema.SenderDisplayName,
			MessageItemSchema.SenderAddressType,
			MessageItemSchema.SenderEmailAddress,
			ItemSchema.Sender
		}, new PropertyUri(PropertyUriEnum.Sender), new PropertyCommand.CreatePropertyCommand(SenderProperty.CreateCommand));

		public static readonly PropertyInformation ToRecipients = new PropertyInformation("ToRecipients", ExchangeVersion.Exchange2007, null, new PropertyUri(PropertyUriEnum.ToRecipients), new PropertyCommand.CreatePropertyCommand(ToRecipientsProperty.CreateCommand));

		public static readonly PropertyInformation ApprovalRequestData = new PropertyInformation(PropertyUriEnum.ApprovalRequestData.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.ApprovalRequestData.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalDecision,
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalDecisionTime
		}, new PropertyUri(PropertyUriEnum.ApprovalRequestData), new PropertyCommand.CreatePropertyCommand(ApprovalRequestDataProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation ReminderMessageData = new PropertyInformation(PropertyUriEnum.ReminderMessageData.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.ReminderMessageData.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2013, new PropertyDefinition[]
		{
			ReminderMessageSchema.ReminderText,
			CalendarItemBaseSchema.Location,
			ReminderMessageSchema.ReminderStartTime,
			ReminderMessageSchema.ReminderEndTime,
			ReminderMessageSchema.ReminderItemGlobalObjectId,
			ReminderMessageSchema.ReminderOccurrenceGlobalObjectId
		}, new PropertyUri(PropertyUriEnum.ReminderMessageData), new PropertyCommand.CreatePropertyCommand(ReminderMessageDataProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation VotingInformation = new PropertyInformation(PropertyUriEnum.VotingInformation.ToString(), ExchangeVersion.Exchange2012, MessageItemSchema.VotingBlob, new PropertyUri(PropertyUriEnum.VotingInformation), new PropertyCommand.CreatePropertyCommand(VotingInformationProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation RelyOnConversationIndex = new PropertyInformation("RelyOnConversationIndex", ExchangeVersion.Exchange2013, MessageItemSchema.RelyOnConversationIndex, new PropertyUri(PropertyUriEnum.RelyOnConversationIndex), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation IsSpecificMessageReplyStamped = new PropertyInformation("IsSpecificMessageReplyStamped", ExchangeVersion.Exchange2013, MessageItemSchema.IsSpecificMessageReplyStamped, new PropertyUri(PropertyUriEnum.IsSpecificMessageReplyStamped), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation IsSpecificMessageReply = new PropertyInformation("IsSpecificMessageReply", ExchangeVersion.Exchange2013, MessageItemSchema.IsSpecificMessageReply, new PropertyUri(PropertyUriEnum.IsSpecificMessageReply), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation ModernReminders = new PropertyInformation("ModernReminders", ExchangeVersion.Exchange2013, MessageItemSchema.QuickCaptureReminders, new PropertyUri(PropertyUriEnum.ModernReminders), new PropertyCommand.CreatePropertyCommand(ModernRemindersMessageProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetUpdateCommand);

		public static readonly PropertyInformation LikeCount = new PropertyInformation("LikeCount", ExchangeVersion.Exchange2013, MessageItemSchema.LikeCount, new PropertyUri(PropertyUriEnum.LikeCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Likers = new PropertyInformation("Likers", ExchangeVersion.Exchange2013, MessageItemSchema.LikersBlob, new PropertyUri(PropertyUriEnum.Likers), new PropertyCommand.CreatePropertyCommand(LikersProperty.CreateCommand));

		public static readonly PropertyInformation RecipientCounts = new PropertyInformation("RecipientCounts", ExchangeVersion.Exchange2013, null, new PropertyUri(PropertyUriEnum.RecipientCounts), new PropertyCommand.CreatePropertyCommand(RecipientCountsProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation IsGroupEscalationMessage = new PropertyInformation("IsGroupEscalationMessage", ExchangeVersion.Exchange2013, MessageItemSchema.IsGroupEscalationMessage, new PropertyUri(PropertyUriEnum.IsGroupEscalationMessage), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);
	}
}
