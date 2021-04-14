using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Export", "RecipientDataProperty", DefaultParameterSetName = "ExportPicture", SupportsShouldProcess = true)]
	public sealed class ExportRecipientDataProperty : RecipientObjectActionTask<MailboxUserContactIdParameter, ADRecipient>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MailboxUserContactIdParameter Identity
		{
			get
			{
				return (MailboxUserContactIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(ParameterSetName = "ExportPicture")]
		public SwitchParameter Picture
		{
			get
			{
				return (SwitchParameter)(base.Fields["Picture"] ?? false);
			}
			set
			{
				base.Fields["Picture"] = value;
			}
		}

		[Parameter(ParameterSetName = "ExportSpokenName")]
		public SwitchParameter SpokenName
		{
			get
			{
				return (SwitchParameter)(base.Fields["SpokenName"] ?? false);
			}
			set
			{
				base.Fields["SpokenName"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportRecipientDataProperty(this.Identity.ToString());
			}
		}

		public ExportRecipientDataProperty()
		{
			this.data = new BinaryFileDataObject();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Picture.IsPresent)
			{
				if (this.DataObject.ThumbnailPhoto == null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorRecipientDoesNotHavePicture(this.DataObject.Name)), ErrorCategory.InvalidData, null);
					return;
				}
			}
			else if (this.SpokenName.IsPresent)
			{
				if (this.DataObject.UMSpokenName == null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorRecipientDoesNotHaveSpokenName(this.DataObject.Name)), ErrorCategory.InvalidData, null);
					return;
				}
			}
			else
			{
				base.WriteError(new LocalizedException(Strings.ErrorUseDataPropertyNameParameter), ErrorCategory.InvalidData, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.data.SetIdentity(this.DataObject.Identity);
			if (this.Picture.IsPresent)
			{
				this.data.FileData = this.DataObject.ThumbnailPhoto;
			}
			else if (this.SpokenName.IsPresent)
			{
				this.data.FileData = this.DataObject.UMSpokenName;
			}
			base.WriteObject(this.data);
		}

		private BinaryFileDataObject data;
	}
}
