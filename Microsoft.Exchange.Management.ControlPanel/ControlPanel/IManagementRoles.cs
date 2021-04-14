using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ManagementRoles")]
	public interface IManagementRoles : IGetListService<ManagementRoleFilter, ManagementRoleRow>
	{
	}
}
