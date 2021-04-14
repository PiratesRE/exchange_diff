using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "ThrottlingPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveThrottlingPolicy : RemoveSystemConfigurationObjectTask<ThrottlingPolicyIdParameter, ThrottlingPolicy>
	{
		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.DataObject.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Global)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotDeleteGlobalThrottlingPolicy), ErrorCategory.InvalidOperation, base.DataObject.Identity);
				return;
			}
			if (!this.Force)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, base.TenantGlobalCatalogSession.SessionSettings, 74, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\throttling\\RemoveThrottlingPolicy.cs");
				if (tenantOrRootOrgRecipientSession.IsThrottlingPolicyInUse(base.DataObject.Id))
				{
					base.WriteError(new CannotRemoveAssociatedThrottlingPolicyException(base.DataObject.Id.DistinguishedName), ErrorCategory.InvalidOperation, null);
					return;
				}
			}
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveThrottlingPolicy(this.Identity.ToString(), base.DataObject.ThrottlingPolicyScope.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}
	}
}
