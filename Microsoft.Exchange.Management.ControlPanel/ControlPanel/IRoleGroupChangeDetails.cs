using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RoleGroupChangeDetails")]
	public interface IRoleGroupChangeDetails : IGetObjectService<AdminAuditLogDetailRow>
	{
	}
}
