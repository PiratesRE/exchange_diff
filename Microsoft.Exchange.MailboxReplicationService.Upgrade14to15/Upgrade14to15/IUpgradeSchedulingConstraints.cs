using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.WorkloadService.IUpgradeSchedulingConstraints")]
	public interface IUpgradeSchedulingConstraints
	{
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadinessArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadiness", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadinessResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadinessAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateTenantReadiness(TenantReadiness[] tenantReadiness);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadiness", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateTenantReadinessResponse")]
		IAsyncResult BeginUpdateTenantReadiness(TenantReadiness[] tenantReadiness, AsyncCallback callback, object asyncState);

		void EndUpdateTenantReadiness(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroup", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroupResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroupAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroupArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateGroup(Group[] group);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroup", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateGroupResponse")]
		IAsyncResult BeginUpdateGroup(Group[] group, AsyncCallback callback, object asyncState);

		void EndUpdateGroup(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacity", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacityResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacityAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacityArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateCapacity(GroupCapacity[] capacities);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacity", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateCapacityResponse")]
		IAsyncResult BeginUpdateCapacity(GroupCapacity[] capacities, AsyncCallback callback, object asyncState);

		void EndUpdateCapacity(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackout", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackoutResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackoutAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackoutArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateBlackout(GroupBlackout[] blackouts);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackout", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateBlackoutResponse")]
		IAsyncResult BeginUpdateBlackout(GroupBlackout[] blackouts, AsyncCallback callback, object asyncState);

		void EndUpdateBlackout(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraint", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraintResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraintAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraintArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void UpdateConstraint(Constraint[] constraint);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraint", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/UpdateConstraintResponse")]
		IAsyncResult BeginUpdateConstraint(Constraint[] constraint, AsyncCallback callback, object asyncState);

		void EndUpdateConstraint(IAsyncResult result);

		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadinessAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadiness", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadinessResponse")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadinessArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		TenantReadiness[] QueryTenantReadiness(Guid[] tenantIds);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadiness", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryTenantReadinessResponse")]
		IAsyncResult BeginQueryTenantReadiness(Guid[] tenantIds, AsyncCallback callback, object asyncState);

		TenantReadiness[] EndQueryTenantReadiness(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroup", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroupResponse")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroupArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroupAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		Group[] QueryGroup(string[] groupNames);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroup", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryGroupResponse")]
		IAsyncResult BeginQueryGroup(string[] groupNames, AsyncCallback callback, object asyncState);

		Group[] EndQueryGroup(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacityArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacityAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacity", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacityResponse")]
		GroupCapacity[] QueryCapacity(string[] groupNames);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacity", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryCapacityResponse")]
		IAsyncResult BeginQueryCapacity(string[] groupNames, AsyncCallback callback, object asyncState);

		GroupCapacity[] EndQueryCapacity(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackoutArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackoutAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackout", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackoutResponse")]
		GroupBlackout[] QueryBlackout(string[] groupNames);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackout", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryBlackoutResponse")]
		IAsyncResult BeginQueryBlackout(string[] groupNames, AsyncCallback callback, object asyncState);

		GroupBlackout[] EndQueryBlackout(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraintArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraintAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraint", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraintResponse")]
		Constraint[] QueryConstraint(string[] constraintName);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraint", ReplyAction = "http://tempuri.org/IUpgradeSchedulingConstraints/QueryConstraintResponse")]
		IAsyncResult BeginQueryConstraint(string[] constraintName, AsyncCallback callback, object asyncState);

		Constraint[] EndQueryConstraint(IAsyncResult result);
	}
}
