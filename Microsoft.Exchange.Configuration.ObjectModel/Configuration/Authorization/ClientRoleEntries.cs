using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class ClientRoleEntries
	{
		internal static readonly string[] ParametersForProxy = new string[]
		{
			"Force",
			"Confirm"
		};

		internal static RoleEntryInfo[] EMCRequiredRoleEntries = new RoleEntryInfo[]
		{
			new RoleEntryInfo(new ScriptRoleEntry("ConsoleInitialize.ps1", null))
		};

		internal static RoleEntry[] ECPRequiredParameters = new RoleEntry[]
		{
			new CmdletRoleEntry("Get-Contact", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-DistributionGroup", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-DistributionGroupMember", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-DynamicDistributionGroup", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-Group", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-Mailbox", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-MailContact", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-MailUser", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-Recipient", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-RoleGroup", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-SiteMailbox", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Get-User", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"ReadFromDomainController"
			}),
			new CmdletRoleEntry("Import-UMPrompt", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"PromptFileStream"
			})
		};

		internal static RoleEntry[] TenantReportingRequiredParameters = new RoleEntry[]
		{
			new CmdletRoleEntry("Get-ConnectionByClientTypeDetailReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ConnectionByClientTypeReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsActiveUserReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsAVConferenceTimeReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsClientDeviceReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsConferenceReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsP2PAVTimeReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-CsP2PSessionReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ExternalActivityByDomainReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ExternalActivityByUserReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ExternalActivityReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ExternalActivitySummaryReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-GroupActivityReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-LicenseVsUsageSummaryReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-MailboxActivityReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-MailboxUsageDetailReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-MailboxUsageReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-MobileDeviceDashboardSummaryReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-MobileDeviceDetailsReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-O365ClientBrowserDetailReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-O365ClientBrowserReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-O365ClientOSDetailReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-O365ClientOSReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-PartnerClientExpiringSubscriptionReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-PartnerCustomerUserReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ScorecardClientDeviceReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ScorecardClientOSReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ScorecardClientOutlookReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-ScorecardMetricsReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOActiveUserReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOOneDriveForBusinessFileActivityReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOOneDriveForBusinessUserStatisticsReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOSkyDriveProDeployedReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOSkyDriveProStorageReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOTeamSiteDeployedReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOTeamSiteStorageReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-SPOTenantStorageMetricReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-StaleMailboxDetailReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-StaleMailboxReport", "Microsoft.Exchange.Management.PowerShell.E2010", new string[]
			{
				"Expression"
			})
		};

		internal static RoleEntry[] EngineeringFundamentalsReportingRequiredParameters = new RoleEntry[]
		{
			new CmdletRoleEntry("Get-DMEngineeringFundamentalsMetadataSourceReport", "Microsoft.Exchange.Monitoring.Reporting", new string[]
			{
				"Expression"
			}),
			new CmdletRoleEntry("Get-DMEngineeringFundamentalsPumperStatusReport", "Microsoft.Exchange.Monitoring.Reporting", new string[]
			{
				"Expression"
			})
		};
	}
}
