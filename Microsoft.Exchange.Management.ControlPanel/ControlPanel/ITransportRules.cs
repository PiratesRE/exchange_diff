using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "TransportRules")]
	public interface ITransportRules : IDataSourceService<TransportRuleFilter, RuleRow, TransportRule, SetTransportRule, NewTransportRule>, IDataSourceService<TransportRuleFilter, RuleRow, TransportRule, SetTransportRule, NewTransportRule, BaseWebServiceParameters>, IEditListService<TransportRuleFilter, RuleRow, TransportRule, NewTransportRule, BaseWebServiceParameters>, IGetListService<TransportRuleFilter, RuleRow>, INewObjectService<RuleRow, NewTransportRule>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<TransportRule, SetTransportRule, RuleRow>, IGetObjectService<TransportRule>, IGetObjectForListService<RuleRow>
	{
		[OperationContract]
		PowerShellResults IncreasePriority(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults DecreasePriority(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<RuleRow> DisableRule(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<RuleRow> EnableRule(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
