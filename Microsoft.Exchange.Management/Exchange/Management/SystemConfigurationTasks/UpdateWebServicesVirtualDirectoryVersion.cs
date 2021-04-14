using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "WebServicesVirtualDirectoryVersion", SupportsShouldProcess = true)]
	public sealed class UpdateWebServicesVirtualDirectoryVersion : UpdateVirtualDirectoryVersion<ADWebServicesVirtualDirectory>
	{
	}
}
