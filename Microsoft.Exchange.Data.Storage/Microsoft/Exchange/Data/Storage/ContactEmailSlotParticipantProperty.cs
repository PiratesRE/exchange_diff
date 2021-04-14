using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactEmailSlotParticipantProperty : EmbeddedParticipantProperty
	{
		internal ContactEmailSlotParticipantProperty(EmailAddressIndex emailAddressIndex, NativeStorePropertyDefinition displayNamePropertyDefinition, NativeStorePropertyDefinition emailAddressPropertyDefinition, NativeStorePropertyDefinition routingTypePropertyDefinition, NativeStorePropertyDefinition entryIdPropertyDefinition, NativeStorePropertyDefinition emailAddressForDisplayPropertyDefinition, params PropertyDependency[] additionalDependencies) : base("Contact" + emailAddressIndex.ToString(), ParticipantEntryIdConsumer.ContactEmailSlot, displayNamePropertyDefinition, emailAddressPropertyDefinition, routingTypePropertyDefinition, entryIdPropertyDefinition, null, null, null, null, EmbeddedParticipantProperty.GetDependencies(additionalDependencies, new NativeStorePropertyDefinition[]
		{
			InternalSchema.EntryId,
			emailAddressForDisplayPropertyDefinition
		}))
		{
			this.emailAddressForDisplayPropDef = emailAddressForDisplayPropertyDefinition;
			this.emailAddressIndex = emailAddressIndex;
		}

		internal static Dictionary<EmailAddressIndex, ContactEmailSlotParticipantProperty> AllInstances
		{
			get
			{
				if (ContactEmailSlotParticipantProperty.allInstances == null)
				{
					Dictionary<EmailAddressIndex, ContactEmailSlotParticipantProperty> dictionary = new Dictionary<EmailAddressIndex, ContactEmailSlotParticipantProperty>();
					foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty in new ContactEmailSlotParticipantProperty[]
					{
						InternalSchema.ContactEmail1,
						InternalSchema.ContactEmail2,
						InternalSchema.ContactEmail3,
						InternalSchema.ContactBusinessFax,
						InternalSchema.ContactHomeFax,
						InternalSchema.ContactOtherFax
					})
					{
						dictionary.Add(contactEmailSlotParticipantProperty.emailAddressIndex, contactEmailSlotParticipantProperty);
					}
					ContactEmailSlotParticipantProperty.allInstances = dictionary;
				}
				return ContactEmailSlotParticipantProperty.allInstances;
			}
		}

		internal ReadOnlyCollection<NativeStorePropertyDefinition> EmailSlotProperties
		{
			get
			{
				if (this.emailSlotProperties == null)
				{
					this.emailSlotProperties = this.GetEmailSlotProperties();
				}
				return this.emailSlotProperties;
			}
		}

		protected virtual ReadOnlyCollection<NativeStorePropertyDefinition> GetEmailSlotProperties()
		{
			IList<NativeStorePropertyDefinition> allPropertyDefinitions = base.AllPropertyDefinitions;
			if (this.emailAddressForDisplayPropDef != null)
			{
				NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[allPropertyDefinitions.Count + 1];
				allPropertyDefinitions.CopyTo(array, 0);
				array[allPropertyDefinitions.Count] = this.emailAddressForDisplayPropDef;
				return new ReadOnlyCollection<NativeStorePropertyDefinition>(array);
			}
			return new ReadOnlyCollection<NativeStorePropertyDefinition>(base.AllPropertyDefinitions);
		}

		protected override IList<NativeStorePropertyDefinition> AllPropertyDefinitions
		{
			get
			{
				return this.EmailSlotProperties;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			Participant.Builder builder = new Participant.Builder();
			bool flag = base.Get(propertyBag, builder);
			if (this.emailAddressForDisplayPropDef != null)
			{
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(this.emailAddressForDisplayPropDef);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					builder[ParticipantSchema.EmailAddressForDisplay] = valueOrDefault;
					if (Participant.RoutingTypeEquals(builder.RoutingType, "EX") && PropertyError.IsPropertyNotFound(builder.TryGetProperty(ParticipantSchema.SmtpAddress)) && SmtpAddress.IsValidSmtpAddress(valueOrDefault))
					{
						builder[ParticipantSchema.SmtpAddress] = valueOrDefault;
					}
				}
			}
			if (!flag && PropertyError.IsPropertyNotFound(builder.TryGetProperty(ParticipantSchema.EmailAddressForDisplay)))
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.EntryId);
			if (valueOrDefault2 != null)
			{
				builder.Origin = new StoreParticipantOrigin(StoreObjectId.FromProviderSpecificId(valueOrDefault2), this.emailAddressIndex);
			}
			return builder.ToParticipant();
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			Participant participant = (Participant)value;
			ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.ContactEmailSlot);
			propertyBag.SetValueWithFixup(base.DisplayNamePropertyDefinition, participant.DisplayName ?? string.Empty);
			propertyBag.SetValueWithFixup(base.EmailAddressPropertyDefinition, participant.EmailAddress ?? string.Empty);
			propertyBag.SetValueWithFixup(base.RoutingTypePropertyDefinition, participant.RoutingType ?? string.Empty);
			if (this.emailAddressForDisplayPropDef != null)
			{
				AtomicStorePropertyDefinition propertyDefinition = this.emailAddressForDisplayPropDef;
				string propertyValue;
				if ((propertyValue = participant.GetValueOrDefault<string>(ParticipantSchema.EmailAddressForDisplay)) == null)
				{
					propertyValue = (participant.EmailAddress ?? string.Empty);
				}
				propertyBag.SetValueWithFixup(propertyDefinition, propertyValue);
			}
			if (participantEntryId != null)
			{
				propertyBag.SetValueWithFixup(base.EntryIdPropertyDefinition, participantEntryId.ToByteArray());
				return;
			}
			propertyBag.Delete(base.EntryIdPropertyDefinition);
		}

		private readonly NativeStorePropertyDefinition emailAddressForDisplayPropDef;

		private readonly EmailAddressIndex emailAddressIndex;

		private static Dictionary<EmailAddressIndex, ContactEmailSlotParticipantProperty> allInstances;

		private ReadOnlyCollection<NativeStorePropertyDefinition> emailSlotProperties;
	}
}
