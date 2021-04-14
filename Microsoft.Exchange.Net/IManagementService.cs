using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[ServiceContract(ConfigurationName = "IManagementService")]
internal interface IManagementService
{
	[OperationContract(Action = "http://tempuri.org/IManagementService/PingManagementService", ReplyAction = "http://tempuri.org/IManagementService/PingManagementServiceResponse")]
	bool PingManagementService();

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetCompanyByNameServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetCompanyByName", ReplyAction = "http://tempuri.org/IManagementService/GetCompanyByNameResponse")]
	Company GetCompanyByName(string name);

	[OperationContract(Action = "http://tempuri.org/IManagementService/GetCompanyById", ReplyAction = "http://tempuri.org/IManagementService/GetCompanyByIdResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetCompanyByIdServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	Company GetCompanyById(int companyId);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetCompanyByGuidServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetCompanyByGuid", ReplyAction = "http://tempuri.org/IManagementService/GetCompanyByGuidResponse")]
	Company GetCompanyByGuid(Guid companyGuid);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetDomainServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetDomain", ReplyAction = "http://tempuri.org/IManagementService/GetDomainResponse")]
	Domain GetDomain(string domainName);

	[OperationContract(Action = "http://tempuri.org/IManagementService/GetAllCompanies", ReplyAction = "http://tempuri.org/IManagementService/GetAllCompaniesResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetAllCompaniesServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	Company[] GetAllCompanies(int parentCompanyId, int pageSize, int pageIndex);

	[OperationContract(Action = "http://tempuri.org/IManagementService/GetAllDomains", ReplyAction = "http://tempuri.org/IManagementService/GetAllDomainsResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetAllDomainsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	Domain[] GetAllDomains(int companyId);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetAllDomainsUnderResellerServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetAllDomainsUnderReseller", ReplyAction = "http://tempuri.org/IManagementService/GetAllDomainsUnderResellerResponse")]
	Domain[] GetAllDomainsUnderReseller(int resellerCompanyId, int pageSize, int pageIndex);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetDataCenterIPsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetDataCenterIPs", ReplyAction = "http://tempuri.org/IManagementService/GetDataCenterIPsResponse")]
	string[] GetDataCenterIPs();

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetCompanyInboundIPListServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetCompanyInboundIPList", ReplyAction = "http://tempuri.org/IManagementService/GetCompanyInboundIPListResponse")]
	SmtpProfile[] GetCompanyInboundIPList(int companyId);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetDomainInboundIPListServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetDomainInboundIPList", ReplyAction = "http://tempuri.org/IManagementService/GetDomainInboundIPListResponse")]
	SmtpProfile GetDomainInboundIPList(string domainName);

	[OperationContract(Action = "http://tempuri.org/IManagementService/GetCompanyIPList", ReplyAction = "http://tempuri.org/IManagementService/GetCompanyIPListResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetCompanyIPListServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	string[] GetCompanyIPList(int companyId, IPAddressType ipType);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/GetDomainIPListServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/GetDomainIPList", ReplyAction = "http://tempuri.org/IManagementService/GetDomainIPListResponse")]
	string[] GetDomainIPList(string domainName, IPAddressType ipType);

	[OperationContract(Action = "http://tempuri.org/IManagementService/EnableCompanies", ReplyAction = "http://tempuri.org/IManagementService/EnableCompaniesResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/EnableCompaniesServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	CompanyResponseInfoSet EnableCompanies(int[] companyIdList);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/DisableCompaniesServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/DisableCompanies", ReplyAction = "http://tempuri.org/IManagementService/DisableCompaniesResponse")]
	CompanyResponseInfoSet DisableCompanies(int[] companyIdList);

	[OperationContract(Action = "http://tempuri.org/IManagementService/EnableDomains", ReplyAction = "http://tempuri.org/IManagementService/EnableDomainsResponse")]
	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/EnableDomainsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	DomainResponseInfoSet EnableDomains(Domain[] domainList);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/DisableDomainsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/DisableDomains", ReplyAction = "http://tempuri.org/IManagementService/DisableDomainsResponse")]
	DomainResponseInfoSet DisableDomains(Domain[] domainList);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/UpdateCompanySettingsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/UpdateCompanySettings", ReplyAction = "http://tempuri.org/IManagementService/UpdateCompanySettingsResponse")]
	CompanyResponseInfoSet UpdateCompanySettings(CompanyConfigurationSettings[] settings);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/UpdateDomainSettingsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/UpdateDomainSettings", ReplyAction = "http://tempuri.org/IManagementService/UpdateDomainSettingsResponse")]
	DomainResponseInfoSet UpdateDomainSettings(DomainConfigurationSettings[] settings);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/SetDefaultOutboundDomainsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/SetDefaultOutboundDomains", ReplyAction = "http://tempuri.org/IManagementService/SetDefaultOutboundDomainsResponse")]
	DomainResponseInfoSet SetDefaultOutboundDomains(Domain[] defaultDomains);

	[FaultContract(typeof(ServiceFault), Action = "http://tempuri.org/IManagementService/UpdateDomainSettingsByGuidsServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[OperationContract(Action = "http://tempuri.org/IManagementService/UpdateDomainSettingsByGuids", ReplyAction = "http://tempuri.org/IManagementService/UpdateDomainSettingsByGuidsResponse")]
	DomainResponseInfoSet UpdateDomainSettingsByGuids(DomainConfigurationSettings[] domainConfigs);
}
