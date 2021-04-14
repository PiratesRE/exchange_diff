using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "RemoteDomain", DefaultParameterSetName = "Identity")]
	public class GetRemoteDomain : GetMultitenancySystemConfigurationObjectTask<RemoteDomainIdParameter, DomainContentConfig>
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
