using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.JobQueues;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "SiteMailbox", DefaultParameterSetName = "TeamMailboxIW", SupportsShouldProcess = true)]
	public sealed class NewSiteMailbox : NewMailboxOrSyncMailbox
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0)]
		[ValidateNotNullOrEmpty]
		public new string DisplayName
		{
			get
			{
				return base.DisplayName;
			}
			set
			{
				base.DisplayName = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[ValidateNotNullOrEmpty]
		public new OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
			set
			{
				base.OrganizationalUnit = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[ValidateNotNullOrEmpty]
		public new DatabaseIdParameter Database
		{
			get
			{
				return base.Database;
			}
			set
			{
				base.Database = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public new string Alias
		{
			get
			{
				return base.Alias;
			}
			set
			{
				base.Alias = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)base.Fields["SharePointUrl"];
			}
			set
			{
				if (value != null && (!value.IsAbsoluteUri || (!(value.Scheme == Uri.UriSchemeHttps) && !(value.Scheme == Uri.UriSchemeHttp))))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxSharePointUrl), ExchangeErrorCategory.Client, null);
				}
				base.Fields["SharePointUrl"] = value;
			}
		}

		private new SwitchParameter AccountDisabled { get; set; }

		private new MailboxPolicyIdParameter ActiveSyncMailboxPolicy { get; set; }

		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy { get; set; }

		private new SwitchParameter Arbitration { get; set; }

		private new MailboxIdParameter ArbitrationMailbox { get; set; }

		private new SwitchParameter Archive { get; set; }

		private new DatabaseIdParameter ArchiveDatabase { get; set; }

		private new SmtpDomain ArchiveDomain { get; set; }

		private new SwitchParameter AuditLog { get; set; }

		private new SwitchParameter AuxMailbox { get; set; }

		private new SwitchParameter BypassLiveId { get; set; }

		private new SwitchParameter Discovery { get; set; }

		private new SwitchParameter Equipment { get; set; }

		private new SwitchParameter EvictLiveId { get; set; }

		private new string ExternalDirectoryObjectId { get; set; }

		private new string FederatedIdentity { get; set; }

		private new string FirstName { get; set; }

		private new SwitchParameter ForestWideDomainControllerAffinityByExecutingUser { get; set; }

		private new string ImmutableId { get; set; }

		private new SwitchParameter ImportLiveId { get; set; }

		private new string Initials { get; set; }

		private new MultiValuedProperty<CultureInfo> Languages { get; set; }

		private new string LastName { get; set; }

		private new PSCredential LinkedCredential { get; set; }

		private new string LinkedDomainController { get; set; }

		private new UserIdParameter LinkedMasterAccount { get; set; }

		private new SwitchParameter LinkedRoom { get; set; }

		private new MailboxPlanIdParameter MailboxPlan { get; set; }

		private new Guid MailboxContainerGuid { get; set; }

		private new WindowsLiveId MicrosoftOnlineServicesID { get; set; }

		private new MultiValuedProperty<ModeratorIDParameter> ModeratedBy { get; set; }

		private new bool ModerationEnabled { get; set; }

		private new NetID NetID { get; set; }

		private new SecureString Password { get; set; }

		private new SmtpAddress PrimarySmtpAddress { get; set; }

		private new SwitchParameter PublicFolder { get; set; }

		private new SwitchParameter HoldForMigration { get; set; }

		private new bool QueryBaseDNRestrictionEnabled { get; set; }

		private new RemoteAccountPolicyIdParameter RemoteAccountPolicy { get; set; }

		private new SwitchParameter RemoteArchive { get; set; }

		private new bool RemotePowerShellEnabled { get; set; }

		private new RemovedMailboxIdParameter RemovedMailbox { get; set; }

		private new bool ResetPasswordOnNextLogon { get; set; }

		private new MailboxPolicyIdParameter RetentionPolicy { get; set; }

		private new MailboxPolicyIdParameter RoleAssignmentPolicy { get; set; }

		private new SwitchParameter Room { get; set; }

		private new string SamAccountName { get; set; }

		private new TransportModerationNotificationFlags SendModerationNotifications { get; set; }

		private new SwitchParameter Shared { get; set; }

		private new SharingPolicyIdParameter SharingPolicy { get; set; }

		private new bool SKUAssigned { get; set; }

		private new MultiValuedProperty<Capability> AddOnSKUCapability { get; set; }

		private new Capability SKUCapability { get; set; }

		private new SwitchParameter TargetAllMDBs { get; set; }

		private new ThrottlingPolicyIdParameter ThrottlingPolicy { get; set; }

		private new CountryInfo UsageLocation { get; set; }

		private new SwitchParameter UseExistingLiveId { get; set; }

		private new string UserPrincipalName { get; set; }

		private new WindowsLiveId WindowsLiveID { get; set; }

		private new SecureString RoomMailboxPassword { get; set; }

		private new bool EnableRoomMailboxAccount { get; set; }

		private new bool IsExcludedFromServingHierarchy { get; set; }

		private new MailboxProvisioningConstraint MailboxProvisioningConstraint { get; set; }

		private new MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewTeamMailbox(this.Name.ToString(), base.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SiteMailboxProvisioningInExecutingUserOUEnabled.Enabled && this.OrganizationalUnit == null && base.CurrentTaskContext.UserInfo != null)
			{
				string identity = ADRecipient.OrganizationUnitFromADObjectId(base.CurrentTaskContext.UserInfo.ExecutingUserId);
				this.OrganizationalUnit = new OrganizationalUnitIdParameter(identity);
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalStateReset()
		{
			if (string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.DisplayName))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNewTeamMailboxParameter), ExchangeErrorCategory.Client, null);
			}
			IList<TeamMailboxProvisioningPolicy> teamMailboxPolicies = this.GetTeamMailboxPolicies();
			if (teamMailboxPolicies == null || teamMailboxPolicies.Count == 0)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxCanNotLoadPolicy), ExchangeErrorCategory.Client, null);
			}
			this.provisioningPolicy = teamMailboxPolicies[0];
			if (string.IsNullOrEmpty(this.Name))
			{
				string prefix = null;
				if (!string.IsNullOrEmpty(this.provisioningPolicy.AliasPrefix))
				{
					prefix = this.provisioningPolicy.AliasPrefix;
				}
				else if (this.provisioningPolicy.DefaultAliasPrefixEnabled)
				{
					prefix = (Datacenter.IsMicrosoftHostedOnly(true) ? "SMO-" : "SM-");
				}
				this.Name = TeamMailboxHelper.GenerateUniqueAliasForSiteMailbox(base.TenantGlobalCatalogSession, base.CurrentOrganizationId, this.DisplayName, prefix, Datacenter.IsMicrosoftHostedOnly(true), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				SiteMailboxAddressesTemplate siteMailboxAddressesTemplate = null;
				try
				{
					siteMailboxAddressesTemplate = SiteMailboxAddressesTemplate.GetSiteMailboxAddressesTemplate(this.ConfigurationSession, base.ProvisioningCache);
				}
				catch (ErrorSiteMailboxCannotLoadAddressTemplateException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.Client, null);
				}
				base.UserPrincipalName = RecipientTaskHelper.GenerateUniqueUserPrincipalName(base.TenantGlobalCatalogSession, this.Name, siteMailboxAddressesTemplate.UserPrincipalNameDomain, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				base.UserSpecifiedParameters["SpecificAddressTemplates"] = siteMailboxAddressesTemplate.AddressTemplates;
			}
			base.InternalStateReset();
		}

		protected override void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareUserObject(user);
			user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.TeamMailbox, false));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!base.TryGetExecutingUserId(out this.executingUserId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxCannotIdentifyTheUser), ExchangeErrorCategory.Client, null);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SiteMailboxCheckSharePointUrlAgainstTrustedHosts.Enabled && base.Fields.IsModified("SharePointUrl"))
			{
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					this.CheckSharePointUrlSafetyInDatacenter(this.SharePointUrl);
					return;
				}
				if (!base.Fields.IsModified("Force"))
				{
					this.CheckSharePointUrlSafetyOnPremises(this.SharePointUrl);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			ADUser dataObject = this.DataObject;
			TeamMailbox teamMailbox = TeamMailbox.FromDataObject(dataObject);
			this.teamMailboxHelper = new TeamMailboxHelper(teamMailbox, base.ExchangeRunspaceConfig.ExecutingUser, base.ExchangeRunspaceConfig.ExecutingUserOrganizationId, (IRecipientSession)base.DataSession, new TeamMailboxGetDataObject<ADUser>(base.GetDataObject<ADUser>));
			TeamMailboxMembershipHelper teamMailboxMembershipHelper = new TeamMailboxMembershipHelper(teamMailbox, (IRecipientSession)base.DataSession);
			Exception ex = null;
			ADUser aduser = TeamMailboxADUserResolver.Resolve((IRecipientSession)base.DataSession, this.executingUserId, out ex);
			if (aduser == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorExecutingUserIsNull), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified("DisplayName"))
			{
				teamMailbox.DisplayName = this.DisplayName;
			}
			teamMailbox.SetPolicy(this.provisioningPolicy);
			base.WriteVerbose(Strings.SiteMailboxPolicySet(this.provisioningPolicy.ToString()));
			teamMailbox.SetMyRole(this.executingUserId);
			Uri sharePointUrl = this.SharePointUrl;
			SharePointMemberShip sharePointMemberShip = SharePointMemberShip.Others;
			Uri webCollectionUrl = null;
			Guid empty = Guid.Empty;
			if (base.Fields.IsModified("SharePointUrl") && !base.Fields.IsModified("Force"))
			{
				try
				{
					TeamMailboxHelper.CheckSharePointSite(SmtpAddress.Empty, ref sharePointUrl, base.ExchangeRunspaceConfig.ExecutingUser, base.ExchangeRunspaceConfig.ExecutingUserOrganizationId, aduser, out sharePointMemberShip, out webCollectionUrl, out empty);
				}
				catch (RecipientTaskException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.Client, null);
				}
			}
			teamMailbox.SharePointUrl = sharePointUrl;
			teamMailbox.SetSharePointSiteInfo(webCollectionUrl, empty);
			teamMailbox.SharePointLinkedBy = this.executingUserId;
			List<ADObjectId> list = new List<ADObjectId>();
			List<ADObjectId> list2 = new List<ADObjectId>();
			IList<ADObjectId> list3 = null;
			IList<ADObjectId> usersToRemove = null;
			if (TeamMailboxMembershipHelper.IsUserQualifiedType(aduser))
			{
				if (sharePointMemberShip == SharePointMemberShip.Owner)
				{
					list.Add(this.executingUserId);
					teamMailboxMembershipHelper.UpdateTeamMailboxUserList(teamMailbox.Owners, list, out list3, out usersToRemove);
					teamMailboxMembershipHelper.UpdateTeamMailboxUserList(teamMailbox.OwnersAndMembers, list, out list3, out usersToRemove);
				}
				else if (sharePointMemberShip == SharePointMemberShip.Member)
				{
					list2.Add(this.executingUserId);
					teamMailboxMembershipHelper.UpdateTeamMailboxUserList(teamMailbox.OwnersAndMembers, list2, out list3, out usersToRemove);
				}
			}
			Exception ex2 = null;
			try
			{
				teamMailboxMembershipHelper.SetTeamMailboxUserPermissions(list3, usersToRemove, new SecurityIdentifier[]
				{
					WellKnownSids.SiteMailboxGrantedAccessMembers
				}, false);
				if (list3 != null)
				{
					base.WriteVerbose(Strings.SiteMailboxCreatorSet(list3[0].ToString()));
				}
			}
			catch (OverflowException ex3)
			{
				ex2 = ex3;
			}
			catch (COMException ex4)
			{
				ex2 = ex4;
			}
			catch (UnauthorizedAccessException ex5)
			{
				ex2 = ex5;
			}
			catch (TransientException ex6)
			{
				ex2 = ex6;
			}
			catch (DataSourceOperationException ex7)
			{
				ex2 = ex7;
			}
			if (ex2 != null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorSetTeamMailboxUserPermissions(teamMailbox.DisplayName, ex2.Message)), ExchangeErrorCategory.Client, null);
			}
			base.InternalProcessRecord();
			if (base.Fields.IsModified("SharePointUrl") && !base.Fields.IsModified("Force"))
			{
				try
				{
					this.teamMailboxHelper.LinkSharePointSite(sharePointUrl, true, false);
					base.WriteVerbose(Strings.SiteMailboxLinkedToSharePointSite(sharePointUrl.AbsoluteUri));
				}
				catch (RecipientTaskException exception2)
				{
					base.DataSession.Delete(this.DataObject);
					base.WriteError(exception2, ExchangeErrorCategory.Client, null);
				}
			}
			IList<Exception> list4;
			teamMailboxMembershipHelper.SetShowInMyClient(list3, usersToRemove, out list4);
			foreach (Exception ex8 in list4)
			{
				this.WriteWarning(Strings.ErrorTeamMailboxResolveUser(ex8.Message));
			}
			EnqueueResult enqueueResult = EnqueueResult.Success;
			if (this.databaseLocationInfo != null)
			{
				int num = 0;
				for (;;)
				{
					base.WriteVerbose(new LocalizedString(string.Format("Trying to send membership sync request to server {0} for MailboxGuid {1} using domain controller {2}", this.databaseLocationInfo.ServerFqdn, dataObject.ExchangeGuid, this.lastUsedDc)));
					enqueueResult = RpcClientWrapper.EnqueueTeamMailboxSyncRequest(this.databaseLocationInfo.ServerFqdn, dataObject.ExchangeGuid, QueueType.TeamMailboxMembershipSync, base.CurrentOrganizationId, "NewTMCMD_" + base.ExecutingUserIdentityName, this.lastUsedDc, SyncOption.Default);
					base.WriteVerbose(new LocalizedString(string.Format("The membership sync result is {0}", enqueueResult.Result)));
					if (enqueueResult.Result == EnqueueResultType.Successful)
					{
						goto IL_409;
					}
					if (num > 12)
					{
						break;
					}
					Thread.Sleep(5000);
					num++;
				}
				this.WriteWarning(Strings.ErrorTeamMailboxEnqueueMembershipSyncEvent(enqueueResult.ResultDetail));
				goto IL_414;
				IL_409:
				base.WriteVerbose(Strings.SiteMailboxMembershipSyncEventEnqueued);
				IL_414:
				enqueueResult = RpcClientWrapper.EnqueueTeamMailboxSyncRequest(this.databaseLocationInfo.ServerFqdn, dataObject.ExchangeGuid, QueueType.TeamMailboxDocumentSync, base.CurrentOrganizationId, "NewTMCMD_" + base.ExecutingUserIdentityName, this.lastUsedDc, SyncOption.Default);
				base.WriteVerbose(new LocalizedString(string.Format("Document sync request to server {0} for MailboxGuid {1} using domain controller {2}. The result is: {3}", new object[]
				{
					this.databaseLocationInfo.ServerFqdn,
					dataObject.ExchangeGuid,
					this.lastUsedDc,
					enqueueResult.ResultDetail
				})));
				return;
			}
			this.WriteWarning(Strings.ErrorTeamMailboxEnqueueMembershipSyncEvent("No database location information available"));
		}

		protected override void WriteResult(ADObject result)
		{
			ADUser dataObject = (ADUser)result;
			TeamMailbox teamMailbox = TeamMailbox.FromDataObject(dataObject);
			teamMailbox.SetMyRole(this.executingUserId);
			teamMailbox.ShowInMyClient = true;
			base.WriteResult(teamMailbox);
		}

		private IList<TeamMailboxProvisioningPolicy> GetTeamMailboxPolicies()
		{
			OrganizationId currentOrganizationId = base.CurrentOrganizationId;
			IConfigurationSession session = this.ConfigurationSession;
			if (SharedConfiguration.IsDehydratedConfiguration(currentOrganizationId) || (SharedConfiguration.GetSharedConfigurationState(currentOrganizationId) & SharedTenantConfigurationState.Static) != SharedTenantConfigurationState.UnSupported)
			{
				ADSessionSettings adsessionSettings = null;
				SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(currentOrganizationId);
				if (sharedConfiguration != null)
				{
					adsessionSettings = sharedConfiguration.GetSharedConfigurationSessionSettings();
				}
				if (adsessionSettings == null)
				{
					adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, currentOrganizationId, base.ExecutingUserOrganizationId, false);
					adsessionSettings.IsSharedConfigChecked = true;
				}
				session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, adsessionSettings, 732, "GetTeamMailboxPolicies", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\TeamMailbox\\NewSiteMailbox.cs");
			}
			return DefaultTeamMailboxProvisioningPolicyUtility.GetDefaultPolicies(session);
		}

		private void CheckSharePointUrlSafetyInDatacenter(Uri proposedUrl)
		{
			bool flag = false;
			LocalizedString message = Strings.ErrorSharePointUrlNotWhitelisted;
			string[] source = new string[]
			{
				"not used in E15 CUs because they are on-premises only"
			};
			string text = this.ExtractSecondLevelDomainLowercase(proposedUrl);
			if (string.IsNullOrEmpty(text))
			{
				message = Strings.ErrorCannotParseUsefulHostnameFrom(proposedUrl.ToString());
			}
			else
			{
				flag = source.Contains(text);
				if (!flag)
				{
					Uri rootSiteUrl = Microsoft.Exchange.UnifiedGroups.SharePointUrl.GetRootSiteUrl(base.CurrentOrganizationId);
					string b = this.ExtractSecondLevelDomainLowercase(rootSiteUrl);
					flag = string.Equals(text, b, StringComparison.OrdinalIgnoreCase);
				}
			}
			if (!flag)
			{
				base.WriteError(new UrlInValidException(message), ExchangeErrorCategory.Authorization, null);
			}
		}

		private void CheckSharePointUrlSafetyOnPremises(Uri proposedUrl)
		{
			bool flag = false;
			if (proposedUrl != null)
			{
				string host = proposedUrl.Host;
				PartnerApplication[] rootOrgPartnerApplications = OAuthConfigHelper.GetRootOrgPartnerApplications();
				foreach (PartnerApplication partnerApplication in rootOrgPartnerApplications)
				{
					string a = null;
					try
					{
						base.WriteVerbose(Strings.VerboseCheckingAgainstPartnerApplicationMetadataUrl(partnerApplication.AuthMetadataUrl));
						Uri uri = new Uri(partnerApplication.AuthMetadataUrl);
						a = uri.Host;
					}
					catch
					{
					}
					if (string.Equals(a, host, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				base.WriteError(new UrlInValidException(Strings.ErrorSharePointUrlDoesNotMatchPartnerApplication), ExchangeErrorCategory.Authorization, null);
			}
		}

		private string ExtractSecondLevelDomainLowercase(Uri uri)
		{
			string result = null;
			if (uri != null)
			{
				string[] array = uri.Host.Split(new char[]
				{
					'.'
				});
				if (array.Length >= 2)
				{
					result = string.Format("{0}.{1}", array[array.Length - 2], array[array.Length - 1]).ToLowerInvariant();
				}
			}
			return result;
		}

		private ADObjectId executingUserId;

		private TeamMailboxHelper teamMailboxHelper;

		private TeamMailboxProvisioningPolicy provisioningPolicy;
	}
}
