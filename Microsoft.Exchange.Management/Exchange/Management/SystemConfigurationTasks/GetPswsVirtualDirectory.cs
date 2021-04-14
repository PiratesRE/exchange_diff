using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PswsVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetPswsVirtualDirectory : GetPowerShellCommonVirtualDirectory<ADPswsVirtualDirectory>
	{
		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.Psws;
			}
		}
	}
}
