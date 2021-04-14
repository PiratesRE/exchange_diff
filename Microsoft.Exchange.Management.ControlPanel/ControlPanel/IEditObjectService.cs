using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IEditObjectService")]
	public interface IEditObjectService<O, U> : IGetObjectService<O> where O : BaseRow
	{
		[OperationContract]
		PowerShellResults<O> SetObject(Identity identity, U properties);
	}
}
