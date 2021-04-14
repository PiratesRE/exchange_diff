using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Disable", "JournalArchiving", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableJournalArchiving : RemoveMailboxBase<MailboxIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationDisableJournalArchiving;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (adrecipient.RecipientTypeDetails != RecipientTypeDetails.UserMailbox)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			this.mailUser = (ADUser)MailboxTaskHelper.GetJournalArchiveMailUser(base.DataSession as IRecipientSession, (ADUser)adrecipient);
			return adrecipient;
		}

		protected override void InternalValidate()
		{
			this.skipJournalArchivingCheck = true;
			base.InternalValidate();
			if (this.mailUser != null && this.mailUser.IsDirSyncEnabled && MailboxTaskHelper.IsOrgDirSyncEnabled(this.ConfigurationSession, this.mailUser.OrganizationId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorRemoveMailboxWithJournalArchiveWithDirSync), ExchangeErrorCategory.Client, this.mailUser.Identity);
			}
		}

		protected override bool ShouldSoftDeleteObject()
		{
			return DisableJournalArchiving.ShouldSoftDeleteUser(base.DataObject);
		}

		protected override void InternalProcessRecord()
		{
			this.RemoveMailUser();
			base.DataObject.JournalArchiveAddress = SmtpAddress.NullReversePath;
			base.InternalProcessRecord();
		}

		private static bool ShouldSoftDeleteUser(ADUser user)
		{
			return user != null && !(user.OrganizationId == null) && user.OrganizationId.ConfigurationUnit != null && Globals.IsMicrosoftHostedOnly && SoftDeletedTaskHelper.IsSoftDeleteSupportedRecipientTypeDetail(user.RecipientTypeDetails);
		}

		private void RemoveMailUser()
		{
			if (this.mailUser == null)
			{
				this.WriteWarning(Strings.WarningJournalArchiveMailboxHasNoMailUser);
				return;
			}
			if (DisableJournalArchiving.ShouldSoftDeleteUser(this.mailUser))
			{
				SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(base.DataSession as IRecipientSession, this.mailUser, false);
				SoftDeletedTaskHelper.UpdateExchangeGuidForMailEnabledUser(this.mailUser);
				this.mailUser.JournalArchiveAddress = SmtpAddress.NullReversePath;
				base.WriteVerbose(TaskVerboseStringHelper.GetDeleteObjectVerboseString(this.mailUser.Identity, base.DataSession, typeof(ADUser)));
				try
				{
					try
					{
						RecipientTaskHelper.CreateSoftDeletedObjectsContainerIfNecessary(this.mailUser.Id.Parent, base.DomainController);
						base.DataSession.Save(this.mailUser);
					}
					catch (DataSourceTransientException exception)
					{
						base.WriteError(exception, ExchangeErrorCategory.ServerTransient, null);
					}
					return;
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
			}
			base.WriteError(new RecipientTaskException(Strings.ErrorDisableJournalArchiveMailuserNotSoftDeleted), ExchangeErrorCategory.Client, this.mailUser.Identity);
		}

		private ADUser mailUser;
	}
}
