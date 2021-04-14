using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "INewGetObjectService")]
	public interface INewGetObjectService<L, C, W> : INewObjectService<L, C> where L : BaseRow
	{
		[OperationContract]
		PowerShellResults<W> GetObjectForNew(Identity identity);
	}
}
