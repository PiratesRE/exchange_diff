using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMMailboxConfigurationSchema : SimpleProviderObjectSchema
	{
		private static SimpleProviderPropertyDefinition CreatePropertyDefinition(string propertyName, Type propertyType, object defaultValue)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static SimpleProviderPropertyDefinition Greeting = UMMailboxConfigurationSchema.CreatePropertyDefinition("Greeting", typeof(MailboxGreetingEnum), MailboxGreetingEnum.Voicemail);

		public static SimpleProviderPropertyDefinition FolderToReadEmailsFrom = UMMailboxConfigurationSchema.CreatePropertyDefinition("FolderToReadEmailsFrom", typeof(MailboxFolder), null);

		public static SimpleProviderPropertyDefinition ReadOldestUnreadVoiceMessagesFirst = UMMailboxConfigurationSchema.CreatePropertyDefinition("ReadOldestUnreadVoiceMessageFirst", typeof(bool), false);

		public static SimpleProviderPropertyDefinition DefaultPlayOnPhoneNumber = UMMailboxConfigurationSchema.CreatePropertyDefinition("DefaultPlayOnPhoneNumber", typeof(string), null);

		public static SimpleProviderPropertyDefinition ReceivedVoiceMailPreviewEnabled = UMMailboxConfigurationSchema.CreatePropertyDefinition("ReceivedVoiceMailPreviewEnabled", typeof(bool), true);

		public static SimpleProviderPropertyDefinition SentVoiceMailPreviewEnabled = UMMailboxConfigurationSchema.CreatePropertyDefinition("SentVoiceMailPreviewEnabled", typeof(bool), true);
	}
}
