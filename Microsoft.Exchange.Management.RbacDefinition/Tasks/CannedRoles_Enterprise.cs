using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class CannedRoles_Enterprise
	{
		internal static RoleDefinition[] Definition = new RoleDefinition[]
		{
			new RoleDefinition("Active Directory Permissions", RoleType.ActiveDirectoryPermissions, CannedRoles_Enterprise.Active_Directory_Permissions.Cmdlets),
			new RoleDefinition("Address Lists", RoleType.AddressLists, CannedRoles_Enterprise.Address_Lists.Cmdlets),
			new RoleDefinition("ApplicationImpersonation", RoleType.ApplicationImpersonation, CannedRoles_Enterprise.ApplicationImpersonation.Cmdlets),
			new RoleDefinition("Audit Logs", RoleType.AuditLogs, CannedRoles_Enterprise.Audit_Logs.Cmdlets),
			new RoleDefinition("Cmdlet Extension Agents", RoleType.CmdletExtensionAgents, CannedRoles_Enterprise.Cmdlet_Extension_Agents.Cmdlets),
			new RoleDefinition("Data Loss Prevention", RoleType.DataLossPrevention, CannedRoles_Enterprise.Data_Loss_Prevention.Cmdlets),
			new RoleDefinition("DataCenter Operations", RoleType.DataCenterOperations, CannedRoles_Enterprise.DataCenter_Operations.Cmdlets),
			new RoleDefinition("Database Availability Groups", RoleType.DatabaseAvailabilityGroups, CannedRoles_Enterprise.Database_Availability_Groups.Cmdlets),
			new RoleDefinition("Database Copies", RoleType.DatabaseCopies, CannedRoles_Enterprise.Database_Copies.Cmdlets),
			new RoleDefinition("Databases", RoleType.Databases, CannedRoles_Enterprise.Databases.Cmdlets),
			new RoleDefinition("Disaster Recovery", RoleType.DisasterRecovery, CannedRoles_Enterprise.Disaster_Recovery.Cmdlets),
			new RoleDefinition("Distribution Groups", RoleType.DistributionGroups, CannedRoles_Enterprise.Distribution_Groups.Cmdlets),
			new RoleDefinition("E-Mail Address Policies", RoleType.EmailAddressPolicies, CannedRoles_Enterprise.E_Mail_Address_Policies.Cmdlets),
			new RoleDefinition("Edge Subscriptions", RoleType.EdgeSubscriptions, CannedRoles_Enterprise.Edge_Subscriptions.Cmdlets),
			new RoleDefinition("Exchange Connectors", RoleType.ExchangeConnectors, CannedRoles_Enterprise.Exchange_Connectors.Cmdlets),
			new RoleDefinition("Exchange Server Certificates", RoleType.ExchangeServerCertificates, CannedRoles_Enterprise.Exchange_Server_Certificates.Cmdlets),
			new RoleDefinition("Exchange Servers", RoleType.ExchangeServers, CannedRoles_Enterprise.Exchange_Servers.Cmdlets),
			new RoleDefinition("Exchange Virtual Directories", RoleType.ExchangeVirtualDirectories, CannedRoles_Enterprise.Exchange_Virtual_Directories.Cmdlets),
			new RoleDefinition("ExchangeCrossServiceIntegration", RoleType.ExchangeCrossServiceIntegration, CannedRoles_Enterprise.ExchangeCrossServiceIntegration.Cmdlets),
			new RoleDefinition("Federated Sharing", RoleType.FederatedSharing, CannedRoles_Enterprise.Federated_Sharing.Cmdlets),
			new RoleDefinition("Information Rights Management", RoleType.InformationRightsManagement, CannedRoles_Enterprise.Information_Rights_Management.Cmdlets),
			new RoleDefinition("Journaling", RoleType.Journaling, CannedRoles_Enterprise.Journaling.Cmdlets),
			new RoleDefinition("Legal Hold", RoleType.LegalHold, CannedRoles_Enterprise.Legal_Hold.Cmdlets),
			new RoleDefinition("Mail Enabled Public Folders", RoleType.MailEnabledPublicFolders, CannedRoles_Enterprise.Mail_Enabled_Public_Folders.Cmdlets),
			new RoleDefinition("Mail Recipient Creation", RoleType.MailRecipientCreation, CannedRoles_Enterprise.Mail_Recipient_Creation.Cmdlets),
			new RoleDefinition("Mail Recipients", RoleType.MailRecipients, CannedRoles_Enterprise.Mail_Recipients.Cmdlets),
			new RoleDefinition("Mail Tips", RoleType.MailTips, CannedRoles_Enterprise.Mail_Tips.Cmdlets),
			new RoleDefinition("Mailbox Import Export", RoleType.MailboxImportExport, CannedRoles_Enterprise.Mailbox_Import_Export.Cmdlets),
			new RoleDefinition("Mailbox Search", RoleType.MailboxSearch, CannedRoles_Enterprise.Mailbox_Search.Cmdlets),
			new RoleDefinition("Message Tracking", RoleType.MessageTracking, CannedRoles_Enterprise.Message_Tracking.Cmdlets),
			new RoleDefinition("Migration", RoleType.Migration, CannedRoles_Enterprise.Migration.Cmdlets),
			new RoleDefinition("Monitoring", RoleType.Monitoring, CannedRoles_Enterprise.Monitoring.Cmdlets),
			new RoleDefinition("Move Mailboxes", RoleType.MoveMailboxes, CannedRoles_Enterprise.Move_Mailboxes.Cmdlets),
			new RoleDefinition("My Custom Apps", RoleType.MyCustomApps, CannedRoles_Enterprise.My_Custom_Apps.Cmdlets),
			new RoleDefinition("My Marketplace Apps", RoleType.MyMarketplaceApps, CannedRoles_Enterprise.My_Marketplace_Apps.Cmdlets),
			new RoleDefinition("My ReadWriteMailbox Apps", RoleType.MyReadWriteMailboxApps, CannedRoles_Enterprise.My_ReadWriteMailbox_Apps.Cmdlets),
			new RoleDefinition("MyAddressInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Enterprise.MyAddressInformation.Cmdlets),
			new RoleDefinition("MyBaseOptions", RoleType.MyBaseOptions, CannedRoles_Enterprise.MyBaseOptions.Cmdlets),
			new RoleDefinition("MyContactInformation", RoleType.MyContactInformation, CannedRoles_Enterprise.MyContactInformation.Cmdlets),
			new RoleDefinition("MyDiagnostics", RoleType.MyDiagnostics, CannedRoles_Enterprise.MyDiagnostics.Cmdlets),
			new RoleDefinition("MyDisplayName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Enterprise.MyDisplayName.Cmdlets),
			new RoleDefinition("MyDistributionGroupMembership", RoleType.MyDistributionGroupMembership, CannedRoles_Enterprise.MyDistributionGroupMembership.Cmdlets),
			new RoleDefinition("MyDistributionGroups", RoleType.MyDistributionGroups, CannedRoles_Enterprise.MyDistributionGroups.Cmdlets),
			new RoleDefinition("MyMailboxDelegation", RoleType.MyMailboxDelegation, CannedRoles_Enterprise.MyMailboxDelegation.Cmdlets),
			new RoleDefinition("MyMobileInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Enterprise.MyMobileInformation.Cmdlets),
			new RoleDefinition("MyName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Enterprise.MyName.Cmdlets),
			new RoleDefinition("MyPersonalInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Enterprise.MyPersonalInformation.Cmdlets),
			new RoleDefinition("MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Enterprise.MyProfileInformation.Cmdlets),
			new RoleDefinition("MyRetentionPolicies", RoleType.MyRetentionPolicies, CannedRoles_Enterprise.MyRetentionPolicies.Cmdlets),
			new RoleDefinition("MyTeamMailboxes", RoleType.MyTeamMailboxes, CannedRoles_Enterprise.MyTeamMailboxes.Cmdlets),
			new RoleDefinition("MyTextMessaging", RoleType.MyTextMessaging, CannedRoles_Enterprise.MyTextMessaging.Cmdlets),
			new RoleDefinition("MyVoiceMail", RoleType.MyVoiceMail, CannedRoles_Enterprise.MyVoiceMail.Cmdlets),
			new RoleDefinition("Org Custom Apps", RoleType.OrgCustomApps, CannedRoles_Enterprise.Org_Custom_Apps.Cmdlets),
			new RoleDefinition("Org Marketplace Apps", RoleType.OrgMarketplaceApps, CannedRoles_Enterprise.Org_Marketplace_Apps.Cmdlets),
			new RoleDefinition("Organization Client Access", RoleType.OrganizationClientAccess, CannedRoles_Enterprise.Organization_Client_Access.Cmdlets),
			new RoleDefinition("Organization Configuration", RoleType.OrganizationConfiguration, CannedRoles_Enterprise.Organization_Configuration.Cmdlets),
			new RoleDefinition("Organization Transport Settings", RoleType.OrganizationTransportSettings, CannedRoles_Enterprise.Organization_Transport_Settings.Cmdlets),
			new RoleDefinition("POP3 And IMAP4 Protocols", RoleType.POP3AndIMAP4Protocols, CannedRoles_Enterprise.POP3_And_IMAP4_Protocols.Cmdlets),
			new RoleDefinition("Public Folders", RoleType.PublicFolders, CannedRoles_Enterprise.Public_Folders.Cmdlets),
			new RoleDefinition("Receive Connectors", RoleType.ReceiveConnectors, CannedRoles_Enterprise.Receive_Connectors.Cmdlets),
			new RoleDefinition("Recipient Policies", RoleType.RecipientPolicies, CannedRoles_Enterprise.Recipient_Policies.Cmdlets),
			new RoleDefinition("Remote and Accepted Domains", RoleType.RemoteAndAcceptedDomains, CannedRoles_Enterprise.Remote_and_Accepted_Domains.Cmdlets),
			new RoleDefinition("Reset Password", RoleType.ResetPassword, CannedRoles_Enterprise.Reset_Password.Cmdlets),
			new RoleDefinition("Retention Management", RoleType.RetentionManagement, CannedRoles_Enterprise.Retention_Management.Cmdlets),
			new RoleDefinition("Role Management", RoleType.RoleManagement, CannedRoles_Enterprise.Role_Management.Cmdlets),
			new RoleDefinition("Security Group Creation and Membership", RoleType.SecurityGroupCreationAndMembership, CannedRoles_Enterprise.Security_Group_Creation_and_Membership.Cmdlets),
			new RoleDefinition("Send Connectors", RoleType.SendConnectors, CannedRoles_Enterprise.Send_Connectors.Cmdlets),
			new RoleDefinition("Support Diagnostics", RoleType.SupportDiagnostics, CannedRoles_Enterprise.Support_Diagnostics.Cmdlets),
			new RoleDefinition("Team Mailboxes", RoleType.TeamMailboxes, CannedRoles_Enterprise.Team_Mailboxes.Cmdlets),
			new RoleDefinition("Transport Agents", RoleType.TransportAgents, CannedRoles_Enterprise.Transport_Agents.Cmdlets),
			new RoleDefinition("Transport Hygiene", RoleType.TransportHygiene, CannedRoles_Enterprise.Transport_Hygiene.Cmdlets),
			new RoleDefinition("Transport Queues", RoleType.TransportQueues, CannedRoles_Enterprise.Transport_Queues.Cmdlets),
			new RoleDefinition("Transport Rules", RoleType.TransportRules, CannedRoles_Enterprise.Transport_Rules.Cmdlets),
			new RoleDefinition("UM Mailboxes", RoleType.UMMailboxes, CannedRoles_Enterprise.UM_Mailboxes.Cmdlets),
			new RoleDefinition("UM Prompts", RoleType.UMPrompts, CannedRoles_Enterprise.UM_Prompts.Cmdlets),
			new RoleDefinition("UnScoped Role Management", RoleType.UnScopedRoleManagement, CannedRoles_Enterprise.UnScoped_Role_Management.Cmdlets),
			new RoleDefinition("Unified Messaging", RoleType.UnifiedMessaging, CannedRoles_Enterprise.Unified_Messaging.Cmdlets),
			new RoleDefinition("User Options", RoleType.UserOptions, CannedRoles_Enterprise.User_Options.Cmdlets),
			new RoleDefinition("View-Only Audit Logs", RoleType.ViewOnlyAuditLogs, CannedRoles_Enterprise.View_Only_Audit_Logs.Cmdlets),
			new RoleDefinition("View-Only Configuration", RoleType.ViewOnlyConfiguration, CannedRoles_Enterprise.View_Only_Configuration.Cmdlets),
			new RoleDefinition("View-Only Recipients", RoleType.ViewOnlyRecipients, CannedRoles_Enterprise.View_Only_Recipients.Cmdlets),
			new RoleDefinition("WorkloadManagement", RoleType.WorkloadManagement, CannedRoles_Enterprise.WorkloadManagement.Cmdlets)
		};

		private class Active_Directory_Permissions
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ADPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,ChildObjectTypes,Confirm,Debug,Deny,DomainController,ErrorAction,ErrorVariable,ExtendedRights,Identity,InheritanceType,InheritedObjectType,Instance,OutBuffer,OutVariable,Owner,Properties,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,User,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,ShowPartnerLinked,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ADPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,ChildObjectTypes,Confirm,Debug,Deny,DomainController,ErrorAction,ErrorVariable,ExtendedRights,Identity,InheritanceType,InheritedObjectType,Instance,OutBuffer,OutVariable,Properties,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Address_Lists
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-AddressListPaging", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-AddressListPaging", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"AddressListsEnabled",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Container,SearchText"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "DefaultOnly,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Move-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Target,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Move-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,GlobalAddressList,Name,OfflineAddressBook,OutBuffer,OutVariable,RoomList,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,Container,DisplayName,ErrorAction,ErrorVariable,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,RecipientContainer,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,ErrorAction,ErrorVariable,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,RecipientContainer,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,DiffRetentionPeriod,ErrorAction,ErrorVariable,IsDefault,Name,OutBuffer,OutVariable,Versions,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,GeneratingMailbox,GlobalWebDistributionEnabled,PublicFolderDatabase,PublicFolderDistributionEnabled,Schedule,Server,ShadowMailboxDistributionEnabled,SkipPublicFolderInitialization,Verbose,VirtualDirectories")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recursive,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Restore-DetailsTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,ErrorAction,ErrorVariable,GlobalAddressList,Identity,Name,OfflineAddressBook,OutBuffer,OutVariable,RoomList,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,DisplayName,ErrorAction,ErrorVariable,Identity,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ForceUpgrade,RecipientContainer,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DetailsTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Pages,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,ErrorAction,ErrorVariable,Identity,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ForceUpgrade,RecipientContainer,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,ApplyMandatoryProperties,ConfiguredAttributes,Confirm,DiffRetentionPeriod,ErrorAction,ErrorVariable,Identity,IsDefault,Name,OutBuffer,OutVariable,Schedule,UseDefaultAttributes,Versions,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,GeneratingMailbox,GlobalWebDistributionEnabled,MaxBinaryPropertySize,MaxMultivaluedBinaryPropertySize,MaxMultivaluedStringPropertySize,MaxStringPropertySize,ShadowMailboxDistributionEnabled,Verbose,VirtualDirectories")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressListsEnabled",
						"EOPPremiumRestrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class ApplicationImpersonation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Impersonate-ExchangeUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "")
				}, "a")
			};
		}

		private class Audit_Logs
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAuditBypassAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,LogonTypes,Mailboxes,Name,Operations,OutBuffer,OutVariable,ShowDetails,StartDate,StatusMailRecipients,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MailboxAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,LogonTypes,Mailboxes,Operations,OutBuffer,OutVariable,ResultSize,ShowDetails,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AdminAuditLogAgeLimit,AdminAuditLogCmdlets,AdminAuditLogEnabled,AdminAuditLogExcludedCmdlets,AdminAuditLogParameters,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,LogLevel,Name,OutBuffer,OutVariable,TestCmdletLoggingEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuditAdmin,AuditDelegate,AuditEnabled,AuditLogAgeLimit,AuditOwner,Force"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "DefaultPublicFolderMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxAuditBypassAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuditBypassEnabled,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Cmdlet_Extension_Agents
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Assembly,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Assembly,ClassFactory,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Priority,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Priority,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Data_Loss_Prevention
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mode,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-DlpPolicyCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Action,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Locale,Original,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions"
					}, "DlpPolicy"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-DlpPolicyCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-CompliancePolicySyncNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Force,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Fingerprints,Locale,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Mode,Name,OutBuffer,OutVariable,Parameters,State,Template,TemplateData,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Fingerprint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,Description,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Value,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions"
					}, "DlpPolicy,ExceptIfMessageContainsDataClassifications,MessageContainsDataClassifications,NotifySender"),
					new RoleParameters(new string[]
					{
						"IRMPremiumFeaturesPermissions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyRightsProtectionTemplate"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,Debug,DeleteMessage,Disconnect,DomainController,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,Verbose,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-CompliancePolicySyncNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Force,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Fingerprints,Identity,IsDefault,Locale,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Identity,Mode,Name,OutBuffer,OutVariable,State,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Value,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions"
					}, "DlpPolicy,ExceptIfMessageContainsDataClassifications,MessageContainsDataClassifications,NotifySender"),
					new RoleParameters(new string[]
					{
						"IRMPremiumFeaturesPermissions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyRightsProtectionTemplate,Identity"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,Debug,DeleteMessage,Disconnect,DomainController,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,Verbose,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Test-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,WhatIf"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "TransportRules")
				}, "c")
			};
		}

		private class DataCenter_Operations
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Component,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,MaxVersion,MinVersion,Name,OutBuffer,OutVariable,Parameters,Reason,Section,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MaxVersion,MinVersion,OutBuffer,OutVariable,Parameters,Reason,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Database_Availability_Groups
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DatabaseAvailabilityGroupServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,SkipDagValidation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroupConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroupNetwork", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DagConfiguration,DatabaseAvailabilityGroupIpAddresses,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,ThirdPartyReplication,Verbose,WarningAction,WarningVariable,WhatIf,WitnessDirectory,WitnessServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DatabaseAvailabilityGroupConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,CopiesPerDatabase,DatabasesPerServer,DatabasesPerVolume,Debug,ErrorAction,ErrorVariable,MinCopiesPerDatabaseForMonitoring,Name,OutBuffer,OutVariable,ServersPerDag,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DatabaseAvailabilityGroupNetwork", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DatabaseAvailabilityGroup,Debug,Description,DomainController,ErrorAction,ErrorVariable,IgnoreNetwork,Name,OutBuffer,OutVariable,ReplicationEnabled,Subnets,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DatabaseAvailabilityGroupConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DatabaseAvailabilityGroupNetwork", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DatabaseAvailabilityGroupServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,SkipDagValidation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowCrossSiteRpcClientAccess,AlternateWitnessDirectory,AlternateWitnessServer,AutoDagAllServersInstalled,AutoDagAutoReseedEnabled,AutoDagBitlockerEnabled,AutoDagDatabaseCopiesPerDatabase,AutoDagDatabaseCopiesPerVolume,AutoDagDatabasesRootFolderPath,AutoDagDiskReclaimerEnabled,AutoDagFailedVolumesRootFolderPath,AutoDagTotalNumberOfDatabases,AutoDagTotalNumberOfServers,AutoDagVolumesRootFolderPath,Confirm,DagConfiguration,DatabaseAvailabilityGroupIpAddresses,DatacenterActivationMode,Debug,DiscoverNetworks,DomainController,ErrorAction,ErrorVariable,ManualDagNetworkConfiguration,NetworkCompression,NetworkEncryption,OutBuffer,OutVariable,ReplayLagManagerEnabled,ReplicationPort,SkipDagValidation,Verbose,WarningAction,WarningVariable,WhatIf,WitnessDirectory,WitnessServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DatabaseAvailabilityGroupConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,CopiesPerDatabase,DatabasesPerServer,DatabasesPerVolume,Debug,ErrorAction,ErrorVariable,MinCopiesPerDatabaseForMonitoring,OutBuffer,OutVariable,ServersPerDag,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DatabaseAvailabilityGroupNetwork", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,IgnoreNetwork,Name,OutBuffer,OutVariable,ReplicationEnabled,Subnets,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Database_Copies
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivationPreference,ConfigurationOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,ReplayLagTime,SeedingPostponed,TruncationLagTime,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabaseCopyStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Active,ConnectionStatus,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedErrorInfo,Identity,Local,OutBuffer,OutVariable,Server,UseServerCache,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DisableReplayLag,DisableReplayLagReason,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReplicationOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivationPreference,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReplayLagTime,TruncationLagTime,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivationOnly,Confirm,Debug,DomainController,EnableReplayLag,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ForceUpgrade,OnPremisesCredentials,OutBuffer,OutVariable,SuppressOAuthWarning,TenantCredentials,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions"
					}, "DeleteExistingFiles"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BeginSeed,CancelSeed,CatalogOnly,Confirm,DatabaseOnly,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,ManualResume,MaximumSeedsInParallel,Network,NetworkCompressionOverride,NetworkEncryptionOverride,OutBuffer,OutVariable,SafeDeleteExistingFiles,SourceServer,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Databases
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ResubmitRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,CorrelationId,Debug,Destination,EndTime,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recipient,Sender,Server,StartTime,TestOnly,UnresponsivePrimaryServers,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MailboxQuarantine", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Dismount-Database", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MailboxQuarantine", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowMigration,Confirm,Debug,Duration,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Database,Debug,Detailed,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,StoreMailbox,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResubmitRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Mount-Database", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptDataLoss,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Move-ActiveMailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions"
					}, "MountDialOverride,SkipHealthChecks"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivateOnServer,ActivatePreferredOnServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MoveComment,OutBuffer,OutVariable,Server,SkipActiveCopyChecks,SkipClientExperienceChecks,SkipLagChecks,SkipMaximumActiveDatabasesChecks,TerminateOnWarning,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Move-DatabasePath", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationOnly,Confirm,Debug,DomainController,EdbFilePath,ErrorAction,ErrorVariable,Force,Identity,LogFolderPath,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDagExcludeFromMonitoring,Confirm,Debug,DomainController,EdbFilePath,ErrorAction,ErrorVariable,IsExcludedFromInitialProvisioning,IsExcludedFromProvisioning,IsSuspendedFromProvisioning,LogFolderPath,Name,OfflineAddressBook,OutBuffer,OutVariable,PublicFolderDatabase,Recovery,Server,SkipDatabaseLogFolderCreation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,CorruptionType,Database,Debug,DetectOnly,DomainController,ErrorAction,ErrorVariable,Force,Mailbox,OutBuffer,OutVariable,StoreMailbox,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ResubmitRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-StoreMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Database,Debug,ErrorAction,ErrorVariable,Identity,MailboxState,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowFileRestore,AutoDagExcludeFromMonitoring,BackgroundDatabaseMaintenance,CalendarLoggingQuota,CircularLoggingEnabled,Confirm,DataMoveReplicationConstraint,DatabaseGroup,Debug,DeletedItemRetention,DomainController,ErrorAction,ErrorVariable,EventHistoryRetentionPeriod,IndexEnabled,IsExcludedFromInitialProvisioning,IsExcludedFromProvisioning,IsSuspendedFromProvisioning,IssueWarningQuota,JournalRecipient,MailboxRetention,MaintenanceSchedule,MountAtStartup,Name,OfflineAddressBook,OutBuffer,OutVariable,ProhibitSendQuota,ProhibitSendReceiveQuota,PublicFolderDatabase,QuotaNotificationSchedule,RecoverableItemsQuota,RecoverableItemsWarningQuota,RetainDeletedItemsUntilBackup,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ResubmitRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-DatabaseSchema", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,MajorVersion,MinorVersion,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Server")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-StoreMailboxState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Database,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Disaster_Recovery
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,SourceDatabase,Status,Suspend,TargetMailbox,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRestoreRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDagExcludeFromMonitoring,Confirm,Debug,DomainController,EdbFilePath,ErrorAction,ErrorVariable,IsExcludedFromInitialProvisioning,IsExcludedFromProvisioning,IsSuspendedFromProvisioning,LogFolderPath,Name,OfflineAddressBook,OutBuffer,OutVariable,PublicFolderDatabase,Recovery,Server,SkipDatabaseLogFolderCreation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AllowLegacyDNMismatch,AssociatedMessagesCopyOption,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ConflictResolutionOption,Debug,DomainController,ErrorAction,ErrorVariable,ExcludeDumpster,ExcludeFolders,IncludeFolders,InternalFlags,LargeItemLimit,Name,OutBuffer,OutVariable,Priority,SkipMerging,SourceDatabase,SourceRootFolder,SourceStoreMailbox,Suspend,SuspendComment,TargetIsArchive,TargetMailbox,TargetRootFolder,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Restore-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectorySite,AlternateWitnessDirectory,AlternateWitnessServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UsePrimaryWitnessServer,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions"
					}, "Database"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ArchiveDatabase,Confirm,DomainController,IgnoreDefaultScope,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IsExcludedFromInitialProvisioning")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Priority,RehomeRequest,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectorySite,ConfigurationOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectorySite,ConfigurationOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxRestoreRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Distribution_Groups
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Alias,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PrimarySmtpAddress,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,SamAccountName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox,Debug,DomainController,IgnoreNamingPolicy,MemberDepartRestriction,OrganizationalUnit,RoomList,Verbose"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassNestedModerationEnabled"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,ArbitrationMailbox,ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,IncludedRecipients,Name,OrganizationalUnit,OutBuffer,OutVariable,PrimarySmtpAddress,RecipientContainer,RecipientFilter,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Debug,DisplayName,DomainController,EmailAddressPolicyEnabled,EmailAddresses,ErrorAction,ErrorVariable,ExpansionServer,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,IgnoreDefaultScope,IgnoreNamingPolicy,ManagedBy,MaxReceiveSize,MaxSendSize,MemberDepartRestriction,MemberJoinRestriction,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,RoomList,SamAccountName,SendOofMessageToOriginatorEnabled,SimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ForceUpgrade"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "BypassNestedModerationEnabled"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "BypassModerationFromSendersOrMembers")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PrimarySmtpAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Debug,DisplayName,DomainController,EmailAddressPolicyEnabled,EmailAddresses,ExpansionServer,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ForceUpgrade,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreDefaultScope,IncludedRecipients,ManagedBy,MaxReceiveSize,MaxSendSize,Name,Notes,PhoneticDisplayName,RecipientContainer,RecipientFilter,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,SendOofMessageToOriginatorEnabled,SimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"ResourceMailboxRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity,IsHierarchicalGroup,ManagedBy,Name,Notes,OutBuffer,OutVariable,PhoneticDisplayName,SeniorityIndex,SimpleDisplayName,Universal,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IgnoreDefaultScope,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DistributionGroupDefaultOU,DistributionGroupNameBlockedWordsList,DistributionGroupNamingPolicy,HierarchicalAddressBookRoot")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class E_Mail_Address_Policies
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeMailboxSettingOnlyPolicy,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,Debug,DisabledEmailAddressTemplates,DomainController,EnabledEmailAddressTemplates,EnabledPrimarySMTPAddressTemplate,ErrorAction,ErrorVariable,IncludedRecipients,Name,OutBuffer,OutVariable,Priority,RecipientContainer,RecipientFilter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,Debug,DisabledEmailAddressTemplates,DomainController,EnabledEmailAddressTemplates,EnabledPrimarySMTPAddressTemplate,ErrorAction,ErrorVariable,ForceUpgrade,IncludedRecipients,Name,OutBuffer,OutVariable,Priority,RecipientContainer,RecipientFilter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FixMissingAlias,Identity,OutBuffer,OutVariable,UpdateSecondaryAddressesOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Edge_Subscriptions
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADSite", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EdgeSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-EdgeSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccountExpiryDuration,Confirm,CreateInboundSendConnector,CreateInternetSendConnector,Debug,DomainController,ErrorAction,ErrorVariable,FileData,FileName,Force,OutBuffer,OutVariable,Site,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-EdgeSyncServiceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationSyncInterval,Confirm,CookieValidDuration,Debug,DomainController,ErrorAction,ErrorVariable,FailoverDCInterval,LockDuration,LockRenewalDuration,LogEnabled,LogLevel,LogMaxAge,LogMaxDirectorySize,LogMaxFileSize,LogPath,OptionDuration,OutBuffer,OutVariable,RecipientSyncInterval,Site,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-EdgeSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-EdgeSyncServiceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationSyncInterval,Confirm,CookieValidDuration,Debug,DomainController,ErrorAction,ErrorVariable,FailoverDCInterval,LockDuration,LockRenewalDuration,LogEnabled,LogLevel,LogMaxAge,LogMaxDirectorySize,LogMaxFileSize,LogPath,Name,OptionDuration,OutBuffer,OutVariable,RecipientSyncInterval,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-EdgeSynchronization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,ForceFullSync,ForceUpdateCookie,OutBuffer,OutVariable,Server,TargetServer,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Exchange_Connectors
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeliveryAgentConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeliveryAgentConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddressSpaces,Comment,Confirm,Debug,DeliveryProtocol,DomainController,Enabled,ErrorAction,ErrorVariable,IsScopedConnector,MaxConcurrentConnections,MaxMessageSize,MaxMessagesPerConnection,Name,OutBuffer,OutVariable,SourceTransportServers,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeliveryAgentConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeliveryAgentConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddressSpaces,Comment,Confirm,Debug,DeliveryProtocol,DomainController,Enabled,ErrorAction,ErrorVariable,Force,IsScopedConnector,MaxConcurrentConnections,MaxMessageSize,MaxMessagesPerConnection,Name,OutBuffer,OutVariable,SourceTransportServers,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Exchange_Server_Certificates
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DoNotRequireSsl,DomainController,ErrorAction,ErrorVariable,Force,Identity,NetworkServiceAllowed,OutBuffer,OutVariable,Server,Services,Thumbprint,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BinaryEncoded,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileName,Identity,OutBuffer,OutVariable,Password,Server,Thumbprint,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Identity,Instance,OutBuffer,OutVariable,Server,Thumbprint,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,Settings,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,FileName,FriendlyName,Instance,OutBuffer,OutVariable,Password,PrivateKeyExportable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BinaryEncoded,Confirm,Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Force,FriendlyName,GenerateRequest,IncludeAcceptedDomains,IncludeAutoDiscover,IncludeServerFQDN,IncludeServerNetBIOSName,Instance,KeySize,OutBuffer,OutVariable,PrivateKeyExportable,RequestFile,Server,Services,SubjectKeyIdentifier,SubjectName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Thumbprint,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,ProcessType,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Exchange_Servers
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DatabaseAvailabilityGroupServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,SkipDagValidation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ApplyVersion,Confirm,Debug,Duration,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,PropertyValue,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Immediate,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultAuthenticationMethod,DomainController,ErrorAction,ErrorVariable,ExternalClientAuthenticationMethod,ExternalClientsRequireSsl,ExternalHostname,IISAuthenticationMethods,InternalClientAuthenticationMethod,InternalClientsRequireSsl,InternalHostname,OutBuffer,OutVariable,SSLOffloading,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeAlternateServiceAccountCredentialStatus,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ContentFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EventLogLevel", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IntraOrgConnectorProtocolLoggingLevel,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilteringServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MonitoringItemHelp", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MonitoringItemIdentity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-NetworkConnectionInfo", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Invoke-MonitoringProbe", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Account,Debug,Endpoint,ErrorAction,ErrorVariable,Identity,ItemTargetExtension,OutBuffer,OutVariable,Password,PropertyOverride,SecondaryAccount,SecondaryEndpoint,SecondaryPassword,Server,TimeOutSeconds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IISAuthenticationMethods")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ContentFilterPhrase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Phrase,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DatabaseAvailabilityGroupServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxServer,OutBuffer,OutVariable,SkipDagValidation,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClientAccessServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AlternateServiceAccountCredential,Array,AutoDiscoverServiceInternalUri,AutoDiscoverSiteScope,CleanUpInvalidAlternateServiceAccountCredentials,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,RemoveAlternateServiceAccountCredentials,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ContentFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassedRecipients,BypassedSenderDomains,BypassedSenders,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,OutlookEmailPostmarkValidationEnabled,QuarantineMailbox,RejectionResponse,SCLDeleteEnabled,SCLDeleteThreshold,SCLQuarantineEnabled,SCLQuarantineThreshold,SCLRejectEnabled,SCLRejectThreshold,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-EventLogLevel", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Level,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,CustomerFeedbackEnabled,Debug,DomainController,ErrorAction,ErrorReportingEnabled,ErrorVariable,InternetWebProxy,MonitoringGroup,OutBuffer,OutVariable,ProductKey,StaticConfigDomainController,StaticDomainControllers,StaticExcludedDomainControllers,StaticGlobalCatalogs,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgentLogEnabled,AgentLogMaxAge,AgentLogMaxDirectorySize,AgentLogMaxFileSize,AgentLogPath,AntispamAgentsEnabled,Confirm,ConnectivityLogEnabled,ConnectivityLogMaxAge,ConnectivityLogMaxDirectorySize,ConnectivityLogMaxFileSize,ConnectivityLogPath,Debug,DnsLogEnabled,DnsLogMaxAge,DnsLogMaxDirectorySize,DnsLogMaxFileSize,DnsLogPath,DomainController,ErrorAction,ErrorVariable,ExternalDNSAdapterEnabled,ExternalDNSAdapterGuid,ExternalDNSProtocolOption,ExternalDNSServers,ExternalIPAddress,InternalDNSAdapterEnabled,InternalDNSAdapterGuid,InternalDNSProtocolOption,InternalDNSServers,IntraOrgConnectorProtocolLoggingLevel,MaxConnectionRatePerMinute,OutBuffer,OutVariable,ReceiveProtocolLogMaxAge,ReceiveProtocolLogMaxDirectorySize,ReceiveProtocolLogMaxFileSize,ReceiveProtocolLogPath,SendProtocolLogMaxAge,SendProtocolLogMaxDirectorySize,SendProtocolLogMaxFileSize,SendProtocolLogPath,ServerState,TransientFailureRetryCount,TransientFailureRetryInterval,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDatabaseMountDial")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxDatabaseCopy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DatabaseCopyAutoActivationPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDagServerConfigured,AutoDatabaseMountDial,CalendarRepairIntervalEndWindow,CalendarRepairLogDirectorySizeLimit,CalendarRepairLogEnabled,CalendarRepairLogFileAgeLimit,CalendarRepairLogPath,CalendarRepairLogSubjectLoggingEnabled,CalendarRepairMissingItemFixDisabled,CalendarRepairMode,CalendarRepairWorkCycle,CalendarRepairWorkCycleCheckpoint,Confirm,DarTaskStoreTimeBasedAssistantWorkCycle,DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint,DatabaseCopyActivationDisabledAndMoveNow,DatabaseCopyAutoActivationPolicy,Debug,DirectoryProcessorWorkCycle,DirectoryProcessorWorkCycleCheckpoint,DomainController,ErrorAction,ErrorVariable,FaultZone,FolderLogForManagedFoldersEnabled,ForceGroupMetricsGeneration,GroupMailboxWorkCycle,GroupMailboxWorkCycleCheckpoint,InferenceDataCollectionWorkCycle,InferenceDataCollectionWorkCycleCheckpoint,InferenceTrainingWorkCycle,InferenceTrainingWorkCycleCheckpoint,IsExcludedFromProvisioning,JournalingLogForManagedFoldersEnabled,JunkEmailOptionsCommitterWorkCycle,Locale,LogDirectorySizeLimitForManagedFolders,LogFileAgeLimitForManagedFolders,LogFileSizeLimitForManagedFolders,LogPathForManagedFolders,MAPIEncryptionRequired,MailboxAssociationReplicationWorkCycle,MailboxAssociationReplicationWorkCycleCheckpoint,MailboxProcessorWorkCycle,MailboxProcessorWorkCycleCheckpoint,ManagedFolderAssistantSchedule,ManagedFolderWorkCycle,ManagedFolderWorkCycleCheckpoint,MaximumActiveDatabases,MaximumPreferredActiveDatabases,MigrationLogFilePath,MigrationLogLoggingLevel,MigrationLogMaxAge,MigrationLogMaxDirectorySize,MigrationLogMaxFileSize,OABGeneratorWorkCycle,OABGeneratorWorkCycleCheckpoint,OutBuffer,OutVariable,PeopleCentricTriageWorkCycle,PeopleCentricTriageWorkCycleCheckpoint,PeopleRelevanceWorkCycle,PeopleRelevanceWorkCycleCheckpoint,ProbeTimeBasedAssistantWorkCycle,ProbeTimeBasedAssistantWorkCycleCheckpoint,PublicFolderWorkCycle,PublicFolderWorkCycleCheckpoint,RetentionLogForManagedFoldersEnabled,SearchIndexRepairTimeBasedAssistantWorkCycle,SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint,SharePointSignalStoreWorkCycle,SharePointSignalStoreWorkCycleCheckpoint,SharingPolicySchedule,SharingPolicyWorkCycle,SharingPolicyWorkCycleCheckpoint,SharingSyncWorkCycle,SharingSyncWorkCycleCheckpoint,SiteMailboxWorkCycle,SiteMailboxWorkCycleCheckpoint,StoreDsMaintenanceWorkCycle,StoreDsMaintenanceWorkCycleCheckpoint,StoreIntegrityCheckWorkCycle,StoreIntegrityCheckWorkCycleCheckpoint,StoreMaintenanceWorkCycle,StoreMaintenanceWorkCycleCheckpoint,StoreScheduledIntegrityCheckWorkCycle,StoreScheduledIntegrityCheckWorkCycleCheckpoint,StoreUrgentMaintenanceWorkCycle,StoreUrgentMaintenanceWorkCycleCheckpoint,SubjectLogForManagedFoldersEnabled,SubmissionServerOverrideList,TopNWorkCycle,TopNWorkCycleCheckpoint,UMReportingWorkCycle,UMReportingWorkCycleCheckpoint,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ConnectivityLogEnabled,ConnectivityLogMaxAge,ConnectivityLogMaxDirectorySize,ConnectivityLogMaxFileSize,ConnectivityLogPath,ContentConversionTracingEnabled,Debug,DomainController,ErrorAction,ErrorVariable,MailboxDeliveryAgentLogEnabled,MailboxDeliveryAgentLogMaxAge,MailboxDeliveryAgentLogMaxDirectorySize,MailboxDeliveryAgentLogMaxFileSize,MailboxDeliveryAgentLogPath,MailboxDeliveryConnectorProtocolLoggingLevel,MailboxDeliveryThrottlingLogEnabled,MailboxDeliveryThrottlingLogMaxAge,MailboxDeliveryThrottlingLogMaxDirectorySize,MailboxDeliveryThrottlingLogMaxFileSize,MailboxDeliveryThrottlingLogPath,MailboxSubmissionAgentLogEnabled,MailboxSubmissionAgentLogMaxAge,MailboxSubmissionAgentLogMaxDirectorySize,MailboxSubmissionAgentLogMaxFileSize,MailboxSubmissionAgentLogPath,MaxConcurrentMailboxDeliveries,MaxConcurrentMailboxSubmissions,OutBuffer,OutVariable,PipelineTracingEnabled,PipelineTracingPath,PipelineTracingSenderAddress,ReceiveProtocolLogMaxAge,ReceiveProtocolLogMaxDirectorySize,ReceiveProtocolLogMaxFileSize,ReceiveProtocolLogPath,SendProtocolLogMaxAge,SendProtocolLogMaxDirectorySize,SendProtocolLogMaxFileSize,SendProtocolLogPath,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilteringServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassFiltering,Confirm,Debug,DeferAttempts,DeferWaitTime,DomainController,ErrorAction,ErrorVariable,ForceRescan,MinimumSuccessfulEngineScans,OutBuffer,OutVariable,PrimaryUpdatePath,ScanErrorAction,ScanTimeout,SecondaryUpdatePath,UpdateFrequency,UpdateTimeout,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IISAuthenticationMethods")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultAuthenticationMethod,DomainController,ErrorAction,ErrorVariable,ExternalClientAuthenticationMethod,ExternalClientsRequireSsl,ExternalHostname,IISAuthenticationMethods,InternalClientAuthenticationMethod,InternalClientsRequireSsl,InternalHostname,Name,OutBuffer,OutVariable,SSLOffloading,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,LocalOnly,OutBuffer,OutVariable,RemoteOnly,Requester,State,TimeoutInSeconds,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ServerMonitor", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Repairing,Server,TargetResource,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveUserStatisticsLogMaxAge,ActiveUserStatisticsLogMaxDirectorySize,ActiveUserStatisticsLogMaxFileSize,ActiveUserStatisticsLogPath,AgentLogEnabled,AgentLogMaxAge,AgentLogMaxDirectorySize,AgentLogMaxFileSize,AgentLogPath,AntispamAgentsEnabled,Confirm,ConnectivityLogEnabled,ConnectivityLogMaxAge,ConnectivityLogMaxDirectorySize,ConnectivityLogMaxFileSize,ConnectivityLogPath,ContentConversionTracingEnabled,Debug,DelayNotificationTimeout,DnsLogEnabled,DnsLogMaxAge,DnsLogMaxDirectorySize,DnsLogMaxFileSize,DnsLogPath,DomainController,ErrorAction,ErrorVariable,ExternalDNSAdapterEnabled,ExternalDNSAdapterGuid,ExternalDNSProtocolOption,ExternalDNSServers,ExternalIPAddress,InternalDNSAdapterEnabled,InternalDNSAdapterGuid,InternalDNSProtocolOption,InternalDNSServers,IntraOrgConnectorProtocolLoggingLevel,IntraOrgConnectorSmtpMaxMessagesPerConnection,IrmLogEnabled,IrmLogMaxAge,IrmLogMaxDirectorySize,IrmLogMaxFileSize,IrmLogPath,MaxConcurrentMailboxDeliveries,MaxConcurrentMailboxSubmissions,MaxConnectionRatePerMinute,MaxOutboundConnections,MaxPerDomainOutboundConnections,MessageExpirationTimeout,MessageRetryInterval,MessageTrackingLogEnabled,MessageTrackingLogMaxAge,MessageTrackingLogMaxDirectorySize,MessageTrackingLogMaxFileSize,MessageTrackingLogPath,MessageTrackingLogSubjectLoggingEnabled,OutBuffer,OutVariable,OutboundConnectionFailureRetryInterval,PickupDirectoryMaxHeaderSize,PickupDirectoryMaxMessagesPerMinute,PickupDirectoryMaxRecipientsPerMessage,PickupDirectoryPath,PipelineTracingEnabled,PipelineTracingPath,PipelineTracingSenderAddress,PoisonMessageDetectionEnabled,PoisonThreshold,QueueLogMaxAge,QueueLogMaxDirectorySize,QueueLogMaxFileSize,QueueLogPath,QueueMaxIdleTime,ReceiveProtocolLogMaxAge,ReceiveProtocolLogMaxDirectorySize,ReceiveProtocolLogMaxFileSize,ReceiveProtocolLogPath,RecipientValidationCacheEnabled,ReplayDirectoryPath,RootDropDirectoryPath,RoutingTableLogMaxAge,RoutingTableLogMaxDirectorySize,RoutingTableLogPath,SendProtocolLogMaxAge,SendProtocolLogMaxDirectorySize,SendProtocolLogMaxFileSize,SendProtocolLogPath,ServerStatisticsLogMaxAge,ServerStatisticsLogMaxDirectorySize,ServerStatisticsLogMaxFileSize,ServerStatisticsLogPath,TransientFailureRetryCount,TransientFailureRetryInterval,UseDowngradedExchangeServerAuth,Verbose,WarningAction,WarningVariable,WhatIf,WlmLogMaxAge,WlmLogMaxDirectorySize,WlmLogMaxFileSize,WlmLogPath")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveUserStatisticsLogMaxAge,ActiveUserStatisticsLogMaxDirectorySize,ActiveUserStatisticsLogMaxFileSize,ActiveUserStatisticsLogPath,AgentLogEnabled,AgentLogMaxAge,AgentLogMaxDirectorySize,AgentLogMaxFileSize,AgentLogPath,AntispamAgentsEnabled,Confirm,ConnectivityLogEnabled,ConnectivityLogMaxAge,ConnectivityLogMaxDirectorySize,ConnectivityLogMaxFileSize,ConnectivityLogPath,ContentConversionTracingEnabled,Debug,DelayNotificationTimeout,DnsLogEnabled,DnsLogMaxAge,DnsLogMaxDirectorySize,DnsLogMaxFileSize,DnsLogPath,DomainController,ErrorAction,ErrorVariable,ExternalDNSAdapterEnabled,ExternalDNSAdapterGuid,ExternalDNSProtocolOption,ExternalDNSServers,ExternalIPAddress,InternalDNSAdapterEnabled,InternalDNSAdapterGuid,InternalDNSProtocolOption,InternalDNSServers,IntraOrgConnectorProtocolLoggingLevel,IntraOrgConnectorSmtpMaxMessagesPerConnection,IrmLogEnabled,IrmLogMaxAge,IrmLogMaxDirectorySize,IrmLogMaxFileSize,IrmLogPath,MaxConcurrentMailboxDeliveries,MaxConcurrentMailboxSubmissions,MaxConnectionRatePerMinute,MaxOutboundConnections,MaxPerDomainOutboundConnections,MessageExpirationTimeout,MessageRetryInterval,MessageTrackingLogEnabled,MessageTrackingLogMaxAge,MessageTrackingLogMaxDirectorySize,MessageTrackingLogMaxFileSize,MessageTrackingLogPath,MessageTrackingLogSubjectLoggingEnabled,OutBuffer,OutVariable,OutboundConnectionFailureRetryInterval,PickupDirectoryMaxHeaderSize,PickupDirectoryMaxMessagesPerMinute,PickupDirectoryMaxRecipientsPerMessage,PickupDirectoryPath,PipelineTracingEnabled,PipelineTracingPath,PipelineTracingSenderAddress,PoisonMessageDetectionEnabled,PoisonThreshold,QueueLogMaxAge,QueueLogMaxDirectorySize,QueueLogMaxFileSize,QueueLogPath,QueueMaxIdleTime,ReceiveProtocolLogMaxAge,ReceiveProtocolLogMaxDirectorySize,ReceiveProtocolLogMaxFileSize,ReceiveProtocolLogPath,RecipientValidationCacheEnabled,ReplayDirectoryPath,ResourceLogEnabled,ResourceLogMaxAge,ResourceLogMaxDirectorySize,ResourceLogMaxFileSize,ResourceLogPath,RootDropDirectoryPath,RoutingTableLogMaxAge,RoutingTableLogMaxDirectorySize,RoutingTableLogPath,SendProtocolLogMaxAge,SendProtocolLogMaxDirectorySize,SendProtocolLogMaxFileSize,SendProtocolLogPath,ServerState,ServerStatisticsLogMaxAge,ServerStatisticsLogMaxDirectorySize,ServerStatisticsLogMaxFileSize,ServerStatisticsLogPath,TransientFailureRetryCount,TransientFailureRetryInterval,TransportMaintenanceLogEnabled,TransportMaintenanceLogMaxAge,TransportMaintenanceLogMaxDirectorySize,TransportMaintenanceLogMaxFileSize,TransportMaintenanceLogPath,TransportSyncAccountsPoisonAccountThreshold,TransportSyncAccountsPoisonDetectionEnabled,TransportSyncAccountsPoisonItemThreshold,TransportSyncAccountsSuccessivePoisonItemThreshold,TransportSyncEnabled,TransportSyncExchangeEnabled,TransportSyncHubHealthLogEnabled,TransportSyncHubHealthLogFilePath,TransportSyncHubHealthLogMaxAge,TransportSyncHubHealthLogMaxDirectorySize,TransportSyncHubHealthLogMaxFileSize,TransportSyncImapEnabled,TransportSyncLogEnabled,TransportSyncLogFilePath,TransportSyncLogLoggingLevel,TransportSyncLogMaxAge,TransportSyncLogMaxDirectorySize,TransportSyncLogMaxFileSize,TransportSyncMaxDownloadItemsPerConnection,TransportSyncMaxDownloadSizePerConnection,TransportSyncMaxDownloadSizePerItem,TransportSyncPopEnabled,TransportSyncRemoteConnectionTimeout,UseDowngradedExchangeServerAuth,Verbose,WarningAction,WarningVariable,WhatIf,WindowsLiveHotmailTransportSyncEnabled,WlmLogMaxAge,WlmLogMaxDirectorySize,WlmLogMaxFileSize,WlmLogPath")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallRouterSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DialPlans,DomainController,ErrorAction,ErrorVariable,IPAddressFamily,IPAddressFamilyConfigurable,MaxCallsAllowed,OutBuffer,OutVariable,Server,SipTcpListeningPort,SipTlsListeningPort,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DialPlans,DomainController,ErrorAction,ErrorVariable,GrammarGenerationSchedule,IPAddressFamily,IPAddressFamilyConfigurable,IrmLogEnabled,IrmLogMaxAge,IrmLogMaxDirectorySize,IrmLogMaxFileSize,IrmLogPath,MaxCallsAllowed,OutBuffer,OutVariable,SIPAccessService,Status,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-ExchangeHelp", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Force,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Exchange_Virtual_Directories
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalClientAuthenticationMethod,ExternalClientsRequireSsl,ExternalHostname,InternalClientAuthenticationMethod,InternalClientsRequireSsl,InternalHostname,Role,SSLOffloading")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-ActiveSyncLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,EndDate,ErrorAction,ErrorVariable,Filename,Force,OutBuffer,OutVariable,OutputPath,OutputPrefix,StartDate,UseGMT,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAutoblockThreshold", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AutodiscoverVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EcpVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ShowMailboxVirtualDirectories")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PowerShellVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-WebServicesVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ActiveSyncVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppPoolId,ApplicationRoot,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalAuthenticationMethods,ExternalUrl,InstallProxySubDirectory,InternalAuthenticationMethods,InternalUrl,OutBuffer,OutVariable,Path,Role,Server,Verbose,WarningAction,WarningVariable,WebSiteName,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AutodiscoverVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppPoolId,ApplicationRoot,BasicAuthentication,Confirm,Debug,DigestAuthentication,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,OAuthAuthentication,OutBuffer,OutVariable,Path,Role,Server,Verbose,WSSecurityAuthentication,WarningAction,WarningVariable,WebSiteName,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-EcpVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppPoolId,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,InternalUrl,OutBuffer,OutVariable,Path,Role,Server,Verbose,WarningAction,WarningVariable,WebSiteName,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,InternalUrl,OutBuffer,OutVariable,Path,Role,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,InternalUrl,OutBuffer,OutVariable,Path,PollInterval,Recovery,RequireSSL,Role,Server,Verbose,WarningAction,WarningVariable,WebSiteName,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppPoolId,ApplicationRoot,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalAuthenticationMethods,ExternalUrl,InternalUrl,Name,OutBuffer,OutVariable,Path,Role,Server,Verbose,WarningAction,WarningVariable,WebSiteName,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PowerShellVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BasicAuthentication,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExternalUrl,InternalUrl,Name,OutBuffer,OutVariable,RequireSSL,Role,Server,Verbose,WarningAction,WarningVariable,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-WebServicesVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppPoolId,AppPoolIdForManagement,ApplicationRoot,BasicAuthentication,Confirm,Debug,DigestAuthentication,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,Force,GzipLevel,InternalNLBBypassUrl,InternalUrl,MRSProxyEnabled,OAuthAuthentication,OutBuffer,OutVariable,Path,Role,Server,Verbose,WSSecurityAuthentication,WarningAction,WarningVariable,WebSiteName,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AutodiscoverVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-EcpVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OwaVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PowerShellVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-WebServicesVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncDeviceAutoblockThreshold", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AdminEmailInsert,BehaviorTypeIncidenceDuration,BehaviorTypeIncidenceLimit,Confirm,Debug,DeviceBlockDuration,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveSyncServer,BadItemReportingEnabled,BasicAuthEnabled,ClientCertAuth,CompressionEnabled,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalAuthenticationMethods,ExternalUrl,InstallIsapiFilter,InternalAuthenticationMethods,InternalUrl,MobileClientCertTemplateName,MobileClientCertificateAuthorityURL,MobileClientCertificateProvisioningEnabled,Name,OutBuffer,OutVariable,RemoteDocumentsActionForUnknownServers,RemoteDocumentsAllowedServers,RemoteDocumentsBlockedServers,RemoteDocumentsInternalDomainSuffixList,SendWatsonReport,Verbose,WarningAction,WarningVariable,WhatIf,WindowsAuthEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AutodiscoverVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BasicAuthentication,Confirm,Debug,DigestAuthentication,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,Identity,OutBuffer,OutVariable,Verbose,WSSecurityAuthentication,WarningAction,WarningVariable,WhatIf,WindowsAuthentication"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "OAuthAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ResetAutoBlockedDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-EcpVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AdfsAuthentication,AdminEnabled,BasicAuthentication,Confirm,Debug,DigestAuthentication,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalAuthenticationMethods,ExternalUrl,FormsAuthentication,GzipLevel,Identity,InternalUrl,OutBuffer,OutVariable,OwaOptionsEnabled,Verbose,WarningAction,WarningVariable,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ApplyDefaults,ExternalUrl,InternalUrl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BasicAuthentication,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,InternalUrl,OAuthAuthentication,OutBuffer,OutVariable,PollInterval,RequireSSL,Verbose,WarningAction,WarningVariable,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalClientsRequireSsl,InternalClientsRequireSsl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OwaVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AdfsAuthentication,AllAddressListsEnabled,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,AnonymousFeaturesEnabled,BasicAuthentication,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,ClientAuthCleanupLevel,Confirm,ContactsEnabled,Debug,DefaultClientLanguage,DefaultDomain,DefaultTheme,DelegateAccessEnabled,DigestAuthentication,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,DomainController,ErrorAction,ErrorVariable,Exchange2003Url,ExchwebProxyDestination,ExplicitLogonEnabled,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalAuthenticationMethods,ExternalUrl,FailbackUrl,FilterWebBeaconsAndHtmlForms,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,FormsAuthentication,GlobalAddressListEnabled,GzipLevel,IRMEnabled,InstantMessagingCertificateThumbprint,InstantMessagingEnabled,InstantMessagingServerName,InstantMessagingType,IntegratedFeaturesEnabled,InternalUrl,IsPublic,JournalEnabled,JunkEmailEnabled,LogonAndErrorLanguage,LogonFormat,LogonPageLightSelectionEnabled,LogonPagePublicPrivateSelectionEnabled,NotesEnabled,NotificationInterval,OAuthAuthentication,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PremiumClientEnabled,PublicFoldersEnabled,RecoverDeletedItemsEnabled,RedirectToOptimalOWAServer,RemindersAndNotificationsEnabled,RemoteDocumentsActionForUnknownServers,RemoteDocumentsAllowedServers,RemoteDocumentsBlockedServers,RemoteDocumentsInternalDomainSuffixList,ReportJunkEmailEnabled,RulesEnabled,SMimeEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UNCAccessOnPrivateComputersEnabled,UNCAccessOnPublicComputersEnabled,UseGB18030,UseISO885915,UserContextTimeout,Verbose,VirtualDirectoryType,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf,WindowsAuthentication"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "AllowCopyContactsToDeviceAddressBook")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PowerShellVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BasicAuthentication,CertificateAuthentication,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExternalUrl,Identity,InternalUrl,OutBuffer,OutVariable,RequireSSL,Verbose,WarningAction,WarningVariable,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-WebServicesVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BasicAuthentication,CertificateAuthentication,Confirm,Debug,DigestAuthentication,DomainController,ErrorAction,ErrorVariable,ExtendedProtectionFlags,ExtendedProtectionSPNList,ExtendedProtectionTokenChecking,ExternalUrl,Force,GzipLevel,Identity,InternalNLBBypassUrl,InternalUrl,MRSProxyEnabled,OAuthAuthentication,OutBuffer,OutVariable,Verbose,WSSecurityAuthentication,WarningAction,WarningVariable,WhatIf,WindowsAuthentication")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class ExchangeCrossServiceIntegration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Organization,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,Organization,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,UsnForReconciliationSearch,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Credential,Debug,DomainController,IgnoreDefaultScope,Organization,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GroupMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccountPartition,Anr,Database,Debug,DomainController,ErrorAction,ErrorVariable,ExecutingUser,Filter,Identity,IgnoreDefaultScope,IncludeInactiveMailbox,IncludeMailboxUrls,IncludeMemberSyncStatus,IncludeMembers,IncludePermissionsVersion,Organization,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Organization,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,UsnForReconciliationSearch,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Anr,ArchiveDatabase,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Organization,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SoftDeletedMailUser,SortBy,UsnForReconciliationSearch,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountPartition,Anr,AuthenticationType,BookmarkDisplayName,Capabilities,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,Organization,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,SamAccountName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox,Debug,DomainController,ExternalDirectoryObjectId,IgnoreNamingPolicy,MemberDepartRestriction,Organization,OrganizationalUnit,OverrideRecipientQuotas,RoomList,Type,Verbose"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassNestedModerationEnabled"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-GroupMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,AutoSubscribeNewGroupMembers,Confirm,Database,Debug,Description,DisplayName,DomainController,EmailAddresses,ErrorAction,ErrorVariable,ExecutingUser,ExternalDirectoryObjectId,FromSyncClient,Language,Members,ModernGroupType,Name,Organization,OrganizationalUnit,OutBuffer,OutVariable,OverrideRecipientQuotas,Owners,PrimarySmtpAddress,PublicToGroups,RecipientIdType,RequireSenderAuthenticationEnabled,SharePointResources,SharePointUrl,ValidationOrganization,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,ArbitrationMailbox,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalDirectoryObjectId,ExternalEmailAddress,FirstName,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,Organization,OrganizationalUnit,OutBuffer,OutVariable,OverrideRecipientQuotas,PrimarySmtpAddress,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,ArbitrationMailbox,BypassLiveId,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalDirectoryObjectId,ExternalEmailAddress,FederatedIdentity,FirstName,ImmutableId,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,NetID,Organization,OrganizationalUnit,OutBuffer,OutVariable,OverrideRecipientQuotas,PrimarySmtpAddress,RemotePowerShellEnabled,SKUAssigned,SKUCapability,SamAccountName,UsageLocation,UsePreferMessageFormat,UserPrincipalName,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"MSOIdPermissions"
					}, "MicrosoftOnlineServicesID"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"NewUserPasswordManagementPermissions"
					}, "Password"),
					new RoleParameters(new string[]
					{
						"NewUserResetPasswordOnNextLogonPermissions"
					}, "ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"UserLiveIdManagementPermissions"
					}, "EvictLiveId,ImportLiveId,UseExistingLiveId,WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ForReconciliation,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-GroupMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExecutingUser,ForReconciliation,Force,FromSyncClient,Identity,OutBuffer,OutVariable,Permanent,RecipientIdType,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ForReconciliation,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ForReconciliation,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreLegalHold,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "Permanent"),
					new RoleParameters(new string[]
					{
						"UserLiveIdManagementPermissions"
					}, "KeepWindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AssistantName,City,Company,CountryOrRegion,Department,DisplayName,Fax,FirstName,GeoCoordinates,HomePhone,Identity,Initials,LastName,Manager,MobilePhone,Name,Notes,Office,OtherFax,OtherHomePhone,OtherTelephone,Pager,Phone,PhoneticDisplayName,PostalCode,SeniorityIndex,StateOrProvince,StreetAddress,TelephoneAssistant,Title,WebPage"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AllowUMCallsFromNonUsers,Confirm,CreateDTMFMap,Debug,IgnoreDefaultScope,OutBuffer,OutVariable,PostOfficeBox,SimpleDisplayName,UMCallingLineIds,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ArbitrationMailbox,BypassSecurityGroupManagerCheck,Confirm,CreateDTMFMap,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Debug,DisplayName,DomainController,EmailAddressPolicyEnabled,EmailAddresses,ErrorAction,ErrorVariable,ExpansionServer,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,IgnoreDefaultScope,IgnoreNamingPolicy,ManagedBy,MaxReceiveSize,MaxSendSize,MemberDepartRestriction,MemberJoinRestriction,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,RoomList,SamAccountName,SendOofMessageToOriginatorEnabled,SimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ForceUpgrade,GenerateExternalDirectoryObjectId"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "BypassNestedModerationEnabled"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "BypassModerationFromSendersOrMembers"),
					new RoleParameters(new string[]
					{
						"ResourceMailboxRestrictions",
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity,IsHierarchicalGroup,ManagedBy,Name,Notes,OutBuffer,OutVariable,PhoneticDisplayName,SeniorityIndex,SimpleDisplayName,Universal,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IgnoreDefaultScope,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-GroupMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AddOwners,AddedMembers,AutoSubscribeNewGroupMembers,ConfigurationActions,Confirm,Debug,Description,DisplayName,DomainController,EmailAddresses,ErrorAction,ErrorVariable,ExecutingUser,ExternalDirectoryObjectId,Force,ForcePublishExternalResources,FromSyncClient,Identity,Language,Name,OutBuffer,OutVariable,Owners,PermissionsVersion,PrimarySmtpAddress,RecipientIdType,RemoveOwners,RemovedMembers,RequireSenderAuthenticationEnabled,SharePointResources,SharePointUrl,SwitchToGroupType,Verbose,WarningAction,WarningVariable,WhatIf,YammerGroupEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,Name,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox,Confirm,CreateDTMFMap,Debug,DomainController,EmailAddressPolicyEnabled,ErrorAction,ErrorVariable,ForceUpgrade,GenerateExternalDirectoryObjectId,IgnoreDefaultScope,MacAttachmentFormat,MaxReceiveSize,MaxRecipientPerMessage,MaxSendSize,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,RemovePicture,RemoveSpokenName,SecondaryAddress,SecondaryDialPlan,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "ModeratedBy,ModerationEnabled"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,ImmutableId,Name,NetID,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AddOnSKUCapability,ArbitrationMailbox,ArchiveGuid,ArchiveName,ArchiveQuota,ArchiveWarningQuota,BypassLiveId,CalendarVersionStoreDisabled,Confirm,CreateDTMFMap,Debug,DomainController,EmailAddressPolicyEnabled,EndDateForRetentionHold,ErrorAction,ErrorVariable,ExchangeGuid,ForceUpgrade,IgnoreDefaultScope,JournalArchiveAddress,LitigationHoldDate,LitigationHoldEnabled,LitigationHoldOwner,MacAttachmentFormat,MaxReceiveSize,MaxSendSize,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,RecipientLimits,RecoverableItemsQuota,RecoverableItemsWarningQuota,RemovePicture,RemoveSpokenName,RetainDeletedItemsFor,RetentionComment,RetentionHoldEnabled,RetentionUrl,SKUAssigned,SKUCapability,SamAccountName,SecondaryAddress,SecondaryDialPlan,SimpleDisplayName,SingleItemRecoveryEnabled,StartDateForRetentionHold,UsageLocation,UseMapiRichTextFormat,UsePreferMessageFormat,UserPrincipalName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MSOIdPermissions"
					}, "MicrosoftOnlineServicesID"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "FederatedIdentity"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap"),
					new RoleParameters(new string[]
					{
						"UserLiveIdManagementPermissions"
					}, "WindowsLiveID")
				}, "c")
			};
		}

		private class Federated_Sharing
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccessMethod,Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,ProxyUrl,TargetAutodiscoverEpr,UseServiceAccount,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-FederatedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedDomainProof", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DomainName,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Thumbprint,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeExtendedDomainInfo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassAdditionalDomainValidation,Debug,DomainName,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TrustedHostnames,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ForeignConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PendingFederatedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AdministratorProvisioningId,ApplicationIdentifier,ApplicationUri,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,MetadataUrl,Name,OutBuffer,OutVariable,SkipNamespaceProviderProvisioning,Thumbprint,UseLegacyProvisioningService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ForeignConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddressSpaces,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IsScopedConnector,Name,OutBuffer,OutVariable,SourceTransportServers,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DiscoveryEndpoint,DomainController,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,TargetAddressDomains,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArchiveAccessEnabled,Confirm,Debug,DomainController,DomainNames,Enabled,ErrorAction,ErrorVariable,FreeBusyAccessEnabled,FreeBusyAccessLevel,FreeBusyAccessScope,MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope,MailboxMoveEnabled,Name,OrganizationContact,OutBuffer,OutVariable,PhotosEnabled,TargetApplicationUri,TargetAutodiscoverEpr,TargetOwaURL,TargetSharingEpr,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"MessageTrackingPermissions"
					}, "DeliveryReportEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Default,DomainController,Domains,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-FederatedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ForeignConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,PerUserAccount,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountNamespace,Confirm,Debug,DefaultDomain,DelegationFederationTrust,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OrganizationContact,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ApplicationUri,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,MetadataUrl,Name,OutBuffer,OutVariable,PublishFederationCertificate,RefreshMetadata,Thumbprint,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ForeignConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddressSpaces,Comment,Confirm,Debug,DomainController,DropDirectory,DropDirectoryQuota,Enabled,ErrorAction,ErrorVariable,Force,IsScopedConnector,MaxMessageSize,Name,OutBuffer,OutVariable,RelayDsnRequired,SourceTransportServers,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DiscoveryEndpoint,DomainController,Enabled,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TargetAddressDomains,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "PublicComputersDetectionEnabled,PublicFolderMailboxesLockedForNewConnections,PublicFolderMailboxesMigrationComplete"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "UMAvailableLanguages")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArchiveAccessEnabled,Confirm,Debug,DomainController,DomainNames,Enabled,ErrorAction,ErrorVariable,Force,FreeBusyAccessEnabled,FreeBusyAccessLevel,FreeBusyAccessScope,MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope,MailboxMoveEnabled,Name,OrganizationContact,OutBuffer,OutVariable,PhotosEnabled,TargetApplicationUri,TargetAutodiscoverEpr,TargetOwaURL,TargetSharingEpr,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"MessageTrackingPermissions"
					}, "DeliveryReportEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PendingFederatedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PendingAccountNamespace,PendingDomains,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Default,DomainController,Domains,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,MonitoringContext,OutBuffer,OutVariable,UserIdentity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-FederationTrustCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UserIdentity,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ForceUpgrade,OnPremisesCredentials,OutBuffer,OutVariable,SuppressOAuthWarning,TenantCredentials,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Information_Rights_Management
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ApplyRightsProtectionTemplate,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Force,FromDepartment,Name,OutBuffer,OutVariable,Priority,SentTo,SentToScope,UserCanOverride,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "JournalReportDecryptionEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ClientAccessServerEnabled,Confirm,Debug,DomainController,EDiscoverySuperUserEnabled,ErrorAction,ErrorVariable,ExternalLicensingEnabled,Force,InternalLicensingEnabled,LicensingLocation,OutBuffer,OutVariable,RefreshServerCertificates,SearchEnabled,TransportDecryptionSetting,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ApplyRightsProtectionTemplate,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,FromDepartment,Identity,Name,OutBuffer,OutVariable,Priority,SentTo,SentToScope,UserCanOverride,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Recipient,Sender,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Journaling
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,JournalEmailAddress,Name,OutBuffer,OutVariable,Recipient,Scope,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,JournalEmailAddress,Name,OutBuffer,OutVariable,Recipient,Scope,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "JournalingReportNdrTo")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Legal_Hold
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "InPlaceHoldIdentity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,ShowDeletionInProgressSearches,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "InPlaceHoldEnabled,InPlaceHoldIdentity,ItemHoldPeriod"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,SourceMailboxes,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ArbitrationMailbox,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,LitigationHoldEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Arbitration,ArbitrationMailbox,ClientExtensions,Confirm,Force,GMGen,Management,MessageTracking,Migration,OABGen,OMEncryption,RetentionPolicy,SingleItemRecoveryEnabled,UMDataStorage,UMGrammar"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ManagedFoldersPermissions"
					}, "RemoveManagedFolderAndPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "InPlaceHoldEnabled,ItemHoldPeriod"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,SourceMailboxes,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "Arbitration,PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mail_Enabled_Public_Folders
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,HiddenFromAddressListsEnabled,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Server,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "Debug,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"PublicFoldersEnabled"
					}, "HoldForMigration,IsExcludedFromServingHierarchy,PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RecipientManagementPermissions"
					}, "Name,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncMailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Alias,Confirm,Debug,DomainController,EmailAddresses,EntryId,ErrorAction,ErrorVariable,ExternalEmailAddress,HiddenFromAddressListsEnabled,Name,OutBuffer,OutVariable,OverrideRecipientQuotas,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PropertiesMasteredOnPremiseRestrictions",
						"PublicFoldersEnabled"
					}, "PrimarySmtpAddress"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,Contacts,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Debug,DeliverToMailboxAndForward,DisplayName,DomainController,EmailAddressPolicyEnabled,EmailAddresses,EntryId,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,ForwardingAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreDefaultScope,MaxReceiveSize,MaxSendSize,Name,OutBuffer,OutVariable,PhoneticDisplayName,PublicFolderType,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,SimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
				}, "c")
			};
		}

		private class Mail_Recipient_Creation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-PushNotificationProxy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-PushNotificationProxy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Organization,OutBuffer,OutVariable,Uri,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,Verbose,WarningAction,WarningVariable,WritableDatabase,WritableRecipient,WritableServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Database,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OnPremisesOrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Explicit,Identity,OutBuffer,OutVariable,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,SortBy,ThrottlingPolicy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Trust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainName,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPrincipalNamesSuffix", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OrganizationalUnit,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,ArbitrationMailbox,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalEmailAddress,FirstName,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,OrganizationalUnit,OutBuffer,OutVariable,PrimarySmtpAddress,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,ArbitrationMailbox,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalEmailAddress,FirstName,ImmutableId,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,OrganizationalUnit,OutBuffer,OutVariable,PrimarySmtpAddress,RemotePowerShellEnabled,SamAccountName,UsePreferMessageFormat,UserPrincipalName,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"NewUserPasswordManagementPermissions"
					}, "Password"),
					new RoleParameters(new string[]
					{
						"NewUserResetPasswordOnNextLogonPermissions"
					}, "ResetPasswordOnNextLogon")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions"
					}, "ActiveSyncMailboxPolicy"),
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "AccountDisabled,AddressBookPolicy,Arbitration,ArbitrationMailbox,ArchiveDatabase,ArchiveDomain,Database,Debug,DomainController,EnableRoomMailboxAccount,LinkedCredential,LinkedDomainController,LinkedMasterAccount,LinkedRoom,RemoteArchive,RetentionPolicy,RoomMailboxPassword,SamAccountName,SharingPolicy,ThrottlingPolicy,UserPrincipalName,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions",
						"PilotingOrganization_Restrictions"
					}, "ModeratedBy,ModerationEnabled,Office,Phone,PrimarySmtpAddress,ResourceCapacity,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"NewUserPasswordManagementPermissions",
						"PilotingOrganization_Restrictions"
					}, "Password"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"NewUserResetPasswordOnNextLogonPermissions",
						"PilotingOrganization_Restrictions"
					}, "ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"PublicFoldersEnabled"
					}, "HoldForMigration,IsExcludedFromServingHierarchy,PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RecipientManagementPermissions"
					}, "Alias,Confirm,Discovery,DisplayName,Equipment,ErrorAction,ErrorVariable,FirstName,Force,ImmutableId,Initials,LastName,Name,OrganizationalUnit,OutBuffer,OutVariable,RemotePowerShellEnabled,Room,Shared,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ACLableSyncedObjectEnabled,AccountDisabled,Alias,Archive,Confirm,Debug,DisplayName,DomainController,Equipment,ErrorAction,ErrorVariable,FirstName,ImmutableId,Initials,LastName,ModeratedBy,ModerationEnabled,Name,OnPremisesOrganizationalUnit,OutBuffer,OutVariable,PrimarySmtpAddress,RemotePowerShellEnabled,RemoteRoutingAddress,Room,SamAccountName,SendModerationNotifications,Shared,UserPrincipalName,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"NewUserPasswordManagementPermissions"
					}, "Password"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"NewUserResetPasswordOnNextLogonPermissions"
					}, "ResetPasswordOnNextLogon")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreLegalHold,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "IgnoreLegalHold"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "Arbitration,Confirm,Database,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Permanent,PublicFolder,RemoveArbitrationMailboxWithOABsAllowed,RemoveLastArbitrationMailboxAllowed,StoreMailboxIdentity,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PushNotificationSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Force,Mailbox,OutBuffer,OutVariable,RemoveStorage,SubscriptionStoreId,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mail_Recipients
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Instance,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,AutoMapping,Confirm,Deny,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,InheritanceType,OutBuffer,OutVariable,Owner,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Connect-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveSyncMailboxPolicy,AddressBookPolicy,Alias,AllowLegacyDNMismatch,Archive,Confirm,Database,Debug,DomainController,Equipment,ErrorAction,ErrorVariable,Force,Identity,LinkedCredential,LinkedDomainController,LinkedMasterAccount,ManagedFolderMailboxPolicy,ManagedFolderMailboxPolicyAllowed,OutBuffer,OutVariable,RetentionPolicy,Room,Shared,User,ValidateOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"RichCoexistenceRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Arbitration,Confirm,Debug,DisableArbitrationMailboxWithOABsAllowed,DisableLastArbitrationMailboxAllowed,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,PublicFolder,RemoteArchive,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-ServiceEmailChannel", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Force")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Alias,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalEmailAddress,Identity,MacAttachmentFormat,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,PrimarySmtpAddress,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Alias,Confirm,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,ExternalEmailAddress,Identity,MacAttachmentFormat,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,PrimarySmtpAddress,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions"
					}, "ActiveSyncMailboxPolicy"),
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"RichCoexistenceRestrictions"
					}, "Archive,ArchiveName"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AddressBookPolicy,Alias,Arbitration,ArchiveDatabase,ArchiveDomain,ArchiveGuid,Confirm,Database,Debug,Discovery,DisplayName,DomainController,Equipment,ErrorAction,ErrorVariable,Force,HoldForMigration,Identity,LinkedCredential,LinkedDomainController,LinkedMasterAccount,LinkedRoom,OutBuffer,OutVariable,PrimarySmtpAddress,PublicFolder,RemoteArchive,RetentionPolicy,Room,Shared,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ACLableSyncedObjectEnabled,Alias,Archive,ArchiveName,Confirm,Debug,DisplayName,DomainController,Equipment,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PrimarySmtpAddress,RemoteRoutingAddress,Room,Shared,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-ServiceEmailChannel", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ActiveSyncDebugLogging,ProtocolSettings,RecalculateHasActiveSyncDevicePartnership"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LogonStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Database,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,FolderScope,Identity,IncludeAnalysis,IncludeOldestAndNewestItems,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,ReadFromDomainController,ResultSize,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,VerifyDefaultFolderNameLanguage,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Database,Debug,Detailed,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,StoreMailbox,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludePassive,IncludeQuarantineDetails,NoADLookup,OutBuffer,OutVariable,Server,StoreMailboxIdentity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,Verbose,WarningAction,WarningVariable,WritableDatabase,WritableRecipient,WritableServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync,GetMailboxLog,NotificationEmailAddresses,ShowRecoveryPassword"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PhysicalAvailabilityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DailyStatistics,Database,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExchangeServer,OutBuffer,OutVariable,ReportingDatabase,ReportingPeriod,ReportingServer,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Archive,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OnPremisesOrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceAvailabilityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DailyStatistics,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportingDatabase,ReportingPeriod,ReportingServer,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,MaintenanceWindowDays,OutBuffer,OutVariable,ReportingDatabase,ReportingServer,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,BypassOwnerCheck,Debug,DeletedSiteMailbox,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Trust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainName,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPrincipalNamesSuffix", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OrganizationalUnit,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,ExternalIPAddresses,Features,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,HasAttachment,HasClassification,HeaderContainsWords,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "EnableRoomMailboxAccount")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,CorruptionType,Database,Debug,DetectOnly,DomainController,ErrorAction,ErrorVariable,Force,Mailbox,OutBuffer,OutVariable,StoreMailbox,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Instance,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,Deny,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,InheritanceType,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxRepairRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PrimarySmtpAddress"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSyncDebugLogging,ActiveSyncEnabled,ActiveSyncMailboxPolicy,OWAforDevicesEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DisplayName,DomainController,ECPEnabled,EmailAddresses,ErrorAction,ErrorVariable,IgnoreDefaultScope,ImapEnableExactRFC822Size,MAPIBlockOutlookExternalConnectivity,MAPIBlockOutlookNonCachedMode,MAPIBlockOutlookRpcHttp,MAPIBlockOutlookVersions,MapiHttpEnabled,Name,OutBuffer,OutVariable,PopEnableExactRFC822Size,SamAccountName,ShowGalAsDefaultView,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EwsPermissions"
					}, "EwsAllowEntourage,EwsAllowList,EwsAllowMacOutlook,EwsAllowOutlook,EwsApplicationAccessPolicy,EwsBlockList,EwsEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapPermissions"
					}, "ImapEnabled,ImapForceICalForCalendarRetrievalOption,ImapMessagesRetrievalMimeFormat,ImapSuppressReadReceipt,ImapUseProtocolDefaults"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "OwaMailboxPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "OWAEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OutlookAnywherePermissions"
					}, "MAPIEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopPermissions"
					}, "PopEnabled,PopForceICalForCalendarRetrievalOption,PopMessagesRetrievalMimeFormat,PopSuppressReadReceipt,PopUseProtocolDefaults")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddAdditionalResponse,AddNewRequestsTentatively,AddOrganizerToSubject,AdditionalResponse,AllBookInPolicy,AllRequestInPolicy,AllRequestOutOfPolicy,AllowConflicts,AllowRecurringMeetings,AutomateProcessing,BookInPolicy,BookingWindowInDays,Confirm,ConflictPercentageAllowed,Debug,DeleteAttachments,DeleteComments,DeleteNonCalendarItems,DeleteSubject,DomainController,EnableResponseDetails,EnforceSchedulingHorizon,ErrorAction,ErrorVariable,ForwardRequestsToDelegates,IgnoreDefaultScope,MaximumConflictInstances,MaximumDurationInMinutes,OrganizerInfo,OutBuffer,OutVariable,RemoveForwardedMeetingNotifications,RemoveOldMeetingMessages,RemovePrivateProperty,RequestInPolicy,RequestOutOfPolicy,ResourceDelegates,ScheduleOnlyDuringWorkHours,TentativePendingApproval,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "ProcessExternalMeetingMessages")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AssistantName,City,Company,CountryOrRegion,Department,DisplayName,Fax,FirstName,GeoCoordinates,HomePhone,Identity,Initials,LastName,Manager,MobilePhone,Name,Notes,Office,OtherFax,OtherHomePhone,OtherTelephone,Pager,Phone,PhoneticDisplayName,PostalCode,SeniorityIndex,StateOrProvince,StreetAddress,TelephoneAssistant,Title,WebPage"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,IgnoreDefaultScope,OutBuffer,OutVariable,PostOfficeBox,SimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServers,Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,ExternalIPAddresses,Features,Name,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,HasAttachment,HasClassification,HeaderContainsWords,Identity,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,Name,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,EmailAddressPolicyEnabled,ErrorAction,ErrorVariable,ForceUpgrade,IgnoreDefaultScope,MacAttachmentFormat,MaxReceiveSize,MaxRecipientPerMessage,MaxSendSize,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,RemovePicture,RemoveSpokenName,SecondaryAddress,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "ModeratedBy,ModerationEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,ImmutableId,Name,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,EmailAddressPolicyEnabled,ErrorAction,ErrorVariable,ForceUpgrade,IgnoreDefaultScope,MacAttachmentFormat,MaxReceiveSize,MaxSendSize,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,RecipientLimits,RecoverableItemsQuota,RecoverableItemsWarningQuota,RemovePicture,RemoveSpokenName,SamAccountName,SecondaryAddress,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,UserCertificate,UserPrincipalName,UserSMimeCertificate,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,Identity,ImmutableId,LitigationHoldEnabled,Name,Office,PrimarySmtpAddress"),
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressBookPolicy"),
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions",
						"RichCoexistenceRestrictions"
					}, "ArchiveName"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,AntispamBypassEnabled,ApplyMandatoryProperties,Arbitration,ArbitrationMailbox,ArchiveDomain,ArchiveQuota,ArchiveStatus,ArchiveWarningQuota,CalendarLoggingQuota,CalendarRepairDisabled,CalendarVersionStoreDisabled,ClientExtensions,Confirm,Debug,DeliverToMailboxAndForward,DomainController,DowngradeHighPriorityMessagesEnabled,DumpsterMessagesPerFolderCountReceiveQuota,DumpsterMessagesPerFolderCountWarningQuota,EmailAddressPolicyEnabled,EnableRoomMailboxAccount,EndDateForRetentionHold,ErrorAction,ErrorVariable,ExtendedPropertiesCountQuota,ExternalOofOptions,FolderHierarchyChildrenCountReceiveQuota,FolderHierarchyChildrenCountWarningQuota,FolderHierarchyDepthReceiveQuota,FolderHierarchyDepthWarningQuota,FoldersCountReceiveQuota,FoldersCountWarningQuota,Force,ForwardingSmtpAddress,GMGen,GrantSendOnBehalfTo,IgnoreDefaultScope,Languages,LinkedCredential,LinkedDomainController,LinkedMasterAccount,MailboxMessagesPerFolderCountReceiveQuota,MailboxMessagesPerFolderCountWarningQuota,Management,MaxBlockedSenders,MaxReceiveSize,MaxSafeSenders,MaxSendSize,MessageCopyForSendOnBehalfEnabled,MessageCopyForSentAsEnabled,MessageTracking,Migration,OABGen,OMEncryption,OfflineAddressBook,OutBuffer,OutVariable,PstProvider,QueryBaseDN,RecipientLimits,RecoverableItemsQuota,RecoverableItemsWarningQuota,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RemoteRecipientType,RemovePicture,RemoveSpokenName,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,RetainDeletedItemsUntilBackup,RetentionHoldEnabled,RetentionPolicy,RoomMailboxPassword,RulesQuota,SCLDeleteEnabled,SCLDeleteThreshold,SCLJunkEnabled,SCLJunkThreshold,SCLQuarantineEnabled,SCLQuarantineThreshold,SCLRejectEnabled,SCLRejectThreshold,SamAccountName,SecondaryAddress,SharingPolicy,SimpleDisplayName,SingleItemRecoveryEnabled,StartDateForRetentionHold,ThrottlingPolicy,Type,UMDataStorage,UMGrammar,UseDatabaseQuotaDefaults,UseDatabaseRetentionDefaults,UserCertificate,UserPrincipalName,UserSMimeCertificate,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailboxQuotaPermissions"
					}, "IssueWarningQuota,ProhibitSendQuota,ProhibitSendReceiveQuota"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailboxSIRPermissions"
					}, "RetainDeletedItemsFor"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ManagedFoldersPermissions"
					}, "RemoveManagedFolderAndPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "MessageTrackingReadStatusEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PropertiesMasteredOnPremiseRestrictions",
						"SetHiddenFromAddressListPermissions"
					}, "HiddenFromAddressListsEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "IsExcludedFromServingHierarchy,IsHierarchyReady,PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoReplyState,Confirm,Debug,DomainController,EndTime,ErrorAction,ErrorVariable,ExternalAudience,ExternalMessage,IgnoreDefaultScope,OutBuffer,OutVariable,StartTime,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "InternalMessage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultReminderTime,DomainController,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,Verbose,WarningAction,WarningVariable,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "WorkDays")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PublishDateRangeFrom,PublishDateRangeTo,PublishEnabled,ResetUrl,SearchableUrlEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BlockedSendersAndDomains,Confirm,ContactsTrusted,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IgnoreDefaultScope,OutBuffer,OutVariable,TrustedListsOnly,TrustedSendersAndDomains,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,Debug,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,DomainController,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,IgnoreDefaultScope,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DateFormat,ErrorAction,ErrorVariable,Language,LocalizeDefaultFolderName,OutBuffer,OutVariable,TimeFormat,TimeZone,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CheckBeforeSend,Confirm,DictionaryLanguage,ErrorAction,ErrorVariable,IgnoreMixedDigits,IgnoreUppercase,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,InstantMessagingType,JunkEmailEnabled,SMimeEnabled,UNCAccessOnPrivateComputersEnabled,UNCAccessOnPublicComputersEnabled,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,IsDefault,JournalEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PremiumClientEnabled,PublicFoldersEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ACLableSyncedObjectEnabled,AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ArchiveGuid,ArchiveName,BypassModerationFromSendersOrMembers,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Debug,DisplayName,DomainController,EmailAddressPolicyEnabled,EmailAddresses,ErrorAction,ErrorVariable,ExchangeGuid,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreDefaultScope,ImmutableId,ModeratedBy,ModerationEnabled,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RecoverableItemsQuota,RecoverableItemsWarningQuota,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RemoteRoutingAddress,RemovePicture,RemoveSpokenName,RequireSenderAuthenticationEnabled,SamAccountName,SendModerationNotifications,Type,UserPrincipalName,Verbose,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "ResetPasswordOnNextLogon")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PhoneticDisplayName,SeniorityIndex"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,LinkedCredential,LinkedDomainController,LinkedMasterAccount,OutBuffer,OutVariable,RemotePowerShellEnabled,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"OpenDomainProfileUpdatePermissions",
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "DisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "PostOfficeBox,SimpleDisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "City,CountryOrRegion,Fax,FirstName,GeoCoordinates,HomePhone,Initials,LastName,MobilePhone,Notes,Office,Pager,Phone,PostalCode,StateOrProvince,StreetAddress,WebPage"),
					new RoleParameters(new string[]
					{
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "AssistantName,Company,Department,Manager,Name,OtherFax,OtherHomePhone,OtherTelephone,TelephoneAssistant,Title"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "CertificateSubject,Confirm,Debug,DomainController,IgnoreDefaultScope,PublicFolder,ResetPasswordOnNextLogon,SamAccountName,UserPrincipalName,Verbose,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,PictureData,PictureStream,Preview,Save,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MAPIConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,ForceUpgrade,OnPremisesCredentials,OutBuffer,OutVariable,SuppressOAuthWarning,TenantCredentials,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mail_Tips
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccessMethod,Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,TargetAutodiscoverEpr,UseServiceAccount,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Name,TargetAddressDomains")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DomainNames,MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope,Name")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,PerUserAccount,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,CustomerFeedbackEnabled,Debug,DomainController,ErrorAction,ErrorVariable,Industry,MailTipsAllTipsEnabled,MailTipsGroupMetricsEnabled,MailTipsMailboxSourcedTipsEnabled,OutBuffer,OutVariable,PublicComputersDetectionEnabled,PublicFolderMailboxesLockedForNewConnections,PublicFolderMailboxesMigrationComplete,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTipsExternalRecipientsTipsEnabled,MailTipsLargeAudienceThreshold")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mailbox_Import_Export
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Mailbox,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxExportRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Mailbox,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxImportRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AssociatedMessagesCopyOption,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ConflictResolutionOption,ContentFilter,ContentFilterLanguage,Debug,DomainController,ErrorAction,ErrorVariable,ExcludeDumpster,ExcludeFolders,FilePath,IncludeFolders,InternalFlags,IsArchive,LargeItemLimit,Mailbox,Name,OutBuffer,OutVariable,Priority,RemoteCredential,RemoteHostName,SkipMerging,SourceRootFolder,Suspend,SuspendComment,TargetRootFolder,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AssociatedMessagesCopyOption,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ConflictResolutionOption,ContentCodePage,Debug,DomainController,ErrorAction,ErrorVariable,ExcludeDumpster,ExcludeFolders,FilePath,IncludeFolders,InternalFlags,IsArchive,LargeItemLimit,Mailbox,Name,OutBuffer,OutVariable,Priority,RemoteCredential,RemoteHostName,SkipMerging,SourceRootFolder,Suspend,SuspendComment,TargetRootFolder,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DeleteContent,DoNotIncludeArchive,DomainController,ErrorAction,ErrorVariable,EstimateResultOnly,Force,Identity,IncludeUnsearchableItems,LogLevel,LogOnly,OutBuffer,OutVariable,SearchDumpster,SearchDumpsterOnly,SearchQuery,TargetFolder,TargetMailbox,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Priority,RehomeRequest,RemoteCredential,RemoteHostName,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Priority,RehomeRequest,RemoteCredential,RemoteHostName,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mailbox_Search
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Mailbox,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxExportRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,ShowDeletionInProgressSearches,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,Organization,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AssociatedMessagesCopyOption,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ConflictResolutionOption,ContentFilter,ContentFilterLanguage,Debug,DomainController,ErrorAction,ErrorVariable,ExcludeDumpster,ExcludeFolders,FilePath,IncludeFolders,InternalFlags,IsArchive,LargeItemLimit,Mailbox,Name,OutBuffer,OutVariable,Priority,SkipMerging,SourceRootFolder,Suspend,SuspendComment,TargetRootFolder,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,SourceMailboxes,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "AllPublicFolderSources,AllSourceMailboxes,EndDate,EstimateOnly,ExcludeDuplicateMessages,IncludeKeywordStatistics,IncludeUnsearchableItems,Language,LogLevel,MessageTypes,PublicFolderSources,Recipients,SearchQuery,Senders,StartDate,StatusMailRecipients,TargetMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DoNotIncludeArchive,DomainController,ErrorAction,ErrorVariable,EstimateResultOnly,Force,Identity,IncludeUnsearchableItems,LogLevel,LogOnly,OutBuffer,OutVariable,SearchDumpster,SearchDumpsterOnly,SearchQuery,TargetFolder,TargetMailbox,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Priority,RehomeRequest,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,SourceMailboxes,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "AllPublicFolderSources,AllSourceMailboxes,EndDate,EstimateOnly,ExcludeDuplicateMessages,IncludeKeywordStatistics,IncludeUnsearchableItems,Language,LogLevel,MessageTypes,PublicFolderSources,Recipients,SearchQuery,Senders,StartDate,StatisticsStartIndex,StatusMailRecipients,TargetMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Resume,StatisticsStartIndex,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Message_Tracking
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccessMethod,Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,TargetAutodiscoverEpr,UseServiceAccount,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,End,ErrorAction,ErrorVariable,EventId,InternalMessageId,MessageId,MessageSubject,OutBuffer,OutVariable,Recipients,Reference,ResultSize,Sender,Server,Start,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Debug,DetailLevel,DoNotResolve,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientPathFilter,Recipients,ReportTemplate,ResultSize,Status,TraceLevel,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Name,TargetAddressDomains")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DomainNames,Name"),
					new RoleParameters(new string[]
					{
						"MessageTrackingPermissions"
					}, "DeliveryReportEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxExportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,Debug,DoNotResolve,DomainController,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,TraceLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,PerUserAccount,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"MessageTrackingPermissions"
					}, "DeliveryReportEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Migration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Complete-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MigrationReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,CsvStream,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RowCount,StartingRowIndex,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-RecipientDataProperty", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Picture,SpokenName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitSkippedItemsTo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-RecipientDataProperty", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,Identity,OutBuffer,OutVariable,Picture,SpokenName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "ArchiveOnly,AutoComplete,BadItemLimit,LargeItemLimit,Local,PrimaryOnly,SkipSteps,SourceEndpoint,SourcePublicFolderDatabase,TargetDeliveryDomain,TargetEndpoint"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "AllowIncrementalSyncs,AllowUnknownColumnsInCSV,AutoRetryCount,AutoStart,CSVData,CompleteAfter,Confirm,Debug,DisableOnCopy,DisallowExistingUsers,DomainController,ErrorAction,ErrorVariable,Locale,Name,NotificationEmails,OutBuffer,OutVariable,ReportInterval,StartAfter,TargetArchiveDatabases,TargetDatabases,TimeZone,UserIds,Users,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "Autodiscover,EmailAddress,ExchangeRemoteMove,PSTImport"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,Name,OutBuffer,OutVariable,RemoteServer,SkipVerification,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArchiveGuid,ArchiveName,ExchangeGuid")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "BadItemLimit,LargeItemLimit,SourcePublicFolderDatabase"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "AllowIncrementalSyncs,AllowUnknownColumnsInCSV,AutoRetryCount,CSVData,CompleteAfter,Confirm,Debug,DisallowExistingUsers,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,ReportInterval,StartAfter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Features,MaxConcurrentMigrations,MaxNumberOfBatches,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,OutBuffer,OutVariable,RemoteServer,SkipVerification,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Validate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MigrationServerAvailability", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Autodiscover,Confirm,Credentials,Debug,EmailAddress,Endpoint,ErrorAction,ErrorVariable,ExchangeRemoteMove,OutBuffer,OutVariable,PSTImport,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapMigrationPermissions"
					}, "FilePath,RemoteServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Monitoring
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Dump-ProvisioningCache", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Application,CacheKeys,Confirm,CurrentOrganization,Debug,ErrorAction,ErrorVariable,GlobalCache,Organizations,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-ActiveSyncLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,EndDate,ErrorAction,ErrorVariable,Filename,Force,OutBuffer,OutVariable,OutputPath,OutputPrefix,StartDate,UseGMT,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FailedContentIndexDocuments", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorCode,ErrorVariable,FailureMode,Identity,MailboxDatabase,OutBuffer,OutVariable,ResultSize,Server,StartDate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-StoreUsageStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludePassive,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,IncludeSimulator,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Extension,Identity,MimeType,Name,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Reset-ProvisioningCache", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Application,CacheKeys,Confirm,CurrentOrganization,Debug,ErrorAction,ErrorVariable,GlobalCache,Organizations,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ActiveSyncConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowUnsecureAccess,ClientAccessServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,LightMode,MailboxCredential,MailboxServer,MonitoringContext,MonitoringInstance,OutBuffer,OutVariable,ResetTestAccountCredentials,Timeout,TrustAnySSLCertificate,URL,UseAutodiscoverForClientAccessServer,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ArchiveConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,IncludeArchiveMRMConfiguration,MessageId,OutBuffer,OutVariable,UserSmtp,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-AssistantHealth", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,IncludeCrashDump,MaxProcessingTimeInMinutes,MonitoringContext,OutBuffer,OutVariable,ResolveProblems,ServerName,Verbose,WarningAction,WarningVariable,WatermarkBehindWarningThreholdInMinutes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-CalendarConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,LightMode,MailboxServer,MonitoringContext,OutBuffer,OutVariable,ResetTestAccountCredentials,TestType,Timeout,TrustAnySSLCertificate,Verbose,VirtualDirectoryName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-EcpConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,LightMode,MailboxServer,MonitoringContext,OutBuffer,OutVariable,RSTEndPoint,ResetTestAccountCredentials,TestType,Timeout,TrustAnySSLCertificate,Verbose,VirtualDirectoryName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-EdgeSynchronization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExcludeRecipientTest,FullCompareMode,MaxReportSize,MonitoringContext,OutBuffer,OutVariable,TargetServer,Verbose,VerifyRecipient,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ExchangeSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IndexingTimeoutInSeconds,MailboxDatabase,MonitoringContext,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recipient,Sender,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ImapConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServer,Confirm,ConnectionType,Debug,DomainController,ErrorAction,ErrorVariable,LightMode,MailboxCredential,MailboxServer,MonitoringContext,OutBuffer,OutVariable,PerConnectionTimeout,PortClientAccessServer,ResetTestAccountCredentials,Timeout,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MAPIConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectoryTimeout,AllConnectionsTimeout,Archive,Confirm,CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludePassive,MonitoringContext,OutBuffer,OutVariable,PerConnectionTimeout,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MRSHealth", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MRSProxyCredentials,MRSProxyServer,MaxQueueScanAgeSeconds,MonitoringContext,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-Mailflow", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectoryTimeout,AutoDiscoverTargetMailboxServer,Confirm,CrossPremises,CrossPremisesExpirationTimeout,CrossPremisesPendingErrorCount,Debug,DomainController,ErrorAction,ErrorLatency,ErrorVariable,ExecutionTimeout,Identity,MonitoringContext,OutBuffer,OutVariable,TargetDatabase,TargetEmailAddress,TargetEmailAddressDisplayName,TargetMailboxServer,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OAuthConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,ReloadConfig,Service,TargetUri,UseCachedToken,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,UserIdentity,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OutlookConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,ErrorAction,ErrorVariable,Hostname,MailboxId,OutBuffer,OutVariable,ProbeIdentity,RunFromServerId,TimeOutSeconds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OutlookWebServices", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDiscoverServer,ClientAccessServer,Confirm,Debug,ErrorAction,ErrorVariable,Identity,MailboxCredential,MonitoringContext,OutBuffer,OutVariable,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-PopConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServer,Confirm,ConnectionType,Debug,DomainController,ErrorAction,ErrorVariable,LightMode,MailboxCredential,MailboxServer,MonitoringContext,OutBuffer,OutVariable,PerConnectionTimeout,PortClientAccessServer,ResetTestAccountCredentials,Timeout,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-PowerShellConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Authentication,ClientAccessServer,Confirm,ConnectionUri,Debug,DomainController,ErrorAction,ErrorVariable,MailboxServer,MonitoringContext,OutBuffer,OutVariable,ResetTestAccountCredentials,TestCredential,TestType,TrustAnySSLCertificate,Verbose,VirtualDirectoryName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ReplicationHealth", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectoryTimeout,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MonitoringContext,OutBuffer,OutVariable,OutputObjects,TransientEventSuppressionWindow,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SenderId", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,HelloDomain,IPAddress,OutBuffer,OutVariable,PurportedResponsibleDomain,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ServiceHealth", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActiveDirectoryTimeout,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,MonitoringContext,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestorIdentity,SharePointUrl,UseAppTokenOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SmtpConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MonitoringContext,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-UMConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CallRouter,CertificateThumbprint,Confirm,Debug,DiagDtmfDurationInMilisecs,DiagDtmfSequence,DiagInitialSilenceInMilisecs,DiagInterDtmfDiffGapInMilisecs,DiagInterDtmfGapInMilisecs,DomainController,ErrorAction,ErrorVariable,From,ListenPort,MediaSecured,MonitoringContext,OutBuffer,OutVariable,PIN,Phone,RemotePort,ResetPIN,Secured,TUILogon,TUILogonAll,Timeout,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-WebServicesConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoDiscoverServer,ClientAccessServer,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,LightMode,MailboxCredential,MonitoringContext,OutBuffer,OutVariable,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Move_Mailboxes
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Flags,HighPriority,Identity,IncludeSoftDeletedObjects,MoveStatus,Offline,OrganizationalUnit,OutBuffer,OutVariable,Protect,RemoteHostName,ResultSize,SortBy,SourceDatabase,Suspend,SuspendWhenReadyToComplete,TargetDatabase,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,MailboxGuid,MoveRequestQueue,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Database,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AllowLargeItems,ArchiveDomain,ArchiveOnly,ArchiveTargetDatabase,BadItemLimit,BatchName,CompleteAfter,CompletedRequestAgeLimit,Confirm,Debug,DoNotPreserveMailboxSignature,DomainController,ErrorAction,ErrorVariable,ForceOffline,ForcePull,ForcePush,Identity,IgnoreRuleLimitErrors,IncrementalSyncInterval,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Outbound,PreventCompletion,PrimaryOnly,Priority,Protect,Remote,RemoteArchiveTargetDatabase,RemoteCredential,RemoteGlobalCatalog,RemoteHostName,RemoteLegacy,RemoteOrganizationName,RemoteTargetDatabase,SkipMoving,StartAfter,Suspend,SuspendComment,SuspendWhenReadyToComplete,TargetDatabase,TargetDeliveryDomain,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxGuid,MoveRequestQueue,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendWhenReadyToComplete,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArchiveGuid,ArchiveName,ExchangeGuid")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,ArchiveTargetDatabase,BadItemLimit,BatchName,CompleteAfter,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IgnoreRuleLimitErrors,IncrementalSyncInterval,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,PreventCompletion,Priority,Protect,RemoteCredential,RemoteGlobalCatalog,RemoteHostName,SkipMoving,StartAfter,SuspendWhenReadyToComplete,TargetDatabase,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class My_Custom_Apps
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Url"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "FileData,FileStream")
				}, "c")
			};
		}

		private class My_Marketplace_Apps
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DownloadOnly,Etoken,MarketplaceAssetID,MarketplaceQueryMarket,MarketplaceServicesUrl")
				}, "c")
			};
		}

		private class My_ReadWriteMailbox_Apps
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AllowReadWriteMailbox")
				}, "c")
			};
		}

		private class MyAddressInformation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "PostOfficeBox"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "City,CountryOrRegion,Fax,GeoCoordinates,Office,Phone,PostalCode,StateOrProvince,StreetAddress")
				}, "c")
			};
		}

		private class MyBaseOptions
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Force")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ActiveSyncDebugLogging,ProtocolSettings,RecalculateHasActiveSyncDevicePartnership"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,MailFolderOnly,OutBuffer,OutVariable,Recurse,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,VerifyDefaultFolderNameLanguage,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientPathFilter,Recipients,ReportTemplate,ResultSize,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync,GetMailboxLog,ShowRecoveryPassword"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,HasAttachment,HasClassification,HeaderContainsWords,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Body,BodyFormat,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Subject,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Parent,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSyncDebugLogging"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ShowGalAsDefaultView,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapPermissions"
					}, "ImapForceICalForCalendarRetrievalOption,ImapMessagesRetrievalMimeFormat,ImapSuppressReadReceipt,ImapUseProtocolDefaults"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopPermissions"
					}, "PopForceICalForCalendarRetrievalOption,PopMessagesRetrievalMimeFormat,PopSuppressReadReceipt,PopUseProtocolDefaults")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddAdditionalResponse,AddNewRequestsTentatively,AddOrganizerToSubject,AdditionalResponse,AllBookInPolicy,AllRequestInPolicy,AllRequestOutOfPolicy,AllowConflicts,AllowRecurringMeetings,AutomateProcessing,BookInPolicy,BookingWindowInDays,Confirm,ConflictPercentageAllowed,DeleteAttachments,DeleteComments,DeleteNonCalendarItems,DeleteSubject,EnableResponseDetails,EnforceSchedulingHorizon,ErrorAction,ErrorVariable,ForwardRequestsToDelegates,IgnoreDefaultScope,MaximumConflictInstances,MaximumDurationInMinutes,OrganizerInfo,OutBuffer,OutVariable,RemoveForwardedMeetingNotifications,RemoveOldMeetingMessages,RemovePrivateProperty,RequestInPolicy,RequestOutOfPolicy,ResourceDelegates,ScheduleOnlyDuringWorkHours,TentativePendingApproval,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "ProcessExternalMeetingMessages")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,HasAttachment,HasClassification,HeaderContainsWords,Identity,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,UserCertificate,UserSMimeCertificate"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,DeliverToMailboxAndForward,ErrorAction,ErrorVariable,ExternalOofOptions,ForwardingSmtpAddress,GrantSendOnBehalfTo,Languages,MessageCopyForSendOnBehalfEnabled,MessageCopyForSentAsEnabled,OutBuffer,OutVariable,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "NewPassword,OldPassword,Password"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoReplyState,Confirm,EndTime,ErrorAction,ErrorVariable,ExternalAudience,ExternalMessage,IgnoreDefaultScope,OutBuffer,OutVariable,StartTime,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "InternalMessage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DefaultReminderTime,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,WarningAction,WarningVariable,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "WorkDays")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DetailLevel,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PublishDateRangeFrom,PublishDateRangeTo,PublishEnabled,ResetUrl,SearchableUrlEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BlockedSendersAndDomains,ContactsTrusted,Enabled,ErrorAction,ErrorVariable,IgnoreDefaultScope,OutBuffer,OutVariable,TrustedListsOnly,TrustedSendersAndDomains,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,IgnoreDefaultScope,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DateFormat,ErrorAction,ErrorVariable,Language,LocalizeDefaultFolderName,OutBuffer,OutVariable,TimeFormat,TimeZone,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CheckBeforeSend,Confirm,DictionaryLanguage,ErrorAction,ErrorVariable,IgnoreMixedDigits,IgnoreUppercase,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,PictureData,PictureStream,Preview,Save,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class MyContactInformation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "PostOfficeBox"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "City,CountryOrRegion,Fax,GeoCoordinates,HomePhone,MobilePhone,Office,Pager,Phone,PostalCode,StateOrProvince,StreetAddress,WebPage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c")
			};
		}

		private class MyDiagnostics
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MailboxDiagnosticLogs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,ComponentName,Confirm,Credential,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProperties,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticAnalysis", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CalendarLogs,Credential,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,GlobalObjectId,LogLocation,OutBuffer,OutVariable,OutputAs,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,Identity,Latest,LogLocation,MeetingID,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,StartDate,Subject,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IncludeHidden")
				}, "c")
			};
		}

		private class MyDisplayName
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "SimpleDisplayName")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"OpenDomainProfileUpdatePermissions",
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "DisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "SimpleDisplayName")
				}, "c")
			};
		}

		private class MyDistributionGroupMembership
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class MyDistributionGroups
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,SamAccountName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddressPolicyEnabled,EmailAddresses,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,Identity,ManagedBy,MemberJoinRestriction,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,SamAccountName,SendOofMessageToOriginatorEnabled,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"PropertiesMasteredOnPremiseRestrictions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "BypassModerationFromSendersOrMembers")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"ResourceMailboxRestrictions"
					}, "MailTip,MailTipTranslations")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity,ManagedBy,Name,Notes,OutBuffer,OutVariable,PhoneticDisplayName,SeniorityIndex,SimpleDisplayName,Universal,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class MyMailboxDelegation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "GrantSendOnBehalfTo")
				}, "c")
			};
		}

		private class MyMobileInformation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MobilePhone,Pager")
				}, "c")
			};
		}

		private class MyName
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "FirstName,Initials,LastName,Notes")
				}, "c")
			};
		}

		private class MyPersonalInformation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "HomePhone,WebPage")
				}, "c")
			};
		}

		private class MyProfileInformation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "SimpleDisplayName")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"OpenDomainProfileUpdatePermissions",
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "DisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "SimpleDisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "FirstName,Initials,LastName,Notes")
				}, "c")
			};
		}

		private class MyRetentionPolicies
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeSystemTags,Mailbox,OptionalInMailbox,OutBuffer,OutVariable,Types,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,MessageClass,OptionalInMailbox,OutBuffer,OutVariable,RetentionId,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class MyTeamMailboxes
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,SharePointUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ShowInMyClient,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestorIdentity,UseAppTokenOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class MyTextMessaging
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Compare-TextMessagingVerificationCode", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,VerificationCode,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"SMSPermissions"
					}, "SendTextMessageNotificationTo"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "Name")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Send-TextMessagingVerificationCode", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "CalendarUpdateNotification,CalendarUpdateSendDuringWorkHour,Confirm,DailyAgendaNotification,DailyAgendaNotificationSendTime,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,MeetingReminderNotification,MeetingReminderSendDuringWorkHour,NextDays,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"SMSPermissions"
					}, "SendTextMessageNotificationTo"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Confirm,CountryRegionId,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,MobileOperatorId,NotificationPhoneNumber,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMSMSMsgWaitingPermissions"
					}, "UMSMSNotificationOption")
				}, "c")
			};
		}

		private class MyVoiceMail
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,KeyMappings,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,Identity,KeyMappings,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,ImListMigrationCompleted,MissedCallNotificationEnabled,OutBuffer,OutVariable,PinlessAccessToVoiceMailEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,DefaultPlayOnPhoneNumber,ErrorAction,ErrorVariable,FolderToReadEmailsFrom,Greeting,Identity,OutBuffer,OutVariable,ReadOldestUnreadVoiceMessagesFirst,ReceivedVoiceMailPreviewEnabled,SentVoiceMailPreviewEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Pin,PinExpired,SendEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PhoneticDisplayName,SeniorityIndex")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "AwayVoicemailGreeting,CallAnsweringRuleId,Confirm,DefaultVoicemailGreeting,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PhoneNumber,UMMailbox,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Org_Custom_Apps
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Url"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultStateForUser,DomainController,Enabled,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AllowReadWriteMailbox,FileData,FileStream")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultStateForUser,DomainController,Enabled,ErrorAction,ErrorVariable,Force,Name,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Org_Marketplace_Apps
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DownloadOnly,Etoken,MarketplaceAssetID,MarketplaceQueryMarket,MarketplaceServicesUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultStateForUser,DomainController,Enabled,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AllowReadWriteMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultStateForUser,DomainController,Enabled,ErrorAction,ErrorVariable,Force,Name,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Client_Access
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-AutoDiscoverConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DeleteConfig,DomainController,ErrorAction,ErrorVariable,MultipleExchangeDeployments,OutBuffer,OutVariable,PreferredSourceFqdn,SourceForestCredential,TargetForestCredential,TargetForestDomainController,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Monitoring")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceClass", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ProtocolSettings"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessArray", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Site,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Monitoring")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RpcClientAccess", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AccessLevel,Characteristic,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,QueryString,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuthScheme,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,TargetUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuthMetadataUrl,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,TrustAnySSLCertificate,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ClientAccessArray", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ArrayDefinition,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,ServerCount,Site,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Action,AnyOfAuthenticationTypes,AnyOfClientIPAddressesOrRanges,AnyOfProtocols,AnyOfSourceTcpPortNumbers,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExceptAnyOfAuthenticationTypes,ExceptAnyOfClientIPAddressesOrRanges,ExceptAnyOfProtocols,ExceptAnyOfSourceTcpPortNumbers,ExceptUserIsMemberOf,ExceptUsernameMatchesAnyOfPatterns,Name,OutBuffer,OutVariable,Priority,UserIsMemberOf,UserRecipientFilter,UsernameMatchesAnyOfPatterns,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OutlookProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptSecurityIdentifierInformation,ActAsPermissions,AppOnlyPermissions,ApplicationIdentifier,AuthMetadataUrl,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IssuerIdentifier,LinkedAccount,Name,OutBuffer,OutVariable,Realm,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDeviceClass", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ClientAccessArray", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OutlookProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AccessLevel,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AdminMailRecipients,AllowAccessForUnSupportedPlatform,Confirm,DefaultAccessLevel,ErrorAction,ErrorVariable,Identity,OtaNotificationMailInsert,OutBuffer,OutVariable,UserMailInsert,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuthConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CertificateThumbprint,ClearPreviousCertificate,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,NewCertificateEffectiveDate,NewCertificateThumbprint,OutBuffer,OutVariable,PublishCertificate,Realm,Server,ServiceName,SkipImmediateCertificateDeployment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TargetUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuthMetadataUrl,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IsDefaultAuthorizationEndpoint,Name,OutBuffer,OutVariable,RefreshAuthMetadata,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSyncAllowedDeviceIDs,ActiveSyncBlockedDeviceIDs"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClientAccessArray", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ArrayDefinition,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,ServerCount,Site,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Action,AnyOfAuthenticationTypes,AnyOfClientIPAddressesOrRanges,AnyOfProtocols,AnyOfSourceTcpPortNumbers,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExceptAnyOfAuthenticationTypes,ExceptAnyOfClientIPAddressesOrRanges,ExceptAnyOfProtocols,ExceptAnyOfSourceTcpPortNumbers,ExceptUserIsMemberOf,ExceptUsernameMatchesAnyOfPatterns,Identity,Name,OutBuffer,OutVariable,Priority,UserIsMemberOf,UserRecipientFilter,UsernameMatchesAnyOfPatterns,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutlookProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CertPrincipalName,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,OutlookProviderFlags,RequiredClientVersions,Server,TTL,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptSecurityIdentifierInformation,ActAsPermissions,AppOnlyPermissions,ApplicationIdentifier,AuthMetadataUrl,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IssuerIdentifier,LinkedAccount,Name,OutBuffer,OutVariable,Realm,RefreshAuthMetadata,TrustAnySSLCertificate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RpcClientAccess", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BlockedClientVersions,Confirm,Debug,DomainController,EncryptionRequired,ErrorAction,ErrorVariable,MaximumConnections,Name,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuthenticationType,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Protocol,RemoteAddress,RemotePort,User,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccessMethod,Confirm,Credentials,Debug,DomainController,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,ProxyUrl,TargetAutodiscoverEpr,UseServiceAccount,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ApplyVersion,Confirm,Debug,DomainController,Duration,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,PropertyValue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IncludeAlternateServiceAccountCredentialPassword")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeAssistanceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuthScheme,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,TargetUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Component,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,MaxVersion,MinVersion,Name,OutBuffer,OutVariable,Parameters,Reason,Section,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TargetUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,PerUserAccount,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ExchangeAssistanceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CommunityLinkDisplayEnabled,CommunityURL,Confirm,ControlPanelFeedbackEnabled,ControlPanelFeedbackURL,ControlPanelHelpURL,Debug,DomainController,ErrorAction,ErrorVariable,ExchangeHelpAppOnline,ManagementConsoleFeedbackEnabled,ManagementConsoleFeedbackURL,ManagementConsoleHelpURL,OWAFeedbackEnabled,OWAFeedbackURL,OWAHelpURL,OWALightFeedbackEnabled,OWALightFeedbackURL,OWALightHelpURL,OutBuffer,OutVariable,PrivacyLinkDisplayEnabled,PrivacyStatementURL,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ACLableSyncedObjectEnabled,ActivityBasedAuthenticationTimeoutEnabled,ActivityBasedAuthenticationTimeoutInterval,ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled,AdfsAudienceUris,AdfsAuthenticationConfiguration,AdfsEncryptCertificateThumbprint,AdfsIssuer,AdfsSignCertificateThumbprints,AppsForOfficeEnabled,Confirm,CustomerFeedbackEnabled,Debug,DomainController,ErrorAction,ErrorVariable,Industry,IsExcludedFromOffboardMigration,IsExcludedFromOnboardMigration,IsFfoMigrationInProgress,MailTipsAllTipsEnabled,MailTipsGroupMetricsEnabled,MailTipsMailboxSourcedTipsEnabled,ManagedFolderHomepage,MapiHttpEnabled,MaxConcurrentMigrations,MicrosoftExchangeRecipientEmailAddressPolicyEnabled,MicrosoftExchangeRecipientEmailAddresses,MicrosoftExchangeRecipientPrimarySmtpAddress,MicrosoftExchangeRecipientReplyRecipient,OAuth2ClientProfileEnabled,OrganizationSummary,OutBuffer,OutVariable,SCLJunkThreshold,Verbose,WACDiscoveryEndpoint,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EwsPermissions"
					}, "EwsAllowEntourage,EwsAllowList,EwsAllowMacOutlook,EwsAllowOutlook,EwsApplicationAccessPolicy,EwsBlockList,EwsEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTipsExternalRecipientsTipsEnabled,MailTipsLargeAudienceThreshold"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "ReadTrackingEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "DefaultPublicFolderAgeLimit,DefaultPublicFolderDeletedItemRetention,DefaultPublicFolderMaxItemSize,DefaultPublicFolderMovedItemRetention,DefaultPublicFolderProhibitPostQuota,PublicFolderMigrationComplete,PublicFoldersEnabled,PublicFoldersLockedForMigration,RemotePublicFolderMailboxes"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "SiteMailboxCreationURL")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResourcePropertySchema,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MaxVersion,MinVersion,OutBuffer,OutVariable,Parameters,Reason,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Identity,OWAAllowUserChoiceOfSigningCertificate,OWAAlwaysEncrypt,OWAAlwaysSign,OWABCCEncryptedEmailForking,OWACRLConnectionTimeout,OWACRLRetrievalTimeout,OWACheckCRLOnSend,OWAClearSign,OWACopyRecipientHeaders,OWADLExpansionTimeout,OWADisableCRLCheck,OWAEncryptTemporaryBuffers,OWAEncryptionAlgorithms,OWAForceSMIMEClientUpgrade,OWAIncludeCertificateChainAndRootCertificate,OWAIncludeCertificateChainWithoutRootCertificate,OWAIncludeSMIMECapabilitiesInMessage,OWAOnlyUseSmartCard,OWASenderCertificateAttributesToDisplay,OWASignedEmailCertificateInclusion,OWASigningAlgorithms,OWATripleWrapSignedEncryptedMail,OWAUseKeyIdentifier,OWAUseSecondaryProxiesWhenFindingCertificates,SMIMECertificateIssuingCA,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,FullSync,Identity,Organization,OutBuffer,OutVariable,Server,Target,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Transport_Settings
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADSite", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdSiteLink", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EdgeSyncServiceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Site,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SystemMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Original,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SystemMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,DsnCode,ErrorAction,ErrorVariable,Internal,Language,OutBuffer,OutVariable,QuotaMessageType,Text,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SystemMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADSite", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,HubSiteEnabled,InboundMailEnabled,OutBuffer,OutVariable,PartnerId,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AdSiteLink", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExchangeCost,MaxMessageSize,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SystemMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,Original,OutBuffer,OutVariable,Text,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressBookPolicyRoutingEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgentGeneratedMessageLoopDetectionInSmtpEnabled,AgentGeneratedMessageLoopDetectionInSubmissionEnabled,ClearCategories,Confirm,ConvertDisclaimerWrapperToEml,DSNConversionMode,Debug,DiagnosticsAggregationServicePort,DomainController,ErrorAction,ErrorVariable,ExternalDelayDsnEnabled,ExternalDsnDefaultLanguage,ExternalDsnLanguageDetectionEnabled,ExternalDsnMaxMessageAttachSize,ExternalDsnReportingAuthority,ExternalDsnSendHtml,ExternalPostmasterAddress,Force,GenerateCopyOfDSNFor,HeaderPromotionModeSetting,Identity,InternalDelayDsnEnabled,InternalDsnDefaultLanguage,InternalDsnLanguageDetectionEnabled,InternalDsnMaxMessageAttachSize,InternalDsnReportingAuthority,InternalDsnSendHtml,InternalSMTPServers,JournalingReportNdrTo,MaxAllowedAgentGeneratedMessageDepth,MaxAllowedAgentGeneratedMessageDepthPerAgent,MaxDumpsterSizePerDatabase,MaxDumpsterTime,MaxReceiveSize,MaxRecipientEnvelopeLimit,MaxRetriesForLocalSiteShadow,MaxRetriesForRemoteSiteShadow,MaxSendSize,OutBuffer,OutVariable,QueueDiagnosticsAggregationInterval,RejectMessageOnShadowFailure,Rfc2231EncodingEnabled,SafetyNetHoldTime,ShadowHeartbeatFrequency,ShadowHeartbeatRetryCount,ShadowHeartbeatTimeoutInterval,ShadowMessageAutoDiscardInterval,ShadowMessagePreferenceSetting,ShadowRedundancyEnabled,ShadowResubmitTimeSpan,SupervisionTags,TLSReceiveDomainSecureList,TLSSendDomainSecureList,TransportRuleAttachmentTextScanLimit,Verbose,VerifySecureSubmitEnabled,VoicemailJournalingEnabled,WarningAction,WarningVariable,WhatIf,Xexch50Enabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UserIdentity,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class POP3_And_IMAP4_Protocols
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ImapSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PopSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallRouterSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ImapSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuthenticatedConnectionTimeout,Banner,CalendarItemRetrievalOption,Confirm,Debug,DomainController,EnableExactRFC822Size,EnableGSSAPIAndNTLMAuth,EnforceCertificateErrors,ErrorAction,ErrorVariable,ExtendedProtectionPolicy,ExternalConnectionSettings,InternalConnectionSettings,LogFileLocation,LogFileRollOverSettings,LogPerFileSizeQuota,LoginType,MaxCommandSize,MaxConnectionFromSingleIP,MaxConnections,MaxConnectionsPerUser,MessageRetrievalMimeFormat,OutBuffer,OutVariable,OwaServerUrl,PreAuthenticatedConnectionTimeout,ProtocolLogEnabled,ProxyTargetPort,SSLBindings,Server,ShowHiddenFoldersEnabled,SuppressReadReceipt,UnencryptedOrTLSBindings,Verbose,WarningAction,WarningVariable,WhatIf,X509CertificateName")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PopSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AuthenticatedConnectionTimeout,Banner,CalendarItemRetrievalOption,Confirm,Debug,DomainController,EnableExactRFC822Size,EnableGSSAPIAndNTLMAuth,EnforceCertificateErrors,ErrorAction,ErrorVariable,ExtendedProtectionPolicy,ExternalConnectionSettings,InternalConnectionSettings,LogFileLocation,LogFileRollOverSettings,LogPerFileSizeQuota,LoginType,MaxCommandSize,MaxConnectionFromSingleIP,MaxConnections,MaxConnectionsPerUser,MessageRetrievalMimeFormat,MessageRetrievalSortOrder,OutBuffer,OutVariable,OwaServerUrl,PreAuthenticatedConnectionTimeout,ProtocolLogEnabled,ProxyTargetPort,SSLBindings,Server,SuppressReadReceipt,UnencryptedOrTLSBindings,Verbose,WarningAction,WarningVariable,WhatIf,X509CertificateName")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Public_Folders
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AccessRights,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Server,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,User,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderItemStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeDumpsterInfo,IncludeHierarchyInfo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "Debug,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"PublicFoldersEnabled"
					}, "HoldForMigration,IsExcludedFromServingHierarchy,PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RecipientManagementPermissions"
					}, "Name,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,EformsLocaleId,ErrorAction,ErrorVariable,Mailbox,Name,OutBuffer,OutVariable,Path,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CSVData,CSVStream,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,InternalFlags,LargeItemLimit,Name,OutBuffer,OutVariable,Priority,SkipMerging,SourceDatabase,Suspend,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,AllowLargeItems,BadItemLimit,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Folders,InternalFlags,Name,OutBuffer,OutVariable,Priority,Suspend,SuspendComment,SuspendWhenReadyToComplete,TargetMailbox,Verbose,WarningAction,WarningVariable,WhatIf,WorkloadType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recurse,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "DefaultPublicFolderMailbox,IsExcludedFromServingHierarchy,IsHierarchyReady,PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "DefaultPublicFolderAgeLimit,DefaultPublicFolderDeletedItemRetention,DefaultPublicFolderIssueWarningQuota,DefaultPublicFolderMaxItemSize,DefaultPublicFolderMovedItemRetention,DefaultPublicFolderProhibitPostQuota,PublicFolderMigrationComplete,PublicFoldersEnabled,PublicFoldersLockedForMigration,RemotePublicFolderMailboxes")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AgeLimit,Confirm,Debug,DomainController,EformsLocaleId,ErrorAction,ErrorVariable,Force,Identity,IssueWarningQuota,MailEnabled,MailRecipientGuid,MaxItemSize,Name,OutBuffer,OutVariable,OverrideContentMailbox,Path,PerUserReadStateEnabled,ProhibitPostQuota,RetainDeletedItemsFor,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "InternalFlags"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,BadItemLimit,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,LargeItemLimit,OutBuffer,OutVariable,Priority,RehomeRequest,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "InternalFlags"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,LargeItemLimit,OutBuffer,OutVariable,PreventCompletion,Priority,RehomeRequest,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,BadItemLimit,CompletedRequestAgeLimit,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,InternalFlags,LargeItemLimit,OutBuffer,OutVariable,Priority,SuspendWhenReadyToComplete,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-PublicFolderMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FullSync,Identity,InvokeSynchronizer,OutBuffer,OutVariable,ReconcileFolders,SuppressStatus,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Receive_Connectors
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AdvertiseClientSettings,AuthMechanism,Banner,BinaryMimeEnabled,Bindings,ChunkingEnabled,Client,Comment,Confirm,ConnectionInactivityTimeout,ConnectionTimeout,Custom,Debug,DefaultDomain,DeliveryStatusNotificationEnabled,DomainController,DomainSecureEnabled,EightBitMimeEnabled,EnableAuthGSSAPI,Enabled,EnhancedStatusCodesEnabled,ErrorAction,ErrorVariable,ExtendedProtectionPolicy,Fqdn,Internal,Internet,LongAddressesEnabled,MaxAcknowledgementDelay,MaxHeaderSize,MaxHopCount,MaxInboundConnection,MaxInboundConnectionPerSource,MaxInboundConnectionPercentagePerSource,MaxLocalHopCount,MaxLogonFailures,MaxMessageSize,MaxProtocolErrors,MaxRecipientsPerMessage,MessageRateLimit,MessageRateSource,Name,OrarEnabled,OutBuffer,OutVariable,Partner,PermissionGroups,PipeliningEnabled,ProtocolLoggingLevel,RemoteIPRanges,RequireEHLODomain,RequireTLS,Server,ServiceDiscoveryFqdn,SizeEnabled,SuppressXAnonymousTls,TarpitInterval,TlsCertificateName,TlsDomainCapabilities,TlsSenderCertificateName,TransportRole,Usage,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AdvertiseClientSettings,AuthMechanism,Banner,BareLinefeedRejectionEnabled,BinaryMimeEnabled,Bindings,ChunkingEnabled,Comment,Confirm,ConnectionInactivityTimeout,ConnectionTimeout,Debug,DefaultDomain,DeliveryStatusNotificationEnabled,DomainController,DomainSecureEnabled,EightBitMimeEnabled,EnableAuthGSSAPI,Enabled,EnhancedStatusCodesEnabled,ErrorAction,ErrorVariable,ExtendedProtectionPolicy,Fqdn,LongAddressesEnabled,MaxAcknowledgementDelay,MaxHeaderSize,MaxHopCount,MaxInboundConnection,MaxInboundConnectionPerSource,MaxInboundConnectionPercentagePerSource,MaxLocalHopCount,MaxLogonFailures,MaxMessageSize,MaxProtocolErrors,MaxRecipientsPerMessage,MessageRateLimit,MessageRateSource,Name,OrarEnabled,OutBuffer,OutVariable,PermissionGroups,PipeliningEnabled,ProtocolLoggingLevel,RemoteIPRanges,RequireEHLODomain,RequireTLS,ServiceDiscoveryFqdn,SizeEnabled,SmtpUtf8Enabled,SuppressXAnonymousTls,TarpitInterval,TlsCertificateName,TlsDomainCapabilities,TlsSenderCertificateName,TransportRole,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Recipient_Policies
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Monitoring")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ProtocolSettings"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DetailsTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Monitoring")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Explicit,Identity,OutBuffer,OutVariable,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,SortBy,ThrottlingPolicy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,ExternalIPAddresses,Features,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnonymousCutoffBalance,AnonymousMaxBurst,AnonymousMaxConcurrency,AnonymousRechargeRate,ComplianceMaxExpansionDGRecipients,ComplianceMaxExpansionNestedDGs,Confirm,CpaCutoffBalance,CpaMaxBurst,CpaMaxConcurrency,CpaRechargeRate,Debug,DiscoveryMaxConcurrency,DiscoveryMaxKeywords,DiscoveryMaxKeywordsPerPage,DiscoveryMaxMailboxes,DiscoveryMaxMailboxesResultsOnly,DiscoveryMaxPreviewSearchMailboxes,DiscoveryMaxRefinerResults,DiscoveryMaxSearchQueueDepth,DiscoveryMaxStatsSearchMailboxes,DiscoveryPreviewSearchResultsPageSize,DiscoverySearchTimeoutPeriod,DomainController,EasCutoffBalance,EasMaxBurst,EasMaxConcurrency,EasMaxDeviceDeletesPerMonth,EasMaxDevices,EasMaxInactivityForDeviceCleanup,EasRechargeRate,ErrorAction,ErrorVariable,EwsCutoffBalance,EwsMaxBurst,EwsMaxConcurrency,EwsMaxSubscriptions,EwsRechargeRate,ExchangeMaxCmdlets,ForwardeeLimit,ImapCutoffBalance,ImapMaxBurst,ImapMaxConcurrency,ImapRechargeRate,IsServiceAccount,MessageRateLimit,Name,OutBuffer,OutVariable,OutlookServiceCutoffBalance,OutlookServiceMaxBurst,OutlookServiceMaxConcurrency,OutlookServiceMaxSocketConnectionsPerDevice,OutlookServiceMaxSocketConnectionsPerUser,OutlookServiceMaxSubscriptions,OutlookServiceRechargeRate,OwaCutoffBalance,OwaMaxBurst,OwaMaxConcurrency,OwaRechargeRate,OwaVoiceCutoffBalance,OwaVoiceMaxBurst,OwaVoiceMaxConcurrency,OwaVoiceRechargeRate,PopCutoffBalance,PopMaxBurst,PopMaxConcurrency,PopRechargeRate,PowerShellCutoffBalance,PowerShellMaxBurst,PowerShellMaxCmdletQueueDepth,PowerShellMaxCmdlets,PowerShellMaxCmdletsTimePeriod,PowerShellMaxConcurrency,PowerShellMaxDestructiveCmdlets,PowerShellMaxDestructiveCmdletsTimePeriod,PowerShellMaxOperations,PowerShellMaxRunspaces,PowerShellMaxRunspacesTimePeriod,PowerShellMaxTenantConcurrency,PowerShellMaxTenantRunspaces,PowerShellRechargeRate,PswsMaxConcurrency,PswsMaxRequest,PswsMaxRequestTimePeriod,PushNotificationCutoffBalance,PushNotificationMaxBurst,PushNotificationMaxBurstPerDevice,PushNotificationMaxConcurrency,PushNotificationRechargeRate,PushNotificationRechargeRatePerDevice,PushNotificationSamplingPeriodPerDevice,RcaCutoffBalance,RcaMaxBurst,RcaMaxConcurrency,RcaRechargeRate,RecipientRateLimit,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOStandardRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "EncryptionRecipientCutoffBalance,EncryptionRecipientMaxBurst,EncryptionRecipientMaxConcurrency,EncryptionRecipientRechargeRate,EncryptionSenderCutoffBalance,EncryptionSenderMaxBurst,EncryptionSenderMaxConcurrency,EncryptionSenderRechargeRate")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServers,Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,Name,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,InstantMessagingType,JunkEmailEnabled,SMimeEnabled,UNCAccessOnPrivateComputersEnabled,UNCAccessOnPublicComputersEnabled,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,JournalEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PremiumClientEnabled,PublicFoldersEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnonymousCutoffBalance,AnonymousMaxBurst,AnonymousMaxConcurrency,AnonymousRechargeRate,ComplianceMaxExpansionDGRecipients,ComplianceMaxExpansionNestedDGs,Confirm,CpaCutoffBalance,CpaMaxBurst,CpaMaxConcurrency,CpaRechargeRate,Debug,DiscoveryMaxConcurrency,DiscoveryMaxKeywords,DiscoveryMaxKeywordsPerPage,DiscoveryMaxMailboxes,DiscoveryMaxMailboxesResultsOnly,DiscoveryMaxPreviewSearchMailboxes,DiscoveryMaxRefinerResults,DiscoveryMaxSearchQueueDepth,DiscoveryMaxStatsSearchMailboxes,DiscoveryPreviewSearchResultsPageSize,DiscoverySearchTimeoutPeriod,DomainController,EasCutoffBalance,EasMaxBurst,EasMaxConcurrency,EasMaxDeviceDeletesPerMonth,EasMaxDevices,EasMaxInactivityForDeviceCleanup,EasRechargeRate,ErrorAction,ErrorVariable,EwsCutoffBalance,EwsMaxBurst,EwsMaxConcurrency,EwsMaxSubscriptions,EwsRechargeRate,ExchangeMaxCmdlets,Force,ForwardeeLimit,ImapCutoffBalance,ImapMaxBurst,ImapMaxConcurrency,ImapRechargeRate,IsServiceAccount,MessageRateLimit,Name,OutBuffer,OutVariable,OutlookServiceCutoffBalance,OutlookServiceMaxBurst,OutlookServiceMaxConcurrency,OutlookServiceMaxSocketConnectionsPerDevice,OutlookServiceMaxSocketConnectionsPerUser,OutlookServiceMaxSubscriptions,OutlookServiceRechargeRate,OwaCutoffBalance,OwaMaxBurst,OwaMaxConcurrency,OwaRechargeRate,OwaVoiceCutoffBalance,OwaVoiceMaxBurst,OwaVoiceMaxConcurrency,OwaVoiceRechargeRate,PopCutoffBalance,PopMaxBurst,PopMaxConcurrency,PopRechargeRate,PowerShellCutoffBalance,PowerShellMaxBurst,PowerShellMaxCmdletQueueDepth,PowerShellMaxCmdlets,PowerShellMaxCmdletsTimePeriod,PowerShellMaxConcurrency,PowerShellMaxDestructiveCmdlets,PowerShellMaxDestructiveCmdletsTimePeriod,PowerShellMaxOperations,PowerShellMaxRunspaces,PowerShellMaxRunspacesTimePeriod,PowerShellMaxTenantConcurrency,PowerShellMaxTenantRunspaces,PowerShellRechargeRate,PswsMaxConcurrency,PswsMaxRequest,PswsMaxRequestTimePeriod,PushNotificationCutoffBalance,PushNotificationMaxBurst,PushNotificationMaxBurstPerDevice,PushNotificationMaxConcurrency,PushNotificationRechargeRate,PushNotificationRechargeRatePerDevice,PushNotificationSamplingPeriodPerDevice,RcaCutoffBalance,RcaMaxBurst,RcaMaxConcurrency,RcaRechargeRate,RecipientRateLimit,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOStandardRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "EncryptionRecipientCutoffBalance,EncryptionRecipientMaxBurst,EncryptionRecipientMaxConcurrency,EncryptionRecipientRechargeRate,EncryptionSenderCutoffBalance,EncryptionSenderMaxBurst,EncryptionSenderMaxConcurrency,EncryptionSenderRechargeRate")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ThrottlingPolicy,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Remote_and_Accepted_Domains
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-X400AuthoritativeDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AcceptedDomains",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,DomainName,DomainType,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-X400AuthoritativeDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf,X400DomainName,X400ExternalRelay")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AcceptedDomains",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-X400AuthoritativeDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "AddressBookEnabled,Confirm,Debug,DomainController,DomainType,ErrorAction,ErrorVariable,MakeDefault,Name,OutBuffer,OutVariable,PendingCompletion,PendingRemoval,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ByteEncoderTypeFor7BitCharsets,PreferredInternetCodePageForShiftJis,RequiredCharsetCoverage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowedOOFType,AutoForwardEnabled,AutoReplyEnabled,ByteEncoderTypeFor7BitCharsets,CharacterSet,Confirm,ContentType,Debug,DeliveryReportEnabled,DisplaySenderName,DomainController,ErrorAction,ErrorVariable,Identity,IsInternal,LineWrapSize,MeetingForwardNotificationEnabled,MessageCountThreshold,NDRDiagnosticInfoEnabled,NDREnabled,Name,NonMimeCharacterSet,OutBuffer,OutVariable,PreferredInternetCodePageForShiftJis,RequiredCharsetCoverage,TNEFEnabled,TargetDeliveryDomain,TrustedMailInboundEnabled,TrustedMailOutboundEnabled,UseSimpleDisplayName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-X400AuthoritativeDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf,X400DomainName,X400ExternalRelay")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Reset_Password
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "RoomMailboxPassword"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon")
				}, "c")
			};
		}

		private class Retention_Management
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MailboxDiagnosticLogs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,ComponentName,Confirm,Credential,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProperties,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticAnalysis", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CalendarLogs,Credential,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,GlobalObjectId,LogLocation,OutBuffer,OutVariable,OutputAs,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,Identity,Latest,LogLocation,MeetingID,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,StartDate,Subject,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IncludeHidden")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeSystemTags,Mailbox,OptionalInMailbox,OutBuffer,OutVariable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "EnableRoomMailboxAccount")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgeLimitForRetention,Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,LocalizedComment,LocalizedRetentionPolicyTagName,ManagedFolderToUpgrade,MessageClass,MustDisplayCommentEnabled,Name,OutBuffer,OutVariable,RetentionAction,RetentionEnabled,RetentionId,SystemTag,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,LitigationHoldEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,EnableRoomMailboxAccount,EndDateForRetentionHold,ErrorAction,ErrorVariable,OutBuffer,OutVariable,RetentionHoldEnabled,RetentionPolicy,SingleItemRecoveryEnabled,StartDateForRetentionHold,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ManagedFoldersPermissions"
					}, "RemoveManagedFolderAndPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgeLimitForRetention,Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,LegacyManagedFolder,LocalizedComment,LocalizedRetentionPolicyTagName,Mailbox,MessageClass,MustDisplayCommentEnabled,Name,OptionalInMailbox,OutBuffer,OutVariable,RetentionAction,RetentionEnabled,RetentionId,SystemTag,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-ManagedFolderAssistant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,HoldCleanup,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-RetentionAutoTagLearning", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Clear,Confirm,CrossValidate,Debug,DomainController,ErrorAction,ErrorVariable,Identity,NumberOfSegments,OutBuffer,OutVariable,Train,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-ManagedFolderAssistant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Role_Management
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Overwrite,PSSnapinName,Parameters,ParentRoleEntry,Role,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Cmdlet,CmdletParameters,Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,Verbose,WarningAction,WarningVariable,WritableDatabase,WritableRecipient,WritableServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PSSnapinName,Parameters,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Exclusive,Identity,Orphan,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,ShowPartnerLinked,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Parent,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Computer,Confirm,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,ErrorAction,ErrorVariable,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,Name,OutBuffer,OutVariable,Policy,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,Role,SecurityGroup,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,DatabaseList,DatabaseRestrictionFilter,Debug,DomainController,ErrorAction,ErrorVariable,Exclusive,Force,Name,OutBuffer,OutVariable,RecipientRestrictionFilter,RecipientRoot,ServerList,ServerRestrictionFilter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,IsDefault,Name,OutBuffer,OutVariable,Roles,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Description,DisplayName,DomainController,ErrorAction,ErrorVariable,LinkedCredential,LinkedDomainController,LinkedForeignGroup,ManagedBy,Name,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,Roles,SamAccountName,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions",
						"RoleGroupMembershipRestrictions"
					}, "Members")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recurse,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions",
						"RoleGroupMembershipRestrictions"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "AddParameter,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Parameters,RemoveParameter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,DatabaseRestrictionFilter,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Name,OutBuffer,OutVariable,RecipientRestrictionFilter,RecipientRoot,ServerRestrictionFilter,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Identity,IsDefault,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,Description,DisplayName,DomainController,ErrorAction,ErrorVariable,LinkedCredential,LinkedDomainController,LinkedForeignGroup,ManagedBy,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Security_Group_Creation_and_Membership
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Identity,Member")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Identity,IgnoreDefaultScope")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,SamAccountName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArbitrationMailbox,Debug,DomainController,MemberDepartRestriction,OrganizationalUnit,RoomList,Type,Verbose"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassNestedModerationEnabled"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions",
						"ResourceMailboxRestrictions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Identity,Member")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Identity,RoomList")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassSecurityGroupManagerCheck")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Identity,Members")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Send_Connectors
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IntraOrgConnectorProtocolLoggingLevel,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SendConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SendConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddressSpaces,AuthenticationCredential,CloudServicesMailEnabled,Comment,Confirm,ConnectionInactivityTimeOut,Custom,DNSRoutingEnabled,Debug,DomainController,DomainSecureEnabled,Enabled,ErrorAction,ErrorPolicies,ErrorVariable,Force,ForceHELO,Fqdn,FrontendProxyEnabled,IgnoreSTARTTLS,Internal,Internet,IsScopedConnector,MaxMessageSize,Name,OutBuffer,OutVariable,Partner,Port,ProtocolLoggingLevel,RequireOorg,RequireTLS,SmartHostAuthMechanism,SmartHosts,SmtpMaxMessagesPerConnection,SourceIPAddress,SourceTransportServers,TlsAuthLevel,TlsCertificateName,TlsDomain,Usage,UseExternalDNSServersEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SendConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SendConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AddressSpaces,AuthenticationCredential,CloudServicesMailEnabled,Comment,Confirm,ConnectionInactivityTimeOut,DNSRoutingEnabled,Debug,DomainController,DomainSecureEnabled,Enabled,ErrorAction,ErrorPolicies,ErrorVariable,Force,ForceHELO,Fqdn,FrontendProxyEnabled,IgnoreSTARTTLS,IsScopedConnector,MaxMessageSize,Name,OutBuffer,OutVariable,Port,ProtocolLoggingLevel,RequireOorg,RequireTLS,SmartHostAuthMechanism,SmartHosts,SmtpMaxMessagesPerConnection,SourceIPAddress,SourceTransportServers,TlsAuthLevel,TlsCertificateName,TlsDomain,UseExternalDNSServersEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Support_Diagnostics
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MailboxDiagnosticLogs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,ComponentName,Confirm,Credential,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProperties,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AgentLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,EndDate,ErrorAction,ErrorVariable,Location,OutBuffer,OutVariable,StartDate,TransportService,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticAnalysis", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CalendarLogs,Credential,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,GlobalObjectId,LogLocation,OutBuffer,OutVariable,OutputAs,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,Identity,Latest,LogLocation,MeetingID,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,StartDate,Subject,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FailedContentIndexDocuments", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorCode,ErrorVariable,FailureMode,Identity,MailboxDatabase,OutBuffer,OutVariable,ResultSize,Server,StartDate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IncludeHidden")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QueueDigest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Dag,Debug,DetailsLevel,ErrorAction,ErrorVariable,Filter,Forest,GroupBy,OutBuffer,OutVariable,ResultSize,Server,Site,Timeout,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-StoreUsageStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludePassive,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Extension,Identity,MimeType,Name,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-CalendarValidationResult", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,FailureCategoryType,Identity,IncludeAnalysis,IntervalEndDate,IntervalStartDate,Location,MaxThreads,MeetingID,OnlyReportErrors,OutBuffer,OutVariable,Subject")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-DatabaseEvent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "CopyOnServer,Debug,DomainController,ErrorAction,ErrorVariable,EventNames,Identity,IncludeMoveDestinationEvents,IncludePassive,MailboxGuid,OutBuffer,OutVariable,ResultSize,Server,StartCounter,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-DatabaseEventWatermark", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ConsumerGuid,CopyOnServer,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludePassive,MailboxGuid,OutBuffer,OutVariable,ResultSize,Server,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-ExchangeDiagnosticInfo", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Argument,AsJob,Component,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Process,Server,Unlimited,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-FolderRestriction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Database,Debug,DomainController,ErrorAction,ErrorVariable,FolderEntryId,MailboxGuid,OutBuffer,OutVariable,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-WebDnsRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Thumbprint,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Repair-Migration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BatchId,CacheEntry,Confirm,Debug,ErrorAction,ErrorVariable,FlushSubscription,FolderId,Force,OutBuffer,OutVariable,Remove,RemoveSyncSubscription,ReportId,ResumeSubscription,Revert,Status,SyncSubscription,Update,UpdateSyncSubscription,UpgradeConstraint,UserId,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Test-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Arbitration,Confirm,Debug,DeliverMessage,ErrorAction,ErrorVariable,InboxRules,MessageFileData,Options,OutBuffer,OutVariable,Recipients,SendReportTo,Sender,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Team_Mailboxes
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,BypassOwnerCheck,Debug,DeletedSiteMailbox,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Alias,Database,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,Force,Name,OrganizationalUnit,OutBuffer,OutVariable,OverrideRecipientQuotas,SharePointUrl,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "AliasPrefix,Confirm,Debug,DefaultAliasPrefixEnabled,DomainController,ErrorAction,ErrorVariable,IsDefault,IssueWarningQuota,MaxReceiveSize,Name,OutBuffer,OutVariable,ProhibitSendReceiveQuota,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "SiteMailboxCreationURL")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Active,Debug,DisplayName,DomainController,ErrorAction,ErrorVariable,Force,Members,OutBuffer,OutVariable,Owners,RemoveDuplicateMessages,SharePointUrl,ShowInMyClient,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "AliasPrefix,Confirm,Debug,DefaultAliasPrefixEnabled,DomainController,ErrorAction,ErrorVariable,Identity,IsDefault,IssueWarningQuota,MaxReceiveSize,Name,OutBuffer,OutVariable,ProhibitSendReceiveQuota,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestorIdentity,UseAppTokenOnly,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,FullSync,Identity,OutBuffer,OutVariable,Server,Target,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Agents
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TransportService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TransportService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TransportService,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportPipeline", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Install-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssemblyPath,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,TransportAgentFactory,TransportService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Priority,TransportService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Uninstall-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,TransportService,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Hygiene
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ContentFilterPhrase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Influence,OutBuffer,OutVariable,Phrase,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-IPAllowListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,ErrorAction,ErrorVariable,ExpirationTime,IPAddress,IPRange,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnyMatch,BitmaskMatch,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IPAddressesMatch,LookupDomain,Name,OutBuffer,OutVariable,Priority,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-IPBlockListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,ErrorAction,ErrorVariable,ExpirationTime,IPAddress,IPRange,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnyMatch,BitmaskMatch,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IPAddressesMatch,LookupDomain,Name,OutBuffer,OutVariable,Priority,RejectionResponse,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-AntispamUpdates", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SpamSignatureUpdatesEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AgentLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,EndDate,ErrorAction,ErrorVariable,Location,OutBuffer,OutVariable,StartDate,TransportService,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ContentFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ContentFilterPhrase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Phrase,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,IntraOrgConnectorProtocolLoggingLevel,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,ResultSize,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,ResultSize,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilteringServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderIdConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderReputationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Action,AdminDisplayName,BypassInboundMessages,BypassOutboundMessages,Confirm,CustomAlertText,CustomExternalBody,CustomExternalSubject,CustomFromAddress,CustomFromName,CustomInternalBody,CustomInternalSubject,CustomNotifications,Debug,DomainController,EnableExternalSenderAdminNotifications,EnableExternalSenderNotifications,EnableInternalSenderAdminNotifications,EnableInternalSenderNotifications,ErrorAction,ErrorVariable,ExternalSenderAdminAddress,InternalSenderAdminAddress,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,MalwareFilterPolicy,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ContentFilterPhrase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Phrase,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IPAllowListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IPBlockListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ContentFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassedRecipients,BypassedSenderDomains,BypassedSenders,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,OutlookEmailPostmarkValidationEnabled,QuarantineMailbox,RejectionResponse,SCLDeleteEnabled,SCLDeleteThreshold,SCLQuarantineEnabled,SCLQuarantineThreshold,SCLRejectEnabled,SCLRejectThreshold,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AntispamAgentsEnabled,Confirm,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPAllowListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnyMatch,BitmaskMatch,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IPAddressesMatch,LookupDomain,Name,OutBuffer,OutVariable,Priority,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPAllowListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPBlockListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,MachineEntryRejectionResponse,OutBuffer,OutVariable,StaticEntryRejectionResponse,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnyMatch,BitmaskMatch,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IPAddressesMatch,LookupDomain,Name,OutBuffer,OutVariable,Priority,RejectionResponse,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-IPBlockListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassedRecipients,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Action,AdminDisplayName,BypassInboundMessages,BypassOutboundMessages,Confirm,CustomAlertText,CustomExternalBody,CustomExternalSubject,CustomFromAddress,CustomFromName,CustomInternalBody,CustomInternalSubject,CustomNotifications,Debug,DomainController,EnableExternalSenderAdminNotifications,EnableExternalSenderNotifications,EnableInternalSenderAdminNotifications,EnableInternalSenderNotifications,ErrorAction,ErrorVariable,ExternalSenderAdminAddress,Identity,InternalSenderAdminAddress,MakeDefault,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,Identity,MalwareFilterPolicy,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilteringServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassFiltering,Confirm,Debug,DeferAttempts,DeferWaitTime,DomainController,ErrorAction,ErrorVariable,ForceRescan,MinimumSuccessfulEngineScans,OutBuffer,OutVariable,PrimaryUpdatePath,ScanErrorAction,ScanTimeout,SecondaryUpdatePath,UpdateFrequency,UpdateTimeout,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RecipientFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BlockListEnabled,BlockedRecipients,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,RecipientValidationEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SenderFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Action,BlankSenderBlockingEnabled,BlockedDomains,BlockedDomainsAndSubdomains,BlockedSenders,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,RecipientBlockedSenderAction,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SenderIdConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BypassedRecipients,BypassedSenderDomains,Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OutBuffer,OutVariable,SpoofedDomainAction,TempErrorAction,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SenderReputationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExternalMailEnabled,InternalMailEnabled,OpenProxyDetectionEnabled,OutBuffer,OutVariable,ProxyServerName,ProxyServerPort,ProxyServerType,SenderBlockingEnabled,SenderBlockingPeriod,SrlBlockThreshold,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,Confirm,ErrorAction,ErrorVariable,LocalOnly,OutBuffer,OutVariable,RemoteOnly,Requester,State,TimeoutInSeconds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AntispamAgentsEnabled,Confirm,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AntispamAgentsEnabled,Confirm,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SenderId", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,HelloDomain,IPAddress,OutBuffer,OutVariable,PurportedResponsibleDomain,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SafeList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,EnsureJunkEmailRule,ErrorAction,ErrorVariable,Identity,IncludeDomains,OutBuffer,OutVariable,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Queues
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BookmarkIndex,BookmarkObject,Debug,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmark,IncludeComponentLatencyInfo,IncludeRecipientInfo,OutBuffer,OutVariable,Queue,ResultSize,ReturnPageInfo,SearchForward,Server,SortOrder,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BookmarkIndex,BookmarkObject,Debug,ErrorAction,ErrorVariable,Exclude,Filter,Identity,Include,IncludeBookmark,OutBuffer,OutVariable,ResultSize,ReturnPageInfo,SearchForward,Server,SortOrder,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Redirect-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Target,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf,WithNDR")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Retry-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Resubmit,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Rules
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mode,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-JournalRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-JournalRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClassificationID,Confirm,Debug,DisplayName,DisplayPrecedence,DomainController,ErrorAction,ErrorVariable,Locale,Name,OutBuffer,OutVariable,PermissionMenuVisible,RecipientDescription,RetainClassificationEnabled,SenderDescription,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"IRMPremiumFeaturesPermissions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyRightsProtectionTemplate"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,Debug,DeleteMessage,Disconnect,DomainController,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,Verbose,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClassificationID,Confirm,Debug,DisplayName,DisplayPrecedence,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,PermissionMenuVisible,RecipientDescription,RetainClassificationEnabled,SenderDescription,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"IRMPremiumFeaturesPermissions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyRightsProtectionTemplate,Identity"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,Debug,DeleteMessage,Disconnect,DomainController,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,Verbose,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Test-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,WhatIf"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "TransportRules")
				}, "c")
			};
		}

		private class UM_Mailboxes
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,KeepProperties,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AutomaticSpeechRecognitionEnabled,Confirm,ErrorAction,ErrorVariable,Extensions,Identity,NotifyEmail,OutBuffer,OutVariable,PilotNumber,Pin,PinExpired,SIPResourceIdentifier,UMMailboxPolicy,ValidateOnly,VoiceMailAnalysisEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,KeyMappings,Mailbox,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AllowUMCallsFromNonUsers,CreateDTMFMap,UMCallingLineIds"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "CreateDTMFMap,Identity"),
					new RoleParameters(new string[]
					{
						"ResourceMailboxRestrictions",
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreateDTMFMap"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreateDTMFMap,SecondaryDialPlan"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreateDTMFMap,SecondaryDialPlan"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,IgnoreDefaultScope,ImListMigrationCompleted,OutBuffer,OutVariable,SecondaryAddress,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "SecondaryDialPlan"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CreateDTMFMap,UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,Identity,KeyMappings,Mailbox,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMFaxPermissions"
					}, "FaxEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMOutDialingPermissions"
					}, "PlayOnPhoneEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "OperatorNumber,UMMailboxPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "AllowUMCallsFromNonUsers,AnonymousCallersCanLeaveMessages,AutomaticSpeechRecognitionEnabled,CallAnsweringAudioCodec,Confirm,ErrorAction,ErrorVariable,ImListMigrationCompleted,MissedCallNotificationEnabled,Name,OutBuffer,OutVariable,PinlessAccessToVoiceMailEnabled,SubscriberAccessEnabled,TUIAccessToCalendarEnabled,TUIAccessToEmailEnabled,VoiceMailAnalysisEnabled,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPersonalAutoAttendantPermissions"
					}, "CallAnsweringRulesEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMSMSMsgWaitingPermissions"
					}, "UMSMSNotificationOption")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,DefaultPlayOnPhoneNumber,ErrorAction,ErrorVariable,FolderToReadEmailsFrom,Greeting,Identity,OutBuffer,OutVariable,ReadOldestUnreadVoiceMessagesFirst,ReceivedVoiceMailPreviewEnabled,SentVoiceMailPreviewEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,LockedOut,NotifyEmail,OutBuffer,OutVariable,Pin,PinExpired,SendEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PhoneticDisplayName,SeniorityIndex"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "Confirm,Debug,DomainController,IgnoreDefaultScope,PublicFolder,UMCallingLineIds,Verbose,WhatIf"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "AllowUMCallsFromNonUsers,CreateDTMFMap,UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "AwayVoicemailGreeting,CallAnsweringRuleId,Confirm,DefaultVoicemailGreeting,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PhoneNumber,UMMailbox,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class UM_Prompts
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-UMPrompt", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PromptFileName,UMAutoAttendant,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-UMPrompt", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PromptFileData,PromptFileName,PromptFileStream,UMAutoAttendant,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "AfterHoursMainMenuCustomPromptEnabled,AfterHoursMainMenuCustomPromptFilename,AfterHoursWelcomeGreetingEnabled,AfterHoursWelcomeGreetingFilename,BusinessHoursMainMenuCustomPromptEnabled,BusinessHoursMainMenuCustomPromptFilename,BusinessHoursWelcomeGreetingEnabled,BusinessHoursWelcomeGreetingFilename,Confirm,ErrorAction,ErrorVariable,Identity,InfoAnnouncementEnabled,InfoAnnouncementFilename,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,InfoAnnouncementEnabled,InfoAnnouncementFilename,WarningAction,WarningVariable,WelcomeGreetingEnabled,WelcomeGreetingFilename,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class UnScoped_Role_Management
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Overwrite,PSSnapinName,Parameters,ParentRoleEntry,Role,Type,UnScopedTopLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Cmdlet,CmdletParameters,Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,Verbose,WarningAction,WarningVariable,WritableDatabase,WritableRecipient,WritableServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PSSnapinName,Parameters,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,Description,DomainController,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Parent,UnScopedTopLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Computer,Confirm,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,ErrorAction,ErrorVariable,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,Name,OutBuffer,OutVariable,Policy,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,Role,SecurityGroup,UnScopedTopLevel,User,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recurse,UnScopedTopLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "AddParameter,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Parameters,RemoveParameter,UnScopedTopLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Unified_Messaging
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Immediate,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Immediate,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ClientStream,Confirm,Date,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"AddressListsEnabled",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Container,SearchText"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Identity,Instance,OutBuffer,OutVariable,Server,Thumbprint,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMActiveCalls", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DialPlan,DomainController,ErrorAction,ErrorVariable,IPGateway,InstanceServer,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GroupBy,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMHuntGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,IncludeSimulator,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "Confirm,DTMFFallbackAutoAttendant,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,PilotIdentifierList,SpeechEnabled,Status,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMFaxPermissions"
					}, "FaxEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AccessTelephoneNumbers,Confirm,CountryOrRegionCode,DefaultLanguage,ErrorAction,ErrorVariable,GenerateUMMailboxPolicy,Name,NumberOfDigitsInExtension,OutBuffer,OutVariable,URIType,VoIPSecurity,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMHuntGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,PilotIdentifier,UMDialPlan,UMIPGateway,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Address,Confirm,ErrorAction,ErrorVariable,IPAddressFamily,Name,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMHuntGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "AfterHoursKeyMapping,AfterHoursKeyMappingEnabled,AfterHoursMainMenuCustomPromptEnabled,AfterHoursMainMenuCustomPromptFilename,AfterHoursTransferToOperatorEnabled,AfterHoursWelcomeGreetingEnabled,AfterHoursWelcomeGreetingFilename,AllowDialPlanSubscribers,AllowExtensions,AllowedInCountryOrRegionGroups,AllowedInternationalGroups,BusinessHoursKeyMapping,BusinessHoursKeyMappingEnabled,BusinessHoursMainMenuCustomPromptEnabled,BusinessHoursMainMenuCustomPromptFilename,BusinessHoursSchedule,BusinessHoursTransferToOperatorEnabled,BusinessHoursWelcomeGreetingEnabled,BusinessHoursWelcomeGreetingFilename,BusinessLocation,BusinessName,CallSomeoneEnabled,Confirm,ContactAddressList,ContactRecipientContainer,ContactScope,DTMFFallbackAutoAttendant,ErrorAction,ErrorVariable,ForceUpgrade,HolidaySchedule,Identity,InfoAnnouncementEnabled,InfoAnnouncementFilename,Language,MatchedNameSelectionMethod,Name,NameLookupEnabled,OperatorExtension,OutBuffer,OutVariable,PilotIdentifierList,SendVoiceMsgEnabled,SpeechEnabled,TimeZone,TimeZoneName,WarningAction,WarningVariable,WeekStartDay,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallRouterSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DialPlans,DomainController,ErrorAction,ErrorVariable,IPAddressFamily,IPAddressFamilyConfigurable,MaxCallsAllowed,OutBuffer,OutVariable,Server,SipTcpListeningPort,SipTlsListeningPort,UMStartupMode,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMFaxPermissions"
					}, "FaxEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AccessTelephoneNumbers,AllowDialPlanSubscribers,AllowExtensions,AllowHeuristicADCallingLineIdResolution,AllowedInCountryOrRegionGroups,AllowedInternationalGroups,AudioCodec,AutomaticSpeechRecognitionEnabled,CallSomeoneEnabled,ConfiguredInCountryOrRegionGroups,ConfiguredInternationalGroups,Confirm,ContactAddressList,ContactRecipientContainer,ContactScope,CountryOrRegionCode,DefaultLanguage,DialByNamePrimary,DialByNameSecondary,EquivalentDialPlanPhoneContexts,ErrorAction,ErrorVariable,Extension,ForceUpgrade,InCountryOrRegionNumberFormat,InfoAnnouncementEnabled,InfoAnnouncementFilename,InputFailuresBeforeDisconnect,InternationalAccessCode,InternationalNumberFormat,LegacyPromptPublishingPoint,LogonFailuresBeforeDisconnect,MatchedNameSelectionMethod,MaxCallDuration,MaxRecordingDuration,Name,NationalNumberPrefix,NumberingPlanFormats,OperatorExtension,OutBuffer,OutVariable,OutsideLineAccessCode,PilotIdentifierList,RecordingIdleTimeout,SendVoiceMsgEnabled,TUIPromptEditingEnabled,UMAutoAttendant,VoIPSecurity,WarningAction,WarningVariable,WelcomeGreetingEnabled,WelcomeGreetingFilename,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPersonalAutoAttendantPermissions"
					}, "CallAnsweringRulesEnabled")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ForceUpgrade,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Address,Confirm,DelayedSourcePartyInfoEnabled,ErrorAction,ErrorVariable,IPAddressFamily,MessageWaitingIndicatorAllowed,Name,OutBuffer,OutVariable,OutcallsAllowed,Port,Simulator,Status,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMFaxPermissions"
					}, "AllowFax,FaxMessageText,FaxServerURI"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMOutDialingPermissions"
					}, "AllowDialPlanSubscribers,AllowExtensions,AllowedInCountryOrRegionGroups,AllowedInternationalGroups"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AllowAutomaticSpeechRecognition,AllowCommonPatterns,AllowMessageWaitingIndicator,AllowPlayOnPhone,AllowVoiceMailPreview,AllowVoiceResponseToOtherMessageTypes,ForceUpgrade,MaxGreetingDuration,MaxLogonAttempts,MinPINLength,Name,PINHistoryCount,PINLifetime,ProtectAuthenticatedVoiceMail,ProtectUnauthenticatedVoiceMail,ProtectedVoiceMailText,RequireProtectedPlayOnPhone,SourceForestPolicyNames,UMDialPlan,VoiceMailPreviewPartnerAddress,VoiceMailPreviewPartnerAssignedID,VoiceMailPreviewPartnerMaxDeliveryDelay,VoiceMailPreviewPartnerMaxMessageDuration"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "AllowMissedCallNotifications,AllowPinlessVoiceMailAccess,AllowSubscriberAccess,AllowTUIAccessToCalendar,AllowTUIAccessToDirectory,AllowTUIAccessToEmail,AllowTUIAccessToPersonalContacts,AllowVoiceMailAnalysis,Confirm,ErrorAction,ErrorVariable,InformCallerOfVoiceMailAnalysis,LogonFailuresBeforePINReset,OutBuffer,OutVariable,ResetPINText,UMEnabledText,VoiceMailText,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPersonalAutoAttendantPermissions"
					}, "AllowCallAnsweringRules"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMSMSMsgWaitingPermissions"
					}, "AllowSMSNotification")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DialPlans,DomainController,ErrorAction,ErrorVariable,GrammarGenerationSchedule,IPAddressFamily,IPAddressFamilyConfigurable,MaxCallsAllowed,OutBuffer,OutVariable,SIPAccessService,Status,UMStartupMode,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-UMConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CallRouter,CertificateThumbprint,Confirm,Debug,DiagDtmfDurationInMilisecs,DiagDtmfSequence,DiagInitialSilenceInMilisecs,DiagInterDtmfDiffGapInMilisecs,DiagInterDtmfGapInMilisecs,DomainController,ErrorAction,ErrorVariable,From,ListenPort,MediaSecured,MonitoringContext,OutBuffer,OutVariable,PIN,Phone,RemotePort,ResetPIN,Secured,TUILogon,TUILogonAll,Timeout,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class User_Options
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Force")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ActiveSyncDebugLogging,ProtocolSettings,RecalculateHasActiveSyncDevicePartnership"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,VerifyDefaultFolderNameLanguage,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludePassive,IncludeQuarantineDetails,NoADLookup,OutBuffer,OutVariable,Server,StoreMailboxIdentity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync,GetMailboxLog,NotificationEmailAddresses,ShowRecoveryPassword"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DownloadOnly,Etoken,MarketplaceAssetID,MarketplaceQueryMarket,MarketplaceServicesUrl,Url"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AllowReadWriteMailbox,FileData,FileStream,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,ExternalIPAddresses,Features,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,HasAttachment,HasClassification,HeaderContainsWords,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Body,BodyFormat,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Subject,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,KeyMappings,Mailbox,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSyncDebugLogging"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ShowGalAsDefaultView,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapPermissions"
					}, "ImapForceICalForCalendarRetrievalOption,ImapMessagesRetrievalMimeFormat,ImapSuppressReadReceipt,ImapUseProtocolDefaults"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopPermissions"
					}, "PopForceICalForCalendarRetrievalOption,PopMessagesRetrievalMimeFormat,PopSuppressReadReceipt,PopUseProtocolDefaults")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AddAdditionalResponse,AddNewRequestsTentatively,AddOrganizerToSubject,AdditionalResponse,AllBookInPolicy,AllRequestInPolicy,AllRequestOutOfPolicy,AllowConflicts,AllowRecurringMeetings,AutomateProcessing,BookInPolicy,BookingWindowInDays,Confirm,ConflictPercentageAllowed,Debug,DeleteAttachments,DeleteComments,DeleteNonCalendarItems,DeleteSubject,DomainController,EnableResponseDetails,EnforceSchedulingHorizon,ErrorAction,ErrorVariable,ForwardRequestsToDelegates,IgnoreDefaultScope,MaximumConflictInstances,MaximumDurationInMinutes,OrganizerInfo,OutBuffer,OutVariable,RemoveForwardedMeetingNotifications,RemoveOldMeetingMessages,RemovePrivateProperty,RequestInPolicy,RequestOutOfPolicy,ResourceDelegates,ScheduleOnlyDuringWorkHours,TentativePendingApproval,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "ProcessExternalMeetingMessages")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientAccessServers,Confirm,Debug,DomainController,Domains,EdgeTransportServers,ErrorAction,ErrorVariable,ExternalIPAddresses,Features,Name,OnPremisesSmartHost,OutBuffer,OutVariable,ReceivingTransportServers,SendingTransportServers,ServiceInstance,TlsCertificateName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,HasAttachment,HasClassification,HeaderContainsWords,Identity,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,UserCertificate,UserSMimeCertificate"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "MailTip,MailTipTranslations")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "DisplayName,Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,DeliverToMailboxAndForward,ErrorAction,ErrorVariable,ExternalOofOptions,ForwardingSmtpAddress,GrantSendOnBehalfTo,Languages,MessageCopyForSendOnBehalfEnabled,MessageCopyForSentAsEnabled,OutBuffer,OutVariable,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,RoomMailboxPassword,SimpleDisplayName,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "NewPassword,OldPassword,Password"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AutoReplyState,Confirm,Debug,DomainController,EndTime,ErrorAction,ErrorVariable,ExternalAudience,ExternalMessage,IgnoreDefaultScope,OutBuffer,OutVariable,StartTime,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "InternalMessage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DefaultReminderTime,DomainController,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,Verbose,WarningAction,WarningVariable,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OrganizationalAffinityPermissions"
					}, "WorkDays")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PublishDateRangeFrom,PublishDateRangeTo,PublishEnabled,ResetUrl,SearchableUrlEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BlockedSendersAndDomains,Confirm,ContactsTrusted,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,IgnoreDefaultScope,OutBuffer,OutVariable,TrustedListsOnly,TrustedSendersAndDomains,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,Debug,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,DomainController,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,IgnoreDefaultScope,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DateFormat,Debug,DomainController,ErrorAction,ErrorVariable,Language,LocalizeDefaultFolderName,OutBuffer,OutVariable,TimeFormat,TimeZone,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CheckBeforeSend,Confirm,Debug,DictionaryLanguage,DomainController,ErrorAction,ErrorVariable,IgnoreMixedDigits,IgnoreUppercase,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "CallerIds,CallersCanInterruptGreeting,CheckAutomaticReplies,Confirm,ErrorAction,ErrorVariable,ExtensionsDialed,Identity,KeyMappings,Mailbox,Name,OutBuffer,OutVariable,Priority,ScheduleStatus,TimeOfDay,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,DefaultPlayOnPhoneNumber,ErrorAction,ErrorVariable,FolderToReadEmailsFrom,Greeting,Identity,OutBuffer,OutVariable,ReadOldestUnreadVoiceMessagesFirst,ReceivedVoiceMailPreviewEnabled,SentVoiceMailPreviewEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Pin,PinExpired,SendEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PhoneticDisplayName,SeniorityIndex"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"OpenDomainProfileUpdatePermissions",
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "DisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions"
					}, "PostOfficeBox,SimpleDisplayName"),
					new RoleParameters(new string[]
					{
						"ProfileUpdatePermissions",
						"PropertiesMasteredOnPremiseRestrictions"
					}, "City,CountryOrRegion,Fax,FirstName,GeoCoordinates,HomePhone,Initials,LastName,MobilePhone,Notes,Office,Pager,Phone,PostalCode,StateOrProvince,StreetAddress,WebPage"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,PictureData,PictureStream,Preview,Save,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "AwayVoicemailGreeting,CallAnsweringRuleId,Confirm,DefaultVoicemailGreeting,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PhoneNumber,UMMailbox,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-UMPhoneSession", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class View_Only_Audit_Logs
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAuditBypassAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,LogonTypes,Mailboxes,Name,Operations,OutBuffer,OutVariable,ShowDetails,StartDate,StatusMailRecipients,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MailboxAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,LogonTypes,Mailboxes,Operations,OutBuffer,OutVariable,ResultSize,ShowDetails,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class View_Only_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ApplyVersion,Confirm,Debug,DomainController,Duration,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,PropertyValue,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ApplyVersion,Confirm,Debug,Duration,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,PropertyValue,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-ActiveSyncLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,EndDate,ErrorAction,ErrorVariable,Filename,Force,OutBuffer,OutVariable,OutputPath,OutputPrefix,StartDate,UseGMT,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-AutoDiscoverConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DeleteConfig,DomainController,ErrorAction,ErrorVariable,MultipleExchangeDeployments,OutBuffer,OutVariable,PreferredSourceFqdn,SourceForestCredential,TargetForestCredential,TargetForestDomainController,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-DlpPolicyCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-JournalRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ClientStream,Confirm,Date,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,User,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADSite", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAutoblockThreshold", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceClass", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdSiteLink", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"AddressListsEnabled",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Container,SearchText"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AgentLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,EndDate,ErrorAction,ErrorVariable,Location,OutBuffer,OutVariable,StartDate,TransportService,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthRedirect", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AutodiscoverVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessArray", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Site,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeAlternateServiceAccountCredentialStatus,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CmdletExtensionAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Assembly,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ContentFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ContentFilterPhrase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Phrase,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroupConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DatabaseAvailabilityGroupNetwork", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeliveryAgentConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DetailsTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EcpVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EdgeSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EdgeSyncServiceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Site,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EmailAddressPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeMailboxSettingOnlyPolicy,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-EventLogLevel", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeAssistanceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,DomainName,ErrorAction,ErrorVariable,Identity,Instance,OutBuffer,OutVariable,Server,Thumbprint,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,Domain,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicense", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExchangeServerAccessLicenseUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,LicenseName,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FailedContentIndexDocuments", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorCode,ErrorVariable,FailureMode,Identity,MailboxDatabase,OutBuffer,OutVariable,ResultSize,Server,StartDate,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedDomainProof", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DomainName,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Thumbprint,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeExtendedDomainInfo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassAdditionalDomainValidation,Debug,DomainName,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TrustedHostnames,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ForeignConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FrontendTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IntraOrgConnectorProtocolLoggingLevel,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "DefaultOnly,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HealthReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GroupSize,HaImpactingOnly,HealthSet,Identity,MinimumOnlinePercent,OutBuffer,OutVariable,RollupGroup,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,ResultSize,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPAllowListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,IPAddress,Identity,OutBuffer,OutVariable,ResultSize,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IPBlockListProvidersConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ImapSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAuditBypassAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,DumpsterStatistics,ErrorAction,ErrorVariable,Identity,IncludePreExchange2013,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxDatabaseCopyStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Active,ConnectionStatus,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedErrorInfo,Identity,Local,OutBuffer,OutVariable,Server,UseServerCache,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxTransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilteringServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Cmdlet,CmdletParameters,Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomConfigWriteScope,CustomRecipientWriteScope,Debug,Delegating,DomainController,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveConfigWriteScope,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,Verbose,WarningAction,WarningVariable,WritableDatabase,WritableRecipient,WritableServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PSSnapinName,Parameters,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Exclusive,Identity,Orphan,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MapiVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Message", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BookmarkIndex,BookmarkObject,Debug,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmark,IncludeComponentLatencyInfo,IncludeRecipientInfo,OutBuffer,OutVariable,Queue,ResultSize,ReturnPageInfo,SearchForward,Server,SortOrder,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitSkippedItemsTo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MonitoringItemHelp", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MonitoringItemIdentity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-NetworkConnectionInfo", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OabVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookAnywhere", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProvider", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PendingFederatedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PhysicalAvailabilityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DailyStatistics,Database,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExchangeServer,OutBuffer,OutVariable,ReportingDatabase,ReportingPeriod,ReportingServer,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Action,Debug,DomainController,ErrorAction,ErrorVariable,Identity,Locale,Original,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PopSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PowerShellVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderDatabase", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PushNotificationSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,ExpirationTimeInHours,Mailbox,OutBuffer,ResultSize,ShowAll,Verbose,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BookmarkIndex,BookmarkObject,Debug,ErrorAction,ErrorVariable,Exclude,Filter,Identity,Include,IncludeBookmark,OutBuffer,OutVariable,ResultSize,ReturnPageInfo,SearchForward,Server,SortOrder,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QueueDigest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Dag,Debug,DetailsLevel,ErrorAction,ErrorVariable,Filter,Forest,GroupBy,OutBuffer,OutVariable,ResultSize,Server,Site,Timeout,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ReceiveConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResubmitRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeSystemTags,Mailbox,OptionalInMailbox,OutBuffer,OutVariable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,ShowPartnerLinked,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RpcClientAccess", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SendConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderFilterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderIdConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SenderReputationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerComponentState", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Component,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerHealth", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,HaImpactingOnly,HealthSet,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceAvailabilityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DailyStatistics,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportingDatabase,ReportingPeriod,ReportingServer,StartDate,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,MaintenanceWindowDays,OutBuffer,OutVariable,ReportingDatabase,ReportingServer,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SettingOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,BypassOwnerCheck,Debug,DeletedSiteMailbox,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-StoreUsageStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludePassive,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SystemMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Original,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostics,DomainController,ErrorAction,ErrorVariable,Explicit,Identity,OutBuffer,OutVariable,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,SortBy,ThrottlingPolicy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportAgent", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportPipeline", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions"
					}, "DlpPolicy"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Trust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainName,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMActiveCalls", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DialPlan,DomainController,ErrorAction,ErrorVariable,IPGateway,InstanceServer,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallRouterSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GroupBy,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMHuntGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,IncludeSimulator,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMService", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPrincipalNamesSuffix", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OrganizationalUnit,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-WebServicesVirtualDirectory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ADPropertiesOnly,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,ShowMailboxVirtualDirectories,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-X400AuthoritativeDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Invoke-MonitoringProbe", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Account,Debug,Endpoint,ErrorAction,ErrorVariable,Identity,ItemTargetExtension,OutBuffer,OutVariable,Password,PropertyOverride,SecondaryAccount,SecondaryEndpoint,SecondaryPassword,Server,TimeOutSeconds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Enabled,Server,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-GlobalMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Server,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ServerMonitoringOverride", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,ItemType,OutBuffer,OutVariable,PropertyName,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Retry-Queue", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,Resubmit,Server,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "UMAvailableLanguages")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Enabled,Server,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ServerMonitor", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Repairing,Server,TargetResource,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Identity,OWAAllowUserChoiceOfSigningCertificate,OWAAlwaysEncrypt,OWAAlwaysSign,OWABCCEncryptedEmailForking,OWACRLConnectionTimeout,OWACRLRetrievalTimeout,OWACheckCRLOnSend,OWAClearSign,OWACopyRecipientHeaders,OWADLExpansionTimeout,OWADisableCRLCheck,OWAEncryptTemporaryBuffers,OWAEncryptionAlgorithms,OWAForceSMIMEClientUpgrade,OWAIncludeCertificateChainAndRootCertificate,OWAIncludeCertificateChainWithoutRootCertificate,OWAIncludeSMIMECapabilitiesInMessage,OWAOnlyUseSmartCard,OWASenderCertificateAttributesToDisplay,OWASignedEmailCertificateInclusion,OWASigningAlgorithms,OWATripleWrapSignedEncryptedMail,OWAUseKeyIdentifier,OWAUseSecondaryProxiesWhenFindingCertificates,SMIMECertificateIssuingCA,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-FederationTrustCertificate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UserIdentity,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.Powershell.Support", "Get-ExchangeDiagnosticInfo", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Argument,AsJob,Component,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Process,Server,Unlimited,Verbose,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class View_Only_Recipients
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MailboxDiagnosticLogs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,ComponentName,Confirm,Credential,Debug,DomainController,ErrorAction,ErrorVariable,ExtendedProperties,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"AddressListsEnabled",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Container,SearchText"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ActiveSyncDebugLogging,ProtocolSettings,RecalculateHasActiveSyncDevicePartnership"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticAnalysis", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CalendarLogs,Credential,Debug,DetailLevel,DomainController,ErrorAction,ErrorVariable,GlobalObjectId,LogLocation,OutBuffer,OutVariable,OutputAs,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,Identity,Latest,LogLocation,MeetingID,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,StartDate,Subject,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DomainController", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainName,ErrorAction,ErrorVariable,Forest,GlobalCatalog,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IncludeHidden,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LogonStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Database,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Server,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,RemoteArchive,ResultSize,Server,SortBy,Verbose,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxCalendarFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,FolderScope,Identity,IncludeAnalysis,IncludeOldestAndNewestItems,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,ReadFromDomainController,ResultSize,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxRegionalConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,VerifyDefaultFolderNameLanguage,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxSpellingConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CopyOnServer,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeMoveHistory,IncludeMoveReport,IncludePassive,IncludeQuarantineDetails,NoADLookup,OutBuffer,OutVariable,Server,StoreMailboxIdentity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,End,ErrorAction,ErrorVariable,EventId,InternalMessageId,MessageId,MessageSubject,OutBuffer,OutVariable,Recipients,Reference,ResultSize,Sender,Server,Start,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Debug,DetailLevel,DoNotResolve,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientPathFilter,Recipients,ReportTemplate,ResultSize,Status,TraceLevel,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,Debug,DomainController,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitSkippedItemsTo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,Monitoring,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ActiveSync,GetMailboxLog,NotificationEmailAddresses,ShowRecoveryPassword"),
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Flags,HighPriority,Identity,IncludeSoftDeletedObjects,MoveStatus,Offline,OrganizationalUnit,OutBuffer,OutVariable,Protect,RemoteHostName,ResultSize,SortBy,SourceDatabase,Suspend,SuspendWhenReadyToComplete,TargetDatabase,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,MailboxGuid,MoveRequestQueue,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,User,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderItemStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,IncludeDumpsterInfo,IncludeHierarchyInfo,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,Debug,DomainController,ErrorAction,ErrorVariable,HighPriority,Identity,Name,OutBuffer,OutVariable,RequestQueue,ResultSize,Status,Suspend,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,Diagnostic,DiagnosticArgument,DomainController,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,Credential,Database,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,ReadFromDomainController,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OnPremisesOrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ResourceConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,Debug,DomainController,IgnoreDefaultScope,OrganizationalUnit,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,IgnoreDefaultScope,ReadFromDomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Arbitration,Credential,Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ReadFromDomainController,RecipientTypeDetails,ResultSize,SortBy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,DomainController,Verbose"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,Debug,DoNotResolve,DomainController,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,TraceLevel,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ADServerSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ConfigurationDomainController,Confirm,Debug,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PreferredGlobalCatalog,PreferredServer,RecipientViewRoot,RunspaceServerSettings,SetPreferredDomainControllers,Verbose,ViewEntireForest,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class WorkloadManagement
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Diagnostics,DomainController,ErrorAction,ErrorVariable,Explicit,Identity,OutBuffer,OutVariable,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,SortBy,ThrottlingPolicy,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnonymousCutoffBalance,AnonymousMaxBurst,AnonymousMaxConcurrency,AnonymousRechargeRate,ComplianceMaxExpansionDGRecipients,ComplianceMaxExpansionNestedDGs,Confirm,CpaCutoffBalance,CpaMaxBurst,CpaMaxConcurrency,CpaRechargeRate,Debug,DiscoveryMaxConcurrency,DiscoveryMaxKeywords,DiscoveryMaxKeywordsPerPage,DiscoveryMaxMailboxes,DiscoveryMaxMailboxesResultsOnly,DiscoveryMaxPreviewSearchMailboxes,DiscoveryMaxRefinerResults,DiscoveryMaxSearchQueueDepth,DiscoveryMaxStatsSearchMailboxes,DiscoveryPreviewSearchResultsPageSize,DiscoverySearchTimeoutPeriod,DomainController,EasCutoffBalance,EasMaxBurst,EasMaxConcurrency,EasMaxDeviceDeletesPerMonth,EasMaxDevices,EasMaxInactivityForDeviceCleanup,EasRechargeRate,ErrorAction,ErrorVariable,EwsCutoffBalance,EwsMaxBurst,EwsMaxConcurrency,EwsMaxSubscriptions,EwsRechargeRate,ExchangeMaxCmdlets,ForwardeeLimit,ImapCutoffBalance,ImapMaxBurst,ImapMaxConcurrency,ImapRechargeRate,IsServiceAccount,MessageRateLimit,Name,OutBuffer,OutVariable,OutlookServiceCutoffBalance,OutlookServiceMaxBurst,OutlookServiceMaxConcurrency,OutlookServiceMaxSocketConnectionsPerDevice,OutlookServiceMaxSocketConnectionsPerUser,OutlookServiceMaxSubscriptions,OutlookServiceRechargeRate,OwaCutoffBalance,OwaMaxBurst,OwaMaxConcurrency,OwaRechargeRate,OwaVoiceCutoffBalance,OwaVoiceMaxBurst,OwaVoiceMaxConcurrency,OwaVoiceRechargeRate,PopCutoffBalance,PopMaxBurst,PopMaxConcurrency,PopRechargeRate,PowerShellCutoffBalance,PowerShellMaxBurst,PowerShellMaxCmdletQueueDepth,PowerShellMaxCmdlets,PowerShellMaxCmdletsTimePeriod,PowerShellMaxConcurrency,PowerShellMaxDestructiveCmdlets,PowerShellMaxDestructiveCmdletsTimePeriod,PowerShellMaxOperations,PowerShellMaxRunspaces,PowerShellMaxRunspacesTimePeriod,PowerShellMaxTenantConcurrency,PowerShellMaxTenantRunspaces,PowerShellRechargeRate,PswsMaxConcurrency,PswsMaxRequest,PswsMaxRequestTimePeriod,PushNotificationCutoffBalance,PushNotificationMaxBurst,PushNotificationMaxBurstPerDevice,PushNotificationMaxConcurrency,PushNotificationRechargeRate,PushNotificationRechargeRatePerDevice,PushNotificationSamplingPeriodPerDevice,RcaCutoffBalance,RcaMaxBurst,RcaMaxConcurrency,RcaRechargeRate,RecipientRateLimit,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOStandardRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "EncryptionRecipientCutoffBalance,EncryptionRecipientMaxBurst,EncryptionRecipientMaxConcurrency,EncryptionRecipientRechargeRate,EncryptionSenderCutoffBalance,EncryptionSenderMaxBurst,EncryptionSenderMaxConcurrency,EncryptionSenderRechargeRate")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ExchangeServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorReportingEnabled,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ThrottlingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AnonymousCutoffBalance,AnonymousMaxBurst,AnonymousMaxConcurrency,AnonymousRechargeRate,ComplianceMaxExpansionDGRecipients,ComplianceMaxExpansionNestedDGs,Confirm,CpaCutoffBalance,CpaMaxBurst,CpaMaxConcurrency,CpaRechargeRate,Debug,DiscoveryMaxConcurrency,DiscoveryMaxKeywords,DiscoveryMaxKeywordsPerPage,DiscoveryMaxMailboxes,DiscoveryMaxMailboxesResultsOnly,DiscoveryMaxPreviewSearchMailboxes,DiscoveryMaxRefinerResults,DiscoveryMaxSearchQueueDepth,DiscoveryMaxStatsSearchMailboxes,DiscoveryPreviewSearchResultsPageSize,DiscoverySearchTimeoutPeriod,DomainController,EasCutoffBalance,EasMaxBurst,EasMaxConcurrency,EasMaxDeviceDeletesPerMonth,EasMaxDevices,EasMaxInactivityForDeviceCleanup,EasRechargeRate,ErrorAction,ErrorVariable,EwsCutoffBalance,EwsMaxBurst,EwsMaxConcurrency,EwsMaxSubscriptions,EwsRechargeRate,ExchangeMaxCmdlets,Force,ForwardeeLimit,ImapCutoffBalance,ImapMaxBurst,ImapMaxConcurrency,ImapRechargeRate,IsServiceAccount,MessageRateLimit,Name,OutBuffer,OutVariable,OutlookServiceCutoffBalance,OutlookServiceMaxBurst,OutlookServiceMaxConcurrency,OutlookServiceMaxSocketConnectionsPerDevice,OutlookServiceMaxSocketConnectionsPerUser,OutlookServiceMaxSubscriptions,OutlookServiceRechargeRate,OwaCutoffBalance,OwaMaxBurst,OwaMaxConcurrency,OwaRechargeRate,OwaVoiceCutoffBalance,OwaVoiceMaxBurst,OwaVoiceMaxConcurrency,OwaVoiceRechargeRate,PopCutoffBalance,PopMaxBurst,PopMaxConcurrency,PopRechargeRate,PowerShellCutoffBalance,PowerShellMaxBurst,PowerShellMaxCmdletQueueDepth,PowerShellMaxCmdlets,PowerShellMaxCmdletsTimePeriod,PowerShellMaxConcurrency,PowerShellMaxDestructiveCmdlets,PowerShellMaxDestructiveCmdletsTimePeriod,PowerShellMaxOperations,PowerShellMaxRunspaces,PowerShellMaxRunspacesTimePeriod,PowerShellMaxTenantConcurrency,PowerShellMaxTenantRunspaces,PowerShellRechargeRate,PswsMaxConcurrency,PswsMaxRequest,PswsMaxRequestTimePeriod,PushNotificationCutoffBalance,PushNotificationMaxBurst,PushNotificationMaxBurstPerDevice,PushNotificationMaxConcurrency,PushNotificationRechargeRate,PushNotificationRechargeRatePerDevice,PushNotificationSamplingPeriodPerDevice,RcaCutoffBalance,RcaMaxBurst,RcaMaxConcurrency,RcaRechargeRate,RecipientRateLimit,ThrottlingPolicyScope,Verbose,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOStandardRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "EncryptionRecipientCutoffBalance,EncryptionRecipientMaxBurst,EncryptionRecipientMaxConcurrency,EncryptionRecipientRechargeRate,EncryptionSenderCutoffBalance,EncryptionSenderMaxBurst,EncryptionSenderMaxConcurrency,EncryptionSenderRechargeRate")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ThrottlingPolicyAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ThrottlingPolicy,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}
	}
}
