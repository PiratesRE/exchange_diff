using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "LitigationHoldChangeDetails")]
	public interface ILitigationHoldChangeDetails : IGetObjectService<AdminAuditLogDetailRow>
	{
	}
}
