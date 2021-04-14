using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class LitigationHoldChangeDetails : DataSourceService, ILitigationHoldChangeDetails, IGetObjectService<AdminAuditLogDetailRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization")]
		public PowerShellResults<AdminAuditLogDetailRow> GetObject(Identity identity)
		{
			PowerShellResults<AdminAuditLogDetailRow> powerShellResults = new PowerShellResults<AdminAuditLogDetailRow>();
			if (identity != null && identity.RawIdentity != null)
			{
				AuditLogDetailsId auditLogDetailsId = new AuditLogDetailsId(identity);
				AdminAuditLogSearchFilter adminAuditLogSearchFilter = new AdminAuditLogSearchFilter();
				adminAuditLogSearchFilter.Cmdlets = "Set-Mailbox";
				adminAuditLogSearchFilter.Parameters = "LitigationHoldEnabled";
				if (auditLogDetailsId.StartDate != "NoStart")
				{
					adminAuditLogSearchFilter.StartDate = auditLogDetailsId.StartDate;
				}
				if (auditLogDetailsId.EndDate != "NoEnd")
				{
					adminAuditLogSearchFilter.EndDate = auditLogDetailsId.EndDate;
				}
				PSCommand pscommand = new PSCommand().AddCommand("Search-AdminAuditLog").AddParameters(adminAuditLogSearchFilter);
				pscommand.AddParameter("objectIds", auditLogDetailsId.Object);
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
					AdminAuditLogDetailRow[] array = new AdminAuditLogDetailRow[powerShellResults2.Output.Length];
					for (int i = 0; i < powerShellResults2.Output.Length; i++)
					{
						array[i] = new AdminAuditLogDetailRow(identity, powerShellResults2.Output[i]);
					}
					powerShellResults.MergeOutput(array);
				}
				powerShellResults.MergeErrors<AdminAuditLogEvent>(powerShellResults2);
			}
			return powerShellResults;
		}

		internal const string ReadScope = "@R:Organization";

		private const string GetObjectRole = "Search-AdminAuditLog?StartDate&EndDate&ObjectIds&Cmdlets@R:Organization";
	}
}
