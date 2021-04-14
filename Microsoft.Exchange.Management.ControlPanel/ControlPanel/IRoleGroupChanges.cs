using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RoleGroupChanges")]
	public interface IRoleGroupChanges : IGetListService<AdminAuditLogSearchFilter, AdminAuditLogResultRow>
	{
	}
}
