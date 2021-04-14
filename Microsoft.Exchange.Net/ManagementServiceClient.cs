using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[DebuggerStepThrough]
internal class ManagementServiceClient : ClientBase<IManagementService>, IManagementService
{
	public ManagementServiceClient()
	{
	}

	public ManagementServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public ManagementServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ManagementServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ManagementServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public bool PingManagementService()
	{
		return base.Channel.PingManagementService();
	}

	public Company GetCompanyByName(string name)
	{
		return base.Channel.GetCompanyByName(name);
	}

	public Company GetCompanyById(int companyId)
	{
		return base.Channel.GetCompanyById(companyId);
	}

	public Company GetCompanyByGuid(Guid companyGuid)
	{
		return base.Channel.GetCompanyByGuid(companyGuid);
	}

	public Domain GetDomain(string domainName)
	{
		return base.Channel.GetDomain(domainName);
	}

	public Company[] GetAllCompanies(int parentCompanyId, int pageSize, int pageIndex)
	{
		return base.Channel.GetAllCompanies(parentCompanyId, pageSize, pageIndex);
	}

	public Domain[] GetAllDomains(int companyId)
	{
		return base.Channel.GetAllDomains(companyId);
	}

	public Domain[] GetAllDomainsUnderReseller(int resellerCompanyId, int pageSize, int pageIndex)
	{
		return base.Channel.GetAllDomainsUnderReseller(resellerCompanyId, pageSize, pageIndex);
	}

	public string[] GetDataCenterIPs()
	{
		return base.Channel.GetDataCenterIPs();
	}

	public SmtpProfile[] GetCompanyInboundIPList(int companyId)
	{
		return base.Channel.GetCompanyInboundIPList(companyId);
	}

	public SmtpProfile GetDomainInboundIPList(string domainName)
	{
		return base.Channel.GetDomainInboundIPList(domainName);
	}

	public string[] GetCompanyIPList(int companyId, IPAddressType ipType)
	{
		return base.Channel.GetCompanyIPList(companyId, ipType);
	}

	public string[] GetDomainIPList(string domainName, IPAddressType ipType)
	{
		return base.Channel.GetDomainIPList(domainName, ipType);
	}

	public CompanyResponseInfoSet EnableCompanies(int[] companyIdList)
	{
		return base.Channel.EnableCompanies(companyIdList);
	}

	public CompanyResponseInfoSet DisableCompanies(int[] companyIdList)
	{
		return base.Channel.DisableCompanies(companyIdList);
	}

	public DomainResponseInfoSet EnableDomains(Domain[] domainList)
	{
		return base.Channel.EnableDomains(domainList);
	}

	public DomainResponseInfoSet DisableDomains(Domain[] domainList)
	{
		return base.Channel.DisableDomains(domainList);
	}

	public CompanyResponseInfoSet UpdateCompanySettings(CompanyConfigurationSettings[] settings)
	{
		return base.Channel.UpdateCompanySettings(settings);
	}

	public DomainResponseInfoSet UpdateDomainSettings(DomainConfigurationSettings[] settings)
	{
		return base.Channel.UpdateDomainSettings(settings);
	}

	public DomainResponseInfoSet SetDefaultOutboundDomains(Domain[] defaultDomains)
	{
		return base.Channel.SetDefaultOutboundDomains(defaultDomains);
	}

	public DomainResponseInfoSet UpdateDomainSettingsByGuids(DomainConfigurationSettings[] domainConfigs)
	{
		return base.Channel.UpdateDomainSettingsByGuids(domainConfigs);
	}
}
