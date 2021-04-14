using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MailContact", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailContact : SetMailContactBase<MailContact>
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
		public SwitchParameter GenerateExternalDirectoryObjectId
		{
			get
			{
				return (SwitchParameter)(base.Fields["GenerateExternalDirectoryObjectId"] ?? false);
			}
			set
			{
				base.Fields["GenerateExternalDirectoryObjectId"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			ADContact dataObject = this.DataObject;
			if (dataObject.RecipientTypeDetails == RecipientTypeDetails.MailForestContact && this.IsObjectStateChanged())
			{
				base.WriteError(new TaskInvalidOperationException(Strings.SetMailForestContactNotAllowed(dataObject.Name)), ExchangeErrorCategory.Client, this.DataObject.Identity);
			}
			if (this.GenerateExternalDirectoryObjectId && (RecipientTaskHelper.GetAcceptedRecipientTypes() & this.DataObject.RecipientTypeDetails) == RecipientTypeDetails.None)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotGenerateExternalDirectoryObjectIdOnInternalRecipientType(this.Identity.ToString(), this.DataObject.RecipientTypeDetails.ToString())), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
			{
				if (this.GenerateExternalDirectoryObjectId && string.IsNullOrEmpty(this.DataObject.ExternalDirectoryObjectId))
				{
					this.DataObject.ExternalDirectoryObjectId = Guid.NewGuid().ToString();
				}
				base.InternalProcessRecord();
			}
		}
	}
}
