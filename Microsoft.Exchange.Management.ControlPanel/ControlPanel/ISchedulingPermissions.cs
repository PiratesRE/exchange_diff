using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SchedulingPermissions")]
	public interface ISchedulingPermissions : IResourceBase<SchedulingPermissionsConfiguration, SetSchedulingPermissionsConfiguration>, IEditObjectService<SchedulingPermissionsConfiguration, SetSchedulingPermissionsConfiguration>, IGetObjectService<SchedulingPermissionsConfiguration>
	{
	}
}
