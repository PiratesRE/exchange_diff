using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum ServerRole
	{
		[LocDescription(DirectoryStrings.IDs.ServerRoleNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.ServerRoleCafe)]
		Cafe = 1,
		[LocDescription(DirectoryStrings.IDs.ServerRoleMailbox)]
		Mailbox = 2,
		[LocDescription(DirectoryStrings.IDs.ServerRoleClientAccess)]
		ClientAccess = 4,
		[LocDescription(DirectoryStrings.IDs.ServerRoleUnifiedMessaging)]
		UnifiedMessaging = 16,
		[LocDescription(DirectoryStrings.IDs.ServerRoleHubTransport)]
		HubTransport = 32,
		[LocDescription(DirectoryStrings.IDs.ServerRoleEdge)]
		Edge = 64,
		[LocDescription(DirectoryStrings.IDs.ServerRoleAll)]
		All = 16503,
		[LocDescription(DirectoryStrings.IDs.ServerRoleMonitoring)]
		Monitoring = 128,
		[LocDescription(DirectoryStrings.IDs.ServerRoleExtendedRole2)]
		CentralAdmin = 256,
		[LocDescription(DirectoryStrings.IDs.ServerRoleExtendedRole3)]
		CentralAdminDatabase = 512,
		[LocDescription(DirectoryStrings.IDs.ServerRoleExtendedRole4)]
		DomainController = 1024,
		[LocDescription(DirectoryStrings.IDs.ServerRoleExtendedRole5)]
		WindowsDeploymentServer = 2048,
		[LocDescription(DirectoryStrings.IDs.ServerRoleProvisionedServer)]
		ProvisionedServer = 4096,
		[LocDescription(DirectoryStrings.IDs.ServerRoleLanguagePacks)]
		LanguagePacks = 8192,
		[LocDescription(DirectoryStrings.IDs.ServerRoleFrontendTransport)]
		FrontendTransport = 16384,
		[LocDescription(DirectoryStrings.IDs.ServerRoleCafeArray)]
		CafeArray = 32768,
		[LocDescription(DirectoryStrings.IDs.ServerRoleFfoWebServices)]
		FfoWebService = 65536,
		[LocDescription(DirectoryStrings.IDs.ServerRoleOSP)]
		OSP = 131072,
		[LocDescription(DirectoryStrings.IDs.ServerRoleExtendedRole7)]
		ARR = 262144,
		[LocDescription(DirectoryStrings.IDs.ServerRoleManagementFrontEnd)]
		ManagementFrontEnd = 524288,
		[LocDescription(DirectoryStrings.IDs.ServerRoleManagementBackEnd)]
		ManagementBackEnd = 1048576,
		[LocDescription(DirectoryStrings.IDs.ServerRoleSCOM)]
		SCOM = 2097152,
		[LocDescription(DirectoryStrings.IDs.ServerRoleCentralAdminFrontEnd)]
		CentralAdminFrontEnd = 4194304,
		[LocDescription(DirectoryStrings.IDs.ServerRoleNAT)]
		NAT = 8388608,
		[LocDescription(DirectoryStrings.IDs.ServerRoleDHCP)]
		DHCP = 16777216
	}
}
