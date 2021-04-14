using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "PswsVirtualDirectoryVersion", SupportsShouldProcess = true)]
	public sealed class UpdatePswsVirtualDirectoryVersion : UpdatePowerShellCommonVirtualDirectoryVersion<ADPswsVirtualDirectory>
	{
		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.Psws;
			}
		}

		protected override bool ShouldUpdateVirtualDirectory()
		{
			bool flag = base.ShouldUpdateVirtualDirectory();
			if (!flag && base.Server.IsMailboxServer)
			{
				flag = true;
			}
			return flag;
		}
	}
}
