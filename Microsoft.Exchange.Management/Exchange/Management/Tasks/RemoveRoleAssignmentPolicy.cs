using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "RoleAssignmentPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRoleAssignmentPolicy : RemoveMailboxPolicyBase<RoleAssignmentPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRBACPolicy(this.Identity.ToString());
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
			base.InternalValidate();
			if (base.DataObject.IsDefault)
			{
				base.WriteError(new InvalidOperationException(Strings.RemovingDefaultPolicyIsNotSupported(this.Identity.ToString())), ErrorCategory.WriteError, base.DataObject);
			}
			if (RoleAssignmentPolicyHelper.RoleAssignmentsForPolicyExist((IConfigurationSession)base.DataSession, base.DataObject))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRemovingPolicyInUse(base.DataObject.Id.ToString())), ErrorCategory.WriteError, base.DataObject);
			}
		}

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			base.WriteError(new CannotDeleteAssociatedMailboxPolicyException(this.Identity.ToString()), ErrorCategory.WriteError, base.DataObject);
			return false;
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}
	}
}
