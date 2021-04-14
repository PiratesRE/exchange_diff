using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ParticipantProperty : SingleRecipientPropertyBase, IToXmlCommand, IPropertyCommand
	{
		private ParticipantProperty(CommandContext commandContext, PropertyDefinition displayNamePropDef, PropertyDefinition routingTypePropDef, PropertyDefinition emailAddressPropDef, PropertyDefinition participantPropDef, PropertyDefinition sipUriPropDef) : base(commandContext)
		{
			this.displayNamePropertyDefinition = displayNamePropDef;
			this.routingTypePropertyDefinition = routingTypePropDef;
			this.emailAddressPropertyDefinition = emailAddressPropDef;
			this.participantPropertyDefinition = participantPropDef;
			this.sipUriPropertyDefinition = sipUriPropDef;
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			Participant result = null;
			if (PropertyCommand.StorePropertyExists(storeItem, this.participantPropertyDefinition))
			{
				result = (storeItem[this.participantPropertyDefinition] as Participant);
			}
			return result;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
		}

		protected override PropertyDefinition GetParticipantDisplayNamePropertyDefinition()
		{
			return this.displayNamePropertyDefinition;
		}

		protected override PropertyDefinition GetParticipantEmailAddressPropertyDefinition()
		{
			return this.emailAddressPropertyDefinition;
		}

		protected override PropertyDefinition GetParticipantRoutingTypePropertyDefinition()
		{
			return this.routingTypePropertyDefinition;
		}

		protected override PropertyDefinition GetParticipantSipUriPropertyDefinition()
		{
			return this.sipUriPropertyDefinition;
		}

		public static ParticipantProperty CreateCommandForReceivedBy(CommandContext commandContext)
		{
			return new ParticipantProperty(commandContext, MessageItemSchema.ReceivedByName, MessageItemSchema.ReceivedByAddrType, MessageItemSchema.ReadReceiptEmailAddress, MessageItemSchema.ReceivedBy, ParticipantSchema.SipUri);
		}

		public static ParticipantProperty CreateCommandForReceivedRepresenting(CommandContext commandContext)
		{
			return new ParticipantProperty(commandContext, MessageItemSchema.ReceivedRepresentingDisplayName, MessageItemSchema.ReceivedRepresentingAddressType, MessageItemSchema.ReceivedRepresentingEmailAddress, MessageItemSchema.ReceivedRepresenting, ParticipantSchema.SipUri);
		}

		private PropertyDefinition displayNamePropertyDefinition;

		private PropertyDefinition routingTypePropertyDefinition;

		private PropertyDefinition emailAddressPropertyDefinition;

		private PropertyDefinition participantPropertyDefinition;

		private PropertyDefinition sipUriPropertyDefinition;
	}
}
