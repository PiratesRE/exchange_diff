using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ActiveSyncSettingsService : DataSourceService, IActiveSyncSettingsService, IEditObjectService<ActiveSyncSettings, SetActiveSyncSettings>, IGetObjectService<ActiveSyncSettings>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig")]
		public PowerShellResults<ActiveSyncSettings> GetObject(Identity identity)
		{
			return base.GetObject<ActiveSyncSettings>("Get-ActiveSyncOrganizationSettings");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig+Set-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig")]
		public PowerShellResults<ActiveSyncSettings> SetObject(Identity identity, SetActiveSyncSettings properties)
		{
			properties.FaultIfNull();
			properties.IgnoreNullOrEmpty = false;
			PowerShellResults<ActiveSyncSettings> powerShellResults = base.Invoke<ActiveSyncSettings>(new PSCommand().AddCommand("Set-ActiveSyncOrganizationSettings").AddParameters(properties));
			if (powerShellResults.Succeeded)
			{
				PowerShellResults<ActiveSyncSettings> @object = this.GetObject(null);
				if (@object != null)
				{
					powerShellResults.MergeAll(@object);
				}
			}
			return powerShellResults;
		}

		private const string Noun = "ActiveSyncOrganizationSettings";

		internal const string GetCmdlet = "Get-ActiveSyncOrganizationSettings";

		internal const string SetCmdlet = "Set-ActiveSyncOrganizationSettings";

		internal const string ReadScope = "@C:OrganizationConfig";

		internal const string WriteScope = "@C:OrganizationConfig";

		private const string GetObjectRole = "Get-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig";

		private const string SetObjectRole = "Get-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig+Set-ActiveSyncOrganizationSettings?Identity@C:OrganizationConfig";
	}
}
