using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal enum CannedRoleGroup
	{
		[Description("Organization Management")]
		OrganizationManagement,
		[Description("Recipient Management")]
		RecipientManagement,
		[Description("View-Only Organization Management")]
		ViewOnlyOrganizationManagement,
		[Description("Public Folder Management")]
		PublicFolderManagement,
		[Description("UM Management")]
		UMManagement,
		[Description("Help Desk")]
		HelpDesk,
		[Description("Records Management")]
		RecordsManagement,
		[Description("Discovery Management")]
		DiscoveryManagement,
		[Description("Server Management")]
		ServerManagement,
		[Description("Delegated Setup")]
		DelegatedSetup,
		[Description("Hygiene Management")]
		HygieneManagement,
		[Description("Management Forest Operator")]
		ManagementForestOperator,
		[Description("Management Forest Tier 1 Support")]
		ManagementForestTier1Support,
		[Description("View-Only Mgmt Forest Operator")]
		ViewOnlyManagementForestOperator,
		[Description("Management Forest Monitoring")]
		ManagementForestMonitoring,
		[Description("DataCenter Management")]
		DataCenterManagement,
		[Description("View-Only Local Server Access")]
		ViewOnlyLocalServerAccess,
		[Description("Destructive Access")]
		DestructiveAccess,
		[Description("Elevated Permissions")]
		ElevatedPermissions,
		[Description("Service Accounts")]
		ServiceAccounts,
		[Description("Operations")]
		Operations,
		[Description("View-Only")]
		ViewOnly,
		[Description("Compliance Management")]
		ComplianceManagement,
		[Description("View-Only PII")]
		ViewOnlyPII,
		[Description("Capacity Destructive Access")]
		CapacityDestructiveAccess,
		[Description("Capacity Server Admins")]
		CapacityServerAdmins,
		[Description("Customer Change Access")]
		CustomerChangeAccess,
		[Description("Customer Data Access")]
		CustomerDataAccess,
		[Description("Customer Destructive Access")]
		CustomerDestructiveAccess,
		[Description("Customer PII Access")]
		CustomerPIIAccess,
		[Description("Management Admin Access")]
		ManagementAdminAccess,
		[Description("Management Server Admins")]
		ManagementServerAdmins,
		[Description("Management Change Access")]
		ManagementChangeAccess,
		[Description("Capacity DC Admins")]
		CapacityDCAdmins,
		[Description("Networking Admin Access")]
		NetworkingAdminAccess,
		[Description("Management Destructive Access")]
		ManagementDestructiveAccess,
		[Description("Management CA Core Admin")]
		ManagementCACoreAdmin,
		[Description("Mailbox Management")]
		MailboxManagement,
		[Description("Cafe Server Admins")]
		CapacityFrontendServerAdmin,
		[Description("Ffo AntiSpam Admins")]
		FfoAntiSpamAdmins,
		[Description("Dedicated Support Access")]
		DedicatedSupportAccess,
		[Description("Networking Change Access")]
		NetworkingChangeAccess,
		[Description("AppLocker Exemption")]
		AppLockerExemption = 48,
		[Description("ECS Admin - Server Access")]
		ECSAdminServerAccess,
		[Description("ECS PII Access - Server Access")]
		ECSPIIAccessServerAccess,
		[Description("ECS Admin")]
		ECSAdmin,
		[Description("ECS PII Access")]
		ECSPIIAccess,
		[Description("Access To Customer Data - DC Only")]
		AccessToCustomerDataDCOnly,
		[Description("Datacenter Operations - DC Only")]
		DatacenterOperationsDCOnly
	}
}
