using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface ISendConnector : IEntity<ISendConnector>
	{
		string Name { get; }

		MultiValuedProperty<AddressSpace> AddressSpaces { get; }

		MultiValuedProperty<ADObjectId> SourceTransportServers { get; }

		bool DNSRoutingEnabled { get; }

		MultiValuedProperty<SmartHost> SmartHosts { get; }

		bool RequireTLS { get; }

		TlsAuthLevel? TlsAuthLevel { get; }

		string TlsDomain { get; }

		ErrorPolicies ErrorPolicies { get; }

		SmtpX509Identifier TlsCertificateName { get; }

		bool CloudServicesMailEnabled { get; set; }

		string Fqdn { get; }
	}
}
