using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IEditObjectService")]
	public interface IEditObjectForListService<O, U, L> : IGetObjectService<O>, IGetObjectForListService<L> where O : L where L : BaseRow
	{
		[OperationContract]
		PowerShellResults<L> SetObject(Identity identity, U properties);
	}
}
