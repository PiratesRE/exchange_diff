using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("MailPublicFolder")]
	[Serializable]
	public class MailPublicFolder : MailEnabledRecipient
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailPublicFolder.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public MultiValuedProperty<ADObjectId> Contacts
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailPublicFolderSchema.Contacts];
			}
		}

		public ADObjectId ContentMailbox
		{
			get
			{
				return (ADObjectId)this[MailPublicFolderSchema.ContentMailbox];
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[MailPublicFolderSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[MailPublicFolderSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[MailPublicFolderSchema.ExternalEmailAddress];
			}
			set
			{
				this[MailPublicFolderSchema.ExternalEmailAddress] = value;
			}
		}

		public string EntryId
		{
			get
			{
				return (string)this[MailPublicFolderSchema.EntryId];
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[MailPublicFolderSchema.ForwardingAddress];
			}
			set
			{
				this[MailPublicFolderSchema.ForwardingAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[MailPublicFolderSchema.PhoneticDisplayName];
			}
			set
			{
				this[MailPublicFolderSchema.PhoneticDisplayName] = value;
			}
		}

		public MailPublicFolder()
		{
		}

		public MailPublicFolder(ADPublicFolder dataObject) : base(dataObject)
		{
		}

		internal static MailPublicFolder FromDataObject(ADPublicFolder dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new MailPublicFolder(dataObject);
		}

		private static MailPublicFolderSchema schema = ObjectSchema.GetInstance<MailPublicFolderSchema>();
	}
}
