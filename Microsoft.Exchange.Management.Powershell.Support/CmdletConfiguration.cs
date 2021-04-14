using System;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal class CmdletConfiguration
	{
		internal static CmdletConfigurationEntry[] SupportCmdletConfigurationEntries
		{
			get
			{
				return CmdletConfiguration.supportCmdletConfigurationEntries;
			}
		}

		internal static FormatConfigurationEntry[] SupportFormatConfigurationEntries
		{
			get
			{
				return CmdletConfiguration.supportFormatConfigurationEntries;
			}
		}

		private static CmdletConfigurationEntry[] supportCmdletConfigurationEntries = new CmdletConfigurationEntry[]
		{
			new CmdletConfigurationEntry("Get-DatabaseEvent", typeof(GetDatabaseEvent), "Microsoft.Exchange.Support-Help.xml"),
			new CmdletConfigurationEntry("Get-DatabaseEventWatermark", typeof(GetDatabaseEventWatermark), "Microsoft.Exchange.Support-Help.xml"),
			new CmdletConfigurationEntry("Get-CalendarValidationResult", typeof(GetCalendarValidationResult), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-ExchangeDiagnosticInfo", typeof(GetExchangeDiagnosticInfo), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Test-Message", typeof(TestMessage), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-MailboxActivityLog", typeof(GetMailboxActivityLog), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Repair-Migration", typeof(RepairMigration), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-FolderRestriction", typeof(GetFolderRestriction), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-OABFile", typeof(GetOABFile), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-MailboxFileStore", typeof(GetMailboxFileStore), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Remove-MailboxFileStore", typeof(RemoveMailboxFileStore), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-WebDnsRecord", typeof(GetWebDnsRecord), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-GroupCapacity", typeof(GetGroupCapacity), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-GroupBlackout", typeof(GetGroupBlackout), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-Constraint", typeof(GetConstraint), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-SymphonyGroup", typeof(GetSymphonyGroup), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-TenantReadiness", typeof(GetTenantReadiness), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-UpgradeWorkItem", typeof(GetUpgradeWorkItem), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-Constraint", typeof(SetConstraint), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-GroupCapacity", typeof(SetGroupCapacity), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-GroupBlackout", typeof(SetGroupBlackout), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-SymphonyGroup", typeof(SetSymphonyGroup), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-TenantReadiness", typeof(SetTenantReadiness), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Set-UpgradeWorkItem", typeof(SetUpgradeWorkItem), "Microsoft.Exchange.Management-Help.xml"),
			new CmdletConfigurationEntry("Get-UnifiedGroup", typeof(GetUnifiedGroup), null),
			new CmdletConfigurationEntry("New-UnifiedGroup", typeof(NewUnifiedGroup), null),
			new CmdletConfigurationEntry("Remove-UnifiedGroup", typeof(RemoveUnifiedGroup), null),
			new CmdletConfigurationEntry("Set-UnifiedGroup", typeof(SetUnifiedGroup), null)
		};

		private static FormatConfigurationEntry[] supportFormatConfigurationEntries = new FormatConfigurationEntry[]
		{
			new FormatConfigurationEntry("Exchange.Support.format.ps1xml")
		};
	}
}
