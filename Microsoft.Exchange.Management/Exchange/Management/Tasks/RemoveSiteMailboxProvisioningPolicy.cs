using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "SiteMailboxProvisioningPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSiteMailboxProvisioningPolicy : RemoveMailboxPolicyBase<TeamMailboxProvisioningPolicy>
	{
		protected override bool HandleRemoveWithAssociatedUsers()
		{
			return true;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveTeamMailboxProvisioningPolicy(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override void InternalValidate()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (base.DataObject.IsDefault)
			{
				base.WriteError(new CannotRemoveDefaultSiteMailboxProvisioningPolicyException(), ErrorCategory.InvalidOperation, this.Identity);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
