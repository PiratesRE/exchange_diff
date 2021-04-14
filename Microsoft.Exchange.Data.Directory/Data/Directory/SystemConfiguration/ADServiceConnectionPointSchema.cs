using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADServiceConnectionPointSchema : ADNonExchangeObjectSchema
	{
		public new static readonly ADPropertyDefinition ExchangeVersion = new ADPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), null, ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.DoNotProvisionalClone, ExchangeObjectVersion.Exchange2003, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Keywords = new ADPropertyDefinition("Keywords", ExchangeObjectVersion.Exchange2003, typeof(string), "Keywords", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceBindingInformation = new ADPropertyDefinition("ServiceBindingInformation", ExchangeObjectVersion.Exchange2003, typeof(string), "ServiceBindingInformation", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceDnsName = new ADPropertyDefinition("ServiceDnsName", ExchangeObjectVersion.Exchange2003, typeof(string), "ServiceDnsName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceClassName = new ADPropertyDefinition("ServiceClassName", ExchangeObjectVersion.Exchange2003, typeof(string), "ServiceClassName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
