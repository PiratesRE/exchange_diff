using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("get", "ActiveSyncMailboxPolicy", DefaultParameterSetName = "Identity")]
	public class GetActiveSyncMailboxPolicy : GetMailboxPolicyBase<ActiveSyncMailboxPolicy>
	{
		protected override void InternalProcessRecord()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Get-ActiveSyncMailboxPolicy", "Get-MobileDeviceMailboxPolicy"));
			base.InternalProcessRecord();
		}
	}
}
