using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement.ITestTenantManagement")]
	public interface ITestTenantManagement
	{
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulateAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulateArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulate", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulateResponse")]
		Tenant[] QueryTenantsToPopulate(PopulationStatus status);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulate", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToPopulateResponse")]
		IAsyncResult BeginQueryTenantsToPopulate(PopulationStatus status, AsyncCallback callback, object asyncState);

		Tenant[] EndQueryTenantsToPopulate(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidate", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		Tenant[] QueryTenantsToValidate(ValidationStatus status);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidate", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateResponse")]
		IAsyncResult BeginQueryTenantsToValidate(ValidationStatus status, AsyncCallback callback, object asyncState);

		Tenant[] EndQueryTenantsToValidate(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenarioArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenarioAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenario", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenarioResponse")]
		Tenant[] QueryTenantsToValidateByScenario(ValidationStatus status, string scenario);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenario", ReplyAction = "http://tempuri.org/ITestTenantManagement/QueryTenantsToValidateByScenarioResponse")]
		IAsyncResult BeginQueryTenantsToValidateByScenario(ValidationStatus status, string scenario, AsyncCallback callback, object asyncState);

		Tenant[] EndQueryTenantsToValidateByScenario(IAsyncResult result);

		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatus", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		void UpdateTenantPopulationStatus(Guid tenantId, PopulationStatus status);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatus", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusResponse")]
		IAsyncResult BeginUpdateTenantPopulationStatus(Guid tenantId, PopulationStatus status, AsyncCallback callback, object asyncState);

		void EndUpdateTenantPopulationStatus(IAsyncResult result);

		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenarioAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenario", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenarioResponse")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenarioArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		void UpdateTenantPopulationStatusWithScenario(Guid tenantId, PopulationStatus status, string scenario, string comment);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenario", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantPopulationStatusWithScenarioResponse")]
		IAsyncResult BeginUpdateTenantPopulationStatusWithScenario(Guid tenantId, PopulationStatus status, string scenario, string comment, AsyncCallback callback, object asyncState);

		void EndUpdateTenantPopulationStatusWithScenario(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatus", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		void UpdateTenantValidationStatus(Guid tenantId, ValidationStatus status, int? office15BugId);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatus", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusResponse")]
		IAsyncResult BeginUpdateTenantValidationStatus(Guid tenantId, ValidationStatus status, int? office15BugId, AsyncCallback callback, object asyncState);

		void EndUpdateTenantValidationStatus(IAsyncResult result);

		[OperationContract(Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReason", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReasonResponse")]
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReasonAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		[FaultContract(typeof(ArgumentFault), Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReasonArgumentFaultFault", Name = "ArgumentFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts.Common")]
		void UpdateTenantValidationStatusWithReason(Guid tenantId, ValidationStatus status, string failureReason);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReason", ReplyAction = "http://tempuri.org/ITestTenantManagement/UpdateTenantValidationStatusWithReasonResponse")]
		IAsyncResult BeginUpdateTenantValidationStatusWithReason(Guid tenantId, ValidationStatus status, string failureReason, AsyncCallback callback, object asyncState);

		void EndUpdateTenantValidationStatusWithReason(IAsyncResult result);
	}
}
