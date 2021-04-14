using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	[Cmdlet("New", "RecipientEnforcementProvisioningPolicy", SupportsShouldProcess = true)]
	public sealed class NewRecipientEnforcementProvisioningPolicy : NewEnforcementProvisioningPolicyTaskBase<RecipientEnforcementProvisioningPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRecipientEnforcementProvisioningPolicy(this.DataObject.Name.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }
	}
}
