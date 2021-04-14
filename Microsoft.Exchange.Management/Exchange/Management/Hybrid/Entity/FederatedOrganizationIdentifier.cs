using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class FederatedOrganizationIdentifier : IFederatedOrganizationIdentifier
	{
		public bool Enabled { get; set; }

		public SmtpDomain AccountNamespace { get; set; }

		public ADObjectId DelegationTrustLink { get; set; }

		public MultiValuedProperty<FederatedDomain> Domains { get; set; }

		public SmtpDomain DefaultDomain { get; set; }
	}
}
