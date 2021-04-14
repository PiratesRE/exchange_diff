using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IDataSourceService")]
	public interface IDataSourceService<F, L, O, U, C, R> : IEditListService<F, L, O, C, R>, IGetListService<F, L>, INewObjectService<L, C>, IRemoveObjectsService<R>, IEditObjectForListService<O, U, L>, IGetObjectService<O>, IGetObjectForListService<L> where L : BaseRow where O : L
	{
	}
}
