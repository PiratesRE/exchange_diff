using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class Supervision : DataSourceService, ISupervision, IEditObjectService<SupervisionStatus, SetSupervisionStatus>, IGetObjectService<SupervisionStatus>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-SupervisionPolicy?Identity@R:Self")]
		public PowerShellResults<SupervisionStatus> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			if (RbacPrincipal.Current.IsInRole("Get-SupervisionPolicy?DisplayDetails"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-SupervisionPolicy");
				pscommand.AddParameter("DisplayDetails");
				return base.GetObject<SupervisionStatus>(pscommand, identity);
			}
			return base.GetObject<SupervisionStatus>("Get-SupervisionPolicy", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-SupervisionPolicy?Identity@R:Self+MultiTenant+Set-SupervisionPolicy?Identity@W:Organization")]
		public PowerShellResults<SupervisionStatus> SetObject(Identity identity, SetSupervisionStatus properties)
		{
			identity = Identity.FromExecutingUserId();
			properties.FaultIfNull();
			PowerShellResults<SupervisionStatus> powerShellResults = new PowerShellResults<SupervisionStatus>();
			powerShellResults.MergeErrors<SupervisionStatus>(base.SetObject<SupervisionStatus, SetSupervisionStatus>("Set-SupervisionPolicy", identity, properties));
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			powerShellResults.MergeAll(base.SetObject<SupervisionStatus, SetClosedCampusOutboundPolicyConfiguration>("Set-SupervisionPolicy", identity, properties.MyClosedCampusOutboundPolicyConfiguration));
			return powerShellResults;
		}

		private const string DisplayDetails = "DisplayDetails";

		internal const string GetCmdlet = "Get-SupervisionPolicy";

		internal const string GetSupervisionPolicyDetailsRole = "Get-SupervisionPolicy?DisplayDetails";

		internal const string SetCmdlet = "Set-SupervisionPolicy";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Organization";

		private const string GetObjectRole = "MultiTenant+Get-SupervisionPolicy?Identity@R:Self";

		private const string SetObjectRole = "MultiTenant+Get-SupervisionPolicy?Identity@R:Self+MultiTenant+Set-SupervisionPolicy?Identity@W:Organization";
	}
}
