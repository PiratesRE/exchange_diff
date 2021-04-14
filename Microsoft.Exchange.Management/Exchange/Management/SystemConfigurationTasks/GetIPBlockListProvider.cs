using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IPBlockListProvider")]
	public sealed class GetIPBlockListProvider : GetSystemConfigurationObjectTask<IPBlockListProviderIdParameter, IPBlockListProvider>
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
