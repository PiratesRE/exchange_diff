using System;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADSubnetSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition Site = new ADPropertyDefinition("Site", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "siteObject", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IPAddress = new ADPropertyDefinition("IPAddress", ExchangeObjectVersion.Exchange2003, typeof(IPAddress), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, null, new GetterDelegate(ADSubnet.IPAddressGetter), new SetterDelegate(ADSubnet.IPAddressSetter), null, null);

		public static readonly ADPropertyDefinition MaskBits = new ADPropertyDefinition("MaskBits", ExchangeObjectVersion.Exchange2003, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 2, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, null, new GetterDelegate(ADSubnet.MaskBitsGetter), new SetterDelegate(ADSubnet.MaskBitsSetter), null, null);
	}
}
