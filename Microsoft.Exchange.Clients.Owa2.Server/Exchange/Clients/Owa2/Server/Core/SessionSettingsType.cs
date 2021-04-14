using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SessionSettingsType
	{
		internal SessionSettingsType()
		{
		}

		internal SessionSettingsType(UserContext userContext, MailboxSession mailboxSession, UserAgent userAgent, CallContext callContext, UMSettingsData umSettings, OwaHelpUrlData helpUrlData)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.ExchangePrincipal == null)
			{
				throw new OwaInvalidRequestException("userContext.ExchangePrincipal is null");
			}
			StorePerformanceCountersCapture countersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			this.userDisplayName = userContext.ExchangePrincipal.MailboxInfo.DisplayName;
			this.userEmailAddress = userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			this.userLegacyExchangeDN = userContext.ExchangePrincipal.LegacyDn;
			this.hasArchive = this.UserHasArchive(userContext.ExchangePrincipal);
			this.archiveDisplayName = (this.hasArchive ? userContext.ExchangePrincipal.GetArchiveMailbox().ArchiveName : string.Empty);
			IEnumerable<string> source = from emailAddress in userContext.ExchangePrincipal.MailboxInfo.EmailAddresses
			select emailAddress.AddressString;
			if (source.Any<string>())
			{
				this.userProxyAddresses = source.ToArray<string>();
			}
			this.UpdateMailboxQuotaLimits(mailboxSession);
			this.isBposUser = userContext.IsBposUser;
			this.userSipUri = userContext.SipUri;
			this.userPrincipalName = userContext.UserPrincipalName;
			this.isGallatin = SessionSettingsType.GetIsGallatin();
			if (userContext.ExchangePrincipal.MailboxInfo.OrganizationId != null)
			{
				this.TenantGuid = userContext.ExchangePrincipal.MailboxInfo.OrganizationId.GetTenantGuid().ToString();
			}
			if (userContext.LogEventCommonData != null)
			{
				this.TenantDomain = userContext.LogEventCommonData.TenantDomain;
			}
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SessionSettingsMisc, countersCapture, true);
			int? maximumMessageSize = SessionSettingsType.GetMaximumMessageSize(mailboxSession);
			this.maxMessageSizeInKb = ((maximumMessageSize != null) ? maximumMessageSize.Value : 5120);
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SessionSettingsMessageSize, countersCapture, true);
			this.isPublicLogon = UserContextUtilities.IsPublicRequest(callContext.HttpContext.Request);
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SessionSettingsIsPublicLogon, countersCapture, true);
			ADUser aduser = null;
			if (userContext.IsExplicitLogon)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, userContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 303, ".ctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\types\\SessionSettingsType.cs");
				aduser = (DirectoryHelper.ReadADRecipient(userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, userContext.ExchangePrincipal.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession) as ADUser);
				if (aduser != null && aduser.SharePointUrl != null)
				{
					this.sharePointUrl = aduser.SharePointUrl.ToString();
					this.sharePointTitle = aduser.DisplayName;
				}
			}
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.TeamMailbox, countersCapture, true);
			if (userContext.LogonIdentity != null)
			{
				OWAMiniRecipient owaminiRecipient = userContext.LogonIdentity.GetOWAMiniRecipient();
				this.LogonEmailAddress = string.Empty;
				if (owaminiRecipient != null)
				{
					SmtpAddress primarySmtpAddress = owaminiRecipient.PrimarySmtpAddress;
					this.LogonEmailAddress = owaminiRecipient.PrimarySmtpAddress.ToString();
				}
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.GetOWAMiniRecipient, countersCapture, false);
			}
			this.MailboxGuid = userContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString();
			this.isExplicitLogon = userContext.IsExplicitLogon;
			this.isExplicitLogonOthersMailbox = false;
			this.canActAsOwner = true;
			countersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			this.SetDefaultFolderMapping(mailboxSession);
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SetDefaultFolderMapping, countersCapture, false);
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			this.userCulture = currentUICulture.Name;
			this.isUserCultureSpeechEnabled = Culture.IsCultureSpeechEnabled(currentUICulture);
			this.isUserCultureRightToLeft = currentUICulture.TextInfo.IsRightToLeft;
			countersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			this.playOnPhoneDialString = umSettings.PlayOnPhoneDialString;
			this.isRequireProtectedPlayOnPhone = umSettings.IsRequireProtectedPlayOnPhone;
			this.isUMEnabled = umSettings.IsUMEnabled;
			if (SyncUtilities.IsDatacenterMode())
			{
				SendAsSubscriptionsAndPeopleConnectResult allSendAsSubscriptionsAndPeopleConnect = SubscriptionManager.GetAllSendAsSubscriptionsAndPeopleConnect(mailboxSession);
				List<PimAggregationSubscription> pimSendAsAggregationSubscriptionList = allSendAsSubscriptionsAndPeopleConnect.PimSendAsAggregationSubscriptionList;
				this.PeopleConnectionsExist = allSendAsSubscriptionsAndPeopleConnect.PeopleConnectionsExist;
				List<AggregatedAccountInfo> list = null;
				if (aduser == null && userContext.ExchangePrincipal != null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession2 = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, userContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 375, ".ctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\types\\SessionSettingsType.cs");
					aduser = (DirectoryHelper.ReadADRecipient(userContext.ExchangePrincipal.MailboxInfo.MailboxGuid, userContext.ExchangePrincipal.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession2) as ADUser);
				}
				if (aduser != null)
				{
					AggregatedAccountHelper aggregatedAccountHelper = new AggregatedAccountHelper(mailboxSession, aduser);
					list = aggregatedAccountHelper.GetListOfAccounts();
				}
				int capacity = pimSendAsAggregationSubscriptionList.Count + ((list != null) ? list.Count : 0);
				List<ConnectedAccountInfo> list2 = new List<ConnectedAccountInfo>(capacity);
				foreach (PimAggregationSubscription pimAggregationSubscription in pimSendAsAggregationSubscriptionList)
				{
					list2.Add(new ConnectedAccountInfo
					{
						SubscriptionGuid = pimAggregationSubscription.SubscriptionGuid,
						EmailAddress = SessionSettingsType.DecodeIdnDomain(pimAggregationSubscription.UserEmailAddress),
						DisplayName = pimAggregationSubscription.UserDisplayName
					});
				}
				if (list != null)
				{
					foreach (AggregatedAccountInfo aggregatedAccountInfo in list)
					{
						bool flag = false;
						string aggregatedAccountEmail = SessionSettingsType.DecodeIdnDomain(aggregatedAccountInfo.SmtpAddress);
						if (!string.IsNullOrWhiteSpace(aggregatedAccountEmail))
						{
							if (list2.Find((ConnectedAccountInfo account) => StringComparer.InvariantCultureIgnoreCase.Equals(account.EmailAddress, aggregatedAccountEmail)) != null)
							{
								break;
							}
							if (!flag)
							{
								list2.Add(new ConnectedAccountInfo
								{
									SubscriptionGuid = aggregatedAccountInfo.RequestGuid,
									EmailAddress = aggregatedAccountEmail,
									DisplayName = aggregatedAccountEmail
								});
							}
						}
					}
				}
				this.connectedAccountInfos = list2.ToArray();
			}
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.IsDatacenterMode, countersCapture, true);
			this.helpUrl = helpUrlData.HelpUrl;
			this.isPublicComputerSession = UserContextUtilities.IsPublicComputerSession(callContext.HttpContext);
			string errorString = string.Empty;
			try
			{
				IMailboxInfo mailboxInfo = userContext.ExchangePrincipal.MailboxInfo;
				TenantPublicFolderConfiguration tenantPublicFolderConfiguration = null;
				if (TenantPublicFolderConfigurationCache.Instance.TryGetValue(mailboxInfo.OrganizationId, out tenantPublicFolderConfiguration))
				{
					ADObjectId defaultPublicFolderMailbox = userContext.ExchangePrincipal.DefaultPublicFolderMailbox;
					PublicFolderRecipient publicFolderRecipient = tenantPublicFolderConfiguration.GetPublicFolderRecipient(mailboxInfo.MailboxGuid, defaultPublicFolderMailbox);
					if (publicFolderRecipient != null)
					{
						if (publicFolderRecipient.IsLocal)
						{
							this.DefaultPublicFolderMailbox = publicFolderRecipient.PrimarySmtpAddress.ToString();
						}
						else if (publicFolderRecipient.ObjectId == null)
						{
							errorString = "publicFolderRecipient not local and ObjectId null";
						}
						else
						{
							errorString = "publicFolderRecipient not local and ObjectId " + publicFolderRecipient.ObjectId.ObjectGuid;
						}
					}
					else
					{
						errorString = "publicFolderRecipient null";
					}
				}
			}
			catch (LocalizedException ex)
			{
				errorString = ex.ToString();
			}
			finally
			{
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.DefaultPublicFolderMailbox, countersCapture, true, errorString);
			}
		}

		[DataMember]
		public bool IsExplicitLogon
		{
			get
			{
				return this.isExplicitLogon;
			}
			set
			{
				this.isExplicitLogon = value;
			}
		}

		[DataMember]
		public bool IsExplicitLogonOthersMailbox
		{
			get
			{
				return this.isExplicitLogonOthersMailbox;
			}
			set
			{
				this.isExplicitLogonOthersMailbox = value;
			}
		}

		[DataMember]
		public bool IsPublicLogon
		{
			get
			{
				return this.isPublicLogon;
			}
			set
			{
				this.isPublicLogon = value;
			}
		}

		[DataMember]
		public bool IsPublicComputerSession
		{
			get
			{
				return this.isPublicComputerSession;
			}
			set
			{
				this.isPublicComputerSession = value;
			}
		}

		[DataMember]
		public bool CanActAsOwner
		{
			get
			{
				return this.canActAsOwner;
			}
			set
			{
				this.canActAsOwner = value;
			}
		}

		[DataMember]
		public bool IsBposUser
		{
			get
			{
				return this.isBposUser;
			}
			set
			{
				this.isBposUser = value;
			}
		}

		[DataMember]
		public bool IsGallatin
		{
			get
			{
				return this.isGallatin;
			}
			set
			{
				this.isGallatin = value;
			}
		}

		[DataMember]
		public string UserDisplayName
		{
			get
			{
				return this.userDisplayName;
			}
			set
			{
				this.userDisplayName = value;
			}
		}

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
			set
			{
				this.userPrincipalName = value;
			}
		}

		[DataMember]
		public string UserEmailAddress
		{
			get
			{
				return this.userEmailAddress;
			}
			set
			{
				this.userEmailAddress = value;
			}
		}

		[DataMember]
		public string UserLegacyExchangeDN
		{
			get
			{
				return this.userLegacyExchangeDN;
			}
			set
			{
				this.userLegacyExchangeDN = value;
			}
		}

		[DataMember]
		public string[] UserProxyAddresses
		{
			get
			{
				return this.userProxyAddresses;
			}
			set
			{
				this.userProxyAddresses = value;
			}
		}

		[DataMember]
		public string LogonEmailAddress { get; set; }

		[DataMember]
		public string MailboxGuid { get; set; }

		[DataMember]
		public string PlayOnPhoneDialString
		{
			get
			{
				return this.playOnPhoneDialString;
			}
			set
			{
				this.playOnPhoneDialString = value;
			}
		}

		[DataMember]
		public bool IsRequireProtectedPlayOnPhone
		{
			get
			{
				return this.isRequireProtectedPlayOnPhone;
			}
			set
			{
				this.isRequireProtectedPlayOnPhone = value;
			}
		}

		[DataMember]
		public bool IsUMEnabled
		{
			get
			{
				return this.isUMEnabled;
			}
			set
			{
				this.isUMEnabled = value;
			}
		}

		[DataMember]
		public bool HasArchive
		{
			get
			{
				return this.hasArchive;
			}
			set
			{
				this.hasArchive = value;
			}
		}

		[DataMember]
		public string ArchiveDisplayName
		{
			get
			{
				return this.archiveDisplayName;
			}
			set
			{
				this.archiveDisplayName = value;
			}
		}

		[DataMember]
		public int MaxMessageSizeInKb
		{
			get
			{
				return this.maxMessageSizeInKb;
			}
			set
			{
				this.maxMessageSizeInKb = value;
			}
		}

		[DataMember]
		public FolderId[] DefaultFolderIds
		{
			get
			{
				return this.defaultFolderIds;
			}
			set
			{
				this.defaultFolderIds = value;
			}
		}

		[DataMember]
		public string[] DefaultFolderNames
		{
			get
			{
				return this.defaultFolderNames;
			}
			set
			{
				this.defaultFolderNames = value;
			}
		}

		[DataMember]
		public string DefaultPublicFolderMailbox { get; set; }

		[DataMember]
		public string UserCulture
		{
			get
			{
				return this.userCulture;
			}
			set
			{
				this.userCulture = value;
			}
		}

		[DataMember]
		public string TenantDomain { get; set; }

		[DataMember]
		public string TenantGuid { get; set; }

		[DataMember]
		public string SharePointUrl
		{
			get
			{
				return this.sharePointUrl;
			}
			set
			{
				this.sharePointUrl = value;
			}
		}

		[DataMember]
		public string SharePointTitle
		{
			get
			{
				return this.sharePointTitle;
			}
			set
			{
				this.sharePointTitle = value;
			}
		}

		[DataMember]
		public bool IsUserCultureSpeechEnabled
		{
			get
			{
				return this.isUserCultureSpeechEnabled;
			}
			set
			{
				this.isUserCultureSpeechEnabled = value;
			}
		}

		[DataMember]
		public bool IsUserCultureRightToLeft
		{
			get
			{
				return this.isUserCultureRightToLeft;
			}
			set
			{
				this.isUserCultureRightToLeft = value;
			}
		}

		[DataMember]
		public long QuotaSend
		{
			get
			{
				return this.quotaSend;
			}
			set
			{
				this.quotaSend = value;
			}
		}

		[DataMember]
		public long QuotaWarning
		{
			get
			{
				return this.quotaWarning;
			}
			set
			{
				this.quotaWarning = value;
			}
		}

		[DataMember]
		public long QuotaUsed
		{
			get
			{
				return this.quotaUsed;
			}
			set
			{
				this.quotaUsed = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public ConnectedAccountInfo[] ConnectedAccountInfos
		{
			get
			{
				return this.connectedAccountInfos;
			}
			set
			{
				this.connectedAccountInfos = value;
			}
		}

		[DataMember]
		public bool PeopleConnectionsExist
		{
			get
			{
				return this.peopleConnectionsExist;
			}
			set
			{
				this.peopleConnectionsExist = value;
			}
		}

		[DataMember]
		public string UserSipUri
		{
			get
			{
				return this.userSipUri;
			}
			set
			{
				this.userSipUri = value;
			}
		}

		[DataMember]
		public string HelpUrl
		{
			get
			{
				return this.helpUrl;
			}
			set
			{
				this.helpUrl = value;
			}
		}

		private static string DecodeIdnDomain(SmtpAddress smtpAddress)
		{
			string domain = smtpAddress.Domain;
			if (!string.IsNullOrEmpty(domain))
			{
				IdnMapping idnMapping = new IdnMapping();
				string unicode = idnMapping.GetUnicode(domain);
				return smtpAddress.Local + "@" + unicode;
			}
			return smtpAddress.ToString();
		}

		private static int? GetMaximumMessageSize(MailboxSession mailboxSession)
		{
			object obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MaxUserMessageSize);
			if (!(obj is PropertyError))
			{
				return new int?((int)obj);
			}
			return null;
		}

		private bool UserHasArchive(ExchangePrincipal principal)
		{
			IMailboxInfo archiveMailbox = principal.GetArchiveMailbox();
			return archiveMailbox != null && (archiveMailbox.ArchiveState == ArchiveState.Local || archiveMailbox.ArchiveState == ArchiveState.HostedProvisioned);
		}

		private void SetDefaultFolderMapping(MailboxSession session)
		{
			DefaultFolderType[] array = (DefaultFolderType[])Enum.GetValues(typeof(DefaultFolderType));
			this.defaultFolderIds = new FolderId[array.Length];
			this.defaultFolderNames = new string[array.Length];
			int num = 0;
			Dictionary<DefaultFolderType, string> defaultFolderTypeToFolderNameMapForMailbox = IdConverter.GetDefaultFolderTypeToFolderNameMapForMailbox();
			DefaultFolderType[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				DefaultFolderType defaultFolderType = array2[i];
				if (defaultFolderType == DefaultFolderType.None)
				{
					this.defaultFolderNames[num] = Enum.GetName(typeof(DefaultFolderType), defaultFolderType);
					goto IL_7E;
				}
				if (defaultFolderTypeToFolderNameMapForMailbox.TryGetValue(defaultFolderType, out this.defaultFolderNames[num]))
				{
					goto Block_2;
				}
				IL_B8:
				i++;
				continue;
				Block_2:
				try
				{
					IL_7E:
					StoreObjectId defaultFolderId = session.GetDefaultFolderId(defaultFolderType);
					if (defaultFolderId == null)
					{
						this.defaultFolderIds[num] = null;
					}
					else
					{
						this.defaultFolderIds[num] = IdConverter.ConvertStoreFolderIdToFolderId(defaultFolderId, session);
					}
				}
				catch (InvalidOperationException)
				{
					this.defaultFolderIds[num] = null;
				}
				num++;
				goto IL_B8;
			}
		}

		private void UpdateMailboxQuotaLimits(MailboxSession mailboxSession)
		{
			mailboxSession.Mailbox.ForceReload(new PropertyDefinition[]
			{
				MailboxSchema.QuotaUsedExtended,
				MailboxSchema.QuotaProhibitSend,
				MailboxSchema.StorageQuotaLimit
			});
			this.quotaUsed = 0L;
			object obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended);
			if (!(obj is PropertyError))
			{
				this.quotaUsed = (long)obj;
			}
			this.quotaSend = 0L;
			obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.QuotaProhibitSend);
			if (!(obj is PropertyError))
			{
				this.quotaSend = (long)((int)obj) * 1024L;
			}
			this.quotaWarning = 0L;
			obj = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.StorageQuotaLimit);
			if (!(obj is PropertyError))
			{
				this.quotaWarning = (long)((int)obj) * 1024L;
			}
		}

		private static bool GetIsGallatin()
		{
			bool flag = false;
			return bool.TryParse(ConfigurationManager.AppSettings["IsGallatin"], out flag) && flag;
		}

		private const int DefaultMaxMessageSizeInKb = 5120;

		private const string IsGallatinConfigKey = "IsGallatin";

		private bool isExplicitLogon;

		private bool isExplicitLogonOthersMailbox;

		private bool isPublicLogon;

		private bool isPublicComputerSession;

		private bool canActAsOwner;

		private bool isBposUser;

		private bool isGallatin;

		private string userDisplayName;

		private string userEmailAddress;

		private string userLegacyExchangeDN;

		private string[] userProxyAddresses;

		private string userSipUri;

		private bool hasArchive;

		private string archiveDisplayName;

		private int maxMessageSizeInKb;

		private FolderId[] defaultFolderIds;

		private string[] defaultFolderNames;

		private string userCulture;

		private string sharePointUrl;

		private string sharePointTitle;

		private string playOnPhoneDialString;

		private bool isRequireProtectedPlayOnPhone;

		private bool isUMEnabled;

		private bool isUserCultureSpeechEnabled;

		private bool isUserCultureRightToLeft;

		private long quotaSend;

		private long quotaWarning;

		private long quotaUsed;

		private ConnectedAccountInfo[] connectedAccountInfos;

		private bool peopleConnectionsExist;

		private string helpUrl;

		private string userPrincipalName;
	}
}
