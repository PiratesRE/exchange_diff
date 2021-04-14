using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ExternalAccessDetails : DataSourceService, IExternalAccessDetails, IGetObjectService<AdminAuditLogDetailRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-AdminAuditLog?ResultSize&StartDate&EndDate&ExternalAccess@R:Organization")]
		public PowerShellResults<AdminAuditLogDetailRow> GetObject(Identity identity)
		{
			PowerShellResults<AdminAuditLogDetailRow> powerShellResults = new PowerShellResults<AdminAuditLogDetailRow>();
			if (identity != null && identity.RawIdentity != null)
			{
				ExternalAccessLogDetailsId externalAccessLogDetailsId = new ExternalAccessLogDetailsId(identity);
				ExternalAccessFilter externalAccessFilter = new ExternalAccessFilter();
				if (externalAccessLogDetailsId.StartDate != "NoStart")
				{
					externalAccessFilter.StartDate = externalAccessLogDetailsId.StartDate;
				}
				if (externalAccessLogDetailsId.EndDate != "NoEnd")
				{
					externalAccessFilter.EndDate = externalAccessLogDetailsId.EndDate;
				}
				externalAccessFilter.ExternalAccess = new bool?(true);
				PSCommand pscommand = new PSCommand().AddCommand("Search-AdminAuditLog").AddParameters(externalAccessFilter);
				pscommand.AddParameter("ObjectIds", externalAccessLogDetailsId.ObjectModified);
				pscommand.AddParameter("Cmdlets", externalAccessLogDetailsId.Cmdlet);
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

		private const string GetObjectRole = "Search-AdminAuditLog?ResultSize&StartDate&EndDate&ExternalAccess@R:Organization";
	}
}
