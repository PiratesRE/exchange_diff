using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SearchAllGroups")]
	public interface ISearchAllGroups : IGetListService<SearchAllGroupFilter, GroupRecipientRow>, IGetObjectService<ViewDistributionGroupData>
	{
		[OperationContract]
		PowerShellResults JoinGroups(Identity[] identities);

		[OperationContract]
		PowerShellResults<ViewDistributionGroupData> JoinGroup(Identity identity);

		[OperationContract]
		PowerShellResults<ViewDistributionGroupData> LeaveGroup(Identity identity);
	}
}
