using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ExternalAccessDetails")]
	public interface IExternalAccessDetails : IGetObjectService<AdminAuditLogDetailRow>
	{
	}
}
