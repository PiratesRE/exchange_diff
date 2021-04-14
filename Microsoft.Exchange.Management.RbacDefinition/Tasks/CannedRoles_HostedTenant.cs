using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class CannedRoles_HostedTenant
	{
		internal static RoleDefinition[] Definition = new RoleDefinition[]
		{
			new RoleDefinition("Address Lists", RoleType.AddressLists, CannedRoles_HostedTenant.Address_Lists.Cmdlets),
			new RoleDefinition("ApplicationImpersonation", RoleType.ApplicationImpersonation, CannedRoles_HostedTenant.ApplicationImpersonation.Cmdlets),
			new RoleDefinition("Audit Logs", RoleType.AuditLogs, CannedRoles_HostedTenant.Audit_Logs.Cmdlets),
			new RoleDefinition("Data Loss Prevention", RoleType.DataLossPrevention, CannedRoles_HostedTenant.Data_Loss_Prevention.Cmdlets),
			new RoleDefinition("Distribution Groups", RoleType.DistributionGroups, CannedRoles_HostedTenant.Distribution_Groups.Cmdlets),
			new RoleDefinition("Journaling", RoleType.Journaling, CannedRoles_HostedTenant.Journaling.Cmdlets),
			new RoleDefinition("Legal Hold", RoleType.LegalHold, CannedRoles_HostedTenant.Legal_Hold.Cmdlets),
			new RoleDefinition("Mail Recipient Creation", RoleType.MailRecipientCreation, CannedRoles_HostedTenant.Mail_Recipient_Creation.Cmdlets),
			new RoleDefinition("Mail Recipients", RoleType.MailRecipients, CannedRoles_HostedTenant.Mail_Recipients.Cmdlets),
			new RoleDefinition("Mail Tips", RoleType.MailTips, CannedRoles_HostedTenant.Mail_Tips.Cmdlets),
			new RoleDefinition("Mailbox Import Export", RoleType.MailboxImportExport, CannedRoles_HostedTenant.Mailbox_Import_Export.Cmdlets),
			new RoleDefinition("Mailbox Search", RoleType.MailboxSearch, CannedRoles_HostedTenant.Mailbox_Search.Cmdlets),
			new RoleDefinition("Message Tracking", RoleType.MessageTracking, CannedRoles_HostedTenant.Message_Tracking.Cmdlets),
			new RoleDefinition("Migration", RoleType.Migration, CannedRoles_HostedTenant.Migration.Cmdlets),
			new RoleDefinition("Move Mailboxes", RoleType.MoveMailboxes, CannedRoles_HostedTenant.Move_Mailboxes.Cmdlets),
			new RoleDefinition("My Custom Apps", RoleType.MyCustomApps, CannedRoles_HostedTenant.My_Custom_Apps.Cmdlets),
			new RoleDefinition("My Marketplace Apps", RoleType.MyMarketplaceApps, CannedRoles_HostedTenant.My_Marketplace_Apps.Cmdlets),
			new RoleDefinition("My ReadWriteMailbox Apps", RoleType.MyReadWriteMailboxApps, CannedRoles_HostedTenant.My_ReadWriteMailbox_Apps.Cmdlets),
			new RoleDefinition("MyAddressInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_HostedTenant.MyAddressInformation.Cmdlets),
			new RoleDefinition("MyBaseOptions", RoleType.MyBaseOptions, CannedRoles_HostedTenant.MyBaseOptions.Cmdlets),
			new RoleDefinition("MyContactInformation", RoleType.MyContactInformation, CannedRoles_HostedTenant.MyContactInformation.Cmdlets),
			new RoleDefinition("MyDisplayName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_HostedTenant.MyDisplayName.Cmdlets),
			new RoleDefinition("MyDistributionGroupMembership", RoleType.MyDistributionGroupMembership, CannedRoles_HostedTenant.MyDistributionGroupMembership.Cmdlets),
			new RoleDefinition("MyDistributionGroups", RoleType.MyDistributionGroups, CannedRoles_HostedTenant.MyDistributionGroups.Cmdlets),
			new RoleDefinition("MyMailboxDelegation", RoleType.MyMailboxDelegation, CannedRoles_HostedTenant.MyMailboxDelegation.Cmdlets),
			new RoleDefinition("MyMobileInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_HostedTenant.MyMobileInformation.Cmdlets),
			new RoleDefinition("MyName", "MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_HostedTenant.MyName.Cmdlets),
			new RoleDefinition("MyPersonalInformation", "MyContactInformation", RoleType.MyContactInformation, CannedRoles_HostedTenant.MyPersonalInformation.Cmdlets),
			new RoleDefinition("MyProfileInformation", RoleType.MyProfileInformation, CannedRoles_HostedTenant.MyProfileInformation.Cmdlets),
			new RoleDefinition("MyRetentionPolicies", RoleType.MyRetentionPolicies, CannedRoles_HostedTenant.MyRetentionPolicies.Cmdlets),
			new RoleDefinition("MyTeamMailboxes", RoleType.MyTeamMailboxes, CannedRoles_HostedTenant.MyTeamMailboxes.Cmdlets),
			new RoleDefinition("MyTextMessaging", RoleType.MyTextMessaging, CannedRoles_HostedTenant.MyTextMessaging.Cmdlets),
			new RoleDefinition("MyVoiceMail", RoleType.MyVoiceMail, CannedRoles_HostedTenant.MyVoiceMail.Cmdlets),
			new RoleDefinition("Org Custom Apps", RoleType.OrgCustomApps, CannedRoles_HostedTenant.Org_Custom_Apps.Cmdlets),
			new RoleDefinition("Org Marketplace Apps", RoleType.OrgMarketplaceApps, CannedRoles_HostedTenant.Org_Marketplace_Apps.Cmdlets),
			new RoleDefinition("Organization Client Access", RoleType.OrganizationClientAccess, CannedRoles_HostedTenant.Organization_Client_Access.Cmdlets),
			new RoleDefinition("Organization Configuration", RoleType.OrganizationConfiguration, CannedRoles_HostedTenant.Organization_Configuration.Cmdlets),
			new RoleDefinition("Organization Transport Settings", RoleType.OrganizationTransportSettings, CannedRoles_HostedTenant.Organization_Transport_Settings.Cmdlets),
			new RoleDefinition("Recipient Policies", RoleType.RecipientPolicies, CannedRoles_HostedTenant.Recipient_Policies.Cmdlets),
			new RoleDefinition("Remote and Accepted Domains", RoleType.RemoteAndAcceptedDomains, CannedRoles_HostedTenant.Remote_and_Accepted_Domains.Cmdlets),
			new RoleDefinition("Reset Password", RoleType.ResetPassword, CannedRoles_HostedTenant.Reset_Password.Cmdlets),
			new RoleDefinition("Retention Management", RoleType.RetentionManagement, CannedRoles_HostedTenant.Retention_Management.Cmdlets),
			new RoleDefinition("Role Management", RoleType.RoleManagement, CannedRoles_HostedTenant.Role_Management.Cmdlets),
			new RoleDefinition("Security Group Creation and Membership", RoleType.SecurityGroupCreationAndMembership, CannedRoles_HostedTenant.Security_Group_Creation_and_Membership.Cmdlets),
			new RoleDefinition("Team Mailboxes", RoleType.TeamMailboxes, CannedRoles_HostedTenant.Team_Mailboxes.Cmdlets),
			new RoleDefinition("Transport Rules", RoleType.TransportRules, CannedRoles_HostedTenant.Transport_Rules.Cmdlets),
			new RoleDefinition("User Options", RoleType.UserOptions, CannedRoles_HostedTenant.User_Options.Cmdlets),
			new RoleDefinition("View-Only Audit Logs", RoleType.ViewOnlyAuditLogs, CannedRoles_HostedTenant.View_Only_Audit_Logs.Cmdlets),
			new RoleDefinition("View-Only Configuration", RoleType.ViewOnlyConfiguration, CannedRoles_HostedTenant.View_Only_Configuration.Cmdlets),
			new RoleDefinition("View-Only Recipients", RoleType.ViewOnlyRecipients, CannedRoles_HostedTenant.View_Only_Recipients.Cmdlets)
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-MessageClassification", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeLocales,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,WarningAction,WarningVariable,WhatIf,WithImportance")
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
					}, "Identity"),
					new RoleParameters(new string[]
					{
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,WarningAction,WarningVariable,WhatIf,WithImportance")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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

		private class Mail_Recipient_Creation
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AddressBookPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"AddressBookPolicyPermissions",
						"EOPPremiumRestrictions"
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
					}, "Anr,ErrorAction,ErrorVariable,Filter,Identity,MailboxPlan,OrganizationalUnit,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable"),
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
					}, "Alias,Confirm,DisplayName,ErrorAction,ErrorVariable,ExternalEmailAddress,FirstName,ImmutableId,Initials,LastName,MacAttachmentFormat,MessageBodyFormat,MessageFormat,Name,OrganizationalUnit,OutBuffer,OutVariable,RemotePowerShellEnabled,UsePreferMessageFormat,UserPrincipalName,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"ModeratedRecipientsPermissions"
					}, "ModeratedBy,ModerationEnabled,SendModerationNotifications"),
					new RoleParameters(new string[]
					{
						"NewUserPasswordManagementPermissions"
					}, "Password")
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
					}, "EnableRoomMailboxAccount,RoomMailboxPassword,UserPrincipalName"),
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
						"PilotingOrganization_Restrictions",
						"PublicFoldersEnabled"
					}, "HoldForMigration,IsExcludedFromServingHierarchy,PublicFolder"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RecipientManagementPermissions"
					}, "Alias,Confirm,Discovery,DisplayName,Equipment,ErrorAction,ErrorVariable,FirstName,Force,ImmutableId,Initials,LastName,MailboxPlan,Name,OrganizationalUnit,OutBuffer,OutVariable,RemotePowerShellEnabled,Room,Shared,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PilotingOrganization_Restrictions",
						"RoleAssignmentPolicyPermissions"
					}, "RoleAssignmentPolicy")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,PublicFolder,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MailboxFolderPermission", new RoleParameters[]
				{
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
					}, "Identity")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RecipientPermission", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AccessRights,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,Trustee,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
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
					}, "Confirm,ErrorAction,ErrorVariable,ForceUpgrade,MacAttachmentFormat,MessageBodyFormat,MessageFormat,OutBuffer,OutVariable,SecondaryAddress,SimpleDisplayName,UseMapiRichTextFormat,UsePreferMessageFormat,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
					}, "AcceptMessagesOnlyFrom,AcceptMessagesOnlyFromDLMembers,AcceptMessagesOnlyFromSendersOrMembers,ApplyMandatoryProperties,CalendarRepairDisabled,CalendarVersionStoreDisabled,Confirm,DeliverToMailboxAndForward,EnableRoomMailboxAccount,EndDateForRetentionHold,ErrorAction,ErrorVariable,ExternalOofOptions,Force,ForwardingSmtpAddress,GrantSendOnBehalfTo,Languages,MessageCopyForSendOnBehalfEnabled,MessageCopyForSentAsEnabled,OutBuffer,OutVariable,RejectMessagesFrom,RejectMessagesFromDLMembers,RejectMessagesFromSendersOrMembers,RequireSenderAuthenticationEnabled,ResourceCapacity,ResourceCustom,RetentionHoldEnabled,RetentionPolicy,RoomMailboxPassword,RulesQuota,SecondaryAddress,SharingPolicy,SimpleDisplayName,SingleItemRecoveryEnabled,StartDateForRetentionHold,Type,WarningAction,WarningVariable,WhatIf,WindowsEmailAddress"),
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
					}, "PublicFolder"),
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
					}, "AfterMoveOrDeleteBehavior,AlwaysShowBcc,AlwaysShowFrom,AutoAddSignature,AutoAddSignatureOnMobile,CheckForForgottenAttachments,Confirm,ConversationSortOrder,DefaultFontColor,DefaultFontFlags,DefaultFontName,DefaultFontSize,DefaultFormat,EmailComposeMode,EmptyDeletedItemsOnLogoff,ErrorAction,ErrorVariable,HideDeletedItems,NewItemNotification,OutBuffer,OutVariable,PreviewMarkAsReadBehavior,PreviewMarkAsReadDelaytime,ReadReceiptResponse,ShowConversationAsTree,SignatureHtml,SignatureText,SignatureTextOnMobile,UseDefaultSignatureOnMobile,WarningAction,WarningVariable,WhatIf")
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
					}, "InstantMessagingType"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,IsDefault,JournalEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PremiumClientEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-User", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity,SeniorityIndex"),
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
					}, "Confirm,PublicFolder,WhatIf,WindowsEmailAddress")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-UserPhoto", new RoleParameters[]
				{
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
					}, "Archive,Confirm,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MailTipsPermissions"
					}, "MailTipsExternalRecipientsTipsEnabled,MailTipsLargeAudienceThreshold")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-Mailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SearchMessagePermissions"
					}, "Confirm,DeleteContent,DoNotIncludeArchive,ErrorAction,ErrorVariable,EstimateResultOnly,Force,Identity,IncludeUnsearchableItems,LogLevel,LogOnly,OutBuffer,OutVariable,SearchDumpster,SearchDumpsterOnly,SearchQuery,TargetFolder,TargetMailbox,WarningAction,WarningVariable,WhatIf")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,DoNotResolve,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,WarningAction,WarningVariable,WhatIf")
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
					}, "AllowIncrementalSyncs,AllowUnknownColumnsInCSV,AutoRetryCount,AutoStart,CSVData,CompleteAfter,Confirm,DisableOnCopy,DisallowExistingUsers,ErrorAction,ErrorVariable,Name,NotificationEmails,OutBuffer,OutVariable,ReportInterval,StartAfter,TargetArchiveDatabases,TargetDatabases,TimeZone,UserIds,Users,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,Credentials,ErrorAction,ErrorVariable,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,Name,OutBuffer,OutVariable,RemoteServer,SkipVerification,WarningAction,WarningVariable,WhatIf")
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
						"ExchangeMigrationPermissions",
						"HotmailMigrationPermissions",
						"ImapMigrationPermissions"
					}, "Confirm,Credentials,ErrorAction,ErrorVariable,Identity,MaxConcurrentIncrementalSyncs,MaxConcurrentMigrations,OutBuffer,OutVariable,RemoteServer,SkipVerification,WarningAction,WarningVariable,WhatIf")
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
					}, "Autodiscover,Confirm,Credentials,EmailAddress,Endpoint,ErrorAction,ErrorVariable,ExchangeRemoteMove,OutBuffer,OutVariable,PSTImport,WarningAction,WarningVariable,WhatIf"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"ImapMigrationPermissions"
					}, "FilePath,RemoteServer")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
						"UserMailboxAccessPermissions"
					}, "Anr,Credential,ErrorAction,ErrorVariable,Filter,Identity,IgnoreDefaultScope,OutBuffer,OutVariable,Preview,ReadFromDomainController,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-App", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-InboxRule", new RoleParameters[]
				{
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-InboxRule", new RoleParameters[]
				{
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
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
				}, "c")
			};
		}

		private class MyVoiceMail
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
					}, "Identity")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ActivityBasedAuthenticationTimeoutEnabled,ActivityBasedAuthenticationTimeoutInterval,ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled,AppsForOfficeEnabled,Confirm,ErrorAction,ErrorVariable,OAuth2ClientProfileEnabled,OutBuffer,OutVariable,WhatIf"),
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
					}, "DefaultPublicFolderAgeLimit,DefaultPublicFolderProhibitPostQuota,RemotePublicFolderMailboxes"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"TeamMailboxPermissions"
					}, "SiteMailboxCreationURL")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TransportConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Write-AdminAuditLog", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Comment,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-OwaMailboxPolicy", new RoleParameters[]
				{
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
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimpleDevicePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericDevicePasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePasswordEnabled,DevicePasswordExpiration,DevicePasswordHistory,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,IsDefaultPolicy,MaxAttachmentSize,MaxCalendarAgeFilter,MaxDevicePasswordFailedAttempts,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeDeviceLock,MinDevicePasswordComplexCharacters,MinDevicePasswordLength,Name,OutBuffer,OutVariable,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-MobileDeviceMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"ActiveSyncPermissions",
						"EOPPremiumRestrictions"
					}, "AllowBluetooth,AllowBrowser,AllowCamera,AllowConsumerEmail,AllowDesktopSync,AllowExternalDeviceManagement,AllowHTMLEmail,AllowInternetSharing,AllowIrDA,AllowMobileOTAUpdate,AllowNonProvisionableDevices,AllowPOPIMAPEmail,AllowRemoteDesktop,AllowSMIMEEncryptionAlgorithmNegotiation,AllowSMIMESoftCerts,AllowSimplePassword,AllowStorageCard,AllowTextMessaging,AllowUnsignedApplications,AllowUnsignedInstallationPackages,AllowWiFi,AlphanumericPasswordRequired,ApprovedApplicationList,AttachmentsEnabled,Confirm,DeviceEncryptionEnabled,DevicePolicyRefreshInterval,ErrorAction,ErrorVariable,Identity,IrmEnabled,IsDefault,MaxAttachmentSize,MaxCalendarAgeFilter,MaxEmailAgeFilter,MaxEmailBodyTruncationSize,MaxEmailHTMLBodyTruncationSize,MaxInactivityTimeLock,MaxPasswordFailedAttempts,MinPasswordComplexCharacters,MinPasswordLength,Name,OutBuffer,OutVariable,PasswordEnabled,PasswordExpiration,PasswordHistory,PasswordRecoveryEnabled,RequireDeviceEncryption,RequireEncryptedSMIMEMessages,RequireEncryptionSMIMEAlgorithm,RequireManualSyncWhenRoaming,RequireSignedSMIMEAlgorithm,RequireSignedSMIMEMessages,RequireStorageCardEncryption,UNCAccessEnabled,UnapprovedInROMApplicationList,WSSAccessEnabled,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-OwaMailboxPolicy", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "InstantMessagingType"),
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"OWAPermissions"
					}, "ActionForUnknownFileAndMIMETypes,ActiveSyncIntegrationEnabled,AllAddressListsEnabled,AllowCopyContactsToDeviceAddressBook,AllowOfflineOn,AllowedFileTypes,AllowedMimeTypes,BlockedFileTypes,BlockedMimeTypes,CalendarEnabled,ChangePasswordEnabled,Confirm,ContactsEnabled,DefaultClientLanguage,DefaultTheme,DelegateAccessEnabled,DirectFileAccessOnPrivateComputersEnabled,DirectFileAccessOnPublicComputersEnabled,DisplayPhotosEnabled,ErrorAction,ErrorVariable,ExplicitLogonEnabled,ForceSaveAttachmentFilteringEnabled,ForceSaveFileTypes,ForceSaveMimeTypes,ForceWacViewingFirstOnPrivateComputers,ForceWacViewingFirstOnPublicComputers,ForceWebReadyDocumentViewingFirstOnPrivateComputers,ForceWebReadyDocumentViewingFirstOnPublicComputers,GlobalAddressListEnabled,GroupCreationEnabled,IRMEnabled,Identity,InstantMessagingEnabled,JournalEnabled,LogonAndErrorLanguage,Name,NotesEnabled,OWALightEnabled,OrganizationEnabled,OutBuffer,OutVariable,OutboundCharset,PhoneticSupportEnabled,PremiumClientEnabled,RecoverDeletedItemsEnabled,RemindersAndNotificationsEnabled,ReportJunkEmailEnabled,RulesEnabled,SearchFoldersEnabled,SetPhotoEnabled,SetPhotoURL,SignaturesEnabled,SilverlightEnabled,SkipCreateUnifiedGroupCustomSharepointClassification,SpellCheckerEnabled,TasksEnabled,TextMessagingEnabled,ThemeSelectionEnabled,UMIntegrationEnabled,UseGB18030,UseISO885915,WSSAccessOnPrivateComputersEnabled,WSSAccessOnPublicComputersEnabled,WacExternalServicesEnabled,WacOMEXEnabled,WacViewingOnPrivateComputersEnabled,WacViewingOnPublicComputersEnabled,WarningAction,WarningVariable,WebPartsFrameOptionsType,WebReadyDocumentViewingForAllSupportedTypes,WebReadyDocumentViewingOnPrivateComputersEnabled,WebReadyDocumentViewingOnPublicComputersEnabled,WebReadyDocumentViewingSupportedFileTypes,WebReadyDocumentViewingSupportedMimeTypes,WebReadyFileTypes,WebReadyMimeTypes,WhatIf")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RemoteDomain", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Confirm,DomainName,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,DomainType,WhatIf")
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
					}, "Password")
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
					}, "Password")
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
					}, "PublicFolder")
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
					}, "Confirm,ErrorAction,ErrorVariable,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RetentionPolicyTag", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "AgeLimitForRetention,Comment,Confirm,ErrorAction,ErrorVariable,LocalizedComment,LocalizedRetentionPolicyTagName,MessageClass,MustDisplayCommentEnabled,Name,OutBuffer,OutVariable,RetentionAction,RetentionEnabled,RetentionId,SystemTag,Type,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,Force,Name,OutBuffer,OutVariable,RetentionId,RetentionPolicyTagLinks,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,ErrorAction,ErrorVariable,HoldCleanup,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
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
					}, "Confirm,CustomRecipientWriteScope,Description,DisplayName,ErrorAction,ErrorVariable,ManagedBy,Name,OutBuffer,OutVariable,Roles,WarningAction,WarningVariable,WhatIf"),
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
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
						"PilotingOrganization_Restrictions",
						"RuleMigration_Restrictions",
						"TransportRulesPermissions"
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,Enabled,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,UseLegacyRegex,WarningAction,WarningVariable,WhatIf,WithImportance")
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
					}, "ActivationDate,AdComparisonAttribute,AdComparisonOperator,AddManagerAsRecipientType,AddToRecipients,AnyOfCcHeader,AnyOfCcHeaderMemberOf,AnyOfRecipientAddressContainsWords,AnyOfRecipientAddressMatchesPatterns,AnyOfToCcHeader,AnyOfToCcHeaderMemberOf,AnyOfToHeader,AnyOfToHeaderMemberOf,ApplyClassification,ApplyHtmlDisclaimerFallbackAction,ApplyHtmlDisclaimerLocation,ApplyHtmlDisclaimerText,AttachmentContainsWords,AttachmentExtensionMatchesWords,AttachmentHasExecutableContent,AttachmentIsPasswordProtected,AttachmentIsUnsupported,AttachmentMatchesPatterns,AttachmentNameMatchesPatterns,AttachmentProcessingLimitExceeded,AttachmentPropertyContainsWords,AttachmentSizeOver,BetweenMemberOf1,BetweenMemberOf2,BlindCopyTo,Comments,Confirm,ContentCharacterSetContainsWords,CopyTo,DeleteMessage,Disconnect,ErrorAction,ErrorVariable,ExceptIfAdComparisonAttribute,ExceptIfAdComparisonOperator,ExceptIfAnyOfCcHeader,ExceptIfAnyOfCcHeaderMemberOf,ExceptIfAnyOfRecipientAddressContainsWords,ExceptIfAnyOfRecipientAddressMatchesPatterns,ExceptIfAnyOfToCcHeader,ExceptIfAnyOfToCcHeaderMemberOf,ExceptIfAnyOfToHeader,ExceptIfAnyOfToHeaderMemberOf,ExceptIfAttachmentContainsWords,ExceptIfAttachmentExtensionMatchesWords,ExceptIfAttachmentHasExecutableContent,ExceptIfAttachmentIsPasswordProtected,ExceptIfAttachmentIsUnsupported,ExceptIfAttachmentMatchesPatterns,ExceptIfAttachmentNameMatchesPatterns,ExceptIfAttachmentProcessingLimitExceeded,ExceptIfAttachmentPropertyContainsWords,ExceptIfAttachmentSizeOver,ExceptIfBetweenMemberOf1,ExceptIfBetweenMemberOf2,ExceptIfContentCharacterSetContainsWords,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfFromAddressMatchesPatterns,ExceptIfFromMemberOf,ExceptIfFromScope,ExceptIfHasClassification,ExceptIfHasNoClassification,ExceptIfHasSenderOverride,ExceptIfHeaderContainsMessageHeader,ExceptIfHeaderContainsWords,ExceptIfHeaderMatchesMessageHeader,ExceptIfHeaderMatchesPatterns,ExceptIfManagerAddresses,ExceptIfManagerForEvaluatedUser,ExceptIfMessageSizeOver,ExceptIfMessageTypeMatches,ExceptIfRecipientADAttributeContainsWords,ExceptIfRecipientADAttributeMatchesPatterns,ExceptIfRecipientAddressContainsWords,ExceptIfRecipientAddressMatchesPatterns,ExceptIfRecipientDomainIs,ExceptIfSCLOver,ExceptIfSenderADAttributeContainsWords,ExceptIfSenderADAttributeMatchesPatterns,ExceptIfSenderDomainIs,ExceptIfSenderIpRanges,ExceptIfSenderManagementRelationship,ExceptIfSentTo,ExceptIfSentToMemberOf,ExceptIfSentToScope,ExceptIfSubjectContainsWords,ExceptIfSubjectMatchesPatterns,ExceptIfSubjectOrBodyContainsWords,ExceptIfSubjectOrBodyMatchesPatterns,ExceptIfWithImportance,ExpiryDate,From,FromAddressContainsWords,FromAddressMatchesPatterns,FromMemberOf,FromScope,GenerateIncidentReport,GenerateNotification,HasClassification,HasNoClassification,HasSenderOverride,HeaderContainsMessageHeader,HeaderContainsWords,HeaderMatchesMessageHeader,HeaderMatchesPatterns,IncidentReportContent,IncidentReportOriginalMail,LogEventText,ManagerAddresses,ManagerForEvaluatedUser,MessageSizeOver,MessageTypeMatches,Mode,ModerateMessageByManager,ModerateMessageByUser,Name,OutBuffer,OutVariable,PrependSubject,Priority,Quarantine,RecipientADAttributeContainsWords,RecipientADAttributeMatchesPatterns,RecipientAddressContainsWords,RecipientAddressMatchesPatterns,RecipientDomainIs,RedirectMessageTo,RejectMessageEnhancedStatusCode,RejectMessageReasonText,RemoveHeader,RouteMessageOutboundRequireTls,RuleErrorAction,RuleSubType,SCLOver,SenderADAttributeContainsWords,SenderADAttributeMatchesPatterns,SenderAddressLocation,SenderDomainIs,SenderIpRanges,SenderManagementRelationship,SentTo,SentToMemberOf,SentToScope,SetAuditSeverity,SetHeaderName,SetHeaderValue,SetSCL,SmtpRejectMessageRejectStatusCode,SmtpRejectMessageRejectText,StopRuleProcessing,SubjectContainsWords,SubjectMatchesPatterns,SubjectOrBodyContainsWords,SubjectOrBodyMatchesPatterns,WarningAction,WarningVariable,WhatIf,WithImportance")
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
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-TextMessagingAccount", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"SMSPermissions"
					}, "Credential,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ReadFromDomainController,ResultSize,WarningAction,WarningVariable")
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
					}, "AlwaysDeleteOutlookRulesBlob,ApplyCategory,BodyContainsWords,Confirm,DeleteMessage,ErrorAction,ErrorVariable,ExceptIfBodyContainsWords,ExceptIfFlaggedForAction,ExceptIfFrom,ExceptIfFromAddressContainsWords,ExceptIfHasAttachment,ExceptIfHasClassification,ExceptIfHeaderContainsWords,ExceptIfMessageTypeMatches,ExceptIfMyNameInCcBox,ExceptIfMyNameInToBox,ExceptIfMyNameInToOrCcBox,ExceptIfMyNameNotInToBox,ExceptIfReceivedAfterDate,ExceptIfReceivedBeforeDate,ExceptIfRecipientAddressContainsWords,ExceptIfSentOnlyToMe,ExceptIfSentTo,ExceptIfSubjectContainsWords,ExceptIfSubjectOrBodyContainsWords,ExceptIfWithImportance,ExceptIfWithSensitivity,ExceptIfWithinSizeRangeMaximum,ExceptIfWithinSizeRangeMinimum,FlaggedForAction,Force,ForwardAsAttachmentTo,ForwardTo,From,FromAddressContainsWords,FromMessageId,HasAttachment,HasClassification,HeaderContainsWords,Mailbox,MarkAsRead,MarkImportance,MessageTypeMatches,MyNameInCcBox,MyNameInToBox,MyNameInToOrCcBox,MyNameNotInToBox,Name,OutBuffer,OutVariable,Priority,ReceivedAfterDate,ReceivedBeforeDate,RecipientAddressContainsWords,RedirectTo,SentOnlyToMe,SentTo,StopProcessingRules,SubjectContainsWords,SubjectOrBodyContainsWords,ValidateOnly,WarningAction,WarningVariable,WhatIf,WithImportance,WithSensitivity,WithinSizeRangeMaximum,WithinSizeRangeMinimum")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-MailMessage", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "Body,BodyFormat,Confirm,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Subject,WarningAction,WarningVariable,WhatIf")
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
					}, "MailTip,MailTipTranslations"),
					new RoleParameters(new string[]
					{
						"ResetUserPasswordManagementPermissions"
					}, "Password")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationConfig", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,OutBuffer,OutVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PartnerApplication", new RoleParameters[]
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
				}, "c")
			};
		}

		private class View_Only_Recipients
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-OrganizationalUnit", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,IncludeContainers,OutBuffer,OutVariable,ResultSize,SearchText,SingleNodeOnly,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-PublicFolderMailboxDiagnostics", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"PublicFoldersEnabled"
					}, "Confirm,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EXOCoreFeatures"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OrganizationalUnit,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RemovedMailbox", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"EXOCoreFeatures"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
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
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-MessageTrackingReport", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"EOPPremiumRestrictions",
						"MessageTrackingPermissions"
					}, "BypassDelegateChecking,Confirm,DoNotResolve,ErrorAction,ErrorVariable,Identity,MessageEntryId,MessageId,OutBuffer,OutVariable,Recipients,ResultSize,Sender,Subject,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}
	}
}
