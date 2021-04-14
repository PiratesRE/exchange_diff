using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class LocatorServiceDiagnosticsClient : ClientBase<ILocatorServiceDiagnostics>, ILocatorServiceDiagnostics
{
	public LocatorServiceDiagnosticsClient()
	{
	}

	public LocatorServiceDiagnosticsClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public LocatorServiceDiagnosticsClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public LocatorServiceDiagnosticsClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public LocatorServiceDiagnosticsClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public DIFindTenantResponse DIFindTenant(RequestIdentity identity, DIFindTenantRequest dIFindTenantRequest)
	{
		return base.Channel.DIFindTenant(identity, dIFindTenantRequest);
	}

	public IAsyncResult BeginDIFindTenant(RequestIdentity identity, DIFindTenantRequest dIFindTenantRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDIFindTenant(identity, dIFindTenantRequest, callback, asyncState);
	}

	public DIFindTenantResponse EndDIFindTenant(IAsyncResult result)
	{
		return base.Channel.EndDIFindTenant(result);
	}

	public DIFindDomainsResponse DIFindDomains(RequestIdentity identity, DIFindDomainsRequest dIFindDomainsRequest)
	{
		return base.Channel.DIFindDomains(identity, dIFindDomainsRequest);
	}

	public IAsyncResult BeginDIFindDomains(RequestIdentity identity, DIFindDomainsRequest dIFindDomainsRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDIFindDomains(identity, dIFindDomainsRequest, callback, asyncState);
	}

	public DIFindDomainsResponse EndDIFindDomains(IAsyncResult result)
	{
		return base.Channel.EndDIFindDomains(result);
	}
}
