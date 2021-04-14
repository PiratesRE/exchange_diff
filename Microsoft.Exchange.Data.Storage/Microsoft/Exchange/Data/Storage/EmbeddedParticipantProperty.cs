using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EmbeddedParticipantProperty : SmartPropertyDefinition
	{
		protected EmbeddedParticipantProperty(string displayName, ParticipantEntryIdConsumer entryIdConsumer, NativeStorePropertyDefinition displayNamePropertyDefinition, NativeStorePropertyDefinition emailAddressPropertyDefinition, NativeStorePropertyDefinition routingTypePropertyDefinition, NativeStorePropertyDefinition entryIdPropertyDefinition, NativeStorePropertyDefinition smtpAddressPropertyDefinition, NativeStorePropertyDefinition sipUriPropertyDefinition, NativeStorePropertyDefinition sidPropertyDefinition, NativeStorePropertyDefinition guidPropertyDefinition, params PropertyDependency[] additionalDependencies) : base(displayName, typeof(Participant), PropertyFlags.None, Array<PropertyDefinitionConstraint>.Empty, EmbeddedParticipantProperty.GetDependencies(additionalDependencies, new NativeStorePropertyDefinition[]
		{
			displayNamePropertyDefinition,
			emailAddressPropertyDefinition,
			routingTypePropertyDefinition,
			entryIdPropertyDefinition,
			smtpAddressPropertyDefinition,
			sipUriPropertyDefinition,
			sidPropertyDefinition,
			guidPropertyDefinition
		}))
		{
			this.entryIdConsumer = entryIdConsumer;
			this.displayNamePropDef = displayNamePropertyDefinition;
			this.emailAddressPropDef = emailAddressPropertyDefinition;
			this.routingTypePropDef = routingTypePropertyDefinition;
			this.entryIdPropDef = entryIdPropertyDefinition;
			this.smtpAddressPropDef = smtpAddressPropertyDefinition;
			this.sipUriPropDef = sipUriPropertyDefinition;
			this.sidPropDef = sidPropertyDefinition;
			this.guidPropDef = guidPropertyDefinition;
		}

		internal EmbeddedParticipantProperty(string displayName, ParticipantEntryIdConsumer entryIdConsumer, NativeStorePropertyDefinition displayNamePropertyDefinition, NativeStorePropertyDefinition emailAddressPropertyDefinition, NativeStorePropertyDefinition routingTypePropertyDefinition, NativeStorePropertyDefinition entryIdPropertyDefinition, NativeStorePropertyDefinition smtpAddressPropertyDefinition, NativeStorePropertyDefinition sipUriPropertyDefinition, NativeStorePropertyDefinition sidPropertyDefinition, NativeStorePropertyDefinition guidPropertyDefinition) : this(displayName, entryIdConsumer, displayNamePropertyDefinition, emailAddressPropertyDefinition, routingTypePropertyDefinition, entryIdPropertyDefinition, smtpAddressPropertyDefinition, sipUriPropertyDefinition, sidPropertyDefinition, guidPropertyDefinition, Array<PropertyDependency>.Empty)
		{
		}

		protected static PropertyDependency[] GetDependencies(PropertyDependency[] additionalDependencies, params NativeStorePropertyDefinition[] propDefsForReadAndWrite)
		{
			List<PropertyDependency> list = new List<PropertyDependency>(additionalDependencies.Length + 5);
			list.AddRange(additionalDependencies);
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in propDefsForReadAndWrite)
			{
				if (nativeStorePropertyDefinition != null)
				{
					list.Add(new PropertyDependency(nativeStorePropertyDefinition, PropertyDependencyType.NeedForRead));
				}
			}
			return list.ToArray();
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in this.AllPropertyDefinitions)
			{
				if (nativeStorePropertyDefinition != null)
				{
					propertyBag.Delete(nativeStorePropertyDefinition);
				}
			}
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			Participant participant = (Participant)value;
			ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, this.entryIdConsumer);
			propertyBag.Update(this.displayNamePropDef, participant.DisplayName);
			propertyBag.Update(this.emailAddressPropDef, participant.EmailAddress);
			propertyBag.Update(this.routingTypePropDef, participant.RoutingType);
			propertyBag.Update(this.entryIdPropDef, (participantEntryId != null) ? participantEntryId.ToByteArray() : null);
			if (this.smtpAddressPropDef != null)
			{
				propertyBag.Update(this.smtpAddressPropDef, participant.TryGetProperty(ParticipantSchema.SmtpAddress));
			}
			if (this.sipUriPropDef != null)
			{
				propertyBag.Update(this.sipUriPropDef, participant.TryGetProperty(ParticipantSchema.SipUri));
			}
			if (this.sidPropDef != null)
			{
				propertyBag.Update(this.sidPropDef, participant.TryGetProperty(ParticipantSchema.ParticipantSID));
			}
			if (this.guidPropDef != null)
			{
				propertyBag.Update(this.guidPropDef, participant.TryGetProperty(ParticipantSchema.ParticipantGuid));
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			Participant.Builder builder = new Participant.Builder();
			if (!this.Get(propertyBag, builder))
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return builder.ToParticipant();
		}

		internal NativeStorePropertyDefinition DisplayNamePropertyDefinition
		{
			get
			{
				return this.displayNamePropDef;
			}
		}

		internal NativeStorePropertyDefinition EmailAddressPropertyDefinition
		{
			get
			{
				return this.emailAddressPropDef;
			}
		}

		internal NativeStorePropertyDefinition RoutingTypePropertyDefinition
		{
			get
			{
				return this.routingTypePropDef;
			}
		}

		internal NativeStorePropertyDefinition EntryIdPropertyDefinition
		{
			get
			{
				return this.entryIdPropDef;
			}
		}

		internal NativeStorePropertyDefinition SmtpAddressPropertyDefinition
		{
			get
			{
				return this.smtpAddressPropDef;
			}
		}

		internal NativeStorePropertyDefinition SipUriPropertyDefinition
		{
			get
			{
				return this.sipUriPropDef;
			}
		}

		internal NativeStorePropertyDefinition SidPropertyDefinition
		{
			get
			{
				return this.sidPropDef;
			}
		}

		internal NativeStorePropertyDefinition GuidPropertyDefinition
		{
			get
			{
				return this.guidPropDef;
			}
		}

		protected virtual IList<NativeStorePropertyDefinition> AllPropertyDefinitions
		{
			get
			{
				List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>();
				list.Add(this.displayNamePropDef);
				list.Add(this.emailAddressPropDef);
				list.Add(this.routingTypePropDef);
				list.Add(this.entryIdPropDef);
				if (this.smtpAddressPropDef != null)
				{
					list.Add(this.smtpAddressPropDef);
				}
				if (this.sipUriPropDef != null)
				{
					list.Add(this.sipUriPropDef);
				}
				if (this.sidPropDef != null)
				{
					list.Add(this.sidPropDef);
				}
				if (this.guidPropDef != null)
				{
					list.Add(this.guidPropDef);
				}
				return list;
			}
		}

		protected bool Get(PropertyBag.BasicPropertyStore propertyBag, Participant.Builder participantBuilder)
		{
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(this.entryIdPropDef);
			if (valueOrDefault != null)
			{
				participantBuilder.SetPropertiesFrom(ParticipantEntryId.TryFromEntryId(valueOrDefault));
			}
			participantBuilder.DisplayName = propertyBag.GetValueOrDefault<string>(this.displayNamePropDef, participantBuilder.DisplayName);
			participantBuilder.EmailAddress = propertyBag.GetValueOrDefault<string>(this.emailAddressPropDef, participantBuilder.EmailAddress);
			participantBuilder.RoutingType = propertyBag.GetValueOrDefault<string>(this.routingTypePropDef, participantBuilder.RoutingType);
			if (this.smtpAddressPropDef != null)
			{
				string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(this.smtpAddressPropDef);
				if (!string.IsNullOrEmpty(valueOrDefault2))
				{
					participantBuilder[ParticipantSchema.SmtpAddress] = valueOrDefault2;
				}
			}
			if (this.sipUriPropDef != null)
			{
				string valueOrDefault3 = propertyBag.GetValueOrDefault<string>(this.sipUriPropDef);
				if (!string.IsNullOrEmpty(valueOrDefault3))
				{
					participantBuilder[ParticipantSchema.SipUri] = valueOrDefault3;
				}
			}
			if (this.sidPropDef != null)
			{
				byte[] valueOrDefault4 = propertyBag.GetValueOrDefault<byte[]>(this.sidPropDef);
				if (valueOrDefault4 != null)
				{
					participantBuilder[ParticipantSchema.ParticipantSID] = valueOrDefault4;
				}
			}
			if (this.guidPropDef != null)
			{
				byte[] valueOrDefault5 = propertyBag.GetValueOrDefault<byte[]>(this.guidPropDef);
				if (valueOrDefault5 != null)
				{
					participantBuilder[ParticipantSchema.ParticipantGuid] = valueOrDefault5;
				}
			}
			return !string.IsNullOrEmpty(participantBuilder.DisplayName) || !string.IsNullOrEmpty(participantBuilder.EmailAddress);
		}

		private readonly ParticipantEntryIdConsumer entryIdConsumer;

		private readonly NativeStorePropertyDefinition displayNamePropDef;

		private readonly NativeStorePropertyDefinition emailAddressPropDef;

		private readonly NativeStorePropertyDefinition entryIdPropDef;

		private readonly NativeStorePropertyDefinition routingTypePropDef;

		private readonly NativeStorePropertyDefinition smtpAddressPropDef;

		private readonly NativeStorePropertyDefinition sipUriPropDef;

		private readonly NativeStorePropertyDefinition sidPropDef;

		private readonly NativeStorePropertyDefinition guidPropDef;
	}
}
