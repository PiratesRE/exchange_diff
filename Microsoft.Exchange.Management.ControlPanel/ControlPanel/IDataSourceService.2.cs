using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IDataSourceService")]
	public interface IDataSourceService<F, L, O, U, C> : IDataSourceService<F, L, O, U, C, BaseWebServiceParameters>, IEditListService<F, L, O, C, BaseWebServiceParameters>, IGetListService<F, L>, INewObjectService<L, C>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<O, U, L>, IGetObjectService<O>, IGetObjectForListService<L> where L : BaseRow where O : L
	{
	}
}
