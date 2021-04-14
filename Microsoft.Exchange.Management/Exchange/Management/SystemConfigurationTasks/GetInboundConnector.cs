using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "InboundConnector", DefaultParameterSetName = "Identity")]
	public class GetInboundConnector : GetMultitenancySystemConfigurationObjectTask<InboundConnectorIdParameter, TenantInboundConnector>
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
