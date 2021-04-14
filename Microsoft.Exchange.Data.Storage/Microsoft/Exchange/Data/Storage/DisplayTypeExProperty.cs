using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class DisplayTypeExProperty : SmartPropertyDefinition
	{
		internal DisplayTypeExProperty() : base("DisplayTypeEx", typeof(RecipientDisplayType), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.DisplayTypeExInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.DisplayTypeExInternal);
			if (PropertyError.IsPropertyError(value))
			{
				return value;
			}
			return (RecipientDisplayType)((int)value);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			RecipientDisplayType recipientDisplayType = (RecipientDisplayType)value;
			propertyBag.SetValueWithFixup(InternalSchema.DisplayTypeExInternal, (int)recipientDisplayType);
			propertyBag.SetValueWithFixup(InternalSchema.DisplayType, (int)DisplayTypeExProperty.GetLegacyRecipientDisplayType(recipientDisplayType));
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.DisplayTypeExInternal);
			propertyBag.Delete(InternalSchema.DisplayType);
		}

		internal static bool IsDL(LegacyRecipientDisplayType legacyRecipientDisplayType)
		{
			return legacyRecipientDisplayType == LegacyRecipientDisplayType.DistributionList || legacyRecipientDisplayType == LegacyRecipientDisplayType.DynamicDistributionList || legacyRecipientDisplayType == LegacyRecipientDisplayType.PersonalDistributionList;
		}

		internal static bool IsDL(RecipientDisplayType recipientDisplayType)
		{
			RecipientDisplayType recipientDisplayTypeInNativeForest = DisplayTypeExProperty.GetRecipientDisplayTypeInNativeForest(recipientDisplayType);
			return recipientDisplayTypeInNativeForest == RecipientDisplayType.DistributionGroup || recipientDisplayTypeInNativeForest == RecipientDisplayType.DynamicDistributionGroup || recipientDisplayTypeInNativeForest == RecipientDisplayType.SecurityDistributionGroup || recipientDisplayTypeInNativeForest == RecipientDisplayType.PrivateDistributionList;
		}

		internal static bool IsMailboxUser(LegacyRecipientDisplayType legacyRecipientDisplayType)
		{
			return legacyRecipientDisplayType == LegacyRecipientDisplayType.MailUser || legacyRecipientDisplayType == LegacyRecipientDisplayType.RemoteMailUser;
		}

		internal static bool IsMailboxUser(RecipientDisplayType recipientDisplayType)
		{
			RecipientDisplayType recipientDisplayTypeInNativeForest = DisplayTypeExProperty.GetRecipientDisplayTypeInNativeForest(recipientDisplayType);
			return recipientDisplayTypeInNativeForest == RecipientDisplayType.MailboxUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.RemoteMailUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.ACLableMailboxUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.ACLableRemoteMailUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.ACLableSyncedMailboxUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.ACLableSyncedRemoteMailUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.LinkedUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.SyncedMailboxUser || recipientDisplayTypeInNativeForest == RecipientDisplayType.SyncedRemoteMailUser;
		}

		internal static bool IsRoom(RecipientDisplayType recipientDisplayType)
		{
			RecipientDisplayType recipientDisplayTypeInNativeForest = DisplayTypeExProperty.GetRecipientDisplayTypeInNativeForest(recipientDisplayType);
			return recipientDisplayTypeInNativeForest == RecipientDisplayType.ConferenceRoomMailbox;
		}

		internal static bool IsResource(RecipientDisplayType recipientDisplayType)
		{
			RecipientDisplayType recipientDisplayTypeInNativeForest = DisplayTypeExProperty.GetRecipientDisplayTypeInNativeForest(recipientDisplayType);
			return recipientDisplayTypeInNativeForest == RecipientDisplayType.EquipmentMailbox;
		}

		internal static bool IsGroupMailbox(RecipientDisplayType recipientDisplayType)
		{
			RecipientDisplayType recipientDisplayTypeInNativeForest = DisplayTypeExProperty.GetRecipientDisplayTypeInNativeForest(recipientDisplayType);
			return recipientDisplayTypeInNativeForest == RecipientDisplayType.GroupMailboxUser;
		}

		private static bool IsLocalForestRecipient(RecipientDisplayType recipientDisplayType)
		{
			return (recipientDisplayType & (RecipientDisplayType)(-2147483648)) == RecipientDisplayType.MailboxUser;
		}

		private static RecipientDisplayType GetRecipientDisplayTypeInNativeForest(RecipientDisplayType recipientDisplayType)
		{
			if (!DisplayTypeExProperty.IsLocalForestRecipient(recipientDisplayType))
			{
				return DisplayTypeExProperty.GetRecipientDisplayTypeInForeignForest(recipientDisplayType);
			}
			return DisplayTypeExProperty.GetRecipientDisplayTypeInLocalForest(recipientDisplayType);
		}

		private static RecipientDisplayType GetRecipientDisplayTypeInLocalForest(RecipientDisplayType recipientDisplayType)
		{
			return DisplayTypeExProperty.FixRecipientDisplayType(recipientDisplayType & (RecipientDisplayType)1073742079);
		}

		private static RecipientDisplayType GetRecipientDisplayTypeInForeignForest(RecipientDisplayType recipientDisplayType)
		{
			return DisplayTypeExProperty.FixRecipientDisplayType((recipientDisplayType & (RecipientDisplayType)65280) >> 8);
		}

		private static RecipientDisplayType FixRecipientDisplayType(RecipientDisplayType recipientDisplayType)
		{
			if (!EnumValidator.IsValidValue<RecipientDisplayType>(recipientDisplayType) && EnumValidator.IsValidValue<RecipientDisplayType>(recipientDisplayType | RecipientDisplayType.ACLableMailboxUser))
			{
				recipientDisplayType |= RecipientDisplayType.ACLableMailboxUser;
			}
			return recipientDisplayType;
		}

		private static LegacyRecipientDisplayType GetLegacyRecipientDisplayType(RecipientDisplayType recipientDisplayType)
		{
			LegacyRecipientDisplayType legacyRecipientDisplayType = (LegacyRecipientDisplayType)(DisplayTypeExProperty.GetRecipientDisplayTypeInLocalForest(recipientDisplayType) & (RecipientDisplayType)255);
			if (EnumValidator.IsValidValue<LegacyRecipientDisplayType>(legacyRecipientDisplayType))
			{
				return legacyRecipientDisplayType;
			}
			if (legacyRecipientDisplayType != (LegacyRecipientDisplayType)9)
			{
				return LegacyRecipientDisplayType.MailUser;
			}
			return LegacyRecipientDisplayType.DistributionList;
		}

		private const RecipientDisplayType RdtForeignMask = (RecipientDisplayType)(-2147483648);

		private const RecipientDisplayType RdtAclMask = RecipientDisplayType.ACLableMailboxUser;

		private const RecipientDisplayType RdtRemoteMask = (RecipientDisplayType)65280;

		private const RecipientDisplayType RdtLocalMask = (RecipientDisplayType)255;
	}
}
