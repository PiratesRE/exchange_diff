using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

[ServiceContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService", ConfigurationName = "LocatorService")]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public interface LocatorService
{
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	FindTenantResponse FindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantResponse")]
	IAsyncResult BeginFindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest, AsyncCallback callback, object asyncState);

	FindTenantResponse EndFindTenant(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	FindDomainResponse FindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainResponse")]
	IAsyncResult BeginFindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest, AsyncCallback callback, object asyncState);

	FindDomainResponse EndFindDomain(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveTenantResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveTenantLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	SaveTenantResponse SaveTenant(RequestIdentity identity, SaveTenantRequest saveTenantRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveTenantResponse")]
	IAsyncResult BeginSaveTenant(RequestIdentity identity, SaveTenantRequest saveTenantRequest, AsyncCallback callback, object asyncState);

	SaveTenantResponse EndSaveTenant(IAsyncResult result);

	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveDomainLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveDomainResponse")]
	SaveDomainResponse SaveDomain(RequestIdentity identity, SaveDomainRequest saveDomainRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveDomainResponse")]
	IAsyncResult BeginSaveDomain(RequestIdentity identity, SaveDomainRequest saveDomainRequest, AsyncCallback callback, object asyncState);

	SaveDomainResponse EndSaveDomain(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteTenantResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteTenantLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	DeleteTenantResponse DeleteTenant(RequestIdentity identity, DeleteTenantRequest deleteTenantRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteTenantResponse")]
	IAsyncResult BeginDeleteTenant(RequestIdentity identity, DeleteTenantRequest deleteTenantRequest, AsyncCallback callback, object asyncState);

	DeleteTenantResponse EndDeleteTenant(IAsyncResult result);

	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteDomainLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteDomainResponse")]
	DeleteDomainResponse DeleteDomain(RequestIdentity identity, DeleteDomainRequest deleteDomainRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteDomain", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteDomainResponse")]
	IAsyncResult BeginDeleteDomain(RequestIdentity identity, DeleteDomainRequest deleteDomainRequest, AsyncCallback callback, object asyncState);

	DeleteDomainResponse EndDeleteDomain(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomains", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainsResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainsLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	FindDomainsResponse FindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomains", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindDomainsResponse")]
	IAsyncResult BeginFindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest, AsyncCallback callback, object asyncState);

	FindDomainsResponse EndFindDomains(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveUserResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveUserLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	SaveUserResponse SaveUser(RequestIdentity identity, SaveUserRequest saveUserRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/SaveUserResponse")]
	IAsyncResult BeginSaveUser(RequestIdentity identity, SaveUserRequest saveUserRequest, AsyncCallback callback, object asyncState);

	SaveUserResponse EndSaveUser(IAsyncResult result);

	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindUserLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindUserResponse")]
	FindUserResponse FindUser(RequestIdentity identity, FindUserRequest findUserRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindUserResponse")]
	IAsyncResult BeginFindUser(RequestIdentity identity, FindUserRequest findUserRequest, AsyncCallback callback, object asyncState);

	FindUserResponse EndFindUser(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteUserResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteUserLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	DeleteUserResponse DeleteUser(RequestIdentity identity, DeleteUserRequest deleteUserRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteUser", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/DeleteUserResponse")]
	IAsyncResult BeginDeleteUser(RequestIdentity identity, DeleteUserRequest deleteUserRequest, AsyncCallback callback, object asyncState);

	DeleteUserResponse EndDeleteUser(IAsyncResult result);

	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantFromPrimaryLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantFromPrimary", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantFromPrimaryResponse")]
	FindTenantResponse[] FindTenantFromPrimary(RequestIdentity identity, FindTenantRequest findTenantRequest, int? copiesToRead);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantFromPrimary", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/FindTenantFromPrimaryResponse")]
	IAsyncResult BeginFindTenantFromPrimary(RequestIdentity identity, FindTenantRequest findTenantRequest, int? copiesToRead, AsyncCallback callback, object asyncState);

	FindTenantResponse[] EndFindTenantFromPrimary(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/GetGlsMachineStatus", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/GetGlsMachineStatusResponse")]
	string GetGlsMachineStatus();

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/GetGlsMachineStatus", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/LocatorService/GetGlsMachineStatusResponse")]
	IAsyncResult BeginGetGlsMachineStatus(AsyncCallback callback, object asyncState);

	string EndGetGlsMachineStatus(IAsyncResult result);
}
