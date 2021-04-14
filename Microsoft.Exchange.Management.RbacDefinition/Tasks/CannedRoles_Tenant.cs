using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class CannedRoles_Tenant
	{
		internal static RoleDefinition[] Definition = new RoleDefinition[]
		{
			new RoleDefinition("Address Lists", RoleType.AddressLists, CannedRoles_Tenant.Address_Lists.Cmdlets),
			new RoleDefinition("ApplicationImpersonation", RoleType.ApplicationImpersonation, CannedRoles_Tenant.ApplicationImpersonation.Cmdlets),
			new RoleDefinition("Audit Logs", RoleType.AuditLogs, CannedRoles_Tenant.Audit_Logs.Cmdlets),
			new RoleDefinition("Data Loss Prevention", RoleType.DataLossPrevention, CannedRoles_Tenant.Data_Loss_Prevention.Cmdlets),
			new RoleDefinition("Distribution Groups", RoleType.DistributionGroups, CannedRoles_Tenant.Distribution_Groups.Cmdlets),
			new RoleDefinition("Federated Sharing", RoleType.FederatedSharing, CannedRoles_Tenant.Federated_Sharing.Cmdlets),
			new RoleDefinition("GALSynchronizationManagement", RoleType.GALSynchronizationManagement, CannedRoles_Tenant.GALSynchronizationManagement.Cmdlets),
			new RoleDefinition("Information Rights Management", RoleType.InformationRightsManagement, CannedRoles_Tenant.Information_Rights_Management.Cmdlets),
			new RoleDefinition("Journaling", RoleType.Journaling, CannedRoles_Tenant.Journaling.Cmdlets),
			new RoleDefinition("Legal Hold", RoleType.LegalHold, CannedRoles_Tenant.Legal_Hold.Cmdlets),
			new RoleDefinition("Mail Enabled Public Folders", RoleType.MailEnabledPublicFolders, CannedRoles_Tenant.Mail_Enabled_Public_Folders.Cmdlets),
			new RoleDefinition("Mail Recipient Creation", RoleType.MailRecipientCreation, CannedRoles_Tenant.Mail_Recipient_Creation.Cmdlets),
			new RoleDefinition("Mail Recipients", RoleType.MailRecipients, CannedRoles_Tenant.Mail_Recipients.Cmdlets),
			new RoleDefinition("Mail Tips", RoleType.MailTips, CannedRoles_Tenant.Mail_Tips.Cmdlets),
			new RoleDefinition("Mailbox Import Export", RoleType.MailboxImportExport, CannedRoles_Tenant.Mailbox_Import_Export.Cmdlets),
			new RoleDefinition("Mailbox Search", RoleType.MailboxSearch, CannedRoles_Tenant.Mailbox_Search.Cmdlets),
			new RoleDefinition("Message Tracking", RoleType.MessageTracking, CannedRoles_Tenant.Message_Tracking.Cmdlets),
			new RoleDefinition("Migration", RoleType.Migration, CannedRoles_Tenant.Migration.Cmdlets),
			new RoleDefinition("Monitoring", RoleType.Monitoring, CannedRoles_Tenant.Monitoring.Cmdlets),
			new RoleDefinition("Move Mailboxes", RoleType.MoveMailboxes, CannedRoles_Tenant.Move_Mailboxes.Cmdlets),
			new RoleDefinition("My Custom Apps", RoleType.MyCustomApps, CannedRoles_Tenant.My_Custom_Apps.Cmdlets),
			new RoleDefinition("My Marketplace Apps", RoleType.MyMarketplaceApps, CannedRoles_Tenant.My_Marketplace_Apps.Cmdlets),
			new RoleDefinition("My ReadWriteMailbox Apps", RoleType.MyReadWriteMailboxApps, CannedRoles_Tenant.My_ReadWriteMailbox_Apps.Cmdlets),
			new RoleDefinition("MyAddressInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Tenant.MyAddressInformation.Cmdlets),
			new RoleDefinition("MyBaseOptions", RoleType.MyBaseOptions, CannedRoles_Tenant.MyBaseOptions.Cmdlets),
			new RoleDefinition("MyContactInformation", RoleType.MyContactInformation, CannedRoles_Tenant.MyContactInformation.Cmdlets),
			new RoleDefinition("MyDisplayName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Tenant.MyDisplayName.Cmdlets),
			new RoleDefinition("MyDistributionGroupMembership", RoleType.MyDistributionGroupMembership, CannedRoles_Tenant.MyDistributionGroupMembership.Cmdlets),
			new RoleDefinition("MyDistributionGroups", RoleType.MyDistributionGroups, CannedRoles_Tenant.MyDistributionGroups.Cmdlets),
			new RoleDefinition("MyMailSubscriptions", RoleType.MyMailSubscriptions, CannedRoles_Tenant.MyMailSubscriptions.Cmdlets),
			new RoleDefinition("MyMailboxDelegation", RoleType.MyMailboxDelegation, CannedRoles_Tenant.MyMailboxDelegation.Cmdlets),
			new RoleDefinition("MyMobileInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Tenant.MyMobileInformation.Cmdlets),
			new RoleDefinition("MyName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Tenant.MyName.Cmdlets),
			new RoleDefinition("MyPersonalInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_Tenant.MyPersonalInformation.Cmdlets),
			new RoleDefinition("MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_Tenant.MyProfileInformation.Cmdlets),
			new RoleDefinition("MyRetentionPolicies", RoleType.MyRetentionPolicies, CannedRoles_Tenant.MyRetentionPolicies.Cmdlets),
			new RoleDefinition("MyTeamMailboxes", RoleType.MyTeamMailboxes, CannedRoles_Tenant.MyTeamMailboxes.Cmdlets),
			new RoleDefinition("MyTextMessaging", RoleType.MyTextMessaging, CannedRoles_Tenant.MyTextMessaging.Cmdlets),
			new RoleDefinition("MyVoiceMail", RoleType.MyVoiceMail, CannedRoles_Tenant.MyVoiceMail.Cmdlets),
			new RoleDefinition("Org Custom Apps", RoleType.OrgCustomApps, CannedRoles_Tenant.Org_Custom_Apps.Cmdlets),
			new RoleDefinition("Org Marketplace Apps", RoleType.OrgMarketplaceApps, CannedRoles_Tenant.Org_Marketplace_Apps.Cmdlets),
			new RoleDefinition("Organization Client Access", RoleType.OrganizationClientAccess, CannedRoles_Tenant.Organization_Client_Access.Cmdlets),
			new RoleDefinition("Organization Configuration", RoleType.OrganizationConfiguration, CannedRoles_Tenant.Organization_Configuration.Cmdlets),
			new RoleDefinition("Organization Transport Settings", RoleType.OrganizationTransportSettings, CannedRoles_Tenant.Organization_Transport_Settings.Cmdlets),
			new RoleDefinition("Public Folders", RoleType.PublicFolders, CannedRoles_Tenant.Public_Folders.Cmdlets),
			new RoleDefinition("Recipient Policies", RoleType.RecipientPolicies, CannedRoles_Tenant.Recipient_Policies.Cmdlets),
			new RoleDefinition("Remote and Accepted Domains", RoleType.RemoteAndAcceptedDomains, CannedRoles_Tenant.Remote_and_Accepted_Domains.Cmdlets),
			new RoleDefinition("Reset Password", RoleType.ResetPassword, CannedRoles_Tenant.Reset_Password.Cmdlets),
			new RoleDefinition("Retention Management", RoleType.RetentionManagement, CannedRoles_Tenant.Retention_Management.Cmdlets),
			new RoleDefinition("Role Management", RoleType.RoleManagement, CannedRoles_Tenant.Role_Management.Cmdlets),
			new RoleDefinition("Security Group Creation and Membership", RoleType.SecurityGroupCreationAndMembership, CannedRoles_Tenant.Security_Group_Creation_and_Membership.Cmdlets),
			new RoleDefinition("Supervision", RoleType.Supervision, CannedRoles_Tenant.Supervision.Cmdlets),
			new RoleDefinition("Team Mailboxes", RoleType.TeamMailboxes, CannedRoles_Tenant.Team_Mailboxes.Cmdlets),
			new RoleDefinition("Transport Hygiene", RoleType.TransportHygiene, CannedRoles_Tenant.Transport_Hygiene.Cmdlets),
			new RoleDefinition("Transport Rules", RoleType.TransportRules, CannedRoles_Tenant.Transport_Rules.Cmdlets),
			new RoleDefinition("UM Mailboxes", RoleType.UMMailboxes, CannedRoles_Tenant.UM_Mailboxes.Cmdlets),
			new RoleDefinition("UM Prompts", RoleType.UMPrompts, CannedRoles_Tenant.UM_Prompts.Cmdlets),
			new RoleDefinition("Unified Messaging", RoleType.UnifiedMessaging, CannedRoles_Tenant.Unified_Messaging.Cmdlets),
			new RoleDefinition("User Options", RoleType.UserOptions, CannedRoles_Tenant.User_Options.Cmdlets),
			new RoleDefinition("View-Only Audit Logs", RoleType.ViewOnlyAuditLogs, CannedRoles_Tenant.View_Only_Audit_Logs.Cmdlets),
			new RoleDefinition("View-Only Configuration", RoleType.ViewOnlyConfiguration, CannedRoles_Tenant.View_Only_Configuration.Cmdlets),
			new RoleDefinition("View-Only Recipients", RoleType.ViewOnlyRecipients, CannedRoles_Tenant.View_Only_Recipients.Cmdlets)
		};

		private class Address_Lists
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Container,SearchText")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "DefaultOnly,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions",
						"OfflineAddressBookEnabled"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,ErrorAction,ErrorVariable,GlobalAddressList,Name,OfflineAddressBook,OutBuffer,OutVariable,RoomList,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,Container,DisplayName,ErrorAction,ErrorVariable,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,ErrorAction,ErrorVariable,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,DiffRetentionPeriod,ErrorAction,ErrorVariable,IsDefault,Name,OutBuffer,OutVariable,Versions,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recursive,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,Confirm,ErrorAction,ErrorVariable,GlobalAddressList,Identity,Name,OfflineAddressBook,OutBuffer,OutVariable,RoomList,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,DisplayName,ErrorAction,ErrorVariable,Identity,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-GlobalAddressList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,ErrorAction,ErrorVariable,Identity,IncludedRecipients,Name,OutBuffer,OutVariable,RecipientFilter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OfflineAddressBook", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "AddressLists,ApplyMandatoryProperties,ConfiguredAttributes,Confirm,DiffRetentionPeriod,ErrorAction,ErrorVariable,Identity,IsDefault,Name,OutBuffer,OutVariable,Schedule,UseDefaultAttributes,Versions,WarningAction,WarningVariable,WhatIf")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Confirm,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,EndDate,ErrorAction,ErrorVariable,ExternalAccess,LogonTypes,Mailboxes,Name,Operations,OutBuffer,OutVariable,ShowDetails,StartDate,StatusMailRecipients,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,EndDate,ErrorAction,ErrorVariable,ExternalAccess,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MailboxAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,LogonTypes,Mailboxes,Operations,OutBuffer,OutVariable,ResultSize,ShowDetails,StartDate,WarningAction,WarningVariable")
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
					}, "AuditAdmin,AuditDelegate,AuditEnabled,AuditLogAgeLimit,Force")
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
					}, "AuditBypassEnabled,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mode,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-DlpPolicyCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CompliancePolicySyncNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassificationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,JobId,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailDlpPolicyReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Action,Direction,DlpPolicy,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailMalwareReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MalwareName,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailSpamReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailTransportRuleReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailFilterListReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Domain,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,ProbeTag,SelectionTarget,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficPolicyReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,AggregateBy,Direction,DlpPolicy,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,AggregateBy,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Category,DlpPolicy,Domain,EndDate,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficTopReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AggregateBy,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "EndDate,ErrorAction,ErrorVariable,Expression,FromIP,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,Status,ToIP,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTraceDetail", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "Action,EndDate,ErrorAction,ErrorVariable,Event,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Action,ErrorAction,ErrorVariable,Identity,Locale,Original,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,TrustedPublishingDomain,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-DlpPolicyCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-CompliancePolicySyncNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Force,FullSync,Identity,SyncChangeInfos,SyncNow,SyncSvcUrl,Verbose,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,Confirm,Description,ErrorAction,ErrorVariable,Fingerprints,Locale,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Mode,Name,OutBuffer,OutVariable,Parameters,State,Template,TemplateData,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Fingerprint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Value,WarningAction,WarningVariable,WhatIf")
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
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,ApplyOME,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfRecipientInSenderList,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderInRecipientList,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RecipientInSenderList,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RemoveOME,RouteMessageOutboundConnector,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderInRecipientList,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-CompliancePolicySyncNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Force,Identity,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,FileData,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Fingerprints,Identity,IsDefault,Locale,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Identity,Mode,Name,OutBuffer,OutVariable,State,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DisclaimerText,EmailText,Image,OTPEnabled,PortalText")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Value,WarningAction,WarningVariable,WhatIf")
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
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,ApplyOME,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfRecipientInSenderList,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderInRecipientList,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RecipientInSenderList,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RemoveOME,RouteMessageOutboundConnector,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderInRecipientList,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "DLPPolicy,DeliveryStatus,Direction,EndDate,ErrorAction,ErrorVariable,Locale,MessageID,NotifyAddress,OriginalClientIP,OutBuffer,OutVariable,RecipientAddress,ReportTitle,ReportType,SenderAddress,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,JobId,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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

		private class Distribution_Groups
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"AcceptedDomains"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "OrganizationalUnit")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "IgnoreNamingPolicy,MemberDepartRestriction,OrganizationalUnit,RoomList"),
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
					}, "Alias,ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,DisplayName,ErrorAction,ErrorVariable,IncludedRecipients,Name,OrganizationalUnit,OutBuffer,OutVariable,PrimarySmtpAddress,RecipientContainer,RecipientFilter,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,IgnoreNamingPolicy,ManagedBy,MemberDepartRestriction,MemberJoinRestriction,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,RoomList,SendOofMessageToOriginatorEnabled,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ConditionalCompany,ConditionalCustomAttribute1,ConditionalCustomAttribute10,ConditionalCustomAttribute11,ConditionalCustomAttribute12,ConditionalCustomAttribute13,ConditionalCustomAttribute14,ConditionalCustomAttribute15,ConditionalCustomAttribute2,ConditionalCustomAttribute3,ConditionalCustomAttribute4,ConditionalCustomAttribute5,ConditionalCustomAttribute6,ConditionalCustomAttribute7,ConditionalCustomAttribute8,ConditionalCustomAttribute9,ConditionalDepartment,ConditionalStateOrProvince,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ForceUpgrade,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IncludedRecipients,ManagedBy,Name,Notes,PhoneticDisplayName,RecipientContainer,RecipientFilter,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,SendOofMessageToOriginatorEnabled,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
					}, "Confirm,ErrorAction,ErrorVariable")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "AccessMethod,Confirm,Credentials,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,TargetAutodiscoverEpr,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeExtendedDomainInfo,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassAdditionalDomainValidation,DomainName,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TrustedHostnames,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DiscoveryEndpoint,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,TargetAddressDomains,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ArchiveAccessEnabled,Confirm,DomainNames,Enabled,ErrorAction,ErrorVariable,FreeBusyAccessEnabled,FreeBusyAccessLevel,FreeBusyAccessScope,MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope,Name,OrganizationContact,OutBuffer,OutVariable,PhotosEnabled,TargetApplicationUri,TargetAutodiscoverEpr,TargetOwaURL,TargetSharingEpr,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,Default,Domains,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AccountNamespace,Confirm,DefaultDomain,DelegationFederationTrust,Enabled,ErrorAction,ErrorVariable,Identity,OrganizationContact,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,DiscoveryEndpoint,Enabled,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TargetAddressDomains,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"CalendarVersionStoreEnabled",
						"EOPPremiumRestrictions"
					}, "CalendarVersionStoreEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "PublicComputersDetectionEnabled,PublicFolderMailboxesLockedForNewConnections,PublicFolderMailboxesMigrationComplete")
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
					}, "ArchiveAccessEnabled,Confirm,DomainNames,Enabled,ErrorAction,ErrorVariable,Force,FreeBusyAccessEnabled,FreeBusyAccessLevel,FreeBusyAccessScope,MailTipsAccessEnabled,MailTipsAccessLevel,MailTipsAccessScope,Name,OrganizationContact,OutBuffer,OutVariable,PhotosEnabled,TargetApplicationUri,TargetAutodiscoverEpr,TargetOwaURL,TargetSharingEpr,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"MessageTrackingPermissions"
					}, "DeliveryReportEnabled")
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
					}, "Confirm,Default,Domains,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class GALSynchronizationManagement
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncDeletedRecipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Cookie,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Pages,RecipientType,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Cookie,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,Pages,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncDynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Cookie,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,Pages,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncMailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Cookie,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Pages,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncMailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Cookie,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Pages,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,Cookie,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,Pages,PublicFolder,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,Alias,BlockedSendersHash,Confirm,CopyOwnerToMember,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreNamingPolicy,IsHierarchicalGroup,ManagedBy,MemberDepartRestriction,MemberJoinRestriction,Notes,OnPremisesObjectId,OrganizationalUnit,OutBuffer,OutVariable,PhoneticDisplayName,PrimarySmtpAddress,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,RoomList,SafeRecipientsHash,SafeSendersHash,SendOofMessageToOriginatorEnabled,SeniorityIndex,Type,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"GroupAsGroupSyncPermissions"
					}, "Members,Name"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions"
					}, "MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,BypassNestedModerationEnabled,ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncMailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,Alias,AssistantName,BlockedSendersHash,City,Company,Confirm,CountryOrRegion,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Department,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExternalEmailAddress,Fax,FirstName,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HomePhone,Initials,LastName,MacAttachmentFormat,Manager,MasterAccountSid,MessageBodyFormat,MessageFormat,MobilePhone,Name,Notes,Office,OnPremisesObjectId,OrganizationalUnit,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Phone,PhoneticDisplayName,PostalCode,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceType,SafeRecipientsHash,SafeSendersHash,SeniorityIndex,StateOrProvince,StreetAddress,TelephoneAssistant,Title,UsePreferMessageFormat,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions"
					}, "MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncMailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,Alias,ArchiveGuid,ArchiveName,AssistantName,BlockedSendersHash,City,Company,Confirm,CountryOrRegion,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DeliverToMailboxAndForward,Department,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,EvictLiveId,ExchangeGuid,ExternalEmailAddress,Fax,FederatedIdentity,FirstName,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HomePhone,ImmutableId,ImportLiveId,Initials,IntendedMailboxPlanName,Languages,LastName,MacAttachmentFormat,Manager,MasterAccountSid,MessageBodyFormat,MessageFormat,MicrosoftOnlineServicesID,MobilePhone,Name,NetID,Notes,Office,OnPremisesObjectId,OrganizationalUnit,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Password,Phone,PhoneticDisplayName,Picture,PostalCode,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,RemotePowerShellEnabled,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,ResourceType,SafeRecipientsHash,SafeSendersHash,SamAccountName,SeniorityIndex,SharePointUrl,SiteMailboxClosedTime,SiteMailboxOwners,SiteMailboxUsers,StateOrProvince,StreetAddress,TelephoneAssistant,Title,UseExistingLiveId,UseMapiRichTextFormat,UsePreferMessageFormat,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf,WindowsLiveID"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,Alias,AntispamBypassEnabled,ArchiveGuid,ArchiveName,AssistantName,BlockedSendersHash,City,Company,Confirm,CountryOrRegion,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DeliverToMailboxAndForward,Department,DirSyncId,DisplayName,EmailAddresses,Equipment,ErrorAction,ErrorVariable,EvictLiveId,Fax,FederatedIdentity,FirstName,Force,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HoldForMigration,HomePhone,ImmutableId,ImportLiveId,Initials,Languages,LastName,MailboxPlanName,Manager,MasterAccountSid,MicrosoftOnlineServicesID,MobilePhone,Name,NetID,Notes,Office,OnPremisesObjectId,OrganizationalUnit,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Password,Phone,PhoneticDisplayName,Picture,PostalCode,PublicFolder,RejectMessagesFrom,RejectMessagesFromDLMembers,RemotePowerShellEnabled,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,ResourceWindowsLiveID,Room,SafeRecipientsHash,SafeSendersHash,SamAccountName,SeniorityIndex,Shared,SharingPolicy,SpokenName,StateOrProvince,StreetAddress,TargetAllMDbs,TelephoneAssistant,Title,UseExistingLiveId,UseExistingResourceLiveId,UseMapiRichTextFormat,UserCertificate,UserPrincipalName,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf,WindowsLiveID"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"GroupAsGroupSyncPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncMailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncMailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DisableWindowsLiveID,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserLiveIdManagementPermissions"
					}, "KeepWindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DisableWindowsLiveID,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,IgnoreLegalHold,OutBuffer,OutVariable,Permanent,PublicFolder,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserLiveIdManagementPermissions"
					}, "KeepWindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DisableWindowsLiveID,EnterpriseExchangeVersion,ErrorAction,ErrorVariable,FederatedIdentitySourceADAttribute,OutBuffer,OutVariable,PasswordFilePath,ProvisioningDomain,ResetPasswordOnNextLogon,WarningAction,WarningVariable,WhatIf,WlidUseSMTPPrimary")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PrimarySmtpAddress"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,BlockedSendersHash,Confirm,CreateDTMFMap,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreNamingPolicy,IsHierarchicalGroup,ManagedBy,MemberDepartRestriction,MemberJoinRestriction,Name,Notes,OnPremisesObjectId,OutBuffer,OutVariable,PhoneticDisplayName,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,RequireSenderAuthenticationEnabled,SafeRecipientsHash,SafeSendersHash,SendOofMessageToOriginatorEnabled,SeniorityIndex,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,BypassModerationFromSendersOrMembers,BypassNestedModerationEnabled,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncMailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,AssistantName,BlockedSendersHash,City,Company,Confirm,CountryOrRegion,CreateDTMFMap,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,Department,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,Fax,FirstName,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HomePhone,IgnoreDefaultScope,Initials,LastName,MacAttachmentFormat,Manager,MasterAccountSid,MaxRecipientPerMessage,MessageBodyFormat,MessageFormat,MobilePhone,Name,Notes,Office,OnPremisesObjectId,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Phone,PhoneticDisplayName,PostalCode,RawAcceptMessagesOnlyFrom,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RemovePicture,RemoveSpokenName,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceType,SafeRecipientsHash,SafeSendersHash,SecondaryAddress,SecondaryDialPlan,SeniorityIndex,SimpleDisplayName,StateOrProvince,StreetAddress,TelephoneAssistant,Title,UseMapiRichTextFormat,UsePreferMessageFormat,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"UMPermissions"
					}, "UMDtmfMap")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncMailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ArchiveGuid,ArchiveName,AssistantName,BlockedSendersHash,City,Company,Confirm,CountryOrRegion,CreateDTMFMap,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DeliverToMailboxAndForward,Department,DirSyncId,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExchangeGuid,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,Fax,FederatedIdentity,FirstName,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HomePhone,IgnoreDefaultScope,ImmutableId,Initials,IntendedMailboxPlanName,Languages,LastName,MacAttachmentFormat,Manager,MasterAccountSid,MessageBodyFormat,MessageFormat,MicrosoftOnlineServicesID,MobilePhone,Name,Notes,Office,OnPremisesObjectId,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Phone,PhoneticDisplayName,Picture,PostalCode,RawSiteMailboxOwners,RawSiteMailboxUsers,RecipientDisplayType,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RemovePicture,RemoveSpokenName,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,ResourceType,SafeRecipientsHash,SafeSendersHash,SamAccountName,SecondaryAddress,SeniorityIndex,SharePointUrl,SimpleDisplayName,SiteMailboxClosedTime,SiteMailboxOwners,SiteMailboxUsers,StateOrProvince,StreetAddress,TelephoneAssistant,Title,UseMapiRichTextFormat,UsePreferMessageFormat,UserCertificate,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf,WindowsEmailAddress,WindowsLiveID"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "SecondaryDialPlan"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "UMDtmfMap"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,ApplyMandatoryProperties,ArchiveGuid,ArchiveName,AssistantName,BlockedSendersHash,CalendarRepairDisabled,CalendarVersionStoreDisabled,City,Company,Confirm,CountryOrRegion,CreateDTMFMap,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DeliverToMailboxAndForward,Department,DirSyncId,DisplayName,DowngradeHighPriorityMessagesEnabled,EmailAddresses,EndDateForRetentionHold,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalOofOptions,Fax,FederatedIdentity,FirstName,Force,ForwardingSmtpAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,HomePhone,IgnoreDefaultScope,ImmutableId,Initials,Languages,LastName,LinkedCredential,LinkedMasterAccount,MailboxPlanName,Manager,MasterAccountSid,MaxBlockedSenders,MaxSafeSenders,MicrosoftOnlineServicesID,MobilePhone,Name,Notes,Office,OnPremisesObjectId,OtherFax,OtherHomePhone,OtherTelephone,OutBuffer,OutVariable,Pager,Phone,PhoneticDisplayName,Picture,PostalCode,PublicFolder,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RemovePicture,RemoveSpokenName,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,RetentionHoldEnabled,RetentionPolicy,SafeRecipientsHash,SafeSendersHash,SamAccountName,SecondaryAddress,SeniorityIndex,SharingPolicy,SimpleDisplayName,SingleItemRecoveryEnabled,SpokenName,StartDateForRetentionHold,StateOrProvince,StreetAddress,TelephoneAssistant,Title,Type,UserCertificate,UserPrincipalName,UserSMimeCertificate,WarningAction,WarningVariable,WebPage,WhatIf,WindowsEmailAddress,WindowsLiveID"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldEnabled,LitigationHoldOwner,RetentionComment,RetentionUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ManagedFoldersPermissions"
					}, "ManagedFolderMailboxPolicy,ManagedFolderMailboxPolicyAllowed,RemoveManagedFolderAndPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "MessageTrackingReadStatusEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ModeratedRecipientsPermissions"
					}, "BypassModerationFrom,BypassModerationFromDLMembers,BypassModerationFromSendersOrMembers,ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "SecondaryDialPlan"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "UMDtmfMap"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "ForwardingAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SyncDistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AddedMembers,Confirm,ErrorAction,ErrorVariable,Members,OutBuffer,OutVariable,RemovedMembers,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"GroupAsGroupSyncPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SyncStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClientData,Confirm,ErrorAction,ErrorVariable,MailboxCreationElapsedMilliseconds,NumberOfConnectionErrors,NumberOfExportSyncRuns,NumberOfIlmLogicErrors,NumberOfIlmOtherErrors,NumberOfImportSyncRuns,NumberOfLiveIdErrors,NumberOfMailboxesCreated,NumberOfMailboxesToCreate,NumberOfPermissionErrors,NumberOfSucessfulExportSyncRuns,NumberOfSucessfulImportSyncRuns,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,TrustedPublishingDomain,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Default,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Default,ErrorAction,ErrorVariable,ExtranetCertificationUrl,ExtranetLicensingUrl,FileData,IntranetCertificationUrl,IntranetLicensingUrl,Name,OutBuffer,OutVariable,Password,RMSOnline,RefreshTemplates,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ApplyRightsProtectionTemplate,Confirm,Enabled,ErrorAction,ErrorVariable,Force,FromDepartment,Name,OutBuffer,OutVariable,Priority,SentTo,SentToScope,UserCanOverride,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyOME,RemoveOME")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ClientAccessServerEnabled,Confirm,EDiscoverySuperUserEnabled,ErrorAction,ErrorVariable,ExternalLicensingEnabled,Force,InternalLicensingEnabled,OutBuffer,OutVariable,RMSOnlineKeySharingLocation,SearchEnabled,TransportDecryptionSetting,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DisclaimerText,EmailText,Image,OTPEnabled,PortalText")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ApplyRightsProtectionTemplate,Confirm,ErrorAction,ErrorVariable,Force,FromDepartment,Identity,Name,OutBuffer,OutVariable,Priority,SentTo,SentToScope,UserCanOverride,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,Default,ErrorAction,ErrorVariable,ExtranetCertificationUrl,ExtranetLicensingUrl,IntranetCertificationUrl,IntranetLicensingUrl,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"IRMPremiumFeaturesPermissions",
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ApplyOME,RemoveOME")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,RMSOnline,Recipient,Sender,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,Enabled,ErrorAction,ErrorVariable,JournalEmailAddress,Name,OutBuffer,OutVariable,Recipient,Scope,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,JournalEmailAddress,Name,OutBuffer,OutVariable,Recipient,Scope,WarningAction,WarningVariable,WhatIf")
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Legal_Hold
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,ShowDeletionInProgressSearches,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Confirm,Description,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,SourceMailboxes,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,Force,RetentionPolicy,SingleItemRecoveryEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl")
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
					}, "Confirm,Description,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,SourceMailboxes,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,HiddenFromAddressListsEnabled,Identity,OutBuffer,OutVariable,OverrideRecipientQuotas,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "Verbose"),
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
					}, "Alias,Confirm,EmailAddresses,EntryId,ErrorAction,ErrorVariable,ExternalEmailAddress,HiddenFromAddressListsEnabled,Name,OutBuffer,OutVariable,OverrideRecipientQuotas,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
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
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,Contacts,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DeliverToMailboxAndForward,DisplayName,EmailAddressPolicyEnabled,EmailAddresses,EntryId,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,ForwardingAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,IgnoreDefaultScope,MaxReceiveSize,MaxSendSize,Name,OutBuffer,OutVariable,PhoneticDisplayName,PublicFolderType,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
				}, "c")
			};
		}

		private class Mail_Recipient_Creation
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-JournalArchiving", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LinkedUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,InactiveMailboxOnly,IncludeInactiveMailbox,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SoftDeletedMailbox,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllMailboxPlanReleases,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomRecipientWriteScope,Delegating,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,WarningAction,WarningVariable,WritableRecipient")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ToolInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Version,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Alias,Confirm,DisplayName,ErrorAction,ErrorVariable,ExternalEmailAddress,FirstName,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,OrganizationalUnit,OutBuffer,OutVariable,UsePreferMessageFormat,WarningAction,WarningVariable,WhatIf"),
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
					}, "Alias,Confirm,DisplayName,ErrorAction,ErrorVariable,ExternalEmailAddress,FederatedIdentity,FirstName,ImmutableId,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,OrganizationalUnit,OutBuffer,OutVariable,RemotePowerShellEnabled,UsePreferMessageFormat,WarningAction,WarningVariable,WhatIf"),
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
						"UserLiveIdManagementPermissions"
					}, "EvictLiveId,ImportLiveId,UseExistingLiveId,WindowsLiveID")
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
					}, "EnableRoomMailboxAccount,RoomMailboxPassword,TargetAllMDBs"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MSOIdPermissions",
						"PilotingOrganization_Restrictions"
					}, "MicrosoftOnlineServicesID"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailboxRecoveryPermissions",
						"PilotingOrganization_Restrictions"
					}, "RemovedMailbox"),
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
					}, "Alias,Confirm,Discovery,DisplayName,Equipment,ErrorAction,ErrorVariable,FederatedIdentity,FirstName,Force,ImmutableId,Initials,LastName,MailboxPlan,Name,OrganizationalUnit,OutBuffer,OutVariable,RemotePowerShellEnabled,Room,Shared,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"UserLiveIdManagementPermissions"
					}, "EvictLiveId,ImportLiveId,UseExistingLiveId,WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PropertiesMasteredOnPremiseRestrictions",
						"RecipientManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreLegalHold,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"UserLiveIdManagementPermissions"
					}, "KeepWindowsLiveID")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,PublicFolder,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserLiveIdManagementPermissions"
					}, "KeepWindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxFolderPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Undo-SoftDeletedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SoftDeletedFeatureManagementPermissions"
					}, "Confirm,DisplayName,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Password,SoftDeletedObject,WarningAction,WarningVariable,WhatIf,WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AccessRights,AutoMapping,Confirm,Deny,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,InheritanceType,OutBuffer,OutVariable,Owner,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-RecipientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Trustee,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreLegalHold,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Force")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ArchivePermissions",
						"RichCoexistenceRestrictions"
					}, "Archive,ArchiveName"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,InactiveMailboxOnly,IncludeInactiveMailbox,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SoftDeletedMailbox,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
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
					}, "Identity,IncludeAnalysis,IncludeOldestAndNewestItems")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,ReadFromDomainController,ResultSize,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllMailboxPlanReleases,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomRecipientWriteScope,Delegating,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,WarningAction,WarningVariable,WritableRecipient")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,Capabilities,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Trustee,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientStatisticsReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SendAddress", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AddressId,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,MaintenanceWindowDays,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,BypassOwnerCheck,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,SubscriptionType,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ToolInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Version,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,Filter,IgnoreDefaultScope,ResultSize,SortBy")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,Preview,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-ContactList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CSV,CSVData,CSVStream,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaMailboxPolicy", new RoleParameters[]
				{
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxFolderPermission", new RoleParameters[]
				{
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
						"UserMailboxAccessPermissions"
					}, "AccessRights,Confirm,Deny,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,InheritanceType,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RecipientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Trustee,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ActiveSyncDebugLogging,ActiveSyncEnabled,ActiveSyncMailboxPolicy,OWAforDevicesEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ShowGalAsDefaultView,WarningAction,WarningVariable,WhatIf"),
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
					}, "AddAdditionalResponse,AddNewRequestsTentatively,AddOrganizerToSubject,AdditionalResponse,AllBookInPolicy,AllRequestInPolicy,AllRequestOutOfPolicy,AllowConflicts,AllowRecurringMeetings,AutomateProcessing,BookInPolicy,BookingWindowInDays,Confirm,ConflictPercentageAllowed,DeleteAttachments,DeleteComments,DeleteNonCalendarItems,DeleteSubject,EnableResponseDetails,EnforceSchedulingHorizon,ErrorAction,ErrorVariable,ForwardRequestsToDelegates,IgnoreDefaultScope,MaximumConflictInstances,MaximumDurationInMinutes,OrganizerInfo,OutBuffer,OutVariable,RemoveForwardedMeetingNotifications,RemoveOldMeetingMessages,RemovePrivateProperty,RequestInPolicy,RequestOutOfPolicy,ResourceDelegates,ScheduleOnlyDuringWorkHours,TentativePendingApproval,WarningAction,WarningVariable,WhatIf"),
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
					}, "ErrorAction,ErrorVariable"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,OutBuffer,OutVariable,PostOfficeBox,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Group", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,PhoneticDisplayName")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "Confirm,DisplayName,Enabled,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Password,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,Enabled,ErrorAction,ErrorVariable,Force,Identity,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUserName,Mailbox,OutBuffer,OutVariable,ResendVerification,ValidateSecret,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,Identity,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-LinkedUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CertificateSubject,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,Name,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,ForceUpgrade,MacAttachmentFormat,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,SecondaryAddress,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,ExternalEmailAddress,GrantSendOnBehalfTo,HiddenFromAddressListsEnabled,Identity,ImmutableId,Name,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,ForceUpgrade,JournalArchiveAddress,MacAttachmentFormat,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,SecondaryAddress,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
						"UserLiveIdManagementPermissions"
					}, "WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,Identity,ImmutableId,LitigationHoldEnabled,Name,Office"),
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
						"ChangeMailboxPlanAssignmentPermissions",
						"EOPPremiumRestrictions"
					}, "MailboxPlan"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,ApplyMandatoryProperties,CalendarRepairDisabled,CalendarVersionStoreDisabled,Confirm,DeliverToMailboxAndForward,EnableRoomMailboxAccount,EndDateForRetentionHold,ErrorAction,ErrorVariable,ExternalOofOptions,Force,ForwardingSmtpAddress,GrantSendOnBehalfTo,JournalArchiveAddress,Languages,MessageCopyForSendOnBehalfEnabled,MessageCopyForSentAsEnabled,OutBuffer,OutVariable,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,RetentionHoldEnabled,RetentionPolicy,RoomMailboxPassword,RulesQuota,SecondaryAddress,SharingPolicy,SimpleDisplayName,SingleItemRecoveryEnabled,StartDateForRetentionHold,Type,UseDatabaseQuotaDefaults,UseDatabaseRetentionDefaults,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MSOIdPermissions"
					}, "MicrosoftOnlineServicesID"),
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
						"ResetUserPasswordManagementPermissions"
					}, "WindowsLiveID"),
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
					}, "PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "FederatedIdentity,ResetPasswordOnNextLogon"),
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
					}, "AutoReplyState,Confirm,EndTime,ErrorAction,ErrorVariable,ExternalAudience,ExternalMessage,OutBuffer,OutVariable,StartTime,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,DefaultReminderTime,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,WarningAction,WarningVariable,WeatherEnabled,WeatherLocations,WeatherUnit,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
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
					}, "BlockedSendersAndDomains,Confirm,ContactsTrusted,Enabled,ErrorAction,ErrorVariable,OutBuffer,OutVariable,TrustedListsOnly,TrustedSendersAndDomains,WarningAction,WarningVariable,WhatIf")
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
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "SendAddressDefault")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DisplayName,ErrorAction,ErrorVariable,Force,IsDefault,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
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
					}, "DisableFacebook,InstantMessagingType"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"OwaFacebookEnabledRestrictions"
					}, "FacebookEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,IsDefault,JournalEnabled,LinkedInEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PlacesEnabled,PremiumClientEnabled,PublicFoldersEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WeatherEnabled,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,Enabled,ErrorAction,ErrorVariable,Force,Identity,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUserName,LeaveOnServer,Mailbox,OutBuffer,OutVariable,ResendVerification,ValidateSecret,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Authentication,Confirm,ErrorAction,ErrorVariable,Force,Identity,IncrementalSyncInterval,Mailbox,OutBuffer,OutVariable,Password,RemoteServerName,RemoteServerPort,Security,SkipMerging,SmtpServerName,SmtpServerPort,WarningAction,WarningVariable,WhatIf")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,RemotePowerShellEnabled,WarningAction,WarningVariable"),
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
					}, "AssistantName,Company,Department,Manager,Name,OtherFax,OtherHomePhone,OtherTelephone,Title"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "Confirm,PublicFolder,ResetPasswordOnNextLogon,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,PictureData,PictureStream,Preview,Save,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MAPIConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OAuthConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,ReloadConfig,Service,TargetUri,UseCachedToken,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-HybridConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,ForceUpgrade,OnPremisesCredentials,OutBuffer,OutVariable,SuppressOAuthWarning,TenantCredentials,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "AccessMethod,Confirm,Credentials,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,TargetAutodiscoverEpr,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,MailTipsAllTipsEnabled,MailTipsGroupMetricsEnabled,MailTipsMailboxSourcedTipsEnabled,OutBuffer,OutVariable,PublicComputersDetectionEnabled,PublicFolderMailboxesLockedForNewConnections,PublicFolderMailboxesMigrationComplete,WhatIf"),
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,ErrorAction,ErrorVariable,Identity,Mailbox,Name,OutBuffer,OutVariable,ResultSize,Status,Suspend,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxImportRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,AssociatedMessagesCopyOption,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ConflictResolutionOption,ContentCodePage,ErrorAction,ErrorVariable,ExcludeDumpster,ExcludeFolders,FilePath,IncludeFolders,IsArchive,LargeItemLimit,Mailbox,Name,OutBuffer,OutVariable,RemoteCredential,RemoteHostName,SkipMerging,SourceRootFolder,Suspend,SuspendComment,TargetRootFolder,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,DeleteContent,DoNotIncludeArchive,ErrorAction,ErrorVariable,EstimateResultOnly,Force,Identity,IncludeUnsearchableItems,LogLevel,LogOnly,OutBuffer,OutVariable,SearchDumpster,SearchDumpsterOnly,SearchQuery,TargetFolder,TargetMailbox,WarningAction,WarningVariable,WhatIf")
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
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ErrorAction,ErrorVariable,LargeItemLimit,OutBuffer,OutVariable,RemoteCredential,RemoteHostName,SkipMerging,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MailboxImportRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mailbox_Search
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,InactiveMailboxOnly,IncludeInactiveMailbox,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,ShowDeletionInProgressSearches,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,SourceMailboxes,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "AllPublicFolderSources,AllSourceMailboxes,EndDate,EstimateOnly,ExcludeDuplicateMessages,IncludeKeywordStatistics,IncludeUnsearchableItems,Language,LogLevel,MessageTypes,PublicFolderSources,Recipients,SearchQuery,Senders,StartDate,StatusMailRecipients,TargetMailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,DoNotIncludeArchive,ErrorAction,ErrorVariable,EstimateResultOnly,Force,Identity,IncludeUnsearchableItems,LogLevel,LogOnly,OutBuffer,OutVariable,SearchDumpster,SearchDumpsterOnly,SearchQuery,TargetFolder,TargetMailbox,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions",
						"SearchMessagePermissions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,SourceMailboxes,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Resume,StatisticsStartIndex,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-MailboxSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "AccessMethod,Confirm,Credentials,ErrorAction,ErrorVariable,ForestName,OutBuffer,OutVariable,TargetAutodiscoverEpr,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityAddressSpace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,DoNotResolve,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientPathFilter,Recipients,ReportTemplate,ResultSize,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,DoNotResolve,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MigrationReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,CsvStream,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RowCount,StartingRowIndex,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Diagnostic,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitSkippedItemsTo,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "ArchiveOnly,AutoComplete,BadItemLimit,LargeItemLimit,PrimaryOnly,SkipSteps,SourceEndpoint,TargetDeliveryDomain,TargetEndpoint"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "AllowIncrementalSyncs,AllowUnknownColumnsInCSV,AutoRetryCount,AutoStart,CSVData,CompleteAfter,Confirm,DisableOnCopy,DisallowExistingUsers,ErrorAction,ErrorVariable,ExcludeFolders,Name,NotificationEmails,OutBuffer,OutVariable,ReportInterval,StartAfter,TargetArchiveDatabases,TargetDatabases,TimeZone,UserIds,Users,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "Autodiscover,EmailAddress,ExchangeOutlookAnywhere,ExchangeRemoteMove,ExchangeServer,MailboxPermission,NspiServer,PSTImport,RPCProxyServer,SourceMailboxLegacyDN,TestMailbox"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Credentials,ErrorAction,ErrorVariable,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,Name,OutBuffer,OutVariable,RemoteServer,SkipVerification,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Authentication,Security"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapMigrationPermissions"
					}, "IMAP,Port")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ExchangeGuid")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "BadItemLimit,LargeItemLimit"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "AllowIncrementalSyncs,AllowUnknownColumnsInCSV,AutoRetryCount,CSVData,CompleteAfter,Confirm,DisallowExistingUsers,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,ReportInterval,StartAfter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions"
					}, "EmailAddress,ExchangeServer,NspiServer,SourceMailboxLegacyDN,TestMailbox"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Authentication,Confirm,Credentials,ErrorAction,ErrorVariable,Identity,MailboxPermission,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,OutBuffer,OutVariable,RPCProxyServer,RemoteServer,Security,SkipVerification,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapMigrationPermissions"
					}, "Port")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Validate,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-MigrationServerAvailability", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Authentication,Autodiscover,Confirm,Credentials,EmailAddress,Endpoint,ErrorAction,ErrorVariable,ExchangeOutlookAnywhere,ExchangeRemoteMove,ExchangeServer,Imap,MailboxPermission,OutBuffer,OutVariable,PSTImport,RPCProxyServer,Security,SourceMailboxLegacyDN,TestMailbox,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapMigrationPermissions"
					}, "FilePath,Port,RemoteServer")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Monitoring
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OAuthConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OrganizationDomain,OutBuffer,OutVariable,ReloadConfig,Service,TargetUri,UseCachedToken,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Move_Mailboxes
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,ErrorAction,ErrorVariable,Flags,Identity,MoveStatus,Offline,OrganizationalUnit,OutBuffer,OutVariable,RemoteHostName,ResultSize,SortBy,Suspend,SuspendWhenReadyToComplete,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptLargeDataLoss,ArchiveDomain,ArchiveOnly,BadItemLimit,BatchName,CompleteAfter,CompletedRequestAgeLimit,Confirm,ErrorAction,ErrorVariable,ForceOffline,Identity,IgnoreRuleLimitErrors,IncrementalSyncInterval,LargeItemLimit,OutBuffer,OutVariable,Outbound,PreventCompletion,PrimaryOnly,Remote,RemoteArchiveTargetDatabase,RemoteCredential,RemoteGlobalCatalog,RemoteHostName,RemoteOrganizationName,RemoteTargetDatabase,SkipMoving,StartAfter,Suspend,SuspendComment,SuspendWhenReadyToComplete,TargetDeliveryDomain,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendWhenReadyToComplete,WarningAction,WarningVariable,WhatIf")
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
					}, "ExchangeGuid")
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
					}, "AcceptLargeDataLoss,BadItemLimit,BatchName,CompleteAfter,CompletedRequestAgeLimit,Confirm,ErrorAction,ErrorVariable,IgnoreRuleLimitErrors,IncrementalSyncInterval,LargeItemLimit,OutBuffer,OutVariable,PreventCompletion,RemoteCredential,RemoteGlobalCatalog,RemoteHostName,SkipMoving,StartAfter,SuspendWhenReadyToComplete,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmails,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ConnectSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
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
					}, "ErrorAction,ErrorVariable,GetChildren,Identity,MailFolderOnly,OutBuffer,OutVariable,Recurse,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SendAddress", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AddressId,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity,Tag")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity")
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
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-ContactList", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "CSV,CSVData,CSVStream,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ConnectSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,LinkedIn,Mailbox,OAuthVerifier,OutBuffer,OutVariable,RedirectUri,RequestSecret,RequestToken,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"OwaFacebookEnabledRestrictions"
					}, "AppAuthorizationCode,Facebook")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
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
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,Parent,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ConnectSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-InboxRule", new RoleParameters[]
				{
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ConnectSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,LinkedIn,OAuthVerifier,OutBuffer,OutVariable,RedirectUri,RequestSecret,RequestToken,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"OwaFacebookEnabledRestrictions"
					}, "AppAuthorizationCode,Facebook")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,CopyToFolder,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,Identity,MarkAsRead,MarkImportance,MessageTypeMatches,MoveToFolder,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
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
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon")
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
					}, "Password"),
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
					}, "Confirm,DefaultReminderTime,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,WarningAction,WarningVariable,WeatherEnabled,WeatherLocations,WeatherUnit,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
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
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,IgnoreDefaultScope,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "SendAddressDefault")
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
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
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AcceptedDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
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
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,WarningAction,WarningVariable,WhatIf"),
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
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,Alias,Confirm,CustomAttribute1,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,DisplayName,EmailAddresses,ErrorAction,ErrorVariable,ExtensionCustomAttribute1,ExtensionCustomAttribute2,ExtensionCustomAttribute3,ExtensionCustomAttribute4,ExtensionCustomAttribute5,GrantSendOnBehalfTo,Identity,ManagedBy,MemberJoinRestriction,Name,OutBuffer,OutVariable,PrimarySmtpAddress,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,ReportToManagerEnabled,ReportToOriginatorEnabled,SendOofMessageToOriginatorEnabled,SimpleDisplayName,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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

		private class MyMailSubscriptions
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,SubscriptionType,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "DisplayName,EmailAddress,ErrorAction,ErrorVariable,Mailbox,Name,OutBuffer,OutVariable,Password,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,ErrorAction,ErrorVariable,Force,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUsername,Mailbox,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,ErrorAction,ErrorVariable,Force,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUsername,LeaveOnServer,Mailbox,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "Hotmail"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,ErrorAction,ErrorVariable,Force,Mailbox,Name,OutBuffer,OutVariable,Password,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "Imap"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "Pop")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "Eas"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Authentication,Confirm,ErrorAction,ErrorVariable,Force,IncrementalSyncInterval,Mailbox,Name,OutBuffer,OutVariable,Password,RemoteEmailAddress,RemoteServerName,RemoteServerPort,Security,SkipMerging,SmtpServerName,SmtpServerPort,TargetRootFolder,UserName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "Imap"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "Pop")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "Confirm,DisplayName,Enabled,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,Password,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,Enabled,ErrorAction,ErrorVariable,Force,Identity,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUserName,Mailbox,OutBuffer,OutVariable,ResendVerification,ValidateSecret,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "Confirm,DisplayName,EmailAddress,Enabled,ErrorAction,ErrorVariable,Force,Identity,IncomingAuth,IncomingPassword,IncomingPort,IncomingSecurity,IncomingServer,IncomingUserName,LeaveOnServer,Mailbox,OutBuffer,OutVariable,ResendVerification,ValidateSecret,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Authentication,Confirm,ErrorAction,ErrorVariable,Force,Identity,IncrementalSyncInterval,Mailbox,OutBuffer,OutVariable,Password,RemoteServerName,RemoteServerPort,Security,SkipMerging,SmtpServerName,SmtpServerPort,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "DisplayName,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,SharePointUrl,WarningAction,WarningVariable,WhatIf")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,ShowInMyClient,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestorIdentity,UseAppTokenOnly,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Compare-TextMessagingVerificationCode", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,VerificationCode,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
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
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-CalendarNotification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "CalendarUpdateNotification,CalendarUpdateSendDuringWorkHour,Confirm,DailyAgendaNotification,DailyAgendaNotificationSendTime,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,MeetingReminderNotification,MeetingReminderSendDuringWorkHour,NextDays,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,CountryRegionId,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,MobileOperatorId,NotificationPhoneNumber,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UMCloudServicePermissions"
					}, "PhoneNumber,PhoneProviderId,VerifyGlobalRoutingEntry"),
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
					}, "Identity")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,WarningAction,WarningVariable"),
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
					}, "Confirm,DefaultStateForUser,Enabled,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,DefaultStateForUser,Enabled,ErrorAction,ErrorVariable,Force,Name,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,WarningAction,WarningVariable"),
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
					}, "Confirm,DefaultStateForUser,Enabled,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,DefaultStateForUser,Enabled,ErrorAction,ErrorVariable,Force,Name,OrganizationApp,OutBuffer,OutVariable,ProvidedTo,UserList,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Client_Access
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceClass", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ToolInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Version,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AccessLevel,Characteristic,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,QueryString,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Action,AnyOfAuthenticationTypes,AnyOfClientIPAddressesOrRanges,AnyOfProtocols,AnyOfSourceTcpPortNumbers,Confirm,Debug,Enabled,ErrorAction,ErrorVariable,ExceptAnyOfAuthenticationTypes,ExceptAnyOfClientIPAddressesOrRanges,ExceptAnyOfProtocols,ExceptAnyOfSourceTcpPortNumbers,ExceptUserIsMemberOf,ExceptUsernameMatchesAnyOfPatterns,Name,OutBuffer,OutVariable,Priority,UserIsMemberOf,UserRecipientFilter,UsernameMatchesAnyOfPatterns,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AcceptSecurityIdentifierInformation,ApplicationIdentifier,Confirm,Enabled,ErrorAction,ErrorVariable,IssuerIdentifier,LinkedAccount,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AccessLevel,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AdminMailRecipients,AllowAccessForUnSupportedPlatform,Confirm,DefaultAccessLevel,ErrorAction,ErrorVariable,Identity,OtaNotificationMailInsert,OutBuffer,OutVariable,UserMailInsert,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ClientAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Action,AnyOfAuthenticationTypes,AnyOfClientIPAddressesOrRanges,AnyOfProtocols,AnyOfSourceTcpPortNumbers,Confirm,Debug,Enabled,ErrorAction,ErrorVariable,ExceptAnyOfAuthenticationTypes,ExceptAnyOfClientIPAddressesOrRanges,ExceptAnyOfProtocols,ExceptAnyOfSourceTcpPortNumbers,ExceptUserIsMemberOf,ExceptUsernameMatchesAnyOfPatterns,Identity,Name,OutBuffer,OutVariable,Priority,UserIsMemberOf,UserRecipientFilter,UsernameMatchesAnyOfPatterns,Verbose,WarningAction,WarningVariable,WhatIf")
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
					}, "AcceptSecurityIdentifierInformation,ApplicationIdentifier,Confirm,Enabled,ErrorAction,ErrorVariable,IssuerIdentifier,LinkedAccount,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-OrganizationCustomization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ConfigCustomizationsPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DevicePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpCompliancePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpComplianceRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldCompliancePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldComplianceRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PerimeterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AvailabilityConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OrgWideAccount,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"CalendarVersionStoreEnabled",
						"EOPPremiumRestrictions"
					}, "CalendarVersionStoreEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivityBasedAuthenticationTimeoutEnabled,ActivityBasedAuthenticationTimeoutInterval,ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled,AppsForOfficeEnabled,Confirm,ErrorAction,ErrorVariable,ExchangeNotificationEnabled,ExchangeNotificationRecipients,OAuth2ClientProfileEnabled,OutBuffer,OutVariable,WhatIf"),
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
					}, "DefaultPublicFolderAgeLimit,DefaultPublicFolderProhibitPostQuota,PublicFoldersEnabled,RemotePublicFolderMailboxes"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "SiteMailboxCreationURL")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PerimeterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,GatewayIPAddresses,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Identity,OWAAllowUserChoiceOfSigningCertificate,OWAAlwaysEncrypt,OWAAlwaysSign,OWABCCEncryptedEmailForking,OWACRLConnectionTimeout,OWACRLRetrievalTimeout,OWACheckCRLOnSend,OWAClearSign,OWACopyRecipientHeaders,OWADLExpansionTimeout,OWADisableCRLCheck,OWAEncryptTemporaryBuffers,OWAEncryptionAlgorithms,OWAForceSMIMEClientUpgrade,OWAIncludeCertificateChainAndRootCertificate,OWAIncludeCertificateChainWithoutRootCertificate,OWAIncludeSMIMECapabilitiesInMessage,OWAOnlyUseSmartCard,OWASenderCertificateAttributesToDisplay,OWASignedEmailCertificateInclusion,OWASigningAlgorithms,OWATripleWrapSignedEncryptedMail,OWAUseKeyIdentifier,OWAUseSecondaryProxiesWhenFindingCertificates,SMIMECertificateIssuingCA,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Transport_Settings
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridMailflow", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridMailflowDatacenterIPs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HybridMailflow", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"FFOMigrationInProgress_Restrictions"
					}, "CentralizedTransportEnabled,CertificateSubject,Confirm,ErrorAction,ErrorVariable,InboundIPs,OnPremisesFQDN,OutBuffer,OutVariable,OutboundDomains,SecureMailEnabled,WarningAction,WarningVariable,WhatIf")
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
					}, "ClearCategories,Confirm,ConvertDisclaimerWrapperToEml,DSNConversionMode,ErrorAction,ErrorVariable,ExternalDelayDsnEnabled,ExternalDsnDefaultLanguage,ExternalDsnLanguageDetectionEnabled,ExternalDsnMaxMessageAttachSize,ExternalDsnReportingAuthority,ExternalDsnSendHtml,ExternalPostmasterAddress,Force,HeaderPromotionModeSetting,InternalDelayDsnEnabled,InternalDsnDefaultLanguage,InternalDsnLanguageDetectionEnabled,InternalDsnMaxMessageAttachSize,InternalDsnReportingAuthority,InternalDsnSendHtml,JournalingReportNdrTo,OutBuffer,OutVariable,Rfc2231EncodingEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OAuthConnectivity", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AppOnly,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Mailbox,OrganizationDomain,OutBuffer,OutVariable,ReloadConfig,Service,TargetUri,UseCachedToken,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "AccessRights,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderItemStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IncludeDumpsterInfo,IncludeHierarchyInfo,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,ErrorAction,ErrorVariable,Identity,Name,OutBuffer,OutVariable,ResultSize,Status,Suspend,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"PilotingOrganization_Restrictions"
					}, "Verbose"),
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
					}, "Confirm,EformsLocaleId,ErrorAction,ErrorVariable,Mailbox,Name,OutBuffer,OutVariable,Path,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,AuthenticationMethod,BadItemLimit,BatchName,CSVData,CSVStream,CompletedRequestAgeLimit,Confirm,ErrorAction,ErrorVariable,LargeItemLimit,Name,OutBuffer,OutVariable,OutlookAnywhereHostName,RemoteCredential,RemoteMailboxLegacyDN,RemoteMailboxServerLegacyDN,SkipMerging,Suspend,SuspendComment,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Recurse,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Resume-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "DefaultPublicFolderMailbox,IsExcludedFromServingHierarchy,PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "DefaultPublicFolderAgeLimit,DefaultPublicFolderDeletedItemRetention,DefaultPublicFolderIssueWarningQuota,DefaultPublicFolderMaxItemSize,DefaultPublicFolderMovedItemRetention,DefaultPublicFolderProhibitPostQuota,PublicFoldersEnabled,RemotePublicFolderMailboxes")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AgeLimit,Confirm,EformsLocaleId,ErrorAction,ErrorVariable,Force,Identity,IssueWarningQuota,MailEnabled,MailRecipientGuid,MaxItemSize,Name,OutBuffer,OutVariable,Path,PerUserReadStateEnabled,ProhibitPostQuota,RetainDeletedItemsFor,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolderMailboxMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,BadItemLimit,Confirm,ErrorAction,ErrorVariable,Identity,LargeItemLimit,OutBuffer,OutVariable,SkipMerging,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "AcceptLargeDataLoss,AuthenticationMethod,BadItemLimit,BatchName,CompletedRequestAgeLimit,Confirm,ErrorAction,ErrorVariable,Identity,LargeItemLimit,OutBuffer,OutVariable,OutlookAnywhereHostName,PreventCompletion,RemoteCredential,RemoteMailboxLegacyDN,RemoteMailboxServerLegacyDN,SkipMerging,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Suspend-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SuspendComment,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-PublicFolderMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,FullSync,Identity,InvokeSynchronizer,OutBuffer,OutVariable,ReconcileFolders,SuppressStatus,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Recipient_Policies
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowApplePushNotifications,AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions"
					}, "AllowGooglePushNotifications,AllowMicrosoftPushNotifications"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowApplePushNotifications,AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IsDefault"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAMailboxPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowApplePushNotifications,AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions"
					}, "AllowGooglePushNotifications,AllowMicrosoftPushNotifications"),
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowApplePushNotifications,AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "DisableFacebook,InstantMessagingType"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures",
						"OwaFacebookEnabledRestrictions"
					}, "FacebookEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,IsDefault,JournalEnabled,LinkedInEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PremiumClientEnabled,PublicFoldersEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Remote_and_Accepted_Domains
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-SecondaryDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddSecondaryDomainPermissions",
						"EOPPremiumRestrictions"
					}, "AuthenticationType,Confirm,DomainName,ErrorAction,ErrorVariable,IsDataCenter,Name,OutBoundOnly,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "OrganizationGuid")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OnPremisesOrganization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "ErrorAction,ErrorVariable,Identity,IsTransportRuleScoped,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AuthenticationType")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "AssociatedAcceptedDomains,CloudServicesMailEnabled,Comment,Confirm,ConnectorSource,ConnectorType,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,RequireTls,RestrictDomainsToCertificate,RestrictDomainsToIPAddresses,SenderDomains,SenderIPAddresses,TlsSenderCertificateName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OnPremisesOrganization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,HybridDomains,InboundConnector,Name,OrganizationGuid,OrganizationName,OrganizationRelationship,OutBuffer,OutVariable,OutboundConnector,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OutboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "AllAcceptedDomains,CloudServicesMailEnabled,Comment,Confirm,ConnectorSource,ConnectorType,Enabled,ErrorAction,ErrorVariable,IsTransportRuleScoped,Name,OutBuffer,OutVariable,RecipientDomains,RouteAllMessagesViaOnPremises,SmartHosts,TlsDomain,TlsSettings,UseMXRecord,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DomainName,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-InboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OnPremisesOrganization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-OutboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,DomainType,EnableNego2Authentication,MatchSubDomains,OutboundOnly,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "AssociatedAcceptedDomains,CloudServicesMailEnabled,Comment,Confirm,ConnectorSource,ConnectorType,Enabled,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,RequireTls,RestrictDomainsToCertificate,RestrictDomainsToIPAddresses,SenderDomains,SenderIPAddresses,TlsSenderCertificateName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OnPremisesOrganization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,HybridDomains,InboundConnector,OrganizationName,OrganizationRelationship,OutBuffer,OutVariable,OutboundConnector,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ByteEncoderTypeFor7BitCharsets,PreferredInternetCodePageForShiftJis,RequiredCharsetCoverage")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OutboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "AllAcceptedDomains,CloudServicesMailEnabled,Comment,Confirm,ConnectorSource,ConnectorType,Enabled,ErrorAction,ErrorVariable,IsTransportRuleScoped,Name,OutBuffer,OutVariable,RecipientDomains,RouteAllMessagesViaOnPremises,SmartHosts,TlsDomain,TlsSettings,UseMXRecord,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllowedOOFType,AutoForwardEnabled,AutoReplyEnabled,ByteEncoderTypeFor7BitCharsets,CharacterSet,Confirm,ContentType,DeliveryReportEnabled,DisplaySenderName,ErrorAction,ErrorVariable,Identity,IsInternal,LineWrapSize,MeetingForwardNotificationEnabled,NDRDiagnosticInfoEnabled,NDREnabled,Name,NonMimeCharacterSet,OutBuffer,OutVariable,PreferredInternetCodePageForShiftJis,RequiredCharsetCoverage,TNEFEnabled,TargetDeliveryDomain,TrustedMailInboundEnabled,TrustedMailOutboundEnabled,UseSimpleDisplayName,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Reset_Password
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon")
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
					}, "RoomMailboxPassword"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon")
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
					}, "PublicFolder,ResetPasswordOnNextLogon")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Retention_Management
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
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
					}, "Confirm,ErrorAction,ErrorVariable,IsDefault,IsDefaultArbitrationMailbox,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgeLimitForRetention,Comment,Confirm,ErrorAction,ErrorVariable,IsDefaultAutoGroupPolicyTag,IsDefaultModeratedRecipientsPolicyTag,LocalizedComment,LocalizedRetentionPolicyTagName,MessageClass,MustDisplayCommentEnabled,Name,OutBuffer,OutVariable,RetentionAction,RetentionEnabled,RetentionId,SystemTag,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RetentionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "EnableRoomMailboxAccount,EndDateForRetentionHold,ErrorAction,ErrorVariable,OutBuffer,OutVariable,RetentionHoldEnabled,RetentionPolicy,SingleItemRecoveryEnabled,StartDateForRetentionHold,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"LitigationHoldPermissions"
					}, "LitigationHoldDate,LitigationHoldDuration,LitigationHoldOwner,RetentionComment,RetentionUrl")
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,IsDefault,IsDefaultArbitrationMailbox,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgeLimitForRetention,Comment,Confirm,ErrorAction,ErrorVariable,Force,Identity,LegacyManagedFolder,LocalizedComment,LocalizedRetentionPolicyTagName,Mailbox,MessageClass,MustDisplayCommentEnabled,Name,OptionalInMailbox,OutBuffer,OutVariable,RetentionAction,RetentionEnabled,RetentionId,SystemTag,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-ManagedFolderAssistant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,HoldCleanup,Identity,InactiveMailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-RetentionAutoTagLearning", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Clear,Confirm,CrossValidate,ErrorAction,ErrorVariable,Identity,NumberOfSegments,OutBuffer,OutVariable,Train,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Overwrite,Parameters,ParentRoleEntry,Role,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
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
					}, "OrganizationalUnit")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "Cmdlet,CmdletParameters,ErrorAction,ErrorVariable,GetChildren,Identity,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomRecipientWriteScope,Delegating,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,WarningAction,WarningVariable,WritableRecipient")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PSSnapinName,Parameters,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Exclusive,Identity,Orphan,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,ShowPartnerLinked,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,Parent,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomRecipientWriteScope,Delegating,ErrorAction,ErrorVariable,ExclusiveRecipientWriteScope,Force,Name,OutBuffer,OutVariable,Policy,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,Role,SecurityGroup,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Exclusive,Force,Name,OutBuffer,OutVariable,RecipientRestrictionFilter,RecipientRoot,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,IsDefault,Name,OutBuffer,OutVariable,Roles,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomRecipientWriteScope,Description,DisplayName,ErrorAction,ErrorVariable,Force,ManagedBy,Name,OutBuffer,OutVariable,Roles,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Recurse,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions",
						"RoleGroupMembershipRestrictions"
					}, "BypassSecurityGroupManagerCheck,Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,CustomRecipientWriteScope,Enabled,ErrorAction,ErrorVariable,ExclusiveRecipientWriteScope,Force,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientRelativeWriteScope,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "AddParameter,Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Parameters,RemoveParameter,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RBACManagementPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,RecipientRestrictionFilter,RecipientRoot,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"RoleAssignmentPolicyPermissions"
					}, "Confirm,Description,ErrorAction,ErrorVariable,Identity,IsDefault,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "BypassSecurityGroupManagerCheck,Confirm,Description,DisplayName,ErrorAction,ErrorVariable,Force,ManagedBy,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Confirm,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Alias,Confirm,CopyOwnerToMember,DisplayName,ErrorAction,ErrorVariable,ManagedBy,MemberJoinRestriction,Members,Name,Notes,OutBuffer,OutVariable,PrimarySmtpAddress,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "MemberDepartRestriction,OrganizationalUnit,RoomList,Type"),
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
					}, "BypassSecurityGroupManagerCheck,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "BypassSecurityGroupManagerCheck,Identity,Member")
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Supervision
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-SupervisionListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "Confirm,Entry,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Tag,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "Credential,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity,Tag")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "DisplayDetails,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-SupervisionListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "Confirm,Entry,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RemoveAll,Tag,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SupervisionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "AntiBullyingPolicyEnabled,BadWordsList,BadWordsPolicyEnabled,ClosedCampusInboundDomainExceptions,ClosedCampusInboundGroupExceptions,ClosedCampusInboundPolicyEnabled,ClosedCampusOutboundDomainExceptions,ClosedCampusOutboundGroupExceptions,ClosedCampusOutboundPolicyEnabled,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Anr,BypassOwnerCheck,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Alias,DisplayName,ErrorAction,ErrorVariable,Force,Name,OrganizationalUnit,OutBuffer,OutVariable,SharePointUrl,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Confirm,WhatIf")
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
					}, "Active,DisplayName,ErrorAction,ErrorVariable,Force,Members,OutBuffer,OutVariable,Owners,RemoveDuplicateMessages,SharePointUrl,ShowInMyClient,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "AliasPrefix,Confirm,DefaultAliasPrefixEnabled,Identity,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RequestorIdentity,UseAppTokenOnly,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,ErrorAction,ErrorVariable,FullSync,Identity,OutBuffer,OutVariable,Target,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Hygiene
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedConnectionFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedOutboundSpamFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QuarantineMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"QuarantineEnabled"
					}, "Confirm,Direction,Domain,EndExpiresDate,EndReceivedDate,ErrorAction,ErrorVariable,Identity,MessageId,OutBuffer,OutVariable,Page,PageSize,RecipientAddress,Reported,SenderAddress,StartExpiresDate,StartReceivedDate,Subject,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QuarantineMessageHeader", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"QuarantineEnabled"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HostedConnectionFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "AdminDisplayName,Confirm,EnableSafeList,ErrorAction,ErrorVariable,IPAllowList,IPBlockList,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HostedContentFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "AddXHeaderValue,AdminDisplayName,Confirm,DownloadLink,EnableEndUserSpamNotifications,EnableLanguageBlockList,EnableRegionBlockList,EndUserSpamNotificationCustomFromAddress,EndUserSpamNotificationCustomFromName,EndUserSpamNotificationCustomSubject,EndUserSpamNotificationFrequency,EndUserSpamNotificationLanguage,EndUserSpamNotificationLimit,ErrorAction,ErrorVariable,HighConfidenceSpamAction,IncreaseScoreWithBizOrInfoUrls,IncreaseScoreWithImageLinks,IncreaseScoreWithNumericIps,IncreaseScoreWithRedirectToOtherPort,LanguageBlockList,MarkAsSpamBulkMail,MarkAsSpamEmbedTagsInHtml,MarkAsSpamEmptyMessages,MarkAsSpamFormTagsInHtml,MarkAsSpamFramesInHtml,MarkAsSpamFromAddressAuthFail,MarkAsSpamJavaScriptInHtml,MarkAsSpamNdrBackscatter,MarkAsSpamObjectTagsInHtml,MarkAsSpamSensitiveWordList,MarkAsSpamSpfRecordHardFail,MarkAsSpamWebBugsInHtml,ModifySubjectValue,Name,OutBuffer,OutVariable,QuarantineRetentionPeriod,RedirectToRecipients,RegionBlockList,SpamAction,TestModeAction,TestModeBccToRecipients,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,Enabled,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,HostedContentFilterPolicy,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Action,AdminDisplayName,Confirm,CustomAlertText,CustomExternalBody,CustomExternalSubject,CustomFromAddress,CustomFromName,CustomInternalBody,CustomInternalSubject,CustomNotifications,EnableExternalSenderAdminNotifications,EnableExternalSenderNotifications,EnableInternalSenderAdminNotifications,EnableInternalSenderNotifications,ErrorAction,ErrorVariable,ExternalSenderAdminAddress,InternalSenderAdminAddress,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,Enabled,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,MalwareFilterPolicy,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Release-QuarantineMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"QuarantineEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReleaseToAll,ReportFalsePositive,User,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HostedConnectionFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HostedContentFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HostedConnectionFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "AdminDisplayName,Confirm,EnableSafeList,ErrorAction,ErrorVariable,IPAllowList,IPBlockList,Identity,MakeDefault,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HostedContentFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "AddXHeaderValue,AdminDisplayName,Confirm,DownloadLink,EnableEndUserSpamNotifications,EnableLanguageBlockList,EnableRegionBlockList,EndUserSpamNotificationCustomFromAddress,EndUserSpamNotificationCustomFromName,EndUserSpamNotificationCustomSubject,EndUserSpamNotificationFrequency,EndUserSpamNotificationLanguage,EndUserSpamNotificationLimit,ErrorAction,ErrorVariable,HighConfidenceSpamAction,Identity,IncreaseScoreWithBizOrInfoUrls,IncreaseScoreWithImageLinks,IncreaseScoreWithNumericIps,IncreaseScoreWithRedirectToOtherPort,LanguageBlockList,MarkAsSpamBulkMail,MarkAsSpamEmbedTagsInHtml,MarkAsSpamEmptyMessages,MarkAsSpamFormTagsInHtml,MarkAsSpamFramesInHtml,MarkAsSpamFromAddressAuthFail,MarkAsSpamJavaScriptInHtml,MarkAsSpamNdrBackscatter,MarkAsSpamObjectTagsInHtml,MarkAsSpamSensitiveWordList,MarkAsSpamSpfRecordHardFail,MarkAsSpamWebBugsInHtml,ModifySubjectValue,OutBuffer,OutVariable,QuarantineRetentionPeriod,RedirectToRecipients,RegionBlockList,SpamAction,TestModeAction,TestModeBccToRecipients,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,HostedContentFilterPolicy,Identity,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HostedOutboundSpamFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "AdminDisplayName,BccSuspiciousOutboundAdditionalRecipients,BccSuspiciousOutboundMail,BlockUnlistedDomains,Confirm,ErrorAction,ErrorVariable,Identity,NotifyOutboundSpam,NotifyOutboundSpamRecipients,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Action,AdminDisplayName,Confirm,CustomAlertText,CustomExternalBody,CustomExternalSubject,CustomFromAddress,CustomFromName,CustomInternalBody,CustomInternalSubject,CustomNotifications,EnableExternalSenderAdminNotifications,EnableExternalSenderNotifications,EnableInternalSenderAdminNotifications,EnableInternalSenderNotifications,ErrorAction,ErrorVariable,ExternalSenderAdminAddress,Identity,InternalSenderAdminAddress,MakeDefault,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "Comments,Confirm,ErrorAction,ErrorVariable,ExceptIfRecipientDomainIs,ExceptIfSentTo,ExceptIfSentToMemberOf,Identity,MalwareFilterPolicy,Name,OutBuffer,OutVariable,Priority,RecipientDomainIs,SentTo,SentToMemberOf,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mode,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,TrustedPublishingDomain,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Default,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,FileData,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ClassificationID,Confirm,DisplayName,DisplayPrecedence,ErrorAction,ErrorVariable,Locale,Name,OutBuffer,OutVariable,PermissionMenuVisible,RecipientDescription,RetainClassificationEnabled,SenderDescription,WarningAction,WarningVariable,WhatIf")
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
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,ApplyOME,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfRecipientInSenderList,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderInRecipientList,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RecipientInSenderList,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RemoveOME,RouteMessageOutboundConnector,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderInRecipientList,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-TransportRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ClassificationID,Confirm,DisplayName,DisplayPrecedence,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,PermissionMenuVisible,RecipientDescription,RetainClassificationEnabled,SenderDescription,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "DisclaimerText,EmailText,Image,OTPEnabled,PortalText")
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
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,ApplyOME,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfRecipientInSenderList,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderInRecipientList,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RecipientInSenderList,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RemoveOME,RouteMessageOutboundConnector,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderInRecipientList,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,WarningAction,WarningVariable,WhatIf,WithImportance")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMMailbox", new RoleParameters[]
				{
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
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AutomaticSpeechRecognitionEnabled,Confirm,ErrorAction,ErrorVariable,Extensions,Identity,NotifyEmail,OutBuffer,OutVariable,PilotNumber,Pin,PinExpired,SIPResourceIdentifier,UMMailboxPolicy,ValidateOnly,VoiceMailAnalysisEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllMailboxPlanReleases,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
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
						"UMPBXPermissions"
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPolicy", new RoleParameters[]
				{
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMCallAnsweringRule", new RoleParameters[]
				{
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
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,ImListMigrationCompleted,OutBuffer,OutVariable,SecondaryAddress,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMCallAnsweringRule", new RoleParameters[]
				{
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
						"UMCloudServicePermissions"
					}, "PhoneNumber,PhoneProviderId,VerifyGlobalRoutingEntry"),
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
					}, "Confirm,ErrorAction,ErrorVariable,LockedOut,NotifyEmail,OutBuffer,OutVariable,Pin,PinExpired,SendEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"RecipientManagementPermissions"
					}, "Confirm,PublicFolder,WhatIf"),
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PromptFileName,UMAutoAttendant,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Import-UMPrompt", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,PromptFileData,PromptFileName,PromptFileStream,UMAutoAttendant,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMAutoAttendant", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,InfoAnnouncementEnabled,InfoAnnouncementFilename,WarningAction,WarningVariable,WelcomeGreetingEnabled,WelcomeGreetingFilename,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UMAutoAttendantPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-UMIPGateway", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Immediate,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Enable-UMAutoAttendant", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ClientStream,Confirm,Date,ErrorAction,ErrorVariable,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllMailboxPlanReleases,ErrorAction,Filter,OrganizationalUnit,OutBuffer,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
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
					}, "ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,GroupBy,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMIPGateway", new RoleParameters[]
				{
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
						"UMPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPIN", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMAutoAttendant", new RoleParameters[]
				{
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
						"UMFaxPermissions"
					}, "FaxEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AccessTelephoneNumbers,Confirm,CountryOrRegionCode,DefaultLanguage,ErrorAction,ErrorVariable,GenerateUMMailboxPolicy,Name,NumberOfDigitsInExtension,OutBuffer,OutVariable,URIType,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMHuntGroup", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "Address,Confirm,ErrorAction,ErrorVariable,IPAddressFamily,Name,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-UMMailboxPolicy", new RoleParameters[]
				{
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
						"UMAutoAttendantPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMDialPlan", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-UMIPGateway", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "AfterHoursKeyMapping,AfterHoursKeyMappingEnabled,AfterHoursMainMenuCustomPromptEnabled,AfterHoursMainMenuCustomPromptFilename,AfterHoursTransferToOperatorEnabled,AfterHoursWelcomeGreetingEnabled,AfterHoursWelcomeGreetingFilename,AllowDialPlanSubscribers,AllowExtensions,AllowedInCountryOrRegionGroups,AllowedInternationalGroups,BusinessHoursKeyMapping,BusinessHoursKeyMappingEnabled,BusinessHoursMainMenuCustomPromptEnabled,BusinessHoursMainMenuCustomPromptFilename,BusinessHoursSchedule,BusinessHoursTransferToOperatorEnabled,BusinessHoursWelcomeGreetingEnabled,BusinessHoursWelcomeGreetingFilename,BusinessLocation,BusinessName,CallSomeoneEnabled,Confirm,ContactAddressList,ContactRecipientContainer,ContactScope,DTMFFallbackAutoAttendant,ErrorAction,ErrorVariable,ForceUpgrade,HolidaySchedule,Identity,InfoAnnouncementEnabled,InfoAnnouncementFilename,Language,MatchedNameSelectionMethod,Name,NameLookupEnabled,OperatorExtension,OutBuffer,OutVariable,PilotIdentifierList,SendVoiceMsgEnabled,SpeechEnabled,TimeZone,TimeZoneName,WarningAction,WarningVariable,WeekStartDay,WhatIf")
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
						"UMFaxPermissions"
					}, "FaxEnabled"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "AccessTelephoneNumbers,AllowDialPlanSubscribers,AllowExtensions,AllowHeuristicADCallingLineIdResolution,AllowedInCountryOrRegionGroups,AllowedInternationalGroups,AudioCodec,AutomaticSpeechRecognitionEnabled,CallSomeoneEnabled,ConfiguredInCountryOrRegionGroups,ConfiguredInternationalGroups,Confirm,ContactAddressList,ContactRecipientContainer,ContactScope,CountryOrRegionCode,DefaultLanguage,DialByNamePrimary,DialByNameSecondary,EquivalentDialPlanPhoneContexts,ErrorAction,ErrorVariable,Extension,ForceUpgrade,InCountryOrRegionNumberFormat,InfoAnnouncementEnabled,InfoAnnouncementFilename,InputFailuresBeforeDisconnect,InternationalAccessCode,InternationalNumberFormat,LegacyPromptPublishingPoint,LogonFailuresBeforeDisconnect,MatchedNameSelectionMethod,MaxCallDuration,MaxRecordingDuration,Name,NationalNumberPrefix,NumberingPlanFormats,OperatorExtension,OutBuffer,OutVariable,OutsideLineAccessCode,PilotIdentifierList,RecordingIdleTimeout,SendVoiceMsgEnabled,TUIPromptEditingEnabled,UMAutoAttendant,WarningAction,WarningVariable,WelcomeGreetingEnabled,WelcomeGreetingFilename,WhatIf"),
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Clear-MobileDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions",
						"MOWADeviceDataAccessPermissions"
					}, "Cancel,Confirm,ErrorAction,ErrorVariable,Identity,NotificationEmailAddresses,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Disable-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
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
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
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
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionListEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity,Tag")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,Preview,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Confirm,Enabled,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "AllowReadWriteMailbox,FileData,FileStream,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Body,BodyFormat,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Subject,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf"),
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
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
						"UserMailboxAccessPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ActiveSyncDebugLogging")
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
						"EOPPremiumRestrictions",
						"InboxRuleCreationRestrictions",
						"UserMailboxAccessPermissions"
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromSubscription,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromSubscription,HasAttachment,HasClassification,HeaderContainsWords,Identity,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
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
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password,ResetPasswordOnNextLogon")
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
					}, "Password"),
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
					}, "Confirm,DefaultReminderTime,ErrorAction,ErrorVariable,FirstWeekOfYear,OutBuffer,OutVariable,ReminderSoundEnabled,RemindersEnabled,ShowWeekNumbers,TimeIncrement,WarningAction,WarningVariable,WeatherEnabled,WeatherLocations,WeatherUnit,WeekStartDay,WhatIf,WorkingHoursEndTime,WorkingHoursStartTime,WorkingHoursTimeZone"),
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
					}, "BlockedSendersAndDomains,Confirm,ContactsTrusted,Enabled,ErrorAction,ErrorVariable,IgnoreDefaultScope,OutBuffer,OutVariable,TrustedListsOnly,TrustedSendersAndDomains,WarningAction,WarningVariable,WhatIf")
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
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,IgnoreDefaultScope,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "SendAddressDefault")
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
					}, "Identity"),
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
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,Confirm,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailboxAuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,EndDate,ErrorAction,ErrorVariable,ExternalAccess,LogonTypes,Mailboxes,Name,Operations,OutBuffer,OutVariable,ShowDetails,StartDate,StatusMailRecipients,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Cmdlets,EndDate,ErrorAction,ErrorVariable,ExternalAccess,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MailboxAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,LogonTypes,Mailboxes,Operations,OutBuffer,OutVariable,ResultSize,ShowDetails,StartDate,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class View_Only_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-TransportRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-UMCallDataRecord", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ClientStream,Confirm,Date,ErrorAction,ErrorVariable,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,WarningAction,WarningVariable,WhatIf")
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
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceClass", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncOrganizationSettings", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OrganizationApp,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Identity,Mailbox")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditLogSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "CreatedAfter,CreatedBefore,Debug,ErrorAction,ErrorVariable,Identity,ResultSize,Type,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuthServer", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ClassificationRuleCollection", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ClassificationRuleCollectionIdentity,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DataClassificationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DevicePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpCompliancePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpComplianceRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpPolicyTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FailedContentIndexDocuments", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,Confirm,EndDate,ErrorAction,ErrorCode,ErrorVariable,FailureMode,Identity,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederatedOrganizationIdentifier", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeExtendedDomainInfo,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "BypassAdditionalDomainValidation,DomainName,ErrorAction,ErrorVariable,Force,OutBuffer,OutVariable,TrustedHostnames,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FederationTrust", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldCompliancePolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldComplianceRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedConnectionFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedContentFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HostedOutboundSpamFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"HostedSpamFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridMailflow", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HybridMailflowDatacenterIPs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IRMConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OrganizationGuid,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-IntraOrganizationConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-JournalRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"JournalingRulesPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAuditBypassAssociation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MalwareFilterRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MalwareFilteringPolicyCustomizationEnabled",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Cmdlet,CmdletParameters,ErrorAction,ErrorVariable,GetChildren,Identity,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleAssignment", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AssignmentMethod,ConfigWriteScope,CustomRecipientWriteScope,Delegating,Enabled,ErrorAction,ErrorVariable,Exclusive,ExclusiveRecipientWriteScope,GetEffectiveUsers,Identity,OutBuffer,OutVariable,RecipientOrganizationalUnitScope,RecipientWriteScope,Role,RoleAssignee,RoleAssigneeType,WarningAction,WarningVariable,WritableRecipient")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRoleEntry", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,PSSnapinName,Parameters,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementScope", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Exclusive,Identity,Orphan,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageCategory", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Diagnostic,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,Identity,LimitSkippedItemsTo,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Notification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ProcessType,ResultSize,StartDate,Summary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OMEConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Debug")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OnPremisesOrganization", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutboundConnector", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"ServiceConnectors"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutlookProtectionRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"IRMPremiumFeaturesPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerApplication", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PerimeterConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PolicyTipConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"DataLossPreventionEnabled",
						"EXOStandardRestrictions",
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions"
					}, "Action,ErrorAction,ErrorVariable,Identity,Locale,Original,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTemplate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,TrustedPublishingDomain,Type,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RMSTrustedPublishingDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Default,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientStatisticsReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleAssignmentPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,ShowPartnerLinked,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SearchDocumentFormat", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SendAddress", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AddressId,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceStatus", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,MaintenanceWindowDays,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SharingPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "Anr,BypassOwnerCheck,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "BypassOwnerCheck,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,SendMeEmail,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SiteMailboxProvisioningPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Debug,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SupervisionPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions"
					}, "DisplayDetails,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SupervisionPermissions",
						"ViewSupervisionListPermissions"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ToolInformation", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Version,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,State,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRuleAction", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportRulePredicate", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"TransportRulesPermissions"
					}, "ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMAutoAttendant", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMAutoAttendantPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,GroupBy,OutBuffer,OutVariable,UMDialPlan,UMIPGateway,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMDialPlan", new RoleParameters[]
				{
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
						"UMPBXPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMIPGateway", new RoleParameters[]
				{
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
						"UMPermissions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UMDialPlan,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-SmimeConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,Debug,Identity,OWAAllowUserChoiceOfSigningCertificate,OWAAlwaysEncrypt,OWAAlwaysSign,OWABCCEncryptedEmailForking,OWACRLConnectionTimeout,OWACRLRetrievalTimeout,OWACheckCRLOnSend,OWAClearSign,OWACopyRecipientHeaders,OWADLExpansionTimeout,OWADisableCRLCheck,OWAEncryptTemporaryBuffers,OWAEncryptionAlgorithms,OWAForceSMIMEClientUpgrade,OWAIncludeCertificateChainAndRootCertificate,OWAIncludeCertificateChainWithoutRootCertificate,OWAIncludeSMIMECapabilitiesInMessage,OWAOnlyUseSmartCard,OWASenderCertificateAttributesToDisplay,OWASignedEmailCertificateInclusion,OWASigningAlgorithms,OWATripleWrapSignedEncryptedMail,OWAUseKeyIdentifier,OWAUseSecondaryProxiesWhenFindingCertificates,SMIMECertificateIssuingCA,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Test-OrganizationRelationship", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,UserIdentity,WarningAction,WarningVariable,WhatIf")
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
						"UMPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Export-MailboxDiagnosticLogs", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Archive,ComponentName,Confirm,ExtendedProperties,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDevice", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Filter,Identity,Mailbox,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncDeviceStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncDeviceDataAccessPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,GetMailboxLog,Identity,Mailbox,NotificationEmailAddresses,OutBuffer,OutVariable,ShowRecoveryPassword,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ActiveSyncMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CASMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarDiagnosticLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,Identity,Latest,LogLocation,MeetingID,OutBuffer,OutVariable,ResultSize,StartDate,Subject,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CalendarProcessing", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ConnectSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ConnectionByClientTypeDetailReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ConnectionByClientTypeReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Contact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsAVConferenceTimeReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsActiveUserReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsClientDeviceReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsConferenceReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsP2PAVTimeReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-CsP2PSessionReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DistributionGroupMember", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DynamicDistributionGroup", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,ManagedBy,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExternalActivityByDomainReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExternalActivityByUserReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExternalActivityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ExternalActivitySummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-FfoMigrationReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,WarningAction,WarningVariable")
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
					}, "OrganizationalUnit")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-GroupActivityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,JobId,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HotmailSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ImapSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-InboxRule", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "DescriptionTimeFormat,DescriptionTimeZone,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LicenseVsUsageSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LinkedUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-LogonStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailContact", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailMalwareReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MalwareName,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailSpamReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailDetailTransportRuleReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailFilterListReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Domain,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,ProbeTag,SelectionTarget,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailPublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficPolicyReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,AggregateBy,Direction,DlpPolicy,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Action,AggregateBy,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Category,DlpPolicy,Domain,EndDate,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailTrafficTopReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "AggregateBy,Direction,Domain,EndDate,ErrorAction,ErrorVariable,EventType,Expression,OutBuffer,OutVariable,Page,PageSize,ProbeTag,StartDate,SummarizeBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,InactiveMailboxOnly,IncludeInactiveMailbox,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SoftDeletedMailbox,SortBy,WarningAction,WarningVariable"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "PublicFolder")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxActivityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxAutoReplyConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxFolderPermission", new RoleParameters[]
				{
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
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "IncludeAnalysis,IncludeOldestAndNewestItems")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxJunkEmailConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxMessageConfiguration", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Owner,ReadFromDomainController,ResultSize,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AllMailboxPlanReleases,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
						"ArchivePermissions",
						"EOPPremiumRestrictions"
					}, "Archive"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeMoveHistory,IncludeMoveReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxUsageDetailReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MailboxUsageReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrace", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "EndDate,ErrorAction,ErrorVariable,Expression,FromIP,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,Status,ToIP,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTraceDetail", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "Action,EndDate,ErrorAction,ErrorVariable,Event,Expression,MessageId,MessageTraceId,OutBuffer,OutVariable,Page,PageSize,ProbeTag,RecipientAddress,SenderAddress,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,DoNotResolve,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientPathFilter,Recipients,ReportTemplate,ResultSize,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationBatch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,Endpoint,ErrorAction,ErrorVariable,Identity,IncludeReport,LimitErrorsTo,OutBuffer,OutVariable,Status,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationEndpoint", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchStatus,Confirm,ConnectionSettings,Diagnostic,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUser", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "BatchId,ErrorAction,ErrorVariable,Identity,MailboxGuid,OutBuffer,OutVariable,ResultSize,Status,StatusSummary,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MigrationUserStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Diagnostic,ErrorAction,ErrorVariable,Identity,LimitSkippedItemsTo,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceDashboardSummaryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceDetailsReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Page,PageSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
						"MOWADeviceDataAccessPermissions"
					}, "OWAforDevices")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "BatchName,ErrorAction,ErrorVariable,Flags,Identity,MoveStatus,Offline,OrganizationalUnit,OutBuffer,OutVariable,RemoteHostName,ResultSize,SortBy,Suspend,SuspendWhenReadyToComplete,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MoveRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MxRecordReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Domain,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,ProbeTag,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-O365ClientBrowserDetailReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Browser,BrowserVersion,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable,WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-O365ClientBrowserReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Browser,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-O365ClientOSDetailReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OperatingSystem,OperatingSystemVersion,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable,WindowsLiveID")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-O365ClientOSReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OS,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OutboundConnectorReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Domain,ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,ProbeTag,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerClientExpiringSubscriptionReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerCustomerUserReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PopSubscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolder", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,GetChildren,Identity,Mailbox,OutBuffer,OutVariable,Recurse,ResidentFolders,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderClientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,User,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderItemStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,IncludeDumpsterInfo,IncludeHierarchyInfo,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "BatchName,ErrorAction,ErrorVariable,Identity,Name,OutBuffer,OutVariable,ResultSize,Status,Suspend,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMigrationRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QuarantineMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"QuarantineEnabled"
					}, "Confirm,Direction,Domain,EndExpiresDate,EndReceivedDate,ErrorAction,ErrorVariable,Identity,MessageId,OutBuffer,OutVariable,Page,PageSize,RecipientAddress,Reported,SenderAddress,StartExpiresDate,StartReceivedDate,Subject,Type,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-QuarantineMessageHeader", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"PilotingOrganization_Restrictions",
						"QuarantineEnabled"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,AuthenticationType,BookmarkDisplayName,Capabilities,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOActiveUserReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOOneDriveForBusinessFileActivityReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOOneDriveForBusinessUserStatisticsReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOSkyDriveProDeployedReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOSkyDriveProStorageReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOTeamSiteDeployedReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOTeamSiteStorageReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SPOTenantStorageMetricReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ReportType,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ScorecardClientDeviceReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Category,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ScorecardClientOSReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Category,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ScorecardClientOutlookReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Category,EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ScorecardMetricsReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ServiceDeliveryReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Expression,OutBuffer,OutVariable,ProbeTag,Recipient,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-StaleMailboxDetailReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-StaleMailboxReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "EndDate,ErrorAction,ErrorVariable,OutBuffer,OutVariable,ResultSize,StartDate,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Subscription", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "AggregationType,Confirm,ErrorAction,ErrorVariable,Identity,IncludeReport,Mailbox,OutBuffer,OutVariable,ResultSize,SubscriptionType,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequest", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Mailbox,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SyncRequestStatistics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"HotmailSyncPermissions",
						"ImapSyncPermissions",
						"PopSyncPermissions"
					}, "Diagnostic,DiagnosticArgument,ErrorAction,ErrorVariable,Identity,IncludeReport,OutBuffer,OutVariable,RequestGuid,RequestQueue,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMCallAnsweringRule", new RoleParameters[]
				{
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
					}, "ErrorAction,ErrorVariable,Mailbox,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailbox", new RoleParameters[]
				{
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
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,IgnoreErrors,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UMMailboxPlan", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UMPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-UserPhoto", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"UserMailboxAccessPermissions"
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,OrganizationalUnit,OutBuffer,OutVariable,Preview,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,DoNotResolve,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "DLPPolicy,DeliveryStatus,Direction,EndDate,ErrorAction,ErrorVariable,Locale,MessageID,NotifyAddress,OriginalClientIP,OutBuffer,OutVariable,RecipientAddress,ReportTitle,ReportType,SenderAddress,StartDate,TransportRule,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-HistoricalSearch", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"FFOMigrationInProgress_Restrictions",
						"MessageTrace",
						"PilotingOrganization_Restrictions"
					}, "ErrorAction,ErrorVariable,JobId,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c")
			};
		}
	}
}
