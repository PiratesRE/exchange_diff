using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class UserContext : MailboxContextBase, IUserContext, IMailboxContext, IDisposable
	{
		internal UserContext(UserContextKey key, string userAgent) : base(key, userAgent)
		{
			this.themeKey = key + "Theme";
			this.LogEventCommonData = LogEventCommonData.NullInstance;
			if (!Globals.Owa2ServerUnitTestsHook)
			{
				this.lastUserRequestTime = Globals.ApplicationTime;
				this.signIntoIMTime = Globals.ApplicationTime;
			}
		}

		public string Canary
		{
			get
			{
				return base.Key.UserContextId;
			}
		}

		internal AttachmentDataProviderManager AttachmentDataProviderManager
		{
			get
			{
				if (this.attachmentDataProviderManager == null)
				{
					lock (this.syncRoot)
					{
						if (this.attachmentDataProviderManager == null)
						{
							this.attachmentDataProviderManager = new AttachmentDataProviderManager();
						}
					}
				}
				return this.attachmentDataProviderManager;
			}
		}

		internal CancelAttachmentManager CancelAttachmentManager
		{
			get
			{
				if (this.cancelAttachmentManager == null)
				{
					lock (this.syncRoot)
					{
						if (this.cancelAttachmentManager == null)
						{
							this.cancelAttachmentManager = new CancelAttachmentManager(this);
						}
					}
				}
				return this.cancelAttachmentManager;
			}
		}

		internal bool HasActiveHierarchySubscription
		{
			get
			{
				return this.hasActiveHierarchySubscription;
			}
			set
			{
				this.hasActiveHierarchySubscription = value;
			}
		}

		public InstantMessagingTypeOptions InstantMessageType
		{
			get
			{
				if (this.instantMessageType == null)
				{
					if (base.ExchangePrincipal != null)
					{
						ConfigurationContext configurationContext = new ConfigurationContext(this);
						this.instantMessageType = new InstantMessagingTypeOptions?((configurationContext != null) ? configurationContext.InstantMessagingType : InstantMessagingTypeOptions.None);
					}
					else
					{
						this.instantMessageType = new InstantMessagingTypeOptions?(InstantMessagingTypeOptions.None);
					}
				}
				return this.instantMessageType.Value;
			}
		}

		internal string BposSkuCapability { get; private set; }

		public ulong AllowedCapabilitiesFlags
		{
			get
			{
				if (this.allowedCapabilitiesFlags == 0UL)
				{
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.CheckFeatureRestrictions.Enabled && base.MailboxIdentity != null && !Globals.IsPreCheckinApp)
					{
						OWAMiniRecipient owaminiRecipient = base.MailboxIdentity.GetOWAMiniRecipient();
						if (owaminiRecipient[ADUserSchema.PersistedCapabilities] != null)
						{
							foreach (object obj in Enum.GetValues(typeof(Feature)))
							{
								ulong num = (ulong)obj;
								if (num != 18446744073709551615UL)
								{
									string text = string.Format("Owa{0}Restrictions", Enum.GetName(typeof(Feature), num));
									try
									{
										if (ExchangeRunspaceConfiguration.IsFeatureValidOnObject(text, owaminiRecipient))
										{
											this.allowedCapabilitiesFlags |= num;
										}
										else
										{
											ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Feature {0} is restricted by user capabilities", text);
										}
									}
									catch (ArgumentException ex)
									{
										ExTraceGlobals.UserContextTracer.TraceDebug<string, string>(0L, "There was an exception in ExchangeRunspaceConfiguration.IsFeatureValidOnObject when validating feature: {0}. {1}", text, ex.Message);
									}
								}
							}
						}
					}
					if (this.allowedCapabilitiesFlags == 0UL)
					{
						this.allowedCapabilitiesFlags = ulong.MaxValue;
					}
				}
				return this.allowedCapabilitiesFlags;
			}
		}

		public long LastUserRequestTime
		{
			get
			{
				return this.lastUserRequestTime;
			}
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
		}

		public CultureInfo UserCulture
		{
			get
			{
				if (this.userCulture == null)
				{
					return Culture.GetPreferredCultureInfo(base.ExchangePrincipal);
				}
				return this.userCulture;
			}
			set
			{
				this.userCulture = value;
			}
		}

		internal bool IsGroupUserContext { get; private set; }

		internal bool IsUserCultureExplicitlySet
		{
			get
			{
				return this.userCulture != null;
			}
		}

		public long SignIntoIMTime
		{
			get
			{
				return this.signIntoIMTime;
			}
			set
			{
				this.signIntoIMTime = value;
			}
		}

		public bool IsPublicLogon
		{
			get
			{
				if (this.isPublicLogon == null)
				{
					this.isPublicLogon = new bool?(UserContextUtilities.IsPublicLogon(base.ExchangePrincipal.MailboxInfo.OrganizationId, HttpContext.Current));
				}
				return this.isPublicLogon.Value;
			}
			set
			{
				this.isPublicLogon = new bool?(value);
			}
		}

		internal List<IConnectedAccountsNotificationManager> GetConnectedAccountNotificationManagers(MailboxSession mailboxSession)
		{
			if (UserAgentUtilities.IsMonitoringRequest(HttpContext.Current.Request.UserAgent))
			{
				return new List<IConnectedAccountsNotificationManager>();
			}
			if (this.isConnectedAccountsNotificationSetupDone)
			{
				return this.connectedAccountNotificationManagers;
			}
			this.SetupTxSyncNotificationManager(mailboxSession);
			this.SetupMrsNotificationManager(mailboxSession);
			this.isConnectedAccountsNotificationSetupDone = true;
			return this.connectedAccountNotificationManagers;
		}

		public PlayOnPhoneNotificationManager PlayOnPhoneNotificationManager
		{
			get
			{
				if (this.playonPhoneNotificationManager == null)
				{
					this.playonPhoneNotificationManager = new PlayOnPhoneNotificationManager(this);
				}
				return this.playonPhoneNotificationManager;
			}
		}

		public InstantMessageManager InstantMessageManager
		{
			get
			{
				if (this.IsInstantMessageEnabled && this.instantMessageManager == null)
				{
					this.instantMessageManager = new InstantMessageManager(this);
				}
				return this.instantMessageManager;
			}
		}

		internal bool IsBposUser { get; private set; }

		public BposNavBarInfoAssetReader BposNavBarInfoAssetReader
		{
			get
			{
				if (this.IsBposUser && this.bposNavBarInfoAssetReader == null)
				{
					lock (this.syncRoot)
					{
						if (this.bposNavBarInfoAssetReader == null)
						{
							this.bposNavBarInfoAssetReader = new BposNavBarInfoAssetReader(base.LogonIdentity.GetOWAMiniRecipient().UserPrincipalName, this.UserCulture);
						}
					}
				}
				return this.bposNavBarInfoAssetReader;
			}
		}

		public BposShellInfoAssetReader BposShellInfoAssetReader
		{
			get
			{
				if (this.IsBposUser && this.bposShellInfoAssetReader == null)
				{
					lock (this.syncRoot)
					{
						if (this.bposShellInfoAssetReader == null)
						{
							BposHeaderFlight currentHeaderFlight = BposHeaderFlight.E15Parity;
							if (this.FeaturesManager.ClientServerSettings.O365G2Header.Enabled)
							{
								currentHeaderFlight = BposHeaderFlight.E16Gemini2;
							}
							else if (this.FeaturesManager.ClientServerSettings.O365Header.Enabled)
							{
								currentHeaderFlight = BposHeaderFlight.E16Gemini1;
							}
							this.bposShellInfoAssetReader = new BposShellInfoAssetReader(base.LogonIdentity.GetOWAMiniRecipient().UserPrincipalName, this.UserCulture, currentHeaderFlight, this);
						}
					}
				}
				return this.bposShellInfoAssetReader;
			}
		}

		public bool IsInstantMessageEnabled
		{
			get
			{
				return this.InstantMessageType == InstantMessagingTypeOptions.Ocs && InstantMessageOCSProvider.EndpointManager != null;
			}
		}

		internal InstantSearchManager InstantSearchManager
		{
			get
			{
				if (this.instantSearchManager != null)
				{
					return this.instantSearchManager;
				}
				lock (this.syncRoot)
				{
					if (this.instantSearchManager == null)
					{
						this.instantSearchManager = new InstantSearchManager(() => this.CreateMailboxSessionForInstantSearch());
					}
				}
				return this.instantSearchManager;
			}
		}

		internal IInstantSearchNotificationHandler InstantSearchNotificationHandler
		{
			get
			{
				if (this.instantSearchNotificationHandler != null)
				{
					return this.instantSearchNotificationHandler;
				}
				lock (this.syncRoot)
				{
					if (this.instantSearchNotificationHandler == null)
					{
						if (base.IsExplicitLogon)
						{
							this.instantSearchNotificationHandler = new InstantSearchRemoteNotificationHandler(this);
						}
						else
						{
							this.instantSearchNotificationHandler = new InstantSearchNotificationHandler(this);
						}
					}
				}
				return this.instantSearchNotificationHandler;
			}
		}

		internal LogEventCommonData LogEventCommonData { get; private set; }

		internal Theme Theme
		{
			get
			{
				Theme theme = (Theme)HttpRuntime.Cache.Get(this.themeKey);
				if (theme == null)
				{
					lock (this.syncRoot)
					{
						theme = (Theme)HttpRuntime.Cache.Get(this.themeKey);
						if (theme == null)
						{
							theme = this.LoadUserTheme();
							HttpRuntime.Cache.Insert(this.themeKey, theme, null, DateTime.UtcNow.AddMinutes(1.0), Cache.NoSlidingExpiration);
						}
					}
				}
				return theme;
			}
		}

		internal Theme DefaultTheme
		{
			get
			{
				ConfigurationContext configurationContext = new ConfigurationContext(this);
				string text = configurationContext.DefaultTheme;
				if (this.defaultTheme == null || this.defaultTheme.StorageId != text)
				{
					this.defaultTheme = ThemeManagerFactory.GetInstance(this.CurrentOwaVersion).GetDefaultTheme(text);
				}
				return this.defaultTheme;
			}
		}

		public FeaturesManager FeaturesManager
		{
			get
			{
				if (this.featuresManagerFactory == null)
				{
					return null;
				}
				return this.featuresManagerFactory.GetFeaturesManager(HttpContext.Current);
			}
		}

		public override SessionDataCache SessionDataCache
		{
			get
			{
				if (this.sessionDataCache == null)
				{
					this.sessionDataCache = new SessionDataCache();
				}
				return this.sessionDataCache;
			}
		}

		internal bool IsOptimizedForAccessibility
		{
			get
			{
				if (this.isOptimizedForAccessibility == null)
				{
					try
					{
						if (base.IsExplicitLogon)
						{
							this.isOptimizedForAccessibility = new bool?(false);
						}
						else
						{
							base.LockAndReconnectMailboxSession(3000);
							UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.IsOptimizedForAccessibility);
							UserOptionsType userOptionsType = new UserOptionsType();
							userOptionsType.Load(base.MailboxSession, new UserConfigurationPropertyDefinition[]
							{
								propertyDefinition
							}, true);
							this.isOptimizedForAccessibility = new bool?(userOptionsType.IsOptimizedForAccessibility);
						}
					}
					catch (Exception ex)
					{
						ExTraceGlobals.CoreTracer.TraceError<string, string>(0L, "Failed to retrieve IsOptimizedForAccessibility from user options. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
						this.isOptimizedForAccessibility = null;
						throw;
					}
					finally
					{
						base.UnlockAndDisconnectMailboxSession();
					}
					base.LogTrace("IsOptimizedForAccessibility", "userOptions.IsOptimizedForAccessibility loaded");
				}
				return this.isOptimizedForAccessibility.Value;
			}
		}

		public ADObjectId GlobalAddressListId
		{
			get
			{
				if (this.globalAddressListId == null)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, base.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 899, "GlobalAddressListId", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContext.cs");
					this.globalAddressListId = DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(base.ExchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy, tenantOrTopologyConfigurationSession);
				}
				return this.globalAddressListId;
			}
		}

		public string CurrentOwaVersion
		{
			get
			{
				if (string.IsNullOrEmpty(this.currentOwaVersion))
				{
					if (this.FeaturesManager.ServerSettings.OwaVNext.Enabled)
					{
						this.currentOwaVersion = OwaVersionId.VNext;
					}
					else
					{
						this.currentOwaVersion = OwaVersionId.Current;
					}
				}
				return this.currentOwaVersion;
			}
		}

		public bool IsWacEditingEnabled
		{
			get
			{
				if (this.isWacEditingEnabled == null)
				{
					WacConfigData wacEditingEnabled = AttachmentPolicy.ReadAggregatedWacData(this, null);
					this.SetWacEditingEnabled(wacEditingEnabled);
				}
				return this.isWacEditingEnabled.Value;
			}
		}

		internal void SetWacEditingEnabled(WacConfigData wacData)
		{
			bool flag = AttachmentPolicy.IsAttachmentDataProviderAvailable(wacData);
			this.isWacEditingEnabled = new bool?(wacData.IsWacEditingEnabled && flag);
		}

		public string[] GetClientWatsonHistory()
		{
			string[] result;
			lock (this.clientWatsonHistoryLock)
			{
				result = this.clientWatsonHistory.ToArray();
			}
			return result;
		}

		public void SaveToClientWatsonHistory(params string[] clientWatsonsData)
		{
			lock (this.clientWatsonHistoryLock)
			{
				int num = (clientWatsonsData.Length <= 5) ? 0 : (clientWatsonsData.Length - 5);
				for (int i = num; i < clientWatsonsData.Length; i++)
				{
					if (this.clientWatsonHistory.Count == 5)
					{
						this.clientWatsonHistory.Dequeue();
					}
					this.clientWatsonHistory.Enqueue(clientWatsonsData[i]);
				}
			}
		}

		public void UpdateLastUserRequestTime()
		{
			this.lastUserRequestTime = Globals.ApplicationTime;
			if (this.IsInstantMessageEnabled && this.InstantMessageManager != null)
			{
				this.instantMessageManager.ResetPresence();
			}
		}

		public void Touch()
		{
			HttpRuntime.Cache.Get(base.Key.ToString());
		}

		public void ClearCachedTheme()
		{
			lock (this.syncRoot)
			{
				HttpRuntime.Cache.Remove(this.themeKey);
			}
		}

		public bool LockAndReconnectMailboxSession()
		{
			return base.LockAndReconnectMailboxSession(3000);
		}

		public void DoLogoffCleanup()
		{
			base.LogBreadcrumb("DoLogoffCleanup");
			if (!this.isMailboxSessionCreated)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug((long)this.GetHashCode(), "DoLogoffCleanup - No mailbox session on the user context, no cleanup necessary");
				return;
			}
			try
			{
				this.LockAndReconnectMailboxSession();
				UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.EmptyDeletedItemsOnLogoff);
				UserOptionsType userOptionsType = new UserOptionsType();
				userOptionsType.Load(base.MailboxSession, new UserConfigurationPropertyDefinition[]
				{
					propertyDefinition
				});
				if (userOptionsType.EmptyDeletedItemsOnLogoff)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug((long)this.GetHashCode(), "DoLogoffCleanup - Emptying deleted items folder.");
					base.MailboxSession.DeleteAllObjects(DeleteItemFlags.SoftDelete, base.MailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems));
				}
			}
			catch (OwaLockTimeoutException)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug((long)this.GetHashCode(), "DoLogoffCleanup - Encountered OwaLockTimeoutException");
			}
			finally
			{
				base.UnlockAndDisconnectMailboxSession();
			}
		}

		protected override void DoLoad(OwaIdentity logonIdentity, OwaIdentity mailboxIdentity, UserContextStatistics stats)
		{
			HttpContext httpContext = HttpContext.Current;
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.UserContextLoadBegin);
			base.DoLoad(logonIdentity, mailboxIdentity, stats);
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.GetOWAMiniRecipientBegin);
			Stopwatch stopwatch = Stopwatch.StartNew();
			OWAMiniRecipient owaminiRecipient = base.LogonIdentity.GetOWAMiniRecipient();
			stats.MiniRecipientCreationTime = (int)stopwatch.ElapsedMilliseconds;
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.GetOWAMiniRecipientEnd);
			base.LogTrace("UserContext.Load", "GetOWAMiniRecipient finished");
			this.sipUri = ADPersonToContactConverter.GetSipUri(owaminiRecipient);
			stopwatch.Restart();
			this.IsBposUser = CapabilityHelper.HasBposSKUCapability(owaminiRecipient.PersistedCapabilities);
			stats.SKUCapabilityTestTime = (int)stopwatch.ElapsedMilliseconds;
			base.LogTrace("UserContext.Load", "HasBposSKUCapability finished");
			if (Globals.IsFirstReleaseFlightingEnabled)
			{
				this.CreateFeatureManagerFactory(owaminiRecipient);
			}
			else
			{
				RecipientTypeDetails recipientTypeDetails = base.ExchangePrincipal.RecipientTypeDetails;
				this.featuresManagerFactory = new FeaturesManagerFactory(owaminiRecipient, new ConfigurationContext(this), new ScopeFlightsSettingsProvider(), (VariantConfigurationSnapshot c) => new FeaturesStateOverride(c, recipientTypeDetails), string.Empty, false);
			}
			this.BposSkuCapability = string.Empty;
			if (this.IsBposUser)
			{
				Capability? skucapability = CapabilityHelper.GetSKUCapability(owaminiRecipient.PersistedCapabilities);
				if (skucapability != null)
				{
					this.BposSkuCapability = skucapability.ToString();
				}
			}
			this.LogEventCommonData = new LogEventCommonData(this);
			this.IsGroupUserContext = (base.IsExplicitLogon && base.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.UserContextLoadEnd);
		}

		protected override INotificationManager GetNotificationManager(MailboxContextBase mailboxContext)
		{
			if (this.FeaturesManager.ClientServerSettings.NotificationBroker.Enabled)
			{
				return new BrokerNotificationManager(mailboxContext);
			}
			return base.GetNotificationManager(mailboxContext);
		}

		private MailboxSession CreateMailboxSessionForInstantSearch()
		{
			MailboxSession mailboxSession = base.LogonIdentity.CreateInstantSearchMailboxSession(base.ExchangePrincipal, Thread.CurrentThread.CurrentCulture);
			if (mailboxSession == null)
			{
				throw new OwaInvalidOperationException("CreateMailboxSession cannot create a mailbox session");
			}
			return mailboxSession;
		}

		public override void ValidateLogonPermissionIfNecessary()
		{
			if (!base.IsExplicitLogon)
			{
				return;
			}
			if (base.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug(0L, "Validate explicit logon permission by creating a mailbox session.");
				base.CreateMailboxSessionIfNeeded();
				return;
			}
			this.ValidateBackendServerIfNeeded();
		}

		private void SetupTxSyncNotificationManager(MailboxSession mailboxSession)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.OwaDeployment.ConnectedAccountsSync.Enabled || !ConnectedAccountsConfiguration.Instance.NotificationsEnabled)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "UserContext.SetupTxSyncNotificationManager - ConnectedAccountsNotificationManager was not set because no DC or Notifications not enabled.");
				return;
			}
			IConnectedAccountsNotificationManager connectedAccountsNotificationManager = TxSyncConnectedAccountsNotificationManager.Create(mailboxSession, this);
			if (connectedAccountsNotificationManager != null)
			{
				this.connectedAccountNotificationManagers.Add(connectedAccountsNotificationManager);
			}
		}

		internal UserConfigurationManager.IAggregationContext TryConsumeBootAggregation()
		{
			UserConfigurationManager.IAggregationContext comparand = this.bootAggregationContext;
			return Interlocked.CompareExchange<UserConfigurationManager.IAggregationContext>(ref this.bootAggregationContext, null, comparand);
		}

		internal void SetupMrsNotificationManager(MailboxSession mailboxSession)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.OwaDeployment.MrsConnectedAccountsSync.Enabled || !ConnectedAccountsConfiguration.Instance.NotificationsEnabled)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "UserContext.SetupMrsNotificationManager - ConnectedAccountsNotificationManager was not set because no DC or Notifications not enabled.");
				return;
			}
			IConnectedAccountsNotificationManager connectedAccountsNotificationManager = MrsConnectedAccountsNotificationManager.Create(mailboxSession, this);
			if (connectedAccountsNotificationManager != null)
			{
				this.connectedAccountNotificationManagers.Add(connectedAccountsNotificationManager);
			}
		}

		internal void RetireMailboxSessionForGroupMailbox()
		{
			if (!this.IsGroupUserContext)
			{
				throw new InvalidOperationException("RetireMailboxSessionForGroupMailbox is only supported for group mailbox");
			}
			try
			{
				base.InternalRetireMailboxSession();
			}
			catch (LocalizedException ex)
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("RetUcMbSess", this, "UserContext.RetireMailboxSessionForGroupMailbox", ex.ToString()));
			}
		}

		internal AddressBookBase GetGlobalAddressList(IBudget budget)
		{
			if (!this.isGlobalAddressListLoaded)
			{
				IConfigurationSession configurationSession = UserContextUtilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this, budget);
				IRecipientSession recipientSession = UserContextUtilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, false, this, false, budget);
				this.globalAddressList = AddressBookBase.GetGlobalAddressList(base.LogonIdentity.ClientSecurityContext, configurationSession, recipientSession, this.GlobalAddressListId);
				this.isGlobalAddressListLoaded = true;
			}
			return this.globalAddressList;
		}

		internal void RefreshUserSettings(CultureInfo userCulture, EcpUserSettings userSettings)
		{
			if (userCulture != null)
			{
				this.userCulture = userCulture;
				Culture.InternalSetThreadPreferredCulture(userCulture);
			}
			this.RefreshMailboxSession(userSettings);
			this.ClearBposShellData();
		}

		internal long GetNextClientActivitySequenceNumber()
		{
			return Interlocked.Increment(ref this.nextActivitySequenceNumber);
		}

		internal static OwaFlightConfigData ReadAggregatedFlightConfigData(UserConfigurationManager.IAggregationContext aggregator, OrganizationId orgId)
		{
			return UserContextUtilities.ReadAggregatedType<OwaFlightConfigData>(aggregator, "OWA.FlightConfiguration", () => UserContext.ReadFlightConfigDataFromAD(orgId));
		}

		protected override void DisposeMailboxSessionReferencingObjects()
		{
			base.UserContextDiposeGraph.Append(".uc1");
			if (this.instantMessageManager != null)
			{
				base.UserContextDiposeGraph.Append(".uc2");
				this.instantMessageManager.Dispose();
				this.instantMessageManager = null;
			}
			base.DisposeMailboxSessionReferencingObjects();
		}

		protected override void DisposeNonMailboxSessionReferencingObjects()
		{
			base.UserContextDiposeGraph.Append(".ub1");
			if (this.playonPhoneNotificationManager != null)
			{
				base.UserContextDiposeGraph.Append(".ub2");
				this.playonPhoneNotificationManager.Dispose();
				this.playonPhoneNotificationManager = null;
			}
			if (this.instantSearchManager != null)
			{
				base.UserContextDiposeGraph.Append(".ub3");
				this.instantSearchManager.Dispose();
				this.instantSearchManager = null;
			}
			if (this.instantSearchNotificationHandler != null)
			{
				base.UserContextDiposeGraph.Append(".ub4");
				IDisposable disposable = this.instantSearchNotificationHandler as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.instantSearchNotificationHandler = null;
			}
			int num = 5;
			for (int i = 0; i < this.connectedAccountNotificationManagers.Count; i++)
			{
				base.UserContextDiposeGraph.Append(".ub" + num);
				num++;
				this.connectedAccountNotificationManagers[i].Dispose();
				this.connectedAccountNotificationManagers[i] = null;
			}
			this.connectedAccountNotificationManagers = null;
			if (this.sessionDataCache != null)
			{
				this.sessionDataCache.Dispose();
				this.sessionDataCache = null;
			}
			using (this.TryConsumeBootAggregation())
			{
				this.bootAggregationContext = null;
			}
			base.DisposeNonMailboxSessionReferencingObjects();
		}

		protected override MailboxSession CreateMailboxSession()
		{
			if (base.IsDisposed || this.isInProcessOfDisposing)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Cannot call UserContext.CreateMailboxSession when object is disposed. isDisposed={0}, isInProcessOfDisposing={1}.", new object[]
				{
					base.IsDisposed,
					this.isInProcessOfDisposing
				});
				throw new ObjectDisposedException("UserContext", message);
			}
			if (base.LogonIdentity == null)
			{
				throw new OwaInvalidOperationException("Cannot call CreateMailboxSession when logonIdentity is null");
			}
			MailboxSession result;
			try
			{
				if (base.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
				{
					Exception ex = null;
					ExTraceGlobals.UserContextTracer.TraceDebug(0L, "Creating Mailbox session for TeamMailbox");
					SharepointAccessManager.Instance.UpdateAccessTokenIfNeeded(base.ExchangePrincipal, OauthUtils.GetOauthCredential(base.LogonIdentity.GetOWAMiniRecipient()), base.LogonIdentity.ClientSecurityContext, out ex, false);
					if (ex != null)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug<Exception>(0L, "CreateMailboxSession for TeamMailbox hit exception while updating AccessToken: {0}", ex);
					}
				}
				CultureInfo cultureInfo = Culture.GetPreferredCultureInfo(base.ExchangePrincipal) ?? Thread.CurrentThread.CurrentCulture;
				MailboxSession mailboxSession;
				if (base.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
				{
					mailboxSession = base.LogonIdentity.CreateDelegateMailboxSession(base.ExchangePrincipal, cultureInfo);
				}
				else
				{
					mailboxSession = base.LogonIdentity.CreateMailboxSession(base.ExchangePrincipal, cultureInfo);
				}
				if (mailboxSession == null)
				{
					throw new OwaInvalidOperationException("CreateMailboxSession cannot create a mailbox session");
				}
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("user has no access rights to the mailbox", "errorexplicitlogonaccessdenied", innerException);
			}
			return result;
		}

		private static OwaFlightConfigData ReadFlightConfigDataFromAD(OrganizationId organizationId)
		{
			if (organizationId == null || organizationId.ConfigurationUnit == null)
			{
				return new OwaFlightConfigData
				{
					RampId = string.Empty,
					IsFirstRelease = false
				};
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 1563, "ReadFlightConfigDataFromAD", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContext.cs");
			ExchangeConfigurationUnit configurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
			return new OwaFlightConfigData
			{
				RampId = ExchangeConfigurationUnitVariantConfigurationParser.GetRampId(configurationUnit),
				IsFirstRelease = ExchangeConfigurationUnitVariantConfigurationParser.IsFirstRelease(configurationUnit)
			};
		}

		private void ClearBposShellData()
		{
			if (!this.IsBposUser)
			{
				return;
			}
			lock (this.syncRoot)
			{
				this.bposNavBarInfoAssetReader = null;
				this.bposShellInfoAssetReader = null;
			}
		}

		private void RefreshMailboxSession(EcpUserSettings userSettings)
		{
			base.UserContextDiposeGraph.Append(".ur1");
			if ((userSettings & EcpUserSettings.Regional) == EcpUserSettings.Regional)
			{
				ExTimeZone exTimeZone = null;
				try
				{
					if (this.mailboxSessionLock.LockWriterElastic(3000))
					{
						base.UserContextDiposeGraph.Append(".ur2");
						if (base.NotificationManager != null)
						{
							base.UserContextDiposeGraph.Append(".ur3");
							base.NotificationManager.CleanupSubscriptions();
						}
						if (this.mailboxSession != null)
						{
							base.UserContextDiposeGraph.Append(".ur4");
							base.DisposeMailboxSession();
						}
						this.mailboxSession = this.CreateMailboxSession();
						this.isMailboxSessionCreated = true;
						UserContextUtilities.ReconnectStoreSession(this.mailboxSession, this);
						exTimeZone = TimeZoneHelper.GetUserTimeZone(this.mailboxSession);
					}
				}
				finally
				{
					if (this.mailboxSessionLock.IsWriterLockHeld)
					{
						if (this.mailboxSession != null)
						{
							base.UserContextDiposeGraph.Append(".ur5");
							base.UnlockAndDisconnectMailboxSession();
						}
						else
						{
							base.UserContextDiposeGraph.Append(".ur6");
							this.mailboxSessionLock.ReleaseWriterLock();
						}
					}
				}
				if (exTimeZone != null && base.NotificationManager != null)
				{
					base.NotificationManager.RefreshSubscriptions(exTimeZone);
				}
			}
			base.UserContextDiposeGraph.Append(".ur7");
		}

		private Theme LoadUserTheme()
		{
			ConfigurationContext configurationContext = new ConfigurationContext(this);
			Theme result;
			if (configurationContext.IsFeatureEnabled(Feature.Themes))
			{
				string text = null;
				try
				{
					base.LockAndReconnectMailboxSession(30000);
					UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ThemeStorageId);
					UserOptionsType userOptionsType = new UserOptionsType();
					userOptionsType.Load(base.MailboxSession, new UserConfigurationPropertyDefinition[]
					{
						propertyDefinition
					});
					text = userOptionsType.ThemeStorageId;
				}
				catch (Exception)
				{
					ExTraceGlobals.ThemesTracer.TraceError(0L, "Failed to find the user's theme from UserOptions");
				}
				finally
				{
					base.UnlockAndDisconnectMailboxSession();
				}
				if (string.IsNullOrEmpty(text))
				{
					result = this.DefaultTheme;
				}
				else
				{
					uint idFromStorageId = ThemeManagerFactory.GetInstance(this.CurrentOwaVersion).GetIdFromStorageId(text);
					if (idFromStorageId == 4294967295U)
					{
						result = this.DefaultTheme;
					}
					else
					{
						result = ThemeManagerFactory.GetInstance(this.CurrentOwaVersion).Themes[(int)((UIntPtr)idFromStorageId)];
					}
				}
			}
			else
			{
				result = this.DefaultTheme;
			}
			return result;
		}

		private void ValidateBackendServerIfNeeded()
		{
			if (this.isBackendServerValidated)
			{
				return;
			}
			IMailboxInfo mailboxInfo = base.ExchangePrincipal.MailboxInfo;
			string localServerFqdn = LocalServerCache.LocalServerFqdn;
			string serverFqdn = mailboxInfo.Location.ServerFqdn;
			ExTraceGlobals.CoreTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UserContext.ValidateBackendServerIfNeeded: Target Mailbox location {0}. Current Server Name: {1}.", serverFqdn, localServerFqdn);
			if (!localServerFqdn.Equals(serverFqdn, StringComparison.OrdinalIgnoreCase))
			{
				throw new WrongServerException(ServerStrings.IncorrectServerError(mailboxInfo.PrimarySmtpAddress, serverFqdn), mailboxInfo.GetDatabaseGuid(), serverFqdn, mailboxInfo.Location.ServerVersion, null);
			}
			this.isBackendServerValidated = true;
		}

		private void CreateFeatureManagerFactory(OWAMiniRecipient miniRecipient)
		{
			if (this.featuresManagerFactory != null)
			{
				return;
			}
			if (!base.IsExplicitLogon)
			{
				this.CreateFeatureManagerFactoryFromMailbox(miniRecipient);
				return;
			}
			this.CreateFeatureManagerFactoryFromAD(miniRecipient);
		}

		private void CreateFeatureManagerFactoryFromMailbox(OWAMiniRecipient miniRecipient)
		{
			UserConfigurationManager.IAggregationContext aggregationContext = null;
			try
			{
				this.LockAndReconnectMailboxSession();
				aggregationContext = base.MailboxSession.UserConfigurationManager.AttachAggregator(AggregatedUserConfigurationSchema.Instance.OwaUserConfiguration);
				base.UnlockAndDisconnectMailboxSession();
				OwaFlightConfigData owaFlightConfigData = UserContext.ReadAggregatedFlightConfigData(aggregationContext, base.ExchangePrincipal.MailboxInfo.OrganizationId);
				RecipientTypeDetails recipientTypeDetails = base.ExchangePrincipal.RecipientTypeDetails;
				this.featuresManagerFactory = new FeaturesManagerFactory(miniRecipient, new ConfigurationContext(this), new ScopeFlightsSettingsProvider(), (VariantConfigurationSnapshot c) => new FeaturesStateOverride(c, recipientTypeDetails), owaFlightConfigData.RampId, owaFlightConfigData.IsFirstRelease);
			}
			finally
			{
				if (aggregationContext != null)
				{
					aggregationContext.Detach();
					this.bootAggregationContext = aggregationContext;
				}
				if (base.MailboxSessionLockedByCurrentThread())
				{
					base.UnlockAndDisconnectMailboxSession();
				}
			}
		}

		private void CreateFeatureManagerFactoryFromAD(OWAMiniRecipient miniRecipient)
		{
			OwaFlightConfigData owaFlightConfigData = UserContext.ReadFlightConfigDataFromAD(base.ExchangePrincipal.MailboxInfo.OrganizationId);
			RecipientTypeDetails recipientTypeDetails = base.ExchangePrincipal.RecipientTypeDetails;
			this.featuresManagerFactory = new FeaturesManagerFactory(miniRecipient, new ConfigurationContext(this), new ScopeFlightsSettingsProvider(), (VariantConfigurationSnapshot c) => new FeaturesStateOverride(c, recipientTypeDetails), owaFlightConfigData.RampId, owaFlightConfigData.IsFirstRelease);
		}

		private const int MaxClientWatsonHistoryCount = 5;

		private const string ThemeKeySuffix = "Theme";

		private const int ThemeCacheLifeInMinutes = 1;

		private readonly object clientWatsonHistoryLock = new object();

		private readonly Queue<string> clientWatsonHistory = new Queue<string>(5);

		private long lastUserRequestTime;

		private bool hasActiveHierarchySubscription;

		private PlayOnPhoneNotificationManager playonPhoneNotificationManager;

		private InstantMessageManager instantMessageManager;

		private BposNavBarInfoAssetReader bposNavBarInfoAssetReader;

		private BposShellInfoAssetReader bposShellInfoAssetReader;

		private long signIntoIMTime;

		private CultureInfo userCulture;

		private InstantMessagingTypeOptions? instantMessageType;

		private string sipUri;

		private Theme defaultTheme;

		private bool isGlobalAddressListLoaded;

		private AddressBookBase globalAddressList;

		private ADObjectId globalAddressListId;

		private List<IConnectedAccountsNotificationManager> connectedAccountNotificationManagers = new List<IConnectedAccountsNotificationManager>(2);

		private bool isConnectedAccountsNotificationSetupDone;

		private ulong allowedCapabilitiesFlags;

		private readonly string themeKey;

		private AttachmentDataProviderManager attachmentDataProviderManager;

		private CancelAttachmentManager cancelAttachmentManager;

		private long nextActivitySequenceNumber = -1L;

		private bool? isOptimizedForAccessibility = null;

		private FeaturesManagerFactory featuresManagerFactory;

		private volatile InstantSearchManager instantSearchManager;

		private volatile IInstantSearchNotificationHandler instantSearchNotificationHandler;

		private bool isBackendServerValidated;

		private SessionDataCache sessionDataCache;

		private bool? isPublicLogon;

		private string currentOwaVersion;

		private UserConfigurationManager.IAggregationContext bootAggregationContext;

		private bool? isWacEditingEnabled;
	}
}
