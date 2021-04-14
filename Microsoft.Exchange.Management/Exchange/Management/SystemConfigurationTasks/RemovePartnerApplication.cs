using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "PartnerApplication", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemovePartnerApplication : RemoveSystemConfigurationObjectTask<PartnerApplicationIdParameter, PartnerApplication>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemovePartnerApplication(this.Identity.RawIdentity);
			}
		}
	}
}
