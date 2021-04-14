using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class CannedUCCRoles_Datacenter
	{
		internal static RoleDefinition[] Definition = new RoleDefinition[]
		{
			new RoleDefinition("Audit Logs", RoleType.AuditLogs, CannedUCCRoles_Datacenter.Audit_Logs.Cmdlets),
			new RoleDefinition("Mailbox Search", RoleType.MailboxSearch, CannedUCCRoles_Datacenter.Mailbox_Search.Cmdlets),
			new RoleDefinition("Organization Configuration", RoleType.OrganizationConfiguration, CannedUCCRoles_Datacenter.Organization_Configuration.Cmdlets),
			new RoleDefinition("Remote and Accepted Domains", RoleType.RemoteAndAcceptedDomains, CannedUCCRoles_Datacenter.Remote_and_Accepted_Domains.Cmdlets),
			new RoleDefinition("Reset Password", RoleType.ResetPassword, CannedUCCRoles_Datacenter.Reset_Password.Cmdlets),
			new RoleDefinition("Role Management", RoleType.RoleManagement, CannedUCCRoles_Datacenter.Role_Management.Cmdlets),
			new RoleDefinition("Transport Rules", RoleType.TransportRules, CannedUCCRoles_Datacenter.Transport_Rules.Cmdlets),
			new RoleDefinition("User Options", RoleType.UserOptions, CannedUCCRoles_Datacenter.User_Options.Cmdlets),
			new RoleDefinition("View-Only Audit Logs", RoleType.ViewOnlyAuditLogs, CannedUCCRoles_Datacenter.View_Only_Audit_Logs.Cmdlets),
			new RoleDefinition("View-Only Configuration", RoleType.ViewOnlyConfiguration, CannedUCCRoles_Datacenter.View_Only_Configuration.Cmdlets),
			new RoleDefinition("View-Only Recipients", RoleType.ViewOnlyRecipients, CannedUCCRoles_Datacenter.View_Only_Recipients.Cmdlets)
		};

		private class Audit_Logs
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDehydratedFlag,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,Organization,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlets,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,Organization,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlets,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AdminAuditLogConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AdminAuditLogAgeLimit,AdminAuditLogCmdlets,AdminAuditLogEnabled,AdminAuditLogExcludedCmdlets,AdminAuditLogParameters,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,IgnoreDehydratedFlag,LogLevel,Name,OutBuffer,OutVariable,TestCmdletLoggingEnabled,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Mailbox_Search
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,OutBuffer,OutVariable,ResultSize,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AllExchangeBindings,AllOneDriveBindings,AllPublicFolderBindings,AllSharePointBindings,Confirm,Debug,Description,DomainController,EndDate,ErrorAction,ErrorVariable,ExchangeBinding,ExchangeBindingExclusion,Force,IncludeUnindexedItems,KeywordQuery,Language,LogLevel,Name,OneDriveBinding,OneDriveBindingExclusion,OutBuffer,OutVariable,PublicFolderBinding,PublicFolderBindingExclusion,SharePointBinding,SharePointBindingExclusion,StartDate,StatusMailRecipients,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AddExchangeBinding,AddExchangeBindingExclusion,AddOneDriveBinding,AddOneDriveBindingExclusion,AddPublicFolderBinding,AddPublicFolderBindingExclusion,AddSharePointBinding,AddSharePointBindingExclusion,AllExchangeBindings,AllOneDriveBindings,AllPublicFolderBindings,AllSharePointBindings,Confirm,Debug,Description,DomainController,EndDate,ErrorAction,ErrorVariable,ExchangeBinding,ExchangeBindingExclusion,Force,Identity,IncludeUnindexedItems,KeywordQuery,Language,LogLevel,Name,OneDriveBinding,OneDriveBindingExclusion,OutBuffer,OutVariable,PublicFolderBinding,PublicFolderBindingExclusion,RemoveExchangeBinding,RemoveExchangeBindingExclusion,RemoveOneDriveBinding,RemoveOneDriveBindingExclusion,RemovePublicFolderBinding,RemovePublicFolderBindingExclusion,RemoveSharePointBinding,RemoveSharePointBindingExclusion,SharePointBinding,SharePointBindingExclusion,StartDate,StatusMailRecipients,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Start-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Action,Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,Resume,StatisticsStartIndex,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Stop-ComplianceSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Debug,DomainController,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Organization_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Organization,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DevicePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Policy,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpSensitiveInformationType_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Install-UnifiedCompliancePrerequisite_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ForceInitialize,LoadOnly,PolicyCenterSiteOwner,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf,Workload")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuditOperation,Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf,Workload")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceConditionalAccessPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Name,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceConditionalAccessRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccountName,AccountUserName,AllowAppStore,AllowAssistantWhileLocked,AllowConvenienceLogon,AllowDiagnosticSubmission,AllowJailbroken,AllowMove,AllowPassbookWhileLocked,AllowRecentAddressSyncing,AllowScreenshot,AllowSimplePassword,AllowVideoConferencing,AllowVoiceAssistant,AllowVoiceDialing,AllowiCloudBackup,AllowiCloudDocSync,AllowiCloudPhotoSync,AntiVirusSignatureStatus,AntiVirusStatus,AppsRating,AutoUpdateStatus,BluetoothEnabled,CameraEnabled,ContentType,DaysToSync,Debug,DomainController,EmailAddress,EnableRemovableStorage,ErrorAction,ExchangeActiveSyncHost,FirewallStatus,ForceAppStorePassword,ForceEncryptedBackup,MaxPasswordAttemptsBeforeWipe,MaxPasswordGracePeriod,MoviesRating,Organization,PasswordComplexity,PasswordExpirationDays,PasswordHistoryCount,PasswordMinComplexChars,PasswordMinimumLength,PasswordQuality,PasswordRequired,PasswordTimeout,PhoneMemoryEncrypted,Policy,RegionRatings,SmartScreenEnabled,SyncSchedule,SystemSecurityTLS,TVShowsRating,TargetGroups,UseOnlyInEmail,UseSMIME,UseSSL,UserAccountControlStatus,Verbose,WLANEnabled,WhatIf,WorkFoldersSyncUrl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Name,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccountName,AccountUserName,AllowAppStore,AllowAssistantWhileLocked,AllowConvenienceLogon,AllowDiagnosticSubmission,AllowMove,AllowPassbookWhileLocked,AllowRecentAddressSyncing,AllowScreenshot,AllowSimplePassword,AllowVideoConferencing,AllowVoiceAssistant,AllowVoiceDialing,AllowiCloudBackup,AllowiCloudDocSync,AllowiCloudPhotoSync,AntiVirusSignatureStatus,AntiVirusStatus,AppsRating,AutoUpdateStatus,BluetoothEnabled,CameraEnabled,ContentType,DaysToSync,Debug,DomainController,EmailAddress,EnableRemovableStorage,ErrorAction,ExchangeActiveSyncHost,FirewallStatus,ForceAppStorePassword,ForceEncryptedBackup,MaxPasswordAttemptsBeforeWipe,MaxPasswordGracePeriod,MoviesRating,Organization,PasswordComplexity,PasswordExpirationDays,PasswordHistoryCount,PasswordMinComplexChars,PasswordMinimumLength,PasswordQuality,PasswordRequired,PasswordTimeout,PhoneMemoryEncrypted,Policy,RegionRatings,SmartScreenEnabled,SyncSchedule,SystemSecurityTLS,TVShowsRating,TargetGroups,UseOnlyInEmail,UseSMIME,UseSSL,UserAccountControlStatus,Verbose,WLANEnabled,WhatIf,WorkFoldersSyncUrl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceTenantPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DeviceTenantRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ApplyPolicyTo,BlockUnsupportedDevices,Debug,DomainController,ErrorAction,ExclusionList,Organization,Policy,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DlpCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Name,OneDriveBinding,SharePointBinding,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-DlpComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccessScopeIs,BlockAccess,Comment,ContentContainsSensitiveInformation,ContentPropertyContainsWords,Disabled,ErrorAction,Name,Policy,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HoldCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,ExchangeBinding,Force,Name,SharePointBinding")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-HoldComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,ContentDateFrom,ContentDateTo,ContentMatchQuery,Disabled,ErrorAction,HoldContent,HoldDurationDisplayHint,Name,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceConditionalAccessPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceConditionalAccessRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceTenantPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DeviceTenantRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DlpCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ForceDeletion,Identity,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-DlpComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ForceDeletion,Identity,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HoldCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-HoldComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuditConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Organization,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf,Workload")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuditOperation,Debug,DomainController,Enabled,ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceConditionalAccessPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Identity,Name,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceConditionalAccessRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccountName,AccountUserName,AllowAppStore,AllowAssistantWhileLocked,AllowConvenienceLogon,AllowDiagnosticSubmission,AllowJailbroken,AllowMove,AllowPassbookWhileLocked,AllowRecentAddressSyncing,AllowScreenshot,AllowSimplePassword,AllowVideoConferencing,AllowVoiceAssistant,AllowVoiceDialing,AllowiCloudBackup,AllowiCloudDocSync,AllowiCloudPhotoSync,AntiVirusSignatureStatus,AntiVirusStatus,AppsRating,AutoUpdateStatus,BluetoothEnabled,CameraEnabled,ContentType,DaysToSync,Debug,DomainController,EmailAddress,EnableRemovableStorage,Enabled,ErrorAction,ExchangeActiveSyncHost,FirewallStatus,ForceAppStorePassword,ForceEncryptedBackup,Identity,MaxPasswordAttemptsBeforeWipe,MaxPasswordGracePeriod,MoviesRating,PasswordComplexity,PasswordExpirationDays,PasswordHistoryCount,PasswordMinComplexChars,PasswordMinimumLength,PasswordQuality,PasswordRequired,PasswordTimeout,PhoneMemoryEncrypted,RegionRatings,SmartScreenEnabled,SyncSchedule,SystemSecurityTLS,TVShowsRating,TargetGroups,UseOnlyInEmail,UseSMIME,UseSSL,UserAccountControlStatus,Verbose,WLANEnabled,WorkFoldersSyncUrl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Identity,Name,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AllowAppStore,AllowAssistantWhileLocked,AllowConvenienceLogon,AllowDiagnosticSubmission,AllowMove,AllowPassbookWhileLocked,AllowRecentAddressSyncing,AllowScreenshot,AllowSimplePassword,AllowVideoConferencing,AllowVoiceAssistant,AllowVoiceDialing,AllowiCloudBackup,AllowiCloudDocSync,AllowiCloudPhotoSync,AntiVirusSignatureStatus,AntiVirusStatus,AppsRating,AutoUpdateStatus,BluetoothEnabled,CameraEnabled,ContentType,DaysToSync,Debug,DomainController,EmailAddress,EnableRemovableStorage,Enabled,ErrorAction,ExchangeActiveSyncHost,FirewallStatus,ForceAppStorePassword,ForceEncryptedBackup,Identity,MaxPasswordAttemptsBeforeWipe,MaxPasswordGracePeriod,MoviesRating,PasswordComplexity,PasswordExpirationDays,PasswordHistoryCount,PasswordMinComplexChars,PasswordMinimumLength,PasswordQuality,PasswordRequired,PasswordTimeout,PhoneMemoryEncrypted,RegionRatings,SmartScreenEnabled,SyncSchedule,SystemSecurityTLS,TVShowsRating,TargetGroups,UseOnlyInEmail,UseSMIME,UseSSL,UserAccountControlStatus,Verbose,WLANEnabled,WorkFoldersSyncUrl")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceTenantPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,Enabled,ErrorAction,Force,Identity,Name,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DeviceTenantRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ApplyPolicyTo,BlockUnsupportedDevices,Debug,DomainController,Enabled,ExclusionList,Identity,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DlpCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AddOneDriveBinding,AddSharePointBinding,Comment,Enabled,ErrorAction,Force,Identity,Name,RemoveOneDriveBinding,RemoveSharePointBinding,RetryDistribution,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-DlpComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AccessScopeIs,BlockAccess,Comment,ContentContainsSensitiveInformation,ContentPropertyContainsWords,Disabled,ErrorAction,Identity,Name,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HoldCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AddSharePointBinding,Comment,Enabled,ErrorAction,Force,Identity,RemoveExchangeBinding,RemoveSharePointBinding,RetryDistribution")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-HoldComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Comment,ContentDateFrom,ContentDateTo,ContentMatchQuery,Disabled,ErrorAction,HoldContent,HoldDurationDisplayHint,Identity")
				}, "c")
			};
		}

		private class Remote_and_Accepted_Domains
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "AuthenticationType")
				}, "c")
			};
		}

		private class Reset_Password
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,Organization,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,PublicFolder,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class Role_Management
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Add-RoleGroupMember_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlet,CmdletParameters,Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,Organization,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,ShowPartnerLinked,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,Organization,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,PublicFolder,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-RoleGroup_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Description,DisplayName,ErrorAction,ErrorVariable,Force,Members,Name,OutBuffer,OutVariable,Roles,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "OutBuffer,OutVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroupMember_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Member,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-RoleGroup_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Force,Identity,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-RoleGroup_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,Description,DisplayName,ErrorAction,ErrorVariable,Force,Identity,Name,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Update-RoleGroupMember_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Confirm,ErrorAction,ErrorVariable,Identity,Members,OutBuffer,OutVariable,WarningAction,WarningVariable,WhatIf")
				}, "c")
			};
		}

		private class Transport_Rules
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class User_Options
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,AuthenticationType,BookmarkDisplayName,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,PublicFolder,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class View_Only_Audit_Logs
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDehydratedFlag,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,Organization,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "New-AdminAuditLogSearch_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlets,Confirm,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Name,ObjectIds,Organization,OutBuffer,OutVariable,Parameters,StartDate,StatusMailRecipients,UserIds,Verbose,WarningAction,WarningVariable,WhatIf")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Search-AdminAuditLog_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlets,Debug,DomainController,EndDate,ErrorAction,ErrorVariable,ExternalAccess,Identity,IsSuccess,ObjectIds,OutBuffer,OutVariable,Parameters,ResultSize,StartDate,StartIndex,UserIds,Verbose,WarningAction,WarningVariable")
				}, "c")
			};
		}

		private class View_Only_Configuration
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AdminAuditLogConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,IgnoreDehydratedFlag,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Organization,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Identity,Organization,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConditionalAccessRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DevicePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Organization")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DeviceTenantRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,Identity,Organization,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Policy,Verbose")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-DlpSensitiveInformationType_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldCompliancePolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-HoldComplianceRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,Identity,Policy")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-ManagementRole_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Cmdlet,CmdletParameters,Debug,DomainController,ErrorAction,ErrorVariable,GetChildren,Identity,Organization,OutBuffer,OutVariable,Recurse,RoleType,Script,ScriptParameters,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroupMember_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,ResultSize,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-RoleGroup_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,ResultSize,ShowPartnerLinked,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationPolicy_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Remove-AuditConfigurationRule_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,OutBuffer,OutVariable,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Set-AuditConfig_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Organization")
				}, "c")
			};
		}

		private class View_Only_Recipients
		{
			internal static RoleCmdlet[] Cmdlets = new RoleCmdlet[]
			{
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Group_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Identity,OutBuffer,OutVariable,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-Recipient_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Anr,AuthenticationType,BookmarkDisplayName,Capabilities,ErrorAction,ErrorVariable,Filter,Identity,IncludeBookmarkObject,OutBuffer,OutVariable,Properties,PropertySet,RecipientPreviewFilter,RecipientType,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-SecurityPrincipal_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "Debug,DomainController,ErrorAction,ErrorVariable,Filter,Identity,IncludeDomainLocalFrom,Organization,OrganizationalUnit,OutBuffer,OutVariable,ResultSize,RoleGroupAssignable,Types,Verbose,WarningAction,WarningVariable")
				}, "c"),
				new RoleCmdlet("Microsoft.Exchange.Management.PowerShell.E2010", "Get-User_UCC", new RoleParameters[]
				{
					new RoleParameters(new string[]
					{
						"*"
					}, "ErrorAction,ErrorVariable,Filter,Identity,OutBuffer,OutVariable,PublicFolder,RecipientTypeDetails,ResultSize,SortBy,WarningAction,WarningVariable")
				}, "c")
			};
		}
	}
}
