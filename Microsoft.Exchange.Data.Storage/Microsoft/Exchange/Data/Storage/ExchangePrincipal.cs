using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ExchangePrincipal : IExchangePrincipal
	{
		public ExchangePrincipal(IGenericADUser adUser, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			ArgumentValidator.ThrowIfNull("mailboxes", allMailboxes);
			ArgumentValidator.ThrowIfNull("mailboxSelector", mailboxSelector);
			EnumValidator<RemotingOptions>.ThrowIfInvalid(remotingOptions);
			this.MailboxInfo = allMailboxes.FirstOrDefault(mailboxSelector);
			if (this.MailboxInfo == null && (remotingOptions & RemotingOptions.AllowHybridAccess) != RemotingOptions.AllowHybridAccess)
			{
				throw new UserHasNoMailboxException();
			}
			this.AllMailboxes = allMailboxes;
			this.ObjectId = adUser.ObjectId;
			this.LegacyDn = adUser.LegacyDn;
			this.Alias = (adUser.Alias ?? string.Empty);
			this.DefaultPublicFolderMailbox = adUser.DefaultPublicFolderMailbox;
			this.Sid = adUser.Sid;
			this.MasterAccountSid = adUser.MasterAccountSid;
			this.SidHistory = adUser.SidHistory;
			this.Delegates = from delegateUser in adUser.GrantSendOnBehalfTo
			where delegateUser != null
			select delegateUser;
			this.PreferredCultures = ((adUser.Languages == null) ? Enumerable.Empty<CultureInfo>() : new PreferredCultures(adUser.Languages));
			this.RecipientType = adUser.RecipientType;
			this.RecipientTypeDetails = adUser.RecipientTypeDetails;
			this.IsResource = adUser.IsResource;
			this.ModernGroupType = adUser.ModernGroupType;
			this.PublicToGroupSids = adUser.PublicToGroupSids;
			this.ExternalDirectoryObjectId = adUser.ExternalDirectoryObjectId;
			this.AggregatedMailboxGuids = (adUser.AggregatedMailboxGuids ?? ((IEnumerable<Guid>)Array<Guid>.Empty));
			this.remotingOptions = remotingOptions;
		}

		protected ExchangePrincipal(ExchangePrincipal sourceExchangePrincipal)
		{
			this.ObjectId = sourceExchangePrincipal.ObjectId;
			this.LegacyDn = sourceExchangePrincipal.LegacyDn;
			this.Alias = sourceExchangePrincipal.Alias;
			this.DefaultPublicFolderMailbox = sourceExchangePrincipal.DefaultPublicFolderMailbox;
			this.Sid = sourceExchangePrincipal.Sid;
			this.MasterAccountSid = sourceExchangePrincipal.MasterAccountSid;
			this.SidHistory = sourceExchangePrincipal.SidHistory;
			this.Delegates = sourceExchangePrincipal.Delegates;
			this.PreferredCultures = sourceExchangePrincipal.PreferredCultures;
			this.RecipientType = sourceExchangePrincipal.RecipientType;
			this.RecipientTypeDetails = sourceExchangePrincipal.RecipientTypeDetails;
			this.IsResource = sourceExchangePrincipal.IsResource;
			this.ModernGroupType = sourceExchangePrincipal.ModernGroupType;
			this.PublicToGroupSids = sourceExchangePrincipal.PublicToGroupSids;
			this.ExternalDirectoryObjectId = sourceExchangePrincipal.ExternalDirectoryObjectId;
			this.AggregatedMailboxGuids = sourceExchangePrincipal.AggregatedMailboxGuids;
			this.MailboxInfo = sourceExchangePrincipal.MailboxInfo;
			this.AllMailboxes = sourceExchangePrincipal.AllMailboxes;
			this.remotingOptions = sourceExchangePrincipal.remotingOptions;
		}

		public string LegacyDn { get; private set; }

		public string Alias { get; private set; }

		public ADObjectId DefaultPublicFolderMailbox { get; private set; }

		public SecurityIdentifier Sid { get; private set; }

		public SecurityIdentifier MasterAccountSid { get; private set; }

		public IEnumerable<SecurityIdentifier> SidHistory { get; private set; }

		public IEnumerable<ADObjectId> Delegates { get; private set; }

		public IEnumerable<CultureInfo> PreferredCultures { get; private set; }

		public ADObjectId ObjectId { get; private set; }

		public RecipientType RecipientType { get; private set; }

		public RecipientTypeDetails RecipientTypeDetails { get; private set; }

		public bool? IsResource { get; private set; }

		public ModernGroupObjectType ModernGroupType { get; private set; }

		public IEnumerable<SecurityIdentifier> PublicToGroupSids { get; private set; }

		public string ExternalDirectoryObjectId { get; private set; }

		public IEnumerable<Guid> AggregatedMailboxGuids { get; private set; }

		public IMailboxInfo MailboxInfo { get; private set; }

		public IEnumerable<IMailboxInfo> AllMailboxes { get; private set; }

		public bool IsCrossSiteAccessAllowed
		{
			get
			{
				return (this.remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite;
			}
		}

		public virtual bool IsMailboxInfoRequired
		{
			get
			{
				return true;
			}
		}

		public override string ToString()
		{
			return string.Format("LegacyDn: {0}, RecipientType: {1}, RecipientTypeDetails: {2}, Selected Mailbox: {3}", new object[]
			{
				this.LegacyDn,
				this.RecipientType,
				this.RecipientTypeDetails,
				this.MailboxInfo
			});
		}

		public ExchangePrincipal WithDelegates(IEnumerable<ADObjectId> delegates)
		{
			ArgumentValidator.ThrowIfNull("delegates", delegates);
			ExchangePrincipal exchangePrincipal = this.Clone();
			exchangePrincipal.Delegates = delegates;
			return exchangePrincipal;
		}

		public ExchangePrincipal WithPreferredCultures(IEnumerable<CultureInfo> cultures)
		{
			ArgumentValidator.ThrowIfNull("cultures", cultures);
			ExchangePrincipal exchangePrincipal = this.Clone();
			exchangePrincipal.PreferredCultures = cultures;
			return exchangePrincipal;
		}

		public ExchangePrincipal WithSelectedMailbox(IMailboxInfo selectedMailboxInfo, RemotingOptions? remotingOptions)
		{
			remotingOptions = new RemotingOptions?(remotingOptions ?? this.remotingOptions);
			bool flag = (remotingOptions & RemotingOptions.AllowCrossPremise) == RemotingOptions.AllowCrossPremise;
			if (selectedMailboxInfo == null || (selectedMailboxInfo.IsRemote && !flag))
			{
				throw new UserHasNoMailboxException();
			}
			if (!this.AllMailboxes.Contains(selectedMailboxInfo))
			{
				throw new InvalidOperationException("Selected mailbox not found in all mailboxes collection");
			}
			ExchangePrincipal exchangePrincipal = this.Clone();
			exchangePrincipal.MailboxInfo = selectedMailboxInfo;
			exchangePrincipal.remotingOptions = remotingOptions.Value;
			return exchangePrincipal;
		}

		public abstract string PrincipalId { get; }

		protected abstract ExchangePrincipal Clone();

		private static IDatabaseLocationProvider DatabaseLocationProvider
		{
			get
			{
				if (ExchangePrincipal.databaseLocationProvider == null)
				{
					ExchangePrincipal.databaseLocationProvider = new DatabaseLocationProvider();
				}
				return ExchangePrincipal.databaseLocationProvider;
			}
		}

		private static IDirectoryAccessor DirectoryAccessor
		{
			get
			{
				if (ExchangePrincipal.directoryAccessor == null)
				{
					ExchangePrincipal.directoryAccessor = new DirectoryAccessor();
				}
				return ExchangePrincipal.directoryAccessor;
			}
		}

		private static ExchangePrincipalFactory ExchangePrincipalFactory
		{
			get
			{
				if (ExchangePrincipal.exchangePrincipalFactory == null)
				{
					ExchangePrincipal.exchangePrincipalFactory = new ExchangePrincipalFactory(ExchangePrincipal.DirectoryAccessor, ExchangePrincipal.DatabaseLocationProvider);
				}
				return ExchangePrincipal.exchangePrincipalFactory;
			}
		}

		public static ExchangePrincipal FromADUser(ADUser user, string domainController = null)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(user, domainController);
		}

		public static ExchangePrincipal FromADUser(ADUser user, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(user, remotingOptions);
		}

		public static ExchangePrincipal FromADUser(IGenericADUser user, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(user, remotingOptions);
		}

		public static ExchangePrincipal FromADUser(ADSessionSettings adSettings, ADUser user)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(adSettings, user);
		}

		public static ExchangePrincipal FromADUser(ADSessionSettings adSettings, ADUser user, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(adSettings, user, remotingOptions, null);
		}

		public static ExchangePrincipal FromADUser(ADSessionSettings adSettings, IGenericADUser user, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(adSettings, user, remotingOptions, null);
		}

		public static ExchangePrincipal FromADUser(ADUser user, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(user, databaseLocationInfo, remotingOptions);
		}

		public static ExchangePrincipal FromADUser(IGenericADUser user, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADUser(user, databaseLocationInfo, remotingOptions);
		}

		public static IExchangePrincipal FromADMailboxRecipient(IADMailboxRecipient mailbox, RemotingOptions remotingOptions = RemotingOptions.LocalConnectionsOnly)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADMailboxRecipient(mailbox, remotingOptions);
		}

		public static IExchangePrincipal FromADMailboxRecipient(IADMailboxRecipient user, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions = RemotingOptions.LocalConnectionsOnly)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADMailboxRecipient(user, databaseLocationInfo, remotingOptions);
		}

		public static ExchangePrincipal FromWindowsIdentity(ADSessionSettings adSettings, WindowsIdentity windowsIdentity)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromWindowsIdentity(adSettings, windowsIdentity);
		}

		public static ExchangePrincipal FromWindowsIdentity(ADSessionSettings adSettings, WindowsIdentity windowsIdentity, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromWindowsIdentity(adSettings, windowsIdentity, remotingOptions);
		}

		public static ExchangePrincipal FromWindowsIdentity(IRecipientSession recipientSession, WindowsIdentity windowsIdentity)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromWindowsIdentity(recipientSession, windowsIdentity);
		}

		public static ExchangePrincipal FromWindowsIdentity(IRecipientSession recipientSession, WindowsIdentity windowsIdentity, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromWindowsIdentity(recipientSession, windowsIdentity, remotingOptions);
		}

		public static ExchangePrincipal FromUserSid(ADSessionSettings adSettings, SecurityIdentifier userSid)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromUserSid(adSettings, userSid);
		}

		public static ExchangePrincipal FromUserSid(ADSessionSettings adSettings, SecurityIdentifier userSid, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromUserSid(adSettings, userSid, remotingOptions);
		}

		public static ExchangePrincipal FromUserSid(IRecipientSession recipientSession, SecurityIdentifier userSid)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromUserSid(recipientSession, userSid);
		}

		public static ExchangePrincipal FromUserSid(IRecipientSession recipientSession, SecurityIdentifier userSid, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromUserSid(recipientSession, userSid, remotingOptions);
		}

		public static ExchangePrincipal FromProxyAddress(ADSessionSettings adSettings, string proxyAddress)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromProxyAddress(adSettings, proxyAddress);
		}

		public static ExchangePrincipal FromProxyAddress(ADSessionSettings adSettings, string proxyAddress, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromProxyAddress(adSettings, proxyAddress, remotingOptions);
		}

		public static ExchangePrincipal FromProxyAddress(IRecipientSession session, string proxyAddress)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromProxyAddress(session, proxyAddress);
		}

		public static ExchangePrincipal FromProxyAddress(IRecipientSession session, string proxyAddress, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromProxyAddress(session, proxyAddress, remotingOptions);
		}

		public static ExchangePrincipal FromLegacyDN(ADSessionSettings adSettings, string legacyDN)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLegacyDN(adSettings, legacyDN);
		}

		public static ExchangePrincipal FromLegacyDN(ADSessionSettings adSettings, string legacyDN, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLegacyDN(adSettings, legacyDN, remotingOptions);
		}

		public static ExchangePrincipal FromLegacyDN(IRecipientSession recipientSession, string legacyDN, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLegacyDN(recipientSession, legacyDN, remotingOptions);
		}

		public static ExchangePrincipal FromLegacyDNByMiniRecipient(ADSessionSettings adSettings, string legacyDN, RemotingOptions remotingOptions, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLegacyDNByMiniRecipient(adSettings, legacyDN, remotingOptions, miniRecipientProperties, out miniRecipient);
		}

		public static ExchangePrincipal FromLegacyDNByMiniRecipient(IRecipientSession recipientSession, string legacyDN, RemotingOptions remotingOptions, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLegacyDNByMiniRecipient(recipientSession, legacyDN, remotingOptions, miniRecipientProperties, out miniRecipient);
		}

		public static ExchangePrincipal FromADSystemMailbox(ADSessionSettings adSettings, ADSystemMailbox adSystemMailbox, Server server)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADSystemMailbox(adSettings, adSystemMailbox, server);
		}

		public static ExchangePrincipal FromADSystemMailbox(IRecipientSession recipientSession, IGenericADUser adSystemMailbox, string serverFqdn, string serverLegacyDn)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromADSystemMailbox(recipientSession, adSystemMailbox, serverFqdn, serverLegacyDn);
		}

		public static ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, string domainController = null)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxGuid(adSettings, mailboxGuid, domainController);
		}

		public static ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, RemotingOptions remotingOptions, string domainController = null)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxGuid(adSettings, mailboxGuid, remotingOptions, domainController);
		}

		public static ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, Guid mdbGuid, RemotingOptions remotingOptions, string domainController = null, bool isContentIndexing = false)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxGuid(adSettings, mailboxGuid, mdbGuid, remotingOptions, domainController, isContentIndexing);
		}

		public static ExchangePrincipal FromMailboxGuid(IRecipientSession recipientSession, Guid mailboxGuid, Guid mdbGuid, RemotingOptions remotingOptions, bool isContentIndexing = false)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxGuid(recipientSession, mailboxGuid, mdbGuid, remotingOptions, isContentIndexing);
		}

		public static ExchangePrincipal FromLocalServerMailboxGuid(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLocalServerMailboxGuid(adSettings, mdbGuid, mailboxGuid);
		}

		public static ExchangePrincipal FromLocalServerMailboxGuid(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, bool isContentIndexing)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLocalServerMailboxGuid(adSettings, mdbGuid, mailboxGuid, isContentIndexing);
		}

		public static ExchangePrincipal FromLocalServerMailboxGuid(IRecipientSession recipientSession, Guid mdbGuid, Guid mailboxGuid, DatabaseLocationInfo databaseLocationInfo)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromLocalServerMailboxGuid(recipientSession, mdbGuid, mailboxGuid, databaseLocationInfo);
		}

		public static ExchangePrincipal FromDirectoryObjectId(IRecipientSession session, ADObjectId directoryEntry, RemotingOptions remoteOptions = RemotingOptions.LocalConnectionsOnly)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromDirectoryObjectId(session, directoryEntry, remoteOptions);
		}

		public static ExchangePrincipal FromMailboxData(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck = false)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(adSettings, mdbGuid, mailboxGuid, mailboxLegacyDN, primarySmtpAddress, preferredCultures, bypassRemoteCheck);
		}

		public static ExchangePrincipal FromMailboxData(string displayName, ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck = false)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(displayName, adSettings, mdbGuid, mailboxGuid, mailboxLegacyDN, primarySmtpAddress, preferredCultures, bypassRemoteCheck);
		}

		public static ExchangePrincipal FromMailboxData(string displayName, ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck, RecipientType recipientType, RecipientTypeDetails recipientTypeDetails)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(displayName, adSettings, mdbGuid, mailboxGuid, mailboxLegacyDN, primarySmtpAddress, preferredCultures, bypassRemoteCheck, recipientType, recipientTypeDetails);
		}

		public static ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromAnyVersionMailboxData(displayName, mailboxGuid, mdbGuid, primarySmtpAddress, legacyExchangeDN, id, recipientType, masterAccountSid, organizationId);
		}

		public static ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId, bool isArchive)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromAnyVersionMailboxData(displayName, mailboxGuid, mdbGuid, primarySmtpAddress, legacyExchangeDN, id, recipientType, masterAccountSid, organizationId, isArchive);
		}

		public static ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId, RemotingOptions remotingOptions, bool isArchive)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromAnyVersionMailboxData(displayName, mailboxGuid, mdbGuid, primarySmtpAddress, legacyExchangeDN, id, recipientType, masterAccountSid, organizationId, remotingOptions, isArchive);
		}

		public static ExchangePrincipal FromMailboxData(ADSessionSettings adSettings, string displayName, string serverFqdn, string serverLegacyDN, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, IEnumerable<Guid> aggregatedMailboxGuids)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(adSettings, displayName, serverFqdn, serverLegacyDN, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, preferredCultures, aggregatedMailboxGuids);
		}

		public static ExchangePrincipal FromMailboxData(ADSessionSettings adSessionSettings, string displayName, string serverFqdn, string serverLegacyDN, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ADObjectId id, ICollection<CultureInfo> preferredCultures, IEnumerable<Guid> aggregatedMailboxGuids, RecipientType userRecipientType = RecipientType.Invalid, RemotingOptions remotingOptions = RemotingOptions.AllowCrossSite)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(adSessionSettings, displayName, serverFqdn, serverLegacyDN, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, id, preferredCultures, aggregatedMailboxGuids, userRecipientType, remotingOptions);
		}

		public static ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, ICollection<CultureInfo> preferredCultures)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(mailboxGuid, mdbGuid, preferredCultures);
		}

		public static ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(mailboxGuid, mdbGuid, organizationId, preferredCultures);
		}

		public static ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(mailboxGuid, mdbGuid, preferredCultures, remotingOptions);
		}

		public static ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(mailboxGuid, mdbGuid, organizationId, preferredCultures, remotingOptions);
		}

		public static ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions, DatabaseLocationInfo databaseLocationInfo)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMailboxData(mailboxGuid, mdbGuid, organizationId, preferredCultures, remotingOptions, databaseLocationInfo);
		}

		public static ExchangePrincipal FromMiniRecipient(StorageMiniRecipient miniRecipient)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMiniRecipient(miniRecipient);
		}

		public static ExchangePrincipal FromMiniRecipient(StorageMiniRecipient miniRecipient, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMiniRecipient(miniRecipient, remotingOptions);
		}

		public static ExchangePrincipal FromMiniRecipient(IGenericADUser miniRecipient, RemotingOptions remotingOptions)
		{
			return ExchangePrincipal.ExchangePrincipalFactory.FromMiniRecipient(miniRecipient, remotingOptions);
		}

		public static void SetDatabaseLocationProviderForTest(IDatabaseLocationProvider databaseLocationProvider)
		{
			ExchangePrincipal.databaseLocationProvider = databaseLocationProvider;
			ExchangePrincipal.SetFactory(null);
		}

		public static void SetDirectoryAccessorForTest(IDirectoryAccessor directoryAccessor)
		{
			ExchangePrincipal.directoryAccessor = directoryAccessor;
			ExchangePrincipal.SetFactory(null);
		}

		internal static void SetFactory(ExchangePrincipalFactory exchangePrincipalFactory)
		{
			ExchangePrincipal.exchangePrincipalFactory = exchangePrincipalFactory;
		}

		private RemotingOptions remotingOptions;

		private static IDatabaseLocationProvider databaseLocationProvider;

		private static IDirectoryAccessor directoryAccessor;

		private static ExchangePrincipalFactory exchangePrincipalFactory;
	}
}
