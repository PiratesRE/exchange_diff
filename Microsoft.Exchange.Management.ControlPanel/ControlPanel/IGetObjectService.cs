using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IGetObjectService")]
	public interface IGetObjectService<O>
	{
		[OperationContract]
		PowerShellResults<O> GetObject(Identity identity);
	}
}
