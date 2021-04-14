using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IntraOrganizationConnector", DefaultParameterSetName = "Identity")]
	public sealed class GetIntraOrganizationConnector : GetMultitenancySystemConfigurationObjectTask<IntraOrganizationConnectorIdParameter, IntraOrganizationConnector>
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
