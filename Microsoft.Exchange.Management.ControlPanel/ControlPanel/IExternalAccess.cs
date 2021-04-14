using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ExternalAccess")]
	public interface IExternalAccess : IGetListService<ExternalAccessFilter, AdminAuditLogResultRow>
	{
	}
}
