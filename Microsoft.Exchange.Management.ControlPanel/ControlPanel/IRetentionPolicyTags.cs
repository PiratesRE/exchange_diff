using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RetentionPolicyTags")]
	public interface IRetentionPolicyTags : IGetListService<AllAssociatedRPTsFilter, RetentionPolicyTagRow>, IGetObjectService<ViewRetentionPolicyTagRow>, INewObjectService<RetentionPolicyTagRow, AddRetentionPolicyTag>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
	}
}
