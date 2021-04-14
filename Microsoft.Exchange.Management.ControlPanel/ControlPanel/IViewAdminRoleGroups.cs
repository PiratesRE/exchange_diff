using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ViewAdminRoleGroups")]
	public interface IViewAdminRoleGroups : IGetObjectService<AdminRoleGroupObject>
	{
	}
}
