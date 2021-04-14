using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class OrganizationService : DataSourceService, IOrganizationService, IAsyncService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-OrganizationCustomization@C:OrganizationConfig")]
		public PowerShellResults EnableOrganizationCustomization()
		{
			LocalSession localSession = LocalSession.Current;
			RbacSettings.AddSessionToCache(localSession.CacheKeys[0], localSession, false, false);
			return base.InvokeAsync(new PSCommand().AddCommand("Enable-OrganizationCustomization"), delegate(PowerShellResults results)
			{
				if (results != null && results.ErrorRecords.IsNullOrEmpty())
				{
					LocalSession.Current.FlushCache();
				}
			});
		}

		internal const string EnableOrganizationCustomizationCmdlet = "Enable-OrganizationCustomization";

		internal const string WriteScope = "@C:OrganizationConfig";

		internal const string EnableOrganizationCustomizationRole = "Enable-OrganizationCustomization@C:OrganizationConfig";
	}
}
