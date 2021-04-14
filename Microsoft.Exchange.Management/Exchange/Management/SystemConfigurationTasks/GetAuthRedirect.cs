using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AuthRedirect")]
	public sealed class GetAuthRedirect : GetSystemConfigurationObjectTask<AuthRedirectIdParameter, AuthRedirect>
	{
		protected override ObjectId RootId
		{
			get
			{
				return this.ConfigurationSession.GetOrgContainerId().GetChildId(ServiceEndpointContainer.DefaultName);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = base.InternalFilter;
				if (queryFilter == null)
				{
					queryFilter = AuthRedirect.AuthRedirectKeywordsFilter;
				}
				else
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						AuthRedirect.AuthRedirectKeywordsFilter
					});
				}
				return queryFilter;
			}
		}
	}
}
