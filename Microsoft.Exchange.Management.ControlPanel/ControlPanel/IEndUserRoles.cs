using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "EndUserRoles")]
	public interface IEndUserRoles : IGetListService<ManagementRoleFilter, EndUserRoleRow>
	{
	}
}
