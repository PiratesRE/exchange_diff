using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.WorkloadService.IUpgradeHandler")]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public interface IUpgradeHandler
	{
		[OperationContract(Action = "http://tempuri.org/IUpgradeHandler/QueryWorkItems", ReplyAction = "http://tempuri.org/IUpgradeHandler/QueryWorkItemsResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeHandler/QueryWorkItemsAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeHandler/QueryWorkItemsArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		WorkItemQueryResult QueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeHandler/QueryWorkItems", ReplyAction = "http://tempuri.org/IUpgradeHandler/QueryWorkItemsResponse")]
		IAsyncResult BeginQueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark, AsyncCallback callback, object asyncState);

		WorkItemQueryResult EndQueryWorkItems(IAsyncResult result);

		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItemsAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItemsArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItems", ReplyAction = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItemsResponse")]
		WorkItemInfo[] QueryTenantWorkItems(Guid tenantId);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItems", ReplyAction = "http://tempuri.org/IUpgradeHandler/QueryTenantWorkItemsResponse")]
		IAsyncResult BeginQueryTenantWorkItems(Guid tenantId, AsyncCallback callback, object asyncState);

		WorkItemInfo[] EndQueryTenantWorkItems(IAsyncResult result);

		[FaultContract(typeof(InvalidOperationFault), Action = "http://tempuri.org/IUpgradeHandler/UpdateWorkItemInvalidOperationFaultFault", Name = "InvalidOperationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeHandler/UpdateWorkItem", ReplyAction = "http://tempuri.org/IUpgradeHandler/UpdateWorkItemResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeHandler/UpdateWorkItemAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeHandler/UpdateWorkItemArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateWorkItem(string workItemId, WorkItemStatusInfo status);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeHandler/UpdateWorkItem", ReplyAction = "http://tempuri.org/IUpgradeHandler/UpdateWorkItemResponse")]
		IAsyncResult BeginUpdateWorkItem(string workItemId, WorkItemStatusInfo status, AsyncCallback callback, object asyncState);

		void EndUpdateWorkItem(IAsyncResult result);
	}
}
