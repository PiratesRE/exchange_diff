using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AutodiscoverVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetAutodiscoverVirtualDirectory : GetExchangeServiceVirtualDirectory<ADAutodiscoverVirtualDirectory>
	{
	}
}
