using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IFederatedOrganizationIdentifier
	{
		bool Enabled { get; }

		SmtpDomain AccountNamespace { get; }

		ADObjectId DelegationTrustLink { get; }

		MultiValuedProperty<FederatedDomain> Domains { get; }

		SmtpDomain DefaultDomain { get; }
	}
}
