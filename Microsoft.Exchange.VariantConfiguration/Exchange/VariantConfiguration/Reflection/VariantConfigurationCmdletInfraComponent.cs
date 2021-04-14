using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationCmdletInfraComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationCmdletInfraComponent() : base("CmdletInfra")
		{
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-TransportRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ReportingWebService", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "PrePopulateCacheForMailboxBasedOnDatabase", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-MailboxImportRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-HoldComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-HistoricalSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOOneDriveForBusinessFileActivityReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-DataClassification", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SetPasswordWithoutOldPassword", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-AuditConfigurationPolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MailboxSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SiteMailboxCheckSharePointUrlAgainstTrustedHosts", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-DataClassification", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-AuditConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "LimitNameMaxlength", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "CmdletMonitoring", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-ReportSchedule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-HoldComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "GlobalAddressListAttrbutes", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Add-Mailbox", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Install-UnifiedCompliancePrerequisite", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ServiceAccountForest", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOSkyDriveProStorageReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "InactiveMailbox", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-DlpComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-HoldComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Psws", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-ReportSchedule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ClientAccessRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SetDefaultProhibitSendReceiveQuota", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-Mailbox", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ExternalActivityReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-DlpCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ReportToOriginator", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-MailUser", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MailboxExportRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Test-ClientAccessRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ExternalActivitySummaryReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-ClientAccessRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ExternalActivityByUserReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Stop-HistoricalSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-DlpCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-DeviceConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "WinRMExchangeDataUseTypeNamedPipe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ReportScheduleHistory", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "RecoverMailBox", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SiteMailboxProvisioningInExecutingUserOUEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ListedIPWrapper", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-ClientAccessRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ExternalActivityByDomainReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-DeviceConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-CsClientDeviceReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-DlpComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-DeviceConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-HoldCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ShowFismaBanner", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "UseDatabaseQuotaDefaults", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-AuditConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "WriteEventLogInEnglish", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-DlpCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SupportOptimizedFilterOnlyInDDG", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "DepthTwoTypeEntry", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-AuditConfig", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-DataClassification", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MigrationEndpoint", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-MailboxExportRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ValidateExternalEmailAddressInAcceptedDomain", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Enable-EOPMailUser", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-OMEConfiguration", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "EmailAddressPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SkipPiiRedactionForForestWideObject", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-PartnerClientExpiringSubscriptionReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "PiiRedaction", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ValidateFilteringOnlyUser", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SoftDeleteObject", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-MailboxSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOOneDriveForBusinessUserStatisticsReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Add-DelistIP", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "GenerateNewExternalDirectoryObjectId", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "IncludeFBOnlyForCalendarContributor", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ValidateEnableRoomMailboxAccount", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-DlpComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-DlpComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "PswsCmdletProxy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-HoldCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "LegacyRegCodeSupport", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-OMEConfiguration", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOActiveUserReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-AuditConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOSkyDriveProDeployedReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-TransportRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-Fingerprint", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ReputationOverride", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-ReportSchedule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-Mailbox", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "InstallModernGroupsAddressList", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "GenericExchangeSnapin", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-MigrationBatch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-AuditConfigurationPolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-AuditConfigurationRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-ClientAccessRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "OverWriteElcMailboxFlags", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "MaxAddressBookPolicies", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Start-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Test-MigrationServerAvailability", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "WinRMExchangeDataUseAuthenticationType", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "RpsClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Stop-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Resume-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-DlpCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-Mailbox", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOTeamSiteDeployedReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-HoldComplianceRule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "PswsClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Remove-ReputationOverride", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-AuditConfigurationPolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-DnsBlocklistInfo", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-FolderMoveRequestStatistics", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Start-HistoricalSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "CheckForDedicatedTenantAdminRoleNamePrefix", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Suspend-FolderMoveRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MailboxImportRequest", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MigrationBatch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Set-ComplianceSearch", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOTeamSiteStorageReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-HoldCompliancePolicy", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-DlpSensitiveInformationType", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ReportScheduleList", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-Mailbox", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-SPOTenantStorageMetricReport", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-MailUser", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-ReportSchedule", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "SetActiveArchiveStatus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "Get-AuditConfig", typeof(ICmdletSettings), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "WsSecuritySymmetricAndX509Cert", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "ProxyDllUpdate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CmdletInfra.settings.ini", "New-HoldCompliancePolicy", typeof(ICmdletSettings), false));
		}

		public VariantConfigurationSection NewTransportRule
		{
			get
			{
				return base["NewTransportRule"];
			}
		}

		public VariantConfigurationSection ReportingWebService
		{
			get
			{
				return base["ReportingWebService"];
			}
		}

		public VariantConfigurationSection PrePopulateCacheForMailboxBasedOnDatabase
		{
			get
			{
				return base["PrePopulateCacheForMailboxBasedOnDatabase"];
			}
		}

		public VariantConfigurationSection SetMailboxImportRequest
		{
			get
			{
				return base["SetMailboxImportRequest"];
			}
		}

		public VariantConfigurationSection SetHoldComplianceRule
		{
			get
			{
				return base["SetHoldComplianceRule"];
			}
		}

		public VariantConfigurationSection GetHistoricalSearch
		{
			get
			{
				return base["GetHistoricalSearch"];
			}
		}

		public VariantConfigurationSection GetSPOOneDriveForBusinessFileActivityReport
		{
			get
			{
				return base["GetSPOOneDriveForBusinessFileActivityReport"];
			}
		}

		public VariantConfigurationSection RemoveDataClassification
		{
			get
			{
				return base["RemoveDataClassification"];
			}
		}

		public VariantConfigurationSection SetPasswordWithoutOldPassword
		{
			get
			{
				return base["SetPasswordWithoutOldPassword"];
			}
		}

		public VariantConfigurationSection NewAuditConfigurationPolicy
		{
			get
			{
				return base["NewAuditConfigurationPolicy"];
			}
		}

		public VariantConfigurationSection NewMailboxSearch
		{
			get
			{
				return base["NewMailboxSearch"];
			}
		}

		public VariantConfigurationSection SiteMailboxCheckSharePointUrlAgainstTrustedHosts
		{
			get
			{
				return base["SiteMailboxCheckSharePointUrlAgainstTrustedHosts"];
			}
		}

		public VariantConfigurationSection SetDataClassification
		{
			get
			{
				return base["SetDataClassification"];
			}
		}

		public VariantConfigurationSection NewAuditConfigurationRule
		{
			get
			{
				return base["NewAuditConfigurationRule"];
			}
		}

		public VariantConfigurationSection LimitNameMaxlength
		{
			get
			{
				return base["LimitNameMaxlength"];
			}
		}

		public VariantConfigurationSection CmdletMonitoring
		{
			get
			{
				return base["CmdletMonitoring"];
			}
		}

		public VariantConfigurationSection SetReportSchedule
		{
			get
			{
				return base["SetReportSchedule"];
			}
		}

		public VariantConfigurationSection GetHoldComplianceRule
		{
			get
			{
				return base["GetHoldComplianceRule"];
			}
		}

		public VariantConfigurationSection GlobalAddressListAttrbutes
		{
			get
			{
				return base["GlobalAddressListAttrbutes"];
			}
		}

		public VariantConfigurationSection GetComplianceSearch
		{
			get
			{
				return base["GetComplianceSearch"];
			}
		}

		public VariantConfigurationSection AddMailbox
		{
			get
			{
				return base["AddMailbox"];
			}
		}

		public VariantConfigurationSection InstallUnifiedCompliancePrerequisite
		{
			get
			{
				return base["InstallUnifiedCompliancePrerequisite"];
			}
		}

		public VariantConfigurationSection ServiceAccountForest
		{
			get
			{
				return base["ServiceAccountForest"];
			}
		}

		public VariantConfigurationSection GetSPOSkyDriveProStorageReport
		{
			get
			{
				return base["GetSPOSkyDriveProStorageReport"];
			}
		}

		public VariantConfigurationSection InactiveMailbox
		{
			get
			{
				return base["InactiveMailbox"];
			}
		}

		public VariantConfigurationSection NewDlpComplianceRule
		{
			get
			{
				return base["NewDlpComplianceRule"];
			}
		}

		public VariantConfigurationSection RemoveHoldComplianceRule
		{
			get
			{
				return base["RemoveHoldComplianceRule"];
			}
		}

		public VariantConfigurationSection Psws
		{
			get
			{
				return base["Psws"];
			}
		}

		public VariantConfigurationSection RemoveReportSchedule
		{
			get
			{
				return base["RemoveReportSchedule"];
			}
		}

		public VariantConfigurationSection GetClientAccessRule
		{
			get
			{
				return base["GetClientAccessRule"];
			}
		}

		public VariantConfigurationSection SetDefaultProhibitSendReceiveQuota
		{
			get
			{
				return base["SetDefaultProhibitSendReceiveQuota"];
			}
		}

		public VariantConfigurationSection SetMailbox
		{
			get
			{
				return base["SetMailbox"];
			}
		}

		public VariantConfigurationSection GetExternalActivityReport
		{
			get
			{
				return base["GetExternalActivityReport"];
			}
		}

		public VariantConfigurationSection GetDlpCompliancePolicy
		{
			get
			{
				return base["GetDlpCompliancePolicy"];
			}
		}

		public VariantConfigurationSection ReportToOriginator
		{
			get
			{
				return base["ReportToOriginator"];
			}
		}

		public VariantConfigurationSection SetMailUser
		{
			get
			{
				return base["SetMailUser"];
			}
		}

		public VariantConfigurationSection NewMailboxExportRequest
		{
			get
			{
				return base["NewMailboxExportRequest"];
			}
		}

		public VariantConfigurationSection TestClientAccessRule
		{
			get
			{
				return base["TestClientAccessRule"];
			}
		}

		public VariantConfigurationSection GetExternalActivitySummaryReport
		{
			get
			{
				return base["GetExternalActivitySummaryReport"];
			}
		}

		public VariantConfigurationSection NewClientAccessRule
		{
			get
			{
				return base["NewClientAccessRule"];
			}
		}

		public VariantConfigurationSection GetExternalActivityByUserReport
		{
			get
			{
				return base["GetExternalActivityByUserReport"];
			}
		}

		public VariantConfigurationSection GetFolderMoveRequest
		{
			get
			{
				return base["GetFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection StopHistoricalSearch
		{
			get
			{
				return base["StopHistoricalSearch"];
			}
		}

		public VariantConfigurationSection NewDlpCompliancePolicy
		{
			get
			{
				return base["NewDlpCompliancePolicy"];
			}
		}

		public VariantConfigurationSection SetDeviceConfigurationRule
		{
			get
			{
				return base["SetDeviceConfigurationRule"];
			}
		}

		public VariantConfigurationSection WinRMExchangeDataUseTypeNamedPipe
		{
			get
			{
				return base["WinRMExchangeDataUseTypeNamedPipe"];
			}
		}

		public VariantConfigurationSection GetReportScheduleHistory
		{
			get
			{
				return base["GetReportScheduleHistory"];
			}
		}

		public VariantConfigurationSection RemoveFolderMoveRequest
		{
			get
			{
				return base["RemoveFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection RecoverMailBox
		{
			get
			{
				return base["RecoverMailBox"];
			}
		}

		public VariantConfigurationSection SiteMailboxProvisioningInExecutingUserOUEnabled
		{
			get
			{
				return base["SiteMailboxProvisioningInExecutingUserOUEnabled"];
			}
		}

		public VariantConfigurationSection GetListedIPWrapper
		{
			get
			{
				return base["GetListedIPWrapper"];
			}
		}

		public VariantConfigurationSection SetClientAccessRule
		{
			get
			{
				return base["SetClientAccessRule"];
			}
		}

		public VariantConfigurationSection GetExternalActivityByDomainReport
		{
			get
			{
				return base["GetExternalActivityByDomainReport"];
			}
		}

		public VariantConfigurationSection NewDeviceConfigurationRule
		{
			get
			{
				return base["NewDeviceConfigurationRule"];
			}
		}

		public VariantConfigurationSection GetCsClientDeviceReport
		{
			get
			{
				return base["GetCsClientDeviceReport"];
			}
		}

		public VariantConfigurationSection GetDlpComplianceRule
		{
			get
			{
				return base["GetDlpComplianceRule"];
			}
		}

		public VariantConfigurationSection GetDeviceConfigurationRule
		{
			get
			{
				return base["GetDeviceConfigurationRule"];
			}
		}

		public VariantConfigurationSection RemoveHoldCompliancePolicy
		{
			get
			{
				return base["RemoveHoldCompliancePolicy"];
			}
		}

		public VariantConfigurationSection ShowFismaBanner
		{
			get
			{
				return base["ShowFismaBanner"];
			}
		}

		public VariantConfigurationSection UseDatabaseQuotaDefaults
		{
			get
			{
				return base["UseDatabaseQuotaDefaults"];
			}
		}

		public VariantConfigurationSection GetAuditConfigurationRule
		{
			get
			{
				return base["GetAuditConfigurationRule"];
			}
		}

		public VariantConfigurationSection WriteEventLogInEnglish
		{
			get
			{
				return base["WriteEventLogInEnglish"];
			}
		}

		public VariantConfigurationSection SetDlpCompliancePolicy
		{
			get
			{
				return base["SetDlpCompliancePolicy"];
			}
		}

		public VariantConfigurationSection SupportOptimizedFilterOnlyInDDG
		{
			get
			{
				return base["SupportOptimizedFilterOnlyInDDG"];
			}
		}

		public VariantConfigurationSection RemoveComplianceSearch
		{
			get
			{
				return base["RemoveComplianceSearch"];
			}
		}

		public VariantConfigurationSection DepthTwoTypeEntry
		{
			get
			{
				return base["DepthTwoTypeEntry"];
			}
		}

		public VariantConfigurationSection SetAuditConfig
		{
			get
			{
				return base["SetAuditConfig"];
			}
		}

		public VariantConfigurationSection NewDataClassification
		{
			get
			{
				return base["NewDataClassification"];
			}
		}

		public VariantConfigurationSection NewMigrationEndpoint
		{
			get
			{
				return base["NewMigrationEndpoint"];
			}
		}

		public VariantConfigurationSection SetMailboxExportRequest
		{
			get
			{
				return base["SetMailboxExportRequest"];
			}
		}

		public VariantConfigurationSection ValidateExternalEmailAddressInAcceptedDomain
		{
			get
			{
				return base["ValidateExternalEmailAddressInAcceptedDomain"];
			}
		}

		public VariantConfigurationSection EnableEOPMailUser
		{
			get
			{
				return base["EnableEOPMailUser"];
			}
		}

		public VariantConfigurationSection GetOMEConfiguration
		{
			get
			{
				return base["GetOMEConfiguration"];
			}
		}

		public VariantConfigurationSection NewFolderMoveRequest
		{
			get
			{
				return base["NewFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection EmailAddressPolicy
		{
			get
			{
				return base["EmailAddressPolicy"];
			}
		}

		public VariantConfigurationSection SkipPiiRedactionForForestWideObject
		{
			get
			{
				return base["SkipPiiRedactionForForestWideObject"];
			}
		}

		public VariantConfigurationSection GetPartnerClientExpiringSubscriptionReport
		{
			get
			{
				return base["GetPartnerClientExpiringSubscriptionReport"];
			}
		}

		public VariantConfigurationSection PiiRedaction
		{
			get
			{
				return base["PiiRedaction"];
			}
		}

		public VariantConfigurationSection ValidateFilteringOnlyUser
		{
			get
			{
				return base["ValidateFilteringOnlyUser"];
			}
		}

		public VariantConfigurationSection SoftDeleteObject
		{
			get
			{
				return base["SoftDeleteObject"];
			}
		}

		public VariantConfigurationSection SetMailboxSearch
		{
			get
			{
				return base["SetMailboxSearch"];
			}
		}

		public VariantConfigurationSection GetSPOOneDriveForBusinessUserStatisticsReport
		{
			get
			{
				return base["GetSPOOneDriveForBusinessUserStatisticsReport"];
			}
		}

		public VariantConfigurationSection SetFolderMoveRequest
		{
			get
			{
				return base["SetFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection AddDelistIP
		{
			get
			{
				return base["AddDelistIP"];
			}
		}

		public VariantConfigurationSection GenerateNewExternalDirectoryObjectId
		{
			get
			{
				return base["GenerateNewExternalDirectoryObjectId"];
			}
		}

		public VariantConfigurationSection NewComplianceSearch
		{
			get
			{
				return base["NewComplianceSearch"];
			}
		}

		public VariantConfigurationSection IncludeFBOnlyForCalendarContributor
		{
			get
			{
				return base["IncludeFBOnlyForCalendarContributor"];
			}
		}

		public VariantConfigurationSection ValidateEnableRoomMailboxAccount
		{
			get
			{
				return base["ValidateEnableRoomMailboxAccount"];
			}
		}

		public VariantConfigurationSection SetDlpComplianceRule
		{
			get
			{
				return base["SetDlpComplianceRule"];
			}
		}

		public VariantConfigurationSection RemoveDlpComplianceRule
		{
			get
			{
				return base["RemoveDlpComplianceRule"];
			}
		}

		public VariantConfigurationSection PswsCmdletProxy
		{
			get
			{
				return base["PswsCmdletProxy"];
			}
		}

		public VariantConfigurationSection SetHoldCompliancePolicy
		{
			get
			{
				return base["SetHoldCompliancePolicy"];
			}
		}

		public VariantConfigurationSection LegacyRegCodeSupport
		{
			get
			{
				return base["LegacyRegCodeSupport"];
			}
		}

		public VariantConfigurationSection SetOMEConfiguration
		{
			get
			{
				return base["SetOMEConfiguration"];
			}
		}

		public VariantConfigurationSection GetSPOActiveUserReport
		{
			get
			{
				return base["GetSPOActiveUserReport"];
			}
		}

		public VariantConfigurationSection RemoveAuditConfigurationRule
		{
			get
			{
				return base["RemoveAuditConfigurationRule"];
			}
		}

		public VariantConfigurationSection GetSPOSkyDriveProDeployedReport
		{
			get
			{
				return base["GetSPOSkyDriveProDeployedReport"];
			}
		}

		public VariantConfigurationSection SetTransportRule
		{
			get
			{
				return base["SetTransportRule"];
			}
		}

		public VariantConfigurationSection NewFingerprint
		{
			get
			{
				return base["NewFingerprint"];
			}
		}

		public VariantConfigurationSection GetReputationOverride
		{
			get
			{
				return base["GetReputationOverride"];
			}
		}

		public VariantConfigurationSection NewReportSchedule
		{
			get
			{
				return base["NewReportSchedule"];
			}
		}

		public VariantConfigurationSection NewMailbox
		{
			get
			{
				return base["NewMailbox"];
			}
		}

		public VariantConfigurationSection InstallModernGroupsAddressList
		{
			get
			{
				return base["InstallModernGroupsAddressList"];
			}
		}

		public VariantConfigurationSection GenericExchangeSnapin
		{
			get
			{
				return base["GenericExchangeSnapin"];
			}
		}

		public VariantConfigurationSection SetMigrationBatch
		{
			get
			{
				return base["SetMigrationBatch"];
			}
		}

		public VariantConfigurationSection RemoveAuditConfigurationPolicy
		{
			get
			{
				return base["RemoveAuditConfigurationPolicy"];
			}
		}

		public VariantConfigurationSection SetAuditConfigurationRule
		{
			get
			{
				return base["SetAuditConfigurationRule"];
			}
		}

		public VariantConfigurationSection RemoveClientAccessRule
		{
			get
			{
				return base["RemoveClientAccessRule"];
			}
		}

		public VariantConfigurationSection OverWriteElcMailboxFlags
		{
			get
			{
				return base["OverWriteElcMailboxFlags"];
			}
		}

		public VariantConfigurationSection MaxAddressBookPolicies
		{
			get
			{
				return base["MaxAddressBookPolicies"];
			}
		}

		public VariantConfigurationSection StartComplianceSearch
		{
			get
			{
				return base["StartComplianceSearch"];
			}
		}

		public VariantConfigurationSection TestMigrationServerAvailability
		{
			get
			{
				return base["TestMigrationServerAvailability"];
			}
		}

		public VariantConfigurationSection WinRMExchangeDataUseAuthenticationType
		{
			get
			{
				return base["WinRMExchangeDataUseAuthenticationType"];
			}
		}

		public VariantConfigurationSection RpsClientAccessRulesEnabled
		{
			get
			{
				return base["RpsClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection StopComplianceSearch
		{
			get
			{
				return base["StopComplianceSearch"];
			}
		}

		public VariantConfigurationSection ResumeFolderMoveRequest
		{
			get
			{
				return base["ResumeFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection RemoveDlpCompliancePolicy
		{
			get
			{
				return base["RemoveDlpCompliancePolicy"];
			}
		}

		public VariantConfigurationSection RemoveMailbox
		{
			get
			{
				return base["RemoveMailbox"];
			}
		}

		public VariantConfigurationSection GetSPOTeamSiteDeployedReport
		{
			get
			{
				return base["GetSPOTeamSiteDeployedReport"];
			}
		}

		public VariantConfigurationSection NewHoldComplianceRule
		{
			get
			{
				return base["NewHoldComplianceRule"];
			}
		}

		public VariantConfigurationSection PswsClientAccessRulesEnabled
		{
			get
			{
				return base["PswsClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection RemoveReputationOverride
		{
			get
			{
				return base["RemoveReputationOverride"];
			}
		}

		public VariantConfigurationSection GetAuditConfigurationPolicy
		{
			get
			{
				return base["GetAuditConfigurationPolicy"];
			}
		}

		public VariantConfigurationSection GetDnsBlocklistInfo
		{
			get
			{
				return base["GetDnsBlocklistInfo"];
			}
		}

		public VariantConfigurationSection GetFolderMoveRequestStatistics
		{
			get
			{
				return base["GetFolderMoveRequestStatistics"];
			}
		}

		public VariantConfigurationSection StartHistoricalSearch
		{
			get
			{
				return base["StartHistoricalSearch"];
			}
		}

		public VariantConfigurationSection CheckForDedicatedTenantAdminRoleNamePrefix
		{
			get
			{
				return base["CheckForDedicatedTenantAdminRoleNamePrefix"];
			}
		}

		public VariantConfigurationSection SuspendFolderMoveRequest
		{
			get
			{
				return base["SuspendFolderMoveRequest"];
			}
		}

		public VariantConfigurationSection NewMailboxImportRequest
		{
			get
			{
				return base["NewMailboxImportRequest"];
			}
		}

		public VariantConfigurationSection NewMigrationBatch
		{
			get
			{
				return base["NewMigrationBatch"];
			}
		}

		public VariantConfigurationSection SetComplianceSearch
		{
			get
			{
				return base["SetComplianceSearch"];
			}
		}

		public VariantConfigurationSection GetSPOTeamSiteStorageReport
		{
			get
			{
				return base["GetSPOTeamSiteStorageReport"];
			}
		}

		public VariantConfigurationSection GetHoldCompliancePolicy
		{
			get
			{
				return base["GetHoldCompliancePolicy"];
			}
		}

		public VariantConfigurationSection GetDlpSensitiveInformationType
		{
			get
			{
				return base["GetDlpSensitiveInformationType"];
			}
		}

		public VariantConfigurationSection GetReportScheduleList
		{
			get
			{
				return base["GetReportScheduleList"];
			}
		}

		public VariantConfigurationSection GetMailbox
		{
			get
			{
				return base["GetMailbox"];
			}
		}

		public VariantConfigurationSection GetSPOTenantStorageMetricReport
		{
			get
			{
				return base["GetSPOTenantStorageMetricReport"];
			}
		}

		public VariantConfigurationSection NewMailUser
		{
			get
			{
				return base["NewMailUser"];
			}
		}

		public VariantConfigurationSection GetReportSchedule
		{
			get
			{
				return base["GetReportSchedule"];
			}
		}

		public VariantConfigurationSection SetActiveArchiveStatus
		{
			get
			{
				return base["SetActiveArchiveStatus"];
			}
		}

		public VariantConfigurationSection GetAuditConfig
		{
			get
			{
				return base["GetAuditConfig"];
			}
		}

		public VariantConfigurationSection WsSecuritySymmetricAndX509Cert
		{
			get
			{
				return base["WsSecuritySymmetricAndX509Cert"];
			}
		}

		public VariantConfigurationSection ProxyDllUpdate
		{
			get
			{
				return base["ProxyDllUpdate"];
			}
		}

		public VariantConfigurationSection NewHoldCompliancePolicy
		{
			get
			{
				return base["NewHoldCompliancePolicy"];
			}
		}
	}
}
