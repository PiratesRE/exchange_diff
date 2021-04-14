using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "INewObjectService")]
	public interface INewObjectService<L, C> where L : BaseRow
	{
		[OperationContract]
		PowerShellResults<L> NewObject(C properties);
	}
}
