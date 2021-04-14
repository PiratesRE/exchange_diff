using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IOnPremisesOrganization : IEntity<IOnPremisesOrganization>
	{
		Guid OrganizationGuid { get; }

		string OrganizationName { get; }

		MultiValuedProperty<SmtpDomain> HybridDomains { get; }

		ADObjectId InboundConnector { get; }

		ADObjectId OutboundConnector { get; }

		string Name { get; }

		ADObjectId OrganizationRelationship { get; }
	}
}
