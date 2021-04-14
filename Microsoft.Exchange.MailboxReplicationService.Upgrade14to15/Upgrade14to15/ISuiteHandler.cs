using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.SuiteService.ISuiteHandler")]
	public interface ISuiteHandler
	{
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ISuiteHandler/AddPilotUsersArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(InvalidOperationFault), Action = "http://tempuri.org/ISuiteHandler/AddPilotUsersInvalidOperationFaultFault", Name = "InvalidOperationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/ISuiteHandler/AddPilotUsers", ReplyAction = "http://tempuri.org/ISuiteHandler/AddPilotUsersResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ISuiteHandler/AddPilotUsersAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		int AddPilotUsers(Guid tenantId, UserId[] users);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISuiteHandler/AddPilotUsers", ReplyAction = "http://tempuri.org/ISuiteHandler/AddPilotUsersResponse")]
		IAsyncResult BeginAddPilotUsers(Guid tenantId, UserId[] users, AsyncCallback callback, object asyncState);

		int EndAddPilotUsers(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/ISuiteHandler/GetPilotUsers", ReplyAction = "http://tempuri.org/ISuiteHandler/GetPilotUsersResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ISuiteHandler/GetPilotUsersAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ISuiteHandler/GetPilotUsersArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		UserWorkloadStatusInfo[] GetPilotUsers(Guid tenantId);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISuiteHandler/GetPilotUsers", ReplyAction = "http://tempuri.org/ISuiteHandler/GetPilotUsersResponse")]
		IAsyncResult BeginGetPilotUsers(Guid tenantId, AsyncCallback callback, object asyncState);

		UserWorkloadStatusInfo[] EndGetPilotUsers(IAsyncResult result);

		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgradeAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgrade", ReplyAction = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgradeResponse")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgradeArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[FaultContract(typeof(InvalidOperationFault), Action = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgradeInvalidOperationFaultFault", Name = "InvalidOperationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		void PostponeTenantUpgrade(Guid tenantId, string userUpn);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgrade", ReplyAction = "http://tempuri.org/ISuiteHandler/PostponeTenantUpgradeResponse")]
		IAsyncResult BeginPostponeTenantUpgrade(Guid tenantId, string userUpn, AsyncCallback callback, object asyncState);

		void EndPostponeTenantUpgrade(IAsyncResult result);
	}
}
