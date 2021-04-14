using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "RemoteAccountPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetRemoteAccountPolicy : GetMultitenancySystemConfigurationObjectTask<RemoteAccountPolicyIdParameter, RemoteAccountPolicy>
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
