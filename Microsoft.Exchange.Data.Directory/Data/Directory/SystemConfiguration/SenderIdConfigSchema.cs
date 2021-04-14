using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SenderIdConfigSchema : MessageHygieneAgentConfigSchema
	{
		public static readonly PropertyDefinition SpoofedDomainAction = new ADPropertyDefinition("SpoofedDomainAction", ExchangeObjectVersion.Exchange2007, typeof(SenderIdAction), "msExchMessageHygieneSpoofedDomainAction", ADPropertyDefinitionFlags.None, SenderIdAction.StampStatus, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(SenderIdAction))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly PropertyDefinition TempErrorAction = new ADPropertyDefinition("TempErrorAction", ExchangeObjectVersion.Exchange2007, typeof(SenderIdAction), "msExchMessageHygieneTempErrorAction", ADPropertyDefinitionFlags.None, SenderIdAction.StampStatus, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(SenderIdAction))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly PropertyDefinition BypassedRecipients = SharedPropertyDefinitions.BypassedRecipients;

		public static readonly PropertyDefinition BypassedSenderDomains = new ADPropertyDefinition("BypassedSenderDomains", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchMessageHygieneBypassedSenderDomain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
