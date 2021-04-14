using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DatabaseAvailabilityGroupConfigurationSchema : ADConfigurationObjectSchema
	{
		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), "name", ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(DatabaseAvailabilityGroupConfiguration.DagConfigNameGetter), new SetterDelegate(DatabaseAvailabilityGroupConfiguration.DagConfigNameSetter), null, null);

		public static readonly ADPropertyDefinition ConfigurationXML = new ADPropertyDefinition("ConfigurationXML", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchConfigurationXML", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Dags = new ADPropertyDefinition("Dags", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchMDBAvailabilityGroupConfigurationBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
