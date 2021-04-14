using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	internal class HybridMailflowConfigurationSchema : SimpleProviderObjectSchema
	{
		internal static SimpleProviderPropertyDefinition OutboundDomains = new SimpleProviderPropertyDefinition("OutboundDomains", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomainWithSubdomains), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition InboundIPs = new SimpleProviderPropertyDefinition("InboundIPs", ExchangeObjectVersion.Exchange2010, typeof(IPRange), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition OnPremisesFQDN = new SimpleProviderPropertyDefinition("OnPremisesFQDN", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition CertificateSubject = new SimpleProviderPropertyDefinition("CertificateSubject", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition SecureMailEnabled = new SimpleProviderPropertyDefinition("SecureMailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition CentralizedTransportEnabled = new SimpleProviderPropertyDefinition("CentralizedTransportEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
