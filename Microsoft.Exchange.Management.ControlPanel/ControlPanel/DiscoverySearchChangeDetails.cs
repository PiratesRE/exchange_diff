using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class DiscoverySearchChangeDetails : DataSourceService, IDiscoverySearchChangeDetails, IGetObjectService<AdminAuditLogDetailRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization")]
		public PowerShellResults<AdminAuditLogDetailRow> GetObject(Identity identity)
		{
			PowerShellResults<AdminAuditLogDetailRow> powerShellResults = new PowerShellResults<AdminAuditLogDetailRow>();
			if (identity != null && identity.RawIdentity != null)
			{
				AuditLogDetailsId changeId = new AuditLogDetailsId(identity);
				AdminAuditLogSearchFilter adminAuditLogSearchFilter = new AdminAuditLogSearchFilter();
				adminAuditLogSearchFilter.Cmdlets = "New-MailboxSearch, Start-MailboxSearch, Get-MailboxSearch, Stop-MailboxSearch, Remove-MailboxSearch, Set-MailboxSearch";
				adminAuditLogSearchFilter.Parameters = "*";
				if (changeId.StartDate != "NoStart")
				{
					adminAuditLogSearchFilter.StartDate = changeId.StartDate;
				}
				if (changeId.EndDate != "NoEnd")
				{
					adminAuditLogSearchFilter.EndDate = changeId.EndDate;
				}
				PSCommand pscommand = new PSCommand().AddCommand("Search-AdminAuditLog").AddParameters(adminAuditLogSearchFilter);
				pscommand.AddParameter("resultSize", 501);
				PowerShellResults<AdminAuditLogEvent> powerShellResults2 = base.Invoke<AdminAuditLogEvent>(pscommand);
				if (powerShellResults2.Succeeded)
				{
					if (powerShellResults2.Output.Length == 501)
					{
						powerShellResults.Warnings = new string[]
						{
							Strings.TooManyAuditLogsInDetailsPane
						};
					}
					powerShellResults.MergeOutput((from x in powerShellResults2.Output
					where x.SearchObject == changeId.Object
					select new AdminAuditLogDetailRow(identity, x)).ToArray<AdminAuditLogDetailRow>());
				}
				powerShellResults.MergeErrors<AdminAuditLogEvent>(powerShellResults2);
			}
			return powerShellResults;
		}

		internal const string ReadScope = "@R:Organization";

		private const string GetObjectRole = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization";
	}
}
