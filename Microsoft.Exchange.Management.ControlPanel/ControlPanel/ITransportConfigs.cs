using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "TransportConfigs")]
	public interface ITransportConfigs : IGetListService<TransportConfigFilter, SupervisionTag>
	{
	}
}
