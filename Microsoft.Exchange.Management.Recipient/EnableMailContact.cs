using System;
using System.Collections.Generic;
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
	[Cmdlet("Enable", "MailContact", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableMailContact : EnableRecipientObjectTask<ContactIdParameter, ADContact>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableMailContact(this.Identity.ToString(), this.ExternalEmailAddress.ToString());
			}
		}

		[Parameter(Mandatory = true)]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)base.Fields[MailContactSchema.ExternalEmailAddress];
			}
			set
			{
				base.Fields[MailContactSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UsePreferMessageFormat
		{
			get
			{
				return (bool)(base.Fields[ADRecipientSchema.UsePreferMessageFormat] ?? false);
			}
			set
			{
				base.Fields[ADRecipientSchema.UsePreferMessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageFormat MessageFormat
		{
			get
			{
				return (MessageFormat)(base.Fields[ADRecipientSchema.MessageFormat] ?? MessageFormat.Mime);
			}
			set
			{
				base.Fields[ADRecipientSchema.MessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return (MessageBodyFormat)(base.Fields[ADRecipientSchema.MessageBodyFormat] ?? MessageBodyFormat.TextAndHtml);
			}
			set
			{
				base.Fields[ADRecipientSchema.MessageBodyFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return (MacAttachmentFormat)(base.Fields[ADRecipientSchema.MacAttachmentFormat] ?? MacAttachmentFormat.BinHex);
			}
			set
			{
				base.Fields[ADRecipientSchema.MacAttachmentFormat] = value;
			}
		}

		protected override bool DelayProvisioning
		{
			get
			{
				return true;
			}
		}

		protected override void PrepareRecipientObject(ref ADContact contact)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(ref contact);
			if (RecipientType.Contact != contact.RecipientType)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidRecipientType(this.Identity.ToString(), contact.RecipientType.ToString())), ErrorCategory.InvalidArgument, contact.Id);
			}
			contact.SetExchangeVersion(contact.MaximumSupportedExchangeObjectVersion);
			List<PropertyDefinition> list = new List<PropertyDefinition>(DisableMailContact.PropertiesToReset);
			MailboxTaskHelper.RemovePersistentProperties(list);
			MailboxTaskHelper.ClearExchangeProperties(contact, list);
			contact.SetExchangeVersion(contact.MaximumSupportedExchangeObjectVersion);
			if (this.DelayProvisioning && base.IsProvisioningLayerAvailable)
			{
				this.ProvisionDefaultValues(new ADContact(), contact);
			}
			contact.ExternalEmailAddress = this.ExternalEmailAddress;
			contact.DeliverToForwardingAddress = false;
			contact.UsePreferMessageFormat = this.UsePreferMessageFormat;
			contact.MessageFormat = this.MessageFormat;
			contact.MessageBodyFormat = this.MessageBodyFormat;
			contact.MacAttachmentFormat = this.MacAttachmentFormat;
			contact.RequireAllSendersAreAuthenticated = false;
			contact.UseMapiRichTextFormat = UseMapiRichTextFormat.UseDefaultSettings;
			contact.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.RemoteMailUser);
			MailContactTaskHelper.ValidateExternalEmailAddress(contact, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			this.WriteResult();
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter();
			MailContact sendToPipeline = new MailContact(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailContact.FromDataObject((ADContact)dataObject);
		}
	}
}
