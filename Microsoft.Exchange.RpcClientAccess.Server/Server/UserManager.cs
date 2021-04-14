using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class UserManager : BaseObject
	{
		internal UserManager() : this(new DatabaseLocationProvider(), new DatabaseInfoCache(DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 82, ".ctor", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\Server\\UserManager.cs"), ServiceConfiguration.Schema.ADUserDataCacheTimeout.DefaultValue))
		{
		}

		internal UserManager(IDatabaseLocationProvider databaseLocationProvider, LazyLookupTimeoutCache<Guid, DatabaseInfo> databaseInfoCache)
		{
			this.databaseLocationProvider = databaseLocationProvider;
			this.databaseInfoCache = databaseInfoCache;
		}

		private IDatabaseLocationProvider DatabaseLocationProvider
		{
			get
			{
				return this.databaseLocationProvider;
			}
		}

		public ExchangePrincipal FindExchangePrincipal(string legacyDN, string userDomain, out MiniRecipient miniRecipient)
		{
			ExchangePrincipal result = null;
			StorageMiniRecipient storageMiniRecipient = null;
			try
			{
				if (!string.IsNullOrEmpty(userDomain))
				{
					result = ExchangePrincipal.FromLegacyDNByMiniRecipient(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(userDomain), legacyDN, RemotingOptions.AllowCrossSite, UserManager.miniRecipientProperties, out storageMiniRecipient);
				}
				else
				{
					result = ExchangePrincipal.FromLegacyDNByMiniRecipient(ADSessionSettings.FromRootOrgScopeSet(), legacyDN, RemotingOptions.AllowCrossSite, UserManager.miniRecipientProperties, out storageMiniRecipient);
				}
			}
			finally
			{
				miniRecipient = storageMiniRecipient;
			}
			return result;
		}

		public ExchangePrincipal FindExchangePrincipal(string legacyDN, string userDomain)
		{
			MiniRecipient miniRecipient;
			return this.FindExchangePrincipal(legacyDN, userDomain, out miniRecipient);
		}

		public int GetActiveUserCount()
		{
			List<UserManager.User> list = null;
			int num = 0;
			try
			{
				this.userListLock.EnterReadLock();
				foreach (UserManager.User user in this.userList.Values)
				{
					if (user.LastAccessTimestamp == this.lastRetrievalTimestamp)
					{
						num++;
					}
					if (user.CanRemove)
					{
						if (list == null)
						{
							list = new List<UserManager.User>();
						}
						list.Add(user);
					}
				}
				this.lastRetrievalTimestamp++;
			}
			finally
			{
				try
				{
					this.userListLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (list != null && list.Count > 0)
			{
				try
				{
					this.userListLock.EnterWriteLock();
					foreach (UserManager.User user2 in list)
					{
						if (user2.CanRemove)
						{
							this.userList.Remove(user2);
						}
					}
				}
				finally
				{
					try
					{
						this.userListLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			return num;
		}

		public IUser Get(SecurityIdentifier authenticatedUserSid, string actAsLegacyDN, string userDomain)
		{
			UserManager.User user = new UserManager.User(this, authenticatedUserSid, actAsLegacyDN, userDomain);
			bool flag = false;
			UserManager.User user2;
			if (!this.TryGetExistingUser(user, out user2, out flag))
			{
				try
				{
					this.userListLock.EnterUpgradeableReadLock();
					if (!this.TryGetExistingUser(user, out user2, out flag))
					{
						try
						{
							this.userListLock.EnterWriteLock();
							this.userList.Add(user, user);
							user.AddReference();
							flag = true;
						}
						finally
						{
							try
							{
								this.userListLock.ExitWriteLock();
							}
							catch (SynchronizationLockException)
							{
							}
						}
					}
				}
				finally
				{
					try
					{
						this.userListLock.ExitUpgradeableReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			if (user2 != null)
			{
				user = user2;
			}
			bool flag2 = false;
			IUser result;
			try
			{
				if (flag)
				{
					RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.UserCount.Increment();
				}
				user.UpdatePrincipalCacheWrapped(true);
				flag2 = true;
				result = user;
			}
			finally
			{
				if (!flag2)
				{
					user.Release();
				}
			}
			return result;
		}

		internal bool CheckAccess(ClientSecurityContext clientSecurityContext, MiniRecipient accessingMiniRecipient)
		{
			bool result;
			try
			{
				if (accessingMiniRecipient.ExchangeSecurityDescriptor != null && accessingMiniRecipient.Database != null)
				{
					RawSecurityDescriptor rawSecurityDescriptor = this.GetDatabaseInfo(accessingMiniRecipient.Database.ObjectGuid).InheritMailboxSecurityDescriptor(accessingMiniRecipient.ExchangeSecurityDescriptor);
					int grantedAccess = clientSecurityContext.GetGrantedAccess(rawSecurityDescriptor, accessingMiniRecipient.Sid, AccessMask.CreateChild);
					if ((long)grantedAccess == 1L)
					{
						ExTraceGlobals.AccessControlTracer.TraceInformation<string, ClientSecurityContext>(0, Activity.TraceId, "Granted: mailbox security descriptor for mailbox {0} grants owner access to {1}", accessingMiniRecipient.LegacyExchangeDN, clientSecurityContext);
						result = true;
					}
					else
					{
						if (ExTraceGlobals.AccessControlTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.AccessControlTracer.TraceError<string, ClientSecurityContext, int>(0, Activity.TraceId, "Denied: mailbox security descriptor for mailbox {0} denies owner access to {1}. Maximum allowed access: {2:X8}", accessingMiniRecipient.LegacyExchangeDN, clientSecurityContext, clientSecurityContext.GetGrantedAccess(rawSecurityDescriptor, accessingMiniRecipient.Sid, AccessMask.MaximumAllowed));
						}
						result = false;
					}
				}
				else
				{
					ExTraceGlobals.AccessControlTracer.TraceError<RecipientType, string>(0, Activity.TraceId, "Denied: mailbox security descriptor for {0} {1} is not set", accessingMiniRecipient.RecipientType, accessingMiniRecipient.LegacyExchangeDN);
					result = false;
				}
			}
			catch (Win32Exception arg)
			{
				ExTraceGlobals.AccessControlTracer.TraceError<ClientSecurityContext, Win32Exception>(0, Activity.TraceId, "Denied: access check failed on ClientSecurityContext {0} with {1}.", clientSecurityContext, arg);
				result = false;
			}
			return result;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UserManager>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.databaseInfoCache);
			base.InternalDispose();
		}

		private DatabaseInfo GetDatabaseInfo(Guid databaseGuid)
		{
			return this.databaseInfoCache.Get(databaseGuid);
		}

		private void OnUserReferenceReleased(int referencesLeft)
		{
			if (referencesLeft <= 0)
			{
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.UserCount.Decrement();
			}
		}

		private bool TryGetExistingUser(UserManager.User user, out UserManager.User existingUser, out bool isUserActivated)
		{
			existingUser = null;
			isUserActivated = false;
			bool result;
			try
			{
				this.userListLock.EnterReadLock();
				if (this.userList.TryGetValue(user, out existingUser))
				{
					int num = existingUser.AddReference();
					isUserActivated = (num == 1);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				try
				{
					this.userListLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		private static readonly PropertyDefinition[] miniRecipientProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.MAPIBlockOutlookVersions,
			ADRecipientSchema.MAPIBlockOutlookRpcHttp,
			ADRecipientSchema.MAPIEnabled,
			ADUserSchema.UserAccountControl
		}.Union(ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>().AllProperties).ToArray<PropertyDefinition>();

		private readonly Dictionary<UserManager.User, UserManager.User> userList = new Dictionary<UserManager.User, UserManager.User>();

		private readonly ReaderWriterLockSlim userListLock = new ReaderWriterLockSlim();

		private readonly LazyLookupTimeoutCache<Guid, DatabaseInfo> databaseInfoCache;

		private readonly IDatabaseLocationProvider databaseLocationProvider;

		private int lastRetrievalTimestamp = 1;

		internal sealed class User : IUser, WatsonHelper.IProvideWatsonReportData, IEquatable<UserManager.User>
		{
			internal User(UserManager userManager, SecurityIdentifier authenticatedUserSid, string authenticatedUserLegacyDN, string domain)
			{
				this.userManager = userManager;
				this.AuthenticatedUserSid = authenticatedUserSid;
				this.userDn = authenticatedUserLegacyDN;
				this.domain = domain;
				if (this.domain == null)
				{
					this.domain = string.Empty;
				}
				this.exchangeOrganizationInfo = ExOrgInfoFlags.UseAutoDiscoverForPublicFolderConfiguration;
				this.blockedOutlookVersions = new MapiVersionRanges(null);
				this.mapiBlockOutlookRpcHttp = false;
				this.mapiEnabled = true;
				this.mapiCachedModeRequired = false;
			}

			public string LegacyDistinguishedName
			{
				get
				{
					return this.userDn;
				}
			}

			public string Domain
			{
				get
				{
					return this.domain;
				}
			}

			public string DisplayName
			{
				get
				{
					if (this.accessingPrincipal == null)
					{
						return string.Empty;
					}
					return this.accessingPrincipal.MailboxInfo.DisplayName;
				}
			}

			public OrganizationId OrganizationId
			{
				get
				{
					if (this.accessingPrincipal != null)
					{
						return this.accessingPrincipal.MailboxInfo.OrganizationId;
					}
					if (this.miniRecipient != null)
					{
						return this.miniRecipient.OrganizationId;
					}
					return null;
				}
			}

			public SecurityIdentifier MasterAccountSid
			{
				get
				{
					if (this.IsConnectAsValidDisabledUser())
					{
						return this.miniRecipient.MasterAccountSid;
					}
					return null;
				}
			}

			public SecurityIdentifier ConnectAsSid
			{
				get
				{
					if (this.accessingPrincipal != null)
					{
						return this.accessingPrincipal.Sid;
					}
					if (this.miniRecipient != null)
					{
						return this.miniRecipient.Sid;
					}
					return null;
				}
			}

			public ExOrgInfoFlags ExchangeOrganizationInfo
			{
				get
				{
					return this.exchangeOrganizationInfo;
				}
			}

			public MapiVersionRanges MapiBlockOutlookVersions
			{
				get
				{
					return this.blockedOutlookVersions;
				}
			}

			public bool MapiBlockOutlookRpcHttp
			{
				get
				{
					return this.mapiBlockOutlookRpcHttp;
				}
			}

			public bool MapiEnabled
			{
				get
				{
					return this.mapiEnabled;
				}
			}

			public bool MapiCachedModeRequired
			{
				get
				{
					return this.mapiCachedModeRequired;
				}
			}

			public MiniRecipient MiniRecipient
			{
				get
				{
					if (this.miniRecipient == null)
					{
						try
						{
							this.UpdatePrincipalCacheIfNeeded();
						}
						catch (UserHasNoMailboxException)
						{
						}
					}
					return this.miniRecipient;
				}
			}

			public bool IsFederatedSystemAttendant
			{
				get
				{
					return LegacyDnHelper.IsFederatedSystemAttendant(this.userDn);
				}
			}

			private bool CheckSecurityContext(IExchangePrincipal mailboxExchangePrincipal, ClientSecurityContext securityContext)
			{
				ArgumentValidator.ThrowIfNull("securityContext", securityContext);
				bool flag = (mailboxExchangePrincipal.Sid != null && mailboxExchangePrincipal.Sid.Equals(securityContext.UserSid)) || (mailboxExchangePrincipal.MasterAccountSid != null && mailboxExchangePrincipal.MasterAccountSid.Equals(securityContext.UserSid));
				return flag || (mailboxExchangePrincipal.SidHistory != null && mailboxExchangePrincipal.SidHistory.Any((SecurityIdentifier sid) => sid.Equals(securityContext.UserSid)));
			}

			internal int LastAccessTimestamp { get; private set; }

			internal bool CanRemove
			{
				get
				{
					return this.referenceCount == 0 && !this.HasImportantInformation;
				}
			}

			private ExDateTime BackoffConnectUntil
			{
				get
				{
					ExDateTime result;
					lock (this.budgetLock)
					{
						result = this.backoffConnectUntil;
					}
					return result;
				}
			}

			private SecurityIdentifier AuthenticatedUserSid { get; set; }

			private bool HasImportantInformation
			{
				get
				{
					return this.BackoffConnectUntil > ExDateTime.UtcNow;
				}
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as UserManager.User);
			}

			public override int GetHashCode()
			{
				return this.AuthenticatedUserSid.GetHashCode() ^ this.userDn.GetHashCode() ^ this.domain.GetHashCode();
			}

			public bool Equals(UserManager.User other)
			{
				return other != null && this.AuthenticatedUserSid.Equals(other.AuthenticatedUserSid) && LegacyDN.StringComparer.Equals(this.userDn, other.userDn) && string.Equals(this.domain, other.domain, StringComparison.OrdinalIgnoreCase);
			}

			public void CheckCanConnect()
			{
				if (ExDateTime.UtcNow >= this.BackoffConnectUntil)
				{
					this.attemptsLeftBeforeBackoffTakesEffect = 10;
					this.isRepeatingBackoff = false;
					return;
				}
				if (this.attemptsLeftBeforeBackoffTakesEffect <= 0)
				{
					ExTraceGlobals.ClientThrottledTracer.TraceWarning<string>(Activity.TraceId, "Connect for user '{0}' has been backed off", this.LegacyDistinguishedName);
					ClientBackoffException ex = new ClientBackoffException("Client connect is backed off", this.backoffConnectReason);
					ex.IsRepeatingBackoff = this.isRepeatingBackoff;
					this.isRepeatingBackoff = true;
					throw ex;
				}
				ExTraceGlobals.ClientThrottledTracer.TraceDebug<string, int>(Activity.TraceId, "Connect for user '{0}' is going to be backed off after {1} more attempts.", this.LegacyDistinguishedName, this.attemptsLeftBeforeBackoffTakesEffect);
			}

			public void BackoffConnect(Exception reason)
			{
				this.BackoffConnect(UserManager.User.defaultBackoffPeriod, reason);
			}

			public void InvalidatePrincipalCache()
			{
				lock (this.accessingPrincipalUpdateLock)
				{
					this.accessingPrincipalExpirationTime = ExDateTime.MinValue;
				}
				ExTraceGlobals.ConnectRpcTracer.TraceDebug<string>(0, (long)this.GetHashCode(), "Invalidated cached ExchangePrincipal for '{0}'", this.userDn);
			}

			public void RegisterActivity()
			{
				this.LastAccessTimestamp = this.userManager.lastRetrievalTimestamp;
			}

			public void Release()
			{
				this.userManager.OnUserReferenceReleased(this.RemoveReference());
			}

			public void UpdatePrincipalCacheWrapped(bool ignoreCrossForestMailboxErrors)
			{
				try
				{
					this.UpdatePrincipalCacheIfNeeded();
				}
				catch (UserHasNoMailboxException)
				{
					if (!ignoreCrossForestMailboxErrors)
					{
						throw;
					}
					ExTraceGlobals.ConnectRpcTracer.TraceInformation<string>(0, Activity.TraceId, "Failed to retrieve ExchangePrincipal for '{0}': user has no mailbox in this forest", this.LegacyDistinguishedName);
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new UnknownUserException(string.Format("Unable to map userDn '{0}' to exchangePrincipal", this.LegacyDistinguishedName), innerException);
				}
				catch (StoragePermanentException innerException2)
				{
					throw new LoginFailureException("Unable to access AD", innerException2);
				}
				catch (StorageTransientException innerException3)
				{
					throw new LoginFailureException("Unable to access AD", innerException3);
				}
			}

			private void UpdatePrincipalCacheIfNeeded()
			{
				if (LegacyDnHelper.IsFederatedSystemAttendant(this.userDn))
				{
					return;
				}
				lock (this.accessingPrincipalUpdateLock)
				{
					if (this.accessingPrincipalExpirationTime < ExDateTime.UtcNow)
					{
						this.accessingPrincipalExpirationTime = ExDateTime.UtcNow + Configuration.ServiceConfiguration.ADUserDataCacheTimeout;
						this.accessingPrincipalHasNoMailbox = false;
						MiniRecipient miniRecipient = null;
						try
						{
							this.accessingPrincipal = this.userManager.FindExchangePrincipal(this.userDn, this.domain, out miniRecipient);
							this.miniRecipient = miniRecipient;
						}
						catch (UserHasNoMailboxException)
						{
							this.miniRecipient = miniRecipient;
							this.accessingPrincipalHasNoMailbox = true;
							throw;
						}
						try
						{
							this.blockedOutlookVersions = new MapiVersionRanges((string)this.miniRecipient[ADRecipientSchema.MAPIBlockOutlookVersions]);
						}
						catch (FormatException innerException)
						{
							throw new ClientVersionException(string.Format("Version specification should have 3 parts, MAPIBlockOutlookVersions:{0}", this.miniRecipient[ADRecipientSchema.MAPIBlockOutlookVersions]), innerException);
						}
						catch (ArgumentOutOfRangeException innerException2)
						{
							throw new ClientVersionException(string.Format("Version number part should be between 0 and 65535, MAPIBlockOutlookVersions:{0}", this.miniRecipient[ADRecipientSchema.MAPIBlockOutlookVersions]), innerException2);
						}
						this.mapiBlockOutlookRpcHttp = (bool)this.miniRecipient[ADRecipientSchema.MAPIBlockOutlookRpcHttp];
						this.mapiEnabled = (bool)this.miniRecipient[ADRecipientSchema.MAPIEnabled];
						this.mapiCachedModeRequired = (bool)this.miniRecipient[ADRecipientSchema.MAPIBlockOutlookNonCachedMode];
						this.exchangeOrganizationInfo = this.GetExOrgInfoFlags();
						if (ExTraceGlobals.ConnectRpcTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.ConnectRpcTracer.TraceInformation<string, string>(0, Activity.TraceId, "Updated cached ExchangePrincipal for '{0}'. Server = '{1}', Mdb = '{2}'", this.userDn, (this.accessingPrincipal.MailboxInfo.Location != MailboxDatabaseLocation.Unknown) ? this.accessingPrincipal.MailboxInfo.Location.ToString() : "Database location info not available");
						}
					}
					else if (this.accessingPrincipal == null)
					{
						if (this.accessingPrincipalHasNoMailbox)
						{
							throw new UserHasNoMailboxException();
						}
						throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
					}
				}
			}

			public ExchangePrincipal GetExchangePrincipal(string legacyDN)
			{
				if (LegacyDN.StringComparer.Equals(this.LegacyDistinguishedName, legacyDN))
				{
					this.UpdatePrincipalCacheWrapped(false);
					return this.accessingPrincipal;
				}
				return this.userManager.FindExchangePrincipal(legacyDN, this.domain);
			}

			public void CorrelateIdentityWithLegacyDN(ClientSecurityContext clientSecurityContext)
			{
				Exception ex;
				if (this.accessingPrincipal == null)
				{
					if (this.miniRecipient.MasterAccountSid != null && !this.miniRecipient.MasterAccountSid.Equals(UserManager.User.useObjectSid))
					{
						SecurityIdentifier masterAccountSid = this.miniRecipient.MasterAccountSid;
						if (masterAccountSid.Equals(clientSecurityContext.UserSid))
						{
							return;
						}
						foreach (IdentityReference identityReference in clientSecurityContext.GetGroups())
						{
							if (identityReference.Equals(masterAccountSid))
							{
								return;
							}
						}
						ex = new LoginPermException(string.Format("MasterAccountSid of the {0} \"{1}\" {2} doesn't match SID {3} that a client has authenticated with.", new object[]
						{
							this.miniRecipient.RecipientType,
							this.userDn,
							this.miniRecipient.MasterAccountSid,
							clientSecurityContext
						}));
						this.BackoffConnect(ex);
						throw ex;
					}
					else if ((this.miniRecipient.RecipientType != RecipientType.User && this.miniRecipient.RecipientType != RecipientType.MailUser) || !this.miniRecipient.Sid.Equals(clientSecurityContext.UserSid))
					{
						goto IL_13C;
					}
					return;
				}
				if (this.CheckSecurityContext(this.accessingPrincipal, clientSecurityContext))
				{
					return;
				}
				if (this.userManager.CheckAccess(clientSecurityContext, this.miniRecipient))
				{
					return;
				}
				IL_13C:
				ex = new LoginPermException(string.Format("'{0}' can't act as owner of a {1} object '{2}' with SID {3} and MasterAccountSid {4}", new object[]
				{
					clientSecurityContext,
					this.miniRecipient.RecipientType,
					this.userDn,
					this.miniRecipient.Sid,
					this.miniRecipient.MasterAccountSid
				}));
				this.BackoffConnect(ex);
				throw ex;
			}

			string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
			{
				return string.Empty;
			}

			public int AddReference()
			{
				return Interlocked.Increment(ref this.referenceCount);
			}

			private void BackoffConnect(TimeSpan duration, Exception reason)
			{
				if ((this.miniRecipient.RecipientTypeDetails & RecipientTypeDetails.MonitoringMailbox) == RecipientTypeDetails.MonitoringMailbox)
				{
					return;
				}
				ExDateTime exDateTime = ExDateTime.UtcNow.Add(duration);
				int num = 0;
				lock (this.budgetLock)
				{
					if (this.backoffConnectUntil < exDateTime)
					{
						this.backoffConnectUntil = exDateTime;
					}
					num = Interlocked.Decrement(ref this.attemptsLeftBeforeBackoffTakesEffect);
					if (num < 0)
					{
						this.attemptsLeftBeforeBackoffTakesEffect = 0;
					}
					exDateTime = this.backoffConnectUntil;
					this.backoffConnectReason = reason;
				}
				ExTraceGlobals.ClientThrottledTracer.TraceWarning<string, ExDateTime, int>(Activity.TraceId, "Throttling connect requests for '{0}' until {1} after {2} more attempts", this.userDn, exDateTime, num);
			}

			private int RemoveReference()
			{
				return Interlocked.Decrement(ref this.referenceCount);
			}

			private ExOrgInfoFlags GetExOrgInfoFlags()
			{
				Guid arg;
				ExOrgInfoFlags exOrgInfoFlags;
				if (PublicFolderSession.TryGetHierarchyMailboxGuidForUser(this.accessingPrincipal.MailboxInfo.OrganizationId, this.accessingPrincipal.MailboxInfo.MailboxGuid, this.accessingPrincipal.DefaultPublicFolderMailbox, out arg))
				{
					exOrgInfoFlags = (ExOrgInfoFlags.PublicFoldersEnabled | ExOrgInfoFlags.UseAutoDiscoverForPublicFolderConfiguration);
					ExTraceGlobals.ConnectRpcTracer.TraceInformation<Guid, ExOrgInfoFlags>(0, Activity.TraceId, "We found a local public folder mailbox {0}. ExOrgInfoFlags returned:{1}", arg, exOrgInfoFlags);
				}
				else if (this.accessingPrincipal.MailboxInfo.Location.HomePublicFolderDatabaseGuid != Guid.Empty && this.userManager.DatabaseLocationProvider.GetLocationInfo(this.accessingPrincipal.MailboxInfo.Location.HomePublicFolderDatabaseGuid, false, true) != null)
				{
					PublicFoldersDeployment publicFoldersDeploymentType = PublicFolderSession.GetPublicFoldersDeploymentType(this.accessingPrincipal.MailboxInfo.OrganizationId);
					if (publicFoldersDeploymentType == PublicFoldersDeployment.Remote)
					{
						exOrgInfoFlags = (ExOrgInfoFlags.PublicFoldersEnabled | ExOrgInfoFlags.UseAutoDiscoverForPublicFolderConfiguration);
					}
					else
					{
						exOrgInfoFlags = ExOrgInfoFlags.PublicFoldersEnabled;
					}
					ExTraceGlobals.ConnectRpcTracer.TraceInformation<Guid, PublicFoldersDeployment, ExOrgInfoFlags>(0, Activity.TraceId, "Coexistence scenario. HomePublicFolderDatabaseGuid: {0}. PublicFoldersDeploymentType: {1}. ExOrgInfoFlags returned:{2}", this.accessingPrincipal.MailboxInfo.Location.HomePublicFolderDatabaseGuid, publicFoldersDeploymentType, exOrgInfoFlags);
				}
				else
				{
					exOrgInfoFlags = ExOrgInfoFlags.UseAutoDiscoverForPublicFolderConfiguration;
					ExTraceGlobals.ConnectRpcTracer.TraceInformation<ExOrgInfoFlags>(0, Activity.TraceId, "No public folders are provisioned. ExOrgInfoFlags returned:{0}", exOrgInfoFlags);
				}
				return exOrgInfoFlags;
			}

			private bool IsConnectAsValidDisabledUser()
			{
				return this.miniRecipient != null && (this.miniRecipient.RecipientType == RecipientType.User || this.miniRecipient.RecipientType == RecipientType.MailUser || this.miniRecipient.RecipientType == RecipientType.UserMailbox) && ((UserAccountControlFlags)this.miniRecipient[ADUserSchema.UserAccountControl] & UserAccountControlFlags.AccountDisabled) != UserAccountControlFlags.None;
			}

			private const int DefaultAttemptsBeforeBackoff = 10;

			private static readonly SecurityIdentifier useObjectSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);

			private static readonly TimeSpan defaultBackoffPeriod = TimeSpan.FromSeconds(10.0);

			private readonly UserManager userManager;

			private readonly object budgetLock = new object();

			private readonly object accessingPrincipalUpdateLock = new object();

			private readonly string userDn;

			private readonly string domain;

			private int referenceCount;

			private ExDateTime backoffConnectUntil = ExDateTime.MinValue;

			private Exception backoffConnectReason;

			private int attemptsLeftBeforeBackoffTakesEffect;

			private bool isRepeatingBackoff;

			private ExchangePrincipal accessingPrincipal;

			private MiniRecipient miniRecipient;

			private bool accessingPrincipalHasNoMailbox;

			private ExDateTime accessingPrincipalExpirationTime = ExDateTime.MinValue;

			private MapiVersionRanges blockedOutlookVersions;

			private bool mapiBlockOutlookRpcHttp;

			private bool mapiEnabled;

			private bool mapiCachedModeRequired;

			private ExOrgInfoFlags exchangeOrganizationInfo;
		}
	}
}
