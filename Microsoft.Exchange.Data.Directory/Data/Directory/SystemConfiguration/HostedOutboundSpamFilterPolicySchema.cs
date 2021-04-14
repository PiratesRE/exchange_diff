using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class HostedOutboundSpamFilterPolicySchema : ADConfigurationObjectSchema
	{
		internal const int NotifyOutboundSpamShift = 0;

		internal const int BccSuspiciousOutboundMailShift = 2;

		public static readonly ADPropertyDefinition OutboundSpamFilterFlags = new ADPropertyDefinition("OutboundSpamFilterFlags", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchSpamFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NotifyOutboundSpamRecipients = new ADPropertyDefinition("NotifyOutboundSpamRecipients", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchSpamNotifyOutboundRecipients", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BccSuspiciousOutboundAdditionalRecipients = new ADPropertyDefinition("BccSuspiciousOutboundAdditionalRecipients", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchSpamOutboundSpamCc", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NotifyOutboundSpam = ADObject.BitfieldProperty("NotifyOutboundSpam", 0, HostedOutboundSpamFilterPolicySchema.OutboundSpamFilterFlags);

		public static readonly ADPropertyDefinition BccSuspiciousOutboundMail = ADObject.BitfieldProperty("BccSuspiciousOutboundMail", 2, HostedOutboundSpamFilterPolicySchema.OutboundSpamFilterFlags);
	}
}
