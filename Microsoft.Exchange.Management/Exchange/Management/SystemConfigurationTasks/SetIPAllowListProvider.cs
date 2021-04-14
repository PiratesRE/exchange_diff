using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "IPAllowListProvider", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetIPAllowListProvider : SetSystemConfigurationObjectTask<IPAllowListProviderIdParameter, IPAllowListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetIPAllowListProvider(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			IConfigurationSession session = (IConfigurationSession)base.DataSession;
			NewIPListProvider<IPAllowListProvider>.AdjustPriorities(session, this.DataObject, true);
			base.InternalProcessRecord();
		}
	}
}
