using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class CalendarDiagnosticLogService : DataSourceService, ICalendarDiagnosticLogService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Enterprise+Get-CalendarDiagnosticLog?Identity@R:Self")]
		public PowerShellResults SendLog(CalendarDiagnosticLog properties)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-CalendarDiagnosticLog");
			pscommand.AddParameter("Identity", Identity.FromExecutingUserId());
			pscommand.AddParameters(properties);
			return base.Invoke(pscommand);
		}

		internal const string Cmdlet = "Get-CalendarDiagnosticLog";

		internal const string Scope = "@R:Self";

		private const string ObjectRole = "Enterprise+Get-CalendarDiagnosticLog?Identity@R:Self";
	}
}
