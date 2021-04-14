using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MemberOfGroups")]
	public interface IMemberOfGroups : IGetListService<MemberOfGroupFilter, RecipientRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
	}
}
