using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class SingleRecipientPropertyBase : ComplexPropertyBase, IPregatherParticipants, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, IPropertyCommand
	{
		public SingleRecipientPropertyBase(CommandContext commandContext) : base(commandContext)
		{
		}

		protected abstract Participant GetParticipant(Item storeItem);

		protected abstract void SetParticipant(Item storeItem, Participant participant);

		protected abstract PropertyDefinition GetParticipantDisplayNamePropertyDefinition();

		protected abstract PropertyDefinition GetParticipantEmailAddressPropertyDefinition();

		protected abstract PropertyDefinition GetParticipantRoutingTypePropertyDefinition();

		protected abstract PropertyDefinition GetParticipantSipUriPropertyDefinition();

		void IPregatherParticipants.Pregather(StoreObject storeObject, List<Participant> participants)
		{
			Participant participant = this.GetParticipant(storeObject as Item);
			if (participant != null)
			{
				participants.Add(participant);
			}
		}

		public void ToXml()
		{
			throw new InvalidOperationException("SingleRecipientPropertyBase.ToXml should never be called");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("SingleRecipientPropertyBase.ToXmlForPropertyBag should never be called");
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			SingleRecipientType valueOrDefault = serviceObject.GetValueOrDefault<SingleRecipientType>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null && valueOrDefault.Mailbox != null)
			{
				Item item = (Item)storeObject;
				this.SetParticipant(item, this.GetParticipantFromAddress(item, valueOrDefault.Mailbox));
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			Item item = storeObject as Item;
			if (item != null)
			{
				Participant participant = this.GetParticipant(item);
				ParticipantInformation participantInformation;
				if (participant != null && EWSSettings.ParticipantInformation.TryGetParticipant(participant, out participantInformation))
				{
					serviceObject[propertyInformation] = PropertyCommand.CreateRecipientFromParticipant(participantInformation);
				}
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			string empty = string.Empty;
			if (PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.GetParticipantDisplayNamePropertyDefinition(), out empty))
			{
				string emailAddress;
				if (!PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.GetParticipantEmailAddressPropertyDefinition(), out emailAddress))
				{
					emailAddress = null;
				}
				string routingType;
				if (!PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.GetParticipantRoutingTypePropertyDefinition(), out routingType))
				{
					routingType = null;
				}
				string sipUri;
				if (!PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.GetParticipantSipUriPropertyDefinition(), out sipUri))
				{
					sipUri = null;
				}
				ParticipantInformation participantInformation = new ParticipantInformation(empty, routingType, emailAddress, new OneOffParticipantOrigin(), null, sipUri, null);
				SingleRecipientType value = PropertyCommand.CreateRecipientFromParticipant(participantInformation);
				serviceObject[propertyInformation] = value;
			}
		}
	}
}
