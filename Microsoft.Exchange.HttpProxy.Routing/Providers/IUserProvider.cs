using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	internal interface IUserProvider
	{
		User FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, string tenantDomain, IRoutingDiagnostics diagnostics);

		User FindBySmtpAddress(SmtpAddress smtpAddress, IRoutingDiagnostics diagnostics);

		User FindByExternalDirectoryObjectId(Guid userGuid, Guid tenantGuid, IRoutingDiagnostics diagnostics);

		User FindByLiveIdMemberName(SmtpAddress liveIdMemberName, string organizationContext, IRoutingDiagnostics diagnostics);

		string FindResourceForestFqdnByAcceptedDomainName(string acceptedDomain, IRoutingDiagnostics diagnostics);

		string FindResourceForestFqdnByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId, IRoutingDiagnostics diagnostics);
	}
}
