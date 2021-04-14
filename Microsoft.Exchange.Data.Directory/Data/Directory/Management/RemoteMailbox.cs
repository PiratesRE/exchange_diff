using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("RemoteMailbox")]
	[Serializable]
	public class RemoteMailbox : MailUser
	{
		internal static bool IsRemoteMailbox(RecipientTypeDetails recipientTypeDetails)
		{
			return recipientTypeDetails == (RecipientTypeDetails)((ulong)int.MinValue) || recipientTypeDetails == RecipientTypeDetails.RemoteRoomMailbox || recipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox || recipientTypeDetails == RecipientTypeDetails.RemoteTeamMailbox || recipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox;
		}

		public RemoteMailbox()
		{
		}

		public RemoteMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		[Parameter(Mandatory = false)]
		public ProxyAddress RemoteRoutingAddress
		{
			get
			{
				return this.ExternalEmailAddress;
			}
			set
			{
				this.ExternalEmailAddress = value;
			}
		}

		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return (RemoteRecipientType)this[RemoteMailboxSchema.RemoteRecipientType];
			}
			internal set
			{
				this[RemoteMailboxSchema.RemoteRecipientType] = value;
			}
		}

		public string OnPremisesOrganizationalUnit
		{
			get
			{
				return this.OrganizationalUnit;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return (ArchiveState)this[RemoteMailboxSchema.ArchiveState];
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return RemoteMailbox.schema;
			}
		}

		private new ProxyAddress ExternalEmailAddress
		{
			get
			{
				return base.ExternalEmailAddress;
			}
			set
			{
				base.ExternalEmailAddress = value;
			}
		}

		private new string OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
		}

		private new MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return base.MacAttachmentFormat;
			}
			set
			{
				base.MacAttachmentFormat = value;
			}
		}

		private new MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return base.MessageBodyFormat;
			}
			set
			{
				base.MessageBodyFormat = value;
			}
		}

		private new MessageFormat MessageFormat
		{
			get
			{
				return base.MessageFormat;
			}
			set
			{
				base.MessageFormat = value;
			}
		}

		private new bool? SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				base.SKUAssigned = value;
			}
		}

		private new bool UsePreferMessageFormat
		{
			get
			{
				return base.UsePreferMessageFormat;
			}
			set
			{
				base.UsePreferMessageFormat = value;
			}
		}

		private new UseMapiRichTextFormat UseMapiRichTextFormat
		{
			get
			{
				return base.UseMapiRichTextFormat;
			}
			set
			{
				base.UseMapiRichTextFormat = value;
			}
		}

		private new SmtpAddress WindowsLiveID
		{
			get
			{
				return base.WindowsLiveID;
			}
			set
			{
				base.WindowsLiveID = value;
			}
		}

		private new SmtpAddress MicrosoftOnlineServicesID
		{
			get
			{
				return base.MicrosoftOnlineServicesID;
			}
			set
			{
				base.MicrosoftOnlineServicesID = value;
			}
		}

		private new CountryInfo UsageLocation
		{
			get
			{
				return base.UsageLocation;
			}
			set
			{
				base.UsageLocation = value;
			}
		}

		internal new static RemoteMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new RemoteMailbox(dataObject);
		}

		private static RemoteMailboxSchema schema = ObjectSchema.GetInstance<RemoteMailboxSchema>();
	}
}
