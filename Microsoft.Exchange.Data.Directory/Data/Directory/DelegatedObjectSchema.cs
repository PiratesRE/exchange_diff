using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class DelegatedObjectSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Identity = new ADPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(DelegatedObjectId), "delegatedIdentity", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
	}
}
