using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IGetListService")]
	public interface IGetListService<F, L> where L : BaseRow
	{
		[OperationContract]
		PowerShellResults<L> GetList(F filter, SortOptions sort);
	}
}
