using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class Imap4AdConfigurationSchema : PopImapAdConfigurationSchema
	{
		public static readonly ADPropertyDefinition MaxCommandSize = new ADPropertyDefinition("MaxCommandSize", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPopImapCommandSize", ADPropertyDefinitionFlags.PersistDefaultValue, 10240, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1024, 16384)
		}, null, null);

		public static readonly ADPropertyDefinition ShowHiddenFoldersEnabled = new ADPropertyDefinition("ShowHiddenFoldersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PopImapAdConfigurationSchema.PopImapFlags
		}, null, new GetterDelegate(Imap4AdConfiguration.ShowHiddenFoldersEnabledGetter), new SetterDelegate(Imap4AdConfiguration.ShowHiddenFoldersEnabledSetter), null, null);
	}
}
