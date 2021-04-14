using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "PowerShellVirtualDirectoryVersion", SupportsShouldProcess = true)]
	public sealed class UpdatePowerShellVirtualDirectoryVersion : UpdatePowerShellCommonVirtualDirectoryVersion<ADPowerShellVirtualDirectory>
	{
		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.PowerShell;
			}
		}

		protected override bool ShouldUpdateVirtualDirectory()
		{
			bool flag = base.ShouldUpdateVirtualDirectory();
			if (!flag && (base.Server.IsHubTransportServer || base.Server.IsMailboxServer || base.Server.IsUnifiedMessagingServer || base.Server.IsFrontendTransportServer))
			{
				flag = true;
			}
			return flag;
		}
	}
}
