using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "IPBlockListProvider", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetIPBlockListProvider : SetSystemConfigurationObjectTask<IPBlockListProviderIdParameter, IPBlockListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetIPBlockListProvider(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			IConfigurationSession session = (IConfigurationSession)base.DataSession;
			NewIPListProvider<IPBlockListProvider>.AdjustPriorities(session, this.DataObject, true);
			base.InternalProcessRecord();
		}
	}
}
