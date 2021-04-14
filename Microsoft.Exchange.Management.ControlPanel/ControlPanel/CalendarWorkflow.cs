using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class CalendarWorkflow : DataSourceService, ICalendarWorkflow, IEditObjectService<CalendarWorkflowConfiguration, SetCalendarWorkflowConfiguration>, IGetObjectService<CalendarWorkflowConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CalendarProcessing?Identity@R:Self")]
		public PowerShellResults<CalendarWorkflowConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<CalendarWorkflowConfiguration>("Get-CalendarProcessing", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self")]
		public PowerShellResults<CalendarWorkflowConfiguration> SetObject(Identity identity, SetCalendarWorkflowConfiguration properties)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<CalendarWorkflowConfiguration> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<CalendarWorkflowConfiguration, SetCalendarWorkflowConfiguration>("Set-CalendarProcessing", identity, properties);
			}
			if (powerShellResults != null && powerShellResults.Succeeded)
			{
				Util.NotifyOWAUserSettingsChanged(UserSettings.Calendar);
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self")]
		public PowerShellResults<CalendarWorkflowConfiguration> UpdateObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			SetCalendarWorkflowConfiguration setCalendarWorkflowConfiguration = new SetCalendarWorkflowConfiguration();
			setCalendarWorkflowConfiguration.AutomateProcessing = CalendarProcessingFlags.AutoUpdate.ToString();
			PowerShellResults<CalendarWorkflowConfiguration> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<CalendarWorkflowConfiguration, SetCalendarWorkflowConfiguration>("Set-CalendarProcessing", identity, setCalendarWorkflowConfiguration);
			}
			if (powerShellResults != null && powerShellResults.Succeeded)
			{
				Util.NotifyOWAUserSettingsChanged(UserSettings.Calendar);
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-CalendarProcessing";

		internal const string SetCmdlet = "Set-CalendarProcessing";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-CalendarProcessing?Identity@R:Self";

		private const string SetObjectRole = "Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self";
	}
}
