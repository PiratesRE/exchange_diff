using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[DebuggerStepThrough]
internal class ProvisioningServiceClient : ClientBase<IProvisioningService>, IProvisioningService
{
	public ProvisioningServiceClient()
	{
	}

	public ProvisioningServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public ProvisioningServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ProvisioningServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ProvisioningServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public bool PingProvisioningService()
	{
		return base.Channel.PingProvisioningService();
	}

	public CompanyResponseInfoSet CreateCompanies(Company[] companyList)
	{
		return base.Channel.CreateCompanies(companyList);
	}

	public CompanyResponseInfoSet DeleteCompanies(int[] companyIdList)
	{
		return base.Channel.DeleteCompanies(companyIdList);
	}

	public DomainResponseInfoSet AddDomains(Domain[] domainList)
	{
		return base.Channel.AddDomains(domainList);
	}

	public DomainResponseInfoSet DeleteDomains(Domain[] domainList)
	{
		return base.Channel.DeleteDomains(domainList);
	}
}
