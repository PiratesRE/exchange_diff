using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("set", "MailUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailUser : SetMailUserBase<MailUserIdParameter, MailUser>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)base.Fields[ADRecipientSchema.MailboxProvisioningConstraint];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningConstraint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)base.Fields[ADRecipientSchema.MailboxProvisioningPreferences];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningPreferences] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			if (base.Fields.IsModified(ADRecipientSchema.MailboxProvisioningConstraint))
			{
				aduser.MailboxProvisioningConstraint = this.MailboxProvisioningConstraint;
			}
			if (base.Fields.IsModified(ADRecipientSchema.MailboxProvisioningPreferences))
			{
				aduser.MailboxProvisioningPreferences = this.MailboxProvisioningPreferences;
			}
			if (aduser.MailboxProvisioningConstraint != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(new MailboxProvisioningConstraint[]
				{
					aduser.MailboxProvisioningConstraint
				}, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (aduser.MailboxProvisioningPreferences != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(aduser.MailboxProvisioningPreferences, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (ComplianceConfigImpl.JournalArchivingHardeningEnabled && this.DataObject.IsModified(ADRecipientSchema.EmailAddresses) && this.originalJournalArchiveAddress != SmtpAddress.Empty && this.DataObject.JournalArchiveAddress.CompareTo(this.originalJournalArchiveAddress) != 0)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorModifyJournalArchiveAddress), ExchangeErrorCategory.Client, this.DataObject.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
			{
				base.InternalProcessRecord();
			}
		}
	}
}
