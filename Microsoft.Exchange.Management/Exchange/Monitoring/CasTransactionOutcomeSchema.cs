using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	internal class CasTransactionOutcomeSchema : TransactionOutcomeBaseSchema
	{
		public static readonly SimpleProviderPropertyDefinition LocalSite = new SimpleProviderPropertyDefinition("LocalSite", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SecureAccess = new SimpleProviderPropertyDefinition("SecureAccess", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Url = new SimpleProviderPropertyDefinition("Url", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition VirtualDirectoryName = new SimpleProviderPropertyDefinition("VirtualDirectoryName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UrlType = new SimpleProviderPropertyDefinition("UrlType", ExchangeObjectVersion.Exchange2010, typeof(VirtualDirectoryUriScope), PropertyDefinitionFlags.None, VirtualDirectoryUriScope.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ConnectionType = new SimpleProviderPropertyDefinition("Latency", ExchangeObjectVersion.Exchange2010, typeof(ProtocolConnectionType), PropertyDefinitionFlags.None, ProtocolConnectionType.Plaintext, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Port = new SimpleProviderPropertyDefinition("Port", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
