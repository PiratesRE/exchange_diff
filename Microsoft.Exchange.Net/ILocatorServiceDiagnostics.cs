using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

[ServiceContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService", ConfigurationName = "ILocatorServiceDiagnostics")]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public interface ILocatorServiceDiagnostics
{
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindTenantResponse")]
	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindTenantLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	DIFindTenantResponse DIFindTenant(RequestIdentity identity, DIFindTenantRequest dIFindTenantRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindTenant", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindTenantResponse")]
	IAsyncResult BeginDIFindTenant(RequestIdentity identity, DIFindTenantRequest dIFindTenantRequest, AsyncCallback callback, object asyncState);

	DIFindTenantResponse EndDIFindTenant(IAsyncResult result);

	[FaultContract(typeof(LocatorServiceFault), Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindDomainsLocatorServiceFaultFault", Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[OperationContract(Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindDomains", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindDomainsResponse")]
	DIFindDomainsResponse DIFindDomains(RequestIdentity identity, DIFindDomainsRequest dIFindDomainsRequest);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindDomains", ReplyAction = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/ILocatorServiceDiagnostics/DIFindDomainsResponse")]
	IAsyncResult BeginDIFindDomains(RequestIdentity identity, DIFindDomainsRequest dIFindDomainsRequest, AsyncCallback callback, object asyncState);

	DIFindDomainsResponse EndDIFindDomains(IAsyncResult result);
}
