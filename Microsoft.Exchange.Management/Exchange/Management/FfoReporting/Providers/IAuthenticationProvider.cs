using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal interface IAuthenticationProvider
	{
		Guid GetExternalDirectoryOrganizationId(OrganizationId currentOrganizationId);

		void ResolveOrganizationId(OrganizationIdParameter organization, Task task);

		IConfigDataProvider CreateConfigSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId);
	}
}
