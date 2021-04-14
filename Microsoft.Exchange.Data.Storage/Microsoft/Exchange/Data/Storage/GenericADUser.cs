using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GenericADUser : IGenericADUser, IFederatedIdentityParameters
	{
		public GenericADUser()
		{
			this.Alias = string.Empty;
			this.RecipientType = RecipientType.Invalid;
			this.RecipientTypeDetails = RecipientTypeDetails.None;
			this.GrantSendOnBehalfTo = new ADMultiValuedProperty<ADObjectId>();
			this.Languages = new PreferredCultures();
			this.IsResource = null;
			this.EmailAddresses = new ProxyAddressCollection();
			this.AggregatedMailboxGuids = Array<Guid>.Empty;
			this.MailboxLocations = Enumerable.Empty<IMailboxLocationInfo>();
		}

		public GenericADUser(IGenericADUser adUser)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			this.ObjectId = adUser.ObjectId;
			this.DisplayName = adUser.DisplayName;
			this.UserPrincipalName = adUser.UserPrincipalName;
			this.LegacyDn = adUser.LegacyDn;
			this.Alias = adUser.Alias;
			this.DefaultPublicFolderMailbox = adUser.DefaultPublicFolderMailbox;
			this.Sid = adUser.Sid;
			this.MasterAccountSid = adUser.MasterAccountSid;
			this.SidHistory = adUser.SidHistory;
			this.GrantSendOnBehalfTo = adUser.GrantSendOnBehalfTo;
			this.Languages = adUser.Languages;
			this.RecipientType = adUser.RecipientType;
			this.RecipientTypeDetails = adUser.RecipientTypeDetails;
			this.IsResource = adUser.IsResource;
			this.PrimarySmtpAddress = adUser.PrimarySmtpAddress;
			this.ExternalEmailAddress = adUser.ExternalEmailAddress;
			this.EmailAddresses = adUser.EmailAddresses;
			this.OrganizationId = adUser.OrganizationId;
			this.MailboxGuid = adUser.MailboxGuid;
			this.MailboxDatabase = adUser.MailboxDatabase;
			this.WhenMailboxCreated = adUser.WhenMailboxCreated;
			this.WindowsLiveID = adUser.WindowsLiveID;
			this.NetId = adUser.NetId;
			this.ModernGroupType = adUser.ModernGroupType;
			this.PublicToGroupSids = adUser.PublicToGroupSids;
			this.ExternalDirectoryObjectId = adUser.ExternalDirectoryObjectId;
			this.ArchiveDatabase = adUser.ArchiveDatabase;
			this.ArchiveGuid = adUser.ArchiveGuid;
			this.ArchiveName = adUser.ArchiveName;
			this.ArchiveState = adUser.ArchiveState;
			this.ArchiveStatus = adUser.ArchiveStatus;
			this.ArchiveDomain = adUser.ArchiveDomain;
			this.AggregatedMailboxGuids = adUser.AggregatedMailboxGuids;
			this.SharePointUrl = adUser.SharePointUrl;
			this.IsMapiEnabled = adUser.IsMapiEnabled;
			this.IsOwaEnabled = adUser.IsOwaEnabled;
			this.IsMowaEnabled = adUser.IsMowaEnabled;
			this.ThrottlingPolicy = adUser.ThrottlingPolicy;
			this.OwaMailboxPolicy = adUser.OwaMailboxPolicy;
			this.MobileDeviceMailboxPolicy = adUser.MobileDeviceMailboxPolicy;
			this.AddressBookPolicy = adUser.AddressBookPolicy;
			this.IsPersonToPersonMessagingEnabled = adUser.IsPersonToPersonMessagingEnabled;
			this.IsMachineToPersonMessagingEnabled = adUser.IsMachineToPersonMessagingEnabled;
			this.SkuCapability = adUser.SkuCapability;
			this.SkuAssigned = adUser.SkuAssigned;
			this.IsMailboxAuditEnabled = adUser.IsMailboxAuditEnabled;
			this.BypassAudit = adUser.BypassAudit;
			this.MailboxAuditLogAgeLimit = adUser.MailboxAuditLogAgeLimit;
			this.AuditAdminOperations = adUser.AuditAdminOperations;
			this.AuditDelegateOperations = adUser.AuditDelegateOperations;
			this.AuditDelegateAdminOperations = adUser.AuditDelegateAdminOperations;
			this.AuditOwnerOperations = adUser.AuditOwnerOperations;
			this.AuditLastAdminAccess = adUser.AuditLastAdminAccess;
			this.AuditLastDelegateAccess = adUser.AuditLastDelegateAccess;
			this.AuditLastExternalAccess = adUser.AuditLastExternalAccess;
			this.QueryBaseDN = adUser.QueryBaseDN;
			this.MailboxLocations = adUser.MailboxLocations;
		}

		public GenericADUser(IExchangePrincipal exchangePrincipal)
		{
			ArgumentValidator.ThrowIfNull("exchangePrincipal", exchangePrincipal);
			this.LegacyDn = exchangePrincipal.LegacyDn;
			this.Alias = exchangePrincipal.Alias;
			this.DefaultPublicFolderMailbox = exchangePrincipal.DefaultPublicFolderMailbox;
			this.Sid = exchangePrincipal.Sid;
			this.MasterAccountSid = exchangePrincipal.MasterAccountSid;
			this.SidHistory = exchangePrincipal.SidHistory;
			this.GrantSendOnBehalfTo = exchangePrincipal.Delegates;
			this.Languages = exchangePrincipal.PreferredCultures;
			this.ObjectId = exchangePrincipal.ObjectId;
			this.RecipientType = exchangePrincipal.RecipientType;
			this.RecipientTypeDetails = exchangePrincipal.RecipientTypeDetails;
			this.IsResource = exchangePrincipal.IsResource;
			this.AggregatedMailboxGuids = exchangePrincipal.AggregatedMailboxGuids;
			this.ModernGroupType = exchangePrincipal.ModernGroupType;
			this.PublicToGroupSids = exchangePrincipal.PublicToGroupSids;
			this.ExternalDirectoryObjectId = exchangePrincipal.ExternalDirectoryObjectId;
			if (exchangePrincipal.MailboxInfo != null)
			{
				this.DisplayName = exchangePrincipal.MailboxInfo.DisplayName;
				this.PrimarySmtpAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress;
				this.ExternalEmailAddress = exchangePrincipal.MailboxInfo.ExternalEmailAddress;
				this.EmailAddresses = new ProxyAddressCollection();
				foreach (ProxyAddress item in exchangePrincipal.MailboxInfo.EmailAddresses)
				{
					this.EmailAddresses.Add(item);
				}
				this.OrganizationId = exchangePrincipal.MailboxInfo.OrganizationId;
				this.MailboxGuid = exchangePrincipal.MailboxInfo.MailboxGuid;
				this.MailboxDatabase = exchangePrincipal.MailboxInfo.MailboxDatabase;
				this.WhenMailboxCreated = exchangePrincipal.MailboxInfo.WhenMailboxCreated;
				IUserPrincipal userPrincipal = exchangePrincipal as IUserPrincipal;
				if (userPrincipal != null)
				{
					this.WindowsLiveID = userPrincipal.WindowsLiveId;
					this.NetId = userPrincipal.NetId;
				}
				IMailboxInfo mailboxInfo = exchangePrincipal.AllMailboxes.FirstOrDefault((IMailboxInfo mailbox) => mailbox.IsArchive);
				if (mailboxInfo != null)
				{
					this.ArchiveGuid = mailboxInfo.MailboxGuid;
					this.ArchiveDatabase = mailboxInfo.MailboxDatabase;
					this.ArchiveName = new string[]
					{
						mailboxInfo.ArchiveName
					};
					this.ArchiveState = mailboxInfo.ArchiveState;
					this.ArchiveStatus = mailboxInfo.ArchiveStatus;
				}
				this.SharePointUrl = exchangePrincipal.MailboxInfo.Configuration.SharePointUrl;
				this.IsMapiEnabled = exchangePrincipal.MailboxInfo.Configuration.IsMapiEnabled;
				this.IsOwaEnabled = exchangePrincipal.MailboxInfo.Configuration.IsOwaEnabled;
				this.IsMowaEnabled = exchangePrincipal.MailboxInfo.Configuration.IsMowaEnabled;
				this.ThrottlingPolicy = exchangePrincipal.MailboxInfo.Configuration.ThrottlingPolicy;
				this.OwaMailboxPolicy = exchangePrincipal.MailboxInfo.Configuration.OwaMailboxPolicy;
				this.MobileDeviceMailboxPolicy = exchangePrincipal.MailboxInfo.Configuration.MobileDeviceMailboxPolicy;
				this.AddressBookPolicy = exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy;
				this.IsPersonToPersonMessagingEnabled = exchangePrincipal.MailboxInfo.Configuration.IsPersonToPersonMessagingEnabled;
				this.IsMachineToPersonMessagingEnabled = exchangePrincipal.MailboxInfo.Configuration.IsMachineToPersonMessagingEnabled;
				this.SkuCapability = exchangePrincipal.MailboxInfo.Configuration.SkuCapability;
				this.SkuAssigned = exchangePrincipal.MailboxInfo.Configuration.SkuAssigned;
				this.IsMailboxAuditEnabled = exchangePrincipal.MailboxInfo.Configuration.IsMailboxAuditEnabled;
				this.BypassAudit = exchangePrincipal.MailboxInfo.Configuration.BypassAudit;
				this.MailboxAuditLogAgeLimit = exchangePrincipal.MailboxInfo.Configuration.MailboxAuditLogAgeLimit;
				this.AuditAdminOperations = exchangePrincipal.MailboxInfo.Configuration.AuditAdminOperations;
				this.AuditDelegateOperations = exchangePrincipal.MailboxInfo.Configuration.AuditDelegateOperations;
				this.AuditDelegateAdminOperations = exchangePrincipal.MailboxInfo.Configuration.AuditDelegateAdminOperations;
				this.AuditOwnerOperations = exchangePrincipal.MailboxInfo.Configuration.AuditOwnerOperations;
				this.AuditLastAdminAccess = exchangePrincipal.MailboxInfo.Configuration.AuditLastAdminAccess;
				this.AuditLastDelegateAccess = exchangePrincipal.MailboxInfo.Configuration.AuditLastDelegateAccess;
				this.AuditLastExternalAccess = exchangePrincipal.MailboxInfo.Configuration.AuditLastExternalAccess;
				this.MailboxLocations = Enumerable.Empty<IMailboxLocationInfo>();
			}
		}

		public GenericADUser(MiniRecipient miniRecipient)
		{
			ArgumentValidator.ThrowIfNull("miniRecipient", miniRecipient);
			this.UserPrincipalName = miniRecipient.UserPrincipalName;
			this.LegacyDn = miniRecipient.LegacyExchangeDN;
			this.DefaultPublicFolderMailbox = miniRecipient.DefaultPublicFolderMailbox;
			this.Sid = miniRecipient.Sid;
			this.MasterAccountSid = miniRecipient.MasterAccountSid;
			this.SidHistory = miniRecipient.SidHistory;
			this.GrantSendOnBehalfTo = miniRecipient.GrantSendOnBehalfTo;
			this.Languages = miniRecipient.Languages;
			this.ObjectId = miniRecipient.Id;
			this.RecipientType = miniRecipient.RecipientType;
			this.RecipientTypeDetails = miniRecipient.RecipientTypeDetails;
			this.IsResource = new bool?(miniRecipient.IsResource);
			this.AggregatedMailboxGuids = miniRecipient.AggregatedMailboxGuids;
			this.ModernGroupType = miniRecipient.ModernGroupType;
			this.PublicToGroupSids = miniRecipient.PublicToGroupSids;
			this.ExternalDirectoryObjectId = miniRecipient.ExternalDirectoryObjectId;
			this.DisplayName = miniRecipient.DisplayName;
			this.PrimarySmtpAddress = miniRecipient.PrimarySmtpAddress;
			this.ExternalEmailAddress = miniRecipient.ExternalEmailAddress;
			this.EmailAddresses = new ProxyAddressCollection();
			foreach (ProxyAddress item in miniRecipient.EmailAddresses)
			{
				this.EmailAddresses.Add(item);
			}
			this.OrganizationId = miniRecipient.OrganizationId;
			this.MailboxGuid = miniRecipient.ExchangeGuid;
			this.MailboxDatabase = miniRecipient.Database;
			this.WhenMailboxCreated = miniRecipient.WhenMailboxCreated;
			this.WindowsLiveID = miniRecipient.WindowsLiveID;
			this.NetId = miniRecipient.NetID;
			this.ArchiveGuid = miniRecipient.ArchiveGuid;
			this.ArchiveDatabase = miniRecipient.ArchiveDatabase;
			this.ArchiveName = miniRecipient.ArchiveName;
			this.ArchiveState = miniRecipient.ArchiveState;
			this.SharePointUrl = miniRecipient.SharePointUrl;
			this.IsMapiEnabled = miniRecipient.MAPIEnabled;
			this.IsOwaEnabled = miniRecipient.OWAEnabled;
			this.IsMowaEnabled = miniRecipient.MOWAEnabled;
			this.ThrottlingPolicy = miniRecipient.ThrottlingPolicy;
			this.OwaMailboxPolicy = miniRecipient.OwaMailboxPolicy;
			this.MobileDeviceMailboxPolicy = miniRecipient.MobileDeviceMailboxPolicy;
			this.AddressBookPolicy = miniRecipient.AddressBookPolicy;
			this.IsPersonToPersonMessagingEnabled = miniRecipient.IsPersonToPersonTextMessagingEnabled;
			this.IsMachineToPersonMessagingEnabled = miniRecipient.IsMachineToPersonTextMessagingEnabled;
			this.SkuCapability = miniRecipient.SKUCapability;
			this.SkuAssigned = miniRecipient.SKUAssigned;
			this.IsMailboxAuditEnabled = miniRecipient.MailboxAuditEnabled;
			this.BypassAudit = miniRecipient.BypassAudit;
			this.MailboxAuditLogAgeLimit = miniRecipient.MailboxAuditLogAgeLimit;
			this.AuditAdminOperations = miniRecipient.AuditAdminOperations;
			this.AuditDelegateOperations = miniRecipient.AuditDelegateOperations;
			this.AuditDelegateAdminOperations = miniRecipient.AuditDelegateAdminOperations;
			this.AuditOwnerOperations = miniRecipient.AuditOwnerOperations;
			this.AuditLastAdminAccess = miniRecipient.AuditLastAdminAccess;
			this.AuditLastDelegateAccess = miniRecipient.AuditLastDelegateAccess;
			this.AuditLastExternalAccess = miniRecipient.AuditLastExternalAccess;
			this.MailboxLocations = Enumerable.Empty<IMailboxLocationInfo>();
		}

		public string DisplayName { get; set; }

		public string UserPrincipalName { get; set; }

		public string LegacyDn { get; set; }

		public string Alias { get; set; }

		public ADObjectId DefaultPublicFolderMailbox { get; set; }

		public SecurityIdentifier Sid { get; set; }

		public SecurityIdentifier MasterAccountSid { get; set; }

		public IEnumerable<SecurityIdentifier> SidHistory { get; set; }

		public IEnumerable<ADObjectId> GrantSendOnBehalfTo { get; set; }

		public IEnumerable<CultureInfo> Languages { get; set; }

		public RecipientType RecipientType { get; set; }

		public RecipientTypeDetails RecipientTypeDetails { get; set; }

		public bool? IsResource { get; set; }

		public SmtpAddress PrimarySmtpAddress { get; set; }

		public ProxyAddress ExternalEmailAddress { get; set; }

		public ProxyAddressCollection EmailAddresses { get; set; }

		public ADObjectId ObjectId { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public string ImmutableId { get; set; }

		public Guid MailboxGuid { get; set; }

		public ADObjectId MailboxDatabase { get; set; }

		public DateTime? WhenMailboxCreated { get; set; }

		public SmtpAddress WindowsLiveID { get; set; }

		public string ImmutableIdPartial { get; private set; }

		public NetID NetId { get; set; }

		public ModernGroupObjectType ModernGroupType { get; set; }

		public IEnumerable<SecurityIdentifier> PublicToGroupSids { get; set; }

		public string ExternalDirectoryObjectId { get; set; }

		public ADObjectId ArchiveDatabase { get; set; }

		public Guid ArchiveGuid { get; set; }

		public IEnumerable<string> ArchiveName { get; set; }

		public ArchiveState ArchiveState { get; set; }

		public ArchiveStatusFlags ArchiveStatus { get; set; }

		public SmtpDomain ArchiveDomain { get; set; }

		public IEnumerable<Guid> AggregatedMailboxGuids { get; set; }

		public IEnumerable<IMailboxLocationInfo> MailboxLocations { get; set; }

		public Uri SharePointUrl { get; set; }

		public bool IsMapiEnabled { get; set; }

		public bool IsOwaEnabled { get; set; }

		public bool IsMowaEnabled { get; set; }

		public ADObjectId ThrottlingPolicy { get; set; }

		public ADObjectId OwaMailboxPolicy { get; set; }

		public ADObjectId MobileDeviceMailboxPolicy { get; set; }

		public ADObjectId AddressBookPolicy { get; set; }

		public bool IsPersonToPersonMessagingEnabled { get; set; }

		public bool IsMachineToPersonMessagingEnabled { get; set; }

		public Capability? SkuCapability { get; set; }

		public bool? SkuAssigned { get; set; }

		public bool IsMailboxAuditEnabled { get; set; }

		public bool BypassAudit { get; set; }

		public EnhancedTimeSpan MailboxAuditLogAgeLimit { get; set; }

		public MailboxAuditOperations AuditAdminOperations { get; set; }

		public MailboxAuditOperations AuditDelegateOperations { get; set; }

		public MailboxAuditOperations AuditDelegateAdminOperations { get; set; }

		public MailboxAuditOperations AuditOwnerOperations { get; set; }

		public DateTime? AuditLastAdminAccess { get; set; }

		public DateTime? AuditLastDelegateAccess { get; set; }

		public DateTime? AuditLastExternalAccess { get; set; }

		public ADObjectId QueryBaseDN { get; set; }

		internal SmtpAddress? FederatedSmtpAddress { get; set; }

		internal FederatedIdentity FederatedIdentity { get; set; }

		public virtual SmtpAddress GetFederatedSmtpAddress()
		{
			if (this.FederatedSmtpAddress == null)
			{
				this.FederatedSmtpAddress = new SmtpAddress?(DirectoryExtensions.GetWithDirectoryExceptionTranslation<SmtpAddress>(() => this.GetFederatedSmtpAddress(this.PrimarySmtpAddress)));
			}
			return this.FederatedSmtpAddress.Value;
		}

		public virtual FederatedIdentity GetFederatedIdentity()
		{
			if (this.FederatedIdentity == null)
			{
				this.FederatedIdentity = DirectoryExtensions.GetWithDirectoryExceptionTranslation<FederatedIdentity>(() => FederatedIdentityHelper.GetFederatedIdentity(this));
			}
			return this.FederatedIdentity;
		}
	}
}
