using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SenderParticipantProperty : EmbeddedParticipantProperty
	{
		internal SenderParticipantProperty(string displayName, NativeStorePropertyDefinition displayNamePropertyDefinition, NativeStorePropertyDefinition emailAddressPropertyDefinition, NativeStorePropertyDefinition routingTypePropertyDefinition, NativeStorePropertyDefinition entryIdPropertyDefinition, NativeStorePropertyDefinition smtpAddressPropertyDefinition, NativeStorePropertyDefinition sipUriPropertyDefinition, NativeStorePropertyDefinition sidPropertyDefinition, NativeStorePropertyDefinition guidPropertyDefinition) : base(displayName, ParticipantEntryIdConsumer.SupportsADParticipantEntryId, displayNamePropertyDefinition, emailAddressPropertyDefinition, routingTypePropertyDefinition, entryIdPropertyDefinition, smtpAddressPropertyDefinition, sipUriPropertyDefinition, sidPropertyDefinition, guidPropertyDefinition, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.SendRichInfo, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			base.InternalSetValue(propertyBag, value);
			Participant participant = (Participant)value;
			bool? valueAsNullable = participant.GetValueAsNullable<bool>(ParticipantSchema.SendRichInfo);
			if (valueAsNullable != null)
			{
				propertyBag.Update(InternalSchema.SendRichInfo, valueAsNullable.Value);
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			Participant.Builder builder = new Participant.Builder();
			if (base.Get(propertyBag, builder))
			{
				bool? valueAsNullable = propertyBag.GetValueAsNullable<bool>(InternalSchema.SendRichInfo);
				if (valueAsNullable != null)
				{
					builder[ParticipantSchema.SendRichInfo] = valueAsNullable.Value;
				}
				return builder.ToParticipant();
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}
	}
}
