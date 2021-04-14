using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class MailboxPolicySchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition MailboxPolicyFlags = new ADPropertyDefinition("MailboxPolicyFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchRecipientTemplateFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 2)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
