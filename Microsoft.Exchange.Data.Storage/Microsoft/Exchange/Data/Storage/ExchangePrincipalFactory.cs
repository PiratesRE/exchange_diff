using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangePrincipalFactory
	{
		public ExchangePrincipalFactory(IDirectoryAccessor directoryAccessor, IDatabaseLocationProvider databaseLocationProvider)
		{
			this.directoryAccessor = directoryAccessor;
			this.databaseLocationProvider = databaseLocationProvider;
		}

		public ExchangePrincipal FromADUser(ADUser user, string domainController = null)
		{
			Util.ThrowOnNullArgument(user, "user");
			return this.FromADUser(user.OrganizationId.ToADSessionSettings(), user, RemotingOptions.LocalConnectionsOnly, domainController);
		}

		public ExchangePrincipal FromADUser(ADUser user, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(user, "user");
			return this.FromADUser(new ADUserGenericWrapper(user), remotingOptions);
		}

		public ExchangePrincipal FromADUser(IGenericADUser user, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(user, "user");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			return this.InternalFromADUser(user, remotingOptions);
		}

		public ExchangePrincipal FromADUser(ADSessionSettings adSettings, ADUser user)
		{
			return this.FromADUser(adSettings, user, RemotingOptions.LocalConnectionsOnly, null);
		}

		public ExchangePrincipal FromADUser(ADSessionSettings adSettings, ADUser user, RemotingOptions remotingOptions, string domainController = null)
		{
			Util.ThrowOnNullArgument(user, "user");
			return this.FromADUser(adSettings, new ADUserGenericWrapper(user), remotingOptions, domainController);
		}

		public ExchangePrincipal FromADUser(ADSessionSettings adSettings, IGenericADUser user, RemotingOptions remotingOptions, string domainController = null)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			Util.ThrowOnNullArgument(user, "user");
			return this.InternalFromADUser(user, remotingOptions);
		}

		public ExchangePrincipal FromADUser(ADUser user, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(user, "user");
			return this.FromADUser(new ADUserGenericWrapper(user), databaseLocationInfo, remotingOptions);
		}

		public ExchangePrincipal FromADUser(IGenericADUser user, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			Util.ThrowOnNullArgument(user, "user");
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(user.MailboxGuid, user, out mdb);
			return this.InternalFromADUser(user, mdb, databaseLocationInfo, remotingOptions, asArchive, false, null);
		}

		public IExchangePrincipal FromADMailboxRecipient(IADMailboxRecipient mailbox, RemotingOptions remotingOptions)
		{
			return this.FromADMailboxRecipient(mailbox, null, remotingOptions);
		}

		public IExchangePrincipal FromADMailboxRecipient(IADMailboxRecipient mailbox, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(mailbox, "mailbox");
			IADUser iaduser = mailbox as IADUser;
			if (iaduser != null)
			{
				return this.InternalFromADUser(new ADUserGenericWrapper(iaduser), null, databaseLocationInfo, remotingOptions, false, false, null);
			}
			IADGroup iadgroup = mailbox as IADGroup;
			if (iadgroup != null)
			{
				return this.InternalFromADUser(new ADGroupGenericWrapper(iadgroup), null, databaseLocationInfo, remotingOptions, false, false, null);
			}
			throw new InvalidOperationException("ExchangePrincipal.FromADMailboxRecipient doesn't support type " + mailbox.GetType().Name);
		}

		public ExchangePrincipal FromWindowsIdentity(ADSessionSettings adSettings, WindowsIdentity windowsIdentity)
		{
			return this.FromWindowsIdentity(adSettings, windowsIdentity, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromWindowsIdentity(ADSessionSettings adSettings, WindowsIdentity windowsIdentity, RemotingOptions remotingOptions)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			Util.ThrowOnNullArgument(windowsIdentity, "windowsIdentity");
			return this.FromUserSid(adSettings, windowsIdentity.User, remotingOptions);
		}

		public ExchangePrincipal FromWindowsIdentity(IRecipientSession recipientSession, WindowsIdentity windowsIdentity)
		{
			return this.FromWindowsIdentity(recipientSession, windowsIdentity, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromWindowsIdentity(IRecipientSession recipientSession, WindowsIdentity windowsIdentity, RemotingOptions remotingOptions)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			Util.ThrowOnNullArgument(windowsIdentity, "windowsIdentity");
			return this.FromUserSid(recipientSession, windowsIdentity.User, remotingOptions);
		}

		public ExchangePrincipal FromUserSid(ADSessionSettings adSettings, SecurityIdentifier userSid)
		{
			return this.FromUserSid(adSettings, userSid, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromUserSid(ADSessionSettings adSettings, SecurityIdentifier userSid, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			return this.FromUserSid(adSettings.CreateRecipientSession(null), userSid, remotingOptions);
		}

		public ExchangePrincipal FromUserSid(IRecipientSession recipientSession, SecurityIdentifier userSid)
		{
			return this.FromUserSid(recipientSession, userSid, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromUserSid(IRecipientSession recipientSession, SecurityIdentifier userSid, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			Util.ThrowOnNullArgument(userSid, "userSid");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			this.CheckNoCrossPremiseAccess(remotingOptions);
			IGenericADUser genericADUser = this.directoryAccessor.FindBySid(recipientSession, userSid);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			return this.InternalFromADUser(genericADUser, remotingOptions);
		}

		public ExchangePrincipal FromProxyAddress(ADSessionSettings adSettings, string proxyAddress)
		{
			return this.FromProxyAddress(adSettings, proxyAddress, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromProxyAddress(ADSessionSettings adSettings, string proxyAddress, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			return this.FromProxyAddress(adSettings.CreateRecipientSession(null), proxyAddress, remotingOptions);
		}

		public ExchangePrincipal FromProxyAddress(IRecipientSession session, string proxyAddress)
		{
			return this.FromProxyAddress(session, proxyAddress, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromProxyAddress(IRecipientSession session, string proxyAddress, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(proxyAddress, "proxyAddress");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			if (proxyAddress.Length == 0)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			this.CheckNoCrossPremiseAccess(remotingOptions);
			ProxyAddress proxyAddress2 = ProxyAddress.Parse(proxyAddress);
			IGenericADUser genericADUser = this.directoryAccessor.FindByProxyAddress(session, proxyAddress2);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(genericADUser.MailboxGuid, genericADUser, out mdb);
			return this.InternalFromADUser(genericADUser, mdb, null, remotingOptions, asArchive, false, null);
		}

		public ExchangePrincipal FromLegacyDN(ADSessionSettings adSettings, string legacyDN)
		{
			return this.FromLegacyDN(adSettings, legacyDN, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromLegacyDN(ADSessionSettings adSettings, string legacyDN, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			return this.FromLegacyDN(adSettings.CreateRecipientSession(null), legacyDN, remotingOptions);
		}

		public ExchangePrincipal FromLegacyDN(IRecipientSession recipientSession, string legacyDN, RemotingOptions remotingOptions)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			Util.ThrowOnNullArgument(legacyDN, "legacyDN");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			if (legacyDN.Length == 0)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			Guid mbxGuid;
			legacyDN = this.TryToExtractArchiveOrAggregatedMailboxGuid(legacyDN, out mbxGuid);
			IGenericADUser genericADUser = this.directoryAccessor.FindByLegacyExchangeDn(recipientSession, legacyDN);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(mbxGuid, genericADUser, out mdb);
			Guid? aggregatedMailboxGuid = null;
			if (mbxGuid != Guid.Empty && genericADUser.ArchiveGuid != mbxGuid)
			{
				if (genericADUser.AggregatedMailboxGuids != null)
				{
					aggregatedMailboxGuid = (genericADUser.AggregatedMailboxGuids.Any((Guid mailbox) => mailbox == mbxGuid) ? new Guid?(mbxGuid) : null);
				}
				if (aggregatedMailboxGuid == null && genericADUser.MailboxLocations != null)
				{
					aggregatedMailboxGuid = (genericADUser.MailboxLocations.Any((IMailboxLocationInfo mailbox) => mailbox.MailboxGuid.Equals(mbxGuid)) ? new Guid?(mbxGuid) : null);
				}
			}
			ExchangePrincipal exchangePrincipal = this.InternalFromADUser(genericADUser, mdb, null, remotingOptions, asArchive, false, aggregatedMailboxGuid);
			if (mbxGuid != Guid.Empty && !exchangePrincipal.MailboxInfo.MailboxGuid.Equals(mbxGuid))
			{
				throw new ObjectNotFoundException(ServerStrings.AggregatedMailboxNotFound(mbxGuid.ToString()));
			}
			return exchangePrincipal;
		}

		public ExchangePrincipal FromLegacyDNByMiniRecipient(ADSessionSettings adSettings, string legacyDN, RemotingOptions remotingOptions, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			return this.FromLegacyDNByMiniRecipient(adSettings.CreateRecipientSession(null), legacyDN, remotingOptions, miniRecipientProperties, out miniRecipient);
		}

		public ExchangePrincipal FromLegacyDNByMiniRecipient(IRecipientSession recipientSession, string legacyDN, RemotingOptions remotingOptions, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			Util.ThrowOnNullArgument(legacyDN, "legacyDN");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			if (legacyDN.Length == 0)
			{
				throw new ArgumentException("legacyDN has zero length", "legacyDN");
			}
			Guid mbxGuid;
			legacyDN = this.TryToExtractArchiveOrAggregatedMailboxGuid(legacyDN, out mbxGuid);
			IGenericADUser genericADUser = this.directoryAccessor.FindMiniRecipientByProxyAddress(recipientSession, ProxyAddressPrefix.LegacyDN.GetProxyAddress(legacyDN, true), miniRecipientProperties, out miniRecipient);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(mbxGuid, genericADUser, out mdb);
			Guid? aggregatedMailboxGuid = null;
			if (genericADUser.AggregatedMailboxGuids != null)
			{
				aggregatedMailboxGuid = (genericADUser.AggregatedMailboxGuids.Any((Guid mailbox) => mailbox == mbxGuid) ? new Guid?(mbxGuid) : null);
			}
			IMailboxLocation mailboxLocation = new OnDemandMailboxLocation(() => new MailboxDatabaseLocation(this.databaseLocationProvider.GetLocationInfo(mdb.ObjectGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite)));
			ExchangePrincipal exchangePrincipal = this.InternalFromMiniRecipient(genericADUser, mdb, mailboxLocation, remotingOptions, asArchive, aggregatedMailboxGuid);
			if (mbxGuid != Guid.Empty && !exchangePrincipal.MailboxInfo.IsAggregated && !exchangePrincipal.MailboxInfo.IsArchive)
			{
				throw new ObjectNotFoundException(ServerStrings.AggregatedMailboxNotFound(mbxGuid.ToString()));
			}
			return exchangePrincipal;
		}

		public ExchangePrincipal FromADSystemMailbox(ADSessionSettings adSettings, ADSystemMailbox adSystemMailbox, Server server)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			Util.ThrowOnNullArgument(adSystemMailbox, "adSystemMailbox");
			Util.ThrowOnNullArgument(server, "server");
			if (!server.IsMailboxServer)
			{
				throw new ArgumentException("Needs to be a Mailbox server", "server");
			}
			return this.FromADSystemMailbox(adSettings.CreateRecipientSession(null), new ADSystemMailboxGenericWrapper(adSystemMailbox), server.Fqdn, server.ExchangeLegacyDN);
		}

		public ExchangePrincipal FromADSystemMailbox(IRecipientSession recipientSession, IGenericADUser adSystemMailbox, string serverFqdn, string serverLegacyDn)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			Util.ThrowOnNullArgument(adSystemMailbox, "adSystemMailbox");
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			ArgumentValidator.ThrowIfNullOrEmpty("serverLegacyDn", serverLegacyDn);
			if (adSystemMailbox.RecipientType != RecipientType.SystemMailbox)
			{
				throw new ArgumentException("User object doesn't represent SystemMailbox", "adSystemMailbox");
			}
			if (adSystemMailbox.MailboxDatabase == null || adSystemMailbox.MailboxDatabase.ObjectGuid == Guid.Empty)
			{
				throw new UserHasNoMailboxException();
			}
			return this.FromMailboxData(recipientSession.SessionSettings, Util.NullIf<string>(adSystemMailbox.DisplayName, string.Empty) ?? "Microsoft System Attendant", serverFqdn, serverLegacyDn, adSystemMailbox.LegacyDn, adSystemMailbox.MailboxGuid, adSystemMailbox.MailboxDatabase.ObjectGuid, adSystemMailbox.PrimarySmtpAddress.ToString(), adSystemMailbox.ObjectId, new List<CultureInfo>(), Array<Guid>.Empty, RecipientType.SystemMailbox, RemotingOptions.AllowCrossSite);
		}

		public ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, string domainController = null)
		{
			return this.FromMailboxGuid(adSettings, mailboxGuid, RemotingOptions.LocalConnectionsOnly, domainController);
		}

		public ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, RemotingOptions remotingOptions, string domainController = null)
		{
			return this.FromMailboxGuid(adSettings, mailboxGuid, Guid.Empty, remotingOptions, domainController, false);
		}

		public ExchangePrincipal FromMailboxGuid(ADSessionSettings adSettings, Guid mailboxGuid, Guid mdbGuid, RemotingOptions remotingOptions, string domainController = null, bool isContentIndexing = false)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			return this.FromMailboxGuid(adSettings.CreateRecipientSession(domainController), mailboxGuid, mdbGuid, remotingOptions, isContentIndexing);
		}

		public ExchangePrincipal FromMailboxGuid(IRecipientSession recipientSession, Guid mailboxGuid, Guid mdbGuid, RemotingOptions remotingOptions, bool isContentIndexing = false)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid-less mailboxes are not supported by this factory method", "mailboxGuid");
			}
			IGenericADUser genericADUser = this.directoryAccessor.FindByExchangeGuid(recipientSession, mailboxGuid, false);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(mailboxGuid, genericADUser, out mdb);
			if (mdbGuid != Guid.Empty)
			{
				mdb = new ADObjectId(mdbGuid);
			}
			return this.InternalFromADUser(genericADUser, mdb, null, remotingOptions, asArchive, isContentIndexing, new Guid?(mailboxGuid));
		}

		public ExchangePrincipal FromLocalServerMailboxGuid(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid)
		{
			return this.FromLocalServerMailboxGuid(adSettings, mdbGuid, mailboxGuid, false);
		}

		public ExchangePrincipal FromLocalServerMailboxGuid(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, bool isContentIndexing)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			Server localServer = this.directoryAccessor.GetLocalServer();
			if (!localServer.IsMailboxServer)
			{
				throw new InvalidOperationException("This method can only be called on a Mailbox server");
			}
			return this.FromLocalServerMailboxGuid(adSettings.CreateRecipientSession(null), mdbGuid, mailboxGuid, new DatabaseLocationInfo(localServer, false), isContentIndexing);
		}

		public ExchangePrincipal FromLocalServerMailboxGuid(IRecipientSession recipientSession, Guid mdbGuid, Guid mailboxGuid, DatabaseLocationInfo databaseLocationInfo)
		{
			return this.FromLocalServerMailboxGuid(recipientSession, mdbGuid, mailboxGuid, databaseLocationInfo, false);
		}

		public ExchangePrincipal FromLocalServerMailboxGuid(IRecipientSession recipientSession, Guid mdbGuid, Guid mailboxGuid, DatabaseLocationInfo databaseLocationInfo, bool isContentIndexing)
		{
			Util.ThrowOnNullArgument(recipientSession, "recipientSession");
			Util.ThrowOnNullArgument(databaseLocationInfo, "databaseLocationInfo");
			if (mdbGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be Empty", "mdbGuid");
			}
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid-less mailboxes are not supported by this factory method", "mailboxGuid");
			}
			IGenericADUser genericADUser = this.directoryAccessor.FindByExchangeGuid(recipientSession, mailboxGuid, true);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			if (genericADUser.RecipientType == RecipientType.SystemMailbox)
			{
				return this.FromADSystemMailbox(recipientSession, genericADUser, databaseLocationInfo.ServerFqdn, databaseLocationInfo.ServerLegacyDN);
			}
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(genericADUser.RecipientType, mailboxGuid, genericADUser.ArchiveGuid, new ADObjectId(mdbGuid), genericADUser.ArchiveDatabase, out mdb);
			return this.InternalFromADUser(genericADUser, mdb, databaseLocationInfo, RemotingOptions.LocalConnectionsOnly, asArchive, isContentIndexing, new Guid?(mailboxGuid));
		}

		public ExchangePrincipal FromDirectoryObjectId(IRecipientSession session, ADObjectId directoryEntry, RemotingOptions remoteOptions = RemotingOptions.LocalConnectionsOnly)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(directoryEntry, "directoryEntry");
			IGenericADUser genericADUser = this.directoryAccessor.FindByObjectId(session, directoryEntry);
			if (genericADUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			return this.InternalFromADUser(genericADUser, remoteOptions);
		}

		private ExchangePrincipal InternalFromADUser(IGenericADUser user, RemotingOptions remotingOptions)
		{
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(user.MailboxGuid, user, out mdb);
			return this.InternalFromADUser(user, mdb, null, remotingOptions, asArchive, false, null);
		}

		private ExchangePrincipal InternalFromADUser(IGenericADUser user, ADObjectId mdb, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions)
		{
			return this.InternalFromADUser(user, mdb, databaseLocationInfo, remotingOptions, false, false, null);
		}

		private ExchangePrincipal InternalFromADUser(IGenericADUser user, ADObjectId mdb, DatabaseLocationInfo databaseLocationInfo, RemotingOptions remotingOptions, bool asArchive, bool isContentIndexing = false, Guid? aggregatedMailboxGuid = null)
		{
			if (databaseLocationInfo == null && mdb != null)
			{
				databaseLocationInfo = this.databaseLocationProvider.GetLocationInfo(mdb.ObjectGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite);
			}
			return this.CreateExchangePrincipal(user, mdb, this.CreateMailboxLocation(databaseLocationInfo), remotingOptions, asArchive, aggregatedMailboxGuid, this.databaseLocationProvider, isContentIndexing);
		}

		public ExchangePrincipal FromMailboxData(ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck = false)
		{
			return this.FromMailboxData(mailboxLegacyDN, adSettings, mdbGuid, mailboxGuid, mailboxLegacyDN, primarySmtpAddress, preferredCultures, bypassRemoteCheck);
		}

		public ExchangePrincipal FromMailboxData(string displayName, ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck = false)
		{
			return this.FromMailboxData(displayName, adSettings, mdbGuid, mailboxGuid, mailboxLegacyDN, primarySmtpAddress, preferredCultures, bypassRemoteCheck, RecipientType.Invalid, RecipientTypeDetails.None);
		}

		public ExchangePrincipal FromMailboxData(string displayName, ADSessionSettings adSettings, Guid mdbGuid, Guid mailboxGuid, string mailboxLegacyDN, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, bool bypassRemoteCheck, RecipientType recipientType, RecipientTypeDetails recipientTypeDetails)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			Util.ThrowOnNullArgument(mailboxLegacyDN, "mailboxLegacyDN");
			Util.ThrowOnNullArgument(preferredCultures, "preferredCultures");
			if (mailboxLegacyDN.Length == 0)
			{
				throw new ArgumentException("Should not be empty", "mailboxLegacyDN");
			}
			if (mdbGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be Empty", "mdbGuid");
			}
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be Empty", "mailboxGuid");
			}
			return this.CreateExchangePrincipal(displayName, this.CreateMailboxLocation(this.databaseLocationProvider.GetLocationInfo(mdbGuid, false, false)), RemotingOptions.LocalConnectionsOnly, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, null, adSettings.CurrentOrganizationId, preferredCultures, bypassRemoteCheck, recipientType, recipientTypeDetails, null, false, null);
		}

		public ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId)
		{
			return this.FromAnyVersionMailboxData(displayName, mailboxGuid, mdbGuid, primarySmtpAddress, legacyExchangeDN, id, recipientType, masterAccountSid, organizationId, false);
		}

		public ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId, bool isArchive)
		{
			return this.FromAnyVersionMailboxData(displayName, mailboxGuid, mdbGuid, primarySmtpAddress, legacyExchangeDN, id, recipientType, masterAccountSid, organizationId, RemotingOptions.LocalConnectionsOnly, isArchive);
		}

		public ExchangePrincipal FromAnyVersionMailboxData(string displayName, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, string legacyExchangeDN, ADObjectId id, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId, RemotingOptions remotingOptions, bool isArchive)
		{
			EnumValidator.ThrowIfInvalid<RecipientType>(recipientType, "recipientType");
			try
			{
				DatabaseLocationInfo locationInfo = this.databaseLocationProvider.GetLocationInfo(mdbGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite);
				return this.CreateExchangePrincipal(displayName, this.CreateMailboxLocation(locationInfo), remotingOptions, legacyExchangeDN, mailboxGuid, mdbGuid, primarySmtpAddress, id, Array<CultureInfo>.Empty, recipientType, masterAccountSid, organizationId, isArchive);
			}
			catch (DatabaseNotFoundException)
			{
				ExTraceGlobals.StorageTracer.TraceError<Guid>(0L, "Database was not found for mailbox {0}.", mailboxGuid);
			}
			catch (UnableToFindServerForDatabaseException)
			{
				ExTraceGlobals.SessionTracer.TraceError<Guid>(0L, "Server was not found for Database {0}.", mdbGuid);
			}
			return null;
		}

		public ExchangePrincipal FromMailboxData(ADSessionSettings adSettings, string displayName, string serverFqdn, string serverLegacyDN, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ICollection<CultureInfo> preferredCultures, IEnumerable<Guid> aggregatedMailboxGuids)
		{
			Util.ThrowOnNullArgument(adSettings, "adSettings");
			return this.FromMailboxData(adSettings, displayName, serverFqdn, serverLegacyDN, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, null, preferredCultures, aggregatedMailboxGuids, RecipientType.Invalid, RemotingOptions.AllowCrossSite);
		}

		public ExchangePrincipal FromMailboxData(ADSessionSettings adSessionSettings, string displayName, string serverFqdn, string serverLegacyDN, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ADObjectId id, ICollection<CultureInfo> preferredCultures, IEnumerable<Guid> aggregatedMailboxGuids, RecipientType userRecipientType = RecipientType.Invalid, RemotingOptions remotingOptions = RemotingOptions.AllowCrossSite)
		{
			Util.ThrowOnNullArgument(displayName, "displayName");
			Util.ThrowOnNullArgument(serverFqdn, "serverFqdn");
			Util.ThrowOnNullArgument(serverLegacyDN, "serverLegacyDN");
			Util.ThrowOnNullArgument(preferredCultures, "preferredCultures");
			Util.ThrowOnNullArgument(aggregatedMailboxGuids, "aggregatedMailboxGuids");
			if (displayName.Length == 0)
			{
				throw new ArgumentException("displayName has zero length", "displayName");
			}
			if (serverFqdn.Length == 0)
			{
				throw new ArgumentException("serverFqdn has zero length", "serverFqdn");
			}
			if (serverLegacyDN.Length == 0)
			{
				throw new ArgumentException("serverLegacyDN has zero length", "serverLegacyDN");
			}
			if (mdbGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be Empty", "mdbGuid");
			}
			if (string.IsNullOrEmpty(mailboxLegacyDN) && mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException(ServerStrings.ExchangePrincipalFromMailboxDataError);
			}
			Guid? aggregatedMailboxGuid = aggregatedMailboxGuids.Any((Guid mailbox) => mailbox == mailboxGuid) ? new Guid?(mailboxGuid) : null;
			return this.CreateExchangePrincipal(displayName, this.CreateMailboxLocation(new DatabaseLocationInfo(serverFqdn, serverLegacyDN, null, null, false)), remotingOptions, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, id, (adSessionSettings != null) ? adSessionSettings.CurrentOrganizationId : null, preferredCultures, false, userRecipientType, aggregatedMailboxGuid);
		}

		public ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, ICollection<CultureInfo> preferredCultures)
		{
			return this.FromMailboxData(mailboxGuid, mdbGuid, null, preferredCultures, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures)
		{
			return this.FromMailboxData(mailboxGuid, mdbGuid, organizationId, preferredCultures, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions)
		{
			return this.FromMailboxData(mailboxGuid, mdbGuid, null, preferredCultures, remotingOptions);
		}

		public ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions)
		{
			return this.FromMailboxData(mailboxGuid, mdbGuid, organizationId, preferredCultures, remotingOptions, null);
		}

		public ExchangePrincipal FromMailboxData(Guid mailboxGuid, Guid mdbGuid, OrganizationId organizationId, ICollection<CultureInfo> preferredCultures, RemotingOptions remotingOptions, DatabaseLocationInfo databaseLocationInfo)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be empty", "mailboxGuid");
			}
			if (mdbGuid == Guid.Empty)
			{
				throw new ArgumentException("Should not be empty", "mdbGuid");
			}
			return this.CreateExchangePrincipal(mailboxGuid.ToString(), this.CreateMailboxLocation(databaseLocationInfo ?? this.databaseLocationProvider.GetLocationInfo(mdbGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite)), remotingOptions, string.Empty, mailboxGuid, mdbGuid, string.Empty, null, organizationId, preferredCultures, false, RecipientType.Invalid, null);
		}

		public ExchangePrincipal FromMiniRecipient(StorageMiniRecipient miniRecipient)
		{
			return this.FromMiniRecipient(miniRecipient, RemotingOptions.LocalConnectionsOnly);
		}

		public ExchangePrincipal FromMiniRecipient(StorageMiniRecipient miniRecipient, RemotingOptions remotingOptions)
		{
			return this.FromMiniRecipient(new MiniRecipientGenericWrapper(miniRecipient), remotingOptions);
		}

		public ExchangePrincipal FromMiniRecipient(IGenericADUser miniRecipient, RemotingOptions remotingOptions)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			Util.ThrowOnNullArgument(miniRecipient, "miniRecipient");
			ADObjectId mdb;
			bool asArchive = this.UpdateArchiveStatus(miniRecipient.MailboxGuid, miniRecipient, out mdb);
			return this.InternalFromMiniRecipient(miniRecipient, mdb, null, remotingOptions, asArchive, null);
		}

		private ExchangePrincipal InternalFromMiniRecipient(IGenericADUser adUser, ADObjectId mdb, IMailboxLocation mailboxLocation, RemotingOptions remotingOptions, bool asArchive, Guid? aggregatedMailboxGuid = null)
		{
			if (adUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			if (mdb == null)
			{
				mdb = adUser.MailboxDatabase;
			}
			if (mailboxLocation == null && mdb != null)
			{
				mailboxLocation = this.CreateMailboxLocation(this.databaseLocationProvider.GetLocationInfo(mdb.ObjectGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite));
			}
			return this.CreateExchangePrincipal(adUser, mdb, mailboxLocation, remotingOptions, asArchive, aggregatedMailboxGuid, this.databaseLocationProvider, false);
		}

		private ExchangePrincipal CreateExchangePrincipal(IGenericADUser user, ADObjectId mdb, IMailboxLocation mailboxLocation, RemotingOptions remotingOptions, bool asArchive, Guid? aggregatedMailboxGuid, IDatabaseLocationProvider databaseLocationProvider, bool isContentIndexing = false)
		{
			ExchangePrincipalBuilder exchangePrincipalBuilder = ((remotingOptions & RemotingOptions.AllowHybridAccess) == RemotingOptions.AllowHybridAccess) ? new RemoteUserMailboxPrincipalBuilder(user) : this.GetExchangePrincipalBuilder(user);
			exchangePrincipalBuilder.SetRemotingOptions(remotingOptions);
			exchangePrincipalBuilder.SetDatabaseLocationProvider(databaseLocationProvider);
			if (!mdb.IsNullOrEmpty())
			{
				exchangePrincipalBuilder.SetSelectedMailboxDatabase(mdb.ObjectGuid);
			}
			if (mailboxLocation != null)
			{
				exchangePrincipalBuilder.SetSelectedMailboxLocation(mailboxLocation);
			}
			if (asArchive)
			{
				exchangePrincipalBuilder.SelectArchiveMailbox();
			}
			exchangePrincipalBuilder.BypassRecipientTypeValidation(isContentIndexing);
			if (aggregatedMailboxGuid != null && aggregatedMailboxGuid != Guid.Empty)
			{
				exchangePrincipalBuilder.SelectMailbox(aggregatedMailboxGuid.Value);
			}
			return exchangePrincipalBuilder.Build();
		}

		private ExchangePrincipal CreateExchangePrincipal(string displayName, IMailboxLocation mailboxLocation, RemotingOptions remotingOptions, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ADObjectId id, OrganizationId orgId, IEnumerable<CultureInfo> preferredCultures, bool bypassRemoteCheck = false, RecipientType userRecipientType = RecipientType.Invalid, Guid? aggregatedMailboxGuid = null)
		{
			return this.CreateExchangePrincipal(displayName, mailboxLocation, remotingOptions, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, id, orgId, preferredCultures, bypassRemoteCheck, userRecipientType, RecipientTypeDetails.None, null, false, aggregatedMailboxGuid);
		}

		private ExchangePrincipal CreateExchangePrincipal(string displayName, IMailboxLocation mailboxLocation, RemotingOptions remotingOptions, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ADObjectId id, OrganizationId orgId, IEnumerable<CultureInfo> preferredCultures, bool bypassRemoteCheck, RecipientType recipientType, RecipientTypeDetails recipientTypeDetails, SecurityIdentifier masterAccountSid = null, bool isArchive = false, Guid? aggregatedMailboxGuid = null)
		{
			ADObjectId adobjectId = new ADObjectId(mdbGuid);
			IGenericADUser recipient = new GenericADUser
			{
				MailboxDatabase = (isArchive ? null : adobjectId),
				ArchiveDatabase = (isArchive ? adobjectId : null),
				LegacyDn = mailboxLegacyDN,
				OrganizationId = orgId,
				DisplayName = displayName,
				PrimarySmtpAddress = new SmtpAddress(primarySmtpAddress),
				MailboxGuid = (isArchive ? Guid.Empty : mailboxGuid),
				ArchiveGuid = (isArchive ? mailboxGuid : Guid.Empty),
				Languages = preferredCultures,
				RecipientType = recipientType,
				RecipientTypeDetails = recipientTypeDetails,
				ObjectId = id,
				MasterAccountSid = masterAccountSid,
				AggregatedMailboxGuids = ((aggregatedMailboxGuid != null) ? new Guid[]
				{
					aggregatedMailboxGuid.Value
				} : Array<Guid>.Empty)
			};
			ExchangePrincipalBuilder exchangePrincipalBuilder = this.GetExchangePrincipalBuilder(recipient);
			exchangePrincipalBuilder.SetRemotingOptions(remotingOptions);
			exchangePrincipalBuilder.BypassRecipientTypeValidation(true);
			if (mailboxLocation != null)
			{
				exchangePrincipalBuilder.SetSelectedMailboxLocation(mailboxLocation);
			}
			if (isArchive)
			{
				exchangePrincipalBuilder.SelectArchiveMailbox();
			}
			if (aggregatedMailboxGuid != null && aggregatedMailboxGuid != Guid.Empty)
			{
				exchangePrincipalBuilder.SelectMailbox(aggregatedMailboxGuid.Value);
			}
			return exchangePrincipalBuilder.Build();
		}

		private ExchangePrincipal CreateExchangePrincipal(string displayName, IMailboxLocation mailboxLocation, RemotingOptions remotingOptions, string mailboxLegacyDN, Guid mailboxGuid, Guid mdbGuid, string primarySmtpAddress, ADObjectId id, IEnumerable<CultureInfo> preferredCultures, RecipientType recipientType, SecurityIdentifier masterAccountSid, OrganizationId organizationId, bool isArchive = false)
		{
			return this.CreateExchangePrincipal(displayName, mailboxLocation, remotingOptions, mailboxLegacyDN, mailboxGuid, mdbGuid, primarySmtpAddress, id, organizationId, preferredCultures, false, recipientType, RecipientTypeDetails.None, masterAccountSid, isArchive, null);
		}

		private IMailboxLocation CreateMailboxLocation(DatabaseLocationInfo databaseLocationInfo)
		{
			IMailboxLocation result = null;
			if (databaseLocationInfo != null)
			{
				result = new MailboxDatabaseLocation(databaseLocationInfo);
			}
			return result;
		}

		private string TryToExtractArchiveOrAggregatedMailboxGuid(string legacyDN, out Guid mbxGuid)
		{
			mbxGuid = Guid.Empty;
			int num = legacyDN.LastIndexOf("/guid=", StringComparison.OrdinalIgnoreCase);
			if (num != -1 && num + 6 + 1 < legacyDN.Length)
			{
				string g = legacyDN.Substring(num + 6);
				try
				{
					mbxGuid = new Guid(g);
				}
				catch (FormatException)
				{
					return legacyDN;
				}
				catch (OverflowException)
				{
					return legacyDN;
				}
				return legacyDN.Remove(num);
			}
			return legacyDN;
		}

		private bool UpdateArchiveStatus(Guid mailboxToUse, IGenericADUser user, out ADObjectId databaseToUse)
		{
			databaseToUse = null;
			bool flag = this.UpdateArchiveStatus(user.RecipientType, mailboxToUse, user.ArchiveGuid, user.MailboxDatabase, user.ArchiveDatabase, out databaseToUse);
			if (!flag)
			{
				databaseToUse = user.MailboxDatabase;
				if (user.MailboxLocations != null)
				{
					foreach (IMailboxLocationInfo mailboxLocationInfo in user.MailboxLocations)
					{
						if (mailboxLocationInfo.MailboxGuid.Equals(mailboxToUse))
						{
							databaseToUse = mailboxLocationInfo.DatabaseLocation;
							break;
						}
					}
				}
			}
			return flag;
		}

		private bool UpdateArchiveStatus(RecipientType recipientType, Guid mailboxToUse, Guid archiveGuid, ADObjectId primaryDatabase, ADObjectId archiveDatabase, out ADObjectId databaseToUse)
		{
			databaseToUse = primaryDatabase;
			bool flag = this.IsArchiveMailUser(recipientType, archiveGuid, archiveDatabase);
			if (flag)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<Guid>(0L, "ExchangePrincipal::UpdateArchiveStatus. Recipient is an archive mail user. ArchiveGuid: {0}", archiveGuid);
			}
			else
			{
				flag = (mailboxToUse != Guid.Empty && archiveGuid != Guid.Empty && archiveGuid.Equals(mailboxToUse));
			}
			if (flag && archiveDatabase != null)
			{
				databaseToUse = archiveDatabase;
			}
			return flag;
		}

		private void CheckNoCrossPremiseAccess(RemotingOptions options)
		{
			if ((options & RemotingOptions.AllowCrossPremise) == RemotingOptions.AllowCrossPremise)
			{
				throw new NotSupportedException("RemotingOptions.AllowCrossPremise is not supported for this overload of ExchangePrincipal.From().");
			}
		}

		private bool IsArchiveMailUser(RecipientType recipientType, Guid archiveGuid, ADObjectId archiveDatabase)
		{
			return recipientType == RecipientType.MailUser && !archiveGuid.Equals(Guid.Empty) && archiveDatabase != null && !archiveDatabase.ObjectGuid.Equals(Guid.Empty);
		}

		private ExchangePrincipalBuilder GetExchangePrincipalBuilder(IGenericADUser recipient)
		{
			if (recipient is ADGroupGenericWrapper)
			{
				return new GroupPrincipalBuilder(recipient);
			}
			return new UserPrincipalBuilder(recipient);
		}

		private readonly IDirectoryAccessor directoryAccessor;

		private readonly IDatabaseLocationProvider databaseLocationProvider;
	}
}
