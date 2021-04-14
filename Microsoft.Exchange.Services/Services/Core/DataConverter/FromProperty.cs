using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class FromProperty : SingleRecipientPropertyBase, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public FromProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			MessageItem messageItem = storeItem as MessageItem;
			if (messageItem == null)
			{
				return null;
			}
			return messageItem.From;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
			MessageItem messageItem = storeItem as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			messageItem.From = participant;
		}

		protected override PropertyDefinition GetParticipantDisplayNamePropertyDefinition()
		{
			return ItemSchema.SentRepresentingDisplayName;
		}

		protected override PropertyDefinition GetParticipantEmailAddressPropertyDefinition()
		{
			return ItemSchema.SentRepresentingEmailAddress;
		}

		protected override PropertyDefinition GetParticipantRoutingTypePropertyDefinition()
		{
			return ItemSchema.SentRepresentingType;
		}

		protected override PropertyDefinition GetParticipantSipUriPropertyDefinition()
		{
			return ParticipantSchema.SipUri;
		}

		protected override Participant GetParticipantFromAddress(Item item, EmailAddressWrapper address)
		{
			object obj = null;
			try
			{
				obj = item.TryGetProperty(MessageItemSchema.SharingInstanceGuid);
			}
			catch (NotInBagPropertyErrorException)
			{
			}
			if (obj == null || obj is PropertyError)
			{
				return base.GetParticipantFromAddress(item, address);
			}
			return new Participant(address.Name, address.EmailAddress, address.RoutingType);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			MessageItem messageItem = updateCommandSettings.StoreObject as MessageItem;
			if (messageItem != null && messageItem.IsDraft)
			{
				ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
				SingleRecipientType valueOrDefault = serviceObject.GetValueOrDefault<SingleRecipientType>(this.commandContext.PropertyInformation);
				this.SetParticipant(messageItem, this.GetParticipantFromAddress(messageItem, valueOrDefault.Mailbox));
				return;
			}
			throw new InvalidPropertySetException(CoreResources.IDs.MessageMessageIsNotDraft, updateCommandSettings.PropertyUpdate.PropertyPath);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			MessageItem messageItem = updateCommandSettings.StoreObject as MessageItem;
			if (messageItem != null && messageItem.IsDraft)
			{
				messageItem.From = null;
				return;
			}
			throw new InvalidPropertyDeleteException(CoreResources.IDs.MessageMessageIsNotDraft, updateCommandSettings.PropertyUpdate.PropertyPath);
		}

		public static FromProperty CreateCommand(CommandContext commandContext)
		{
			return new FromProperty(commandContext);
		}
	}
}
