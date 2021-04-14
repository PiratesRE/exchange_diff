using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "O365SuiteServiceVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveO365SuiteServiceVirtualDirectory : RemoveExchangeVirtualDirectory<ADO365SuiteServiceVirtualDirectory>
	{
	}
}
