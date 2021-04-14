using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PostItemSchema : Schema
	{
		static PostItemSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				PostItemSchema.ConversationIndex,
				PostItemSchema.ConversationTopic,
				PostItemSchema.From,
				PostItemSchema.InternetMessageId,
				PostItemSchema.IsRead,
				PostItemSchema.PostedTime,
				PostItemSchema.References,
				PostItemSchema.Sender
			};
			PostItemSchema.schema = new PostItemSchema(xmlElements);
		}

		private PostItemSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return PostItemSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation ConversationIndex = new PropertyInformation("ConversationIndex", ExchangeVersion.Exchange2007SP1, ItemSchema.ConversationIndex, new PropertyUri(PropertyUriEnum.ConversationIndex), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ConversationTopic = new PropertyInformation("ConversationTopic", ExchangeVersion.Exchange2007SP1, ItemSchema.ConversationTopic, new PropertyUri(PropertyUriEnum.ConversationTopic), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation From = new PropertyInformation("From", ServiceXml.GetFullyQualifiedName("From"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, new PropertyDefinition[]
		{
			ItemSchema.SentRepresentingDisplayName,
			ItemSchema.SentRepresentingEmailAddress,
			ItemSchema.SentRepresentingType
		}, new PropertyUri(PropertyUriEnum.From), new PropertyCommand.CreatePropertyCommand(PostItemFromProperty.CreateCommand));

		public static readonly PropertyInformation InternetMessageId = new PropertyInformation("InternetMessageId", ExchangeVersion.Exchange2007SP1, ItemSchema.InternetMessageId, new PropertyUri(PropertyUriEnum.InternetMessageId), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsRead = new PropertyInformation("IsRead", ExchangeVersion.Exchange2007SP1, MessageItemSchema.IsRead, new PropertyUri(PropertyUriEnum.IsRead), new PropertyCommand.CreatePropertyCommand(IsReadProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation PostedTime = new PropertyInformation("PostedTime", ExchangeVersion.Exchange2007SP1, ItemSchema.SentTime, new PropertyUri(PropertyUriEnum.PostedTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation References = new PropertyInformation("References", ExchangeVersion.Exchange2007SP1, ItemSchema.InternetReferences, new PropertyUri(PropertyUriEnum.References), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Sender = new PropertyInformation("Sender", ServiceXml.GetFullyQualifiedName("Sender"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, new PropertyDefinition[]
		{
			PostItemSchema.SenderDisplayName,
			PostItemSchema.SenderEmailAddress,
			PostItemSchema.SenderAddressType
		}, new PropertyUri(PropertyUriEnum.Sender), new PropertyCommand.CreatePropertyCommand(PostItemSenderProperty.CreateCommand));
	}
}
