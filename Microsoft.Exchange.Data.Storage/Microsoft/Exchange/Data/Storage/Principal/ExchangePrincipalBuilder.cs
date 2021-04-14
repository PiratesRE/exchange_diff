using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ExchangePrincipalBuilder
	{
		public ExchangePrincipalBuilder(Func<IADUserFinder, IRecipientSession, IGenericADUser> findADUser)
		{
			ArgumentValidator.ThrowIfNull("findADUser", findADUser);
			this.findADUser = findADUser;
		}

		public ExchangePrincipalBuilder(IGenericADUser adUser)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			this.adUser = adUser;
		}

		public void SetADRecipientSession(IRecipientSession recipientSession)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			this.adRecipientSession = recipientSession;
		}

		public void SetADUserFinder(IADUserFinder adUserFinder)
		{
			ArgumentValidator.ThrowIfNull("adUserFinder", adUserFinder);
			this.adUserFinder = adUserFinder;
		}

		public void SetDatabaseLocationProvider(IDatabaseLocationProvider databaseLocationProvider)
		{
			ArgumentValidator.ThrowIfNull("databaseLocationProvider", databaseLocationProvider);
			this.databaseLocationProvider = databaseLocationProvider;
		}

		public void SetRemotingOptions(RemotingOptions remotingOptions)
		{
			EnumValidator<RemotingOptions>.ThrowIfInvalid(remotingOptions);
			this.remotingOptions = remotingOptions;
		}

		public void SelectArchiveMailbox()
		{
			this.isArchiveMailboxSelected = true;
		}

		public void SelectMailbox(Guid mailboxGuid)
		{
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			this.selectedMailboxGuid = new Guid?(mailboxGuid);
		}

		[Obsolete("Use SelectMailboxGuid")]
		public void SelectAggregatedMailbox(Guid aggregatedMailboxGuid)
		{
			ArgumentValidator.ThrowIfEmpty("aggregatedMailboxGuid", aggregatedMailboxGuid);
			this.selectedMailboxGuid = new Guid?(aggregatedMailboxGuid);
		}

		public void SetSelectedMailboxDatabase(Guid mailboxDatabaseGuid)
		{
			ArgumentValidator.ThrowIfEmpty("mailboxDatabaseGuid", mailboxDatabaseGuid);
			this.selectedMailboxDatabaseGuid = mailboxDatabaseGuid;
		}

		public void SetSelectedMailboxLocation(IMailboxLocation mailboxLocation)
		{
			ArgumentValidator.ThrowIfNull("mailboxLocation", mailboxLocation);
			this.selectedMailboxLocation = mailboxLocation;
		}

		public void UseOnDemandLocation(bool useOnDemandLocation)
		{
			this.useOnDemandLocation = useOnDemandLocation;
		}

		public void BypassRecipientTypeValidation(bool bypassRecipientTypeValidation)
		{
			this.bypassRecipientTypeValidation = bypassRecipientTypeValidation;
		}

		public ExchangePrincipal Build()
		{
			IGenericADUser aduser = this.GetADUser();
			ExchangePrincipal exchangePrincipal = this.BuildExchangePrincipal(aduser, this.isArchiveMailboxSelected, new ADObjectId(this.selectedMailboxDatabaseGuid), this.selectedMailboxGuid, new MailboxConfiguration(aduser), this.selectedMailboxLocation, (ADObjectId database) => this.BuildMailboxLocation(this.GetDatabaseLocationProvider(), database, this.remotingOptions), this.remotingOptions);
			if ((this.remotingOptions & RemotingOptions.AllowHybridAccess) != RemotingOptions.AllowHybridAccess && exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.IsRemote)
			{
				this.ThrowIfRemoteMailboxNotAllowed();
			}
			else if (!this.bypassRecipientTypeValidation && !this.IsRecipientTypeSupported(aduser))
			{
				throw new AdUserNotFoundException(ServerStrings.ADUserNotFound);
			}
			return exchangePrincipal;
		}

		protected abstract ExchangePrincipal BuildPrincipal(IGenericADUser recipient, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions);

		private IGenericADUser GetADUser()
		{
			if (this.adUser == null)
			{
				this.adUser = this.findADUser(this.GetADUserFinder(), this.GetADRecipientSession());
			}
			if (this.adUser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			return this.adUser;
		}

		private IADUserFinder GetADUserFinder()
		{
			if (this.adUserFinder != null)
			{
				return this.adUserFinder;
			}
			return this.GetDefaultADUserFinder();
		}

		private IADUserFinder GetDefaultADUserFinder()
		{
			if (ExchangePrincipalBuilder.DefaultADUserFinder == null)
			{
				ExchangePrincipalBuilder.DefaultADUserFinder = new DirectoryAccessor();
			}
			return ExchangePrincipalBuilder.DefaultADUserFinder;
		}

		private IDatabaseLocationProvider GetDatabaseLocationProvider()
		{
			if (this.databaseLocationProvider != null)
			{
				return this.databaseLocationProvider;
			}
			return this.GetDefaultDatabaseLocationProvider();
		}

		private IDatabaseLocationProvider GetDefaultDatabaseLocationProvider()
		{
			if (ExchangePrincipalBuilder.DefaultDatabaseLocationProvider == null)
			{
				ExchangePrincipalBuilder.DefaultDatabaseLocationProvider = new DatabaseLocationProvider();
			}
			return ExchangePrincipalBuilder.DefaultDatabaseLocationProvider;
		}

		protected abstract bool IsRecipientTypeSupported(IGenericADUser user);

		private ExchangePrincipal BuildExchangePrincipal(IGenericADUser user, bool isSelectedMailboxArchive, ADObjectId selectedDatabaseId, Guid? selectedMailboxGuid, MailboxConfiguration mailboxConfiguration, IMailboxLocation selectedMailboxLocation, Func<ADObjectId, IMailboxLocation> locationFactory, RemotingOptions remotingOptions)
		{
			List<IMailboxInfo> list = new List<IMailboxInfo>();
			if (user.ArchiveGuid != Guid.Empty)
			{
				IMailboxInfo mailboxInfo5 = this.BuildMailboxInfo(user.ArchiveGuid, user.ArchiveDatabase, isSelectedMailboxArchive, selectedDatabaseId, selectedMailboxLocation, user, mailboxConfiguration, locationFactory);
				if (mailboxInfo5 != null)
				{
					list.Add(mailboxInfo5);
				}
			}
			bool flag = false;
			if (selectedMailboxGuid != null && selectedMailboxGuid.Value != Guid.Empty)
			{
				if (user.MailboxLocations != null)
				{
					flag = user.MailboxLocations.Any((IMailboxLocationInfo mailboxInfo) => mailboxInfo.MailboxGuid == selectedMailboxGuid.Value);
				}
				if (!flag && user.AggregatedMailboxGuids != null)
				{
					flag = user.AggregatedMailboxGuids.Any((Guid mailbox) => mailbox == selectedMailboxGuid.Value);
				}
			}
			if (user.MailboxGuid != Guid.Empty)
			{
				bool isSelected = (selectedMailboxGuid != null && user.MailboxGuid.Equals(selectedMailboxGuid.Value)) || !flag || !isSelectedMailboxArchive;
				IMailboxInfo mailboxInfo2 = this.BuildMailboxInfo(user.MailboxGuid, user.MailboxDatabase, isSelected, selectedDatabaseId, selectedMailboxLocation, user, mailboxConfiguration, locationFactory);
				if (mailboxInfo2 != null)
				{
					list.Add(mailboxInfo2);
				}
			}
			if (user.MailboxLocations != null)
			{
				foreach (IMailboxLocationInfo mailboxLocationInfo in user.MailboxLocations)
				{
					IMailboxInfo mailboxInfo3 = this.BuildMailboxInfo(mailboxLocationInfo.MailboxGuid, mailboxLocationInfo.DatabaseLocation, mailboxLocationInfo.MailboxLocationType.Equals(MailboxLocationType.MainArchive) ? isSelectedMailboxArchive : (mailboxLocationInfo.MailboxGuid == selectedMailboxGuid), mailboxLocationInfo.DatabaseLocation, selectedMailboxLocation, user, mailboxConfiguration, locationFactory);
					if (mailboxInfo3 != null)
					{
						list.Add(mailboxInfo3);
					}
				}
			}
			if (user.AggregatedMailboxGuids != null)
			{
				foreach (Guid guid in user.AggregatedMailboxGuids)
				{
					IMailboxInfo mailboxInfo4 = this.BuildMailboxInfo(guid, user.MailboxDatabase, selectedMailboxGuid == guid, selectedDatabaseId, selectedMailboxLocation, user, mailboxConfiguration, locationFactory);
					if (mailboxInfo4 != null)
					{
						list.Add(mailboxInfo4);
					}
				}
			}
			Func<IMailboxInfo, bool> mailboxSelector;
			if (isSelectedMailboxArchive)
			{
				mailboxSelector = ((IMailboxInfo mailbox) => mailbox.MailboxType == MailboxLocationType.MainArchive);
			}
			else if (selectedMailboxGuid != null && selectedMailboxGuid != Guid.Empty)
			{
				mailboxSelector = ((IMailboxInfo mailbox) => mailbox.MailboxGuid == selectedMailboxGuid);
			}
			else
			{
				mailboxSelector = ((IMailboxInfo mailbox) => mailbox.MailboxGuid == user.MailboxGuid && mailbox.MailboxType == MailboxLocationType.Primary);
			}
			return this.BuildPrincipal(user, list, mailboxSelector, remotingOptions);
		}

		private IMailboxInfo BuildMailboxInfo(Guid mailboxGuid, ADObjectId mailboxDatabase, bool isSelected, ADObjectId selectedMailboxDatabase, IMailboxLocation selectedMailboxLocation, IGenericADUser adUser, IMailboxConfiguration configuration, Func<ADObjectId, IMailboxLocation> locationFactory)
		{
			ADObjectId adobjectId = mailboxDatabase;
			IMailboxLocation mailboxLocation = null;
			if (isSelected)
			{
				if (!selectedMailboxDatabase.IsNullOrEmpty())
				{
					adobjectId = selectedMailboxDatabase;
				}
				mailboxLocation = selectedMailboxLocation;
			}
			try
			{
				return new MailboxInfo(mailboxGuid, adobjectId, adUser, configuration, mailboxLocation ?? locationFactory(adobjectId));
			}
			catch (ObjectNotFoundException)
			{
			}
			return null;
		}

		private IMailboxLocation BuildMailboxLocation(IDatabaseLocationProvider databaseLocationProvider, ADObjectId databaseId, RemotingOptions remotingOptions)
		{
			if (databaseId.IsNullOrEmpty())
			{
				return MailboxDatabaseLocation.Unknown;
			}
			Func<IMailboxLocation> func = () => new MailboxDatabaseLocation(databaseLocationProvider.GetLocationInfo(databaseId.ObjectGuid, false, (remotingOptions & RemotingOptions.AllowCrossSite) == RemotingOptions.AllowCrossSite));
			if (this.useOnDemandLocation)
			{
				return new OnDemandMailboxLocation(func);
			}
			return func();
		}

		private IRecipientSession GetADRecipientSession()
		{
			if (this.adRecipientSession == null)
			{
				this.adRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 588, "GetADRecipientSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ExchangePrincipal\\ExchangePrincipalBuilder.cs");
			}
			return this.adRecipientSession;
		}

		private void ThrowIfRemoteMailboxNotAllowed()
		{
			if ((this.remotingOptions & RemotingOptions.AllowCrossPremise) != RemotingOptions.AllowCrossPremise)
			{
				throw new UserHasNoMailboxException();
			}
		}

		private static IADUserFinder DefaultADUserFinder;

		private static IDatabaseLocationProvider DefaultDatabaseLocationProvider;

		private IGenericADUser adUser;

		private IADUserFinder adUserFinder;

		private IDatabaseLocationProvider databaseLocationProvider;

		private IRecipientSession adRecipientSession;

		private Guid selectedMailboxDatabaseGuid;

		private IMailboxLocation selectedMailboxLocation;

		private RemotingOptions remotingOptions;

		private bool useOnDemandLocation = true;

		private bool bypassRecipientTypeValidation;

		private bool isArchiveMailboxSelected;

		private Guid? selectedMailboxGuid;

		private readonly Func<IADUserFinder, IRecipientSession, IGenericADUser> findADUser;
	}
}
