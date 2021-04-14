using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OutlookProvider", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOutlookProviderTask : SetSystemConfigurationObjectTask<OutlookProviderIdParameter, OutlookProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOutlookProvider(this.Identity.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return OutlookProvider.GetParentContainer(base.DataSession as ITopologyConfigurationSession);
			}
		}
	}
}
