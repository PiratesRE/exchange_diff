using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.ValidationRules;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeRunspaceConfiguration : IDisposable
	{
		public bool RestrictToFilteredCmdlet { get; protected set; }

		private protected bool HasLinkedRoleGroups { protected get; private set; }

		protected SerializedAccessToken UserAccessToken
		{
			get
			{
				return this.impersonatedUserAccessToken ?? this.logonAccessToken;
			}
		}

		internal bool IsPowerShellWebService { get; private set; }

		internal bool IsAppPasswordUsed
		{
			get
			{
				return this.settings != null && this.settings.UserToken != null && this.settings.UserToken.AppPasswordUsed;
			}
		}

		internal List<RoleEntry> SortedRoleEntryFilter
		{
			get
			{
				return this.sortedRoleEntryFilter;
			}
		}

		internal string ServicePlanForLogging
		{
			get
			{
				if (this.servicePlanForLogging == null && null != this.ExecutingUserOrganizationId && this.ExecutingUserOrganizationId.OrganizationalUnit != null)
				{
					try
					{
						string name = this.ExecutingUserOrganizationId.OrganizationalUnit.Name;
						ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantCUName(name), 639, "ServicePlanForLogging", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
						ExchangeConfigurationUnit exchangeConfigurationUnitByNameOrAcceptedDomain = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(name);
						if (exchangeConfigurationUnitByNameOrAcceptedDomain != null)
						{
							this.servicePlanForLogging = exchangeConfigurationUnitByNameOrAcceptedDomain.ServicePlan;
						}
					}
					catch (Exception ex)
					{
						this.servicePlanForLogging = ex.ToString();
					}
				}
				return this.servicePlanForLogging;
			}
		}

		protected virtual bool ApplyValidationRules
		{
			get
			{
				return true;
			}
		}

		protected bool NoCmdletAllowed { get; set; }

		protected ExchangeRunspaceConfiguration()
		{
		}

		public ExchangeRunspaceConfiguration(IIdentity identity) : this(identity, ExchangeRunspaceConfigurationSettings.GetDefaultInstance())
		{
		}

		public ExchangeRunspaceConfiguration(IIdentity logonIdentity, IIdentity impersonatedIdentity, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, bool callerCheckedAccess) : this(logonIdentity, impersonatedIdentity, ExchangeRunspaceConfigurationSettings.GetDefaultInstance(), roleTypeFilter, sortedRoleEntryFilter, null, callerCheckedAccess)
		{
		}

		internal ExchangeRunspaceConfiguration(IIdentity identity, ExchangeRunspaceConfigurationSettings settings) : this(identity, null, settings, null, null, null, false)
		{
		}

		internal ExchangeRunspaceConfiguration(IIdentity logonIdentity, IIdentity impersonatedIdentity, ExchangeRunspaceConfigurationSettings settings, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess) : this(logonIdentity, impersonatedIdentity, settings, roleTypeFilter, sortedRoleEntryFilter, logonUserRequiredRoleTypes, callerCheckedAccess, false, false, SnapinSet.Default)
		{
		}

		internal ExchangeRunspaceConfiguration(IIdentity logonIdentity, IIdentity impersonatedIdentity, ExchangeRunspaceConfigurationSettings settings, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess, SnapinSet snapinSet) : this(logonIdentity, impersonatedIdentity, settings, roleTypeFilter, sortedRoleEntryFilter, logonUserRequiredRoleTypes, callerCheckedAccess, false, false, snapinSet)
		{
		}

		internal ExchangeRunspaceConfiguration(IIdentity logonIdentity, IIdentity impersonatedIdentity, ExchangeRunspaceConfigurationSettings settings, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess, bool isPowerShellWebService, bool noCmdletAllowed = false, SnapinSet snapinSet = SnapinSet.Default)
		{
			this.logonIdentityToDispose = (logonIdentity as IDisposable);
			this.impersonatedIdentityToDispose = (impersonatedIdentity as IDisposable);
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			this.settings = settings;
			AuthZLogger.SafeAppendGenericInfo("ERC.SnapinCollection", snapinSet.ToString());
			switch (snapinSet)
			{
			case SnapinSet.OSP:
				this.snapinCollection = ExchangeRunspaceConfiguration.OspSnapins;
				break;
			case SnapinSet.Admin:
				this.snapinCollection = ExchangeRunspaceConfiguration.AdminSnapins;
				break;
			default:
				this.snapinCollection = ExchangeRunspaceConfiguration.ExchangeSnapins;
				break;
			}
			if (logonIdentity == null)
			{
				throw new ArgumentNullException("logonIdentity");
			}
			if (DelegatedPrincipal.DelegatedAuthenticationType.Equals(logonIdentity.AuthenticationType))
			{
				this.delegatedPrincipal = new DelegatedPrincipal(logonIdentity, null);
			}
			this.authenticationType = logonIdentity.AuthenticationType;
			this.roleTypeFilter = roleTypeFilter;
			this.sortedRoleEntryFilter = sortedRoleEntryFilter;
			this.logonUserRequiredRoleTypes = logonUserRequiredRoleTypes;
			this.callerCheckedAccess = callerCheckedAccess;
			this.IsPowerShellWebService = isPowerShellWebService;
			this.NoCmdletAllowed = noCmdletAllowed;
			string text;
			if (this.delegatedPrincipal != null)
			{
				text = this.delegatedPrincipal.ToString();
			}
			else
			{
				text = logonIdentity.GetSafeName(true);
			}
			string text2 = null;
			if (impersonatedIdentity != null)
			{
				this.Impersonated = true;
				text2 = impersonatedIdentity.GetSafeName(true);
				this.identityName = Strings.OnbehalfOf(text, text2);
				if (this.PartnerMode)
				{
					throw new ArgumentException("ParterMode is not allowed to create an impersonated runspace.");
				}
			}
			else
			{
				this.Impersonated = false;
				this.identityName = text;
				if (logonUserRequiredRoleTypes != null)
				{
					throw new ArgumentException("logonUserRequiredRoleTypes should only be used for creating impersonated RBAC runspace.");
				}
			}
			ExTraceGlobals.RunspaceConfigTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating new ExchangeRunspaceConfiguration for IIdentity {0}", this.identityName);
			if (this.settings.UserToken == null)
			{
				IIdentity identity = this.Impersonated ? impersonatedIdentity : logonIdentity;
				this.settings.UserToken = UserTokenHelper.CreateDefaultUserTokenInERC(identity, this.delegatedPrincipal, this.Impersonated);
			}
			ADRawEntry adrawEntry = null;
			ADRawEntry adrawEntry2 = null;
			if (this.Impersonated)
			{
				adrawEntry = this.LoadExecutingUser(impersonatedIdentity, null);
				this.executingUser = adrawEntry;
			}
			else if (this.delegatedPrincipal == null)
			{
				adrawEntry2 = this.LoadExecutingUser(logonIdentity, ExchangeRunspaceConfiguration.userPropertyArray);
				this.executingUser = adrawEntry2;
			}
			List<ADObjectId> list;
			if (this.delegatedPrincipal != null)
			{
				OrganizationId orgId;
				this.TryGetGroupAccountsForDelegatedPrincipal(this.delegatedPrincipal, out list, out orgId, out this.ensureTargetIsMemberOfRIMMailboxUsersGroup);
				if (list.Count == 0)
				{
					AuthZLogger.SafeAppendGenericError("ERC.Ctor", "TryGetGroupAccountsForDelegatedPrincipal roleGoups.Count is 0", false);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoGroupsResolvedForDelegatedAdmin, this.identityName, new object[]
					{
						this.identityName,
						this.delegatedPrincipal.DelegatedOrganization,
						this.delegatedPrincipal.Identity.Name
					});
					throw new CmdletAccessDeniedException(Strings.ErrorManagementObjectNotFound(this.identityName));
				}
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string, int>((long)this.GetHashCode(), "Delegated admin authenticated. Creating stub user object for {0}, number of groups {1}", this.identityName, list.Count);
				ADUser aduser = this.CreateDelegatedADUser(this.delegatedPrincipal, orgId);
				if (this.Impersonated)
				{
					this.logonUser = aduser;
				}
				else
				{
					this.executingUser = aduser;
				}
			}
			else
			{
				this.tokenSids = this.GetGroupAccountsSIDs(logonIdentity);
				ExchangeRunspaceConfiguration.TryFindLinkedRoleGroupsBySidList(this.recipientSession, this.tokenSids, this.identityName, out list);
				this.HasLinkedRoleGroups = (list.Count > 0);
			}
			if (this.executingUser == null && this.delegatedPrincipal == null)
			{
				SecurityIdentifier securityIdentifier = this.Impersonated ? impersonatedIdentity.GetSecurityIdentifier() : logonIdentity.GetSecurityIdentifier();
				if (list.Count == 0)
				{
					AuthZLogger.SafeAppendGenericError("ERC.Ctor", "User Not found in AD and no linked role groups", false);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_UserNotFoundBySid, null, new object[]
					{
						this.identityName,
						securityIdentifier,
						this.recipientSession.Source
					});
					throw new CmdletAccessDeniedException(Strings.ErrorManagementObjectNotFound(this.identityName));
				}
				AuthZLogger.SafeAppendGenericInfo("ERC.ExecutingUser", "Linked Role Group Only");
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string, SecurityIdentifier>((long)this.GetHashCode(), "No user found, but found linked role groups associated with identity.  Creating stub user object for {0} with SID {1}", this.identityName, securityIdentifier);
				this.executingUser = this.CreateDelegatedADUser(this.identityName, securityIdentifier);
			}
			if (this.executingUser != null)
			{
				IEnumerable<ADPropertyDefinition> enumerable = ExchangeRunspaceConfiguration.userPropertyArray;
				if (this.executingUser.ObjectSchema != null)
				{
					enumerable = (from x in enumerable
					where x.IsCalculated
					select x).Union(from ADPropertyDefinition x in this.executingUser.ObjectSchema.AllProperties
					where x.IsCalculated
					select x);
				}
				foreach (ProviderPropertyDefinition providerPropertyDefinition in enumerable)
				{
					if (providerPropertyDefinition.IsCalculated)
					{
						object obj = this.executingUser[providerPropertyDefinition];
					}
				}
			}
			if (this.Impersonated)
			{
				if (this.delegatedPrincipal == null)
				{
					this.logonUser = this.LoadExecutingUser(logonIdentity, ExchangeRunspaceConfiguration.userPropertyArray);
					if (this.logonUser == null)
					{
						this.logonUser = this.CreateDelegatedADUser(this.identityName, logonIdentity.GetSecurityIdentifier());
					}
					if (!this.logonUser[ADObjectSchema.OrganizationId].Equals(this.executingUser[ADObjectSchema.OrganizationId]))
					{
						throw new CmdletAccessDeniedException(Strings.NotInSameOrg(this.identityName, text2));
					}
				}
				this.identityName = Strings.OnbehalfOf((this.logonUser != null && this.logonUser.Id != null) ? this.logonUser.Id.ToString() : text, (adrawEntry != null) ? adrawEntry.Id.ToString() : text2);
			}
			else
			{
				this.logonUser = this.executingUser;
				this.identityName = ((adrawEntry2 != null) ? adrawEntry2.Id.ToString() : this.identityName);
			}
			this.intersectRoleEntries = (this.Impersonated && !callerCheckedAccess);
			this.OpenMailboxAsAdmin = this.intersectRoleEntries;
			if (this.delegatedPrincipal == null)
			{
				this.logonAccessToken = this.PopulateGroupMemberships(logonIdentity);
			}
			if (this.Impersonated)
			{
				this.impersonatedUserAccessToken = this.PopulateGroupMemberships(impersonatedIdentity);
			}
			this.LoadRoleCmdletInfo(this.settings.TenantOrganization, roleTypeFilter, sortedRoleEntryFilter, logonUserRequiredRoleTypes, list);
			this.RefreshCombinedCmdletsAndScripts();
			this.RefreshVerboseDebugEnabledCmdlets(this.combinedCmdlets);
			AuthZLogger.SafeAppendGenericInfo("ERC.RoleAssignmentsCnt", this.RoleAssignments.Count.ToString());
			AuthZLogger.SafeAppendGenericInfo("ERC.AllRoleEntriesCnt", this.allRoleEntries.Count.ToString());
			if (this.Impersonated)
			{
				AuthZLogger.SafeAppendGenericInfo("ERC.Impersonated", "true");
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug((long)this.GetHashCode(), "Created ExchangeRunspaceConfiguration for {3} on behalf of {0} with {1} role assingments and {2} entries. CallerCheckedAccess is {4}", new object[]
				{
					this.executingUser.Id,
					this.RoleAssignments.Count,
					this.allRoleEntries.Count,
					this.logonUser.Id,
					callerCheckedAccess
				});
			}
			else
			{
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<ADObjectId, int, int>((long)this.GetHashCode(), "Created ExchangeRunspaceConfiguration for {0} with {1} role assingments and {2} entries", this.executingUser.Id, this.RoleAssignments.Count, this.allRoleEntries.Count);
			}
			this.troubleshootingContext.TraceOperationCompletedAndUpdateContext();
		}

		private ADUser CreateDelegatedADUser(DelegatedPrincipal delegatedPrincipal, OrganizationId orgId)
		{
			PropertyBag propertyBag = new DelegatedPropertyBag();
			propertyBag[DelegatedObjectSchema.Identity] = new DelegatedObjectId(delegatedPrincipal.UserId, delegatedPrincipal.DelegatedOrganization);
			propertyBag[ADObjectSchema.Name] = delegatedPrincipal.GetUserName();
			propertyBag[ADUserSchema.UserPrincipalName] = delegatedPrincipal.UserId;
			propertyBag[ADRecipientSchema.DisplayName] = delegatedPrincipal.DisplayName;
			propertyBag[ADRecipientSchema.HiddenFromAddressListsEnabled] = true;
			if (orgId != null)
			{
				propertyBag[ADObjectSchema.OrganizationId] = orgId;
			}
			return new ADUser(this.recipientSession, propertyBag);
		}

		private ADUser CreateDelegatedADUser(string name, SecurityIdentifier sid)
		{
			PropertyBag propertyBag = new ADPropertyBag();
			propertyBag[ADObjectSchema.Name] = name.Substring(0, Math.Min(64, name.Length));
			propertyBag[ADRecipientSchema.DisplayName] = Strings.DelegatedAdminAccount(name).ToString();
			propertyBag[ADRecipientSchema.HiddenFromAddressListsEnabled] = true;
			if (sid != null)
			{
				propertyBag.DangerousSetValue(ADMailboxRecipientSchema.Sid, sid);
			}
			return new ADUser(this.recipientSession, propertyBag);
		}

		public ICollection<RoleType> RoleTypes
		{
			get
			{
				return this.allRoleTypes.Keys;
			}
		}

		internal DelegatedPrincipal DelegatedPrincipal
		{
			get
			{
				return this.delegatedPrincipal;
			}
		}

		internal virtual ObjectId ExecutingUserIdentity
		{
			get
			{
				return this.executingUser.Identity;
			}
		}

		internal virtual string ExecutingUserDisplayName
		{
			get
			{
				return (string)this.executingUser[ADRecipientSchema.DisplayName];
			}
		}

		internal virtual string ExecutingUserPrincipalName
		{
			get
			{
				return (string)this.executingUser[ADUserSchema.UserPrincipalName];
			}
		}

		internal ADRawEntry ExecutingUser
		{
			get
			{
				return this.executingUser;
			}
		}

		internal ADRecipientOrAddress ExecutingUserAsRecipient
		{
			get
			{
				if (this.executingUserAsRecipient == null)
				{
					Participant participant;
					if (this.delegatedPrincipal != null)
					{
						participant = new Participant(this.ExecutingUserDisplayName, this.delegatedPrincipal.UserId, "SMTP");
					}
					else if (!string.IsNullOrEmpty((string)this.executingUser[ADRecipientSchema.LegacyExchangeDN]))
					{
						participant = new Participant(this.executingUser);
					}
					else
					{
						participant = new Participant(this.ExecutingUserDisplayName, this.ExecutingUserPrincipalName, "SMTP");
					}
					this.executingUserAsRecipient = new ADRecipientOrAddress(participant);
				}
				return this.executingUserAsRecipient;
			}
		}

		public void Dispose()
		{
			if (this.logonIdentityToDispose != null)
			{
				this.logonIdentityToDispose.Dispose();
				this.logonIdentityToDispose = null;
			}
			if (this.impersonatedIdentityToDispose != null)
			{
				this.impersonatedIdentityToDispose.Dispose();
				this.impersonatedIdentityToDispose = null;
			}
			this.EnablePiiMap = false;
		}

		internal virtual bool TryGetExecutingUserId(out ADObjectId executingUserId)
		{
			executingUserId = (ADObjectId)this.executingUser[ADObjectSchema.Id];
			return executingUserId != null;
		}

		internal bool TryGetExecutingWindowsLiveId(out SmtpAddress executingWindowsLiveId)
		{
			executingWindowsLiveId = (SmtpAddress)this.executingUser[ADRecipientSchema.WindowsLiveID];
			return true;
		}

		internal virtual bool TryGetExecutingUserSid(out SecurityIdentifier executingUserSid)
		{
			executingUserSid = (SecurityIdentifier)this.executingUser[IADSecurityPrincipalSchema.Sid];
			return executingUserSid != null;
		}

		internal virtual bool ExecutingUserIsAllowedRemotePowerShell
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.RemotePowerShellEnabled];
			}
		}

		internal virtual OrganizationId ExecutingUserOrganizationId
		{
			get
			{
				return (OrganizationId)this.executingUser[ADObjectSchema.OrganizationId];
			}
		}

		internal virtual bool ExecutingUserIsAllowedECP
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.ECPEnabled];
			}
		}

		internal virtual bool ExecutingUserIsAllowedOWA
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.OWAEnabled];
			}
		}

		internal virtual bool ExecutingUserIsActiveSyncEnabled
		{
			get
			{
				return (bool)this.executingUser[ADUserSchema.ActiveSyncEnabled];
			}
		}

		internal virtual bool ExecutingUserIsPopEnabled
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.PopEnabled];
			}
		}

		internal virtual bool ExecutingUserIsImapEnabled
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.ImapEnabled];
			}
		}

		internal virtual bool ExecutingUserHasRetentionPolicy
		{
			get
			{
				return SharedConfiguration.ExecutingUserHasRetentionPolicy(this.executingUser, this.OrganizationId);
			}
		}

		internal virtual bool ExecutingUserHasExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this.executingUser[ADMailboxRecipientSchema.ExternalOofOptions] == ExternalOofOptions.External;
			}
		}

		internal virtual bool ExecutingUserIsResource
		{
			get
			{
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)this.executingUser[ADRecipientSchema.RecipientTypeDetails];
				return recipientTypeDetails == RecipientTypeDetails.RoomMailbox || recipientTypeDetails == RecipientTypeDetails.EquipmentMailbox || recipientTypeDetails == RecipientTypeDetails.LinkedRoomMailbox;
			}
		}

		internal virtual bool ExecutingUserIsMAPIEnabled
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.MAPIEnabled];
			}
		}

		internal virtual bool ExecutingUserIsUmEnabled
		{
			get
			{
				return (bool)this.executingUser[ADUserSchema.UMEnabled];
			}
		}

		internal virtual SmtpAddress ExecutingUserPrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this.executingUser[ADRecipientSchema.PrimarySmtpAddress];
			}
		}

		internal virtual bool ExecutingUserIsUmConfigured
		{
			get
			{
				if (this.umConfigured == null)
				{
					if (this.ExecutingUserIsUmEnabled)
					{
						this.umConfigured = new bool?(null != UMMailbox.GetPrimaryExtension((ProxyAddressCollection)this.executingUser[ADRecipientSchema.EmailAddresses], ProxyAddressPrefix.UM));
					}
					else
					{
						this.umConfigured = new bool?(false);
					}
				}
				return this.umConfigured.Value;
			}
			set
			{
				if (!this.ExecutingUserIsUmEnabled && value)
				{
					ExTraceGlobals.AccessCheckTracer.TraceError<string>((long)this.GetHashCode(), "User {0} has to be UMEnabled before he could be UMConfigured.", this.executingUser.Id.DistinguishedName);
					throw new InvalidOperationException();
				}
				this.umConfigured = new bool?(value);
			}
		}

		internal virtual bool ExecutingUserIsHiddenFromGAL
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.HiddenFromAddressListsEnabled];
			}
		}

		internal bool ExecutingUserIsPiiApproved
		{
			get
			{
				return this.HasRoleOfType(RoleType.MailRecipientCreation) || this.HasRoleOfType(RoleType.PersonallyIdentifiableInformation);
			}
		}

		public bool NeedSuppressingPiiData
		{
			get
			{
				return this.AuthenticationType == "Kerberos" && !this.ExecutingUserIsPiiApproved && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.PiiRedaction.Enabled;
			}
		}

		internal bool EnablePiiMap
		{
			get
			{
				return this.enablePiiMap;
			}
			set
			{
				this.enablePiiMap = value;
				if (this.enablePiiMap && string.IsNullOrEmpty(this.PiiMapId))
				{
					this.PiiMapId = Guid.NewGuid().ToString();
					return;
				}
				if (!this.enablePiiMap && !string.IsNullOrEmpty(this.PiiMapId))
				{
					this.PiiMapId = null;
				}
			}
		}

		internal string PiiMapId
		{
			get
			{
				return this.piiMapId;
			}
			private set
			{
				if (this.piiMapId != null && this.piiMapId != value)
				{
					PiiMapManager.Instance.Remove(this.piiMapId);
				}
				this.piiMapId = value;
			}
		}

		internal bool ExecutingUserHasResetPasswordPermission
		{
			get
			{
				return this.HasRoleOfType(RoleType.ResetPassword);
			}
		}

		internal bool ExecutingUserLanguagesChanged { get; set; }

		internal virtual MultiValuedProperty<CultureInfo> ExecutingUserLanguages
		{
			get
			{
				MultiValuedProperty<CultureInfo> result;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "ExecutingUserLanguages.get", AuthZLogHelper.AuthZPerfMonitors))
				{
					if (this.ExecutingUserLanguagesChanged)
					{
						ExTraceGlobals.RunspaceConfigTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Fetching updated Languages value for the user: {0}", this.executingUser.Id);
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, false, ConsistencyMode.FullyConsistent, null, this.OrganizationId.ToADSessionSettings(), ConfigScopes.TenantSubTree, 1653, "ExecutingUserLanguages", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
						tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
						ADObjectId entryId;
						if (this.TryGetExecutingUserId(out entryId))
						{
							ADRawEntry adrawEntry = tenantOrRootOrgRecipientSession.ReadADRawEntry(entryId, new ADPropertyDefinition[]
							{
								ADOrgPersonSchema.Languages
							});
							if (adrawEntry != null)
							{
								this.executingUserLanguages = (MultiValuedProperty<CultureInfo>)adrawEntry[ADOrgPersonSchema.Languages];
								this.ExecutingUserLanguagesChanged = false;
							}
						}
					}
					this.executingUserLanguages = (this.executingUserLanguages ?? ((MultiValuedProperty<CultureInfo>)this.executingUser[ADOrgPersonSchema.Languages]));
					result = this.executingUserLanguages;
				}
				return result;
			}
		}

		internal virtual RecipientType ExecutingUserRecipientType
		{
			get
			{
				return (RecipientType)this.executingUser[ADRecipientSchema.RecipientType];
			}
		}

		internal virtual bool ExecutingUserIsPersonToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.IsPersonToPersonTextMessagingEnabled];
			}
		}

		internal virtual bool ExecutingUserIsMachineToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this.executingUser[ADRecipientSchema.IsMachineToPersonTextMessagingEnabled];
			}
		}

		internal virtual bool ExecutingUserIsBposUser
		{
			get
			{
				return CapabilityHelper.HasBposSKUCapability((MultiValuedProperty<Capability>)this.executingUser[SharedPropertyDefinitions.PersistedCapabilities]);
			}
		}

		internal virtual ADRawEntry LogonUser
		{
			get
			{
				return this.logonUser;
			}
		}

		internal virtual string LogonUserDisplayName
		{
			get
			{
				return (string)this.logonUser[ADRecipientSchema.DisplayName];
			}
		}

		internal virtual RecipientType LogonUserRecipientType
		{
			get
			{
				return (RecipientType)this.logonUser[ADRecipientSchema.RecipientType];
			}
		}

		internal virtual SecurityIdentifier LogonUserSid
		{
			get
			{
				return (SecurityIdentifier)this.logonUser[IADSecurityPrincipalSchema.Sid];
			}
		}

		internal virtual OrganizationId OrganizationId
		{
			get
			{
				if (this.organizationId != null && (OrganizationId)this.executingUser[ADObjectSchema.OrganizationId] != OrganizationId.ForestWideOrgId)
				{
					throw new AuthorizationException(Strings.ErrorInvalidStatePartnerOrgNotNull(this.executingUser.Id.ToString()));
				}
				return this.organizationId ?? ((OrganizationId)this.executingUser[ADObjectSchema.OrganizationId]);
			}
		}

		internal virtual OwaSegmentationSettings OwaSegmentationSettings
		{
			get
			{
				OwaSegmentationSettings result;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "OwaSegmentationSettings.get", AuthZLogHelper.AuthZPerfMonitors))
				{
					if (this.owaSegmentationSettings == null)
					{
						lock (this)
						{
							if (this.owaSegmentationSettings == null)
							{
								ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "Creating a new configuration session to read OWA segmentation settings.");
								IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, this.GetADSessionSettings(), 1809, "OwaSegmentationSettings", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
								this.owaSegmentationSettings = OwaSegmentationSettings.GetInstance(tenantOrTopologyConfigurationSession, (ADObjectId)this.executingUser[ADUserSchema.OwaMailboxPolicy], this.OrganizationId);
							}
						}
					}
					result = this.owaSegmentationSettings;
				}
				return result;
			}
		}

		internal virtual IDictionary<ADObjectId, ManagementScope> ScopesCache
		{
			get
			{
				return this.allScopes;
			}
		}

		internal virtual ICollection<ExchangeRoleAssignment> RoleAssignments
		{
			get
			{
				return this.allRoleAssignments;
			}
		}

		internal virtual bool IsDedicatedTenantAdmin
		{
			get
			{
				return this.isDedicatedTenantAdmin;
			}
		}

		internal SerializedAccessToken SecurityAccessToken
		{
			get
			{
				return this.logonAccessToken;
			}
		}

		internal TroubleshootingContext TroubleshootingContext
		{
			get
			{
				return this.troubleshootingContext;
			}
		}

		internal ExchangeRunspaceConfigurationSettings ConfigurationSettings
		{
			get
			{
				return this.settings;
			}
		}

		internal TypeTable TypeTable { get; set; }

		internal string IdentityName
		{
			get
			{
				return this.identityName;
			}
		}

		internal bool Impersonated { get; private set; }

		internal bool OpenMailboxAsAdmin { get; private set; }

		internal bool PartnerMode
		{
			get
			{
				return !string.IsNullOrEmpty(this.ConfigurationSettings.TenantOrganization);
			}
		}

		internal virtual bool HasAdminRoles
		{
			get
			{
				if (this.allRoleTypes != null)
				{
					return this.allRoleTypes.Keys.Any((RoleType x) => ExchangeRole.IsAdminRole(x));
				}
				return false;
			}
		}

		internal string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		private static ADObjectId GetRootOrgUSGContainerId(PartitionId partitionId)
		{
			ADObjectId orAdd;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "GetRootOrgUSGContainerId", AuthZLogHelper.AuthZPerfMonitors))
			{
				orAdd = ExchangeRunspaceConfiguration.rootOrgUSGContainers.GetOrAdd(partitionId.ForestFQDN, delegate(string param0)
				{
					IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 1986, "GetRootOrgUSGContainerId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
					IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 1990, "GetRootOrgUSGContainerId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
					ADGroup adgroup = recipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, configurationSession.ConfigurationNamingContext);
					if (adgroup == null)
					{
						throw new ManagementObjectNotFoundException(DirectoryStrings.ExceptionADTopologyCannotFindWellKnownExchangeGroup);
					}
					return adgroup.Id.Parent;
				});
			}
			return orAdd;
		}

		internal RBACContext GetRbacContext()
		{
			RBACContext result;
			if (this.DelegatedPrincipal != null)
			{
				result = new RBACContext(this.delegatedPrincipal, this.impersonatedUserAccessToken, this.roleTypeFilter, this.sortedRoleEntryFilter, this.logonUserRequiredRoleTypes, this.callerCheckedAccess);
			}
			else
			{
				result = new RBACContext(this.logonAccessToken, this.impersonatedUserAccessToken, this.roleTypeFilter, this.sortedRoleEntryFilter, this.logonUserRequiredRoleTypes, this.callerCheckedAccess);
			}
			return result;
		}

		public static bool TryStampQueryFilterOnManagementScope(ManagementScope managementScope)
		{
			QueryFilter queryFilter;
			string arg;
			if (!RBACHelper.TryConvertPowershellFilterIntoQueryFilter(managementScope.Filter, managementScope.ScopeRestrictionType, null, out queryFilter, out arg))
			{
				ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId, string, string>((long)managementScope.GetHashCode(), "Scope {0} has invalid filter {1}:{2}", managementScope.Id, managementScope.Filter, arg);
				return false;
			}
			managementScope.QueryFilter = queryFilter;
			return true;
		}

		public static bool IsAllowedOrganizationForPartnerAccounts(OrganizationId organizationId)
		{
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				AuthZLogger.SafeAppendGenericError("ERC.IsAllowedOrganizationForPartnerAccounts", "User in Org " + organizationId.GetFriendlyName(), false);
				ExTraceGlobals.AccessDeniedTracer.TraceError<OrganizationId>(0L, "ERC.IsAllowedOrganizationForPartnerAccounts returns false because user belongs to a tenant organization {0}", organizationId);
				return false;
			}
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				AuthZLogger.SafeAppendGenericError("ERC.IsAllowedOrganizationForPartnerAccounts", "Enterprise", false);
				ExTraceGlobals.AccessDeniedTracer.TraceError(0L, "ERC.IsAllowedOrganizationForPartnerAccounts returns false because it is an Enterprise installation");
				return false;
			}
			return true;
		}

		public InitialSessionState CreateInitialSessionState()
		{
			InitialSessionState result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "CreateInitialSessionState", AuthZLogHelper.AuthZPerfMonitors))
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string>(0L, "Entering ERC.CreateInitialSessionState({0})", this.identityName);
				InitialSessionState initialSessionState = InitialSessionStateBuilder.Build(this.combinedCmdlets, this.combinedScripts, this);
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, int>((long)initialSessionState.GetHashCode(), "ERC.CreateInitialSessionState(identity,cmdlets) returns ISS for {0} with {1} commands", this.identityName, initialSessionState.Commands.Count);
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "TraceOperationCompletedAndUpdateContext", AuthZLogHelper.AuthZPerfMonitors))
				{
					this.troubleshootingContext.TraceOperationCompletedAndUpdateContext();
				}
				result = initialSessionState;
			}
			return result;
		}

		public bool HasRoleOfType(RoleType roleType)
		{
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<RoleType>((long)this.GetHashCode(), "Entering ERC.HasRoleOfType({0})", roleType);
			bool flag = this.allRoleTypes != null && this.allRoleTypes.Keys.Contains(roleType);
			this.TracePublicInstanceAPIResult(flag, "ERC.HasRoleOfType({0}) returns {1}", new object[]
			{
				roleType,
				flag
			});
			return flag;
		}

		public virtual bool IsCmdletAllowedInScope(string cmdletName, string[] paramNames, ScopeSet scopeSet)
		{
			if (string.IsNullOrEmpty(cmdletName))
			{
				throw new ArgumentNullException("cmdletName");
			}
			if (scopeSet == null)
			{
				throw new ArgumentNullException("scopeSet");
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, int>((long)this.GetHashCode(), "Entering ERC.IsCmdletAllowedInScope({0}, {1} params, ScopeSet)", cmdletName, (paramNames == null) ? 0 : paramNames.Length);
			int num;
			int num2;
			if (!this.LocateRoleEntriesForCmdlet(cmdletName, out num, out num2))
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "IsCmdletAllowedInScope({0}) returns false for user {1} because this cmdlet is not present in any of the assigned roles.", cmdletName, this.identityName);
				return false;
			}
			List<RoleEntry> list = new List<RoleEntry>(num2 - num + 1);
			for (int i = num; i <= num2; i++)
			{
				if (this.allRoleEntries[i].RoleAssignment.AllPresentScopesMatch(scopeSet))
				{
					list.Add(this.allRoleEntries[i].RoleEntry);
				}
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "IsCmdletAllowedInScope({0}) returns false for user {1} because no role assignments with matching scopes were found.", cmdletName, this.identityName);
				return false;
			}
			bool flag = false;
			if (this.ApplyValidationRules && ValidationRuleFactory.HasApplicableValidationRules(cmdletName, this.executingUser))
			{
				if (scopeSet.RecipientReadScope is RbacScope && ((RbacScope)scopeSet.RecipientReadScope).ScopeType == ScopeType.Self)
				{
					flag = true;
				}
				flag = (flag || this.ContainsOnlySelfScopes(scopeSet.RecipientWriteScopes));
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, bool>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}) isSelfScopePresent '{1}'.", cmdletName, flag);
			}
			if (!flag && paramNames.IsNullOrEmpty<string>())
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}) returns true because cmdlet was found and no params were requested", cmdletName);
				return true;
			}
			RoleEntry roleEntry = RoleEntry.MergeParameters(list);
			bool flag2 = true;
			if (flag)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, RoleEntry>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}). MergedRoleEntry '{1}'.", cmdletName, roleEntry);
				if (roleEntry.Parameters.Count > 0)
				{
					roleEntry = this.TrimRoleEntryParametersWithValidationRules(roleEntry, false);
					flag2 = (roleEntry.Parameters.Count > 0);
				}
			}
			if (!paramNames.IsNullOrEmpty<string>())
			{
				flag2 = roleEntry.ContainsAllParameters(paramNames);
			}
			this.TracePublicInstanceAPIResult(flag2, "ERC.IsCmdletAllowedInScope({0}) returns {1} after successfully locating cmdlet in user's role and matching parameters", new object[]
			{
				cmdletName,
				flag2
			});
			return flag2;
		}

		public virtual bool IsCmdletAllowedInScope(string cmdletName, string[] paramNames, ADRawEntry adObject, ScopeLocation scopeLocation)
		{
			if (string.IsNullOrEmpty(cmdletName))
			{
				throw new ArgumentNullException("cmdletName");
			}
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			if (scopeLocation != ScopeLocation.RecipientRead && scopeLocation != ScopeLocation.RecipientWrite)
			{
				throw new NotSupportedException("Only ScopeLocation.RecipientRead and ScopeLocation.RecipientWrite is supported!");
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, int, string>((long)this.GetHashCode(), "Entering ERC.IsCmdletAllowedInScope({0}, {1} params, {2}, ScopeLocation)", cmdletName, paramNames.IsNullOrEmpty<string>() ? 0 : paramNames.Length, adObject.GetDistinguishedNameOrName());
			RoleEntry roleEntry = null;
			ScopeSet scopeSet = this.CalculateScopeSetForExchangeCmdlet(cmdletName, paramNames, this.OrganizationId, null, out roleEntry);
			if (scopeSet == null)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}, {1}) returns FALSE because no valid scope has been calculated from the specified cmdlet and parameters combination.", cmdletName, adObject.GetDistinguishedNameOrName());
				return false;
			}
			switch (scopeLocation)
			{
			case ScopeLocation.RecipientRead:
				if (!ADSession.IsWithinScope(adObject, scopeSet.RecipientReadScope))
				{
					ExTraceGlobals.PublicInstanceAPITracer.TraceWarning((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}, {1}) returns FALSE because {2} is not in the valid read scope for user {3}.", new object[]
					{
						cmdletName,
						(ADObjectId)adObject[ADObjectSchema.Id],
						(ADObjectId)adObject[ADObjectSchema.Id],
						this.identityName
					});
					return false;
				}
				break;
			case ScopeLocation.RecipientWrite:
			{
				ADScopeException ex;
				if (!ADSession.TryVerifyIsWithinScopes(adObject, scopeSet.RecipientReadScope, scopeSet.RecipientWriteScopes, scopeSet.ExclusiveRecipientScopes, scopeSet.ValidationRules, false, out ex))
				{
					ExTraceGlobals.PublicInstanceAPITracer.TraceWarning((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}, {1}) returns FALSE because {2} is not in the valid read or write scope for user {3}. ADScopeException {4} is caught.", new object[]
					{
						cmdletName,
						(ADObjectId)adObject[ADObjectSchema.Id],
						(ADObjectId)adObject[ADObjectSchema.Id],
						this.identityName,
						ex
					});
					return false;
				}
				break;
			}
			default:
				throw new NotSupportedException("Only ScopeLocation.RecipientRead and ScopeLocation.RecipientWrite is supported!");
			}
			if (paramNames.IsNullOrEmpty<string>() && null != roleEntry && roleEntry.Parameters.Count != 0 && this.TrimRoleEntryParametersWithValidationRules(roleEntry, adObject, false).Parameters.Count == 0)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}, {1}) returns FALSE. All possible parameters of '{0}' violates a Validation Rule on object '{1}'.", cmdletName, adObject.GetDistinguishedNameOrName());
				return false;
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, ADObjectId>((long)this.GetHashCode(), "ERC.IsCmdletAllowedInScope({0}, {1}) returning TRUE.", cmdletName, (ADObjectId)adObject[ADObjectSchema.Id]);
			return true;
		}

		public ICollection<string> GetAllowedParamsForSetCmdlet(string cmdletName, ADRawEntry adObject, ScopeLocation scopeLocation)
		{
			if (string.IsNullOrEmpty(cmdletName))
			{
				throw new ArgumentNullException("cmdletName");
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, string, ScopeLocation>((long)this.GetHashCode(), "Entering ERC.GetAllowedParamsForSetCmdlet({0}, {1}, ScopeLocation {2})", cmdletName, (adObject != null) ? ((ADObjectId)adObject[ADObjectSchema.Id]).ToString() : string.Empty, scopeLocation);
			int num;
			int num2;
			if (!this.LocateRoleEntriesForCmdlet(cmdletName, out num, out num2))
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "GetAllowedParamsForSetCmdlet({0}) returns false for user {1} because this cmdlet is not present in any of the assigned roles.", cmdletName, this.identityName);
				return null;
			}
			List<RoleEntry> list = new List<RoleEntry>(num2 - num + 1);
			int i = num;
			while (i <= num2)
			{
				bool flag = true;
				if (adObject == null)
				{
					goto IL_156;
				}
				flag = false;
				RoleAssignmentScopeSet effectiveScopeSet = this.allRoleEntries[i].RoleAssignment.GetEffectiveScopeSet(this.allScopes, this.UserAccessToken);
				if (effectiveScopeSet != null)
				{
					ScopeSet scopeSet = new ScopeSet(effectiveScopeSet.RecipientReadScope, new ADScopeCollection[]
					{
						new ADScopeCollection(new ADScope[]
						{
							effectiveScopeSet.RecipientWriteScope
						})
					}, null, null);
					switch (scopeLocation)
					{
					case ScopeLocation.RecipientRead:
						if (ADSession.IsWithinScope(adObject, scopeSet.RecipientReadScope))
						{
							flag = true;
							goto IL_156;
						}
						goto IL_156;
					case ScopeLocation.RecipientWrite:
					{
						ADScopeException ex;
						if (ADSession.TryVerifyIsWithinScopes(adObject, scopeSet.RecipientReadScope, scopeSet.RecipientWriteScopes, scopeSet.ExclusiveRecipientScopes, scopeSet.ValidationRules, false, out ex))
						{
							flag = true;
							goto IL_156;
						}
						goto IL_156;
					}
					default:
						throw new NotSupportedException("Only ScopeLocation.DomainRead and ScopeLocation.DomainWrite is supported!");
					}
				}
				IL_171:
				i++;
				continue;
				IL_156:
				if (flag)
				{
					list.Add(this.allRoleEntries[i].RoleEntry);
					goto IL_171;
				}
				goto IL_171;
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceWarning<string, string>((long)this.GetHashCode(), "GetAllowedParamsForSetCmdlet({0}) returns false for user {1} because no role assignments with matching scopes were found.", cmdletName, this.identityName);
				return null;
			}
			return RoleEntry.MergeParameters(list).Parameters;
		}

		public virtual bool IsApplicationImpersonationAllowed(ADUser impersonatedADUserObject)
		{
			if (impersonatedADUserObject == null)
			{
				throw new ArgumentNullException("impersonatedADUserObject");
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Entering ERC.IsApplicationImpersonationAllowed({0})", impersonatedADUserObject.Identity);
			bool flag;
			if (this.PartnerMode)
			{
				flag = this.OrganizationId.Equals(impersonatedADUserObject.OrganizationId);
			}
			else
			{
				flag = this.IsCmdletAllowedInScope("Impersonate-ExchangeUser", ExchangeRunspaceConfiguration.emptyArray, impersonatedADUserObject, ScopeLocation.RecipientWrite);
			}
			if (flag && this.ensureTargetIsMemberOfRIMMailboxUsersGroup && this.OrganizationId != null && this.OrganizationId.ConfigurationUnit != null)
			{
				IRecipientSession recipientSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.recipientSession, this.OrganizationId, true);
				flag = recipientSession.IsMemberOfGroupByWellKnownGuid(WellKnownGuid.RIMMailboxUsersGroupGuid, this.OrganizationId.ConfigurationUnit.DistinguishedName, impersonatedADUserObject.Id);
			}
			this.TracePublicInstanceAPIResult(flag, "ERC.IsApplicationImpersonationAllowed({0}) returns {1} in {2}Partner mode", new object[]
			{
				impersonatedADUserObject.Identity,
				flag,
				this.PartnerMode ? "" : "non-"
			});
			return flag;
		}

		public bool IsVerboseEnabled(string cmdletName)
		{
			bool result;
			lock (this.loadRoleCmdletInfoSyncRoot)
			{
				result = this.verboseEnabledCmdlets.Contains(cmdletName);
			}
			return result;
		}

		public bool IsDebugEnabled(string cmdletName)
		{
			bool result;
			lock (this.loadRoleCmdletInfoSyncRoot)
			{
				result = this.debugEnabledCmdlets.Contains(cmdletName);
			}
			return result;
		}

		internal bool TryGetGroupAccountsForDelegatedPrincipal(DelegatedPrincipal delegatedPrincipal, out List<ADObjectId> groups, out OrganizationId orgId, out bool ensureTargetIsMemberOfRIMMailboxUsersGroup)
		{
			bool result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "TryGetGroupAccountsForDelegatedPrincipal", AuthZLogHelper.AuthZPerfMonitors))
			{
				ensureTargetIsMemberOfRIMMailboxUsersGroup = false;
				ADSessionSettings sessionSettings = null;
				groups = new List<ADObjectId>();
				orgId = null;
				if (delegatedPrincipal == null)
				{
					result = false;
				}
				else
				{
					string[] roles = delegatedPrincipal.Roles;
					try
					{
						sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(delegatedPrincipal.DelegatedOrganization));
					}
					catch (CannotResolveTenantNameException arg)
					{
						AuthZLogger.SafeAppendGenericError("ERC.TryGetGroupAccountsForDelegatedPrincipal", delegatedPrincipal.DelegatedOrganization + " " + arg, false);
						ExTraceGlobals.RunspaceConfigTracer.TraceError<string>((long)delegatedPrincipal.GetHashCode(), "The target organization {0} for a delegated account should be correctly resolved at this stage.", delegatedPrincipal.DelegatedOrganization);
						return false;
					}
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 2670, "TryGetGroupAccountsForDelegatedPrincipal", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
					ExchangeConfigurationUnit exchangeConfigurationUnitByNameOrAcceptedDomain = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(delegatedPrincipal.DelegatedOrganization);
					if (exchangeConfigurationUnitByNameOrAcceptedDomain == null)
					{
						result = false;
					}
					else
					{
						orgId = exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId;
						this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId, exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId, false), 2690, "TryGetGroupAccountsForDelegatedPrincipal", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
						LinkedPartnerGroupInformation[] array = new LinkedPartnerGroupInformation[roles.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = new LinkedPartnerGroupInformation();
							array[i].LinkedPartnerOrganizationId = delegatedPrincipal.UserOrganizationId;
							array[i].LinkedPartnerGroupId = roles[i];
						}
						ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId, exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId, false);
						adsessionSettings.ForceADInTemplateScope = true;
						ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 2713, "TryGetGroupAccountsForDelegatedPrincipal", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs") as ITenantRecipientSession;
						if (tenantRecipientSession != null)
						{
							foreach (Result<ADRawEntry> result2 in tenantRecipientSession.ReadMultipleByLinkedPartnerId(array, ExchangeRunspaceConfiguration.delegatedGroupInfo))
							{
								if (result2.Data != null)
								{
									groups.Add((ADObjectId)result2.Data[ADObjectSchema.Id]);
									ADObjectId[] nestedSecurityGroupMembership = ExchangeRunspaceConfiguration.GetNestedSecurityGroupMembership(result2.Data, tenantRecipientSession, AssignmentMethod.SecurityGroup | AssignmentMethod.RoleGroup);
									if (nestedSecurityGroupMembership != null && nestedSecurityGroupMembership.Length > 0)
									{
										groups.AddRange(nestedSecurityGroupMembership);
									}
								}
								else
								{
									ExTraceGlobals.RunspaceConfigTracer.TraceDebug<LinkedPartnerGroupInformation[], string, string>(0L, "Failed to find delegated group '{0}' for user '{1}' with error '{2}'", array, delegatedPrincipal.UserId, (result2.Error != null) ? result2.Error.ToString() : string.Empty);
								}
							}
						}
						if (array.Length > 1)
						{
							groups = groups.Distinct(ADObjectIdEqualityComparer.Instance).ToList<ADObjectId>();
						}
						DNWithBinary dnwithBinary = TaskHelper.FindWellKnownObjectEntry(exchangeConfigurationUnitByNameOrAcceptedDomain.OtherWellKnownObjects, WellKnownGuid.RIMMailboxAdminsGroupGuid);
						if (dnwithBinary != null)
						{
							ADObjectId item = new ADObjectId(dnwithBinary.DistinguishedName);
							ensureTargetIsMemberOfRIMMailboxUsersGroup = groups.Contains(item);
						}
						result = true;
					}
				}
			}
			return result;
		}

		internal static ADRawEntry TryFindComputer(SecurityIdentifier userSid)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2768, "TryFindComputer", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			return topologyConfigurationSession.FindComputerBySid(userSid);
		}

		internal static bool TryFindLinkedRoleGroupsBySidList(IRecipientSession recipientSession, ICollection<SecurityIdentifier> sids, string identityName, out List<ADObjectId> roleGroups)
		{
			bool result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "TryFindLinkedRoleGroupsBySidList", AuthZLogHelper.AuthZPerfMonitors))
			{
				if (sids == null)
				{
					roleGroups = new List<ADObjectId>(0);
				}
				else
				{
					roleGroups = new List<ADObjectId>(sids.Count);
					foreach (SecurityIdentifier sId in sids)
					{
						IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(recipientSession.SessionSettings.PartitionId), 2805, "TryFindLinkedRoleGroupsBySidList", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
						ADObjectId rootOrgUSGContainerId = ExchangeRunspaceConfiguration.GetRootOrgUSGContainerId(recipientSession.SessionSettings.PartitionId);
						IEnumerable<ADGroup> enumerable = rootOrganizationRecipientSession.FindRoleGroupsByForeignGroupSid(rootOrgUSGContainerId, sId);
						foreach (ADGroup adgroup in enumerable)
						{
							ExTraceGlobals.RunspaceConfigTracer.TraceDebug<ObjectId, string>(0L, "Found linked RoleGroup '{0}' for '{1}'", adgroup.Identity, identityName);
							roleGroups.Add(adgroup.Id);
							ADObjectId[] nestedSecurityGroupMembership = ExchangeRunspaceConfiguration.GetNestedSecurityGroupMembership(adgroup, recipientSession, AssignmentMethod.SecurityGroup | AssignmentMethod.RoleGroup);
							if (nestedSecurityGroupMembership != null && nestedSecurityGroupMembership.Length > 0)
							{
								roleGroups.AddRange(nestedSecurityGroupMembership);
							}
						}
					}
					roleGroups = roleGroups.Distinct(ADObjectIdEqualityComparer.Instance).ToList<ADObjectId>();
				}
				result = (roleGroups.Count != 0);
			}
			return result;
		}

		internal void LoadRoleCmdletInfo()
		{
			lock (this.loadRoleCmdletInfoSyncRoot)
			{
				List<ADObjectId> implicitRoleIds = new List<ADObjectId>(0);
				if (this.tokenSids != null)
				{
					ExchangeRunspaceConfiguration.TryFindLinkedRoleGroupsBySidList(this.recipientSession, this.tokenSids, this.identityName, out implicitRoleIds);
				}
				this.LoadRoleCmdletInfo((this.organizationId == null) ? null : this.organizationId.OrganizationalUnit.Name, null, null, null, implicitRoleIds);
				this.RefreshCombinedCmdletsAndScripts();
				this.RefreshVerboseDebugEnabledCmdlets(this.combinedCmdlets);
			}
		}

		internal ProvisioningBroker GetProvisioningBroker()
		{
			ProvisioningBroker provisioningBroker = this.provisioningBroker;
			if (provisioningBroker == null)
			{
				lock (this.provisioningBrokerSyncRoot)
				{
					if (this.provisioningBroker == null)
					{
						this.provisioningBroker = new ProvisioningBroker();
					}
					provisioningBroker = this.provisioningBroker;
				}
			}
			return provisioningBroker;
		}

		internal virtual ScopeSet CalculateScopeSetForExchangeCmdlet(string exchangeCmdletName, IList<string> parameters, OrganizationId organizationId, Task.ErrorLoggerDelegate writeError)
		{
			RoleEntry roleEntry;
			return this.CalculateScopeSetForExchangeCmdlet(exchangeCmdletName, parameters, organizationId, writeError, out roleEntry);
		}

		protected virtual ScopeSet CalculateScopeSetForExchangeCmdlet(string exchangeCmdletName, IList<string> parameters, OrganizationId organizationId, Task.ErrorLoggerDelegate writeError, out RoleEntry exchangeCmdlet)
		{
			ScopeSet result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "CalculateScopeSetForExchangeCmdlet", AuthZLogHelper.AuthZPerfMonitors))
			{
				if (string.IsNullOrEmpty(exchangeCmdletName))
				{
					throw new ArgumentNullException("exchangeCmdletName");
				}
				if (parameters == null)
				{
					throw new ArgumentNullException("parameters");
				}
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string>((long)this.GetHashCode(), "Entering ERC.CalculateScopeSetForExchangeCmdlet({0})", exchangeCmdletName);
				int num = 1;
				int num2 = -1;
				string text = null;
				bool flag = false;
				exchangeCmdlet = null;
				if (exchangeCmdletName.Equals("Impersonate-ExchangeUser", StringComparison.OrdinalIgnoreCase))
				{
					text = exchangeCmdletName;
					flag = this.LocateRoleEntriesForCmdlet(text, out num, out num2);
				}
				else
				{
					foreach (string str in this.snapinCollection)
					{
						text = str + "\\" + exchangeCmdletName;
						this.FaultInjection_ByPassRBACTestSnapinCheck(ref text, exchangeCmdletName);
						flag = this.LocateRoleEntriesForCmdlet(text, out num, out num2);
						if (flag)
						{
							break;
						}
					}
				}
				if (!flag)
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string, object, string>((long)this.GetHashCode(), "CalculateScopeSetForExchangeCmdlet({0}) returns null (NO SCOPE) because this cmdlet is not in any roles assigned to user {1} ({2}).", exchangeCmdletName, this.executingUser[ADRecipientSchema.PrimarySmtpAddress], this.identityName);
					if (writeError != null)
					{
						AuthZLogger.SafeAppendGenericError("ERC.CalculateScopeSetForExchangeCmdlet", exchangeCmdletName + " Not Allowed for " + this.IdentityName, false);
						TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_CmdletAccessDenied_InvalidCmdlet, this.IdentityName + exchangeCmdletName, new object[]
						{
							exchangeCmdletName,
							this.IdentityName
						});
						writeError(new CmdletAccessDeniedException(Strings.NoRoleEntriesFound(exchangeCmdletName)), (ExchangeErrorCategory)18, null);
					}
					result = null;
				}
				else
				{
					List<RoleEntryInfo> list = new List<RoleEntryInfo>(num2 - num + 1);
					bool flag2 = false;
					using (new MonitoredScope("ExchangeRunspaceConfiguration", "ValidationRuleFactory.HasApplicableValidationRules", AuthZLogHelper.AuthZPerfMonitors))
					{
						flag2 = (this.ApplyValidationRules && ValidationRuleFactory.HasApplicableValidationRules(text, this.executingUser));
					}
					HashSet<string> hashSet = null;
					if (parameters.Count == 0 && flag2)
					{
						hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					}
					int i = num;
					while (i <= num2)
					{
						if (parameters.Count != 0 && !this.allRoleEntries[i].RoleEntry.ContainsAnyParameter(parameters))
						{
							goto IL_281;
						}
						RoleAssignmentScopeSet effectiveScopeSet = this.allRoleEntries[i].RoleAssignment.GetEffectiveScopeSet(this.allScopes, this.UserAccessToken);
						if (effectiveScopeSet != null)
						{
							list.Add(this.allRoleEntries[i]);
							this.allRoleEntries[i].ScopeSet = effectiveScopeSet;
							goto IL_281;
						}
						ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, ADObjectId>((long)this.GetHashCode(), "CalculateScopeSetForExchangeCmdlet({0}) skips role assignment {1} because the required scope objects were not found.", exchangeCmdletName, this.allRoleEntries[i].RoleAssignment.Id);
						IL_2CF:
						i++;
						continue;
						IL_281:
						if (hashSet != null)
						{
							foreach (string item in this.allRoleEntries[i].RoleEntry.Parameters)
							{
								hashSet.Add(item);
							}
							goto IL_2CF;
						}
						goto IL_2CF;
					}
					if (list.Count == 0)
					{
						ExTraceGlobals.AccessDeniedTracer.TraceError<string, string>((long)this.GetHashCode(), "CalculateScopeSetForExchangeCmdlet({0}) returns null (NO SCOPE) for user {1} because no role assignments with matching parameters were found.", exchangeCmdletName, this.identityName);
						if (writeError != null)
						{
							AuthZLogger.SafeAppendGenericError("ERC.CalculateScopeSetForExchangeCmdlet", exchangeCmdletName + " Some of parameters NOT Allowed for " + this.IdentityName, false);
							TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_CmdletAccessDenied_InvalidParameter, this.IdentityName + exchangeCmdletName, new object[]
							{
								exchangeCmdletName,
								this.IdentityName
							});
							writeError(new CmdletAccessDeniedException(Strings.NoRoleEntriesWithParametersFound(exchangeCmdletName)), (ExchangeErrorCategory)18, null);
						}
						result = null;
					}
					else
					{
						ScopeType[] maximumScopesCommonForAllParams = new ScopeType[]
						{
							ScopeType.None,
							ScopeType.None,
							ScopeType.None,
							ScopeType.None
						};
						ScopeType[,] maxParameterScopes = new ScopeType[(parameters.Count == 0) ? 1 : parameters.Count, 4];
						List<RoleAssignmentScopeSet>[] array = this.CalculateScopesForEachParameter(exchangeCmdletName, parameters, organizationId, writeError, num, num2, maximumScopesCommonForAllParams, maxParameterScopes);
						if (array == null)
						{
							result = null;
						}
						else
						{
							ScopeSet scopeSet = this.AggregateScopes(organizationId, array, maximumScopesCommonForAllParams, maxParameterScopes);
							if (flag2)
							{
								exchangeCmdlet = list[0].RoleEntry.Clone((parameters.Count == 0) ? hashSet.ToList<string>() : parameters);
							}
							IList<ValidationRule> list2 = this.ApplyValidationRules ? ValidationRuleFactory.GetApplicableValidationRules(text, parameters, this.executingUser) : null;
							if (list2 != null && list2.Count > 0)
							{
								ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, int>((long)this.GetHashCode(), "ERC.CalculateScopeSetForExchangeCmdlet({0}) Adding {1} 'ValidationRules", exchangeCmdletName, list2.Count);
								List<ValidationRule> list3 = (List<ValidationRule>)scopeSet.ValidationRules;
								list3.AddRange(list2);
							}
							ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<string, ScopeSet>((long)this.GetHashCode(), "ERC.CalculateScopeSetForExchangeCmdlet({0}) returns ScopeSet {1}", exchangeCmdletName, scopeSet);
							result = scopeSet;
						}
					}
				}
			}
			return result;
		}

		internal static bool IsFeatureValidOnObject(string featureName, ADObject targetObject)
		{
			if (targetObject == null)
			{
				throw new ArgumentNullException("targetObject");
			}
			List<ValidationRule> validationRulesByFeature = ValidationRuleFactory.GetValidationRulesByFeature(featureName);
			return validationRulesByFeature == null || validationRulesByFeature.Count == 0 || ExchangeRunspaceConfiguration.EvaluateRulesOnObject(validationRulesByFeature, targetObject);
		}

		internal static bool IsCmdletValidOnObject(string cmdletFullName, IList<string> parameters, ADObject targetObject)
		{
			if (targetObject == null)
			{
				throw new ArgumentNullException("targetObject");
			}
			IList<ValidationRule> applicableValidationRules = ValidationRuleFactory.GetApplicableValidationRules(cmdletFullName, parameters, ValidationRuleSkus.All);
			return ExchangeRunspaceConfiguration.EvaluateRulesOnObject(applicableValidationRules, targetObject);
		}

		private static bool EvaluateRulesOnObject(IList<ValidationRule> rules, ADObject targetObject)
		{
			if (targetObject == null)
			{
				throw new ArgumentNullException("targetObject");
			}
			if (rules != null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string>((long)targetObject.GetHashCode(), "ERC.EvaluateRulesOnObject({0}) Validating rules.", targetObject.Identity.ToString());
				RuleValidationException ex = null;
				foreach (ValidationRule validationRule in rules)
				{
					if (!validationRule.TryValidate(targetObject, out ex))
					{
						ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)targetObject.GetHashCode(), "ERC.EvaluateRulesOnObject({0}) returns false. RuleException '{1}'", validationRule.Name, ex.Message);
						return false;
					}
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string>((long)targetObject.GetHashCode(), "ERC.EvaluateRulesOnObject({0}) returns true.", targetObject.ToString());
			return true;
		}

		internal void SwitchCurrentPartnerOrganizationAndReloadRoleCmdletInfo(string organizationName)
		{
			if (!this.PartnerMode)
			{
				throw new InvalidOperationException();
			}
			if (string.IsNullOrEmpty(organizationName))
			{
				throw new ArgumentException("organizationName");
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 3350, "SwitchCurrentPartnerOrganizationAndReloadRoleCmdletInfo", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
			Dictionary<ADObjectId, ManagementScope> userAllScopes = new Dictionary<ADObjectId, ManagementScope>(this.allScopes);
			this.ReadAndCheckAllScopes(tenantOrTopologyConfigurationSession, userAllScopes, organizationName);
		}

		internal bool IsScriptInUserRole(string scriptFullPath)
		{
			if (string.IsNullOrEmpty(scriptFullPath))
			{
				return false;
			}
			string scriptName;
			if (scriptFullPath.StartsWith(ConfigurationContext.Setup.RemoteScriptPath))
			{
				scriptName = scriptFullPath.Substring(ConfigurationContext.Setup.RemoteScriptPath.Length + 1);
			}
			else
			{
				if (!scriptFullPath.StartsWith(ConfigurationContext.Setup.TorusRemoteScriptPath))
				{
					return false;
				}
				scriptName = scriptFullPath.Substring(ConfigurationContext.Setup.TorusRemoteScriptPath.Length + 1);
			}
			return this.LocateRoleEntriesForScript(scriptName);
		}

		internal ADObjectId[] GetNestedSecurityGroupMembership()
		{
			return this.universalSecurityGroupsCache;
		}

		internal void RefreshProvisioningBroker()
		{
			lock (this.provisioningBrokerSyncRoot)
			{
				this.provisioningBroker = null;
			}
		}

		protected virtual ICollection<SecurityIdentifier> GetGroupAccountsSIDs(IIdentity logonIdentity)
		{
			ICollection<SecurityIdentifier> collection = null;
			try
			{
				collection = logonIdentity.GetGroupAccountsSIDs();
				IdentityCache<ICollection<SecurityIdentifier>>.Current.Add(logonIdentity, collection);
			}
			catch (AuthzException ex)
			{
				if (!IdentityCache<ICollection<SecurityIdentifier>>.Current.TryGetValue(logonIdentity, out collection))
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string, AuthzException>(0L, "Call to NativeMethods.AuthzInitializeContextFromSid() failed when initializing the ClientSecurityContext for user {0}. Exception: {1}", this.identityName, ex);
					AuthZLogger.SafeAppendGenericError("ERC.GetGroupAccountsSIDs", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NativeCallFailed, null, new object[]
					{
						this.identityName,
						ex
					});
					throw new CmdletAccessDeniedException(Strings.ErrorManagementObjectNotFound(this.identityName));
				}
				AuthZLogger.SafeAppendGenericInfo("ERC.GetGroupAccountsSIDs", "Recovered from AuthzException");
			}
			return collection;
		}

		protected virtual SerializedAccessToken PopulateGroupMemberships(IIdentity identity)
		{
			SerializedAccessToken serializedAccessToken = null;
			if (!this.IsPowershellProcess())
			{
				try
				{
					serializedAccessToken = identity.GetAccessToken();
					IdentityCache<SerializedAccessToken>.Current.Add(identity, serializedAccessToken);
				}
				catch (AuthzException ex)
				{
					if (!IdentityCache<SerializedAccessToken>.Current.TryGetValue(identity, out serializedAccessToken))
					{
						ExTraceGlobals.AccessDeniedTracer.TraceError<string, AuthzException>(0L, "Call to NativeMethods.AuthzInitializeContextFromSid() failed when initializing the ClientSecurityContext for user {0}. Exception: {1}", this.identityName, ex);
						AuthZLogger.SafeAppendGenericError("ERC.PopulateGroupMemberships", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
						TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NativeCallFailed, null, new object[]
						{
							this.identityName,
							ex
						});
						throw new CmdletAccessDeniedException(new LocalizedString(ex.Message));
					}
					AuthZLogger.SafeAppendGenericInfo("ERC.PopulateGroupMemberships", "Recovered from AuthzException");
				}
			}
			return serializedAccessToken;
		}

		protected virtual ADRawEntry LoadExecutingUser(IIdentity identity, IList<PropertyDefinition> properties)
		{
			ADRawEntry result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadExecutingUser", AuthZLogHelper.AuthZPerfMonitors))
			{
				PartitionId partitionId = null;
				GenericSidIdentity genericSidIdentity = identity as GenericSidIdentity;
				if (genericSidIdentity != null && !string.IsNullOrEmpty(genericSidIdentity.PartitionId))
				{
					PartitionId.TryParse(genericSidIdentity.PartitionId, out partitionId);
				}
				ADSessionSettings sessionSettings;
				if (partitionId != null)
				{
					sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				}
				else
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 3560, "LoadExecutingUser", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
				SecurityIdentifier securityIdentifier = identity.GetSecurityIdentifier();
				ADRawEntry adrawEntry = this.recipientSession.FindUserBySid(securityIdentifier, properties);
				if (adrawEntry == null && partitionId == null && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled)
				{
					adrawEntry = PartitionDataAggregator.FindUserBySid(securityIdentifier, properties, ref this.recipientSession);
				}
				if (adrawEntry != null && !UserIdParameter.AllowedRecipientTypes.Contains((RecipientType)adrawEntry[ADRecipientSchema.RecipientType]))
				{
					adrawEntry = null;
				}
				if (adrawEntry == null)
				{
					ADComputer adcomputer = (ADComputer)ExchangeRunspaceConfiguration.TryFindComputer(securityIdentifier);
					if (adcomputer == null)
					{
						ExTraceGlobals.AccessDeniedTracer.TraceWarning<SecurityIdentifier>(0L, "Neither User nor Computer {0} could not be found in AD", securityIdentifier);
						return null;
					}
					adrawEntry = adcomputer;
				}
				SecurityIdentifier securityIdentifier2 = (SecurityIdentifier)adrawEntry[IADSecurityPrincipalSchema.Sid];
				result = adrawEntry;
			}
			return result;
		}

		protected virtual ManagementScope[] LoadExclusiveScopes()
		{
			ManagementScope[] result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadExclusiveScopes", AuthZLogHelper.AuthZPerfMonitors))
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), this.OrganizationId, this.OrganizationId, false), 3639, "LoadExclusiveScopes", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
				result = tenantOrTopologyConfigurationSession.GetAllExclusiveScopes().ReadAllPages();
			}
			return result;
		}

		protected virtual Result<ManagementScope>[] LoadScopes(IConfigurationSession session, ADObjectId[] scopeIds)
		{
			Result<ManagementScope>[] result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadScopes", AuthZLogHelper.AuthZPerfMonitors))
			{
				result = session.ReadMultiple<ManagementScope>(scopeIds);
			}
			return result;
		}

		protected virtual Result<ExchangeRole>[] LoadRoles(IConfigurationSession session, List<ADObjectId> roleIds)
		{
			Result<ExchangeRole>[] result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadRoles", AuthZLogHelper.AuthZPerfMonitors))
			{
				result = session.ReadMultiple<ExchangeRole>(roleIds.ToArray());
			}
			return result;
		}

		protected virtual Result<ExchangeRoleAssignment>[] LoadRoleAssignments(IConfigurationSession session, List<ADObjectId> implicitRoleIds)
		{
			return this.LoadRoleAssignments(session, this.executingUser, implicitRoleIds);
		}

		protected virtual Result<ExchangeRoleAssignment>[] LoadRoleAssignments(IConfigurationSession session, ADRawEntry user, List<ADObjectId> implicitRoleIds)
		{
			Result<ExchangeRoleAssignment>[] result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadRoleAssignments", AuthZLogHelper.AuthZPerfMonitors))
			{
				ADObjectId[] nestedSecurityGroupMembership = this.GetNestedSecurityGroupMembership(user);
				ADObjectId[] array = new ADObjectId[nestedSecurityGroupMembership.Length + implicitRoleIds.Count];
				Array.Copy(nestedSecurityGroupMembership, array, nestedSecurityGroupMembership.Length);
				Array.Copy(implicitRoleIds.ToArray(), 0, array, nestedSecurityGroupMembership.Length, implicitRoleIds.Count);
				if (array.Length == 0)
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string>((long)this.GetHashCode(), "Could not retrieve any SIDs for user {0} from AD, access denied", this.identityName);
					AuthZLogger.SafeAppendGenericError("ERC.LoadRoleAssignments", string.Format("Could not retrieve any SIDs for user {0} from AD", this.identityName), false);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoRoleAssignments, null, new object[]
					{
						this.IdentityName,
						this.executingUser.OriginatingServer
					});
					throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
				}
				OrganizationId orgId = (OrganizationId)user[ADObjectSchema.OrganizationId];
				ADObjectId adobjectId = null;
				SharedConfiguration sharedConfiguration = null;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "SharedConfiguration.GetSharedConfiguration", AuthZLogHelper.AuthZPerfMonitors))
				{
					sharedConfiguration = SharedConfiguration.GetSharedConfiguration(orgId);
				}
				if (sharedConfiguration != null)
				{
					using (new MonitoredScope("ExchangeRunspaceConfiguration", "sharedConfig.GetSharedRoleGroupIds", AuthZLogHelper.AuthZPerfMonitors))
					{
						array = sharedConfiguration.GetSharedRoleGroupIds(array);
					}
					if (user[ADRecipientSchema.RoleAssignmentPolicy] == null)
					{
						using (new MonitoredScope("ExchangeRunspaceConfiguration", "sharedConfig.GetSharedRoleAssignmentPolicy", AuthZLogHelper.AuthZPerfMonitors))
						{
							adobjectId = sharedConfiguration.GetSharedRoleAssignmentPolicy();
						}
					}
					if (array.Length == 0 && adobjectId == null)
					{
						ExTraceGlobals.AccessDeniedTracer.TraceError<string>((long)this.GetHashCode(), "User {0} is not member of any RoleGroup and Role Assignment Policy its null. Access Denied", this.identityName);
						AuthZLogger.SafeAppendGenericError("ERC.LoadRoleAssignments", string.Format("User {0} is not member of any RoleGroup and Role Assignment Policy its null", this.identityName), false);
						TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoRoleAssignments, null, new object[]
						{
							this.IdentityName,
							this.executingUser.OriginatingServer
						});
						throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
					}
				}
				else
				{
					adobjectId = (ADObjectId)user[ADRecipientSchema.RoleAssignmentPolicy];
					if (adobjectId == null)
					{
						using (new MonitoredScope("ExchangeRunspaceConfiguration", "RBACHelper.GetDefaultRoleAssignmentPolicy", AuthZLogHelper.AuthZPerfMonitors))
						{
							adobjectId = RBACHelper.GetDefaultRoleAssignmentPolicy(orgId);
						}
					}
				}
				if (adobjectId != null)
				{
					if (ExchangeRunspaceConfiguration.IsComputer(this.executingUser))
					{
						ExTraceGlobals.ADConfigTracer.TraceDebug(0L, "Role assignment policy check skipped because current user is the computer user.");
					}
					else if (!this.Impersonated && this.delegatedPrincipal != null)
					{
						ExTraceGlobals.ADConfigTracer.TraceDebug(0L, "Role assignment policy check skipped because current user is a delegated admin.");
					}
					else
					{
						array = RBACHelper.AddElementToStaticArray(array, adobjectId);
					}
				}
				if (array.Length == 0)
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string>((long)this.GetHashCode(), "Could not retrieve any SIDs for user {0} from AD, access denied - Right before FindRoleAssignmentsByUserIds", this.identityName);
					AuthZLogger.SafeAppendGenericError("ERC.LoadRoleAssignments", string.Format("Could not retrieve any SIDs for user {0} from AD, access denied - Right before FindRoleAssignmentsByUserIds", this.identityName), false);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoRoleAssignments, null, new object[]
					{
						this.IdentityName,
						this.executingUser.OriginatingServer
					});
					throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
				}
				this.universalSecurityGroupsCache = array;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "session.FindRoleAssignmentsByUserIds", AuthZLogHelper.AuthZPerfMonitors))
				{
					result = session.FindRoleAssignmentsByUserIds(array, this.PartnerMode);
				}
			}
			return result;
		}

		private static bool IsComputer(ADRawEntry executingUser)
		{
			return ((MultiValuedProperty<string>)executingUser[ADObjectSchema.ObjectClass]).Contains("computer");
		}

		private static bool SameScopeCollectionExists(List<ADScopeCollection> aggregatedScopes, List<ADScope> scopes)
		{
			for (int i = 0; i < aggregatedScopes.Count; i++)
			{
				if (aggregatedScopes[i].Count == scopes.Count)
				{
					bool flag = true;
					for (int j = 0; j < scopes.Count; j++)
					{
						if (!((RbacScope)scopes[j]).IsPresentInCollection(aggregatedScopes[i]))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void TrimAggregatedScopes(List<ADScopeCollection> aggregatedScopes, bool hiddenFromGAL)
		{
			int num = -1;
			bool flag = false;
			int i = 0;
			while (i < aggregatedScopes.Count)
			{
				ADScopeCollection adscopeCollection = aggregatedScopes[i];
				if (adscopeCollection.Count == 1)
				{
					RbacScope rbacScope = (RbacScope)adscopeCollection[0];
					bool flag2 = true;
					ScopeType scopeType = rbacScope.ScopeType;
					switch (scopeType)
					{
					case ScopeType.Organization:
						goto IL_4D;
					case ScopeType.MyGAL:
						num = i;
						break;
					default:
						if (scopeType == ScopeType.OrganizationConfig)
						{
							goto IL_4D;
						}
						flag2 = false;
						break;
					}
					IL_61:
					if (!flag2)
					{
						goto IL_65;
					}
					goto IL_AB;
					IL_4D:
					aggregatedScopes.RemoveAt(i);
					i--;
					goto IL_61;
				}
				goto IL_65;
				IL_AB:
				i++;
				continue;
				IL_65:
				if (flag)
				{
					goto IL_AB;
				}
				bool flag3 = true;
				for (int j = 0; j < adscopeCollection.Count; j++)
				{
					RbacScope rbacScope2 = (RbacScope)aggregatedScopes[i][j];
					if (!rbacScope2.IsScopeTypeSmallerThan(ScopeType.MyGAL, hiddenFromGAL))
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					flag = true;
					goto IL_AB;
				}
				goto IL_AB;
			}
			if (flag && num != -1)
			{
				aggregatedScopes.RemoveAt(num);
			}
		}

		private bool IsPowershellProcess()
		{
			if (ExchangeRunspaceConfiguration.isPowershellProcess == null)
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					ExchangeRunspaceConfiguration.isPowershellProcess = new bool?(currentProcess.MainModule.ModuleName.Equals("Powershell.exe", StringComparison.OrdinalIgnoreCase));
				}
			}
			return ExchangeRunspaceConfiguration.isPowershellProcess.Value;
		}

		private void LoadRoleCmdletInfo(string organizationName, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, List<ADObjectId> implicitRoleIds)
		{
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "LoadRoleCmdletInfo", AuthZLogHelper.AuthZPerfMonitors))
			{
				this.allScopes = null;
				this.allRoleEntries = null;
				this.allRoleTypes = null;
				this.allRoleAssignments = null;
				this.validationRuleCacheResults = new List<CachedValidationRuleResult>();
				IConfigurationSession session = null;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "GetTenantOrTopologyConfigurationSession", AuthZLogHelper.AuthZPerfMonitors))
				{
					session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.GetADSessionSettings(), 4036, "LoadRoleCmdletInfo", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
				}
				if (this.intersectRoleEntries)
				{
					Dictionary<ADObjectId, ManagementScope> dictionary;
					List<RoleEntryInfo> userAllRoleEntries;
					Dictionary<RoleType, List<ADObjectId>> dictionary2;
					Microsoft.Exchange.Collections.ReadOnlyCollection<ExchangeRoleAssignment> readOnlyCollection;
					this.LoadRoleCmdletInfo(this.logonUser, this.executingUser, session, null, null, null, implicitRoleIds, RoleFilteringMode.DiscardEndUserRole, this.logonAccessToken, out dictionary, out userAllRoleEntries, out dictionary2, out readOnlyCollection);
					if (logonUserRequiredRoleTypes != null)
					{
						bool flag = false;
						foreach (RoleType item in dictionary2.Keys)
						{
							if ((roleTypeFilter == null || roleTypeFilter.Contains(item)) && logonUserRequiredRoleTypes.Contains(item))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							throw new CmdletAccessDeniedException(Strings.NoRequiredRole(this.identityName));
						}
					}
					List<RoleEntry> list;
					List<RoleEntry> list2;
					this.CombineRoleEntries(userAllRoleEntries, out list, out list2);
					sortedRoleEntryFilter = list;
				}
				this.LoadRoleCmdletInfo(this.executingUser, null, session, organizationName, roleTypeFilter, sortedRoleEntryFilter, implicitRoleIds, this.intersectRoleEntries ? RoleFilteringMode.KeepOnlyEndUserRole : ((!this.Impersonated && this.delegatedPrincipal != null) ? RoleFilteringMode.DiscardEndUserRole : RoleFilteringMode.NoFiltering), this.UserAccessToken, out this.allScopes, out this.allRoleEntries, out this.allRoleTypes, out this.allRoleAssignments);
				if (this.executingUser == null)
				{
					this.ConfigurationSettings.VariantConfigurationSnapshot = null;
				}
				else
				{
					this.ConfigurationSettings.VariantConfigurationSnapshot = CmdletFlight.GetSnapshot(this.executingUser, this.ConfigurationSettings.AdditionalConstraints);
				}
				CmdletFlight.FilterCmdletsAndParams(this.ConfigurationSettings.VariantConfigurationSnapshot, this.allRoleEntries);
			}
		}

		private void LoadRoleCmdletInfo(ADRawEntry user, ADRawEntry userToVerifyInScope, IConfigurationSession session, string organizationName, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, List<ADObjectId> implicitRoleIds, RoleFilteringMode roleFilteringMode, SerializedAccessToken securityAccessToken, out Dictionary<ADObjectId, ManagementScope> userAllScopes, out List<RoleEntryInfo> userAllRoleEntries, out Dictionary<RoleType, List<ADObjectId>> userAllRoleTypes, out Microsoft.Exchange.Collections.ReadOnlyCollection<ExchangeRoleAssignment> userAllRoleAssignments)
		{
			Result<ExchangeRoleAssignment>[] array = this.LoadRoleAssignments(session, user, implicitRoleIds);
			AuthZLogger.SafeAppendGenericInfo("RoleAssignments", this.GetUserFriendlyRoleAssignmentInfo(array));
			if (array.Length == 0)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceError<object, string>((long)this.GetHashCode(), "User {0} ({1}) has no role assignments associated, access denied", user[ADRecipientSchema.PrimarySmtpAddress], this.identityName);
				AuthZLogger.SafeAppendGenericError("ERC.LoadRoleCmdletInfo", string.Format("User {0} ({1}) has no role assignments associated, access denied", user[ADRecipientSchema.PrimarySmtpAddress], this.identityName), false);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoRoleAssignments, null, new object[]
				{
					this.IdentityName,
					session.Source
				});
				throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
			}
			List<ADObjectId> list = new List<ADObjectId>(array.Length);
			List<ExchangeRoleAssignment> list2 = new List<ExchangeRoleAssignment>(array.Length);
			List<ExchangeRoleAssignment> list3 = new List<ExchangeRoleAssignment>(array.Length);
			List<ExchangeRoleAssignment> list4 = new List<ExchangeRoleAssignment>(array.Length);
			userAllScopes = new Dictionary<ADObjectId, ManagementScope>();
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.CheckForDedicatedTenantAdminRoleNamePrefix.Enabled;
			AuthZLogger.SafeAppendGenericInfo("roleAssignmentResults.Length", array.Length.ToString());
			for (int i = 0; i < array.Length; i++)
			{
				ExchangeRoleAssignment data = array[i].Data;
				ExTraceGlobals.ADConfigTracer.TraceDebug((long)this.GetHashCode(), "Found {0} RoleAssignment {1} for role {2}: {3}", new object[]
				{
					data.RoleAssignmentDelegationType,
					data.Id,
					data.Role,
					data.Enabled ? "Enabled" : "Disabled"
				});
				if (data.Enabled)
				{
					if (data.CustomRecipientWriteScope != null)
					{
						userAllScopes[data.CustomRecipientWriteScope] = null;
					}
					if (data.CustomConfigWriteScope != null)
					{
						userAllScopes[data.CustomConfigWriteScope] = null;
					}
					if (data.RoleAssignmentDelegationType == RoleAssignmentDelegationType.Regular)
					{
						list4.Add(data);
						list.Add(data.Role);
					}
					else
					{
						list3.Add(data);
					}
				}
			}
			if (list4.Count == 0)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceError<string>((long)this.GetHashCode(), "User {0} has no valid and enabled Regular role assignments associated, access denied", this.identityName);
				AuthZLogger.SafeAppendGenericError("ERC.LoadRoleCmdletInfo", string.Format("User {0} has no valid and enabled Regular role assignments associated, access denied", this.identityName), false);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoValidEnabledRoleAssignments, null, new object[]
				{
					this.IdentityName,
					session.Source
				});
				throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
			}
			this.ReadAndCheckAllScopes(session, userAllScopes, organizationName);
			Result<ExchangeRole>[] array2 = this.LoadRoles(session, list);
			userAllRoleEntries = new List<RoleEntryInfo>(100);
			userAllRoleTypes = new Dictionary<RoleType, List<ADObjectId>>(array2.Length);
			bool flag = false;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "ProcessRoleAssignmentsAndAssociatedRoles", AuthZLogHelper.AuthZPerfMonitors))
			{
				AuthZLogger.SafeAppendGenericInfo("roleResults.Length", array2.Length.ToString());
				for (int j = 0; j < array2.Length; j++)
				{
					Result<ExchangeRole> result = array2[j];
					if (result.Data == null)
					{
						ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Role {0} was not found", list[j]);
					}
					else
					{
						list4[j].IsFromEndUserRole = result.Data.IsEndUserRole;
						if (enabled && !this.isDedicatedTenantAdmin && result.Data.Name.StartsWith("SSA_", StringComparison.OrdinalIgnoreCase) && list4[j].RoleAssigneeType != RoleAssigneeType.RoleAssignmentPolicy)
						{
							this.isDedicatedTenantAdmin = true;
						}
						ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>(0L, "Read Role {0} of type {1}", result.Data.Id, result.Data.RoleType);
						if (roleFilteringMode == RoleFilteringMode.DiscardEndUserRole)
						{
							if (result.Data.IsEndUserRole)
							{
								list4[j] = null;
								ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>(0L, "RoleFilteringMode is DiscardEndUserRole. Role {0} of type {1} skipped because it is an end user role.", result.Data.Id, result.Data.RoleType);
								goto IL_83A;
							}
						}
						else if (roleFilteringMode == RoleFilteringMode.KeepOnlyEndUserRole && !result.Data.IsEndUserRole)
						{
							list4[j] = null;
							ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>(0L, "RoleFilteringMode is KeepOnlyEndUserRole. Role {0} of type {1} skipped because it isn't an end user role.", result.Data.Id, result.Data.RoleType);
							goto IL_83A;
						}
						if (list4[j].RoleAssigneeType == RoleAssigneeType.RoleAssignmentPolicy && !result.Data.IsEndUserRole)
						{
							list4[j] = null;
							ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>(0L, "Role {0} of type {1} skipped because it is assigned through RoleAssignmentPolicy but is not end-user role.", result.Data.Id, result.Data.RoleType);
						}
						else if (roleTypeFilter != null && !roleTypeFilter.Contains(result.Data.RoleType))
						{
							list4[j] = null;
							ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>(0L, "Role {0} of type {1} skipped due to not existed in rolefilter.", result.Data.Id, result.Data.RoleType);
						}
						else
						{
							if (userToVerifyInScope != null)
							{
								if (result.Data.RoleType == RoleType.Custom || result.Data.RoleType >= RoleType.UnScoped)
								{
									list4[j] = null;
									ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>((long)this.GetHashCode(), "Role {0} skipped because it is type {1}.", result.Data.Id, result.Data.RoleType);
									goto IL_83A;
								}
								RoleAssignmentScopeSet effectiveScopeSet = list4[j].GetEffectiveScopeSet(userAllScopes, securityAccessToken);
								if (effectiveScopeSet == null)
								{
									list4[j] = null;
									ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType>((long)this.GetHashCode(), "Role {0} of type {1} skipped because its effective scope set is null.", result.Data.Id, result.Data.RoleType);
									goto IL_83A;
								}
								OrganizationId organizationId = (OrganizationId)user[ADObjectSchema.OrganizationId];
								effectiveScopeSet.RecipientReadScope.PopulateRootAndFilter(organizationId, user);
								effectiveScopeSet.RecipientWriteScope.PopulateRootAndFilter(organizationId, user);
								ADScopeException ex;
								if (!ADSession.TryVerifyIsWithinScopes(userToVerifyInScope, effectiveScopeSet.RecipientReadScope, new ADScopeCollection[]
								{
									new ADScopeCollection(new ADScope[]
									{
										effectiveScopeSet.RecipientWriteScope
									})
								}, this.exclusiveRecipientScopesCollection, false, out ex))
								{
									list4[j] = null;
									ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId, RoleType, string>((long)this.GetHashCode(), "Role {0} named {2} of type {1} skipped because its RecipientScope doesn't include the impersonated user.", result.Data.Id, result.Data.RoleType, result.Data.Name);
									goto IL_83A;
								}
							}
							flag = (RoleType.UnScoped != result.Data.RoleType && result.Data.ExchangeVersion.IsOlderThan(result.Data.MaximumSupportedExchangeObjectVersion) && (j == 0 || flag));
							int count = userAllRoleEntries.Count;
							foreach (RoleEntry roleEntry in result.Data.RoleEntries)
							{
								ExTraceGlobals.ADConfigTracer.TraceDebug<RoleEntry>((long)this.GetHashCode(), "Got Entry {0}", roleEntry);
								RoleEntry roleEntry2 = roleEntry;
								if (sortedRoleEntryFilter != null)
								{
									int num = sortedRoleEntryFilter.BinarySearch(roleEntry, RoleEntry.NameComparer);
									if (num < 0)
									{
										ExTraceGlobals.ADConfigTracer.TraceDebug<RoleEntry>((long)this.GetHashCode(), "Entry {0} skipped due to not existed in entry filter.", roleEntry);
										continue;
									}
									if (this.intersectRoleEntries)
									{
										roleEntry2 = roleEntry.IntersectParameters(sortedRoleEntryFilter[num]);
									}
								}
								if (roleEntry2.Parameters.Count != 0)
								{
									if (result.Data.IsEndUserRole)
									{
										roleEntry2 = this.TrimRoleEntryParametersWithValidationRules(roleEntry2, false);
										if (roleEntry2.Parameters.Count == 0)
										{
											ExTraceGlobals.ADConfigTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "Entry '{0}' skipped due to validation rules filtering. Role '{1}'", roleEntry, result.Data.Name);
											continue;
										}
									}
									else
									{
										roleEntry2 = this.TrimRoleEntryParametersWithValidationRules(roleEntry2, true);
										if (roleEntry2.Parameters.Count == 0)
										{
											ExTraceGlobals.ADConfigTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "Entry '{0}' skipped due to validation rules filtering. Role '{1}'", roleEntry, result.Data.Name);
											continue;
										}
									}
								}
								userAllRoleEntries.Add(new RoleEntryInfo(roleEntry2, list4[j]));
							}
							List<ADObjectId> list5 = null;
							if (!userAllRoleTypes.TryGetValue(result.Data.RoleType, out list5))
							{
								list5 = new List<ADObjectId>();
								userAllRoleTypes[result.Data.RoleType] = list5;
							}
							list5.Add(result.Data.Id);
						}
					}
					IL_83A:;
				}
			}
			if (flag)
			{
				throw new NonMigratedUserDeniedException(Strings.NonMigratedUserException(this.identityName));
			}
			if (userAllRoleEntries.Count == 0 && !this.NoCmdletAllowed)
			{
				throw new CmdletAccessDeniedException(Strings.NoRoleAssignmentsFound(this.identityName));
			}
			for (int k = list4.Count - 1; k >= 0; k--)
			{
				if (list4[k] == null)
				{
					list4.RemoveAt(k);
				}
			}
			list2.AddRange(list4);
			list2.AddRange(list3);
			if (ExchangeAuthorizationPlugin.ShouldShowFismaBanner)
			{
				userAllRoleEntries.Add(ExchangeRunspaceConfiguration.welcomeMessageScriptInitializer);
			}
			userAllRoleAssignments = new Microsoft.Exchange.Collections.ReadOnlyCollection<ExchangeRoleAssignment>(list2);
			userAllRoleEntries.Sort(RoleEntryInfo.NameAndInstanceHashCodeComparer);
			this.AddClientSpecificParameters(ref userAllRoleEntries);
			this.AddClientSpecificRoleEntries(ref userAllRoleEntries);
		}

		private void ReadAndCheckAllScopes(IConfigurationSession session, Dictionary<ADObjectId, ManagementScope> userAllScopes, string tenantOrganizationName)
		{
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "ReadAndCheckAllScopes", AuthZLogHelper.AuthZPerfMonitors))
			{
				OrganizationId organizationId = null;
				int num = 0;
				if (userAllScopes.Keys.Count != 0)
				{
					ADObjectId[] array = new ADObjectId[userAllScopes.Keys.Count];
					userAllScopes.Keys.CopyTo(array, 0);
					Result<ManagementScope>[] array2 = this.LoadScopes(session, array);
					for (int i = 0; i < array2.Length; i++)
					{
						userAllScopes[array[i]] = null;
						Result<ManagementScope> result = array2[i];
						if (result.Data == null)
						{
							ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Scope {0} was not found", array[i]);
							userAllScopes.Remove(array[i]);
						}
						else
						{
							ExTraceGlobals.ADConfigTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Read scope {0}", result.Data.Id);
							if (this.PartnerMode != (result.Data.ScopeRestrictionType == ScopeRestrictionType.PartnerDelegatedTenantScope))
							{
								ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId, ScopeRestrictionType, string>((long)this.GetHashCode(), "We somehow read ManagementScope {0} of type {1} which is not allowed in {2}Partner mode. Ignoring the scope (and associated role assignments)", result.Data.Id, result.Data.ScopeRestrictionType, this.PartnerMode ? "" : "non-");
							}
							else if (ExchangeRunspaceConfiguration.TryStampQueryFilterOnManagementScope(result.Data))
							{
								userAllScopes[array[i]] = result.Data;
								num++;
							}
						}
					}
				}
				if (this.PartnerMode)
				{
					this.ValidateTenantOrganizationAndPartnerScopes(session, num, userAllScopes, tenantOrganizationName, out organizationId);
				}
				else
				{
					this.LoadAllExclusiveScopes(session, userAllScopes);
				}
				this.allScopes = userAllScopes;
				this.organizationId = organizationId;
			}
		}

		private void LoadAllExclusiveScopes(IConfigurationSession session, Dictionary<ADObjectId, ManagementScope> allScopes)
		{
			ManagementScope[] array = this.LoadExclusiveScopes();
			this.exclusiveRecipientScopes = new List<ADScope>();
			Dictionary<string, List<ADScope>> dictionary = new Dictionary<string, List<ADScope>>(2);
			ManagementScope[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				ManagementScope managementScope = array2[i];
				if (!allScopes.ContainsKey(managementScope.Id))
				{
					ExchangeRunspaceConfiguration.TryStampQueryFilterOnManagementScope(managementScope);
					allScopes[managementScope.Id] = managementScope;
				}
				switch (managementScope.ScopeRestrictionType)
				{
				case ScopeRestrictionType.RecipientScope:
				{
					RbacScope rbacScope = new RbacScope(ScopeType.ExclusiveRecipientScope, allScopes[managementScope.Id]);
					rbacScope.PopulateRootAndFilter(this.organizationId, this.executingUser);
					this.exclusiveRecipientScopes.Add(rbacScope);
					break;
				}
				case ScopeRestrictionType.ServerScope:
				{
					RbacScope rbacScope = new RbacScope(ScopeType.ExclusiveConfigScope, allScopes[managementScope.Id]);
					rbacScope.PopulateRootAndFilter(this.organizationId, this.executingUser);
					RBACHelper.AddValueToDictionaryList<ADScope>(dictionary, "Server", rbacScope);
					break;
				}
				case ScopeRestrictionType.PartnerDelegatedTenantScope:
					goto IL_111;
				case ScopeRestrictionType.DatabaseScope:
				{
					RbacScope rbacScope = new RbacScope(ScopeType.ExclusiveConfigScope, allScopes[managementScope.Id]);
					rbacScope.PopulateRootAndFilter(this.organizationId, this.executingUser);
					RBACHelper.AddValueToDictionaryList<ADScope>(dictionary, "Database", rbacScope);
					break;
				}
				default:
					goto IL_111;
				}
				IL_133:
				i++;
				continue;
				IL_111:
				ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId, ScopeRestrictionType>((long)this.GetHashCode(), "We somehow read ManagementScope {0} of type {1} which is not allowed as an exclusive scope. Ignoring the scope.", managementScope.Id, managementScope.ScopeRestrictionType);
				goto IL_133;
			}
			if (this.exclusiveRecipientScopes == null || this.exclusiveRecipientScopes.Count == 0)
			{
				this.exclusiveRecipientScopesCollection = ADScopeCollection.Empty;
			}
			else
			{
				this.exclusiveRecipientScopesCollection = new ADScopeCollection(this.exclusiveRecipientScopes);
			}
			if (dictionary.Count > 0)
			{
				this.exclusiveObjectSpecificConfigScopes = new Dictionary<string, ADScopeCollection>(dictionary.Count);
				foreach (KeyValuePair<string, List<ADScope>> keyValuePair in dictionary)
				{
					this.exclusiveObjectSpecificConfigScopes[keyValuePair.Key] = new ADScopeCollection(keyValuePair.Value);
				}
			}
		}

		private void ValidateTenantOrganizationAndPartnerScopes(IConfigurationSession session, int validScopesCount, Dictionary<ADObjectId, ManagementScope> scopesDictionary, string organizationName, out OrganizationId orgId)
		{
			orgId = null;
			if (validScopesCount == 0)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceError<string, string>((long)this.GetHashCode(), "No partner scopes found for {0} while requested partner access to {1}. Returning Access Denied", this.identityName, organizationName);
				AuthZLogger.SafeAppendGenericError("ERC.ValidateTenantOrganizationAndPartnerScopes", string.Format("No partner scopes found for {0} while requested partner access to {1}. Returning Access Denied", this.identityName, organizationName), false);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_NoPartnerScopes, null, new object[]
				{
					this.identityName,
					session.Source
				});
				throw new CmdletAccessDeniedException(Strings.ErrorNoPartnerScopes(this.identityName));
			}
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantCUName(organizationName), 4816, "ValidateTenantOrganizationAndPartnerScopes", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByNameOrAcceptedDomain = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(organizationName);
			if (exchangeConfigurationUnitByNameOrAcceptedDomain == null)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceError<string, string>((long)this.GetHashCode(), "Organization {0} not found (User {1} requested partner access to it). Returning Access Denied", organizationName, this.identityName);
				AuthZLogger.SafeAppendGenericError("ERC.ValidateTenantOrganizationAndPartnerScopes", string.Format("Organization {0} not found (User {1} requested partner access to it). Returning Access Denied", organizationName, this.identityName), false);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_OrgNotFound, null, new object[]
				{
					this.identityName,
					organizationName,
					tenantConfigurationSession.Source
				});
				throw new CmdletAccessDeniedException(Strings.ErrorOrgNotFound(this.identityName, organizationName));
			}
			TenantOrganizationPresentationObject tenantOrganizationPresentationObject = new TenantOrganizationPresentationObject(exchangeConfigurationUnitByNameOrAcceptedDomain);
			List<ADObjectId> list = new List<ADObjectId>(scopesDictionary.Count);
			foreach (ManagementScope managementScope in scopesDictionary.Values)
			{
				if (OpathFilterEvaluator.FilterMatches(managementScope.QueryFilter, tenantOrganizationPresentationObject))
				{
					ExTraceGlobals.ADConfigTracer.TraceDebug((long)this.GetHashCode(), "ManagementScope {0} with filter {1} DID match organization {2}({3}), setting runspace OrganizationId to the requested value", new object[]
					{
						managementScope.Id,
						managementScope.Filter,
						organizationName,
						tenantOrganizationPresentationObject.Name
					});
				}
				else
				{
					ExTraceGlobals.ADConfigTracer.TraceWarning((long)this.GetHashCode(), "ManagementScope {0} with filter {1} did not match organization {2}({3})", new object[]
					{
						managementScope.Id,
						managementScope.Filter,
						organizationName,
						tenantOrganizationPresentationObject.Name
					});
					list.Add(managementScope.Id);
					validScopesCount--;
				}
			}
			foreach (ADObjectId key in list)
			{
				scopesDictionary[key] = null;
			}
			if (validScopesCount == 0)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceError<string, string, string>((long)this.GetHashCode(), "No matching partner scopes found for organization {0}({1}) (User {2} requested partner access to it). Returning Access Denied", organizationName, tenantOrganizationPresentationObject.Name, this.identityName);
				AuthZLogger.SafeAppendGenericError("ERC.ValidateTenantOrganizationAndPartnerScopes", string.Format("No matching partner scopes found for organization {0}({1}) (User {2} requested partner access to it). Returning Access Denied", organizationName, tenantOrganizationPresentationObject.Name, this.identityName), false);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_AccessDenied_OrgOutOfPartnerScope, null, new object[]
				{
					this.identityName,
					organizationName
				});
				throw new CmdletAccessDeniedException(Strings.ErrorOrgOutOfPartnerScope(this.identityName, organizationName));
			}
			orgId = exchangeConfigurationUnitByNameOrAcceptedDomain.OrganizationId;
		}

		private void CombineRoleEntries(List<RoleEntryInfo> userAllRoleEntries, out List<RoleEntry> combinedCmdlets, out List<RoleEntry> combinedScripts)
		{
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "CombineRoleEntries", AuthZLogHelper.AuthZPerfMonitors))
			{
				combinedCmdlets = new List<RoleEntry>(userAllRoleEntries.Count);
				combinedScripts = new List<RoleEntry>(5);
				List<RoleEntry> list = combinedScripts;
				int num = 0;
				for (int i = 1; i <= userAllRoleEntries.Count; i++)
				{
					if (i == userAllRoleEntries.Count || RoleEntry.CompareRoleEntriesByName(userAllRoleEntries[i - 1].RoleEntry, userAllRoleEntries[i].RoleEntry) != 0)
					{
						RoleEntry[] array = new RoleEntry[i - num];
						for (int j = num; j <= i - 1; j++)
						{
							array[j - num] = userAllRoleEntries[j].RoleEntry;
						}
						RoleEntry roleEntry = RoleEntry.MergeParameters(array);
						if (list == combinedScripts && userAllRoleEntries[i - 1].RoleEntry is CmdletRoleEntry)
						{
							ExTraceGlobals.ADConfigTracer.TraceDebug((long)this.GetHashCode(), "Switching to cmdlets");
							list = combinedCmdlets;
						}
						list.Add(roleEntry);
						ExTraceGlobals.ADConfigTracer.TraceDebug<RoleEntry>((long)this.GetHashCode(), "Added merged entry {0}", roleEntry);
						num = i;
					}
				}
			}
		}

		protected bool LocateRoleEntriesForCmdlet(string cmdletName, out int firstIndex, out int lastIndex)
		{
			firstIndex = -1;
			lastIndex = -1;
			RoleEntryInfo roleInfoForCmdlet;
			try
			{
				roleInfoForCmdlet = RoleEntryInfo.GetRoleInfoForCmdlet(cmdletName);
			}
			catch (FormatException innerException)
			{
				throw new ArgumentException("cmdletName", innerException);
			}
			int num = this.allRoleEntries.BinarySearch(roleInfoForCmdlet, RoleEntryInfo.NameComparer);
			if (num < 0)
			{
				ExTraceGlobals.AccessCheckTracer.TraceWarning<string, string>((long)this.GetHashCode(), "LocateRoleEntriesForCmdlet({0}) returns false for user {1} because this cmdlet is not in role", cmdletName, this.identityName);
				return false;
			}
			firstIndex = num;
			lastIndex = num;
			while (firstIndex > 0)
			{
				if (RoleEntry.CompareRoleEntriesByName(this.allRoleEntries[firstIndex - 1].RoleEntry, roleInfoForCmdlet.RoleEntry) != 0)
				{
					break;
				}
				firstIndex--;
			}
			while (lastIndex < this.allRoleEntries.Count - 1 && RoleEntry.CompareRoleEntriesByName(this.allRoleEntries[lastIndex + 1].RoleEntry, roleInfoForCmdlet.RoleEntry) == 0)
			{
				lastIndex++;
			}
			return true;
		}

		protected virtual ADObjectId[] GetNestedSecurityGroupMembership(ADRawEntry user)
		{
			ADObjectId[] nestedSecurityGroupMembership;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "GetNestedSecurityGroupMembership", AuthZLogHelper.AuthZPerfMonitors))
			{
				ADSessionSettings sessionSettings;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "FromRootOrgScopeSet", AuthZLogHelper.AuthZPerfMonitors))
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "FromAllTenantsOrRootOrgAutoDetect", AuthZLogHelper.AuthZPerfMonitors))
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled && user != null && user.Id != null)
					{
						sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(user.Id);
					}
				}
				IRecipientSession tenantOrRootOrgRecipientSession;
				using (new MonitoredScope("ExchangeRunspaceConfiguration", "GetNestedSecurityGroupMembership.GetTenantOrRootOrgRecipientSession", AuthZLogHelper.AuthZPerfMonitors))
				{
					tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 5087, "GetNestedSecurityGroupMembership", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
				}
				nestedSecurityGroupMembership = ExchangeRunspaceConfiguration.GetNestedSecurityGroupMembership(user, tenantOrRootOrgRecipientSession, AssignmentMethod.All);
			}
			return nestedSecurityGroupMembership;
		}

		protected static ADObjectId[] GetNestedSecurityGroupMembership(ADRawEntry user, IRecipientSession recipientSession, AssignmentMethod assignmentMethod)
		{
			List<string> list;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "recipientSession.GetTokenSids", AuthZLogHelper.AuthZPerfMonitors))
			{
				list = recipientSession.GetTokenSids(user, assignmentMethod);
			}
			if (list.Count == 0)
			{
				return new ADObjectId[0];
			}
			ADSessionSettings sessionSettings;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "FromAllTenantsOrRootOrgAutoDetectOrFromRootOrgScopeSet", AuthZLogHelper.AuthZPerfMonitors))
			{
				sessionSettings = ((VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled && user.Id != null && user.Id.DomainId != null) ? ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(user.Id) : ADSessionSettings.FromRootOrgScopeSet());
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 5124, "GetNestedSecurityGroupMembership", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeRunspaceConfiguration.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			ADObjectId[] result;
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "gcSession.ResolveSidsToADObjectIds", AuthZLogHelper.AuthZPerfMonitors))
			{
				result = tenantOrRootOrgRecipientSession.ResolveSidsToADObjectIds(list.ToArray());
			}
			return result;
		}

		private bool LocateRoleEntriesForScript(string scriptName)
		{
			RoleEntryInfo roleInfoForScript;
			try
			{
				roleInfoForScript = RoleEntryInfo.GetRoleInfoForScript(scriptName);
			}
			catch (FormatException innerException)
			{
				throw new ArgumentException("scriptName", innerException);
			}
			int num = this.allRoleEntries.BinarySearch(roleInfoForScript, RoleEntryInfo.NameComparer);
			if (num < 0)
			{
				ExTraceGlobals.AccessCheckTracer.TraceWarning<string, string>((long)this.GetHashCode(), "LocateRoleEntriesForScript({0}) returns false for user {1} because this script is not in role", scriptName, this.identityName);
				return false;
			}
			return true;
		}

		private void TracePublicInstanceAPIResult(bool result, string format, params object[] parameters)
		{
			if (result)
			{
				ExTraceGlobals.PublicInstanceAPITracer.TraceDebug((long)this.GetHashCode(), format, parameters);
				return;
			}
			ExTraceGlobals.PublicInstanceAPITracer.TraceWarning((long)this.GetHashCode(), format, parameters);
		}

		private List<RoleAssignmentScopeSet>[] CalculateScopesForEachParameter(string exchangeCmdletName, IList<string> parameters, OrganizationId organizationId, Task.ErrorLoggerDelegate writeError, int firstIndex, int lastIndex, ScopeType[] maximumScopesCommonForAllParams, ScopeType[,] maxParameterScopes)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = parameters.Count == 0;
			if (flag7)
			{
				parameters = ExchangeRunspaceConfiguration.noParameterArray;
			}
			List<RoleAssignmentScopeSet>[] array = new List<RoleAssignmentScopeSet>[parameters.Count];
			for (int i = 0; i < parameters.Count; i++)
			{
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = false;
				bool flag11 = false;
				bool flag12 = false;
				bool flag13 = false;
				bool flag14 = false;
				bool flag15 = false;
				List<RoleAssignmentScopeSet> list = new List<RoleAssignmentScopeSet>();
				for (int j = firstIndex; j <= lastIndex; j++)
				{
					if (flag7 || this.allRoleEntries[j].RoleEntry.ContainsParameter(parameters[i]))
					{
						RoleAssignmentScopeSet scopeSet = this.allRoleEntries[j].ScopeSet;
						if (scopeSet != null)
						{
							list.Add(scopeSet);
							if (scopeSet.RecipientReadScope.ScopeType == ScopeType.Organization)
							{
								flag8 = true;
								flag10 = true;
							}
							if (scopeSet.RecipientWriteScope.ScopeType == ScopeType.Organization)
							{
								flag9 = true;
								flag11 = true;
							}
							if (scopeSet.RecipientReadScope.ScopeType == ScopeType.MyGAL)
							{
								flag10 = true;
							}
							if (scopeSet.RecipientWriteScope.ScopeType == ScopeType.MyGAL)
							{
								flag11 = true;
							}
							if (scopeSet.ConfigReadScope.ScopeType == ScopeType.OrganizationConfig)
							{
								flag13 = true;
							}
							if (scopeSet.ConfigWriteScope.ScopeType == ScopeType.OrganizationConfig)
							{
								flag14 = true;
							}
							if (scopeSet.RecipientWriteScope.ScopeType == ScopeType.ExclusiveRecipientScope)
							{
								flag12 = true;
							}
							if (scopeSet.ConfigWriteScope.ScopeType == ScopeType.ExclusiveConfigScope)
							{
								flag15 = true;
							}
						}
					}
				}
				if (list.Count == 0)
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string, string, string>((long)this.GetHashCode(), "CalculateScopeSetForExchangeCmdlet({0}) returns null (NO SCOPE) for user {1} because no role assignments with parameter {2} were found.", exchangeCmdletName, this.identityName, parameters[i]);
					if (writeError != null)
					{
						AuthZLogger.SafeAppendGenericError("ERC.CalculateScopeSetForExchangeCmdlet", string.Format("CalculateScopeSetForExchangeCmdlet({0}) returns null (NO SCOPE) for user {1} because no role assignments with parameter {2} were found.", exchangeCmdletName, this.identityName, parameters[i]), false);
						TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_CmdletAccessDenied_InvalidParameter, this.IdentityName + exchangeCmdletName, new object[]
						{
							exchangeCmdletName,
							this.IdentityName
						});
						writeError(new CmdletAccessDeniedException(Strings.NoRoleEntriesWithParameterFound(exchangeCmdletName, parameters[i])), (ExchangeErrorCategory)18, null);
					}
					return null;
				}
				if (flag12)
				{
					flag11 = false;
					flag9 = false;
				}
				if (flag15)
				{
					flag14 = false;
				}
				array[i] = list;
				flag = ((flag || i == 0) && flag8);
				flag2 = ((flag2 || i == 0) && flag9);
				flag3 = ((flag3 || i == 0) && flag10);
				flag4 = ((flag4 || i == 0) && flag11);
				flag5 = ((flag5 || i == 0) && flag13);
				flag6 = ((flag6 || i == 0) && flag14);
				maxParameterScopes[i, 0] = (flag8 ? ScopeType.Organization : (flag10 ? ScopeType.MyGAL : ScopeType.None));
				maxParameterScopes[i, 1] = (flag9 ? ScopeType.Organization : (flag11 ? ScopeType.MyGAL : ScopeType.None));
				maxParameterScopes[i, 2] = (flag13 ? ScopeType.OrganizationConfig : ScopeType.None);
				maxParameterScopes[i, 3] = (flag14 ? ScopeType.OrganizationConfig : ScopeType.None);
			}
			if (flag)
			{
				maximumScopesCommonForAllParams[0] = ScopeType.Organization;
			}
			else if (flag3)
			{
				maximumScopesCommonForAllParams[0] = ScopeType.MyGAL;
			}
			if (flag2)
			{
				maximumScopesCommonForAllParams[1] = ScopeType.Organization;
			}
			else if (flag4)
			{
				maximumScopesCommonForAllParams[1] = ScopeType.MyGAL;
			}
			if (flag5)
			{
				maximumScopesCommonForAllParams[2] = ScopeType.OrganizationConfig;
			}
			if (flag6)
			{
				maximumScopesCommonForAllParams[3] = ScopeType.OrganizationConfig;
			}
			return array;
		}

		private ScopeSet AggregateScopes(OrganizationId organizationId, List<RoleAssignmentScopeSet>[] parameterScopes, ScopeType[] maximumScopesCommonForAllParams, ScopeType[,] maxParameterScopes)
		{
			RbacScope rbacScope = new RbacScope(maximumScopesCommonForAllParams[2]);
			rbacScope.PopulateRootAndFilter(organizationId, this.executingUser);
			List<ADScopeCollection> combinableScopeCollections = this.AggregateScopeForLocation(ScopeLocation.RecipientRead, organizationId, parameterScopes, maximumScopesCommonForAllParams, maxParameterScopes, this.ExecutingUserIsHiddenFromGAL);
			List<ADScopeCollection> recipientWriteScopes = this.AggregateScopeForLocation(ScopeLocation.RecipientWrite, organizationId, parameterScopes, maximumScopesCommonForAllParams, maxParameterScopes, this.ExecutingUserIsHiddenFromGAL);
			ADScope recipientReadScope = ADScope.CombineScopeCollections(combinableScopeCollections);
			ADScope configWriteScope = null;
			Dictionary<string, IList<ADScopeCollection>> dictionary = null;
			if (rbacScope.ScopeType == ScopeType.None)
			{
				configWriteScope = ADScope.NoAccess;
			}
			else
			{
				bool flag = true;
				List<ADScopeCollection> list = this.AggregateScopeForLocation(ScopeLocation.ConfigWrite, organizationId, parameterScopes, maximumScopesCommonForAllParams, maxParameterScopes, this.ExecutingUserIsHiddenFromGAL);
				if (list.Count == 1 && list[0].Count == 1)
				{
					using (IEnumerator<ADScope> enumerator = list[0].GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ADScope adscope = enumerator.Current;
							if (((RbacScope)adscope).ScopeType != ScopeType.CustomConfigScope)
							{
								flag = false;
								configWriteScope = adscope;
							}
						}
					}
				}
				if (flag)
				{
					dictionary = new Dictionary<string, IList<ADScopeCollection>>();
					Dictionary<string, List<ADScope>> dictionary2 = null;
					List<ADScope> list2 = null;
					foreach (ADScopeCollection adscopeCollection in list)
					{
						dictionary2 = new Dictionary<string, List<ADScope>>();
						list2 = new List<ADScope>();
						foreach (ADScope adscope2 in adscopeCollection)
						{
							RbacScope rbacScope2 = adscope2 as RbacScope;
							if (rbacScope2 != null)
							{
								string key;
								switch (rbacScope2.ScopeRestrictionType)
								{
								case ScopeRestrictionType.NotApplicable:
									list2.Add(rbacScope2);
									continue;
								case ScopeRestrictionType.DomainScope_Obsolete:
								case ScopeRestrictionType.RecipientScope:
								case ScopeRestrictionType.PartnerDelegatedTenantScope:
									continue;
								case ScopeRestrictionType.ServerScope:
									key = "Server";
									break;
								case ScopeRestrictionType.DatabaseScope:
									key = "Database";
									break;
								default:
									continue;
								}
								RBACHelper.AddValueToDictionaryList<ADScope>(dictionary2, key, rbacScope2);
							}
						}
						foreach (KeyValuePair<string, List<ADScope>> keyValuePair in dictionary2)
						{
							IList<ADScopeCollection> list3 = null;
							if (!dictionary.TryGetValue(keyValuePair.Key, out list3))
							{
								list3 = new List<ADScopeCollection>();
								dictionary.Add(keyValuePair.Key, list3);
							}
							if (list2.Count != 0)
							{
								List<ADScope> list4 = new List<ADScope>(keyValuePair.Value);
								list4.AddRange(list2);
								list3.Add(new ADScopeCollection(list4));
							}
							else
							{
								list3.Add(new ADScopeCollection(keyValuePair.Value));
							}
						}
					}
				}
			}
			return new ScopeSet(recipientReadScope, recipientWriteScopes, this.exclusiveRecipientScopes, rbacScope, configWriteScope, dictionary, this.exclusiveObjectSpecificConfigScopes);
		}

		private List<ADScopeCollection> AggregateScopeForLocation(ScopeLocation scopeLocation, OrganizationId organizationId, List<RoleAssignmentScopeSet>[] parameterScopes, ScopeType[] maximumScopesCommonForAllParams, ScopeType[,] maxParameterScopes, bool hiddenFromGAL)
		{
			ScopeType scopeType = maximumScopesCommonForAllParams[(int)scopeLocation];
			if (scopeType != ScopeType.None && scopeType != ScopeType.MyGAL)
			{
				RbacScope rbacScope = new RbacScope(scopeType);
				rbacScope.PopulateRootAndFilter(organizationId, this.executingUser);
				return new List<ADScopeCollection>(1)
				{
					new ADScopeCollection(new ADScope[]
					{
						rbacScope
					})
				};
			}
			List<ADScopeCollection> list = new List<ADScopeCollection>();
			for (int i = 0; i < parameterScopes.Length; i++)
			{
				List<RoleAssignmentScopeSet> list2 = parameterScopes[i];
				List<ADScope> list3 = null;
				for (int j = 0; j < list2.Count; j++)
				{
					RbacScope rbacScope2 = list2[j][scopeLocation];
					if (rbacScope2.Exclusive || !rbacScope2.IsScopeTypeSmallerThan(maxParameterScopes[i, (int)scopeLocation], hiddenFromGAL))
					{
						if (list3 == null)
						{
							list3 = new List<ADScope>();
						}
						if (!rbacScope2.IsPresentInCollection(list3))
						{
							rbacScope2.PopulateRootAndFilter(organizationId, this.executingUser);
							list3.Add(rbacScope2);
						}
					}
				}
				if (!ExchangeRunspaceConfiguration.SameScopeCollectionExists(list, list3))
				{
					list.Add(new ADScopeCollection(list3));
				}
			}
			if (list.Count > 1)
			{
				ExchangeRunspaceConfiguration.TrimAggregatedScopes(list, hiddenFromGAL);
			}
			return list;
		}

		private void AddClientSpecificRoleEntries(ref List<RoleEntryInfo> userAllRoleEntries)
		{
			ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication = this.ConfigurationSettings.ClientApplication;
			if (clientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.EMC)
			{
				return;
			}
			this.AddEMCSpecificRoleEntries(ref userAllRoleEntries);
		}

		private void AddEMCSpecificRoleEntries(ref List<RoleEntryInfo> userAllRoleEntries)
		{
			foreach (RoleEntryInfo roleEntryInfo in ClientRoleEntries.EMCRequiredRoleEntries)
			{
				int num = userAllRoleEntries.BinarySearch(roleEntryInfo, RoleEntryInfo.NameComparer);
				if (num < 0)
				{
					userAllRoleEntries.Insert(~num, roleEntryInfo);
					ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<RoleEntryInfo>((long)this.GetHashCode(), "ERC.AddEMCSpecificRoleEntries adding entry {0} to the current ExchangeRunspaceConfiguration.", roleEntryInfo);
				}
			}
		}

		private void AddClientSpecificParameters(ref List<RoleEntryInfo> userAllRoleEntries)
		{
			ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication = this.ConfigurationSettings.ClientApplication;
			if (clientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP && clientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.ReportingWebService && clientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP)
			{
				return;
			}
			this.AddSpecificParameters(ClientRoleEntries.ECPRequiredParameters, ref userAllRoleEntries);
			this.AddSpecificParameters(ClientRoleEntries.EngineeringFundamentalsReportingRequiredParameters, ref userAllRoleEntries);
			this.AddSpecificParameters(ClientRoleEntries.TenantReportingRequiredParameters, ref userAllRoleEntries);
		}

		private void AddSpecificParameters(RoleEntry[] entryParametersToAdd, ref List<RoleEntryInfo> userAllRoleEntries)
		{
			if (entryParametersToAdd == null || entryParametersToAdd.Length == 0)
			{
				return;
			}
			int num = 0;
			int i = 0;
			RoleEntry roleEntry = entryParametersToAdd[num];
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			while (i < userAllRoleEntries.Count)
			{
				RoleEntry roleEntry2 = userAllRoleEntries[i].RoleEntry;
				if (RoleEntry.CompareRoleEntriesByName(roleEntry2, roleEntry) == 0)
				{
					if (num2 == 0)
					{
						num2 = i;
					}
					num3++;
					if (!roleEntry2.ContainsAllParameters(new List<string>(roleEntry.Parameters)))
					{
						flag = true;
						ExTraceGlobals.PublicInstanceAPITracer.TraceDebug<RoleEntry, RoleEntry>((long)this.GetHashCode(), "ERC.AddECPSpecificParameters adding parameters entry {0} to the existing entry {1}.", roleEntry2, roleEntry);
						userAllRoleEntries[i] = new RoleEntryInfo(RoleEntry.MergeParameters(new RoleEntry[]
						{
							roleEntry2,
							roleEntry
						}), userAllRoleEntries[i].RoleAssignment);
					}
					i++;
				}
				else if (RoleEntry.CompareRoleEntriesByName(roleEntry2, roleEntry) > 0)
				{
					if (flag)
					{
						userAllRoleEntries.Sort(num2, num3, RoleEntryInfo.NameAndInstanceHashCodeComparer);
						flag = false;
					}
					num2 = 0;
					num3 = 0;
					if (num + 1 >= entryParametersToAdd.Length)
					{
						return;
					}
					roleEntry = entryParametersToAdd[++num];
				}
				else
				{
					i++;
				}
			}
		}

		private bool ContainsOnlySelfScopes(IList<ADScopeCollection> recipientWriteScopes)
		{
			if (recipientWriteScopes.Count == 0)
			{
				return false;
			}
			foreach (ADScopeCollection adscopeCollection in recipientWriteScopes)
			{
				foreach (ADScope adscope in adscopeCollection)
				{
					RbacScope rbacScope = adscope as RbacScope;
					if (rbacScope == null || rbacScope.ScopeType != ScopeType.Self)
					{
						return false;
					}
				}
			}
			return true;
		}

		private RoleEntry TrimRoleEntryParametersWithValidationRules(RoleEntry roleEntry, bool onlyOrgLevelValidationRules = false)
		{
			return this.TrimRoleEntryParametersWithValidationRules(roleEntry, this.executingUser, onlyOrgLevelValidationRules);
		}

		private RoleEntry TrimRoleEntryParametersWithValidationRules(RoleEntry roleEntry, ADRawEntry targetRulesUser, bool onlyOrgLevelValidationRules = false)
		{
			ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "Entering ERC.TrimRoleEntryParametersWithValidationRules('{0}', '{1}').", roleEntry, targetRulesUser.GetDistinguishedNameOrName());
			IList<ValidationRule> list = null;
			if (this.ApplyValidationRules)
			{
				list = ValidationRuleFactory.GetApplicableValidationRules((roleEntry is CmdletRoleEntry) ? ((CmdletRoleEntry)roleEntry).FullName : roleEntry.Name, (IList<string>)roleEntry.Parameters, this.executingUser);
				if (onlyOrgLevelValidationRules)
				{
					List<ValidationRule> list2 = new List<ValidationRule>();
					foreach (ValidationRule validationRule in list)
					{
						if (validationRule is OrganizationValidationRule)
						{
							list2.Add(validationRule);
						}
					}
					list = list2;
				}
			}
			if (list != null && list.Count > 0)
			{
				List<string> list3 = new List<string>(roleEntry.Parameters);
				this.TrimParameterListWithValidationRules(targetRulesUser, list3, list);
				if (list3.Count != roleEntry.Parameters.Count)
				{
					return roleEntry.Clone(list3);
				}
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "ERC.TrimRoleEntryParametersWithValidationRules('{0}', '{1}'). Returns SAME ROLE ENTRY", roleEntry, targetRulesUser.GetDistinguishedNameOrName());
			return roleEntry;
		}

		private void TrimParameterListWithValidationRules(ADRawEntry objectToCheck, IList<string> parameters, IList<ValidationRule> validationRules)
		{
			bool flag = false;
			using (IEnumerator<ValidationRule> enumerator = validationRules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValidationRule rule = enumerator.Current;
					if (objectToCheck.Id == null || this.executingUser.Id == null || !objectToCheck.Id.Equals(this.executingUser.Id))
					{
						goto IL_EB;
					}
					CachedValidationRuleResult cachedValidationRuleResult = null;
					try
					{
						this.validationRuleCacheResultsLock.EnterReadLock();
						cachedValidationRuleResult = this.validationRuleCacheResults.FirstOrDefault((CachedValidationRuleResult x) => x.RuleName.Equals(rule.Name, StringComparison.OrdinalIgnoreCase));
					}
					finally
					{
						this.validationRuleCacheResultsLock.ExitReadLock();
					}
					RuleValidationException ex;
					if (cachedValidationRuleResult == null)
					{
						flag = rule.TryValidate(objectToCheck, out ex);
						try
						{
							this.validationRuleCacheResultsLock.EnterWriteLock();
							this.validationRuleCacheResults.Add(new CachedValidationRuleResult(rule.Name, flag));
							goto IL_FB;
						}
						finally
						{
							this.validationRuleCacheResultsLock.ExitWriteLock();
						}
						goto IL_EB;
					}
					flag = cachedValidationRuleResult.EvaluationResult;
					IL_FB:
					if (!flag)
					{
						foreach (string text in rule.Parameters)
						{
							if (parameters.Count == 0)
							{
								return;
							}
							ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ERC.TrimParameterListWithValidationRules('{0}'). Removing Parameter '{1}' per Validation Rule '{2}' failure.", objectToCheck.GetDistinguishedNameOrName(), text, rule.Name);
							parameters.Remove(text);
						}
						continue;
					}
					continue;
					IL_EB:
					flag = rule.TryValidate(objectToCheck, out ex);
					goto IL_FB;
				}
			}
		}

		private ADSessionSettings GetADSessionSettings()
		{
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Ent Sku, returning ADSessionSettings.FromRootOrgScopeSet().");
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. OrganizationId {0} , DelegatedPrincipal {1} , HasLinkedRoleGroups {2}, \r\n                    LogonUser {3} OrgId {4}, \r\n                    Executing User {5} OrgId {6}", new object[]
			{
				(null == this.OrganizationId) ? "<null>" : this.OrganizationId.ToString(),
				(this.DelegatedPrincipal == null) ? "<null>" : this.DelegatedPrincipal.ToString(),
				this.HasLinkedRoleGroups,
				(this.executingUser == null) ? "<null>" : this.logonUser.ToString(),
				(this.executingUser != null && this.logonUser[ADObjectSchema.OrganizationId] != null) ? this.logonUser[ADObjectSchema.OrganizationId] : "<null>",
				(this.executingUser == null) ? "<null>" : this.executingUser.ToString(),
				(this.executingUser != null && this.executingUser[ADObjectSchema.OrganizationId] != null) ? this.executingUser[ADObjectSchema.OrganizationId] : "<null>"
			});
			if (null != this.OrganizationId)
			{
				if (this.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (First Org User), returning ADSessionSettings.FromRootOrgScopeSet().");
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				if (this.PartnerMode)
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (Partner Admin - First Org User), returning ADSessionSettings.FromRootOrgScopeSet().");
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (Tenant User), returning ADSessionSettings.FromAllTenantsPartitionId().");
				return ADSessionSettings.FromAllTenantsPartitionId(this.OrganizationId.PartitionId);
			}
			else
			{
				if (this.DelegatedPrincipal != null)
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (Delegated Principal User), returning ADSessionSettings.FromAllTenantsPartitionId().");
					return ADSessionSettings.FromAllTenantsPartitionId(ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(this.DelegatedPrincipal.DelegatedOrganization));
				}
				if (this.HasLinkedRoleGroups)
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (Linked RoleGroups user), returning ADSessionSettings.FromRootOrgScopeSet().");
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				if (this.logonUser != null && this.logonUser[ADObjectSchema.OrganizationId] != null)
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (logon user org).");
					OrganizationId organizationId = (OrganizationId)this.logonUser[ADObjectSchema.OrganizationId];
					if (!OrganizationId.ForestWideOrgId.Equals(organizationId))
					{
						return ADSessionSettings.FromAllTenantsPartitionId(organizationId.PartitionId);
					}
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				else
				{
					if (this.executingUser == null || this.executingUser[ADObjectSchema.OrganizationId] == null)
					{
						return ADSessionSettings.FromRootOrgScopeSet();
					}
					ExTraceGlobals.RunspaceConfigTracer.TraceDebug((long)this.GetHashCode(), "ERC.GetADSessionSettings. Multitenant Sku (executing user org Id {0}).");
					OrganizationId organizationId2 = (OrganizationId)this.logonUser[ADObjectSchema.OrganizationId];
					if (!OrganizationId.ForestWideOrgId.Equals(organizationId2))
					{
						return ADSessionSettings.FromAllTenantsPartitionId(organizationId2.PartitionId);
					}
					return ADSessionSettings.FromRootOrgScopeSet();
				}
			}
		}

		protected void RefreshCombinedCmdletsAndScripts()
		{
			using (new MonitoredScope("ExchangeRunspaceConfiguration", "RefreshCombinedCmdletsAndScripts", AuthZLogHelper.AuthZPerfMonitors))
			{
				this.combinedCmdlets = null;
				this.combinedScripts = null;
				this.CombineRoleEntries(this.allRoleEntries, out this.combinedCmdlets, out this.combinedScripts);
				bool flag = this.intersectRoleEntries;
				this.combinedCmdlets.Add(ExchangeRunspaceConfiguration.getHelpRoleEntry);
				this.combinedCmdlets.Add(ExchangeRunspaceConfiguration.getCommandRoleEntry);
				if (this.IsPowerShellWebService)
				{
					this.combinedCmdlets.Add(ExchangeRunspaceConfiguration.convertToExchangeXmlEntry);
				}
				if (this.settings.IsProxy)
				{
					for (int i = 0; i < this.combinedCmdlets.Count; i++)
					{
						CmdletRoleEntry cmdletRoleEntry = this.combinedCmdlets[i] as CmdletRoleEntry;
						if (cmdletRoleEntry != null)
						{
							IList<RoleEntry> list = new List<RoleEntry>(2);
							list.Add(cmdletRoleEntry);
							list.Add(new CmdletRoleEntry(cmdletRoleEntry.Name, cmdletRoleEntry.PSSnapinName, ClientRoleEntries.ParametersForProxy));
							this.combinedCmdlets[i] = RoleEntry.MergeParameters(list);
						}
					}
				}
			}
		}

		protected void RefreshVerboseDebugEnabledCmdlets(IEnumerable<RoleEntry> cmdlets)
		{
			this.verboseEnabledCmdlets.Clear();
			this.debugEnabledCmdlets.Clear();
			foreach (RoleEntry roleEntry in cmdlets)
			{
				if (roleEntry.Parameters.Contains("Verbose", StringComparer.OrdinalIgnoreCase))
				{
					this.verboseEnabledCmdlets.Add(roleEntry.Name);
				}
				if (roleEntry.Parameters.Contains("Debug", StringComparer.OrdinalIgnoreCase))
				{
					this.debugEnabledCmdlets.Add(roleEntry.Name);
				}
			}
		}

		private void FaultInjection_ByPassRBACTestSnapinCheck(ref string cmdletFullName, string exchangeCmdletName)
		{
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(4257623357U, ref empty);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3385208125U, ref empty2);
				if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty2) && empty2.IndexOf(exchangeCmdletName, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					cmdletFullName = empty + "\\" + exchangeCmdletName;
				}
			}
		}

		internal string GetRBACInformationSummary()
		{
			if (string.IsNullOrEmpty(this.rbacInformationSummary))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("ServicePlan:{0};", this.ServicePlanForLogging);
				stringBuilder.AppendFormat("IsAdmin:{0};", this.HasAdminRoles);
				this.rbacInformationSummary = stringBuilder.ToString();
			}
			return this.rbacInformationSummary;
		}

		private string GetUserFriendlyRoleAssignmentInfo(Result<ExchangeRoleAssignment>[] roleAssignmentResults)
		{
			Dictionary<RoleAssigneeType, HashSet<string>> dictionary = new Dictionary<RoleAssigneeType, HashSet<string>>(16);
			foreach (Result<ExchangeRoleAssignment> result in roleAssignmentResults)
			{
				ExchangeRoleAssignment data = result.Data;
				if (!dictionary.ContainsKey(data.RoleAssigneeType))
				{
					dictionary.Add(data.RoleAssigneeType, new HashSet<string>());
				}
				if (data.RoleAssigneeType == RoleAssigneeType.User)
				{
					dictionary[data.RoleAssigneeType].Add(string.Format("{0}[CRW:{1} CCW:{2} RRS:{3} RWS{4} CRS:{5} CWS:{6}]", new object[]
					{
						data.Role.Name,
						data.CustomRecipientWriteScope,
						data.CustomConfigWriteScope,
						data.RecipientReadScope,
						data.RecipientWriteScope,
						data.ConfigReadScope,
						data.ConfigWriteScope
					}));
				}
				else
				{
					dictionary[data.RoleAssigneeType].Add(data.User.Name);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (RoleAssigneeType roleAssigneeType in dictionary.Keys)
			{
				if (roleAssigneeType.Equals(RoleAssigneeType.User))
				{
					stringBuilder.AppendFormat("Roles{{", new object[0]);
				}
				else
				{
					stringBuilder.AppendFormat("{0}{{", roleAssigneeType.ToString());
				}
				foreach (string arg in dictionary[roleAssigneeType])
				{
					stringBuilder.AppendFormat("{0}:", arg);
				}
				stringBuilder.AppendFormat("}}", new object[0]);
			}
			return stringBuilder.ToString();
		}

		internal const string ExchangeSetupPSSnapinName = "Microsoft.Exchange.Management.PowerShell.Setup";

		internal const string ExchangeAdminPSSnapinName = "Microsoft.Exchange.Management.PowerShell.E2010";

		internal const string ExchangeCentralAdminPSSnapinName = "Microsoft.Exchange.Management.PowerShell.CentralAdmin";

		internal const string ExchangeFfoCentralAdminPSSnapinName = "Microsoft.Exchange.Management.PowerShell.FfoCentralAdmin";

		internal const string ExchangeFfoAntiSpamPSSnapinName = "Microsoft.Forefront.AntiSpam.PowerShell";

		internal const string ExchangeFfoManagementPSSnapinName = "Microsoft.Forefront.Management.PowerShell";

		internal const string ExchangeSupportPSSnapinName = "Microsoft.Exchange.Management.PowerShell.Support";

		internal const string ExchangeOspPSSnapinName = "Microsoft.Exchange.Monitoring.ServiceHealth";

		internal const string ExchangeOspRptPSSnapinName = "Microsoft.Exchange.Monitoring.Reporting";

		internal const string ExchangeDataCenterCommonPSSnapinName = "Microsoft.Datacenter.Common";

		internal const string ExchangeMonitoringAzureManagementPSSnapinName = "Microsoft.Exchange.Datacenter.Management.ActiveMonitoring";

		internal const string ExchangeNewCapacityPSSnapinName = "Microsoft.Exchange.ServiceManagement.NewCapacityPSSnapIn";

		internal const string DataMiningConfigurationPSSnapinName = "Microsoft.Datacenter.DataMining.Configuration.Powershell";

		internal const string ReportExchangePSSnapinName = "Microsoft.Datacenter.Reporting.Exchange.Powershell";

		internal const string ReportLyncPSSnapinName = "Microsoft.Datacenter.Reporting.Lync.Powershell";

		internal const string GenericExchangeSnapinName = "Exchange";

		internal const string SupportLyncPSSnapinName = "Microsoft.Datacenter.Support.Lync.Powershell";

		internal const string TorusPSSnapinName = "Microsoft.Office.Datacenter.Torus.Cmdlets";

		internal const string ImpersonationMethod = "Impersonate-ExchangeUser";

		internal const string DedicatedTenantAdminRoleNamePrefix = "SSA_";

		private const int estimatedRoleEntryCount = 100;

		private const int estimatedParameterCount = 10;

		private const int estimatedScriptCount = 5;

		internal static readonly System.Collections.ObjectModel.ReadOnlyCollection<string> ExchangeSnapins = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new string[]
		{
			"Microsoft.Exchange.Management.PowerShell.E2010",
			"Microsoft.Exchange.Management.PowerShell.Setup",
			"Microsoft.Exchange.Management.PowerShell.CentralAdmin",
			"Microsoft.Exchange.Management.PowerShell.FfoCentralAdmin",
			"Microsoft.Forefront.AntiSpam.PowerShell",
			"Microsoft.Forefront.Management.PowerShell",
			"Exchange",
			"Microsoft.Exchange.Management.PowerShell.Support",
			"Microsoft.Exchange.Monitoring.ServiceHealth",
			"Microsoft.Exchange.Monitoring.Reporting",
			"Microsoft.Datacenter.Common",
			"Microsoft.Exchange.Datacenter.Management.ActiveMonitoring",
			"Microsoft.Exchange.ServiceManagement.NewCapacityPSSnapIn",
			"Microsoft.Datacenter.DataMining.Configuration.Powershell",
			"Microsoft.Datacenter.Reporting.Exchange.Powershell",
			"Microsoft.Datacenter.Reporting.Lync.Powershell",
			"Microsoft.Office.Datacenter.Torus.Cmdlets"
		});

		internal static readonly System.Collections.ObjectModel.ReadOnlyCollection<string> AdminSnapins = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new string[]
		{
			"Microsoft.Exchange.Management.PowerShell.E2010"
		});

		internal static readonly System.Collections.ObjectModel.ReadOnlyCollection<string> OspSnapins = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new string[]
		{
			"Microsoft.Exchange.Management.PowerShell.E2010",
			"Microsoft.Exchange.Management.PowerShell.CentralAdmin",
			"Microsoft.Exchange.Monitoring.ServiceHealth",
			"Microsoft.Exchange.Monitoring.Reporting",
			"Microsoft.Datacenter.Common",
			"Microsoft.Exchange.Datacenter.Management.ActiveMonitoring",
			"Microsoft.Datacenter.DataMining.Configuration.Powershell",
			"Microsoft.Datacenter.Reporting.Exchange.Powershell",
			"Microsoft.Datacenter.Reporting.Lync.Powershell",
			"Microsoft.Datacenter.Support.Lync.Powershell"
		});

		private static RoleEntryInfo welcomeMessageScriptInitializer = new RoleEntryInfo(new ScriptRoleEntry("ExchangeFismaBannerMessage.ps1", null));

		private static ADPropertyDefinition[] ercUserPropertyArray = new ADPropertyDefinition[]
		{
			ADObjectSchema.RawName,
			ADObjectSchema.Name,
			ADObjectSchema.Id,
			ADRecipientSchema.WindowsLiveID,
			ADObjectSchema.ExchangeVersion,
			IADSecurityPrincipalSchema.Sid,
			ADRecipientSchema.MasterAccountSid,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.AddressListMembership,
			ADRecipientSchema.RemotePowerShellEnabled,
			ADRecipientSchema.ECPEnabled,
			ADUserSchema.OwaMailboxPolicy,
			ADUserSchema.RetentionPolicy,
			ADRecipientSchema.OWAEnabled,
			ADRecipientSchema.EmailAddresses,
			ADUserSchema.ActiveSyncEnabled,
			ADMailboxRecipientSchema.ExternalOofOptions,
			ADRecipientSchema.MAPIEnabled,
			ADUserSchema.UMEnabled,
			ADRecipientSchema.MailboxPlan,
			ADRecipientSchema.RoleAssignmentPolicy,
			ADObjectSchema.ObjectClass,
			ADOrgPersonSchema.Languages,
			ADRecipientSchema.ThrottlingPolicy,
			ADRecipientSchema.HiddenFromAddressListsEnabled,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.IsPersonToPersonTextMessagingEnabled,
			ADRecipientSchema.IsMachineToPersonTextMessagingEnabled,
			ADUserSchema.PersistedCapabilities,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.UsageLocation,
			ADRecipientSchema.RecipientTypeDetails,
			ADUserSchema.UserPrincipalName,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.ExternalDirectoryObjectId,
			SharedPropertyDefinitions.PersistedCapabilities
		};

		private static IEnumerable<ADPropertyDefinition> userFilterSchemaProperties = ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>().AllProperties.OfType<ADPropertyDefinition>();

		internal static ADPropertyDefinition[] userPropertyArray = ExchangeRunspaceConfiguration.ercUserPropertyArray.Union(ExchangeRunspaceConfiguration.userFilterSchemaProperties).ToArray<ADPropertyDefinition>();

		internal static PropertyDefinition[] delegatedGroupInfo = new PropertyDefinition[]
		{
			ADObjectSchema.RawName,
			ADObjectSchema.Name,
			ADGroupSchema.LinkedPartnerGroupAndOrganizationId,
			ADObjectSchema.Id,
			ADObjectSchema.ExchangeVersion
		};

		private static readonly string[] noParameterArray = new string[]
		{
			string.Empty
		};

		private static readonly string[] emptyArray = new string[0];

		private static readonly CmdletRoleEntry getHelpRoleEntry = new CmdletRoleEntry("Get-Help", "Microsoft.PowerShell.Core", new string[]
		{
			"Name",
			"Path",
			"Category",
			"Component",
			"Functionality",
			"Role",
			"Detailed",
			"Full",
			"Examples",
			"Parameter",
			"Online",
			"Verbose",
			"Debug",
			"ErrorAction",
			"WarningAction",
			"ErrorVariable",
			"WarningVariable",
			"OutVariable",
			"OutBuffer"
		});

		private static readonly CmdletRoleEntry getCommandRoleEntry = new CmdletRoleEntry("Get-Command", "Microsoft.PowerShell.Core", new string[]
		{
			"Name",
			"Verb",
			"Noun",
			"Module",
			"CommandType",
			"TotalCount",
			"Syntax",
			"ArgumentList",
			"Verbose",
			"Debug",
			"ErrorAction",
			"WarningAction",
			"ErrorVariable",
			"WarningVariable",
			"OutVariable",
			"OutBuffer"
		});

		private static readonly CmdletRoleEntry convertToExchangeXmlEntry = new CmdletRoleEntry("ConvertTo-ExchangeXml", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
		{
			"InputObject"
		});

		private static bool? isPowershellProcess;

		private static ConcurrentDictionary<string, ADObjectId> rootOrgUSGContainers = new ConcurrentDictionary<string, ADObjectId>(StringComparer.OrdinalIgnoreCase);

		protected string identityName;

		private readonly string authenticationType;

		protected DelegatedPrincipal delegatedPrincipal;

		protected ADRawEntry executingUser;

		private ADRawEntry logonUser;

		private OrganizationId organizationId;

		protected IRecipientSession recipientSession;

		private System.Collections.ObjectModel.ReadOnlyCollection<string> snapinCollection;

		protected ExchangeRunspaceConfigurationSettings settings;

		private ICollection<SecurityIdentifier> tokenSids;

		protected List<RoleEntryInfo> allRoleEntries;

		protected Dictionary<RoleType, List<ADObjectId>> allRoleTypes;

		protected Dictionary<ADObjectId, ManagementScope> allScopes;

		protected List<RoleEntry> combinedCmdlets;

		private List<RoleEntry> combinedScripts;

		private IDisposable logonIdentityToDispose;

		private IDisposable impersonatedIdentityToDispose;

		protected Microsoft.Exchange.Collections.ReadOnlyCollection<ExchangeRoleAssignment> allRoleAssignments;

		private bool intersectRoleEntries;

		private List<ADScope> exclusiveRecipientScopes;

		private Dictionary<string, ADScopeCollection> exclusiveObjectSpecificConfigScopes;

		protected ADScopeCollection exclusiveRecipientScopesCollection;

		private SerializedAccessToken logonAccessToken;

		private TroubleshootingContext troubleshootingContext = new TroubleshootingContext(ExWatson.AppName + ".Powershell");

		private SerializedAccessToken impersonatedUserAccessToken;

		private ProvisioningBroker provisioningBroker;

		private object provisioningBrokerSyncRoot = new object();

		private bool? umConfigured;

		private OwaSegmentationSettings owaSegmentationSettings;

		private MultiValuedProperty<CultureInfo> executingUserLanguages;

		private ADRecipientOrAddress executingUserAsRecipient;

		private ReaderWriterLockSlim validationRuleCacheResultsLock = new ReaderWriterLockSlim();

		private ICollection<CachedValidationRuleResult> validationRuleCacheResults;

		private ADObjectId[] universalSecurityGroupsCache;

		private bool ensureTargetIsMemberOfRIMMailboxUsersGroup;

		private object loadRoleCmdletInfoSyncRoot = new object();

		private HashSet<string> verboseEnabledCmdlets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> debugEnabledCmdlets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private bool enablePiiMap;

		private string piiMapId;

		private bool isDedicatedTenantAdmin;

		private string rbacInformationSummary;

		private string servicePlanForLogging;

		private IList<RoleType> roleTypeFilter;

		private List<RoleEntry> sortedRoleEntryFilter;

		private IList<RoleType> logonUserRequiredRoleTypes;

		private bool callerCheckedAccess;
	}
}
