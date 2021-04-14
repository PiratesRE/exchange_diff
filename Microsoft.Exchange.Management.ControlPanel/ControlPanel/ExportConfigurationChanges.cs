using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ExportConfigurationChanges : DataSourceService, IExportConfigurationChanges, INewObjectService<ExportConfigurationChangesRow, ExportConfigurationChangesParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "New-AdminAuditLogSearch?StartDate&EndDate&StatusMailRecipients@R:Organization")]
		public PowerShellResults<ExportConfigurationChangesRow> NewObject(ExportConfigurationChangesParameters parameters)
		{
			if (parameters.EndDate.IsNullOrBlank() || parameters.StartDate.IsNullOrBlank())
			{
				throw new FaultException(ClientStrings.DatesNotDefined);
			}
			if (parameters.StatusMailRecipients.IsNullOrBlank())
			{
				throw new FaultException(Strings.MailRecipientNotDefined);
			}
			return base.NewObject<ExportConfigurationChangesRow, ExportConfigurationChangesParameters>("New-AdminAuditLogSearch", parameters);
		}

		internal const string WriteScope = "@R:Organization";

		internal const string NewCmdlet = "New-AdminAuditLogSearch";

		private const string NewObjectRole = "New-AdminAuditLogSearch?StartDate&EndDate&StatusMailRecipients@R:Organization";
	}
}
