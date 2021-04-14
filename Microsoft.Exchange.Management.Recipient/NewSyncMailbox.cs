using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "SyncMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "User")]
	public sealed class NewSyncMailbox : NewMailboxOrSyncMailbox
	{
		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return base.AddressBookPolicy;
			}
			set
			{
				base.AddressBookPolicy = value;
			}
		}

		private new Guid MailboxContainerGuid
		{
			get
			{
				return base.MailboxContainerGuid;
			}
		}

		private new SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return base.ForestWideDomainControllerAffinityByExecutingUser;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ArchiveGuid
		{
			get
			{
				return this.DataObject.ArchiveGuid;
			}
			set
			{
				this.DataObject.ArchiveGuid = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return this.DataObject.ArchiveName;
			}
			set
			{
				this.DataObject.ArchiveName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] BypassModerationFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[MailEnabledRecipientSchema.BypassModerationFrom];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] AcceptMessagesOnlyFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] RejectMessagesFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] RejectMessagesFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] BypassModerationFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AntispamBypassEnabled
		{
			get
			{
				return this.DataObject.AntispamBypassEnabled;
			}
			set
			{
				this.DataObject.AntispamBypassEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AssistantName
		{
			get
			{
				return this.DataObject.AssistantName;
			}
			set
			{
				this.DataObject.AssistantName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return this.DataObject.BlockedSendersHash;
			}
			set
			{
				this.DataObject.BlockedSendersHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute1
		{
			get
			{
				return this.DataObject.CustomAttribute1;
			}
			set
			{
				this.DataObject.CustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute10
		{
			get
			{
				return this.DataObject.CustomAttribute10;
			}
			set
			{
				this.DataObject.CustomAttribute10 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute11
		{
			get
			{
				return this.DataObject.CustomAttribute11;
			}
			set
			{
				this.DataObject.CustomAttribute11 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute12
		{
			get
			{
				return this.DataObject.CustomAttribute12;
			}
			set
			{
				this.DataObject.CustomAttribute12 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute13
		{
			get
			{
				return this.DataObject.CustomAttribute13;
			}
			set
			{
				this.DataObject.CustomAttribute13 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute14
		{
			get
			{
				return this.DataObject.CustomAttribute14;
			}
			set
			{
				this.DataObject.CustomAttribute14 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute15
		{
			get
			{
				return this.DataObject.CustomAttribute15;
			}
			set
			{
				this.DataObject.CustomAttribute15 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute2
		{
			get
			{
				return this.DataObject.CustomAttribute2;
			}
			set
			{
				this.DataObject.CustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute3
		{
			get
			{
				return this.DataObject.CustomAttribute3;
			}
			set
			{
				this.DataObject.CustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute4
		{
			get
			{
				return this.DataObject.CustomAttribute4;
			}
			set
			{
				this.DataObject.CustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute5
		{
			get
			{
				return this.DataObject.CustomAttribute5;
			}
			set
			{
				this.DataObject.CustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute6
		{
			get
			{
				return this.DataObject.CustomAttribute6;
			}
			set
			{
				this.DataObject.CustomAttribute6 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute7
		{
			get
			{
				return this.DataObject.CustomAttribute7;
			}
			set
			{
				this.DataObject.CustomAttribute7 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute8
		{
			get
			{
				return this.DataObject.CustomAttribute8;
			}
			set
			{
				this.DataObject.CustomAttribute8 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute9
		{
			get
			{
				return this.DataObject.CustomAttribute9;
			}
			set
			{
				this.DataObject.CustomAttribute9 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute1;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute2;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute3;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute4;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute5;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<string> MailTipTranslations
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

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return this.DataObject.EmailAddresses;
			}
			set
			{
				this.DataObject.EmailAddresses = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] GrantSendOnBehalfTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				base.Fields[ADRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)base.Fields[ADRecipientSchema.HiddenFromAddressListsEnabled];
			}
			set
			{
				base.Fields[ADRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return this.DataObject.Notes;
			}
			set
			{
				this.DataObject.Notes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? ResourceCapacity
		{
			get
			{
				return this.DataObject.ResourceCapacity;
			}
			set
			{
				this.DataObject.ResourceCapacity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceCustom
		{
			get
			{
				return this.DataObject.ResourceCustom;
			}
			set
			{
				this.DataObject.ResourceCustom = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return this.DataObject.SafeRecipientsHash;
			}
			set
			{
				this.DataObject.SafeRecipientsHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return this.DataObject.SafeSendersHash;
			}
			set
			{
				this.DataObject.SafeSendersHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLDeleteThreshold
		{
			get
			{
				return this.DataObject.SCLDeleteThreshold;
			}
			set
			{
				this.DataObject.SCLDeleteThreshold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLQuarantineThreshold
		{
			get
			{
				return this.DataObject.SCLQuarantineThreshold;
			}
			set
			{
				this.DataObject.SCLQuarantineThreshold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLJunkThreshold
		{
			get
			{
				return this.DataObject.SCLJunkThreshold;
			}
			set
			{
				this.DataObject.SCLJunkThreshold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLRejectThreshold
		{
			get
			{
				return this.DataObject.SCLRejectThreshold;
			}
			set
			{
				this.DataObject.SCLRejectThreshold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] Picture
		{
			get
			{
				return this.DataObject.ThumbnailPhoto;
			}
			set
			{
				this.DataObject.ThumbnailPhoto = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SpokenName
		{
			get
			{
				return this.DataObject.UMSpokenName;
			}
			set
			{
				this.DataObject.UMSpokenName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UseMapiRichTextFormat UseMapiRichTextFormat
		{
			get
			{
				return this.DataObject.UseMapiRichTextFormat;
			}
			set
			{
				this.DataObject.UseMapiRichTextFormat = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return this.DataObject.DirSyncId;
			}
			set
			{
				this.DataObject.DirSyncId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string City
		{
			get
			{
				return this.DataObject.City;
			}
			set
			{
				this.DataObject.City = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Company
		{
			get
			{
				return this.DataObject.Company;
			}
			set
			{
				this.DataObject.Company = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)base.Fields[SyncMailboxSchema.CountryOrRegion];
			}
			set
			{
				base.Fields[SyncMailboxSchema.CountryOrRegion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Co
		{
			get
			{
				return this.DataObject.Co;
			}
			set
			{
				this.DataObject.Co = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string C
		{
			get
			{
				return this.DataObject.C;
			}
			set
			{
				this.DataObject.C = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CountryCode
		{
			get
			{
				return this.DataObject.CountryCode;
			}
			set
			{
				this.DataObject.CountryCode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Department
		{
			get
			{
				return this.DataObject.Department;
			}
			set
			{
				this.DataObject.Department = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Fax
		{
			get
			{
				return this.DataObject.Fax;
			}
			set
			{
				this.DataObject.Fax = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HomePhone
		{
			get
			{
				return this.DataObject.HomePhone;
			}
			set
			{
				this.DataObject.HomePhone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserContactIdParameter Manager
		{
			get
			{
				return (UserContactIdParameter)base.Fields["Manager"];
			}
			set
			{
				base.Fields["Manager"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MobilePhone
		{
			get
			{
				return this.DataObject.MobilePhone;
			}
			set
			{
				this.DataObject.MobilePhone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Office
		{
			get
			{
				return this.DataObject.Office;
			}
			set
			{
				this.DataObject.Office = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return this.DataObject.OtherFax;
			}
			set
			{
				this.DataObject.OtherFax = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return this.DataObject.OtherHomePhone;
			}
			set
			{
				this.DataObject.OtherHomePhone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return this.DataObject.OtherTelephone;
			}
			set
			{
				this.DataObject.OtherTelephone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pager
		{
			get
			{
				return this.DataObject.Pager;
			}
			set
			{
				this.DataObject.Pager = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Phone
		{
			get
			{
				return this.DataObject.Phone;
			}
			set
			{
				this.DataObject.Phone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return this.DataObject.PostalCode;
			}
			set
			{
				this.DataObject.PostalCode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StateOrProvince
		{
			get
			{
				return this.DataObject.StateOrProvince;
			}
			set
			{
				this.DataObject.StateOrProvince = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StreetAddress
		{
			get
			{
				return this.DataObject.StreetAddress;
			}
			set
			{
				this.DataObject.StreetAddress = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelephoneAssistant
		{
			get
			{
				return this.DataObject.TelephoneAssistant;
			}
			set
			{
				this.DataObject.TelephoneAssistant = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Title
		{
			get
			{
				return this.DataObject.Title;
			}
			set
			{
				this.DataObject.Title = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebPage
		{
			get
			{
				return this.DataObject.WebPage;
			}
			set
			{
				this.DataObject.WebPage = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedWithSyncMailbox")]
		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return this.DataObject.MasterAccountSid;
			}
			set
			{
				this.DataObject.MasterAccountSid = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPlanIdParameter MailboxPlanName
		{
			get
			{
				return this.MailboxPlan;
			}
			set
			{
				this.MailboxPlan = value;
			}
		}

		public new MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return base.MailboxPlan;
			}
			set
			{
				base.MailboxPlan = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)base.Fields[MailboxSchema.DeliverToMailboxAndForward];
			}
			set
			{
				base.Fields[MailboxSchema.DeliverToMailboxAndForward] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)base.Fields[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return this.DataObject.HABSeniorityIndex;
			}
			set
			{
				this.DataObject.HABSeniorityIndex = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return this.DataObject.PhoneticDisplayName;
			}
			set
			{
				this.DataObject.PhoneticDisplayName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return this.DataObject.OnPremisesObjectId;
			}
			set
			{
				this.DataObject.OnPremisesObjectId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return this.DataObject.IsDirSynced;
			}
			set
			{
				this.DataObject.IsDirSynced = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return this.DataObject.DirSyncAuthorityMetadata;
			}
			set
			{
				this.DataObject.DirSyncAuthorityMetadata = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DoNotCheckAcceptedDomains
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotCheckAcceptedDomains"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DoNotCheckAcceptedDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return this.DataObject.RemoteRecipientType;
			}
			set
			{
				this.DataObject.RemoteRecipientType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return this.DataObject.UserCertificate;
			}
			set
			{
				this.DataObject.UserCertificate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserSMimeCertificate
		{
			get
			{
				return this.DataObject.UserSMIMECertificate;
			}
			set
			{
				this.DataObject.UserSMIMECertificate = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public WindowsLiveId ResourceWindowsLiveID
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

		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		public SwitchParameter UseExistingResourceLiveId
		{
			get
			{
				return base.UseExistingLiveId;
			}
			set
			{
				base.UseExistingLiveId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public new NetID NetID
		{
			get
			{
				return base.NetID;
			}
			set
			{
				base.NetID = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		public new SwitchParameter BypassLiveId
		{
			get
			{
				return base.BypassLiveId;
			}
			set
			{
				base.BypassLiveId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public new CountryInfo UsageLocation
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

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection SmtpAndX500Addresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields["SmtpAndX500Addresses"];
			}
			set
			{
				base.Fields["SmtpAndX500Addresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection SipAddresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields["SipAddresses"];
			}
			set
			{
				base.Fields["SipAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)base.Fields["ReleaseTrack"];
			}
			set
			{
				base.Fields["ReleaseTrack"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ValidationOrganization
		{
			get
			{
				return (string)base.Fields["ValidationOrganization"];
			}
			set
			{
				base.Fields["ValidationOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> InPlaceHoldsRaw
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields[SyncMailboxSchema.InPlaceHoldsRaw];
			}
			set
			{
				base.Fields[SyncMailboxSchema.InPlaceHoldsRaw] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new SwitchParameter AccountDisabled
		{
			get
			{
				return (SwitchParameter)base.Fields[SyncMailboxSchema.AccountDisabled];
			}
			set
			{
				base.Fields[SyncMailboxSchema.AccountDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return this.DataObject.StsRefreshTokensValidFrom;
			}
			set
			{
				this.DataObject.StsRefreshTokensValidFrom = value;
			}
		}

		public NewSyncMailbox()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfNewSyncMailboxCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulNewSyncMailboxCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageNewSyncMailboxResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageNewSyncMailboxResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageNewSyncMailboxResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageNewSyncMailboxResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageNewSyncMailboxResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageNewSyncMailboxResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalNewSyncMailboxResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.NewSyncMailboxCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.NewSyncMailboxCacheActivePercentageBase;
		}

		protected override bool ShouldCheckAcceptedDomains()
		{
			return !this.DoNotCheckAcceptedDomains;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxOrSyncMailbox.InternalBeginProcessing", LoggerHelper.CmdletPerfMonitors))
			{
				if ((this.SmtpAndX500Addresses != null && this.SmtpAndX500Addresses.Count > 0) || (this.SipAddresses != null && this.SipAddresses.Count > 0))
				{
					this.DataObject.EmailAddresses = SyncTaskHelper.MergeAddresses(this.SmtpAndX500Addresses, this.SipAddresses);
				}
				base.InternalBeginProcessing();
				if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
				}
				if (base.Fields.IsModified(SyncMailboxSchema.CountryOrRegion) && (this.DataObject.IsModified(SyncMailboxSchema.C) || this.DataObject.IsModified(SyncMailboxSchema.Co) || this.DataObject.IsModified(SyncMailboxSchema.CountryCode)))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorConflictCountryOrRegion), ExchangeErrorCategory.Client, null);
				}
				if (base.Fields.IsModified("Manager"))
				{
					this.manager = MailboxTaskHelper.LookupManager(this.Manager, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), ExchangeErrorCategory.Client, base.TenantGlobalCatalogSession);
				}
				if (base.Fields.IsModified(ADRecipientSchema.GrantSendOnBehalfTo) && this.GrantSendOnBehalfTo != null && this.GrantSendOnBehalfTo.Length != 0)
				{
					this.grantSendOnBehalfTo = new MultiValuedProperty<ADRecipient>();
					foreach (RecipientIdParameter recipientIdParameter in this.GrantSendOnBehalfTo)
					{
						ADRecipient item = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
						this.grantSendOnBehalfTo.Add(item);
					}
				}
				if (!base.Fields.IsModified(ADRecipientSchema.BypassModerationFromSendersOrMembers))
				{
					if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom) && this.BypassModerationFrom != null)
					{
						MultiValuedProperty<ADObjectId> multiValuedProperty;
						MultiValuedProperty<ADObjectId> multiValuedProperty2;
						this.bypassModerationFromRecipient = SetMailEnabledRecipientObjectTask<MailboxIdParameter, SyncMailbox, ADUser>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFrom, "BypassModerationFrom", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty, out multiValuedProperty2);
						if (multiValuedProperty != null && multiValuedProperty.Count > 0)
						{
							base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorIndividualRecipientNeeded(multiValuedProperty[0].ToString())), ExchangeErrorCategory.Client, null);
						}
						this.bypassModerationFrom = multiValuedProperty2;
					}
					if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromDLMembers) && this.BypassModerationFromDLMembers != null)
					{
						MultiValuedProperty<ADObjectId> multiValuedProperty3;
						MultiValuedProperty<ADObjectId> multiValuedProperty4;
						this.bypassModerationFromDLMembersRecipient = SetMailEnabledRecipientObjectTask<MailboxIdParameter, SyncMailbox, ADUser>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFromDLMembers, "BypassModerationFromDLMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty3, out multiValuedProperty4);
						if (multiValuedProperty4 != null && multiValuedProperty4.Count > 0)
						{
							base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorGroupRecipientNeeded(multiValuedProperty4[0].ToString())), ExchangeErrorCategory.Client, null);
						}
						this.bypassModerationFromDLMembers = multiValuedProperty3;
					}
				}
				if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom) && this.AcceptMessagesOnlyFrom != null && this.AcceptMessagesOnlyFrom.Length != 0)
				{
					this.acceptMessagesOnlyFrom = new MultiValuedProperty<ADRecipient>();
					foreach (DeliveryRecipientIdParameter recipientIdParameter2 in this.AcceptMessagesOnlyFrom)
					{
						ADRecipient item2 = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
						this.acceptMessagesOnlyFrom.Add(item2);
					}
				}
				if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers) && this.AcceptMessagesOnlyFromDLMembers != null && this.AcceptMessagesOnlyFromDLMembers.Length != 0)
				{
					this.acceptMessagesOnlyFromDLMembers = new MultiValuedProperty<ADRecipient>();
					foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter in this.AcceptMessagesOnlyFromDLMembers)
					{
						ADRecipient item3 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter.ToString())), ExchangeErrorCategory.Client);
						this.acceptMessagesOnlyFromDLMembers.Add(item3);
					}
				}
				if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFrom) && this.RejectMessagesFrom != null && this.RejectMessagesFrom.Length != 0)
				{
					this.rejectMessagesFrom = new MultiValuedProperty<ADRecipient>();
					foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter2 in this.RejectMessagesFrom)
					{
						ADRecipient item4 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
						this.rejectMessagesFrom.Add(item4);
					}
				}
				if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromDLMembers) && this.RejectMessagesFromDLMembers != null && this.RejectMessagesFromDLMembers.Length != 0)
				{
					this.rejectMessagesFromDLMembers = new MultiValuedProperty<ADRecipient>();
					foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter3 in this.RejectMessagesFromDLMembers)
					{
						ADRecipient item5 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter3, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter3.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter3.ToString())), ExchangeErrorCategory.Client);
						this.rejectMessagesFromDLMembers.Add(item5);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.manager != null)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, this.manager, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.grantSendOnBehalfTo != null)
			{
				foreach (ADRecipient recipient in this.grantSendOnBehalfTo)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.bypassModerationFromRecipient != null)
			{
				foreach (ADRecipient recipient2 in this.bypassModerationFromRecipient)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient2, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.bypassModerationFromDLMembersRecipient != null)
			{
				foreach (ADRecipient recipient3 in this.bypassModerationFromDLMembersRecipient)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient3, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.DataObject.IsModified(ADUserSchema.ArchiveGuid) && this.ArchiveGuid != Guid.Empty)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxOrSyncMailbox.IsExchangeGuidOrArchiveGuidUnique", LoggerHelper.CmdletPerfMonitors))
				{
					RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(this.DataObject, ADUserSchema.ArchiveGuid, this.ArchiveGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			user.BypassModerationCheck = true;
			if (base.Fields.IsModified("Manager"))
			{
				user.Manager = ((this.manager == null) ? null : ((ADObjectId)this.manager.Identity));
			}
			if (base.Fields.IsModified(ADRecipientSchema.HiddenFromAddressListsEnabled))
			{
				user.HiddenFromAddressListsEnabled = this.HiddenFromAddressListsEnabled;
			}
			if (base.Fields.IsModified(MailboxSchema.DeliverToMailboxAndForward))
			{
				user.DeliverToMailboxAndForward = this.DeliverToMailboxAndForward;
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled))
			{
				user.RequireAllSendersAreAuthenticated = this.RequireSenderAuthenticationEnabled;
			}
			if (base.Fields.IsModified(SyncMailboxSchema.ReleaseTrack))
			{
				user.ReleaseTrack = this.ReleaseTrack;
			}
			if (base.Fields.IsModified(ADRecipientSchema.GrantSendOnBehalfTo) && this.grantSendOnBehalfTo != null)
			{
				foreach (ADRecipient adrecipient in this.grantSendOnBehalfTo)
				{
					user.GrantSendOnBehalfTo.Add(adrecipient.Identity as ADObjectId);
				}
			}
			if (!base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromSendersOrMembers))
			{
				if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom))
				{
					user.BypassModerationFrom = this.bypassModerationFrom;
				}
				if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromDLMembers))
				{
					user.BypassModerationFromDLMembers = this.bypassModerationFromDLMembers;
				}
			}
			if (this.DataObject.IsModified(ADRecipientSchema.EmailAddresses))
			{
				user.EmailAddresses = SyncTaskHelper.FilterDuplicateEmailAddresses(base.TenantGlobalCatalogSession, this.EmailAddresses, this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom))
			{
				user.AcceptMessagesOnlyFrom = (from c in this.acceptMessagesOnlyFrom
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers))
			{
				user.AcceptMessagesOnlyFromDLMembers = (from c in this.acceptMessagesOnlyFromDLMembers
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFrom))
			{
				user.RejectMessagesFrom = (from c in this.rejectMessagesFrom
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromDLMembers))
			{
				user.RejectMessagesFromDLMembers = (from c in this.rejectMessagesFromDLMembers
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(SyncMailboxSchema.CountryOrRegion))
			{
				user.CountryOrRegion = this.CountryOrRegion;
			}
			if (base.Fields.IsModified(SyncMailboxSchema.InPlaceHoldsRaw))
			{
				user[ADRecipientSchema.InPlaceHoldsRaw] = this.InPlaceHoldsRaw;
			}
			if (base.Fields.IsModified(ADRecipientSchema.Certificate))
			{
				user.UserCertificate = this.UserCertificate;
			}
			if (base.Fields.IsModified(ADRecipientSchema.SMimeCertificate))
			{
				user.UserSMIMECertificate = this.UserSMimeCertificate;
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxOrSyncMailbox.WriteResult", LoggerHelper.CmdletPerfMonitors))
			{
				ADUser aduser = (ADUser)result;
				if (null != aduser.MasterAccountSid)
				{
					aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
				if (this.mailboxPlanObject != null)
				{
					aduser.MailboxPlanName = this.mailboxPlanObject.DisplayName;
				}
				aduser.ResetChangeTracking();
				SyncMailbox result2 = new SyncMailbox(aduser);
				base.WriteResult(result2);
				GalsyncCounters.NumberOfMailboxesCreated.Increment();
			}
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(SyncMailbox).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			IConfigurable result;
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxOrSyncMailbox.ConvertDataObjectToPresentationObject", LoggerHelper.CmdletPerfMonitors))
			{
				result = SyncMailbox.FromDataObject((ADUser)dataObject);
			}
			return result;
		}

		protected override void StampChangesAfterSettingPassword()
		{
			base.StampChangesAfterSettingPassword();
			if (base.Fields.IsModified(SyncMailboxSchema.AccountDisabled))
			{
				SyncTaskHelper.SetExchangeAccountDisabledWithADLogon(this.DataObject, this.AccountDisabled);
			}
		}

		private ADObject manager;

		private MultiValuedProperty<ADRecipient> grantSendOnBehalfTo;

		private MultiValuedProperty<ADObjectId> bypassModerationFrom;

		private MultiValuedProperty<ADRecipient> bypassModerationFromRecipient;

		private MultiValuedProperty<ADObjectId> bypassModerationFromDLMembers;

		private MultiValuedProperty<ADRecipient> bypassModerationFromDLMembersRecipient;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFrom;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFromDLMembers;

		private MultiValuedProperty<ADRecipient> rejectMessagesFrom;

		private MultiValuedProperty<ADRecipient> rejectMessagesFromDLMembers;
	}
}
