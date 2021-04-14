using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "SyncMailUser", SupportsShouldProcess = true, DefaultParameterSetName = "DisabledUser")]
	public sealed class NewSyncMailUser : NewMailUserBase
	{
		public NewSyncMailUser()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfNewSyncMailuserCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulNewSyncMailuserCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageNewSyncMailuserResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageNewSyncMailuserResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageNewSyncMailuserResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageNewSyncMailuserResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageNewSyncMailuserResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageNewSyncMailuserResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalNewSyncMailuserResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.NewSyncMailuserCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.NewSyncMailuserCacheActivePercentageBase;
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
		public Guid ExchangeGuid
		{
			get
			{
				return this.DataObject.ExchangeGuid;
			}
			set
			{
				this.DataObject.ExchangeGuid = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter ForwardingAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields["ForwardingAddress"];
			}
			set
			{
				base.Fields["ForwardingAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return this.DataObject.DeliverToMailboxAndForward;
			}
			set
			{
				this.DataObject.DeliverToMailboxAndForward = value;
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
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return this.DataObject.HiddenFromAddressListsEnabled;
			}
			set
			{
				this.DataObject.HiddenFromAddressListsEnabled = value;
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
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return this.DataObject.RecipientDisplayType;
			}
			set
			{
				this.DataObject.RecipientDisplayType = value;
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
				return (CountryInfo)base.Fields[SyncMailUserSchema.CountryOrRegion];
			}
			set
			{
				base.Fields[SyncMailUserSchema.CountryOrRegion] = value;
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
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return this.DataObject.ReleaseTrack;
			}
			set
			{
				this.DataObject.ReleaseTrack = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPlanIdParameter IntendedMailboxPlanName
		{
			get
			{
				return (MailboxPlanIdParameter)base.Fields["IntendedMailboxPlan"];
			}
			set
			{
				base.Fields["IntendedMailboxPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return this.DataObject.RequireAllSendersAreAuthenticated;
			}
			set
			{
				this.DataObject.RequireAllSendersAreAuthenticated = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExchangeResourceType? ResourceType
		{
			get
			{
				return this.DataObject.ResourceType;
			}
			set
			{
				this.DataObject.ResourceType = value;
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
				return (MultiValuedProperty<string>)base.Fields[SyncMailUserSchema.ResourceCustom];
			}
			set
			{
				base.Fields[SyncMailUserSchema.ResourceCustom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceMetaData
		{
			get
			{
				return this.DataObject.ResourceMetaData;
			}
			set
			{
				this.DataObject.ResourceMetaData = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ResourcePropertiesDisplay
		{
			get
			{
				return this.DataObject.ResourcePropertiesDisplay;
			}
			set
			{
				this.DataObject.ResourcePropertiesDisplay = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceSearchProperties
		{
			get
			{
				return this.DataObject.ResourceSearchProperties;
			}
			set
			{
				this.DataObject.ResourceSearchProperties = value;
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
		public bool IsCalculatedTargetAddress
		{
			get
			{
				return this.DataObject.IsCalculatedTargetAddress;
			}
			set
			{
				this.DataObject.IsCalculatedTargetAddress = value;
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
				return (MultiValuedProperty<string>)base.Fields[SyncMailUserSchema.InPlaceHoldsRaw];
			}
			set
			{
				base.Fields[SyncMailUserSchema.InPlaceHoldsRaw] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ElcMailboxFlags ElcMailboxFlags
		{
			get
			{
				return this.DataObject.ElcMailboxFlags;
			}
			set
			{
				this.DataObject.ElcMailboxFlags = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDateForRetentionHold
		{
			get
			{
				return this.DataObject.StartDateForRetentionHold;
			}
			set
			{
				this.DataObject.StartDateForRetentionHold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDateForRetentionHold
		{
			get
			{
				return this.DataObject.EndDateForRetentionHold;
			}
			set
			{
				this.DataObject.EndDateForRetentionHold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionComment
		{
			get
			{
				return this.DataObject.RetentionComment;
			}
			set
			{
				this.DataObject.RetentionComment = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionUrl
		{
			get
			{
				return this.DataObject.RetentionUrl;
			}
			set
			{
				this.DataObject.RetentionUrl = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxAuditEnabled
		{
			get
			{
				return this.DataObject.MailboxAuditEnabled;
			}
			set
			{
				this.DataObject.MailboxAuditEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return this.DataObject.MailboxAuditLogAgeLimit;
			}
			set
			{
				this.DataObject.MailboxAuditLogAgeLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return this.DataObject.AuditAdminOperations;
			}
			set
			{
				this.DataObject.AuditAdminOperations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return this.DataObject.AuditDelegateAdminOperations;
			}
			set
			{
				this.DataObject.AuditDelegateAdminOperations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return this.DataObject.AuditDelegateOperations;
			}
			set
			{
				this.DataObject.AuditDelegateOperations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return this.DataObject.AuditOwnerOperations;
			}
			set
			{
				this.DataObject.AuditOwnerOperations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BypassAudit
		{
			get
			{
				return this.DataObject.BypassAudit;
			}
			set
			{
				this.DataObject.BypassAudit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LitigationHoldOwner
		{
			get
			{
				return this.DataObject.LitigationHoldOwner;
			}
			set
			{
				this.DataObject.LitigationHoldOwner = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? LitigationHoldDate
		{
			get
			{
				return this.DataObject.LitigationHoldDate;
			}
			set
			{
				this.DataObject.LitigationHoldDate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SiteMailboxOwners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[SyncMailUserSchema.SiteMailboxOwners];
			}
			set
			{
				base.Fields[SyncMailUserSchema.SiteMailboxOwners] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SiteMailboxUsers
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[SyncMailUserSchema.SiteMailboxUsers];
			}
			set
			{
				base.Fields[SyncMailUserSchema.SiteMailboxUsers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? SiteMailboxClosedTime
		{
			get
			{
				return this.DataObject.TeamMailboxClosedTime;
			}
			set
			{
				this.DataObject.TeamMailboxClosedTime = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SharePointUrl
		{
			get
			{
				return this.DataObject.SharePointUrl;
			}
			set
			{
				this.DataObject.SharePointUrl = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AccountDisabled
		{
			get
			{
				return (SwitchParameter)base.Fields[SyncMailUserSchema.AccountDisabled];
			}
			set
			{
				base.Fields[SyncMailUserSchema.AccountDisabled] = value;
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

		protected override bool ShouldCheckAcceptedDomains()
		{
			return !this.DoNotCheckAcceptedDomains;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if ((this.SmtpAndX500Addresses != null && this.SmtpAndX500Addresses.Count > 0) || (this.SipAddresses != null && this.SipAddresses.Count > 0))
			{
				this.DataObject.EmailAddresses = SyncTaskHelper.MergeAddresses(this.SmtpAndX500Addresses, this.SipAddresses);
			}
			base.InternalBeginProcessing();
			if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified(SyncMailUserSchema.CountryOrRegion) && (this.DataObject.IsModified(SyncMailUserSchema.C) || this.DataObject.IsModified(SyncMailUserSchema.Co) || this.DataObject.IsModified(SyncMailUserSchema.CountryCode)))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorConflictCountryOrRegion), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified(SyncMailUserSchema.ResourceCustom) && (this.DataObject.IsModified(SyncMailUserSchema.ResourcePropertiesDisplay) || this.DataObject.IsModified(SyncMailUserSchema.ResourceSearchProperties)))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorConflictResourceCustom), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified("Manager"))
			{
				this.manager = MailboxTaskHelper.LookupManager(this.Manager, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), ExchangeErrorCategory.Client, base.TenantGlobalCatalogSession);
			}
			if (base.Fields.IsModified("ForwardingAddress") && this.ForwardingAddress != null)
			{
				this.forwardingAddress = (ADRecipient)base.GetDataObject<ADRecipient>(this.ForwardingAddress, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.ForwardingAddress.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.ForwardingAddress.ToString())), ExchangeErrorCategory.Client);
			}
			if (base.Fields.IsModified("IntendedMailboxPlan"))
			{
				this.intendedMailboxPlanObject = null;
				if (this.IntendedMailboxPlanName != null)
				{
					this.intendedMailboxPlanObject = base.ProvisioningCache.TryAddAndGetOrganizationDictionaryValue<ADUser, string>(CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId, base.CurrentOrganizationId, this.IntendedMailboxPlanName.RawIdentity, () => (ADUser)base.GetDataObject<ADUser>(this.IntendedMailboxPlanName, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.IntendedMailboxPlanName.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.IntendedMailboxPlanName.ToString())), ExchangeErrorCategory.Client));
				}
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
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom) && this.BypassModerationFrom != null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty;
				MultiValuedProperty<ADObjectId> multiValuedProperty2;
				this.bypassModerationFromRecipient = SetMailEnabledRecipientObjectTask<MailUserIdParameter, SyncMailUser, ADUser>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFrom, "BypassModerationFrom", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty, out multiValuedProperty2);
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
				this.bypassModerationFromDLMembersRecipient = SetMailEnabledRecipientObjectTask<MailUserIdParameter, SyncMailUser, ADUser>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFromDLMembers, "BypassModerationFromDLMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty3, out multiValuedProperty4);
				if (multiValuedProperty4 != null && multiValuedProperty4.Count > 0)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorGroupRecipientNeeded(multiValuedProperty4[0].ToString())), ExchangeErrorCategory.Client, null);
				}
				this.bypassModerationFromDLMembers = multiValuedProperty3;
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom) && this.AcceptMessagesOnlyFrom != null && this.AcceptMessagesOnlyFrom.Length != 0)
			{
				this.acceptMessagesOnlyFrom = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter in this.AcceptMessagesOnlyFrom)
				{
					ADRecipient item2 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter.ToString())), ExchangeErrorCategory.Client);
					this.acceptMessagesOnlyFrom.Add(item2);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers) && this.AcceptMessagesOnlyFromDLMembers != null && this.AcceptMessagesOnlyFromDLMembers.Length != 0)
			{
				this.acceptMessagesOnlyFromDLMembers = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter2 in this.AcceptMessagesOnlyFromDLMembers)
				{
					ADRecipient item3 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
					this.acceptMessagesOnlyFromDLMembers.Add(item3);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFrom) && this.RejectMessagesFrom != null && this.RejectMessagesFrom.Length != 0)
			{
				this.rejectMessagesFrom = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter3 in this.RejectMessagesFrom)
				{
					ADRecipient item4 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter3, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter3.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter3.ToString())), ExchangeErrorCategory.Client);
					this.rejectMessagesFrom.Add(item4);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromDLMembers) && this.RejectMessagesFromDLMembers != null && this.RejectMessagesFromDLMembers.Length != 0)
			{
				this.rejectMessagesFromDLMembers = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter4 in this.RejectMessagesFromDLMembers)
				{
					ADRecipient item5 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter4, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter4.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter4.ToString())), ExchangeErrorCategory.Client);
					this.rejectMessagesFromDLMembers.Add(item5);
				}
			}
			if (base.Fields.IsModified(SyncMailUserSchema.SiteMailboxOwners))
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty5 = SetSyncMailUser.ResolveSiteMailboxOwnersReferenceParameter(this.SiteMailboxOwners, base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new WriteWarningDelegate(this.WriteWarning));
				if (multiValuedProperty5 != null && multiValuedProperty5.Count != 0)
				{
					this.siteMailboxOwners = multiValuedProperty5;
				}
			}
			if (base.Fields.IsModified(SyncMailUserSchema.SiteMailboxUsers) && this.SiteMailboxUsers != null && this.SiteMailboxUsers.Length != 0)
			{
				this.siteMailboxUsers = new MultiValuedProperty<ADObjectId>();
				foreach (RecipientIdParameter recipientIdParameter2 in this.SiteMailboxUsers)
				{
					ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
					if (TeamMailboxMembershipHelper.IsUserQualifiedType(adrecipient))
					{
						this.siteMailboxUsers.Add((ADObjectId)adrecipient.Identity);
					}
					else
					{
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorTeamMailboxUserNotResolved(adrecipient.Identity.ToString())), ExchangeErrorCategory.Client, adrecipient.Identity);
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
			if (this.forwardingAddress != null)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, this.forwardingAddress, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.intendedMailboxPlanObject != null)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, this.intendedMailboxPlanObject, new Task.ErrorLoggerDelegate(base.WriteError));
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
			if (this.DataObject.IsModified(ADMailboxRecipientSchema.ExchangeGuid) && this.DataObject.ExchangeGuid != Guid.Empty && this.DataObject.IsModified(ADUserSchema.ArchiveGuid) && this.DataObject.ArchiveGuid != Guid.Empty && this.DataObject.ExchangeGuid == this.DataObject.ArchiveGuid)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidParameterValue("ExchangeGuid", this.DataObject.ExchangeGuid.ToString())), ExchangeErrorCategory.Client, this.DataObject.Identity);
			}
			if (this.DataObject.IsModified(ADMailboxRecipientSchema.ExchangeGuid) && this.ExchangeGuid != Guid.Empty)
			{
				RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(this.DataObject, ADMailboxRecipientSchema.ExchangeGuid, this.ExchangeGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (this.DataObject.IsModified(ADUserSchema.ArchiveGuid) && this.ArchiveGuid != Guid.Empty)
			{
				RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(this.DataObject, ADUserSchema.ArchiveGuid, this.ArchiveGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
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
			if (base.Fields.IsModified("ForwardingAddress"))
			{
				user.ForwardingAddress = ((this.forwardingAddress == null) ? null : ((ADObjectId)this.forwardingAddress.Identity));
			}
			if (base.Fields.IsModified("IntendedMailboxPlan"))
			{
				user.IntendedMailboxPlan = ((this.intendedMailboxPlanObject == null) ? null : this.intendedMailboxPlanObject.Id);
			}
			if (base.Fields.IsModified("ReleaseTrack"))
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
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom))
			{
				user.BypassModerationFrom = this.bypassModerationFrom;
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromDLMembers))
			{
				user.BypassModerationFromDLMembers = this.bypassModerationFromDLMembers;
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
			if (base.Fields.IsModified(SyncMailUserSchema.SiteMailboxOwners))
			{
				user.Owners = this.siteMailboxOwners;
			}
			if (base.Fields.IsModified(SyncMailUserSchema.SiteMailboxUsers))
			{
				user.DelegateListLink = this.siteMailboxUsers;
			}
			if (base.Fields.IsModified(SyncMailUserSchema.CountryOrRegion))
			{
				user.CountryOrRegion = this.CountryOrRegion;
			}
			if (base.Fields.IsModified(SyncMailUserSchema.ResourceCustom))
			{
				user.ResourceCustom = this.ResourceCustom;
			}
			if (base.Fields.IsModified(SyncMailUserSchema.InPlaceHoldsRaw))
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
			SyncMailUser syncMailUser = new SyncMailUser((ADUser)result);
			if (this.intendedMailboxPlanObject != null)
			{
				syncMailUser.IntendedMailboxPlanName = this.intendedMailboxPlanObject.DisplayName;
				syncMailUser.ResetChangeTracking();
			}
			base.WriteResult(syncMailUser);
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(SyncMailUser).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncMailUser.FromDataObject((ADUser)dataObject);
		}

		protected override void StampChangesAfterSettingPassword()
		{
			base.StampChangesAfterSettingPassword();
			if (base.Fields.IsModified(SyncMailUserSchema.AccountDisabled))
			{
				SyncTaskHelper.SetExchangeAccountDisabled(this.DataObject, this.AccountDisabled);
			}
		}

		private ADObject manager;

		private MultiValuedProperty<ADObjectId> bypassModerationFrom;

		private MultiValuedProperty<ADRecipient> bypassModerationFromRecipient;

		private MultiValuedProperty<ADObjectId> bypassModerationFromDLMembers;

		private MultiValuedProperty<ADRecipient> bypassModerationFromDLMembersRecipient;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFrom;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFromDLMembers;

		private MultiValuedProperty<ADRecipient> rejectMessagesFrom;

		private MultiValuedProperty<ADRecipient> rejectMessagesFromDLMembers;

		private MultiValuedProperty<ADObjectId> siteMailboxOwners;

		private MultiValuedProperty<ADObjectId> siteMailboxUsers;

		private ADRecipient forwardingAddress;

		private ADUser intendedMailboxPlanObject;

		private MultiValuedProperty<ADRecipient> grantSendOnBehalfTo;
	}
}
