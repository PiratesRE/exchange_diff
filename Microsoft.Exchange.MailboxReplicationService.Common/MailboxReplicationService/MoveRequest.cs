using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ProvisioningObjectTag("MoveRequest")]
	[Serializable]
	public class MoveRequest : MailEnabledOrgPerson
	{
		public MoveRequest()
		{
			base.SetObjectClass("user");
		}

		public MoveRequest(ADUser dataObject) : base(dataObject)
		{
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[MoveRequestUserSchema.ExchangeGuid];
			}
		}

		public ADObjectId SourceDatabase
		{
			get
			{
				return (ADObjectId)this[MoveRequestUserSchema.SourceDatabase];
			}
		}

		public ADObjectId TargetDatabase
		{
			get
			{
				return (ADObjectId)this[MoveRequestUserSchema.TargetDatabase];
			}
		}

		public ADObjectId SourceArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MoveRequestUserSchema.SourceArchiveDatabase];
			}
		}

		public ADObjectId TargetArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MoveRequestUserSchema.TargetArchiveDatabase];
			}
		}

		public RequestFlags Flags
		{
			get
			{
				return (RequestFlags)this[MoveRequestUserSchema.Flags];
			}
		}

		public string RemoteHostName
		{
			get
			{
				return (string)this[MoveRequestUserSchema.RemoteHostName];
			}
		}

		public string BatchName
		{
			get
			{
				return (string)this[MoveRequestUserSchema.BatchName];
			}
		}

		public RequestStatus Status
		{
			get
			{
				return (RequestStatus)this[MoveRequestUserSchema.Status];
			}
		}

		public RequestStyle RequestStyle
		{
			get
			{
				if ((this.Flags & RequestFlags.CrossOrg) == RequestFlags.None)
				{
					return RequestStyle.IntraOrg;
				}
				return RequestStyle.CrossOrg;
			}
		}

		public RequestDirection Direction
		{
			get
			{
				if ((this.Flags & RequestFlags.Push) == RequestFlags.None)
				{
					return RequestDirection.Pull;
				}
				return RequestDirection.Push;
			}
		}

		public bool IsOffline
		{
			get
			{
				return (this.Flags & RequestFlags.Offline) != RequestFlags.None;
			}
		}

		public bool Protect
		{
			get
			{
				return (this.Flags & RequestFlags.Protected) != RequestFlags.None;
			}
		}

		public bool Suspend
		{
			get
			{
				return (this.Flags & RequestFlags.Suspend) != RequestFlags.None;
			}
		}

		public bool SuspendWhenReadyToComplete
		{
			get
			{
				return (this.Flags & RequestFlags.SuspendWhenReadyToComplete) != RequestFlags.None;
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MoveRequest.schema;
			}
		}

		private new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return base.AcceptMessagesOnlyFrom;
			}
			set
			{
				base.AcceptMessagesOnlyFrom = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return base.AcceptMessagesOnlyFromDLMembers;
			}
			set
			{
				base.AcceptMessagesOnlyFromDLMembers = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return base.AcceptMessagesOnlyFromSendersOrMembers;
			}
			set
			{
				base.AcceptMessagesOnlyFromSendersOrMembers = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> AddressListMembership
		{
			get
			{
				return base.AddressListMembership;
			}
		}

		private new ADObjectId ArbitrationMailbox
		{
			get
			{
				return base.ArbitrationMailbox;
			}
			set
			{
				base.ArbitrationMailbox = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> BypassModerationFromSendersOrMembers
		{
			get
			{
				return base.BypassModerationFromSendersOrMembers;
			}
			set
			{
				base.BypassModerationFromSendersOrMembers = value;
			}
		}

		private new string CustomAttribute1
		{
			get
			{
				return base.CustomAttribute1;
			}
			set
			{
				base.CustomAttribute1 = value;
			}
		}

		private new string CustomAttribute10
		{
			get
			{
				return base.CustomAttribute10;
			}
			set
			{
				base.CustomAttribute10 = value;
			}
		}

		private new string CustomAttribute11
		{
			get
			{
				return base.CustomAttribute11;
			}
			set
			{
				base.CustomAttribute11 = value;
			}
		}

		private new string CustomAttribute12
		{
			get
			{
				return base.CustomAttribute12;
			}
			set
			{
				base.CustomAttribute12 = value;
			}
		}

		private new string CustomAttribute13
		{
			get
			{
				return base.CustomAttribute13;
			}
			set
			{
				base.CustomAttribute13 = value;
			}
		}

		private new string CustomAttribute14
		{
			get
			{
				return base.CustomAttribute14;
			}
			set
			{
				base.CustomAttribute14 = value;
			}
		}

		private new string CustomAttribute15
		{
			get
			{
				return base.CustomAttribute15;
			}
			set
			{
				base.CustomAttribute15 = value;
			}
		}

		private new string CustomAttribute2
		{
			get
			{
				return base.CustomAttribute2;
			}
			set
			{
				base.CustomAttribute2 = value;
			}
		}

		private new string CustomAttribute3
		{
			get
			{
				return base.CustomAttribute3;
			}
			set
			{
				base.CustomAttribute3 = value;
			}
		}

		private new string CustomAttribute4
		{
			get
			{
				return base.CustomAttribute4;
			}
			set
			{
				base.CustomAttribute4 = value;
			}
		}

		private new string CustomAttribute5
		{
			get
			{
				return base.CustomAttribute5;
			}
			set
			{
				base.CustomAttribute5 = value;
			}
		}

		private new string CustomAttribute6
		{
			get
			{
				return base.CustomAttribute6;
			}
			set
			{
				base.CustomAttribute6 = value;
			}
		}

		private new string CustomAttribute7
		{
			get
			{
				return base.CustomAttribute7;
			}
			set
			{
				base.CustomAttribute7 = value;
			}
		}

		private new string CustomAttribute8
		{
			get
			{
				return base.CustomAttribute8;
			}
			set
			{
				base.CustomAttribute8 = value;
			}
		}

		private new string CustomAttribute9
		{
			get
			{
				return base.CustomAttribute9;
			}
			set
			{
				base.CustomAttribute9 = value;
			}
		}

		private new ProxyAddressCollection EmailAddresses
		{
			get
			{
				return base.EmailAddresses;
			}
			set
			{
				base.EmailAddresses = value;
			}
		}

		private new bool EmailAddressPolicyEnabled
		{
			get
			{
				return base.EmailAddressPolicyEnabled;
			}
			set
			{
				base.EmailAddressPolicyEnabled = value;
			}
		}

		private new MultiValuedProperty<string> Extensions
		{
			get
			{
				return base.Extensions;
			}
		}

		private new MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return base.GrantSendOnBehalfTo;
			}
			set
			{
				base.GrantSendOnBehalfTo = value;
			}
		}

		private new bool HasPicture
		{
			get
			{
				return base.HasPicture;
			}
		}

		private new bool HasSpokenName
		{
			get
			{
				return base.HasSpokenName;
			}
		}

		private new bool HiddenFromAddressListsEnabled
		{
			get
			{
				return base.HiddenFromAddressListsEnabled;
			}
			set
			{
				base.HiddenFromAddressListsEnabled = value;
			}
		}

		private new string LegacyExchangeDN
		{
			get
			{
				return base.LegacyExchangeDN;
			}
		}

		private new string MailTip
		{
			get
			{
				return base.MailTip;
			}
			set
			{
				base.MailTip = value;
			}
		}

		private new MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return base.MailTipTranslations;
			}
			set
			{
				base.MailTipTranslations = value;
			}
		}

		private new Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return base.MaxSendSize;
			}
			set
			{
				base.MaxSendSize = value;
			}
		}

		private new Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return base.MaxReceiveSize;
			}
			set
			{
				base.MaxReceiveSize = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> ModeratedBy
		{
			get
			{
				return base.ModeratedBy;
			}
			set
			{
				base.ModeratedBy = value;
			}
		}

		private new bool ModerationEnabled
		{
			get
			{
				return base.ModerationEnabled;
			}
			set
			{
				base.ModerationEnabled = value;
			}
		}

		private new ADObjectId ObjectCategory
		{
			get
			{
				return base.ObjectCategory;
			}
			set
			{
				base.ObjectCategory = value;
			}
		}

		private new MultiValuedProperty<string> ObjectClass
		{
			get
			{
				return base.ObjectClass;
			}
		}

		private new string OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
		}

		private new MultiValuedProperty<string> PoliciesIncluded
		{
			get
			{
				return base.PoliciesIncluded;
			}
		}

		private new MultiValuedProperty<string> PoliciesExcluded
		{
			get
			{
				return base.PoliciesExcluded;
			}
		}

		private new SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return base.PrimarySmtpAddress;
			}
			set
			{
				base.PrimarySmtpAddress = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return base.RejectMessagesFrom;
			}
			set
			{
				base.RejectMessagesFrom = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return base.RejectMessagesFromDLMembers;
			}
			set
			{
				base.RejectMessagesFromDLMembers = value;
			}
		}

		private new MultiValuedProperty<ADObjectId> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return base.RejectMessagesFromSendersOrMembers;
			}
			set
			{
				base.RejectMessagesFromSendersOrMembers = value;
			}
		}

		private new bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return base.RequireSenderAuthenticationEnabled;
			}
			set
			{
				base.RequireSenderAuthenticationEnabled = value;
			}
		}

		private new TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return base.SendModerationNotifications;
			}
			set
			{
				base.SendModerationNotifications = value;
			}
		}

		private new string SimpleDisplayName
		{
			get
			{
				return base.SimpleDisplayName;
			}
			set
			{
				base.SimpleDisplayName = value;
			}
		}

		private new MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return base.UMDtmfMap;
			}
			set
			{
				base.UMDtmfMap = value;
			}
		}

		private new DateTime? WhenChanged
		{
			get
			{
				return base.WhenChanged;
			}
		}

		private new DateTime? WhenCreated
		{
			get
			{
				return base.WhenCreated;
			}
		}

		private new DateTime? WhenChangedUTC
		{
			get
			{
				return base.WhenChangedUTC;
			}
		}

		private new DateTime? WhenCreatedUTC
		{
			get
			{
				return base.WhenCreatedUTC;
			}
		}

		private new SmtpAddress WindowsEmailAddress
		{
			get
			{
				return base.WindowsEmailAddress;
			}
			set
			{
				base.WindowsEmailAddress = value;
			}
		}

		internal static MoveRequest FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			if (dataObject.Database != null)
			{
				dataObject.Database = ADObjectIdResolutionHelper.ResolveDN(dataObject.Database);
			}
			if (dataObject.ArchiveDatabase != null)
			{
				dataObject.ArchiveDatabase = ADObjectIdResolutionHelper.ResolveDN(dataObject.ArchiveDatabase);
			}
			if (dataObject.MailboxMoveSourceMDB != null)
			{
				dataObject.MailboxMoveSourceMDB = ADObjectIdResolutionHelper.ResolveDN(dataObject.MailboxMoveSourceMDB);
			}
			if (dataObject.MailboxMoveTargetMDB != null)
			{
				dataObject.MailboxMoveTargetMDB = ADObjectIdResolutionHelper.ResolveDN(dataObject.MailboxMoveTargetMDB);
			}
			if (dataObject.MailboxMoveSourceArchiveMDB != null)
			{
				dataObject.MailboxMoveSourceArchiveMDB = ADObjectIdResolutionHelper.ResolveDN(dataObject.MailboxMoveSourceArchiveMDB);
			}
			if (dataObject.MailboxMoveTargetArchiveMDB != null)
			{
				dataObject.MailboxMoveTargetArchiveMDB = ADObjectIdResolutionHelper.ResolveDN(dataObject.MailboxMoveTargetArchiveMDB);
			}
			return new MoveRequest(dataObject);
		}

		private static MoveRequestUserSchema schema = ObjectSchema.GetInstance<MoveRequestUserSchema>();
	}
}
