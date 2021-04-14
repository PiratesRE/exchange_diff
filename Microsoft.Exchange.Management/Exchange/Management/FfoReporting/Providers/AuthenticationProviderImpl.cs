using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal class AuthenticationProviderImpl : IAuthenticationProvider
	{
		public Guid GetExternalDirectoryOrganizationId(OrganizationId currentOrganizationId)
		{
			return ADHelper.GetExternalDirectoryOrganizationId(currentOrganizationId);
		}

		public void ResolveOrganizationId(OrganizationIdParameter organization, Task task)
		{
			task.CurrentOrganizationId = ADHelper.ResolveOrganization(organization, task.CurrentOrganizationId, task.ExecutingUserOrganizationId);
		}

		public IConfigDataProvider CreateConfigSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			return ADHelper.CreateConfigSession(currentOrganizationId, executingUserOrganizationId);
		}
	}
}
