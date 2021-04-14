using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IPAllowListProvider")]
	public sealed class GetIPAllowListProvider : GetSystemConfigurationObjectTask<IPAllowListProviderIdParameter, IPAllowListProvider>
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
