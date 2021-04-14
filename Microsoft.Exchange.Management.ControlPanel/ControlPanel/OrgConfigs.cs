using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class OrgConfigs : DataSourceService, IOrgConfigs, IEditObjectService<OrgConfig, SetOrgConfig>, IGetObjectService<OrgConfig>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-OrganizationConfig@C:OrganizationConfig")]
		public PowerShellResults<OrgConfig> GetObject(Identity identity)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-OrganizationConfig");
			return base.Invoke<OrgConfig>(psCommand);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-OrganizationConfig@C:OrganizationConfig+Set-OrganizationConfig@C:OrganizationConfig")]
		public PowerShellResults<OrgConfig> SetObject(Identity identity, SetOrgConfig properties)
		{
			PowerShellResults<OrgConfig> powerShellResults = new PowerShellResults<OrgConfig>();
			properties.IgnoreNullOrEmpty = false;
			if (properties.Any<KeyValuePair<string, object>>())
			{
				PSCommand psCommand = new PSCommand().AddCommand("Set-OrganizationConfig");
				psCommand.AddParameters(properties);
				PowerShellResults<OrgConfig> results = base.Invoke<OrgConfig>(psCommand);
				powerShellResults.MergeAll(results);
			}
			if (powerShellResults.Succeeded)
			{
				powerShellResults.MergeAll(this.GetObject(identity));
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-OrganizationConfig";

		internal const string SetCmdlet = "Set-OrganizationConfig";

		private const string GetObjectRole = "Get-OrganizationConfig@C:OrganizationConfig";

		private const string SetObjectRole = "Get-OrganizationConfig@C:OrganizationConfig+Set-OrganizationConfig@C:OrganizationConfig";
	}
}
