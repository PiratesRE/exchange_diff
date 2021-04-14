using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RecipientBaseParticipantProperty : EmbeddedParticipantProperty
	{
		internal RecipientBaseParticipantProperty() : base("RecipientBaseParticipant", ParticipantEntryIdConsumer.RecipientTableSecondary, InternalSchema.DisplayName, InternalSchema.EmailAddress, InternalSchema.AddrType, InternalSchema.RecipientEntryId, InternalSchema.SmtpAddress, InternalSchema.SipUri, InternalSchema.ParticipantSID, InternalSchema.ParticipantGuid, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.DisplayTypeExInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.DisplayType, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ObjectType, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.SendRichInfo, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.SendInternetEncoding, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TransmitableDisplayName, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			throw new NotSupportedException();
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			base.InternalSetValue(propertyBag, value);
			Participant participant = (Participant)value;
			ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.RecipientTablePrimary);
			if (participantEntryId != null)
			{
				propertyBag.SetValueWithFixup(InternalSchema.EntryId, participantEntryId.ToByteArray());
			}
			else
			{
				propertyBag.Delete(InternalSchema.EntryId);
			}
			RecipientDisplayType? valueAsNullable = participant.GetValueAsNullable<RecipientDisplayType>(ParticipantSchema.DisplayTypeEx);
			LegacyRecipientDisplayType? valueAsNullable2 = participant.GetValueAsNullable<LegacyRecipientDisplayType>(ParticipantSchema.DisplayType);
			if (participant.RoutingType != null)
			{
				propertyBag.Update(InternalSchema.TransmitableDisplayName, participant.DisplayName);
				if (valueAsNullable2 == null)
				{
					valueAsNullable2 = new LegacyRecipientDisplayType?(LegacyRecipientDisplayType.MailUser);
				}
			}
			else
			{
				propertyBag.Delete(InternalSchema.TransmitableDisplayName);
			}
			AtomicStorePropertyDefinition displayTypeExInternal = InternalSchema.DisplayTypeExInternal;
			RecipientDisplayType? recipientDisplayType = valueAsNullable;
			propertyBag.Update(displayTypeExInternal, (recipientDisplayType != null) ? new int?((int)recipientDisplayType.GetValueOrDefault()) : null);
			AtomicStorePropertyDefinition displayType = InternalSchema.DisplayType;
			LegacyRecipientDisplayType? legacyRecipientDisplayType = valueAsNullable2;
			propertyBag.Update(displayType, (legacyRecipientDisplayType != null) ? new int?((int)legacyRecipientDisplayType.GetValueOrDefault()) : null);
			if (valueAsNullable2 != null)
			{
				if (valueAsNullable2 == LegacyRecipientDisplayType.DistributionList || valueAsNullable2 == LegacyRecipientDisplayType.PersonalDistributionList)
				{
					propertyBag.SetValueWithFixup(InternalSchema.ObjectType, 8);
				}
				else
				{
					propertyBag.SetValueWithFixup(InternalSchema.ObjectType, 6);
				}
			}
			else
			{
				propertyBag.Delete(InternalSchema.ObjectType);
			}
			propertyBag.SetValueWithFixup(InternalSchema.SendRichInfo, participant.GetValueOrDefault<bool>(ParticipantSchema.SendRichInfo));
			propertyBag.Update(InternalSchema.SendInternetEncoding, participant.GetValueAsNullable<int>(ParticipantSchema.SendInternetEncoding));
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			Participant.Builder builder = new Participant.Builder();
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.EntryId);
			if (valueOrDefault != null)
			{
				builder.SetPropertiesFrom(ParticipantEntryId.TryFromEntryId(valueOrDefault));
			}
			builder.DisplayName = (propertyBag.GetValueOrDefault<string>(InternalSchema.TransmitableDisplayName) ?? builder.DisplayName);
			if (!base.Get(propertyBag, builder))
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			RecipientDisplayType? valueAsNullable = propertyBag.GetValueAsNullable<RecipientDisplayType>(InternalSchema.DisplayTypeExInternal);
			if (valueAsNullable != null)
			{
				builder[ParticipantSchema.DisplayTypeEx] = valueAsNullable.Value;
				builder.Origin = (builder.Origin ?? new DirectoryParticipantOrigin());
			}
			else if (PropertyError.IsPropertyNotFound(builder.TryGetProperty(ParticipantSchema.DisplayType)))
			{
				LegacyRecipientDisplayType? valueAsNullable2 = propertyBag.GetValueAsNullable<LegacyRecipientDisplayType>(InternalSchema.DisplayType);
				ObjectType? valueAsNullable3 = propertyBag.GetValueAsNullable<ObjectType>(InternalSchema.ObjectType);
				if (valueAsNullable2 != null)
				{
					if (valueAsNullable2 != LegacyRecipientDisplayType.MailUser)
					{
						builder[ParticipantSchema.DisplayType] = (int)valueAsNullable2.Value;
					}
				}
				else if (valueAsNullable3 != null && valueAsNullable3 == ObjectType.MAPI_DISTLIST)
				{
					builder[ParticipantSchema.DisplayType] = 1;
				}
			}
			builder.SetOrDeleteProperty(ParticipantSchema.SendRichInfo, Util.NullIf<object>(propertyBag.GetValueAsNullable<bool>(InternalSchema.SendRichInfo), false));
			builder.SetOrDeleteProperty(ParticipantSchema.SendInternetEncoding, propertyBag.GetValueAsNullable<int>(InternalSchema.SendInternetEncoding));
			return builder.ToParticipant();
		}
	}
}
