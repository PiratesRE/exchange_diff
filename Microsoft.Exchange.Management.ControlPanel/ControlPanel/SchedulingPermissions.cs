using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SchedulingPermissions : ResourceBase, ISchedulingPermissions, IResourceBase<SchedulingPermissionsConfiguration, SetSchedulingPermissionsConfiguration>, IEditObjectService<SchedulingPermissionsConfiguration, SetSchedulingPermissionsConfiguration>, IGetObjectService<SchedulingPermissionsConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self")]
		public PowerShellResults<SchedulingPermissionsConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<SchedulingPermissionsConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self")]
		public PowerShellResults<SchedulingPermissionsConfiguration> SetObject(Identity identity, SetSchedulingPermissionsConfiguration properties)
		{
			return base.SetObject<SchedulingPermissionsConfiguration, SetSchedulingPermissionsConfiguration>(identity, properties);
		}
	}
}
