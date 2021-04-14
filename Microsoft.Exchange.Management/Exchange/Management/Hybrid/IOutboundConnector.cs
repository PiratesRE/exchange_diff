using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IOutboundConnector : IEntity<IOutboundConnector>
	{
		string Name { get; }

		string TlsDomain { get; }

		TenantConnectorType ConnectorType { get; }

		TenantConnectorSource ConnectorSource { get; set; }

		MultiValuedProperty<SmtpDomainWithSubdomains> RecipientDomains { get; }

		MultiValuedProperty<SmartHost> SmartHosts { get; }

		TlsAuthLevel? TlsSettings { get; }

		bool CloudServicesMailEnabled { get; set; }

		bool RouteAllMessagesViaOnPremises { get; set; }
	}
}
