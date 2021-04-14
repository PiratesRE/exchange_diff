using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IRemoveObjectsService")]
	public interface IRemoveObjectsService<R>
	{
		[OperationContract]
		PowerShellResults RemoveObjects(Identity[] identities, R parameters);
	}
}
