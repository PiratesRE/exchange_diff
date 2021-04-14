using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "PerimeterConfigContainer")]
	public sealed class InstallPerimeterConfigContainer : InstallContainerTaskBase<PerimeterConfig>
	{
		protected override void InternalProcessRecord()
		{
			bool flag = base.DataSession.Read<PerimeterConfig>(this.DataObject.Id) != null;
			if (flag)
			{
				base.WriteDebug(string.Format("Updating perimeter config for tenant {0}", this.DataObject.Id));
			}
			else
			{
				base.WriteDebug(string.Format("Installing perimeter config for tenant {0}", this.DataObject.Id));
			}
			if (!flag)
			{
				this.DataObject.EhfAdminAccountSyncEnabled = false;
				this.DataObject.EhfConfigSyncEnabled = true;
				this.DataObject.RouteOutboundViaEhfEnabled = false;
				this.DataObject.RouteOutboundViaFfoFrontendEnabled = true;
				this.DataObject.SyncToHotmailEnabled = false;
				this.DataObject.RMSOFwdSyncEnabled = true;
			}
			base.InternalProcessRecord();
		}
	}
}
