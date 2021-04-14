using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IGetObjectForListService")]
	public interface IGetObjectForListService<L>
	{
		[OperationContract]
		PowerShellResults<L> GetObjectForList(Identity identity);
	}
}
