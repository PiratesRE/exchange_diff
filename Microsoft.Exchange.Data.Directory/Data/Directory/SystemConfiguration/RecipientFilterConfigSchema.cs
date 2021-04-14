using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RecipientFilterConfigSchema : MessageHygieneAgentConfigSchema
	{
		public static readonly PropertyDefinition BlockedRecipients = new ADPropertyDefinition("BlockedRecipients", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress), "msExchMessageHygieneBlockedRecipient", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition BlockListEnabled = new ADPropertyDefinition("BlockListEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(1024, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(1024, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition RecipientValidationEnabled = new ADPropertyDefinition("RecipientValidationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(2048, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(2048, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);
	}
}
