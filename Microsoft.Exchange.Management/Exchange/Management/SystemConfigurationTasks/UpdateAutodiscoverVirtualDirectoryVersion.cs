using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "AutodiscoverVirtualDirectoryVersion", SupportsShouldProcess = true)]
	public sealed class UpdateAutodiscoverVirtualDirectoryVersion : UpdateVirtualDirectoryVersion<ADAutodiscoverVirtualDirectory>
	{
	}
}
