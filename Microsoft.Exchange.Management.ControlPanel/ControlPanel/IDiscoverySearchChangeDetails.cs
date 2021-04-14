using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DiscoverySearchChangeDetails")]
	public interface IDiscoverySearchChangeDetails : IGetObjectService<AdminAuditLogDetailRow>
	{
	}
}
