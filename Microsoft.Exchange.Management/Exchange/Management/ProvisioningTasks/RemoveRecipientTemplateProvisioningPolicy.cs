using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	[Cmdlet("Remove", "RecipientTemplateProvisioningPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRecipientTemplateProvisioningPolicy : RemoveSystemConfigurationObjectTask<ProvisioningPolicyIdParameter, RecipientTemplateProvisioningPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRecipientTemplateProvisioningPolicy(this.Identity.ToString());
			}
		}
	}
}
