using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "InboxRules")]
	public interface IInboxRules : IDataSourceService<InboxRuleFilter, RuleRow, InboxRule, SetInboxRule, NewInboxRule, RemoveInboxRule>, IEditListService<InboxRuleFilter, RuleRow, InboxRule, NewInboxRule, RemoveInboxRule>, IGetListService<InboxRuleFilter, RuleRow>, INewObjectService<RuleRow, NewInboxRule>, IRemoveObjectsService<RemoveInboxRule>, IEditObjectForListService<InboxRule, SetInboxRule, RuleRow>, IGetObjectService<InboxRule>, IGetObjectForListService<RuleRow>
	{
		[OperationContract]
		PowerShellResults IncreasePriority(Identity[] identities, ChangeInboxRule parameters);

		[OperationContract]
		PowerShellResults DecreasePriority(Identity[] identities, ChangeInboxRule parameters);

		[OperationContract]
		PowerShellResults<RuleRow> DisableRule(Identity[] identities, DisableInboxRule parameters);

		[OperationContract]
		PowerShellResults<RuleRow> EnableRule(Identity[] identities, EnableInboxRule parameters);
	}
}
