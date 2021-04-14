using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IRemoveObjectsService")]
	public interface IRemoveObjectsService : IRemoveObjectsService<BaseWebServiceParameters>
	{
	}
}
