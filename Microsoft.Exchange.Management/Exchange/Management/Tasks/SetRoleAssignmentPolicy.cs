using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "RoleAssignmentPolicy", SupportsShouldProcess = true)]
	public sealed class SetRoleAssignmentPolicy : SetMailboxPolicyBase<RoleAssignmentPolicy>
	{
		private bool UpdateOtherDefaultPolicies
		{
			get
			{
				return this.otherDefaultPolicies != null && this.otherDefaultPolicies.Count > 0;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? false);
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.UpdateOtherDefaultPolicies)
				{
					return Strings.ConfirmationMessageSwitchRBACPolicy(this.Identity.ToString());
				}
				return base.ConfirmationMessage;
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
			if (this.IsDefault)
			{
				this.DataObject.IsDefault = true;
				QueryFilter extraFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, this.DataObject.Id.ObjectGuid);
				this.otherDefaultPolicies = RoleAssignmentPolicyHelper.GetDefaultPolicies((IConfigurationSession)base.DataSession, extraFilter);
			}
			else if (!this.IsDefault && base.Fields.IsChanged("IsDefault") && this.DataObject.IsDefault)
			{
				base.WriteError(new InvalidOperationException(Strings.ResettingIsDefaultIsNotSupported("IsDefault", "RoleAssignmentPolicy")), ErrorCategory.WriteError, this.DataObject);
			}
			if (base.Fields.IsChanged("Description"))
			{
				this.DataObject.Description = this.Description;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.UpdateOtherDefaultPolicies && !base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("RoleAssignmentPolicy", this.Identity.ToString())))
			{
				return;
			}
			base.InternalProcessRecord();
			if (this.UpdateOtherDefaultPolicies)
			{
				try
				{
					RoleAssignmentPolicyHelper.ClearIsDefaultOnPolicies((IConfigurationSession)base.DataSession, this.otherDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}
	}
}
