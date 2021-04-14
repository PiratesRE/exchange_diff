using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ActiveSyncVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMobileSyncVirtualDirectory : SetMobileSyncVirtualDirectoryBase
	{
	}
}
