using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.PartnerToken
{
	internal interface IOrganizationScopedIdentity : IIdentity
	{
		OrganizationId OrganizationId { get; }

		IStandardBudget AcquireBudget();
	}
}
