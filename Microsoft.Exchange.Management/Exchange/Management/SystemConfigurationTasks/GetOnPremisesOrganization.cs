using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OnPremisesOrganization", DefaultParameterSetName = "Identity")]
	public sealed class GetOnPremisesOrganization : GetMultitenancySystemConfigurationObjectTask<OnPremisesOrganizationIdParameter, OnPremisesOrganization>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
