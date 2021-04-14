using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SenderProperty : SingleRecipientPropertyBase
	{
		public SenderProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			MessageItem messageItem = storeItem as MessageItem;
			if (messageItem == null)
			{
				return null;
			}
			return messageItem.Sender;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
			MessageItem messageItem = storeItem as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			messageItem.Sender = participant;
		}

		protected override PropertyDefinition GetParticipantDisplayNamePropertyDefinition()
		{
			return MessageItemSchema.SenderDisplayName;
		}

		protected override PropertyDefinition GetParticipantEmailAddressPropertyDefinition()
		{
			return MessageItemSchema.SenderEmailAddress;
		}

		protected override PropertyDefinition GetParticipantRoutingTypePropertyDefinition()
		{
			return MessageItemSchema.SenderAddressType;
		}

		protected override PropertyDefinition GetParticipantSipUriPropertyDefinition()
		{
			return ParticipantSchema.SipUri;
		}

		public static SenderProperty CreateCommand(CommandContext commandContext)
		{
			return new SenderProperty(commandContext);
		}
	}
}
