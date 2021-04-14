using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "SharingPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSharingPolicy : RemoveSystemConfigurationObjectTask<SharingPolicyIdParameter, SharingPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveSharingPolicy(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			base.InternalValidate();
			this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			FederatedOrganizationId federatedOrganizationId = this.ConfigurationSession.GetFederatedOrganizationId(base.DataObject.OrganizationId);
			if (base.DataObject.Id.Equals(federatedOrganizationId.DefaultSharingPolicyLink))
			{
				base.WriteError(new CannotRemoveDefaultSharingPolicy(), ErrorCategory.InvalidOperation, this.Identity);
			}
			IRecipientSession tenantOrRootOrgRecipientSession;
			if (this.ConfigurationSession.ConfigScope == ConfigScopes.TenantSubTree)
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.ConfigurationSession.DomainController, null, this.ConfigurationSession.Lcid, true, ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.NetworkCredential, this.ConfigurationSession.SessionSettings, this.ConfigurationSession.ConfigScope, 68, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\RemoveSharingPolicy.cs");
			}
			else
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.ConfigurationSession.DomainController, null, this.ConfigurationSession.Lcid, true, ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.NetworkCredential, this.ConfigurationSession.SessionSettings, 80, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\RemoveSharingPolicy.cs");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, IADMailStorageSchema.SharingPolicy, base.DataObject.Id);
			ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array.Length > 0)
			{
				base.WriteError(new CannotRemoveSharingPolicyWithUsersAssignedException(), ErrorCategory.InvalidOperation, this.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
