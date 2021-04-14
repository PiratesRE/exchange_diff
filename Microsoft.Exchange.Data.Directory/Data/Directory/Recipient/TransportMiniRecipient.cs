using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class TransportMiniRecipient : MiniRecipient
	{
		internal TransportMiniRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public TransportMiniRecipient()
		{
		}

		public ADObjectId AcceptMessagesOnlyFrom
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.AcceptMessagesOnlyFrom];
			}
		}

		public ADMultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
		}

		public bool AntispamBypassEnabled
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.AntispamBypassEnabled];
			}
		}

		public ADMultiValuedProperty<ADObjectId> ApprovalApplications
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.ApprovalApplications];
			}
		}

		public ADObjectId ArbitrationMailbox
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.ArbitrationMailbox];
			}
		}

		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[TransportMiniRecipientSchema.BlockedSendersHash];
			}
		}

		public ADMultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.BypassModerationFrom];
			}
		}

		public ADMultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.BypassModerationFromDLMembers];
			}
		}

		public string C
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.C];
			}
		}

		public string City
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.City];
			}
		}

		public string Company
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Company];
			}
		}

		public string CustomAttribute1
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute1];
			}
		}

		public string CustomAttribute2
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute2];
			}
		}

		public string CustomAttribute3
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute3];
			}
		}

		public string CustomAttribute4
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute4];
			}
		}

		public string CustomAttribute5
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute5];
			}
		}

		public string CustomAttribute6
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute6];
			}
		}

		public string CustomAttribute7
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute7];
			}
		}

		public string CustomAttribute8
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute8];
			}
		}

		public string CustomAttribute9
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute9];
			}
		}

		public string CustomAttribute10
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute10];
			}
		}

		public string CustomAttribute11
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute11];
			}
		}

		public string CustomAttribute12
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute12];
			}
		}

		public string CustomAttribute13
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute13];
			}
		}

		public string CustomAttribute14
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute14];
			}
		}

		public string CustomAttribute15
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.CustomAttribute15];
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.DeliverToMailboxAndForward];
			}
		}

		public string Department
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Department];
			}
		}

		public ADMultiValuedProperty<ADObjectIdWithString> DLSupervisionList
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectIdWithString>)this[TransportMiniRecipientSchema.DLSupervisionList];
			}
		}

		public bool DowngradeHighPriorityMessagesEnabled
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.DowngradeHighPriorityMessagesEnabled];
			}
		}

		public ElcMailboxFlags ElcMailboxFlags
		{
			get
			{
				return (ElcMailboxFlags)this[TransportMiniRecipientSchema.ElcMailboxFlags];
			}
		}

		public ADObjectId ElcPolicyTemplate
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.ElcPolicyTemplate];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.ExtensionCustomAttribute1];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.ExtensionCustomAttribute2];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.ExtensionCustomAttribute3];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.ExtensionCustomAttribute4];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.ExtensionCustomAttribute5];
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[TransportMiniRecipientSchema.ExternalOofOptions];
			}
		}

		public string Fax
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Fax];
			}
		}

		public string FirstName
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.FirstName];
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.ForwardingAddress];
			}
		}

		public ProxyAddress ForwardingSmtpAddress
		{
			get
			{
				return (ProxyAddress)this[TransportMiniRecipientSchema.ForwardingSmtpAddress];
			}
		}

		public ADObjectId HomeMtaServerId
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.HomeMtaServerId];
			}
		}

		public string HomePhone
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.HomePhone];
			}
		}

		public string Initials
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Initials];
			}
		}

		public ADMultiValuedProperty<ADObjectIdWithString> InternalRecipientSupervisionList
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectIdWithString>)this[TransportMiniRecipientSchema.InternalRecipientSupervisionList];
			}
		}

		public int InternetEncoding
		{
			get
			{
				return (int)this[TransportMiniRecipientSchema.InternetEncoding];
			}
		}

		public string LanguagesRaw
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.LanguagesRaw];
			}
		}

		public string LastName
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.LastName];
			}
		}

		public ADMultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.ManagedBy];
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.Manager];
			}
		}

		public bool? MapiRecipient
		{
			get
			{
				return (bool?)this[TransportMiniRecipientSchema.MapiRecipient];
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportMiniRecipientSchema.MaxReceiveSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportMiniRecipientSchema.MaxSendSize];
			}
		}

		public string MobilePhone
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.MobilePhone];
			}
		}

		public ADMultiValuedProperty<ADObjectId> ModeratedBy
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.ModeratedBy];
			}
		}

		public bool ModerationEnabled
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.ModerationEnabled];
			}
		}

		public int ModerationFlags
		{
			get
			{
				return (int)this[TransportMiniRecipientSchema.ModerationFlags];
			}
		}

		public string Notes
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Notes];
			}
		}

		public string Office
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Office];
			}
		}

		public ADMultiValuedProperty<ADObjectIdWithString> OneOffSupervisionList
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectIdWithString>)this[TransportMiniRecipientSchema.OneOffSupervisionList];
			}
		}

		public bool OpenDomainRoutingDisabled
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.OpenDomainRoutingDisabled];
			}
		}

		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.OtherFax];
			}
		}

		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.OtherHomePhone];
			}
		}

		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportMiniRecipientSchema.OtherTelephone];
			}
		}

		public string Pager
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Pager];
			}
		}

		public string Phone
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Phone];
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.PostalCode];
			}
		}

		public string PostOfficeBox
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.PostOfficeBox];
			}
		}

		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportMiniRecipientSchema.ProhibitSendQuota];
			}
		}

		public ADObjectId PublicFolderContentMailbox
		{
			get
			{
				return (ADObjectId)this[TransportMiniRecipientSchema.PublicFolderContentMailbox];
			}
		}

		public string PublicFolderEntryId
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.PublicFolderEntryId];
			}
		}

		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[TransportMiniRecipientSchema.RecipientDisplayType];
			}
		}

		public Unlimited<int> RecipientLimits
		{
			get
			{
				return (Unlimited<int>)this[TransportMiniRecipientSchema.RecipientLimits];
			}
		}

		public RecipientTypeDetails RecipientTypeDetailsValue
		{
			get
			{
				return (RecipientTypeDetails)this[TransportMiniRecipientSchema.RecipientTypeDetailsValue];
			}
		}

		public ADMultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.RejectMessagesFrom];
			}
		}

		public ADMultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return (ADMultiValuedProperty<ADObjectId>)this[TransportMiniRecipientSchema.RejectMessagesFromDLMembers];
			}
		}

		public bool RequireAllSendersAreAuthenticated
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.RequireAllSendersAreAuthenticated];
			}
		}

		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportMiniRecipientSchema.RulesQuota];
			}
		}

		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[TransportMiniRecipientSchema.SafeRecipientsHash];
			}
		}

		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[TransportMiniRecipientSchema.SafeSendersHash];
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.SamAccountName];
			}
		}

		public bool? SCLDeleteEnabled
		{
			get
			{
				return (bool?)this[TransportMiniRecipientSchema.SCLDeleteEnabled];
			}
		}

		public int? SCLDeleteThreshold
		{
			get
			{
				return (int?)this[TransportMiniRecipientSchema.SCLDeleteThreshold];
			}
		}

		public bool? SCLJunkEnabled
		{
			get
			{
				return (bool?)this[TransportMiniRecipientSchema.SCLJunkEnabled];
			}
		}

		public int? SCLJunkThreshold
		{
			get
			{
				return (int?)this[TransportMiniRecipientSchema.SCLJunkThreshold];
			}
		}

		public bool? SCLQuarantineEnabled
		{
			get
			{
				return (bool?)this[TransportMiniRecipientSchema.SCLQuarantineEnabled];
			}
		}

		public int? SCLQuarantineThreshold
		{
			get
			{
				return (int?)this[TransportMiniRecipientSchema.SCLQuarantineThreshold];
			}
		}

		public bool? SCLRejectEnabled
		{
			get
			{
				return (bool?)this[TransportMiniRecipientSchema.SCLRejectEnabled];
			}
		}

		public int? SCLRejectThreshold
		{
			get
			{
				return (int?)this[TransportMiniRecipientSchema.SCLRejectThreshold];
			}
		}

		public DeliveryReportsReceiver SendDeliveryReportsTo
		{
			get
			{
				return (DeliveryReportsReceiver)this[TransportMiniRecipientSchema.SendDeliveryReportsTo];
			}
		}

		public bool SendOofMessageToOriginatorEnabled
		{
			get
			{
				return (bool)this[TransportMiniRecipientSchema.SendOofMessageToOriginatorEnabled];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.ServerName];
			}
		}

		public string SimpleDisplayName
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.SimpleDisplayName];
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.StateOrProvince];
			}
		}

		public string StreetAddress
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.StreetAddress];
			}
		}

		public string Title
		{
			get
			{
				return (string)this[TransportMiniRecipientSchema.Title];
			}
		}

		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[TransportMiniRecipientSchema.WindowsEmailAddress];
			}
		}

		internal bool IsSenderOrP2RecipientEntry { get; private set; }

		internal override ADObjectSchema Schema
		{
			get
			{
				return TransportMiniRecipientSchema.Schema;
			}
		}

		internal void SetSenderOrP2RecipientEntry()
		{
			this.IsSenderOrP2RecipientEntry = true;
		}
	}
}
