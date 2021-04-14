using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "OwaMailboxPolicy", SupportsShouldProcess = true)]
	public sealed class NewOwaMailboxPolicy : NewMailboxPolicyBase<OwaMailboxPolicy>
	{
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOWAMailboxPolicy(base.Name);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.updateExistingDefaultPolicies)
			{
				if (!base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("OWAMailboxPolicy", this.DataObject.Name)))
				{
					return;
				}
				try
				{
					DefaultMailboxPolicyUtility<OwaMailboxPolicy>.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.existingDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			this.DataObject.ActionForUnknownFileAndMIMETypes = AttachmentBlockingActions.Allow;
			base.InternalProcessRecord();
			this.DataObject.AllowedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultAllowedFileTypes);
			this.DataObject.AllowedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultAllowedMimeTypes);
			this.DataObject.BlockedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultBlockedFileTypes);
			this.DataObject.BlockedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultBlockedMimeTypes);
			this.DataObject.ForceSaveFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultForceSaveFileTypes);
			this.DataObject.ForceSaveMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultForceSaveMimeTypes);
			this.DataObject.WebReadyFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyFileTypes);
			this.DataObject.WebReadyMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyMimeTypes);
			base.DataSession.Save(this.DataObject);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.IsDefault)
			{
				this.DataObject.IsDefault = true;
			}
			if (this.DataObject.IsDefault)
			{
				this.existingDefaultPolicies = DefaultOwaMailboxPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession);
				if (this.existingDefaultPolicies.Count > 0)
				{
					this.updateExistingDefaultPolicies = true;
				}
			}
		}
	}
}
