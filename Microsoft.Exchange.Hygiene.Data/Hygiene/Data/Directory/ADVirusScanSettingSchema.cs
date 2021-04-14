using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADVirusScanSettingSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition ConfigurationIdProp = new HygienePropertyDefinition("configId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition FlagsProp = new HygienePropertyDefinition("flags", typeof(VirusScanFlags));

		public static readonly HygienePropertyDefinition SenderWarningNotificationIdProp = new HygienePropertyDefinition("senderWarningNotificationId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SenderRejectionNotificationIdProp = new HygienePropertyDefinition("senderRejectionNotificationId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition RecipientNotificationIdProp = new HygienePropertyDefinition("recipientNotificationId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition AdminNotificationAddressProp = new HygienePropertyDefinition("adminNotificationAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition OutboundAdminNotificationAddressProp = new HygienePropertyDefinition("outboundAdminNotificationAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
