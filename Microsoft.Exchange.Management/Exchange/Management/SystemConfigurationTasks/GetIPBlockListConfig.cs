using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IPBlockListConfig")]
	public sealed class GetIPBlockListConfig : GetSingletonSystemConfigurationObjectTask<IPBlockListConfig>
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
