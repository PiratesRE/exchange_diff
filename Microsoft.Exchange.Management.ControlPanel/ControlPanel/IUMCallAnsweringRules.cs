using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMCallAnsweringRules")]
	public interface IUMCallAnsweringRules : IDataSourceService<UMCallAnsweringRuleFilter, RuleRow, UMCallAnsweringRule, SetUMCallAnsweringRule, NewUMCallAnsweringRule, RemoveUMCallAnsweringRule>, IEditListService<UMCallAnsweringRuleFilter, RuleRow, UMCallAnsweringRule, NewUMCallAnsweringRule, RemoveUMCallAnsweringRule>, IGetListService<UMCallAnsweringRuleFilter, RuleRow>, INewObjectService<RuleRow, NewUMCallAnsweringRule>, IRemoveObjectsService<RemoveUMCallAnsweringRule>, IEditObjectForListService<UMCallAnsweringRule, SetUMCallAnsweringRule, RuleRow>, IGetObjectService<UMCallAnsweringRule>, IGetObjectForListService<RuleRow>
	{
		[OperationContract]
		PowerShellResults IncreasePriority(Identity[] identities, ChangeUMCallAnsweringRule parameters);

		[OperationContract]
		PowerShellResults DecreasePriority(Identity[] identities, ChangeUMCallAnsweringRule parameters);

		[OperationContract]
		PowerShellResults<RuleRow> DisableRule(Identity[] identities, DisableUMCallAnsweringRule parameters);

		[OperationContract]
		PowerShellResults<RuleRow> EnableRule(Identity[] identities, EnableUMCallAnsweringRule parameters);
	}
}
