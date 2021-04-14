using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniClientAccessServerOrArraySchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Fqdn = ServerSchema.Fqdn;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = ServerSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition Site = ServerSchema.ServerSite;

		public static readonly ADPropertyDefinition IsClientAccessArray = new ADPropertyDefinition("IsClientAccessArray", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ObjectClass
		}, null, new GetterDelegate(ClientAccessArray.IsClientAccessArrayGetter), null, null, null);

		public static readonly ADPropertyDefinition IsClientAccessServer = ServerSchema.IsClientAccessServer;
	}
}
