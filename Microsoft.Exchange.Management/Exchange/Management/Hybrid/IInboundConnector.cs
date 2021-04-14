using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IInboundConnector : IEntity<IInboundConnector>
	{
		string Name { get; }

		string TLSSenderCertificateName { get; }

		TenantConnectorType ConnectorType { get; }

		TenantConnectorSource ConnectorSource { get; set; }

		MultiValuedProperty<AddressSpace> SenderDomains { get; }

		bool RequireTls { get; }

		bool CloudServicesMailEnabled { get; set; }
	}
}
