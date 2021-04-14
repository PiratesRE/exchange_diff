using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class PolicyTipMessageConfigSchema : ADConfigurationObjectSchema
	{
		internal static readonly ADPropertyDefinition Locale = new ADPropertyDefinition("Locale", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchPolicyTipMessageConfigLocale", ADPropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition Action = new ADPropertyDefinition("Action", ExchangeObjectVersion.Exchange2010, typeof(PolicyTipMessageConfigAction), "msExchPolicyTipMessageConfigAction", ADPropertyDefinitionFlags.WriteOnce, PolicyTipMessageConfigAction.NotifyOnly, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Value = new ADPropertyDefinition("Value", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchPolicyTipMessageConfigMessage", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, null, null);
	}
}
