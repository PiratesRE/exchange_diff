using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "JournalRules")]
	public interface IJournalRules : IDataSourceService<JournalRuleFilter, JournalRuleRow, JournalRule, SetJournalRule, NewJournalRule>, IDataSourceService<JournalRuleFilter, JournalRuleRow, JournalRule, SetJournalRule, NewJournalRule, BaseWebServiceParameters>, IEditListService<JournalRuleFilter, JournalRuleRow, JournalRule, NewJournalRule, BaseWebServiceParameters>, IGetListService<JournalRuleFilter, JournalRuleRow>, INewObjectService<JournalRuleRow, NewJournalRule>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<JournalRule, SetJournalRule, JournalRuleRow>, IGetObjectService<JournalRule>, IGetObjectForListService<JournalRuleRow>
	{
		[OperationContract]
		PowerShellResults<JournalRuleRow> DisableRule(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<JournalRuleRow> EnableRule(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
