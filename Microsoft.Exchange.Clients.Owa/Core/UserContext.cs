using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Clients.Owa.Core.Transcoding;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class UserContext : UserContextBase, ISessionContext
	{
		internal UserContextTerminationStatus TerminationStatus { get; set; }

		internal UserContext(UserContextKey key) : base(key)
		{
			this.isProxy = true;
			this.isClientSideDataCollectingEnabled = this.GetIsClientSideDataCollectingEnabled();
			this.IsBposUser = false;
			if (!Globals.DisableBreadcrumbs)
			{
				this.breadcrumbBuffer = new BreadcrumbBuffer(Globals.MaxBreadcrumbs);
			}
			this.pendingRequestManager = new PendingRequestManager(this);
			this.irmLicensingManager = new IrmLicensingManager(this);
		}

		public static int ReminderPollInterval
		{
			get
			{
				return 28800;
			}
		}

		public InstantMessagingTypeOptions InstantMessagingType
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.InstantMessagingType;
				}
				return this.configuration.InstantMessagingType;
			}
		}

		public bool IsPushNotificationsEnabled
		{
			get
			{
				return !this.IsBasicExperience && !this.IsWebPartRequest && this.isPushNotificationsEnabled;
			}
		}

		public bool IsPullNotificationsEnabled
		{
			get
			{
				return this.isPullNotificationsEnabled;
			}
		}

		public string SetPhotoURL
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.SetPhotoURL;
				}
				return this.configuration.SetPhotoURL;
			}
		}

		internal PerformanceNotifier PerformanceConsoleNotifier
		{
			get
			{
				if (this.performanceNotifier == null)
				{
					this.performanceNotifier = new PerformanceNotifier();
				}
				return this.performanceNotifier;
			}
		}

		internal MailTipsNotificationHandler MailTipsNotificationHandler
		{
			get
			{
				if (this.mailTipsNotificationHandler == null)
				{
					this.mailTipsNotificationHandler = new MailTipsNotificationHandler(this);
				}
				return this.mailTipsNotificationHandler;
			}
		}

		public bool IsPerformanceConsoleOn
		{
			get
			{
				return this.isPerformanceConsoleOn;
			}
			set
			{
				this.isPerformanceConsoleOn = value;
				if (this.isPerformanceConsoleOn)
				{
					this.performanceNotifier.RegisterWithPendingRequestNotifier();
					return;
				}
				this.performanceNotifier.UnregisterWithPendingRequestNotifier();
			}
		}

		public bool IsClientSideDataCollectingEnabled
		{
			get
			{
				return this.isClientSideDataCollectingEnabled;
			}
		}

		internal bool IsEmbeddedReadingPaneDisabled
		{
			get
			{
				return this.shouldDisableEmbeddedReadingPane;
			}
		}

		internal void DisableEmbeddedReadingPane()
		{
			this.shouldDisableEmbeddedReadingPane = true;
		}

		internal AutoCompleteCache AutoCompleteCache
		{
			get
			{
				return this.autoCompleteCache;
			}
			set
			{
				this.autoCompleteCache = value;
			}
		}

		internal RoomsCache RoomsCache
		{
			get
			{
				return this.roomsCache;
			}
			set
			{
				this.roomsCache = value;
			}
		}

		internal SendFromCache SendFromCache
		{
			get
			{
				return this.sendFromCache;
			}
			set
			{
				this.sendFromCache = value;
			}
		}

		internal SubscriptionCache SubscriptionCache
		{
			get
			{
				return this.subscriptionCache;
			}
			set
			{
				this.subscriptionCache = value;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				this.ThrowIfProxy();
				PolicyConfiguration policyConfiguration;
				ulong num;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					num = policyConfiguration.SegmentationFlags;
				}
				else
				{
					num = this.configuration.SegmentationFlags;
				}
				if (this.shouldDisableUncAndWssFeatures)
				{
					num &= 18446744073705619455UL;
				}
				if (this.IsExplicitLogonOthersMailbox || this.shouldDisableTextMessageFeatures)
				{
					num &= 18446744073441116159UL;
				}
				return num & 18446744073705619455UL;
			}
		}

		public ulong RestrictedCapabilitiesFlags
		{
			get
			{
				if (this.restrictedCapabilitiesFlags == 0UL)
				{
					if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UsePersistedCapabilities.Enabled)
					{
						this.restrictedCapabilitiesFlags = this.SegmentationFlags;
					}
					else if (this.MailboxIdentity != null)
					{
						OWAMiniRecipient owaminiRecipient = this.MailboxIdentity.GetOWAMiniRecipient();
						if (owaminiRecipient[ADUserSchema.PersistedCapabilities] != null)
						{
							using (IEnumerator enumerator = Enum.GetValues(typeof(Feature)).GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object obj = enumerator.Current;
									ulong num = (ulong)obj;
									if (num != 18446744073709551615UL)
									{
										string text = string.Format("Owa{0}Restrictions", Enum.GetName(typeof(Feature), num));
										try
										{
											if (ExchangeRunspaceConfiguration.IsFeatureValidOnObject(text, owaminiRecipient))
											{
												this.restrictedCapabilitiesFlags |= num;
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
								goto IL_127;
							}
						}
						this.restrictedCapabilitiesFlags = this.SegmentationFlags;
					}
				}
				IL_127:
				return this.restrictedCapabilitiesFlags;
			}
		}

		public uint[] SegmentationBitsForJavascript
		{
			get
			{
				if (this.segmentationBitsForJavascript == null)
				{
					this.segmentationBitsForJavascript = Utilities.GetSegmentationBitsForJavascript(this);
				}
				return this.segmentationBitsForJavascript;
			}
		}

		public AttachmentPolicy AttachmentPolicy
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.AttachmentPolicy;
				}
				return this.configuration.AttachmentPolicy;
			}
		}

		public int DefaultClientLanguage
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.DefaultClientLanguage;
				}
				return this.configuration.DefaultClientLanguage;
			}
		}

		public int LogonAndErrorLanguage
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.LogonAndErrorLanguage;
				}
				return this.configuration.LogonAndErrorLanguage;
			}
		}

		public bool UseGB18030
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.UseGB18030;
				}
				return this.configuration.UseGB18030;
			}
		}

		public bool UseISO885915
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.UseISO885915;
				}
				return this.configuration.UseISO885915;
			}
		}

		internal bool IsBposUser { get; private set; }

		internal string LastRecipientSessionDCServerName { get; set; }

		internal OutboundCharsetOptions OutboundCharset
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.OutboundCharset;
				}
				return this.configuration.OutboundCharset;
			}
		}

		internal bool ShouldDisableUncAndWssFeatures
		{
			get
			{
				return this.shouldDisableUncAndWssFeatures;
			}
			set
			{
				this.shouldDisableUncAndWssFeatures = value;
			}
		}

		internal PendingRequestManager PendingRequestManager
		{
			get
			{
				return this.pendingRequestManager;
			}
		}

		internal OwaMapiNotificationManager MapiNotificationManager
		{
			get
			{
				this.ThrowIfProxy();
				if (this.mapiNotificationManager == null)
				{
					this.mapiNotificationManager = new OwaMapiNotificationManager(this);
				}
				return this.mapiNotificationManager;
			}
		}

		public string Canary
		{
			get
			{
				return base.Key.Canary.ToString();
			}
		}

		internal bool ArchiveAccessed
		{
			get
			{
				return this.archiveAccessed;
			}
		}

		internal bool HasArchive
		{
			get
			{
				bool result = false;
				if (this.exchangePrincipal != null)
				{
					IRecipientSession recipientSession = Utilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.PartiallyConsistent, false, this, false);
					ADUser aduser = (ADUser)recipientSession.Read<ADUser>(this.ExchangePrincipal.ObjectId);
					if (aduser != null)
					{
						result = (aduser.ArchiveState == ArchiveState.Local || aduser.ArchiveState == ArchiveState.HostedProvisioned);
					}
				}
				return result;
			}
		}

		internal string ArchiveMailboxOwnerLegacyDN
		{
			get
			{
				string result = null;
				IMailboxInfo archiveMailbox = this.exchangePrincipal.GetArchiveMailbox();
				if (this.HasArchive && archiveMailbox != null)
				{
					result = this.exchangePrincipal.LegacyDn + "/guid=" + archiveMailbox.MailboxGuid;
				}
				return result;
			}
		}

		internal string ArchiveMailboxDisplayName
		{
			get
			{
				string result = null;
				IMailboxInfo archiveMailbox = this.exchangePrincipal.GetArchiveMailbox();
				if (this.exchangePrincipal != null && archiveMailbox != null)
				{
					result = archiveMailbox.ArchiveName;
				}
				return result;
			}
		}

		public void CommitRecipientCaches()
		{
			if (this.AutoCompleteCache != null)
			{
				this.AutoCompleteCache.Commit(true);
			}
			if (this.RoomsCache != null)
			{
				this.RoomsCache.Commit(true);
			}
			if (this.SendFromCache != null)
			{
				this.SendFromCache.Commit(true);
			}
		}

		internal bool HasValidMailboxSession()
		{
			return null != this.mailboxSession;
		}

		internal void CleanupOnEndRequest()
		{
			this.ThrowIfNotHoldingLock();
			this.ClearAllSessionHandles();
			this.DisconnectAllSessions();
		}

		internal void DisconnectAllSessions()
		{
			this.ThrowIfNotHoldingLock();
			this.DisconnectMailboxSession();
			if (this.alternateMailboxSessionManager != null)
			{
				this.alternateMailboxSessionManager.DisconnectAllSessions();
			}
			if (this.publicFolderSessionCache != null)
			{
				this.publicFolderSessionCache.DisconnectAllSessions();
			}
		}

		internal UserContextLoadResult Load(OwaContext owaContext)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug<UserContext>(0L, "UserContext.Load, User context instance={0}", this);
			this.isExplicitLogon = owaContext.IsExplicitLogon;
			this.isDifferentMailbox = owaContext.IsDifferentMailbox;
			this.clientBrowserStatus = Utilities.GetClientBrowserStatus(owaContext.HttpContext.Request.Browser);
			this.isProxy = false;
			if (!FormsRegistryManager.IsLoaded)
			{
				throw new OwaInvalidOperationException("Forms registry hasn't been loaded", null, this);
			}
			this.isOWAEnabled = owaContext.ExchangePrincipal.MailboxInfo.Configuration.IsOwaEnabled;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, owaContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 1174, "Load", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\UserContext.cs");
			ADRecipient adrecipient = DirectoryHelper.ReadADRecipient(owaContext.ExchangePrincipal.MailboxInfo.MailboxGuid, owaContext.ExchangePrincipal.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession);
			if (adrecipient != null)
			{
				this.isHiddenUser = adrecipient.HiddenFromAddressListsEnabled;
			}
			this.owaMailboxPolicy = owaContext.ExchangePrincipal.MailboxInfo.Configuration.OwaMailboxPolicy;
			ADObjectId adobjectId;
			if (OwaSegmentationSettings.UpdateOwaMailboxPolicy(owaContext.ExchangePrincipal.MailboxInfo.OrganizationId, this.owaMailboxPolicy, out adobjectId))
			{
				this.owaMailboxPolicy = adobjectId;
			}
			if (this.owaMailboxPolicy == null)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug(0L, "No OwaMailboxPolicy available applied to this user");
			}
			else
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<string, Guid, string>(0L, "OwaMailboxPolicy  applied to this user. Policy name = {0}, id = {1}, DN = {2}", this.owaMailboxPolicy.Name, this.owaMailboxPolicy.ObjectGuid, this.owaMailboxPolicy.DistinguishedName);
			}
			try
			{
				this.mailboxSession = this.CreateMailboxSession(owaContext);
			}
			catch
			{
				PerformanceCounterManager.AddStoreLogonResult(false);
				throw;
			}
			if (this.mailboxSession != null)
			{
				PerformanceCounterManager.AddStoreLogonResult(true);
				if (this.CanActAsOwner)
				{
					using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(this.mailboxSession))
					{
						TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(null);
						if (textMessagingAccount.TextMessagingSettings.PersonToPersonPreferences.Count != 0)
						{
							this.shouldDisableTextMessageFeatures = false;
						}
						goto IL_200;
					}
				}
				this.shouldDisableTextMessageFeatures = true;
				ExTraceGlobals.UserContextTracer.TraceDebug(0L, "No permission to read the text message configuration setting.");
			}
			else
			{
				PerformanceCounterManager.AddStoreLogonResult(false);
			}
			IL_200:
			if (this.userOptions == null)
			{
				this.userOptions = new UserOptions(this);
			}
			bool flag = true;
			try
			{
				this.userOptions.LoadAll();
			}
			catch (QuotaExceededException ex)
			{
				ExTraceGlobals.UserContextCallTracer.TraceDebug<string>(0L, "UserContext.Load: userOptions.LoadAll failed. Exception: {0}", ex.Message);
				flag = false;
			}
			string timeZoneName = this.userOptions.TimeZone;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneName, out this.timeZone))
			{
				ExTraceGlobals.UserContextTracer.TraceDebug(0L, "The timezone id in the user options is not valid");
				if (this.Configuration.DefaultClientLanguage <= 0 && !this.IsWebPartRequest && flag)
				{
					if (this.mapiNotificationManager != null)
					{
						this.mapiNotificationManager.Dispose();
						this.mapiNotificationManager = null;
					}
					this.ClearAllSessionHandles();
					this.DisconnectMailboxSession();
					this.mailboxSession.Dispose();
					this.mailboxSession = null;
					return UserContextLoadResult.InvalidTimeZoneKeyName;
				}
				this.timeZone = ExTimeZone.CurrentTimeZone;
			}
			this.TimeZone = this.timeZone;
			RequestDispatcherUtilities.LookupExperiencesForRequest(owaContext, this.userOptions.IsOptimizedForAccessibility, this.IsFeatureEnabled(Feature.RichClient), out this.browserType, out this.browserVersion, out this.experiences);
			if (this.experiences == null || this.experiences.Length == 0)
			{
				throw new OwaClientNotSupportedException("FormsRegistryManager.LookupExperiences couldn't find any experience for this client.", null, this);
			}
			this.browserPlatform = Utilities.GetBrowserPlatform(owaContext.HttpContext.Request.UserAgent);
			this.isMonitoringRequest = UserAgentUtilities.IsMonitoringRequest(owaContext.HttpContext.Request.UserAgent);
			if (!this.IsBasicExperience || this.IsFeatureEnabled(Feature.OWALight))
			{
				this.LoadUserTheme();
				this.messageViewFirstRender = true;
				this.lastClientViewState = new DefaultClientViewState();
				this.mailboxOwnerLegacyDN = this.MailboxSession.MailboxOwnerLegacyDN;
				OWAMiniRecipient owaminiRecipient = owaContext.LogonIdentity.GetOWAMiniRecipient();
				this.LastRecipientSessionDCServerName = owaContext.LogonIdentity.LastRecipientSessionDCServerName;
				this.IsBposUser = CapabilityHelper.HasBposSKUCapability(owaminiRecipient.PersistedCapabilities);
				if (this.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
				{
					this.sipUri = InstantMessageUtilities.GetSipUri(owaminiRecipient);
				}
				this.mobilePhoneNumber = owaminiRecipient.MobilePhoneNumber;
				this.isLoaded = true;
				return UserContextLoadResult.Success;
			}
			if (!this.IsFeatureEnabled(Feature.RichClient))
			{
				throw new OwaDisabledException();
			}
			if (this.userOptions.IsOptimizedForAccessibility || RequestDispatcherUtilities.IsLayoutParameterForLight(owaContext.HttpContext.Request))
			{
				throw new OwaLightDisabledException();
			}
			throw new OwaBrowserUpdateRequiredException(this.browserPlatform);
		}

		internal bool IsFullyInitialized
		{
			get
			{
				return this.isFullyIntialized;
			}
		}

		internal void SetFullyInitialized()
		{
			this.isFullyIntialized = true;
		}

		public bool IsSignedOutOfIM()
		{
			return InstantMessageUtilities.IsSignedOut(this);
		}

		public void SaveSignedOutOfIMStatus()
		{
			InstantMessageUtilities.SetSignedOutFlag(this, true);
		}

		public void SaveSignedInToIMStatus()
		{
			InstantMessageUtilities.SetSignedOutFlag(this, false);
		}

		internal void RecreateMailboxSession(OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (this.mailboxSession == null)
			{
				throw new InvalidOperationException("Cannot call RecreateMailboxSession if mailboxSession is null");
			}
			this.timeZone = this.mailboxSession.ExTimeZone;
			if (this.mapiNotificationManager != null)
			{
				this.mapiNotificationManager.Dispose();
				this.mapiNotificationManager = null;
			}
			this.ClearAllSessionHandles();
			this.DisconnectMailboxSession();
			this.mailboxSession.Dispose();
			this.mailboxSession = this.CreateMailboxSession(owaContext);
			this.mailboxSession.ExTimeZone = this.timeZone;
		}

		internal void RecreatePublicFolderSessions()
		{
			if (this.publicFolderSessionCache != null)
			{
				this.publicFolderSessionCache.Dispose();
				this.publicFolderSessionCache = null;
			}
		}

		public bool IsPublicRequest(HttpRequest request)
		{
			return UserContextUtilities.IsPublicRequest(request);
		}

		private MailboxSession CreateMailboxSession(OwaContext owaContext)
		{
			MailboxSession mailboxSession;
			if (!this.IsWebPartRequest)
			{
				mailboxSession = owaContext.LogonIdentity.CreateMailboxSession(owaContext.ExchangePrincipal, Thread.CurrentThread.CurrentCulture, owaContext.HttpContext.Request);
			}
			else
			{
				mailboxSession = owaContext.LogonIdentity.CreateWebPartMailboxSession(owaContext.ExchangePrincipal, Thread.CurrentThread.CurrentCulture, owaContext.HttpContext.Request);
			}
			this.canActAsOwner = mailboxSession.CanActAsOwner;
			return mailboxSession;
		}

		private void LoadUserTheme()
		{
			if (this.IsBasicExperience)
			{
				this.theme = ThemeManager.BaseTheme;
				return;
			}
			if (!this.IsFeatureEnabled(Feature.Themes))
			{
				this.theme = this.DefaultTheme;
				return;
			}
			if (string.IsNullOrEmpty(this.userOptions.ThemeStorageId))
			{
				this.theme = this.DefaultTheme;
				return;
			}
			uint idFromStorageId = ThemeManager.GetIdFromStorageId(this.userOptions.ThemeStorageId);
			if (idFromStorageId == 4294967295U)
			{
				this.theme = this.DefaultTheme;
				return;
			}
			this.theme = ThemeManager.Themes[(int)((UIntPtr)idFromStorageId)];
		}

		public void OnPostLoadUserContext()
		{
			this.RefreshIsJunkEmailEnabled();
			this.GetAllFolderPolicy();
			this.GetMailboxQuotaLimits();
		}

		internal ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.exchangePrincipal;
			}
			set
			{
				this.exchangePrincipal = value;
			}
		}

		internal void UpdateDisplayPictureCanary()
		{
			this.displayPictureChangeTime = DateTime.UtcNow.ToBinary();
		}

		internal string DisplayPictureCanary
		{
			get
			{
				return this.displayPictureChangeTime.ToString("X");
			}
		}

		internal byte[] UploadedDisplayPicture
		{
			get
			{
				return this.uploadedDisplayPicture;
			}
			set
			{
				this.uploadedDisplayPicture = value;
			}
		}

		internal bool? HasPicture
		{
			get
			{
				return this.hasPicture;
			}
			set
			{
				this.hasPicture = value;
			}
		}

		private void ThrowIfProxy()
		{
			if (this.IsProxy)
			{
				throw new OwaInvalidOperationException("Operation not allowed in a proxy user context");
			}
		}

		internal bool IsSafeToAccessFromCurrentThread()
		{
			return !this.isLoaded || base.LockedByCurrentThread();
		}

		private void ThrowIfNotHoldingLock()
		{
			if (!this.IsSafeToAccessFromCurrentThread())
			{
				throw new InvalidOperationException("Attempted to use MailboxSession in a thread that wasn't holding the UC lock");
			}
		}

		public Configuration Configuration
		{
			get
			{
				return this.configuration;
			}
			set
			{
				this.configuration = value;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				this.ThrowIfNotHoldingLock();
				this.ThrowIfProxy();
				Utilities.ReconnectStoreSession(this.mailboxSession, this);
				if (this.mailboxSession.ItemBinder == null)
				{
					this.mailboxSession.ItemBinder = this.ItemBinder;
				}
				return this.mailboxSession;
			}
		}

		internal PublicFolderSession DefaultPublicFolderSession
		{
			get
			{
				this.ThrowIfNotHoldingLock();
				this.ThrowIfProxy();
				if (!this.IsFeatureEnabled(Feature.PublicFolders))
				{
					throw new OwaSegmentationException("Public Folder feature is disabled");
				}
				PublicFolderSession publicFolderHierarchySession = this.PublicFolderSessionCache.GetPublicFolderHierarchySession();
				Utilities.ReconnectStoreSession(publicFolderHierarchySession, this);
				return publicFolderHierarchySession;
			}
		}

		internal bool IsPublicFoldersAvailable()
		{
			try
			{
				return this.DefaultPublicFolderSession != null;
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
			catch (OwaSegmentationException)
			{
			}
			return false;
		}

		internal PublicFolderViewStatesCache PublicFolderViewStatesCache
		{
			get
			{
				return this.publicFolderViewStatesCache;
			}
			set
			{
				this.publicFolderViewStatesCache = value;
			}
		}

		internal FolderViewStates GetFolderViewStates(Folder folder)
		{
			if (this.IsInMyMailbox(folder))
			{
				return new MailboxFolderViewStates(folder);
			}
			return new PublicFolderViewStates(this, folder);
		}

		internal PublicFolderSession GetContentAvailableSession(StoreObjectId publicFolderStoreObjectId)
		{
			if (publicFolderStoreObjectId == null)
			{
				throw new ArgumentNullException("publicFolderStoreObjectId");
			}
			if (!this.IsFeatureEnabled(Feature.PublicFolders))
			{
				throw new OwaSegmentationException("Public Folder feature is disabled");
			}
			Utilities.ReconnectStoreSession(this.PublicFolderSessionCache.GetPublicFolderHierarchySession(), this);
			PublicFolderSession publicFolderSession = this.PublicFolderSessionCache.GetPublicFolderSession(publicFolderStoreObjectId);
			Utilities.ReconnectStoreSession(publicFolderSession, this);
			return publicFolderSession;
		}

		internal DelegateSessionHandle GetDelegateSessionHandle(ExchangePrincipal exchangePrincipal)
		{
			return this.MailboxSession.GetDelegateSessionHandle(exchangePrincipal);
		}

		internal MailboxSession GetArchiveMailboxSession(string mailboxOwnerLegacyDN)
		{
			ExchangePrincipal principal = this.AlternateMailboxSessionManager.GetExchangePrincipal(mailboxOwnerLegacyDN);
			return this.GetArchiveMailboxSession(principal);
		}

		internal MailboxSession GetArchiveMailboxSession(IExchangePrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (!principal.MailboxInfo.IsArchive)
			{
				throw new ArgumentException("principal is not for archive mailbox");
			}
			return this.AlternateMailboxSessionManager.GetMailboxSession(principal);
		}

		internal void TryLoopArchiveMailboxes(UserContext.DelegateWithMailboxSession doWithArchiveSession)
		{
			if (!this.IsExplicitLogon && !this.ExchangePrincipal.MailboxInfo.IsArchive && !this.ExchangePrincipal.MailboxInfo.IsAggregated)
			{
				IMailboxInfo archiveMailbox = this.ExchangePrincipal.GetArchiveMailbox();
				if (archiveMailbox != null)
				{
					ExchangePrincipal archiveExchangePrincipal = this.ExchangePrincipal.GetArchiveExchangePrincipal(RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
					Guid mailboxGuid = archiveExchangePrincipal.MailboxInfo.MailboxGuid;
					MailboxSession mailboxSession = null;
					try
					{
						mailboxSession = this.GetArchiveMailboxSession(archiveExchangePrincipal);
					}
					catch (MailboxCrossSiteFailoverException innerException)
					{
						string message = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1277698471), new object[]
						{
							mailboxGuid.ToString()
						});
						throw new OwaArchiveInTransitException(message, innerException);
					}
					catch (WrongServerException innerException2)
					{
						string message2 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1277698471), new object[]
						{
							mailboxGuid.ToString()
						});
						throw new OwaArchiveInTransitException(message2, innerException2);
					}
					catch (MailboxInTransitException innerException3)
					{
						string message3 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1277698471), new object[]
						{
							mailboxGuid.ToString()
						});
						throw new OwaArchiveInTransitException(message3, innerException3);
					}
					catch (StoragePermanentException innerException4)
					{
						string message4 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1277698471), new object[]
						{
							mailboxGuid.ToString()
						});
						throw new OwaArchiveNotAvailableException(message4, innerException4);
					}
					catch (StorageTransientException innerException5)
					{
						string message5 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1277698471), new object[]
						{
							mailboxGuid.ToString()
						});
						throw new OwaArchiveNotAvailableException(message5, innerException5);
					}
					if (mailboxSession != null)
					{
						this.archiveAccessed = true;
						doWithArchiveSession(mailboxSession);
					}
				}
			}
		}

		internal OwaStoreObjectId GetArchiveRootFolderId()
		{
			if (this.archiveRootFolderId != null)
			{
				return this.archiveRootFolderId;
			}
			OwaStoreObjectId archiveFolderId = null;
			this.TryLoopArchiveMailboxes(delegate(MailboxSession archiveSession)
			{
				StoreObjectId defaultFolderId = archiveSession.GetDefaultFolderId(DefaultFolderType.Root);
				archiveFolderId = OwaStoreObjectId.CreateFromSessionFolderId(this, archiveSession, defaultFolderId);
				this.archiveRootFolderId = archiveFolderId;
			});
			return archiveFolderId;
		}

		internal string GetArchiveRootFolderIdString()
		{
			string result = null;
			OwaStoreObjectId owaStoreObjectId = this.GetArchiveRootFolderId();
			if (owaStoreObjectId != null)
			{
				result = owaStoreObjectId.ToString();
			}
			return result;
		}

		private PublicFolderSessionCache PublicFolderSessionCache
		{
			get
			{
				if (this.publicFolderSessionCache == null)
				{
					this.publicFolderSessionCache = new PublicFolderSessionCache(this.ExchangePrincipal.MailboxInfo.OrganizationId, this.ExchangePrincipal, this.LogonIdentity.ClientSecurityContext, this.UserCulture, "Client=OWA;Action=PublicFolder", null, null, false);
				}
				return this.publicFolderSessionCache;
			}
		}

		private AlternateMailboxSessionManager AlternateMailboxSessionManager
		{
			get
			{
				if (this.alternateMailboxSessionManager == null)
				{
					this.alternateMailboxSessionManager = new AlternateMailboxSessionManager(this);
				}
				return this.alternateMailboxSessionManager;
			}
		}

		internal OwaNotificationManager NotificationManager
		{
			get
			{
				this.ThrowIfProxy();
				if (this.notificationManager == null)
				{
					this.notificationManager = new OwaNotificationManager();
				}
				return this.notificationManager;
			}
		}

		internal InstantMessageManager InstantMessageManager
		{
			get
			{
				this.ThrowIfProxy();
				if (this.PrivateIsInstantMessageEnabled(true) && this.instantMessageManager == null)
				{
					this.instantMessageManager = new InstantMessageManager(this);
				}
				return this.instantMessageManager;
			}
		}

		internal StoreObjectId TryGetMyDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			return this.MailboxSession.GetDefaultFolderId(defaultFolderType);
		}

		private StoreObjectId GetMyDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			return Utilities.GetDefaultFolderId(this.MailboxSession, defaultFolderType);
		}

		internal StoreObjectId CalendarFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.Calendar);
			}
		}

		public string CalendarFolderOwaIdString
		{
			get
			{
				return this.CalendarFolderOwaId.ToString();
			}
		}

		internal OwaStoreObjectId CalendarFolderOwaId
		{
			get
			{
				return OwaStoreObjectId.CreateFromMailboxFolderId(this.CalendarFolderId);
			}
		}

		internal StoreObjectId ContactsFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.Contacts);
			}
		}

		internal OwaStoreObjectId GetDeletedItemsFolderId(MailboxSession mailboxSession)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			return OwaStoreObjectId.CreateFromSessionFolderId(this, mailboxSession, defaultFolderId);
		}

		internal StoreObjectId DraftsFolderId
		{
			get
			{
				if (this.draftsFolderId == null)
				{
					this.draftsFolderId = this.GetMyDefaultFolderId(DefaultFolderType.Drafts);
				}
				return this.draftsFolderId;
			}
		}

		internal StoreObjectId InboxFolderId
		{
			get
			{
				if (this.inboxFolderId == null)
				{
					this.inboxFolderId = this.GetMyDefaultFolderId(DefaultFolderType.Inbox);
				}
				return this.inboxFolderId;
			}
		}

		internal StoreObjectId JunkEmailFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.JunkEmail);
			}
		}

		internal StoreObjectId JournalFolderId
		{
			get
			{
				StoreObjectId result;
				try
				{
					result = this.GetMyDefaultFolderId(DefaultFolderType.Journal);
				}
				catch (ObjectNotFoundException)
				{
					result = null;
				}
				return result;
			}
		}

		internal StoreObjectId NotesFolderId
		{
			get
			{
				StoreObjectId result;
				try
				{
					result = this.GetMyDefaultFolderId(DefaultFolderType.Notes);
				}
				catch (ObjectNotFoundException)
				{
					result = null;
				}
				return result;
			}
		}

		internal StoreObjectId OutboxFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.Outbox);
			}
		}

		internal StoreObjectId RemindersSearchFolderId
		{
			get
			{
				if (this.remindersSearchFolderId == null)
				{
					this.remindersSearchFolderId = this.GetMyDefaultFolderId(DefaultFolderType.Reminders);
				}
				return this.remindersSearchFolderId;
			}
		}

		internal OwaStoreObjectId RemindersSearchFolderOwaId
		{
			get
			{
				if (this.remindersSearchFolderId == null)
				{
					this.remindersSearchFolderId = this.GetMyDefaultFolderId(DefaultFolderType.Reminders);
				}
				return OwaStoreObjectId.CreateFromMailboxFolderId(this.remindersSearchFolderId);
			}
		}

		internal StoreObjectId GetRootFolderId(MailboxSession mailboxSession)
		{
			return mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
		}

		internal OwaStoreObjectId GetSearchFoldersId(MailboxSession mailboxSession)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			return OwaStoreObjectId.CreateFromSessionFolderId(this, mailboxSession, defaultFolderId);
		}

		internal StoreObjectId SentItemsFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.SentItems);
			}
		}

		internal StoreObjectId TasksFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.Tasks);
			}
		}

		internal OwaStoreObjectId TasksFolderOwaId
		{
			get
			{
				return OwaStoreObjectId.CreateFromMailboxFolderId(this.TasksFolderId);
			}
		}

		internal StoreObjectId FlaggedItemsAndTasksFolderId
		{
			get
			{
				return this.GetMyDefaultFolderId(DefaultFolderType.ToDoSearch);
			}
		}

		internal StoreObjectId PublicFolderRootId
		{
			get
			{
				return this.DefaultPublicFolderSession.GetIpmSubtreeFolderId();
			}
		}

		private StoreObjectId TryGetPublicFolderRootId()
		{
			try
			{
				return this.PublicFolderRootId;
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
			catch (OwaSegmentationException)
			{
			}
			return null;
		}

		internal string TryGetPublicFolderRootIdString()
		{
			StoreObjectId storeObjectId = this.TryGetPublicFolderRootId();
			if (storeObjectId != null)
			{
				return OwaStoreObjectId.CreateFromPublicFolderId(storeObjectId).ToString();
			}
			return null;
		}

		internal bool IsPublicFolderRootId(StoreObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			return folderId.Equals(this.TryGetPublicFolderRootId());
		}

		internal void ReloadUserSettings(OwaContext owaContext, UserSettings settingToReload)
		{
			try
			{
				NewNotification newItemNotify = this.userOptions.NewItemNotify;
				this.userOptions.ReloadAll();
				if (Utilities.IsFlagSet((int)settingToReload, 16))
				{
					this.MailboxSession.PreferedCulture.DateTimeFormat.ShortDatePattern = this.UserOptions.DateFormat;
					this.MailboxSession.PreferedCulture.DateTimeFormat.ShortTimePattern = this.UserOptions.TimeFormat;
					DateTimeUtilities.SetSessionTimeZone(this);
				}
				if (Utilities.IsFlagSet((int)settingToReload, 4))
				{
					this.workingHours = WorkingHours.CreateForSession(this.MailboxSession, this.TimeZone);
					this.calendarSettings = CalendarSettings.CreateForSession(this);
				}
				if (Utilities.IsFlagSet((int)settingToReload, 32))
				{
					CultureInfo cultureInfo = null;
					HttpCookie httpCookie = HttpContext.Current.Request.Cookies["mkt"];
					if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
					{
						cultureInfo = Culture.GetSupportedBrowserLanguage(httpCookie.Value);
					}
					if (cultureInfo == null)
					{
						IRecipientSession recipientSession = Utilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.PartiallyConsistent, false, this, false);
						ADUser aduser = (ADUser)recipientSession.Read<ADUser>(this.ExchangePrincipal.ObjectId);
						if (aduser == null)
						{
							throw new OwaADObjectNotFoundException();
						}
						cultureInfo = Culture.GetPreferredCulture(aduser, owaContext.UserContext);
					}
					if (cultureInfo != null)
					{
						this.UserCulture = cultureInfo;
						Culture.UpdateUserCulture(owaContext.UserContext, this.UserCulture, false);
					}
					else
					{
						ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UserContext.ReloadUserSettings: The user doesn't have a valid culture in AD.");
					}
				}
				if (Utilities.IsFlagSet((int)settingToReload, 1))
				{
					NewNotification newItemNotify2 = this.userOptions.NewItemNotify;
					if (newItemNotify2 == NewNotification.None && newItemNotify != newItemNotify2)
					{
						if (this.IsPushNotificationsEnabled)
						{
							this.MapiNotificationManager.UnsubscribeNewMail();
						}
						if (this.IsPullNotificationsEnabled)
						{
							this.NotificationManager.DisposeOwaLastEventAdvisor();
						}
					}
				}
			}
			catch (QuotaExceededException ex)
			{
				ExTraceGlobals.UserContextCallTracer.TraceDebug<string>(0L, "UserContext.ReloadUserSettings: userOptions.LoadAll failed. Exception: {0}", ex.Message);
			}
		}

		public UserOptions UserOptions
		{
			get
			{
				this.ThrowIfProxy();
				return this.userOptions;
			}
		}

		public WorkingHours WorkingHours
		{
			get
			{
				if (this.workingHours == null)
				{
					this.workingHours = WorkingHours.CreateForSession(this.MailboxSession, this.TimeZone);
				}
				return this.workingHours;
			}
		}

		public System.DayOfWeek WeekStartDay
		{
			get
			{
				return this.UserOptions.WeekStartDay;
			}
		}

		public string DateFormat
		{
			get
			{
				return this.UserOptions.DateFormat;
			}
		}

		public string GetWeekdayDateFormat(bool useFullWeekdayFormat)
		{
			return this.UserOptions.GetWeekdayDateFormat(useFullWeekdayFormat);
		}

		internal OwaDelegateSessionManager DelegateSessionManager
		{
			get
			{
				if (this.delegateSessionManager == null)
				{
					this.delegateSessionManager = new OwaDelegateSessionManager(this);
				}
				return this.delegateSessionManager;
			}
		}

		internal WorkingHours GetOthersWorkingHours(OwaStoreObjectId folderId)
		{
			if (!folderId.IsOtherMailbox)
			{
				throw new ArgumentException("folderId should belong to other's mailbox");
			}
			if (this.othersWorkingHours == null)
			{
				this.othersWorkingHours = new Dictionary<string, WorkingHours>();
			}
			string key = folderId.MailboxOwnerLegacyDN;
			WorkingHours workingHours;
			if (!this.othersWorkingHours.TryGetValue(key, out workingHours))
			{
				MailboxSession mailboxSession = (MailboxSession)folderId.GetSession(this);
				workingHours = WorkingHours.CreateForSession(mailboxSession, mailboxSession.ExTimeZone);
				this.othersWorkingHours.Add(key, workingHours);
			}
			return workingHours;
		}

		internal void AddSessionHandle(OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle)
		{
			if (owaStoreObjectIdSessionHandle == null)
			{
				return;
			}
			if (owaStoreObjectIdSessionHandle.HandleType != OwaStoreObjectIdType.OtherUserMailboxObject && owaStoreObjectIdSessionHandle.HandleType != OwaStoreObjectIdType.GSCalendar)
			{
				owaStoreObjectIdSessionHandle.Dispose();
				owaStoreObjectIdSessionHandle = null;
				return;
			}
			if (this.sessionHandles == null)
			{
				this.sessionHandles = new List<OwaStoreObjectIdSessionHandle>();
			}
			this.sessionHandles.Add(owaStoreObjectIdSessionHandle);
		}

		internal void ClearAllSessionHandles()
		{
			if (this.sessionHandles != null)
			{
				foreach (OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle in this.sessionHandles)
				{
					if (owaStoreObjectIdSessionHandle != null)
					{
						owaStoreObjectIdSessionHandle.Dispose();
					}
				}
				this.sessionHandles.Clear();
			}
		}

		public Experience[] Experiences
		{
			get
			{
				this.ThrowIfProxy();
				return this.experiences;
			}
		}

		internal CalendarSettings CalendarSettings
		{
			get
			{
				if (this.calendarSettings == null)
				{
					this.calendarSettings = CalendarSettings.CreateForSession(this);
				}
				return this.calendarSettings;
			}
		}

		public bool IsBasicExperience
		{
			get
			{
				return string.Compare("Basic", this.Experiences[0].Name, StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		public bool IsRtl
		{
			get
			{
				return Culture.IsRtl;
			}
		}

		public bool IsPhoneticNamesEnabled
		{
			get
			{
				PolicyConfiguration policyConfiguration;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					return policyConfiguration.PhoneticSupportEnabled;
				}
				return this.configuration.PhoneticSupportEnabled;
			}
		}

		public GlobalAddressListInfo GlobalAddressListInfo
		{
			get
			{
				this.globalAddressListInfo = DirectoryAssistance.GetGlobalAddressListInfo(this);
				return this.globalAddressListInfo;
			}
		}

		public AddressListInfo AllRoomsAddressBookInfo
		{
			get
			{
				if (this.allRoomsAddressBookInfo == null)
				{
					this.allRoomsAddressBookInfo = DirectoryAssistance.GetAllRoomsAddressBookInfo(this);
				}
				if (this.allRoomsAddressBookInfo.IsEmpty)
				{
					return null;
				}
				return this.allRoomsAddressBookInfo;
			}
		}

		public BrowserType BrowserType
		{
			get
			{
				return this.browserType;
			}
		}

		public BrowserPlatform BrowserPlatform
		{
			get
			{
				return this.browserPlatform;
			}
		}

		internal AddressListInfo LastUsedAddressBookInfo
		{
			get
			{
				return this.lastUsedAddressBookInfo;
			}
			set
			{
				this.lastUsedAddressBookInfo = value;
			}
		}

		public AddressBookBase GlobalAddressList
		{
			get
			{
				if (!this.isGlobalAddressListLoaded)
				{
					IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this);
					IRecipientSession recipientSession = Utilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, false, this, false);
					this.globalAddressList = AddressBookBase.GetGlobalAddressList(this.LogonIdentity.ClientSecurityContext, configurationSession, recipientSession, this.GlobalAddressListId);
					this.isGlobalAddressListLoaded = true;
				}
				return this.globalAddressList;
			}
		}

		public ADObjectId GlobalAddressListId
		{
			get
			{
				if (this.globalAddressListId == null)
				{
					IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this);
					this.globalAddressListId = DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(this.ExchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy, configurationSession);
				}
				return this.globalAddressListId;
			}
		}

		public IEnumerable<AddressBookBase> AllAddressLists
		{
			get
			{
				IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this);
				return AddressBookBase.GetAllAddressLists(this.LogonIdentity.ClientSecurityContext, configurationSession, this.exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy);
			}
		}

		public AddressBookBase AllRoomsAddressList
		{
			get
			{
				IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this);
				return AddressBookBase.GetAllRoomsAddressList(this.LogonIdentity.ClientSecurityContext, configurationSession, this.exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy);
			}
		}

		internal bool IsAutoCompleteSessionStarted
		{
			get
			{
				return this.isAutoCompleteSessionStarted;
			}
			set
			{
				this.isAutoCompleteSessionStarted = value;
			}
		}

		internal bool IsRoomsCacheSessionStarted
		{
			get
			{
				return this.isRoomsCacheSessionStarted;
			}
			set
			{
				this.isRoomsCacheSessionStarted = value;
			}
		}

		internal bool IsSendFromCacheSessionStarted
		{
			get
			{
				return this.isSendFromCacheSessionStarted;
			}
			set
			{
				this.isSendFromCacheSessionStarted = value;
			}
		}

		internal bool IsMruSessionStarted
		{
			get
			{
				return this.isMruSessionStarted;
			}
			set
			{
				this.isMruSessionStarted = value;
			}
		}

		public bool MessageViewFirstRender
		{
			get
			{
				return this.messageViewFirstRender;
			}
			set
			{
				this.messageViewFirstRender = value;
			}
		}

		public bool IsPublicLogon
		{
			get
			{
				if (this.isPublicLogon == null)
				{
					if (this.IsProxy)
					{
						this.isPublicLogon = new bool?(this.IsPublicRequest(HttpContext.Current.Request));
					}
					else
					{
						this.isPublicLogon = new bool?(UserContextUtilities.IsPublicLogon(this.ExchangePrincipal.MailboxInfo.OrganizationId, HttpContext.Current));
					}
				}
				return this.isPublicLogon.Value;
			}
		}

		public void UpdateLastUserRequestTime()
		{
			this.lastUserRequestTime = Globals.ApplicationTime;
			if (this.IsInstantMessageEnabled() && this.instantMessageManager != null)
			{
				this.instantMessageManager.ResetPresence();
			}
		}

		public long LastUserRequestTime
		{
			get
			{
				return this.lastUserRequestTime;
			}
		}

		public int RenderingFlags
		{
			get
			{
				return this.renderingFlags;
			}
			set
			{
				this.renderingFlags = value;
			}
		}

		internal OwaStoreObjectId SearchFolderId
		{
			get
			{
				return this.searchFolderId;
			}
			set
			{
				this.searchFolderId = value;
			}
		}

		public string LastSearchQueryFilter
		{
			get
			{
				return this.lastSearchQueryFilter;
			}
			set
			{
				this.lastSearchQueryFilter = value;
			}
		}

		internal SearchScope LastSearchScope
		{
			get
			{
				return this.searchScope;
			}
			set
			{
				this.searchScope = value;
			}
		}

		internal OwaStoreObjectId LastSearchFolderId
		{
			get
			{
				return this.lastSearchFolderId;
			}
			set
			{
				this.lastSearchFolderId = value;
			}
		}

		internal bool ForceNewSearch
		{
			get
			{
				return this.forceNewSearch;
			}
			set
			{
				this.forceNewSearch = value;
			}
		}

		public string LastUMCallId
		{
			get
			{
				return this.lastUMCallId;
			}
			set
			{
				this.lastUMCallId = value;
			}
		}

		public string LastUMPhoneNumber
		{
			get
			{
				return this.lastUMPhoneNumber;
			}
			set
			{
				this.lastUMPhoneNumber = value;
			}
		}

		public Theme Theme
		{
			get
			{
				this.ThrowIfProxy();
				return this.theme;
			}
			set
			{
				this.theme = value;
			}
		}

		public Theme DefaultTheme
		{
			get
			{
				this.ThrowIfProxy();
				PolicyConfiguration policyConfiguration;
				string text;
				if (this.TryGetPolicyConfigurationFromCache(out policyConfiguration))
				{
					text = policyConfiguration.DefaultTheme;
				}
				else
				{
					text = this.configuration.DefaultTheme;
				}
				if (this.defaultTheme == null || this.defaultTheme.StorageId != text)
				{
					this.defaultTheme = ThemeManager.GetDefaultTheme(text);
				}
				return this.defaultTheme;
			}
		}

		public ClientViewState LastClientViewState
		{
			get
			{
				return this.lastClientViewState;
			}
			set
			{
				this.lastClientViewState = value;
			}
		}

		public bool IsRemindersSessionStarted
		{
			get
			{
				return this.isRemindersSessionStarted;
			}
			set
			{
				this.isRemindersSessionStarted = value;
			}
		}

		public int RemindersTimeZoneOffset
		{
			get
			{
				return this.remindersTimeZoneOffset;
			}
			set
			{
				this.remindersTimeZoneOffset = value;
			}
		}

		public ColumnId DocumentsModuleSortedColumnId
		{
			get
			{
				return this.documentsSortedColumnId;
			}
			set
			{
				this.documentsSortedColumnId = value;
			}
		}

		public SortOrder DocumentModuleSortOrder
		{
			get
			{
				return this.documentsSortOrder;
			}
			set
			{
				this.documentsSortOrder = value;
			}
		}

		internal ComplianceReader ComplianceReader
		{
			get
			{
				if (this.complianceReader == null)
				{
					this.complianceReader = new ComplianceReader(this.exchangePrincipal.MailboxInfo.OrganizationId);
				}
				return this.complianceReader;
			}
		}

		public bool IsPublicFolderEnabled
		{
			get
			{
				Guid guid;
				return this.Configuration.IsPublicFoldersEnabledOnThisVdir && this.IsFeatureEnabled(Feature.PublicFolders) && PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(this.exchangePrincipal.MailboxInfo.OrganizationId, out guid);
			}
		}

		internal StoreSession.IItemBinder ItemBinder
		{
			get
			{
				if (this.itemBinder == null)
				{
					this.itemBinder = new OwaItemBinder(this);
				}
				return this.itemBinder;
			}
		}

		public void RenderThemeFileUrl(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeFileUrl(writer, themeFileId, this);
		}

		public void RenderThemeFileUrl(TextWriter writer, int themeFileIndex)
		{
			SessionContextUtilities.RenderThemeFileUrl(writer, themeFileIndex, this);
		}

		public void RenderThemeImage(StringBuilder builder, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImage(builder, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeImage(writer, themeFileId, this);
		}

		public void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImage(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderBaseThemeImage(writer, themeFileId, this);
		}

		public void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderBaseThemeImage(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageWithToolTip(writer, themeFileId, styleClass, this, extraAttributes);
		}

		public void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, Strings.IDs tooltipStringId, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageWithToolTip(writer, themeFileId, styleClass, tooltipStringId, this, extraAttributes);
		}

		public void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, this);
		}

		public void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass, bool renderBaseTheme)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, renderBaseTheme, this);
		}

		public void RenderThemeImageEnd(TextWriter writer, ThemeFileId themeFileId)
		{
			SessionContextUtilities.RenderThemeImageEnd(writer, themeFileId);
		}

		public string GetThemeFileUrl(ThemeFileId themeFileId)
		{
			return SessionContextUtilities.GetThemeFileUrl(themeFileId, this);
		}

		public void RenderCssFontThemeFileUrl(TextWriter writer)
		{
			SessionContextUtilities.RenderCssFontThemeFileUrl(writer, this);
		}

		public bool AreFeaturesNotRestricted(ulong features)
		{
			return this.MailboxIdentity == null || (this.RestrictedCapabilitiesFlags & features) == features;
		}

		public bool IsFeatureEnabled(Feature feature)
		{
			return this.AreFeaturesEnabled((ulong)feature);
		}

		public bool AreFeaturesEnabled(ulong features)
		{
			return (features & this.SegmentationFlags) == features && this.AreFeaturesNotRestricted(features);
		}

		public bool IsMobileSyncEnabled()
		{
			if (!this.IsFeatureEnabled(Feature.EasMobileOptions))
			{
				return false;
			}
			bool result = false;
			OWAMiniRecipient owaminiRecipient = this.MailboxIdentity.GetOWAMiniRecipient();
			if (owaminiRecipient != null)
			{
				result = owaminiRecipient.ActiveSyncEnabled;
			}
			return result;
		}

		public bool IsInstantMessageEnabled()
		{
			return this.PrivateIsInstantMessageEnabled(false);
		}

		public bool IsSenderPhotosFeatureEnabled(Feature feature)
		{
			return this.IsFeatureEnabled(feature) && !AppSettings.GetConfiguredValue<bool>("SenderPhotosDisabled", false);
		}

		private bool PrivateIsInstantMessageEnabled(bool trace)
		{
			bool flag;
			if (this.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
			{
				flag = (this.IsFeatureEnabled(Feature.InstantMessage) && InstantMessageOCSProvider.EndpointManager != null && this.sipUri != null && !this.IsExplicitLogon && !this.IsWebPartRequest);
				if (!flag && trace)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "UserContext.IsInstantMessageEnabled, IM is disabled for user {0}, IsFeatureEnabled = {1}, EndpointManager = {2}, SipUri = {3}, IsExplicitLogon = {4}, IsWebPart = {5}", new object[]
					{
						(this.mailboxOwnerLegacyDN != null) ? this.mailboxOwnerLegacyDN : "null",
						this.IsFeatureEnabled(Feature.InstantMessage),
						(InstantMessageOCSProvider.EndpointManager != null) ? "<object>" : "null",
						(this.sipUri != null) ? this.sipUri : "null",
						this.IsExplicitLogon,
						this.IsWebPartRequest
					});
				}
			}
			else
			{
				if (trace)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<string, InstantMessagingTypeOptions>(0L, "UserContext.IsInstantMessageEnabled, IM is disabled for user {0}, Instant Messaging type is not valid: {1}", (this.mailboxOwnerLegacyDN != null) ? this.mailboxOwnerLegacyDN : "null", this.InstantMessagingType);
				}
				flag = false;
			}
			return flag;
		}

		private bool GetIsClientSideDataCollectingEnabled()
		{
			Random random = new Random();
			double num = (double)random.Next(1, 100000) / 100000.0;
			double configuredValue = AppSettings.GetConfiguredValue<double>("ClientSideDataCollectSamplingProbability", 0.0);
			return num <= configuredValue;
		}

		public void RefreshIsJunkEmailEnabled()
		{
			if (this.IsFeatureEnabled(Feature.JunkEMail) && this.CanActAsOwner)
			{
				JunkEmailRule.JunkEmailStatus junkEmailRuleStatus = this.MailboxSession.GetJunkEmailRuleStatus();
				if ((junkEmailRuleStatus & JunkEmailRule.JunkEmailStatus.IsPresent) != JunkEmailRule.JunkEmailStatus.None && (junkEmailRuleStatus & JunkEmailRule.JunkEmailStatus.IsEnabled) != JunkEmailRule.JunkEmailStatus.None)
				{
					this.isJunkEmailEnabled = true;
					return;
				}
			}
			this.isJunkEmailEnabled = false;
		}

		public long UpdateUsedQuota()
		{
			this.lastQuotaUpdateTime = Globals.ApplicationTime;
			this.MailboxSession.Mailbox.ForceReload(new PropertyDefinition[]
			{
				MailboxSchema.QuotaUsedExtended
			});
			long num = 0L;
			object obj = this.MailboxSession.Mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended);
			if (!(obj is PropertyError))
			{
				num = (long)obj;
			}
			if (num >= this.quotaWarning || num >= this.quotaSend)
			{
				this.isQuotaAboveWarning = true;
			}
			else
			{
				this.isQuotaAboveWarning = false;
			}
			return num;
		}

		private void GetMailboxQuotaLimits()
		{
			this.quotaSend = 0L;
			object obj = this.mailboxSession.Mailbox.TryGetProperty(MailboxSchema.QuotaProhibitSend);
			if (!(obj is PropertyError))
			{
				this.quotaSend = (long)((int)obj) * 1024L;
			}
			this.quotaWarning = 0L;
			obj = this.mailboxSession.Mailbox.TryGetProperty(MailboxSchema.StorageQuotaLimit);
			if (!(obj is PropertyError))
			{
				this.quotaWarning = (long)((int)obj) * 1024L;
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				this.ThrowIfProxy();
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
				this.MailboxSession.ExTimeZone = value;
				if (this.alternateMailboxSessionManager != null)
				{
					this.alternateMailboxSessionManager.UpdateTimeZoneOnAllSessions(value);
				}
				if (this.publicFolderSessionCache != null)
				{
					this.publicFolderSessionCache.UpdateTimeZoneOnAllSessions(value);
				}
			}
		}

		public OwaIdentity LogonIdentity
		{
			get
			{
				return this.logonIdentity;
			}
			internal set
			{
				this.logonIdentity = value;
			}
		}

		public OwaIdentity MailboxIdentity
		{
			get
			{
				return this.mailboxIdentity;
			}
			internal set
			{
				this.mailboxIdentity = value;
			}
		}

		public bool IsJunkEmailEnabled
		{
			get
			{
				return this.isJunkEmailEnabled;
			}
		}

		public RequestExecution RequestExecution
		{
			get
			{
				return this.requestExecution;
			}
			internal set
			{
				this.requestExecution = value;
			}
		}

		internal ProxyUriQueue ProxyUriQueue
		{
			get
			{
				return this.proxyUriQueue;
			}
			set
			{
				this.proxyUriQueue = value;
			}
		}

		internal UserContextCookie ProxyUserContextCookie
		{
			get
			{
				return this.proxyUserContextCookie;
			}
			set
			{
				this.proxyUserContextCookie = value;
			}
		}

		public bool IsProxy
		{
			get
			{
				return this.isProxy;
			}
		}

		public bool IsWebPartRequest
		{
			get
			{
				OwaContext owaContext = OwaContext.Current;
				if (owaContext != null)
				{
					if (owaContext.RequestType == OwaRequestType.WebPart || owaContext.IsProxyWebPart)
					{
						this.isWebPartRequest = true;
					}
					else if (owaContext.RequestType == OwaRequestType.Form14)
					{
						ApplicationElement applicationElement = RequestDispatcherUtilities.GetApplicationElement(owaContext.HttpContext.Request);
						if (applicationElement == ApplicationElement.Folder || applicationElement == ApplicationElement.StartPage)
						{
							this.isWebPartRequest = false;
						}
					}
				}
				return this.isWebPartRequest;
			}
		}

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

		public bool IsExplicitLogonOthersMailbox
		{
			get
			{
				return this.isExplicitLogon && this.MailboxIdentity != null && this.LogonIdentity != null && !this.MailboxIdentity.IsPartial && !this.LogonIdentity.IsEqualsTo(this.MailboxIdentity);
			}
		}

		public bool IsDifferentMailbox
		{
			get
			{
				return this.isDifferentMailbox;
			}
		}

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

		public bool AllFoldersPolicyMustDisplayComment
		{
			get
			{
				return this.allFoldersPolicyMustDisplayComment;
			}
		}

		public string AllFoldersPolicyComment
		{
			get
			{
				return this.allFoldersPolicyComment;
			}
		}

		public long QuotaSend
		{
			get
			{
				return this.quotaSend;
			}
		}

		public long QuotaWarning
		{
			get
			{
				return this.quotaWarning;
			}
		}

		public long LastQuotaUpdateTime
		{
			get
			{
				return this.lastQuotaUpdateTime;
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

		public bool IsQuotaAboveWarning
		{
			get
			{
				return this.isQuotaAboveWarning;
			}
		}

		public object WorkingData
		{
			get
			{
				return this.workingData;
			}
			set
			{
				this.workingData = value;
			}
		}

		public bool IsOWAEnabled
		{
			get
			{
				return this.isOWAEnabled;
			}
		}

		public bool IsHiddenUser
		{
			get
			{
				return this.isHiddenUser;
			}
		}

		public CultureInfo UserCulture
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

		public string PreferredDC
		{
			get
			{
				return this.preferredDC;
			}
			set
			{
				this.preferredDC = value;
			}
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
			set
			{
				this.sipUri = value;
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return this.mobilePhoneNumber;
			}
			set
			{
				this.mobilePhoneNumber = value;
			}
		}

		public bool HideMailTipsByDefault
		{
			get
			{
				return this.UserOptions.HideMailTipsByDefault;
			}
		}

		public bool ShowWeekNumbers
		{
			get
			{
				return this.UserOptions.ShowWeekNumbers;
			}
		}

		public CalendarWeekRule FirstWeekOfYear
		{
			get
			{
				return ((FirstWeekRules)this.UserOptions.FirstWeekOfYear).ToCalendarWeekRule();
			}
		}

		public string TimeFormat
		{
			get
			{
				return this.UserOptions.TimeFormat;
			}
		}

		public int HourIncrement
		{
			get
			{
				return this.UserOptions.HourIncrement;
			}
		}

		internal MasterCategoryList GetMasterCategoryList()
		{
			return this.GetMasterCategoryList(false);
		}

		public string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		internal MasterCategoryList GetMasterCategoryList(bool reFetchCategories)
		{
			if (this.CanActAsOwner)
			{
				try
				{
					try
					{
						if (this.masterCategoryList == null || reFetchCategories)
						{
							this.masterCategoryList = this.MailboxSession.GetMasterCategoryList();
						}
					}
					catch (CorruptDataException)
					{
						this.MailboxSession.DeleteMasterCategoryList();
						this.masterCategoryList = this.MailboxSession.GetMasterCategoryList();
					}
					if (this.masterCategoryList.Count == 0)
					{
						this.masterCategoryList = this.MailboxSession.GetMasterCategoryList();
						this.AddDefaultCategories(this.masterCategoryList);
						this.masterCategoryList.Save();
					}
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "UserContext.GetMasterCategoryList: Failed. Exception: {0}", ex.Message);
				}
			}
			return this.masterCategoryList;
		}

		internal MasterCategoryList GetMasterCategoryList(OwaStoreObjectId folderId)
		{
			if (folderId != null && folderId.IsOtherMailbox)
			{
				return this.GetMasterCategoryList((MailboxSession)folderId.GetSession(this));
			}
			return this.GetMasterCategoryList();
		}

		internal MasterCategoryList GetMasterCategoryList(MailboxSession session)
		{
			MasterCategoryList masterCategoryList = null;
			if (this.othersCategories == null)
			{
				this.othersCategories = new Dictionary<string, MasterCategoryList>();
			}
			if (!this.othersCategories.TryGetValue(session.MailboxOwnerLegacyDN, out masterCategoryList))
			{
				try
				{
					masterCategoryList = session.GetMasterCategoryList();
				}
				catch (StoragePermanentException)
				{
					masterCategoryList = new MasterCategoryList(session);
					this.AddDefaultCategories(masterCategoryList);
				}
				catch (ArgumentNullException)
				{
					masterCategoryList = new MasterCategoryList(session);
					this.AddDefaultCategories(masterCategoryList);
				}
				this.othersCategories.Add(session.MailboxOwnerLegacyDN, masterCategoryList);
			}
			return masterCategoryList;
		}

		internal void AddDefaultCategories(MasterCategoryList masterCategoryList)
		{
			if (masterCategoryList == null)
			{
				throw new ArgumentNullException("masterCategoryList");
			}
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(-1273337485), 0, true));
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(-630217384), 1, true));
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(777220966), 3, true));
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(-784120797), 4, true));
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(-1899490322), 7, true));
			masterCategoryList.Add(Category.Create(LocalizedStrings.GetNonEncoded(-136944284), 9, true));
		}

		public void DangerousBeginUnlockedAction(bool serializedUnlockedAction, out int numLocksSuccessfullyUnlocked)
		{
			numLocksSuccessfullyUnlocked = 0;
			ExTraceGlobals.UserContextCallTracer.TraceDebug<UserContext>(0L, "UserContext.DangerousBeginUnlockedAction, User context instance={0}", this);
			if (this.mailboxSession != null && this.mailboxSession.IsConnected)
			{
				this.ClearAllSessionHandles();
				this.mailboxSession.Disconnect();
			}
			int numberOfLocksHeld = base.NumberOfLocksHeld;
			for (numLocksSuccessfullyUnlocked = 0; numLocksSuccessfullyUnlocked < numberOfLocksHeld; numLocksSuccessfullyUnlocked++)
			{
				base.Unlock();
			}
			if (base.LockedByCurrentThread())
			{
				ExWatson.SendReport(new InvalidOperationException(string.Format("NumberOfLocksHeld value ({0}) was incorrect when used in DangerousBeginUnlockedAction. Did not unlock all locks.", numberOfLocksHeld)), ReportOptions.None, null);
				base.ForceReleaseLock();
			}
			if (serializedUnlockedAction && !Monitor.TryEnter(this.unlockedActionLock, Globals.UserContextLockTimeout))
			{
				throw new OwaLockTimeoutException("Attempt to acquire unlocked action lock timed out", null, this);
			}
		}

		public bool DangerousEndUnlockedAction(bool serializedUnlockedAction, int numLocksToRestore)
		{
			if (serializedUnlockedAction && Monitor.IsEntered(this.unlockedActionLock))
			{
				Monitor.Exit(this.unlockedActionLock);
			}
			ExTraceGlobals.UserContextCallTracer.TraceDebug<UserContext>(0L, "UserContext.DangerousEndUnlockedAction, User context instance={0}", this);
			for (int i = numLocksToRestore; i > 0; i--)
			{
				base.Lock();
			}
			return base.State == UserContextState.Active;
		}

		public void LogBreadcrumb(string message)
		{
			if (Globals.DisableBreadcrumbs)
			{
				return;
			}
			if (this.breadcrumbBuffer == null)
			{
				return;
			}
			this.breadcrumbBuffer.Add(new Breadcrumb(ExDateTime.UtcNow, (message != null) ? message : "<null>"));
		}

		internal BreadcrumbBuffer Breadcrumbs
		{
			get
			{
				return this.breadcrumbBuffer;
			}
		}

		public string DumpBreadcrumbs()
		{
			if (Globals.DisableBreadcrumbs)
			{
				return string.Empty;
			}
			if (this.breadcrumbBuffer == null)
			{
				return "<Breadcrumb buffer is null>";
			}
			StringBuilder stringBuilder = new StringBuilder(this.breadcrumbBuffer.Count * 128);
			stringBuilder.Append("OWA breadcrumbs:\r\n");
			for (int i = 0; i < this.breadcrumbBuffer.Count; i++)
			{
				Breadcrumb breadcrumb = this.breadcrumbBuffer[i];
				if (breadcrumb == null)
				{
					stringBuilder.AppendLine("<Found empty breadcrumb entry>");
				}
				stringBuilder.Append(breadcrumb.ToString());
			}
			return stringBuilder.ToString();
		}

		private void GetAllFolderPolicy()
		{
			if (this.CanActAsOwner)
			{
				using (UserConfiguration folderConfiguration = UserConfigurationUtilities.GetFolderConfiguration("ELC", this, this.InboxFolderId))
				{
					if (folderConfiguration != null)
					{
						IDictionary dictionary = folderConfiguration.GetDictionary();
						if (dictionary["elcComment"] != null)
						{
							this.allFoldersPolicyComment = (string)dictionary["elcComment"];
							if (dictionary["elcFlags"] != null)
							{
								this.allFoldersPolicyMustDisplayComment = (((int)dictionary["elcFlags"] & 4) > 0);
							}
						}
					}
				}
			}
		}

		public bool IsInternetExplorer7()
		{
			return this.browserType == BrowserType.IE && this.browserVersion.Build >= 7;
		}

		public void RenderBlankPage(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(this.GetBlankPage());
		}

		public void RenderBlankPage(string path, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			writer.Write(this.GetBlankPage(path));
		}

		internal string GetCachedFolderName(StoreObjectId folderId, MailboxSession mailboxSession)
		{
			if (this.folderNameCache == null)
			{
				this.folderNameCache = new Dictionary<StoreObjectId, string>(1);
			}
			string text = null;
			if (this.folderNameCache.TryGetValue(folderId, out text))
			{
				return text;
			}
			string result;
			using (Folder folder = Folder.Bind(mailboxSession, folderId, new PropertyDefinition[]
			{
				FolderSchema.DisplayName
			}))
			{
				text = folder.DisplayName;
				this.folderNameCache.Add(folderId, text);
				result = text;
			}
			return result;
		}

		internal void ClearFolderNameCache()
		{
			if (this.folderNameCache != null)
			{
				this.folderNameCache = null;
			}
		}

		internal int GetCachedADCount(string containerId, string searchString)
		{
			if (this.adCountCache == null)
			{
				throw new OwaInvalidOperationException("Attempted to get the cached AD count when it does not exist");
			}
			return this.adCountCache[containerId][(!string.IsNullOrEmpty(searchString)) ? searchString : string.Empty];
		}

		internal void SetCachedADCount(string containerId, string searchString, int totalCount)
		{
			if (this.adCountCache == null)
			{
				this.adCountCache = new Dictionary<string, Dictionary<string, int>>(1);
			}
			Dictionary<string, int> dictionary;
			if (!this.adCountCache.TryGetValue(containerId, out dictionary))
			{
				dictionary = new Dictionary<string, int>(1);
			}
			dictionary[(!string.IsNullOrEmpty(searchString)) ? searchString : string.Empty] = totalCount;
			this.adCountCache[containerId] = dictionary;
		}

		public override string ToString()
		{
			return this.GetHashCode().ToString();
		}

		internal long CalculateTimeout()
		{
			long num = (long)this.Configuration.SessionTimeout * 60L;
			long num2 = 4L * (long)this.Configuration.NotificationInterval;
			if (this.isMonitoringRequest)
			{
				num = 30L;
			}
			else if (num2 > 0L && num2 < num && !this.IsProxy && !this.IsWebPartRequest && this.IsFeatureEnabled(Feature.Notifications) && !this.IsBasicExperience)
			{
				num = num2;
			}
			this.timeout = num * 1000L;
			return this.timeout;
		}

		internal long Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		public ClientBrowserStatus ClientBrowserStatus
		{
			get
			{
				return this.clientBrowserStatus;
			}
		}

		private bool TryGetPolicyConfigurationFromCache(out PolicyConfiguration policyConfiguration)
		{
			policyConfiguration = null;
			if (this.owaMailboxPolicy == null)
			{
				return false;
			}
			policyConfiguration = OwaMailboxPolicyCache.Instance.Get(this.owaMailboxPolicy);
			return policyConfiguration != null;
		}

		internal bool IsInMyMailbox(StoreObject storeObject)
		{
			return this.IsMyMailbox(storeObject.Session);
		}

		internal bool IsMyMailbox(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			return mailboxSession != null && this.IsMyMailbox(mailboxSession);
		}

		internal bool IsInOtherMailbox(StoreObject storeObject)
		{
			return this.IsOtherMailbox(storeObject.Session);
		}

		internal bool IsOtherMailbox(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			return mailboxSession != null && !Utilities.IsArchiveMailbox(mailboxSession) && !this.IsMyMailbox(mailboxSession);
		}

		private bool IsMyMailbox(MailboxSession session)
		{
			return string.Equals(this.MailboxSession.MailboxOwnerLegacyDN, session.MailboxOwnerLegacyDN, StringComparison.OrdinalIgnoreCase);
		}

		public string DirectionMark
		{
			get
			{
				return this.GetDirectionMark();
			}
		}

		public void RenderCssLink(TextWriter writer, HttpRequest request, bool phase1Only)
		{
			SessionContextUtilities.RenderCssLink(writer, request, this, phase1Only);
		}

		public void RenderCssLink(TextWriter writer, HttpRequest request)
		{
			this.RenderCssLink(writer, request, false);
		}

		public void RenderOptionsCssLink(TextWriter writer, HttpRequest request)
		{
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderBaseThemeFileUrl(writer, ThemeFileId.PremiumCss);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderCssFontThemeFileUrl(writer, this.IsBasicExperience);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderBaseThemeFileUrl(writer, ThemeFileId.OptionsCss);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderBaseThemeFileUrl(writer, ThemeFileId.CssSpritesCss);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderBaseThemeFileUrl(writer, ThemeFileId.CssSpritesCss2);
			writer.Write("\">");
		}

		public bool IsSmsEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.TextMessage);
			}
		}

		internal bool IsIrmEnabled
		{
			get
			{
				if (!this.IsFeatureEnabled((Feature)(-2147483648)))
				{
					return false;
				}
				bool result;
				try
				{
					result = RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(this.exchangePrincipal.MailboxInfo.OrganizationId);
				}
				catch (ExchangeConfigurationException innerException)
				{
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, innerException);
				}
				catch (RightsManagementException ex)
				{
					if (ex.IsPermanent)
					{
						throw new RightsManagementPermanentException(ServerStrings.RmExceptionGenericMessage, ex);
					}
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, ex);
				}
				return result;
			}
		}

		internal void DisconnectMailboxSession()
		{
			if (this.mailboxSession != null)
			{
				Utilities.DisconnectStoreSession(this.mailboxSession);
			}
		}

		protected override void OnBeforeUnlock()
		{
			if (this.TerminationStatus == UserContextTerminationStatus.TerminatePending)
			{
				UserContextManager.TerminateSession(this, base.AbandonedReason);
			}
		}

		internal void AsyncAcquireIrmLicenses(OwaStoreObjectId messageId, string publishLicense, string requestCorrelator)
		{
			if (messageId == null)
			{
				throw new ArgumentNullException("messageId");
			}
			if (messageId.StoreObjectId == null)
			{
				throw new ArgumentNullException("messageId.StoreObjectId");
			}
			if (string.IsNullOrEmpty(publishLicense))
			{
				throw new ArgumentNullException("publishLicense");
			}
			if (!this.IsIrmEnabled || this.IsBasicExperience)
			{
				return;
			}
			if (this.MailboxSession.LogonType != LogonType.Owner)
			{
				return;
			}
			if (messageId.StoreObjectId.IsFakeId)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<UserContext, OwaStoreObjectId>(0L, "UserContext.AsyncAcquireIrmLicenses: Ignoring embedded item. User context instance={0}; MessageId={1}", this, messageId);
				return;
			}
			this.irmLicensingManager.AsyncAcquireLicenses(this.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId, messageId, publishLicense, this.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), this.MailboxSession.MailboxOwner.Sid, this.MailboxSession.MailboxOwner.RecipientTypeDetails, requestCorrelator);
		}

		private void ClearSearchFolders()
		{
			if (this.SearchFolderId != null)
			{
				Utilities.Delete(this, DeleteItemFlags.HardDelete, new OwaStoreObjectId[]
				{
					this.SearchFolderId
				});
			}
			FolderSearch.ClearSearchFolders(this.MailboxSession);
			if (this.alternateMailboxSessionManager != null)
			{
				this.alternateMailboxSessionManager.ClearAllSearchFolders();
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug<bool, UserContext>(0L, "UserContext.Dispose. IsDisposing: {0}, User context instance={1}", isDisposing, this);
			if (isDisposing && !this.isDisposed)
			{
				this.ThrowIfNotHoldingLock();
				if (this.mailboxSession != null)
				{
					try
					{
						this.mailboxSession.ItemBinder = null;
						if (this.publicFolderViewStatesCache != null)
						{
							try
							{
								this.publicFolderViewStatesCache.Commit();
							}
							catch (StoragePermanentException ex)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>((long)this.GetHashCode(), "Unable to save the public folder view states due to the exception {0}", ex.Message);
							}
							catch (StorageTransientException ex2)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>((long)this.GetHashCode(), "Unable to save the public folder view states due to the transient exception {0}", ex2.Message);
							}
						}
						if (this.isAutoCompleteSessionStarted)
						{
							try
							{
								AutoCompleteCache.FinishSession(this);
								RoomsCache.FinishSession(this);
								SendFromCache.FinishSession(this);
							}
							catch (StoragePermanentException ex3)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to finish autocomplete cache session due to exception {0}", ex3.Message);
							}
							catch (StorageTransientException ex4)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to finish autocomplete cache session due to exception {0}", ex4.Message);
							}
						}
						if (this.isMruSessionStarted)
						{
							try
							{
								FolderMruCache.FinishCacheSession(this);
							}
							catch (StoragePermanentException ex5)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to finish mru cache session due to exception {0}", ex5.Message);
							}
							catch (StorageTransientException ex6)
							{
								ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to finish mru cache session due to exception {0}", ex6.Message);
							}
						}
						try
						{
							this.ClearSearchFolders();
						}
						catch (StoragePermanentException ex7)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to remove search folder due to exception {0}", ex7.Message);
						}
						catch (StorageTransientException ex8)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to remove search folder due to exception {0}", ex8.Message);
						}
						catch (ResourceUnhealthyException ex9)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to remove search folder due to exception {0}", ex9.Message);
						}
						try
						{
							if (this.canActAsOwner)
							{
								JunkEmailRule junkEmailRule = this.MailboxSession.JunkEmailRule;
								if (junkEmailRule.IsContactsFolderTrusted && junkEmailRule.IsContactsCacheOutOfDate)
								{
									Utilities.JunkEmailRuleSynchronizeContactsCache(junkEmailRule);
									junkEmailRule.Save();
								}
							}
						}
						catch (StoragePermanentException ex10)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to synchronize contacts cache due to exception {0}", ex10.Message);
						}
						catch (StorageTransientException ex11)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to synchronize contacts cache due to exception {0}", ex11.Message);
						}
						catch (ADTransientException ex12)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to synchronize contacts cache due to exception {0}", ex12.Message);
						}
						try
						{
							if (this.canActAsOwner)
							{
								this.MailboxSession.SaveMasterCategoryList();
							}
						}
						catch (StoragePermanentException ex13)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to save master category list due to exception {0}", ex13.Message);
						}
						catch (StorageTransientException ex14)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to save master category list due to exception {0}", ex14.Message);
						}
					}
					finally
					{
						try
						{
							if (this.mapiNotificationManager != null)
							{
								this.mapiNotificationManager.Dispose();
								this.mapiNotificationManager = null;
							}
							this.ClearAllSessionHandles();
							this.DisconnectMailboxSession();
						}
						catch (StoragePermanentException ex15)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to disconnect mailbox session.  exception {0}", ex15.Message);
						}
						catch (StorageTransientException ex16)
						{
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to disconnect mailbox session.  exception {0}", ex16.Message);
						}
						finally
						{
							this.mailboxSession.Dispose();
							this.mailboxSession = null;
							this.isDisposed = true;
						}
					}
				}
				if (this.alternateMailboxSessionManager != null)
				{
					this.alternateMailboxSessionManager.Dispose();
					this.alternateMailboxSessionManager = null;
				}
				if (this.publicFolderSessionCache != null)
				{
					this.publicFolderSessionCache.Dispose();
					this.publicFolderSessionCache = null;
				}
				if (this.notificationManager != null)
				{
					this.notificationManager.Dispose();
					this.notificationManager = null;
				}
				if (this.instantMessageManager != null)
				{
					this.instantMessageManager.Dispose();
					this.instantMessageManager = null;
				}
				if (this.pendingRequestManager != null)
				{
					this.pendingRequestManager.Dispose();
					this.pendingRequestManager = null;
				}
				if (this.logonIdentity != null)
				{
					this.logonIdentity.Dispose();
					this.logonIdentity = null;
				}
				if (this.mailboxIdentity != null)
				{
					this.mailboxIdentity.Dispose();
					this.mailboxIdentity = null;
				}
				if (base.Key != null && base.Key.UserContextId != null)
				{
					TranscodingTaskManager.RemoveSession(base.Key.UserContextId);
				}
				if (this.workingData != null && this.workingData is IDisposable)
				{
					(this.workingData as IDisposable).Dispose();
					this.workingData = null;
				}
				if (this.delegateSessionManager != null)
				{
					this.delegateSessionManager.ClearAllExchangePrincipals();
				}
				if (this.othersWorkingHours != null)
				{
					this.othersWorkingHours.Clear();
					this.othersWorkingHours = null;
				}
				if (this.othersCategories != null)
				{
					this.othersCategories.Clear();
					this.othersCategories = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserContext>(this);
		}

		public bool IsReplyByPhoneEnabled
		{
			get
			{
				return false;
			}
		}

		public void RenderCustomizedFormRegistry(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (this.customizedFormRegistryCache == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Experience experience in this.Experiences)
				{
					foreach (FormKey formKey in experience.FormsRegistry.CustomizedFormKeys)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder.Append("{");
						if (formKey.Application != ApplicationElement.NotSet)
						{
							stringBuilder2.Append(string.Format("\"ae\":\"{0}\",", Utilities.JavascriptEncode(formKey.Application.ToString())));
						}
						if (!string.IsNullOrEmpty(formKey.Class))
						{
							stringBuilder2.Append(string.Format("\"t\":\"{0}\",", Utilities.JavascriptEncode(formKey.Class)));
						}
						if (!string.IsNullOrEmpty(formKey.Action))
						{
							stringBuilder2.Append(string.Format("\"a\":\"{0}\",", Utilities.JavascriptEncode(formKey.Action)));
						}
						if (!string.IsNullOrEmpty(formKey.State))
						{
							stringBuilder2.Append(string.Format("\"s\":\"{0}\",", Utilities.JavascriptEncode(formKey.State)));
						}
						if (stringBuilder2.Length > 0)
						{
							stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
						}
						stringBuilder.Append(stringBuilder2.ToString());
						stringBuilder.Append("} ,");
					}
				}
				this.customizedFormRegistryCache = SanitizedHtmlString.GetSanitizedStringWithoutEncoding(stringBuilder.ToString());
			}
			if (this.customizedFormRegistryCache != SanitizedHtmlString.Empty)
			{
				output.WriteLine("var a_rgCustomizedFormRegistry = [");
				output.Write(this.customizedFormRegistryCache);
				output.WriteLine("];");
			}
		}

		private const string AllFolderPolicyConfigurationMessageName = "ELC";

		private const ulong DisableUncAndWssFeatures = 18446744073705619455UL;

		private Experience[] experiences;

		private ExchangePrincipal exchangePrincipal;

		private MailboxSession mailboxSession;

		private OwaIdentity logonIdentity;

		private OwaIdentity mailboxIdentity;

		private string mailboxOwnerLegacyDN;

		private RequestExecution requestExecution;

		private UserContextCookie proxyUserContextCookie;

		private ProxyUriQueue proxyUriQueue;

		private UserOptions userOptions;

		private WorkingHours workingHours;

		private ExTimeZone timeZone;

		private Theme theme;

		private Theme defaultTheme;

		private bool isAutoCompleteSessionStarted;

		private bool isRoomsCacheSessionStarted;

		private bool isSendFromCacheSessionStarted;

		private bool isMruSessionStarted;

		private bool messageViewFirstRender = true;

		private OwaStoreObjectId searchFolderId;

		private string lastSearchQueryFilter;

		private SearchScope searchScope = SearchScope.AllFoldersAndItems;

		private OwaStoreObjectId lastSearchFolderId;

		private bool forceNewSearch;

		private bool isDisposed;

		private OwaNotificationManager notificationManager;

		private OwaMapiNotificationManager mapiNotificationManager;

		private InstantMessageManager instantMessageManager;

		private string lastUMCallId;

		private string lastUMPhoneNumber;

		private BrowserType browserType = BrowserType.Other;

		private BrowserPlatform browserPlatform = BrowserPlatform.Other;

		private UserAgentParser.UserAgentVersion browserVersion;

		private bool isMonitoringRequest;

		private bool isProxy;

		private bool isLoaded;

		private bool isFullyIntialized;

		private CultureInfo userCulture;

		private string preferredDC = string.Empty;

		private PublicFolderSessionCache publicFolderSessionCache;

		private PublicFolderViewStatesCache publicFolderViewStatesCache;

		private StoreSession.IItemBinder itemBinder;

		private ADObjectId owaMailboxPolicy;

		private AlternateMailboxSessionManager alternateMailboxSessionManager;

		private IrmLicensingManager irmLicensingManager;

		private long displayPictureChangeTime = DateTime.UtcNow.ToBinary();

		private byte[] uploadedDisplayPicture;

		private bool? hasPicture = null;

		private long timeout;

		private bool isOWAEnabled = true;

		private bool isHiddenUser;

		private object unlockedActionLock = new object();

		private long lastUserRequestTime = Globals.ApplicationTime;

		private StoreObjectId remindersSearchFolderId;

		private StoreObjectId inboxFolderId;

		private StoreObjectId draftsFolderId;

		private AddressBookBase globalAddressList;

		private ADObjectId globalAddressListId;

		private bool isGlobalAddressListLoaded;

		private GlobalAddressListInfo globalAddressListInfo;

		private AddressListInfo allRoomsAddressBookInfo;

		private AddressListInfo lastUsedAddressBookInfo;

		private int renderingFlags;

		private ClientViewState lastClientViewState;

		private bool isRemindersSessionStarted;

		private int remindersTimeZoneOffset;

		private SortOrder documentsSortOrder;

		private ColumnId documentsSortedColumnId = ColumnId.Count;

		private bool isJunkEmailEnabled;

		private bool isWebPartRequest;

		private bool isExplicitLogon;

		private bool isDifferentMailbox;

		private bool canActAsOwner = true;

		private long lastQuotaUpdateTime = Globals.ApplicationTime;

		private long signIntoIMTime = Globals.ApplicationTime;

		private bool allFoldersPolicyMustDisplayComment;

		private string allFoldersPolicyComment = string.Empty;

		private long quotaSend;

		private long quotaWarning;

		private bool isQuotaAboveWarning;

		private object workingData;

		private Configuration configuration = OwaConfigurationManager.Configuration;

		private BreadcrumbBuffer breadcrumbBuffer;

		private CalendarSettings calendarSettings;

		private ClientBrowserStatus clientBrowserStatus;

		private OwaDelegateSessionManager delegateSessionManager;

		private List<OwaStoreObjectIdSessionHandle> sessionHandles;

		private Dictionary<string, WorkingHours> othersWorkingHours;

		private Dictionary<string, MasterCategoryList> othersCategories;

		private MasterCategoryList masterCategoryList;

		private bool shouldDisableUncAndWssFeatures;

		private bool shouldDisableTextMessageFeatures = true;

		private string sipUri;

		private string mobilePhoneNumber;

		private PendingRequestManager pendingRequestManager;

		private PerformanceNotifier performanceNotifier;

		private MailTipsNotificationHandler mailTipsNotificationHandler;

		private bool isPerformanceConsoleOn;

		private bool isClientSideDataCollectingEnabled;

		private Dictionary<StoreObjectId, string> folderNameCache;

		private Dictionary<string, Dictionary<string, int>> adCountCache;

		private AutoCompleteCache autoCompleteCache;

		private RoomsCache roomsCache;

		private SendFromCache sendFromCache;

		private SubscriptionCache subscriptionCache;

		private bool isPushNotificationsEnabled = Globals.IsPushNotificationsEnabled;

		private bool isPullNotificationsEnabled = Globals.IsPullNotificationsEnabled;

		private bool shouldDisableEmbeddedReadingPane;

		private uint[] segmentationBitsForJavascript;

		private ulong restrictedCapabilitiesFlags;

		private ComplianceReader complianceReader;

		private bool archiveAccessed;

		private OwaStoreObjectId archiveRootFolderId;

		private bool? isPublicLogon;

		private SanitizedHtmlString customizedFormRegistryCache;

		internal delegate void DelegateWithMailboxSession(MailboxSession mailboxSession);
	}
}
