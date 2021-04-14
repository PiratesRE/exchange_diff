using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class IpSafeListings : DataSourceService, IIpSafeListings, IEditObjectService<IpSafeListing, SetIpSafeListing>, IGetObjectService<IpSafeListing>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-PerimeterConfig?Identity@R:Organization")]
		public PowerShellResults<IpSafeListing> GetObject(Identity identity)
		{
			return base.GetObject<IpSafeListing>("Get-PerimeterConfig");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-PerimeterConfig?Identity@R:Organization+Set-PerimeterConfig?Identity@W:Organization")]
		public PowerShellResults<IpSafeListing> SetObject(Identity identity, SetIpSafeListing properties)
		{
			identity = new Identity(RbacPrincipal.Current.RbacConfiguration.OrganizationId.ConfigurationUnit, RbacPrincipal.Current.RbacConfiguration.OrganizationId.ConfigurationUnit.Name);
			return base.SetObject<IpSafeListing, SetIpSafeListing>("Set-PerimeterConfig", identity, properties);
		}

		private const string Noun = "PerimeterConfig";

		internal const string GetCmdlet = "Get-PerimeterConfig";

		internal const string SetCmdlet = "Set-PerimeterConfig";

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		internal const string GetObjectRole = "Get-PerimeterConfig?Identity@R:Organization";

		private const string SetObjectRole = "Get-PerimeterConfig?Identity@R:Organization+Set-PerimeterConfig?Identity@W:Organization";
	}
}
