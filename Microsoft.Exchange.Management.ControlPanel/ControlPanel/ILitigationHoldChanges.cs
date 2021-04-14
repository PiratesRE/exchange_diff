using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "LitigationHoldChanges")]
	public interface ILitigationHoldChanges : IGetListService<AdminAuditLogSearchFilter, AdminAuditLogResultRow>
	{
	}
}
