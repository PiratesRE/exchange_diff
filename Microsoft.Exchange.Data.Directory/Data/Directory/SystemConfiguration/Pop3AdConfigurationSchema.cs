using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class Pop3AdConfigurationSchema : PopImapAdConfigurationSchema
	{
		public static readonly ADPropertyDefinition MaxCommandSize = new ADPropertyDefinition("MaxCommandSize", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPopImapCommandSize", ADPropertyDefinitionFlags.PersistDefaultValue, 512, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(40, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition MessageRetrievalSortOrder = new ADPropertyDefinition("MessageRetrievalSortOrder", ExchangeObjectVersion.Exchange2007, typeof(SortOrder), null, ADPropertyDefinitionFlags.Calculated, SortOrder.Descending, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PopImapAdConfigurationSchema.PopImapFlags
		}, null, new GetterDelegate(Pop3AdConfiguration.MessageRetrievalSortOrderGetter), new SetterDelegate(Pop3AdConfiguration.MessageRetrievalSortOrderSetter), null, null);
	}
}
