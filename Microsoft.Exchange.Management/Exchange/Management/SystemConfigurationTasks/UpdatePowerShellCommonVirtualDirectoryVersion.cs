using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class UpdatePowerShellCommonVirtualDirectoryVersion<T> : UpdateVirtualDirectoryVersion<T> where T : ADPowerShellCommonVirtualDirectory, new()
	{
		protected abstract PowerShellVirtualDirectoryType AllowedVirtualDirectoryType { get; }

		protected override bool ShouldSkipVDir(T vDir)
		{
			return vDir.VirtualDirectoryType != this.AllowedVirtualDirectoryType;
		}
	}
}
