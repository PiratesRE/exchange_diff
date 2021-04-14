using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class JournalReportNdrToService : DataSourceService, IJournalReportNdrTo, IEditObjectService<JournalReportNdrTo, SetJournalReportNdrTo>, IGetObjectService<JournalReportNdrTo>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportConfig@C:OrganizationConfig")]
		public PowerShellResults<JournalReportNdrTo> GetObject(Identity identity)
		{
			return base.GetObject<JournalReportNdrTo>("Get-TransportConfig");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportConfig@C:OrganizationConfig+Set-TransportConfig@C:OrganizationConfig")]
		public PowerShellResults<JournalReportNdrTo> SetObject(Identity identity, SetJournalReportNdrTo properties)
		{
			PowerShellResults<JournalReportNdrTo> powerShellResults = new PowerShellResults<JournalReportNdrTo>();
			properties.IgnoreNullOrEmpty = false;
			if (properties.Any<KeyValuePair<string, object>>())
			{
				PSCommand psCommand = new PSCommand().AddCommand("Set-TransportConfig");
				psCommand.AddParameters(properties);
				PowerShellResults<JournalReportNdrTo> results = base.Invoke<JournalReportNdrTo>(psCommand);
				powerShellResults.MergeAll(results);
			}
			if (powerShellResults.Succeeded)
			{
				powerShellResults.MergeAll(this.GetObject(identity));
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-TransportConfig";

		internal const string SetCmdlet = "Set-TransportConfig";

		internal const string JournalingReportNdrToParameterName = "JournalingReportNdrTo";

		internal const string ReadScope = "@C:OrganizationConfig";

		internal const string WriteScope = "@C:OrganizationConfig";

		private const string GetObjectRole = "Get-TransportConfig@C:OrganizationConfig";

		private const string SetObjectRole = "Get-TransportConfig@C:OrganizationConfig+Set-TransportConfig@C:OrganizationConfig";
	}
}
