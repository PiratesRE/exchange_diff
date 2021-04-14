using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "SnackyServiceVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetSnackyServiceVirtualDirectory : GetExchangeServiceVirtualDirectory<ADSnackyServiceVirtualDirectory>
	{
	}
}
