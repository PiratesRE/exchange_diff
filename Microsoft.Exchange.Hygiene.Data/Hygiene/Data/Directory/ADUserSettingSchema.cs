using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADUserSettingSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition SettingIdProp = new HygienePropertyDefinition("settingsId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition ConfigurationIdProp = new HygienePropertyDefinition("configId", typeof(ADObjectId));

		public static readonly ADPropertyDefinition SafeSendersProp = new HygienePropertyDefinition(ADRecipientSchema.SafeSendersHash.Name, ADRecipientSchema.SafeSendersHash.Type);

		public static readonly ADPropertyDefinition BlockedSendersProp = new HygienePropertyDefinition(ADRecipientSchema.BlockedSendersHash.Name, ADRecipientSchema.BlockedSendersHash.Type);

		public static readonly HygienePropertyDefinition DirectionIdProp = new HygienePropertyDefinition("directionId", typeof(MailDirection), MailDirection.Inbound, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition FlagsProp = new HygienePropertyDefinition("flags", typeof(UserSettingFlags), UserSettingFlags.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly ADPropertyDefinition DisplayNameProp = new HygienePropertyDefinition(ADRecipientSchema.DisplayName.Name, ADRecipientSchema.DisplayName.Type);
	}
}
