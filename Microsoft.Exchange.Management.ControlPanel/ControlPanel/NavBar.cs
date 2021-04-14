using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class NavBar : DataSourceService, INavBarService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
		[PrincipalPermission(SecurityAction.Demand, Role = "MyBaseOptions")]
		public PowerShellResults<NavBarPack> GetObject(Identity identity)
		{
			identity.FaultIfNull();
			bool showAdminFeature = identity.RawIdentity == "myorg";
			NavBarClientBase navBarClientBase = NavBarClientBase.Create(showAdminFeature, false, true);
			PowerShellResults<NavBarPack> powerShellResults = new PowerShellResults<NavBarPack>();
			if (navBarClientBase != null)
			{
				navBarClientBase.PrepareNavBarPack();
				NavBarPack navBarPack = navBarClientBase.GetNavBarPack();
				if (navBarPack != null)
				{
					powerShellResults.Output = new NavBarPack[]
					{
						navBarPack
					};
				}
			}
			return powerShellResults;
		}
	}
}
