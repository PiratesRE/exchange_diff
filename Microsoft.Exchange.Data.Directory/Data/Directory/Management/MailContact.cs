using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("MailContact")]
	[Serializable]
	public class MailContact : MailEnabledOrgPerson, IExternalAndEmailAddresses
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailContact.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public MailContact()
		{
		}

		public MailContact(ADContact dataObject) : base(dataObject)
		{
		}

		internal static MailContact FromDataObject(ADContact dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new MailContact(dataObject);
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[MailContactSchema.ExternalEmailAddress];
			}
			set
			{
				this[MailContactSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxRecipientPerMessage
		{
			get
			{
				return (Unlimited<int>)this[MailContactSchema.MaxRecipientPerMessage];
			}
			set
			{
				this[MailContactSchema.MaxRecipientPerMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UseMapiRichTextFormat UseMapiRichTextFormat
		{
			get
			{
				return (UseMapiRichTextFormat)this[MailContactSchema.UseMapiRichTextFormat];
			}
			set
			{
				this[MailContactSchema.UseMapiRichTextFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UsePreferMessageFormat
		{
			get
			{
				return (bool)this[MailContactSchema.UsePreferMessageFormat];
			}
			set
			{
				this[MailContactSchema.UsePreferMessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageFormat MessageFormat
		{
			get
			{
				return (MessageFormat)this[MailContactSchema.MessageFormat];
			}
			set
			{
				this[MailContactSchema.MessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return (MessageBodyFormat)this[MailContactSchema.MessageBodyFormat];
			}
			set
			{
				this[MailContactSchema.MessageBodyFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return (MacAttachmentFormat)this[MailContactSchema.MacAttachmentFormat];
			}
			set
			{
				this[MailContactSchema.MacAttachmentFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailContactSchema.UserCertificate];
			}
			set
			{
				this[MailContactSchema.UserCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserSMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailContactSchema.UserSMimeCertificate];
			}
			set
			{
				this[MailContactSchema.UserSMimeCertificate] = value;
			}
		}

		private static MailContactSchema schema = ObjectSchema.GetInstance<MailContactSchema>();
	}
}
