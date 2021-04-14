using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class NewMailContactBase : NewMailEnabledRecipientObjectTask<ADContact>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailContact(base.Name.ToString(), this.ExternalEmailAddress.ToString(), base.RecipientContainerId.ToString());
			}
		}

		[Parameter(Mandatory = true)]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return this.DataObject.ExternalEmailAddress;
			}
			set
			{
				this.DataObject.ExternalEmailAddress = value;
			}
		}

		[Parameter]
		public string FirstName
		{
			get
			{
				return this.DataObject.FirstName;
			}
			set
			{
				this.DataObject.FirstName = value;
			}
		}

		[Parameter]
		public string Initials
		{
			get
			{
				return this.DataObject.Initials;
			}
			set
			{
				this.DataObject.Initials = value;
			}
		}

		[Parameter]
		public string LastName
		{
			get
			{
				return this.DataObject.LastName;
			}
			set
			{
				this.DataObject.LastName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UsePreferMessageFormat
		{
			get
			{
				return this.DataObject.UsePreferMessageFormat;
			}
			set
			{
				this.DataObject.UsePreferMessageFormat = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageFormat MessageFormat
		{
			get
			{
				return this.DataObject.MessageFormat;
			}
			set
			{
				this.DataObject.MessageFormat = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return this.DataObject.MessageBodyFormat;
			}
			set
			{
				this.DataObject.MessageBodyFormat = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return this.DataObject.MacAttachmentFormat;
			}
			set
			{
				this.DataObject.MacAttachmentFormat = value;
			}
		}

		public NewMailContactBase()
		{
		}

		protected override void StampDefaultValues(ADContact dataObject)
		{
			base.StampDefaultValues(dataObject);
			dataObject.StampDefaultValues(RecipientType.MailContact);
		}

		protected override void PrepareRecipientObject(ADContact dataObject)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(dataObject);
			MailContactTaskHelper.ValidateExternalEmailAddress(dataObject, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			dataObject.UseMapiRichTextFormat = UseMapiRichTextFormat.Never;
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			DistributionGroupTaskHelper.CheckModerationInMixedEnvironment(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), Strings.WarningLegacyExchangeServerForMailContact);
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(MailContact).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailContact.FromDataObject((ADContact)dataObject);
		}
	}
}
