using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class GALSyncOrganizationSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition GALSyncClientData = new ADPropertyDefinition("GALSyncClientData", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDirsyncID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
	}
}
