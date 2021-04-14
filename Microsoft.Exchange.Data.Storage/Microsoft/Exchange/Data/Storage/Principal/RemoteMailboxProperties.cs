using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoteMailboxProperties
	{
		public RemoteMailboxProperties(IStoreSession storeSession, IDirectoryAccessor directoryAccessor)
		{
			ArgumentValidator.ThrowIfNull("storeSession", storeSession);
			ArgumentValidator.ThrowIfNull("directoryAccessor", directoryAccessor);
			this.storeSession = storeSession;
			this.directoryAccessor = directoryAccessor;
		}

		public Uri GetRemoteEndpoint(SmtpAddress? remoteIdentity)
		{
			this.UpdateCrossPremiseStatus();
			if (this.autoDiscoveryTokenRequest == null || this.autoDiscoveryEndpoint == null || remoteIdentity == null)
			{
				throw new AutoDAccessException(ServerStrings.UnableToMakeAutoDiscoveryRequest);
			}
			SecurityTokenService securityTokenService;
			if (!this.TryGetSecurityTokenService(this.adUser.OrganizationId, out securityTokenService))
			{
				string text = remoteIdentity.Value.ToString();
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ExternalAuthDisabledAutoDiscover, text, new object[]
				{
					text
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "AutoDiscover request failed for {0}, external authentification is disabled.", text);
				throw new AutoDAccessException(ServerStrings.AutoDFailedToGetToken);
			}
			Uri result = null;
			if (!this.TryGetExchangeRpcUrlFromAutodiscoverSettings(remoteIdentity.Value, securityTokenService, this.autoDiscoveryTokenRequest, this.autoDiscoveryEndpoint, out result))
			{
				string text2 = remoteIdentity.Value.ToString();
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcAutoDiscoverRequestFailed, text2, new object[]
				{
					text2
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "AutoDiscover request failed - remote mailbox/archive access will be disabled. User: {0}.", text2);
				throw new AutoDAccessException(ServerStrings.AutoDRequestFailed);
			}
			return result;
		}

		public FederatedClientCredentials GetFederatedCredentialsForDelegation(ExternalAuthentication extAuth)
		{
			this.UpdateCrossPremiseStatus();
			if ((!this.isArchiveCrossPremise || !this.isCrossPremiseArchiveEnabled) && !this.isMailboxCrossPremise)
			{
				throw new InvalidOperationException("This is not an enabled remote mailbox.");
			}
			if (this.xropTokenRequest == null)
			{
				throw new InvalidOperationException("We shouldn't be requesting DelegationTokenRequest for misconfigured remote mailbox properties.");
			}
			FedOrgCredentials organizationCredentials = new FedOrgCredentials(this.xropTokenRequest, extAuth.GetSecurityTokenService(this.adUser.OrganizationId));
			return new FederatedClientCredentials(this.xropUserFederatedIdentity, this.xropUserEmailAddress, organizationCredentials);
		}

		protected virtual IGenericADUser GetADUser(string legacyDn, Guid mailboxGuid)
		{
			if (!string.IsNullOrEmpty(legacyDn))
			{
				return this.directoryAccessor.FindByLegacyExchangeDn(this.storeSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), legacyDn);
			}
			if (mailboxGuid == Guid.Empty)
			{
				throw new UserHasNoMailboxException();
			}
			return this.directoryAccessor.FindByExchangeGuid(this.storeSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), mailboxGuid, false);
		}

		protected virtual bool IsExternalAuthenticationEnabled()
		{
			return ExternalAuthentication.GetCurrent().Enabled;
		}

		protected virtual bool TryGetAutodiscoveryEndpoint(IGenericADUser user, string domain, out TokenTarget tokenTarget, out Uri autodiscoveryEndpoint)
		{
			tokenTarget = null;
			autodiscoveryEndpoint = null;
			OrganizationRelationship organizationRelationship = this.directoryAccessor.GetOrganizationRelationship(user.OrganizationId ?? OrganizationId.ForestWideOrgId, domain);
			if (this.CheckOrgRelationshipFromRemoteConnection(organizationRelationship, user, domain))
			{
				tokenTarget = organizationRelationship.GetTokenTarget();
				autodiscoveryEndpoint = organizationRelationship.TargetAutodiscoverEpr;
				return true;
			}
			return false;
		}

		protected virtual IGenericADUser GetOrganizationFederatedMailbox()
		{
			IGenericADUser result = null;
			try
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(this.directoryAccessor.GetOrganizationFederatedMailboxIdentity(this.storeSession.GetADConfigurationSession(true, ConsistencyMode.IgnoreInvalid)).ToString());
				result = this.directoryAccessor.FindByProxyAddress(this.storeSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), proxyAddress);
			}
			catch (ObjectNotFoundException)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_FederatedMailboxMisconfigured, string.Empty, new object[0]);
				ExTraceGlobals.XtcTracer.TraceError(0L, "Federated mailbox misconfigured for organization, remote mailbox access will be disabled.");
				throw;
			}
			return result;
		}

		protected virtual bool TryGetSecurityTokenService(OrganizationId organizationId, out SecurityTokenService securityTokenService)
		{
			securityTokenService = null;
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			if (current.Enabled)
			{
				securityTokenService = current.GetSecurityTokenService(organizationId);
			}
			return securityTokenService != null;
		}

		protected virtual bool TryGetExchangeRpcUrlFromAutodiscoverSettings(SmtpAddress remoteIdentity, SecurityTokenService securityTokenService, DelegationTokenRequest autoDiscoveryTokenRequest, Uri autoDiscoveryEndpoint, out Uri exchangeRpcUrl)
		{
			exchangeRpcUrl = null;
			FedOrgCredentials credentials = new FedOrgCredentials(autoDiscoveryTokenRequest, securityTokenService);
			bool result;
			using (AutoDiscoverUserSettingsClient autoDiscoverUserSettingsClient = AutoDiscoverUserSettingsClient.CreateInstance(DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.storeSession.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings(), 250, "TryGetExchangeRpcUrlFromAutodiscoverSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ExchangePrincipal\\RemoteMailboxProperties.cs"), credentials, remoteIdentity, autoDiscoveryEndpoint, RemoteMailboxProperties.AutodiscoveryRequestedSettings))
			{
				autoDiscoverUserSettingsClient.AnchorMailbox = this.storeSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
				UserSettings userSettings = autoDiscoverUserSettingsClient.Discover();
				StringSetting stringSetting = userSettings.GetSetting(RemoteMailboxProperties.AutodiscoveryRequestedSettings[0]) as StringSetting;
				result = (stringSetting != null && Uri.TryCreate(stringSetting.Value, UriKind.Absolute, out exchangeRpcUrl));
			}
			return result;
		}

		private void UpdateCrossPremiseStatus()
		{
			if (this.isRemotingStatusInitialized)
			{
				return;
			}
			if (this.adUser == null && VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.CheckForRemoteConnections.Enabled)
			{
				try
				{
					this.adUser = this.GetADUser(this.storeSession.MailboxOwner.LegacyDn, this.storeSession.MailboxOwner.MailboxInfo.MailboxGuid);
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			this.UpdateCrossPremiseStatus(this.adUser);
		}

		private void UpdateCrossPremiseStatus(IGenericADUser user)
		{
			if (this.isRemotingStatusInitialized)
			{
				return;
			}
			this.autoDiscoveryEndpoint = null;
			this.autoDiscoveryTokenRequest = null;
			this.xropUserFederatedIdentity = null;
			this.xropUserEmailAddress = null;
			this.xropTokenRequest = null;
			if (user == null || !VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.RepresentRemoteMailbox.Enabled)
			{
				this.isRemotingStatusInitialized = true;
				return;
			}
			if (user.RecipientType == RecipientType.MailUser && user.MailboxDatabase.IsNullOrEmpty() && (user.RecipientTypeDetails & (RecipientTypeDetails)((ulong)-2147483648)) == (RecipientTypeDetails)((ulong)-2147483648) && user.ExternalEmailAddress != null)
			{
				this.isMailboxCrossPremise = true;
				this.isArchiveCrossPremise = true;
			}
			if (!this.isMailboxCrossPremise && user.ArchiveDomain != null)
			{
				this.isArchiveCrossPremise = true;
			}
			if (!this.isMailboxCrossPremise && !this.isArchiveCrossPremise)
			{
				this.isRemotingStatusInitialized = true;
				return;
			}
			if (!this.IsExternalAuthenticationEnabled())
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ExternalAuthDisabledExchangePrincipal, user.LegacyDn, new object[]
				{
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "External authentification is disabled, remote mailbox/archive access for user {0} will be disabled.", user.LegacyDn);
				this.isRemotingStatusInitialized = true;
				return;
			}
			this.isCrossPremiseArchiveEnabled = ((user.ArchiveStatus & ArchiveStatusFlags.Active) == ArchiveStatusFlags.Active);
			if (this.isMailboxCrossPremise)
			{
				this.UpdateCrossPremiseStatusRemoteMailbox(user);
			}
			else if (this.isCrossPremiseArchiveEnabled)
			{
				this.UpdateCrossPremiseStatusRemoteArchive(user);
			}
			else
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ExternalArchiveDisabled, user.LegacyDn, new object[]
				{
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "Remote archive access is disabled for user {0}.", user.LegacyDn);
			}
			this.isRemotingStatusInitialized = true;
		}

		private void UpdateCrossPremiseStatusRemoteArchive(IGenericADUser user)
		{
			if (user.ArchiveGuid == Guid.Empty)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcArchiveGuidMissing, user.LegacyDn, new object[]
				{
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "User's archive GUID is not configured properly - remote archive access will be disabled. User: {0}.", user.LegacyDn);
				return;
			}
			TokenTarget tokenTarget;
			Uri autodiscoveryEndpoint;
			if (!this.TryGetAutodiscoveryEndpoint(user, user.ArchiveDomain.Domain, out tokenTarget, out autodiscoveryEndpoint))
			{
				return;
			}
			this.UpdateDelegationTokenRequest(tokenTarget, autodiscoveryEndpoint, user);
		}

		private void UpdateCrossPremiseStatusRemoteMailbox(IGenericADUser user)
		{
			if (user.ExternalEmailAddress.Prefix != ProxyAddressPrefix.Smtp || !SmtpAddress.IsValidSmtpAddress(user.ExternalEmailAddress.AddressString))
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcInvalidSmtpAddress, user.LegacyDn, new object[]
				{
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "User's external email address is not a valid SMTP address - remote mailbox/archive access will be disabled. User: {0}.", user.LegacyDn);
				return;
			}
			SmtpAddress smtpAddress = new SmtpAddress(user.ExternalEmailAddress.AddressString);
			TokenTarget tokenTarget;
			Uri autodiscoveryEndpoint;
			if (!this.TryGetAutodiscoveryEndpoint(user, smtpAddress.Domain, out tokenTarget, out autodiscoveryEndpoint))
			{
				return;
			}
			this.UpdateDelegationTokenRequest(tokenTarget, autodiscoveryEndpoint, user);
			if (this.isCrossPremiseArchiveEnabled && user.ArchiveGuid == Guid.Empty)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcArchiveGuidMissing, user.LegacyDn, new object[]
				{
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string>(0L, "User's archive GUID is not configured properly - remote archive access will be disabled. User: {0}.", user.LegacyDn);
			}
		}

		private void UpdateDelegationTokenRequest(TokenTarget tokenTarget, Uri autodiscoveryEndpoint, IGenericADUser user)
		{
			IGenericADUser organizationFederatedMailbox = this.GetOrganizationFederatedMailbox();
			this.xropUserFederatedIdentity = user.GetFederatedIdentity();
			this.xropUserEmailAddress = user.GetFederatedSmtpAddress().ToString();
			if (organizationFederatedMailbox != null)
			{
				this.xropTokenRequest = new DelegationTokenRequest
				{
					FederatedIdentity = organizationFederatedMailbox.GetFederatedIdentity(),
					EmailAddress = organizationFederatedMailbox.GetFederatedSmtpAddress().ToString(),
					Target = tokenTarget,
					Offer = Offer.XropLogon
				};
				this.autoDiscoveryTokenRequest = new DelegationTokenRequest
				{
					FederatedIdentity = organizationFederatedMailbox.GetFederatedIdentity(),
					EmailAddress = organizationFederatedMailbox.GetFederatedSmtpAddress().ToString(),
					Target = tokenTarget,
					Offer = Offer.Autodiscover
				};
			}
			this.autoDiscoveryEndpoint = autodiscoveryEndpoint;
		}

		private bool CheckOrgRelationshipFromRemoteConnection(OrganizationRelationship orgRelationship, IGenericADUser user, string domain)
		{
			if (orgRelationship == null)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcOrgRelationshipMissing, domain, new object[]
				{
					domain,
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "Organization relationship for domain {0} is missing. Remote mailbox/archive access will be disabled for user {1}.", domain, user.LegacyDn);
				return false;
			}
			if (!orgRelationship.ArchiveAccessEnabled)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcOrgRelationshipArchiveDisabled, domain, new object[]
				{
					domain,
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "Archive access is disabled for organization relationship (domain name: {0}). Remote mailbox/archive access will be disabled for user {1}.", domain, user.LegacyDn);
				return false;
			}
			if (orgRelationship.TargetAutodiscoverEpr == null)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcInvalidOrgRelationshipTargetAutodiscoverEpr, domain, new object[]
				{
					domain,
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "Organization relationship for domain {0} doesn't have TargetAutodiscoverEpr set. Remote mailbox/archive access will be disabled for user {1}.", domain, user.LegacyDn);
				return false;
			}
			if (orgRelationship.TargetApplicationUri == null)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcInvalidOrgRelationshipTargetApplicationUri, domain, new object[]
				{
					domain,
					user.LegacyDn
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "Organization relationship for domain {0} doesn't have TargetApplicationUri set. Remote mailbox/archive access will be disabled for user {1}.", domain, user.LegacyDn);
				return false;
			}
			return true;
		}

		public static readonly string[] AutodiscoveryRequestedSettings = new string[]
		{
			"ExchangeRpcUrl"
		};

		private IGenericADUser adUser;

		private DelegationTokenRequest autoDiscoveryTokenRequest;

		private FederatedIdentity xropUserFederatedIdentity;

		private string xropUserEmailAddress;

		private DelegationTokenRequest xropTokenRequest;

		private Uri autoDiscoveryEndpoint;

		private bool isRemotingStatusInitialized;

		private bool isCrossPremiseArchiveEnabled;

		private bool isMailboxCrossPremise;

		private bool isArchiveCrossPremise;

		private readonly IStoreSession storeSession;

		private readonly IDirectoryAccessor directoryAccessor;
	}
}
