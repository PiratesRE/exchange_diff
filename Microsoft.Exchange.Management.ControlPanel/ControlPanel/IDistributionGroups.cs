using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DistributionGroups")]
	public interface IDistributionGroups : IDataSourceService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, SetDistributionGroup, NewDistributionGroupParameters>, IDataSourceService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, SetDistributionGroup, NewDistributionGroupParameters, BaseWebServiceParameters>, IEditListService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, NewDistributionGroupParameters, BaseWebServiceParameters>, IGetListService<DistributionGroupFilter, DistributionGroupRow>, INewObjectService<DistributionGroupRow, NewDistributionGroupParameters>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<DistributionGroup, SetDistributionGroup, DistributionGroupRow>, IGetObjectService<DistributionGroup>, IGetObjectForListService<DistributionGroupRow>
	{
	}
}
