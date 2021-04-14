using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "O365SuiteServiceVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetO365SuiteServiceVirtualDirectory : GetExchangeServiceVirtualDirectory<ADO365SuiteServiceVirtualDirectory>
	{
	}
}
