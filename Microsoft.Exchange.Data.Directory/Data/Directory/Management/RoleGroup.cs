using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.ValidationRules;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class RoleGroup : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return RoleGroup.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public RoleGroup()
		{
		}

		public RoleGroup(ADGroup dataObject, Result<ExchangeRoleAssignment>[] roleAssignmentResults) : base(dataObject)
		{
			if (roleAssignmentResults != null)
			{
				foreach (Result<ExchangeRoleAssignment> result in roleAssignmentResults)
				{
					ExchangeRoleAssignment data = result.Data;
					this.RoleAssignments.Add(data.Id);
					if (data.Role != null && !this.Roles.Contains(data.Role))
					{
						this.Roles.Add(data.Role);
					}
				}
			}
		}

		internal static RoleGroup FromDataObject(ADGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new RoleGroup(dataObject, null);
		}

		internal static bool ContainsRoleAssignments(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[RoleGroupSchema.RoleAssignments];
			return multiValuedProperty.Count != 0;
		}

		internal void PopulateCapabilitiesProperty()
		{
			this.Capabilities = CapabilityIdentifierEvaluatorFactory.GetCapabilities(base.DataObject);
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleGroupSchema.ManagedBy];
			}
			set
			{
				this[RoleGroupSchema.ManagedBy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RoleAssignments
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleGroupSchema.RoleAssignments];
			}
			private set
			{
				this[RoleGroupSchema.RoleAssignments] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Roles
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleGroupSchema.Roles];
			}
			private set
			{
				this[RoleGroupSchema.Roles] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[RoleGroupSchema.DisplayName];
			}
			private set
			{
				this[RoleGroupSchema.DisplayName] = value;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[RoleGroupSchema.ExternalDirectoryObjectId];
			}
		}

		public MultiValuedProperty<ADObjectId> Members
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleGroupSchema.Members];
			}
			private set
			{
				this[RoleGroupSchema.Members] = value;
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[RoleGroupSchema.SamAccountName];
			}
			private set
			{
				this[RoleGroupSchema.SamAccountName] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[RoleGroupSchema.Description];
			}
			set
			{
				this[RoleGroupSchema.Description] = value;
			}
		}

		public RoleGroupType RoleGroupType
		{
			get
			{
				return (RoleGroupType)this[RoleGroupSchema.RoleGroupType];
			}
		}

		public string LinkedGroup
		{
			get
			{
				return (string)this[RoleGroupSchema.LinkedGroup];
			}
		}

		public MultiValuedProperty<Capability> Capabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[RoleGroupSchema.Capabilities];
			}
			private set
			{
				this[RoleGroupSchema.Capabilities] = value;
			}
		}

		public string LinkedPartnerGroupId
		{
			get
			{
				return (string)this[RoleGroupSchema.LinkedPartnerGroupId];
			}
		}

		public string LinkedPartnerOrganizationId
		{
			get
			{
				return (string)this[RoleGroupSchema.LinkedPartnerOrganizationId];
			}
		}

		public const string OrganizationManagement = "Organization Management";

		public const string RecipientManagement = "Recipient Management";

		public const string ViewOnlyOrganizationManagement = "View-Only Organization Management";

		public const string PublicFolderManagement = "Public Folder Management";

		public const string UMManagement = "UM Management";

		public const string HelpDesk = "Help Desk";

		public const string RecordsManagement = "Records Management";

		public const string DiscoveryManagement = "Discovery Management";

		public const string ServerManagement = "Server Management";

		public const string DelegatedSetup = "Delegated Setup";

		public const string HygieneManagement = "Hygiene Management";

		public const string ManagementForestOperator = "Management Forest Operator";

		public const string ManagementForestTier1Support = "Management Forest Tier 1 Support";

		public const string ViewOnlyManagementForestOperator = "View-Only Mgmt Forest Operator";

		public const string ManagementForestMonitoring = "Management Forest Monitoring";

		public const string DataCenterManagement = "DataCenter Management";

		public const string ViewOnlyLocalServerAccess = "View-Only Local Server Access";

		public const string DestructiveAccess = "Destructive Access";

		public const string ElevatedPermissions = "Elevated Permissions";

		public const string ServiceAccounts = "Service Accounts";

		public const string Operations = "Operations";

		public const string ViewOnly = "View-Only";

		public const string ComplianceManagement = "Compliance Management";

		public const string ViewOnlyPII = "View-Only PII";

		public const string CapacityDestructiveAccess = "Capacity Destructive Access";

		public const string CapacityServerAdmins = "Capacity Server Admins";

		public const string CapacityFrontendServerAdmins = "Cafe Server Admins";

		public const string CustomerChangeAccess = "Customer Change Access";

		public const string CustomerDataAccess = "Customer Data Access";

		public const string AccessToCustomerDataDCOnly = "Access To Customer Data - DC Only";

		public const string DatacenterOperationsDCOnly = "Datacenter Operations - DC Only";

		public const string CustomerDestructiveAccess = "Customer Destructive Access";

		public const string CustomerPIIAccess = "Customer PII Access";

		public const string DedicatedSupportAccess = "Dedicated Support Access";

		public const string ECSAdminServerAccess = "ECS Admin - Server Access";

		public const string ECSPIIAccessServerAccess = "ECS PII Access - Server Access";

		public const string ECSAdmin = "ECS Admin";

		public const string ECSPIIAccess = "ECS PII Access";

		public const string ManagementAdminAccess = "Management Admin Access";

		public const string ManagementCACoreAdmin = "Management CA Core Admin";

		public const string ManagementChangeAccess = "Management Change Access";

		public const string ManagementDestructiveAccess = "Management Destructive Access";

		public const string ManagementServerAdmins = "Management Server Admins";

		public const string CapacityDCAdmins = "Capacity DC Admins";

		public const string NetworkingAdminAccess = "Networking Admin Access";

		public const string NetworkingChangeAccess = "Networking Change Access";

		public const string CommunicationManagers = "Communications Manager";

		public const string MailboxManagement = "Mailbox Management";

		public const string FfoAntiSpamAdmins = "Ffo AntiSpam Admins";

		public const string AppLockerExemption = "AppLocker Exemption";

		public const string MsoPartnerTenantAdmin = "AdminAgents";

		public const string MsoPartnerTenantHelpdesk = "HelpdeskAgents";

		public const string MsoManagedTenantAdmin = "TenantAdmins";

		public const string MsoMailTenantAdmin = "ExchangeServiceAdmins";

		public const string MsoManagedTenantHelpdesk = "HelpdeskAdmins";

		public static readonly Guid OrganizationManagementWkGuid = WellKnownGuid.EoaWkGuid;

		public static readonly Guid RecipientManagementWkGuid = WellKnownGuid.EmaWkGuid;

		public static readonly Guid ViewOnlyOrganizationManagementWkGuid = WellKnownGuid.EraWkGuid;

		public static readonly Guid PublicFolderManagementWkGuid = WellKnownGuid.EpaWkGuid;

		public static readonly Guid UMManagementWkGuid = WellKnownGuid.RgUmManagementWkGuid;

		public static readonly Guid HelpDeskWkGuid = WellKnownGuid.RgHelpDeskWkGuid;

		public static readonly Guid RecordsManagementWkGuid = WellKnownGuid.RgRecordsManagementWkGuid;

		public static readonly Guid DiscoveryManagementWkGuid = WellKnownGuid.RgDiscoveryManagementWkGuid;

		public static readonly Guid ServerManagementWkGuid = WellKnownGuid.RgServerManagementWkGuid;

		public static readonly Guid DelegatedSetupWkGuid = WellKnownGuid.RgDelegatedSetupWkGuid;

		public static readonly Guid HygieneManagementWkGuid = WellKnownGuid.RgHygieneManagementWkGuid;

		public static readonly Guid ManagementForestOperatorWkGuid = WellKnownGuid.RgManagementForestOperatorWkGuid;

		public static readonly Guid ManagementForestTier1SupportWkGuid = WellKnownGuid.RgManagementForestTier1SupportWkGuid;

		public static readonly Guid ViewOnlyManagementForestOperatorWkGuid = WellKnownGuid.RgViewOnlyManagementForestOperatorWkGuid;

		public static readonly Guid ManagementForestMonitoringWkGuid = WellKnownGuid.RgManagementForestMonitoringWkGuid;

		public static readonly Guid DataCenterManagementWkGuid = WellKnownGuid.RgDataCenterManagementWkGuid;

		public static readonly Guid ViewOnlyLocalServerAccessWkGuid = WellKnownGuid.RgViewOnlyLocalServerAccessWkGuid;

		public static readonly Guid DestructiveAccessWkGuid = WellKnownGuid.RgDestructiveAccessWkGuid;

		public static readonly Guid ElevatedPermissionsWkGuid = WellKnownGuid.RgElevatedPermissionsWkGuid;

		public static readonly Guid ServiceAccountsWkGuid = WellKnownGuid.RgServiceAccountsWkGuid;

		public static readonly Guid OperationsWkGuid = WellKnownGuid.RgOperationsWkGuid;

		public static readonly Guid ViewOnlyWkGuid = WellKnownGuid.RgViewOnlyWkGuid;

		public static readonly Guid CapacityDestructiveAccessWkGuid = WellKnownGuid.RgCapacityDestructiveAccessWkGuid;

		public static readonly Guid CapacityServerAdminsWkGuid = WellKnownGuid.RgCapacityServerAdminsWkGuid;

		public static readonly Guid CapacityFrontendServerAdminsWkGuid = WellKnownGuid.RgCapacityFrontendServerAdminsWkGuid;

		public static readonly Guid CustomerChangeAccessWkGuid = WellKnownGuid.RgCustomerChangeAccessWkGuid;

		public static readonly Guid CustomerDataAccessWkGuid = WellKnownGuid.RgCustomerDataAccessWkGuid;

		public static readonly Guid AccessToCustomerDataDCOnlyWkGuid = WellKnownGuid.RgAccessToCustomerDataDCOnlyWkGuid;

		public static readonly Guid DatacenterOperationsDCOnlyWkGuid = WellKnownGuid.RgDatacenterOperationsDCOnlyWkGuid;

		public static readonly Guid CustomerDestructiveAccessWkGuid = WellKnownGuid.RgCustomerDestructiveAccessWkGuid;

		public static readonly Guid CustomerPIIAccessWkGuid = WellKnownGuid.RgCustomerPIIAccessWkGuid;

		public static readonly Guid ManagementAdminAccessWkGuid = WellKnownGuid.RgManagementAdminAccessWkGuid;

		public static readonly Guid ManagementCACoreAdminWkGuid = WellKnownGuid.RgManagementCACoreAdminWkGuid;

		public static readonly Guid ManagementChangeAccessWkGuid = WellKnownGuid.RgManagementChangeAccessWkGuid;

		public static readonly Guid ManagementDestructiveAccessWkGuid = WellKnownGuid.RgManagementDestructiveAccessWkGuid;

		public static readonly Guid ManagementServerAdminsWkGuid = WellKnownGuid.RgManagementServerAdminsWkGuid;

		public static readonly Guid CapacityDCAdminsWkGuid = WellKnownGuid.RgCapacityDCAdminsWkGuid;

		public static readonly Guid NetworkingAdminAccessWkGuid = WellKnownGuid.RgNetworkingAdminAccessWkGuid;

		public static readonly Guid NetworkingChangeAccessWkGuid = WellKnownGuid.RgNetworkingChangeAccessWkGuid;

		public static readonly Guid MailboxManagementWkGuid = WellKnownGuid.RgMailboxManagementWkGuid;

		public static readonly Guid FfoAntiSpamAdminsWkGuid = WellKnownGuid.RgFfoAntiSpamAdminsWkGuid;

		public static readonly Guid AppLockerExemptionWkGuid = WellKnownGuid.RgAppLockerExemptionWkGuid;

		internal static Dictionary<int, string> RoleGroupStringIds = new Dictionary<int, string>
		{
			{
				1,
				"ExchangeOrgAdminDescription"
			},
			{
				2,
				"ExchangeRecipientAdminDescription"
			},
			{
				3,
				"ExchangeViewOnlyAdminDescription"
			},
			{
				4,
				"ExchangePublicFolderAdminDescription"
			},
			{
				5,
				"ExchangeUMManagementDescription"
			},
			{
				6,
				"ExchangeHelpDeskDescription"
			},
			{
				7,
				"ExchangeRecordsManagementDescription"
			},
			{
				8,
				"ExchangeDiscoveryManagementDescription"
			},
			{
				9,
				"ExchangeServerManagementDescription"
			},
			{
				10,
				"ExchangeDelegatedSetupDescription"
			},
			{
				11,
				"ExchangeHygieneManagementDescription"
			},
			{
				12,
				"ExchangeManagementForestOperatorDescription"
			},
			{
				13,
				"ExchangeManagementForestTier1SupportDescription"
			},
			{
				14,
				"ExchangeViewOnlyManagementForestOperatorDescription"
			},
			{
				15,
				"ExchangeManagementForestMonitoringDescription"
			},
			{
				23,
				"MsoManagedTenantAdminGroupDescription"
			},
			{
				24,
				"MsoMailTenantAdminGroupDescription"
			},
			{
				25,
				"MsoManagedTenantHelpdeskGroupDescription"
			},
			{
				26,
				"ComplianceManagementGroupDescription"
			},
			{
				27,
				"ViewOnlyPIIGroupDescription"
			}
		};

		public static readonly RoleGroupInitInfo OrganizationManagement_InitInfo = new RoleGroupInitInfo("Organization Management", 1, WellKnownGuid.EoaWkGuid);

		public static readonly RoleGroupInitInfo RecipientManagement_InitInfo = new RoleGroupInitInfo("Recipient Management", 2, WellKnownGuid.EmaWkGuid);

		public static readonly RoleGroupInitInfo ViewOnlyOrganizationManagement_InitInfo = new RoleGroupInitInfo("View-Only Organization Management", 3, WellKnownGuid.EraWkGuid);

		public static readonly RoleGroupInitInfo PublicFolderManagement_InitInfo = new RoleGroupInitInfo("Public Folder Management", 4, WellKnownGuid.EpaWkGuid);

		public static readonly RoleGroupInitInfo UMManagement_InitInfo = new RoleGroupInitInfo("UM Management", 5, WellKnownGuid.RgUmManagementWkGuid);

		public static readonly RoleGroupInitInfo HelpDesk_InitInfo = new RoleGroupInitInfo("Help Desk", 6, WellKnownGuid.RgHelpDeskWkGuid);

		public static readonly RoleGroupInitInfo RecordsManagement_InitInfo = new RoleGroupInitInfo("Records Management", 7, WellKnownGuid.RgRecordsManagementWkGuid);

		public static readonly RoleGroupInitInfo DiscoveryManagement_InitInfo = new RoleGroupInitInfo("Discovery Management", 8, WellKnownGuid.RgDiscoveryManagementWkGuid);

		public static readonly RoleGroupInitInfo ServerManagement_InitInfo = new RoleGroupInitInfo("Server Management", 9, WellKnownGuid.RgServerManagementWkGuid);

		public static readonly RoleGroupInitInfo DelegatedSetup_InitInfo = new RoleGroupInitInfo("Delegated Setup", 10, WellKnownGuid.RgDelegatedSetupWkGuid);

		public static readonly RoleGroupInitInfo HygieneManagement_InitInfo = new RoleGroupInitInfo("Hygiene Management", 11, WellKnownGuid.RgHygieneManagementWkGuid);

		public static readonly RoleGroupInitInfo ManagementForestOperator_InitInfo = new RoleGroupInitInfo("Management Forest Operator", 12, WellKnownGuid.RgManagementForestOperatorWkGuid);

		public static readonly RoleGroupInitInfo ManagementForestTier1Support_InitInfo = new RoleGroupInitInfo("Management Forest Tier 1 Support", 13, WellKnownGuid.RgManagementForestTier1SupportWkGuid);

		public static readonly RoleGroupInitInfo ViewOnlyManagementForestOperator_InitInfo = new RoleGroupInitInfo("View-Only Mgmt Forest Operator", 14, WellKnownGuid.RgViewOnlyManagementForestOperatorWkGuid);

		public static readonly RoleGroupInitInfo ManagementForestMonitoring_InitInfo = new RoleGroupInitInfo("Management Forest Monitoring", 15, WellKnownGuid.RgManagementForestMonitoringWkGuid);

		public static readonly RoleGroupInitInfo DataCenterManagement_InitInfo = new RoleGroupInitInfo("DataCenter Management", 16, WellKnownGuid.RgDataCenterManagementWkGuid);

		public static readonly RoleGroupInitInfo ViewOnlyLocalServerAccess_InitInfo = new RoleGroupInitInfo("View-Only Local Server Access", 17, WellKnownGuid.RgViewOnlyLocalServerAccessWkGuid);

		public static readonly RoleGroupInitInfo DestructiveAccess_InitInfo = new RoleGroupInitInfo("Destructive Access", 18, WellKnownGuid.RgDestructiveAccessWkGuid);

		public static readonly RoleGroupInitInfo ElevatedPermissions_InitInfo = new RoleGroupInitInfo("Elevated Permissions", 19, WellKnownGuid.RgElevatedPermissionsWkGuid);

		public static readonly RoleGroupInitInfo ServiceAccounts_InitInfo = new RoleGroupInitInfo("Service Accounts", 20, WellKnownGuid.RgServiceAccountsWkGuid);

		public static readonly RoleGroupInitInfo Operations_InitInfo = new RoleGroupInitInfo("Operations", 21, WellKnownGuid.RgOperationsWkGuid);

		public static readonly RoleGroupInitInfo ViewOnly_InitInfo = new RoleGroupInitInfo("View-Only", 22, WellKnownGuid.RgViewOnlyWkGuid);

		public static readonly RoleGroupInitInfo ComplianceManagement_InitInfo = new RoleGroupInitInfo("Compliance Management", 26, WellKnownGuid.RgComplianceManagementWkGuid);

		public static readonly RoleGroupInitInfo ViewOnlyPII_InitInfo = new RoleGroupInitInfo("View-Only PII", 27, WellKnownGuid.RgViewOnlyPIIWkGuid);

		public static readonly RoleGroupInitInfo CapacityDestructiveAccess_InitInfo = new RoleGroupInitInfo("Capacity Destructive Access", 28, WellKnownGuid.RgCapacityDestructiveAccessWkGuid);

		public static readonly RoleGroupInitInfo CapacityServerAdmins_InitInfo = new RoleGroupInitInfo("Capacity Server Admins", 29, WellKnownGuid.RgCapacityServerAdminsWkGuid);

		public static readonly RoleGroupInitInfo CapacityFrontendServerAdmins_InitInfo = new RoleGroupInitInfo("Cafe Server Admins", 43, WellKnownGuid.RgCapacityFrontendServerAdminsWkGuid);

		public static readonly RoleGroupInitInfo CustomerChangeAccess_InitInfo = new RoleGroupInitInfo("Customer Change Access", 30, WellKnownGuid.RgCustomerChangeAccessWkGuid);

		public static readonly RoleGroupInitInfo CustomerDataAccess_InitInfo = new RoleGroupInitInfo("Customer Data Access", 31, WellKnownGuid.RgCustomerDataAccessWkGuid);

		public static readonly RoleGroupInitInfo AccessToCustomerDataDCOnly_InitInfo = new RoleGroupInitInfo("Access To Customer Data - DC Only", 52, WellKnownGuid.RgAccessToCustomerDataDCOnlyWkGuid);

		public static readonly RoleGroupInitInfo DatacenterOperationsDCOnly_InitInfo = new RoleGroupInitInfo("Datacenter Operations - DC Only", 53, WellKnownGuid.RgDatacenterOperationsDCOnlyWkGuid);

		public static readonly RoleGroupInitInfo CustomerDestructiveAccess_InitInfo = new RoleGroupInitInfo("Customer Destructive Access", 32, WellKnownGuid.RgCustomerDestructiveAccessWkGuid);

		public static readonly RoleGroupInitInfo CustomerPIIAccess_InitInfo = new RoleGroupInitInfo("Customer PII Access", 33, WellKnownGuid.RgCustomerPIIAccessWkGuid);

		public static readonly RoleGroupInitInfo DedicatedSupportAccess_InitInfo = new RoleGroupInitInfo("Dedicated Support Access", 45, WellKnownGuid.RgDedicatedSupportAccessWkGuid);

		public static readonly RoleGroupInitInfo ECSAdminServerAccess_InitInfo = new RoleGroupInitInfo("ECS Admin - Server Access", 48, WellKnownGuid.RgECSAdminServerAccessWkGuid);

		public static readonly RoleGroupInitInfo ECSPIIAccessServerAccess_InitInfo = new RoleGroupInitInfo("ECS PII Access - Server Access", 49, WellKnownGuid.RgECSPIIAccessServerAccessWkGuid);

		public static readonly RoleGroupInitInfo ECSAdmin_InitInfo = new RoleGroupInitInfo("ECS Admin", 50, WellKnownGuid.RgECSAdminWkGuid);

		public static readonly RoleGroupInitInfo ECSPIIAccess_InitInfo = new RoleGroupInitInfo("ECS PII Access", 51, WellKnownGuid.RgECSPIIAccessWkGuid);

		public static readonly RoleGroupInitInfo ManagementAdminAccess_InitInfo = new RoleGroupInitInfo("Management Admin Access", 34, WellKnownGuid.RgManagementAdminAccessWkGuid);

		public static readonly RoleGroupInitInfo ManagementCACoreAdmin_InitInfo = new RoleGroupInitInfo("Management CA Core Admin", 41, WellKnownGuid.RgManagementCACoreAdminWkGuid);

		public static readonly RoleGroupInitInfo ManagementChangeAccess_InitInfo = new RoleGroupInitInfo("Management Change Access", 35, WellKnownGuid.RgManagementChangeAccessWkGuid);

		public static readonly RoleGroupInitInfo ManagementDestructiveAccess_InitInfo = new RoleGroupInitInfo("Management Destructive Access", 39, WellKnownGuid.RgManagementDestructiveAccessWkGuid);

		public static readonly RoleGroupInitInfo ManagementServerAdmins_InitInfo = new RoleGroupInitInfo("Management Server Admins", 36, WellKnownGuid.RgManagementServerAdminsWkGuid);

		public static readonly RoleGroupInitInfo CapacityDCAdmins_InitInfo = new RoleGroupInitInfo("Capacity DC Admins", 37, WellKnownGuid.RgCapacityDCAdminsWkGuid);

		public static readonly RoleGroupInitInfo NetworkingAdminAccess_InitInfo = new RoleGroupInitInfo("Networking Admin Access", 38, WellKnownGuid.RgNetworkingAdminAccessWkGuid);

		public static readonly RoleGroupInitInfo NetworkingChangeAccess_InitInfo = new RoleGroupInitInfo("Networking Change Access", 46, WellKnownGuid.RgNetworkingChangeAccessWkGuid);

		public static readonly RoleGroupInitInfo CommunicationManagers_InitInfo = new RoleGroupInitInfo("Communications Manager", 40, WellKnownGuid.RgCommunicationManagersWkGuid);

		public static readonly RoleGroupInitInfo MailboxManagement_InitInfo = new RoleGroupInitInfo("Mailbox Management", 42, WellKnownGuid.RgMailboxManagementWkGuid);

		public static readonly RoleGroupInitInfo FfoAntiSpamAdmins_InitInfo = new RoleGroupInitInfo("Ffo AntiSpam Admins", 44, WellKnownGuid.RgFfoAntiSpamAdminsWkGuid);

		public static readonly RoleGroupInitInfo AppLockerExemption_InitInfo = new RoleGroupInitInfo("AppLocker Exemption", 47, WellKnownGuid.RgAppLockerExemptionWkGuid);

		private static RoleGroupSchema schema = ObjectSchema.GetInstance<RoleGroupSchema>();

		internal enum RoleGroupTypeIds
		{
			Unknown,
			OrganizationManagement,
			RecipientManagement,
			ViewOnlyOrganizationManagement,
			PublicFolderManagement,
			UMManagement,
			HelpDesk,
			RecordsManagement,
			DiscoveryManagement,
			ServerManagement,
			DelegatedSetup,
			HygieneManagement,
			ManagementForestOperator,
			ManagementForestTier1Support,
			ViewOnlyManagementForestOperator,
			ManagementForestMonitoring,
			DataCenterManagement,
			ViewOnlyLocalServerAccess,
			DestructiveAccess,
			ElevatedPermissions,
			ServiceAccounts,
			Operations,
			ViewOnly,
			MsoManagedTenantAdmin,
			MsoMailTenantAdmin,
			MsoManagedTenantHelpdesk,
			ComplianceManagement,
			ViewOnlyPII,
			CapacityDestructiveAccess,
			CapacityServerAdmins,
			CustomerChangeAccess,
			CustomerDataAccess,
			CustomerDestructiveAccess,
			CustomerPIIAccess,
			ManagementAdminAccess,
			ManagementChangeAccess,
			ManagementServerAdmins,
			CapacityDCAdmins,
			NetworkingAdminAccess,
			ManagementDestructiveAccess,
			CommunicationManagers,
			ManagementCACoreAdmin,
			MailboxManagement,
			CapacityFrontendServerAdmins,
			FfoAntiSpamAdmins,
			DedicatedSupportAccess,
			NetworkingChangeAccess,
			AppLockerExemption,
			ECSAdminServerAccess,
			ECSPIIAccessServerAccess,
			ECSAdmin,
			ECSPIIAccess,
			AccessToCustomerDataDCOnly,
			DatacenterOperationsDCOnly
		}
	}
}
