using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADCrossRefSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition NCName = new ADPropertyDefinition("NCName", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "nCName", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DnsRoot = new ADPropertyDefinition("DnsRoot", ExchangeObjectVersion.Exchange2003, typeof(string), "dnsRoot", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NetBiosName = new ADPropertyDefinition("NetBiosName", ExchangeObjectVersion.Exchange2003, typeof(string), "netBiosName", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TrustParent = new ADPropertyDefinition("TrustParent", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "trustParent", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
