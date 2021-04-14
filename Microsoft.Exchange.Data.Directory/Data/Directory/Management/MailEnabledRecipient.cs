using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class MailEnabledRecipient : ADPresentationObject
	{
		protected MailEnabledRecipient()
		{
		}

		protected MailEnabledRecipient(ADObject dataObject) : base(dataObject)
		{
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				this[MailEnabledRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				this[MailEnabledRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers];
			}
			internal set
			{
				this[MailEnabledRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AddressListMembership
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.AddressListMembership];
			}
		}

		[Parameter(Mandatory = false)]
		public string Alias
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.Alias];
			}
			set
			{
				this[MailEnabledRecipientSchema.Alias] = value;
			}
		}

		public ADObjectId ArbitrationMailbox
		{
			get
			{
				return (ADObjectId)this[MailEnabledRecipientSchema.ArbitrationMailbox];
			}
			internal set
			{
				this[MailEnabledRecipientSchema.ArbitrationMailbox] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.BypassModerationFrom];
			}
			set
			{
				this[MailEnabledRecipientSchema.BypassModerationFrom] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				this[MailEnabledRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> BypassModerationFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.BypassModerationFromSendersOrMembers];
			}
			internal set
			{
				this[MailEnabledRecipientSchema.BypassModerationFromSendersOrMembers] = value;
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.OrganizationalUnit];
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute1
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute1];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute1] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute10
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute10];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute10] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute11
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute11];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute11] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute12
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute12];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute12] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute13
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute13];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute13] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute14
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute14];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute14] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute15
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute15];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute15] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute2
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute2];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute2] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute3
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute3];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute3] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute4
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute4];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute4] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute5
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute5];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute5] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute6
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute6];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute6] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute7
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute7];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute7] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute8
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute8];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute8] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute9
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.CustomAttribute9];
			}
			set
			{
				this[MailEnabledRecipientSchema.CustomAttribute9] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.ExtensionCustomAttribute1];
			}
			set
			{
				this[MailEnabledRecipientSchema.ExtensionCustomAttribute1] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.ExtensionCustomAttribute2];
			}
			set
			{
				this[MailEnabledRecipientSchema.ExtensionCustomAttribute2] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.ExtensionCustomAttribute3];
			}
			set
			{
				this[MailEnabledRecipientSchema.ExtensionCustomAttribute3] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.ExtensionCustomAttribute4];
			}
			set
			{
				this[MailEnabledRecipientSchema.ExtensionCustomAttribute4] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.ExtensionCustomAttribute5];
			}
			set
			{
				this[MailEnabledRecipientSchema.ExtensionCustomAttribute5] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.DisplayName];
			}
			set
			{
				this[MailEnabledRecipientSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[MailEnabledRecipientSchema.EmailAddresses];
			}
			set
			{
				this[MailEnabledRecipientSchema.EmailAddresses] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				this[MailEnabledRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.ExternalDirectoryObjectId];
			}
		}

		[Parameter(Mandatory = false)]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)this[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled];
			}
			set
			{
				this[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}

		[ProvisionalCloneEnabledState(CloneSet.CloneLimitedSet)]
		internal bool HiddenFromAddressListsValue
		{
			get
			{
				return (bool)this[MailEnabledRecipientSchema.HiddenFromAddressListsValue];
			}
			set
			{
				this[MailEnabledRecipientSchema.HiddenFromAddressListsValue] = value;
			}
		}

		public DateTime? LastExchangeChangedTime
		{
			get
			{
				return (DateTime?)this[MailEnabledRecipientSchema.LastExchangeChangedTime];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.LegacyExchangeDN];
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailEnabledRecipientSchema.MaxSendSize];
			}
			set
			{
				this[MailEnabledRecipientSchema.MaxSendSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailEnabledRecipientSchema.MaxReceiveSize];
			}
			set
			{
				this[MailEnabledRecipientSchema.MaxReceiveSize] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ModeratedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.ModeratedBy];
			}
			set
			{
				this[MailEnabledRecipientSchema.ModeratedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ModerationEnabled
		{
			get
			{
				return (bool)this[MailEnabledRecipientSchema.ModerationEnabled];
			}
			set
			{
				this[MailEnabledRecipientSchema.ModerationEnabled] = value;
			}
		}

		public MultiValuedProperty<string> PoliciesIncluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.PoliciesIncluded];
			}
		}

		public MultiValuedProperty<string> PoliciesExcluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.PoliciesExcluded];
			}
		}

		[Parameter(Mandatory = false)]
		public bool EmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[MailEnabledRecipientSchema.EmailAddressPolicyEnabled];
			}
			set
			{
				this[MailEnabledRecipientSchema.EmailAddressPolicyEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[MailEnabledRecipientSchema.PrimarySmtpAddress];
			}
			set
			{
				this[MailEnabledRecipientSchema.PrimarySmtpAddress] = value;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[MailEnabledRecipientSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[MailEnabledRecipientSchema.RecipientTypeDetails];
			}
			internal set
			{
				this[MailEnabledRecipientSchema.RecipientTypeDetails] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				this[MailEnabledRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				this[MailEnabledRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.RejectMessagesFromSendersOrMembers];
			}
			internal set
			{
				this[MailEnabledRecipientSchema.RejectMessagesFromSendersOrMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)this[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled];
			}
			set
			{
				this[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SimpleDisplayName
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.SimpleDisplayName];
			}
			set
			{
				this[MailEnabledRecipientSchema.SimpleDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return (TransportModerationNotificationFlags)this[MailEnabledRecipientSchema.SendModerationNotifications];
			}
			set
			{
				this[MailEnabledRecipientSchema.SendModerationNotifications] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.UMDtmfMap];
			}
			set
			{
				this[MailEnabledRecipientSchema.UMDtmfMap] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[MailEnabledRecipientSchema.WindowsEmailAddress];
			}
			set
			{
				this[MailEnabledRecipientSchema.WindowsEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MailTip
		{
			get
			{
				return (string)this[MailEnabledRecipientSchema.MailTip];
			}
			set
			{
				this[MailEnabledRecipientSchema.MailTip] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledRecipientSchema.MailTipTranslations];
			}
			set
			{
				this[MailEnabledRecipientSchema.MailTipTranslations] = value;
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.DisplayName))
			{
				return this.DisplayName;
			}
			if (!string.IsNullOrEmpty(base.Name))
			{
				return base.Name;
			}
			if (base.Id != null)
			{
				return base.Id.ToString();
			}
			return base.ToString();
		}
	}
}
