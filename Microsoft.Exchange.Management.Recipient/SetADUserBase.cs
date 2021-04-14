using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.RbacTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetADUserBase<TIdentity, TPublicObject> : SetOrgPersonObjectTask<TIdentity, TPublicObject, ADUser> where TIdentity : IIdentityParameter, new() where TPublicObject : User, new()
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				TIdentity identity = this.Identity;
				return Strings.ConfirmationMessageSetUser(identity.ToString());
			}
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			this.orgAdminHelper = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			SetADUserBase<TIdentity, TPublicObject>.ValidateUserParameters(this.DataObject, this.ConfigurationSession, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, this.DataObject.Id), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client, this.ShouldCheckAcceptedDomains(), base.ProvisioningCache);
			if (this.DataObject.IsChanged(UserSchema.WindowsLiveID) && this.DataObject.WindowsLiveID != SmtpAddress.Empty)
			{
				if (this.ShouldCheckAcceptedDomains())
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(this.ConfigurationSession, this.DataObject.OrganizationId, this.DataObject.WindowsLiveID.Domain, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
					MailboxTaskHelper.IsLiveIdExists((IRecipientSession)base.DataSession, this.DataObject.WindowsLiveID, this.DataObject.NetID, new Task.ErrorLoggerDelegate(base.WriteError));
				}
				this.DataObject.UserPrincipalName = this.DataObject.WindowsLiveID.ToString();
			}
			if (this.DataObject.IsModified(UserSchema.CertificateSubject))
			{
				NewLinkedUser.ValidateCertificateSubject(this.DataObject.CertificateSubject, OrganizationId.ForestWideOrgId.Equals(this.DataObject.OrganizationId) ? null : this.DataObject.OrganizationId.PartitionId, this.DataObject.Id, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.IsDisablingRemotePowerShell() && this.orgAdminHelper.ShouldPreventLastAdminRemoval(this, this.DataObject.OrganizationId) && this.orgAdminHelper.IsLastAdmin(this.DataObject))
			{
				TIdentity identity = this.Identity;
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotDisableRemotePowershelForLastDelegatingOrgAdmin(identity.ToString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		internal static void ValidateUserParameters(ADUser userObject, IConfigurationSession configSession, IRecipientSession globalCatalogSession, Task.TaskVerboseLoggingDelegate verboseLogger, Task.ErrorLoggerDelegate errorLogger, ExchangeErrorCategory errorLoggerCategory, bool shouldCheckAcceptedDomains, ProvisioningCache provisioningCache)
		{
			if (userObject.IsModified(UserSchema.ResetPasswordOnNextLogon) && userObject.ResetPasswordOnNextLogon && (userObject.UserAccountControl & UserAccountControlFlags.DoNotExpirePassword) != UserAccountControlFlags.None)
			{
				errorLogger(new TaskInvalidOperationException(Strings.ErrorUserCannotChangePasswordAtNextLogon(userObject.Identity.ToString())), errorLoggerCategory, userObject.Identity);
			}
			if (userObject.IsModified(UserSchema.UserPrincipalName))
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ValidateExternalEmailAddressInAcceptedDomain.Enabled && shouldCheckAcceptedDomains)
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(configSession, userObject.OrganizationId, RecipientTaskHelper.GetDomainPartOfUserPrincalName(userObject.UserPrincipalName), errorLogger, provisioningCache);
				}
				RecipientTaskHelper.IsUserPrincipalNameUnique(globalCatalogSession, userObject, userObject.UserPrincipalName, verboseLogger, errorLogger, errorLoggerCategory);
			}
			if (userObject.IsModified(UserSchema.SamAccountName))
			{
				RecipientTaskHelper.IsSamAccountNameUnique(globalCatalogSession, userObject, userObject.SamAccountName, verboseLogger, errorLogger, errorLoggerCategory);
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return User.FromDataObject((ADUser)dataObject);
		}

		private bool IsDisablingRemotePowerShell()
		{
			return this.DataObject.IsChanged(ADRecipientSchema.RemotePowerShellEnabled) && !this.DataObject.RemotePowerShellEnabled;
		}

		private RoleAssignmentsGlobalConstraints orgAdminHelper;
	}
}
