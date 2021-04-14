using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[DebuggerStepThrough]
public class LocatorServiceClient : ClientBase<LocatorService>, LocatorService
{
	public LocatorServiceClient()
	{
	}

	public LocatorServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public LocatorServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public LocatorServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public LocatorServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public FindTenantResponse FindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest)
	{
		return base.Channel.FindTenant(identity, findTenantRequest);
	}

	public IAsyncResult BeginFindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginFindTenant(identity, findTenantRequest, callback, asyncState);
	}

	public FindTenantResponse EndFindTenant(IAsyncResult result)
	{
		return base.Channel.EndFindTenant(result);
	}

	public FindDomainResponse FindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest)
	{
		return base.Channel.FindDomain(identity, findDomainRequest);
	}

	public IAsyncResult BeginFindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginFindDomain(identity, findDomainRequest, callback, asyncState);
	}

	public FindDomainResponse EndFindDomain(IAsyncResult result)
	{
		return base.Channel.EndFindDomain(result);
	}

	public SaveTenantResponse SaveTenant(RequestIdentity identity, SaveTenantRequest saveTenantRequest)
	{
		return base.Channel.SaveTenant(identity, saveTenantRequest);
	}

	public IAsyncResult BeginSaveTenant(RequestIdentity identity, SaveTenantRequest saveTenantRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginSaveTenant(identity, saveTenantRequest, callback, asyncState);
	}

	public SaveTenantResponse EndSaveTenant(IAsyncResult result)
	{
		return base.Channel.EndSaveTenant(result);
	}

	public SaveDomainResponse SaveDomain(RequestIdentity identity, SaveDomainRequest saveDomainRequest)
	{
		return base.Channel.SaveDomain(identity, saveDomainRequest);
	}

	public IAsyncResult BeginSaveDomain(RequestIdentity identity, SaveDomainRequest saveDomainRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginSaveDomain(identity, saveDomainRequest, callback, asyncState);
	}

	public SaveDomainResponse EndSaveDomain(IAsyncResult result)
	{
		return base.Channel.EndSaveDomain(result);
	}

	public DeleteTenantResponse DeleteTenant(RequestIdentity identity, DeleteTenantRequest deleteTenantRequest)
	{
		return base.Channel.DeleteTenant(identity, deleteTenantRequest);
	}

	public IAsyncResult BeginDeleteTenant(RequestIdentity identity, DeleteTenantRequest deleteTenantRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDeleteTenant(identity, deleteTenantRequest, callback, asyncState);
	}

	public DeleteTenantResponse EndDeleteTenant(IAsyncResult result)
	{
		return base.Channel.EndDeleteTenant(result);
	}

	public DeleteDomainResponse DeleteDomain(RequestIdentity identity, DeleteDomainRequest deleteDomainRequest)
	{
		return base.Channel.DeleteDomain(identity, deleteDomainRequest);
	}

	public IAsyncResult BeginDeleteDomain(RequestIdentity identity, DeleteDomainRequest deleteDomainRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDeleteDomain(identity, deleteDomainRequest, callback, asyncState);
	}

	public DeleteDomainResponse EndDeleteDomain(IAsyncResult result)
	{
		return base.Channel.EndDeleteDomain(result);
	}

	public FindDomainsResponse FindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest)
	{
		return base.Channel.FindDomains(identity, findDomainsRequest);
	}

	public IAsyncResult BeginFindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginFindDomains(identity, findDomainsRequest, callback, asyncState);
	}

	public FindDomainsResponse EndFindDomains(IAsyncResult result)
	{
		return base.Channel.EndFindDomains(result);
	}

	public SaveUserResponse SaveUser(RequestIdentity identity, SaveUserRequest saveUserRequest)
	{
		return base.Channel.SaveUser(identity, saveUserRequest);
	}

	public IAsyncResult BeginSaveUser(RequestIdentity identity, SaveUserRequest saveUserRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginSaveUser(identity, saveUserRequest, callback, asyncState);
	}

	public SaveUserResponse EndSaveUser(IAsyncResult result)
	{
		return base.Channel.EndSaveUser(result);
	}

	public FindUserResponse FindUser(RequestIdentity identity, FindUserRequest findUserRequest)
	{
		return base.Channel.FindUser(identity, findUserRequest);
	}

	public IAsyncResult BeginFindUser(RequestIdentity identity, FindUserRequest findUserRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginFindUser(identity, findUserRequest, callback, asyncState);
	}

	public FindUserResponse EndFindUser(IAsyncResult result)
	{
		return base.Channel.EndFindUser(result);
	}

	public DeleteUserResponse DeleteUser(RequestIdentity identity, DeleteUserRequest deleteUserRequest)
	{
		return base.Channel.DeleteUser(identity, deleteUserRequest);
	}

	public IAsyncResult BeginDeleteUser(RequestIdentity identity, DeleteUserRequest deleteUserRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDeleteUser(identity, deleteUserRequest, callback, asyncState);
	}

	public DeleteUserResponse EndDeleteUser(IAsyncResult result)
	{
		return base.Channel.EndDeleteUser(result);
	}

	public FindTenantResponse[] FindTenantFromPrimary(RequestIdentity identity, FindTenantRequest findTenantRequest, int? copiesToRead)
	{
		return base.Channel.FindTenantFromPrimary(identity, findTenantRequest, copiesToRead);
	}

	public IAsyncResult BeginFindTenantFromPrimary(RequestIdentity identity, FindTenantRequest findTenantRequest, int? copiesToRead, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginFindTenantFromPrimary(identity, findTenantRequest, copiesToRead, callback, asyncState);
	}

	public FindTenantResponse[] EndFindTenantFromPrimary(IAsyncResult result)
	{
		return base.Channel.EndFindTenantFromPrimary(result);
	}

	public string GetGlsMachineStatus()
	{
		return base.Channel.GetGlsMachineStatus();
	}

	public IAsyncResult BeginGetGlsMachineStatus(AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetGlsMachineStatus(callback, asyncState);
	}

	public string EndGetGlsMachineStatus(IAsyncResult result)
	{
		return base.Channel.EndGetGlsMachineStatus(result);
	}
}
