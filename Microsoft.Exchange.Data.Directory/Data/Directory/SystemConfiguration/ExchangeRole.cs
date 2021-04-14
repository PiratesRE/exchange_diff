using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Resources;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ExchangeRole : ADConfigurationObject
	{
		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeRole.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeRole.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeRoleSchema.CurrentRoleVersion;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RoleEntry> RoleEntries
		{
			get
			{
				return (MultiValuedProperty<RoleEntry>)this.propertyBag[ExchangeRoleSchema.RoleEntries];
			}
			internal set
			{
				this.propertyBag[ExchangeRoleSchema.RoleEntries] = value;
			}
		}

		public RoleType RoleType
		{
			get
			{
				return (RoleType)this[ExchangeRoleSchema.RoleType];
			}
			internal set
			{
				this[ExchangeRoleSchema.RoleType] = value;
			}
		}

		public ScopeType ImplicitRecipientReadScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleSchema.ImplicitRecipientReadScope];
			}
		}

		public ScopeType ImplicitRecipientWriteScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleSchema.ImplicitRecipientWriteScope];
			}
		}

		public ScopeType ImplicitConfigReadScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleSchema.ImplicitConfigReadScope];
			}
		}

		public ScopeType ImplicitConfigWriteScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleSchema.ImplicitConfigWriteScope];
			}
		}

		public bool IsRootRole
		{
			get
			{
				return (bool)this[ExchangeRoleSchema.IsRootRole];
			}
		}

		public bool IsEndUserRole
		{
			get
			{
				return (bool)this[ExchangeRoleSchema.IsEndUserRole];
			}
		}

		internal bool IsPartnerApplicationRole
		{
			get
			{
				return ExchangeRole.IsPartnerApplicationRoleType(this.RoleType);
			}
		}

		public string MailboxPlanIndex
		{
			get
			{
				return (string)this[ExchangeRoleSchema.MailboxPlanIndex];
			}
			internal set
			{
				this[ExchangeRoleSchema.MailboxPlanIndex] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[ExchangeRoleSchema.Description];
			}
			set
			{
				this[ExchangeRoleSchema.Description] = value;
			}
		}

		public ADObjectId Parent
		{
			get
			{
				return (ADObjectId)this[ExchangeRoleSchema.Parent];
			}
		}

		internal RoleState RoleState
		{
			get
			{
				return (RoleState)this[ExchangeRoleSchema.RoleState];
			}
			set
			{
				this[ExchangeRoleSchema.RoleState] = value;
			}
		}

		public bool IsDeprecated
		{
			get
			{
				return (bool)this[ExchangeRoleSchema.IsDeprecated];
			}
		}

		internal bool HasDownlevelData
		{
			get
			{
				return (bool)this[ExchangeRoleSchema.HasDownlevelData];
			}
		}

		internal bool IsUnscopedTopLevel
		{
			get
			{
				return this.IsRootRole && this.IsUnscoped;
			}
		}

		internal bool IsUnscoped
		{
			get
			{
				return this.RoleType == RoleType.UnScoped;
			}
		}

		internal bool AllowEmptyRole
		{
			get
			{
				return this.allowEmptyRole;
			}
			set
			{
				this.allowEmptyRole = value;
			}
		}

		static ExchangeRole()
		{
			ExchangeRole.EndUserRoleTypes = new RoleType[]
			{
				RoleType.MyOptions,
				RoleType.MyFacebookEnabled,
				RoleType.MyLinkedInEnabled,
				RoleType.MyBaseOptions,
				RoleType.MyMailSubscriptions,
				RoleType.MyContactInformation,
				RoleType.MyProfileInformation,
				RoleType.MyRetentionPolicies,
				RoleType.MyTextMessaging,
				RoleType.MyVoiceMail,
				RoleType.MyDistributionGroups,
				RoleType.MyDistributionGroupMembership,
				RoleType.MyDiagnostics,
				RoleType.MyMailboxDelegation,
				RoleType.MyTeamMailboxes,
				RoleType.MyMarketplaceApps,
				RoleType.MyCustomApps,
				RoleType.MyReadWriteMailboxApps
			};
			Array.Sort<RoleType>(ExchangeRole.EndUserRoleTypes);
			ExchangeRole.PartnerApplicationRoleTypes = new RoleType[]
			{
				RoleType.UserApplication,
				RoleType.ArchiveApplication,
				RoleType.LegalHoldApplication,
				RoleType.OfficeExtensionApplication,
				RoleType.TeamMailboxLifecycleApplication,
				RoleType.MailboxSearchApplication
			};
			Array.Sort<RoleType>(ExchangeRole.PartnerApplicationRoleTypes);
			ExchangeRole.ScopeSets = new Dictionary<RoleType, SimpleScopeSet<ScopeType>>(68);
			ExchangeRole.ScopeSets[RoleType.Custom] = new SimpleScopeSet<ScopeType>(ScopeType.NotApplicable, ScopeType.NotApplicable, ScopeType.NotApplicable, ScopeType.NotApplicable);
			ExchangeRole.ScopeSets[RoleType.UnScoped] = new SimpleScopeSet<ScopeType>(ScopeType.NotApplicable, ScopeType.NotApplicable, ScopeType.NotApplicable, ScopeType.NotApplicable);
			ExchangeRole.ScopeSets[RoleType.MyBaseOptions] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyDiagnostics] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyMailSubscriptions] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyContactInformation] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyProfileInformation] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyRetentionPolicies] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyTextMessaging] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyMarketplaceApps] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyCustomApps] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyReadWriteMailboxApps] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyTeamMailboxes] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyVoiceMail] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DistributionGroupManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.MyDistributionGroups] = new SimpleScopeSet<ScopeType>(ScopeType.MyGAL, ScopeType.MyDistributionGroups, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.MyDistributionGroupMembership] = new SimpleScopeSet<ScopeType>(ScopeType.MyGAL, ScopeType.MyGAL, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.HelpdeskRecipientManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.GALSynchronizationManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ApplicationImpersonation] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.PartnerDelegatedTenantManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.CentralAdminManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyCentralAdminManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyCentralAdminSupport] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.UnScopedRoleManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.LawEnforcementRequests] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyRoleManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.Reporting] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.PersonallyIdentifiableInformation] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.MailRecipients] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.FederatedSharing] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DatabaseAvailabilityGroups] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.Databases] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.PublicFolders] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.AddressLists] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.RecipientPolicies] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DisasterRecovery] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.Monitoring] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DatabaseCopies] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UnifiedMessaging] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.Journaling] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.RemoteAndAcceptedDomains] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.EmailAddressPolicies] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TransportRules] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.SendConnectors] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.EdgeSubscriptions] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrganizationTransportSettings] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ExchangeServers] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ExchangeVirtualDirectories] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ExchangeServerCertificates] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.POP3AndIMAP4Protocols] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ReceiveConnectors] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UMMailboxes] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UserOptions] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.SecurityGroupCreationAndMembership] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MailRecipientCreation] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MessageTracking] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.RoleManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyRecipients] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyConfiguration] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.DistributionGroups] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MailEnabledPublicFolders] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MoveMailboxes] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ResetPassword] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.AuditLogs] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.RetentionManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.SupportDiagnostics] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MailboxSearch] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.LegalHold] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.MailTips] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ActiveDirectoryPermissions] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UMPrompts] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.Migration] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DataCenterOperations] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TransportHygiene] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TransportQueues] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.Supervision] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.CmdletExtensionAgents] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrganizationConfiguration] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrganizationClientAccess] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ExchangeConnectors] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TransportAgents] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.InformationRightsManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MailboxImportExport] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyAuditLogs] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.DataCenterDestructiveOperations] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.NetworkingManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.WorkloadManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.CentralAdminCredentialManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TeamMailboxes] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ActiveMonitoring] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DataLossPrevention] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrgMarketplaceApps] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrgCustomApps] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UserApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ArchiveApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.LegalHoldApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OfficeExtensionApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.TeamMailboxLifecycleApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MailboxSearchApplication] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.PublicFolderReplication] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.OrganizationManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.ViewOnlyOrganizationManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.None, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.RecipientManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UmManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.RecordsManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UmRecipientManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.UMPromptManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DiscoveryManagement] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.None, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.MyOptions] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyFacebookEnabled] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyLinkedInEnabled] = new SimpleScopeSet<ScopeType>(ScopeType.Self, ScopeType.Self, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.MyMailboxDelegation] = new SimpleScopeSet<ScopeType>(ScopeType.MyGAL, ScopeType.MailboxICanDelegate, ScopeType.OrganizationConfig, ScopeType.None);
			ExchangeRole.ScopeSets[RoleType.ExchangeCrossServiceIntegration] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.AccessToCustomerDataDCOnly] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
			ExchangeRole.ScopeSets[RoleType.DatacenterOperationsDCOnly] = new SimpleScopeSet<ScopeType>(ScopeType.Organization, ScopeType.Organization, ScopeType.OrganizationConfig, ScopeType.OrganizationConfig);
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (!base.ExchangeVersion.IsOlderThan(ExchangeRoleSchema.RoleType.VersionAdded))
			{
				MultiValuedProperty<RoleEntry> roleEntries = this.RoleEntries;
				if (!base.ExchangeVersion.IsOlderThan(ExchangeRoleSchema.Exchange2009_R4DF5) && roleEntries.Count == 0 && !this.IsDeprecated && !this.AllowEmptyRole)
				{
					if (RoleType.UnScoped != this.RoleType)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.NoRoleEntriesFound, this.Identity, base.OriginatingServer));
					}
					else if (!this.IsRootRole)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.NoRoleEntriesCmdletOrScriptFound, this.Identity, base.OriginatingServer));
					}
				}
				object obj = null;
				if (!this.propertyBag.TryGetField(ExchangeRoleSchema.RoleType, ref obj) || obj == null)
				{
					errors.Add(new PropertyValidationError(DataStrings.PropertyIsMandatory, ExchangeRoleSchema.RoleType, null));
				}
				foreach (RoleEntry roleEntry in roleEntries)
				{
					ManagementRoleEntryType managementRoleEntryType;
					if (roleEntry is CmdletRoleEntry)
					{
						managementRoleEntryType = ManagementRoleEntryType.Cmdlet;
					}
					else if (roleEntry is ScriptRoleEntry)
					{
						managementRoleEntryType = ManagementRoleEntryType.Script;
					}
					else if (roleEntry is ApplicationPermissionRoleEntry)
					{
						managementRoleEntryType = ManagementRoleEntryType.ApplicationPermission;
					}
					else
					{
						if (!(roleEntry is WebServiceRoleEntry))
						{
							continue;
						}
						managementRoleEntryType = ManagementRoleEntryType.WebService;
					}
					if ((managementRoleEntryType == ManagementRoleEntryType.Script && this.RoleType != RoleType.Custom && this.RoleType != RoleType.UnScoped) || (managementRoleEntryType == ManagementRoleEntryType.ApplicationPermission && this.RoleType != RoleType.ApplicationImpersonation && this.RoleType != RoleType.PersonallyIdentifiableInformation))
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.InvalidRoleEntryType(roleEntry.ToString(), this.RoleType, managementRoleEntryType), this.Identity, base.OriginatingServer));
					}
				}
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if ((int)this[ExchangeRoleSchema.RoleFlags] == 0)
			{
				errors.Add(new PropertyValidationError(DataStrings.PropertyIsMandatory, ExchangeRoleSchema.RoleFlags, 0));
			}
		}

		internal static object HasDownlevelDataGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<RoleEntry> multiValuedProperty = (MultiValuedProperty<RoleEntry>)propertyBag[ExchangeRoleSchema.InternalDownlevelRoleEntries];
			return ((ExchangeObjectVersion)propertyBag[ADObjectSchema.ExchangeVersion]).IsSameVersion(ExchangeRoleSchema.CurrentRoleVersion) && multiValuedProperty != null && multiValuedProperty.Count > 0;
		}

		internal static object IsRootRoleGetter(IPropertyBag propertyBag)
		{
			ADObjectId roleId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			return ExchangeRole.IsRootRoleInternal(roleId);
		}

		internal static bool IsRootRoleInternal(ADObjectId roleId)
		{
			return roleId != null && (roleId.Parent.Name.Equals("Roles", StringComparison.OrdinalIgnoreCase) && roleId.Parent.Parent.Name.Equals("RBAC", StringComparison.OrdinalIgnoreCase)) && roleId.DistinguishedName.LastIndexOf("CN=Roles,CN=RBAC", StringComparison.OrdinalIgnoreCase) == roleId.DistinguishedName.IndexOf("CN=Roles,CN=RBAC", StringComparison.OrdinalIgnoreCase);
		}

		internal void StampIsEndUserRole()
		{
			this[ExchangeRoleSchema.IsEndUserRole] = (Array.BinarySearch<RoleType>(ExchangeRole.EndUserRoleTypes, this.RoleType) >= 0);
		}

		internal static object MailboxPlanIndexGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ExchangeRoleSchema.RawMailboxPlanIndex];
			RoleType roleType = (RoleType)propertyBag[ExchangeRoleSchema.RoleType];
			string name = ((ADObjectId)propertyBag[ADObjectSchema.Id]).Name;
			if (!ExchangeRoleSchema.RawMailboxPlanIndex.DefaultValue.Equals(text))
			{
				return text;
			}
			if (propertyBag[ADObjectSchema.ConfigurationUnit] != null && Array.BinarySearch<RoleType>(ExchangeRole.EndUserRoleTypes, roleType) >= 0 && (bool)ExchangeRole.IsRootRoleGetter(propertyBag) && !name.Equals(roleType.ToString(), StringComparison.OrdinalIgnoreCase) && !name.Replace(" ", string.Empty).Equals(roleType.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return "0";
			}
			return ExchangeRoleSchema.RawMailboxPlanIndex.DefaultValue;
		}

		internal static void MailboxPlanIndexSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ExchangeRoleSchema.RawMailboxPlanIndex] = value;
		}

		internal static object DescriptionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ExchangeRoleSchema.RawDescription];
			if (ExchangeRoleSchema.RawDescription.DefaultValue.Equals(text))
			{
				RoleType roleType = (RoleType)propertyBag[ExchangeRoleSchema.RoleType];
				if (Enum.IsDefined(typeof(RoleType), roleType))
				{
					try
					{
						if ((bool)ExchangeRole.IsRootRoleGetter(propertyBag))
						{
							text = ExchangeRole.resourceManager.GetString("RoleTypeDescription_" + roleType.ToString());
						}
						else
						{
							ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
							int num = adobjectId.Name.IndexOf("_");
							string str = (num == -1) ? adobjectId.Name : adobjectId.Name.Substring(0, num);
							string @string = ExchangeRole.resourceManager.GetString("CustomRoleDescription_" + str);
							if (!string.IsNullOrEmpty(@string))
							{
								text = @string;
							}
						}
					}
					catch (MissingManifestResourceException)
					{
					}
				}
			}
			return text;
		}

		internal static void DescriptionSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ExchangeRoleSchema.RawDescription] = value;
		}

		internal static object ParentGetter(IPropertyBag propertyBag)
		{
			if ((bool)ExchangeRole.IsRootRoleGetter(propertyBag))
			{
				return null;
			}
			return ((ADObjectId)propertyBag[ADObjectSchema.Id]).Parent;
		}

		internal void ApplyChangesToDownlevelData(ExchangeRole parentRole)
		{
			if (parentRole == null)
			{
				throw new ArgumentNullException("parentRole");
			}
			if (!this.HasDownlevelData)
			{
				return;
			}
			MultiValuedProperty<RoleEntry> multiValuedProperty = (MultiValuedProperty<RoleEntry>)this[ExchangeRoleSchema.InternalDownlevelRoleEntries];
			object[] removed = this.RoleEntries.Removed;
			for (int i = 0; i < removed.Length; i++)
			{
				RoleEntry roleEntry = (RoleEntry)removed[i];
				RoleEntry downlevelEntryToFind = roleEntry.MapToPreviousVersion();
				RoleEntry roleEntry2 = multiValuedProperty.Find((RoleEntry dre) => 0 == RoleEntry.CompareRoleEntriesByName(dre, downlevelEntryToFind));
				if (roleEntry2 != null)
				{
					multiValuedProperty.Remove(roleEntry2);
				}
			}
			object[] added = this.RoleEntries.Added;
			for (int j = 0; j < added.Length; j++)
			{
				RoleEntry roleEntry3 = (RoleEntry)added[j];
				RoleEntry downlevelEntryToFind = roleEntry3.MapToPreviousVersion();
				MultiValuedProperty<RoleEntry> multiValuedProperty2 = (MultiValuedProperty<RoleEntry>)parentRole[ExchangeRoleSchema.InternalDownlevelRoleEntries];
				RoleEntry roleEntry4 = multiValuedProperty2.Find((RoleEntry dre) => 0 == RoleEntry.CompareRoleEntriesByName(dre, downlevelEntryToFind));
				if (!(roleEntry4 == null))
				{
					List<string> list = new List<string>();
					foreach (string newParameter in roleEntry3.Parameters)
					{
						string text = roleEntry3.MapParameterToPreviousVersion(newParameter);
						if (roleEntry4.ContainsParameter(text))
						{
							list.Add(text);
						}
					}
					RoleEntry item = downlevelEntryToFind.Clone(list);
					multiValuedProperty.Add(item);
				}
			}
		}

		internal static bool IsAdminRole(RoleType roleToCheck)
		{
			return Array.BinarySearch<RoleType>(ExchangeRole.EndUserRoleTypes, roleToCheck) < 0;
		}

		internal static bool IsPartnerApplicationRoleType(RoleType roleToCheck)
		{
			return Array.BinarySearch<RoleType>(ExchangeRole.PartnerApplicationRoleTypes, roleToCheck) >= 0;
		}

		internal void StampImplicitScopes()
		{
			SimpleScopeSet<ScopeType> simpleScopeSet = ExchangeRole.ScopeSets[this.RoleType];
			this[ExchangeRoleSchema.ImplicitRecipientReadScope] = simpleScopeSet.RecipientReadScope;
			this[ExchangeRoleSchema.ImplicitRecipientWriteScope] = simpleScopeSet.RecipientWriteScope;
			this[ExchangeRoleSchema.ImplicitConfigReadScope] = simpleScopeSet.ConfigReadScope;
			this[ExchangeRoleSchema.ImplicitConfigWriteScope] = simpleScopeSet.ConfigWriteScope;
		}

		internal SimpleScopeSet<ScopeType> GetImplicitScopeSet()
		{
			return ExchangeRole.ScopeSets[this.RoleType];
		}

		private const string RolesToken = "Roles";

		private const string RbacToken = "RBAC";

		private const string RdnRolesContainer = "CN=Roles,CN=RBAC";

		internal static readonly ADObjectId RdnContainer = new ADObjectId("CN=Roles,CN=RBAC");

		private static ExchangeRoleSchema schema = ObjectSchema.GetInstance<ExchangeRoleSchema>();

		private static string mostDerivedClass = "msExchRole";

		internal static readonly RoleType[] EndUserRoleTypes;

		internal static readonly RoleType[] PartnerApplicationRoleTypes;

		private static ExchangeResourceManager resourceManager = ExchangeResourceManager.GetResourceManager(typeof(CoreStrings).FullName, typeof(CoreStrings).Assembly);

		internal static Dictionary<RoleType, SimpleScopeSet<ScopeType>> ScopeSets;

		private bool allowEmptyRole;
	}
}
