using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.Online.Provisioning.CompanyManagement;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[DebuggerStepThrough]
public class CompanyManagerFederatedServiceTestClient : ClientBase<ICompanyManagerFederatedServiceTest>, ICompanyManagerFederatedServiceTest
{
	public CompanyManagerFederatedServiceTestClient()
	{
	}

	public CompanyManagerFederatedServiceTestClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public CompanyManagerFederatedServiceTestClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public CompanyManagerFederatedServiceTestClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public CompanyManagerFederatedServiceTestClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public void FederatedServiceAddAuthorizedServiceInstanceToCompany(Guid contextId, string serviceInstance)
	{
		base.Channel.FederatedServiceAddAuthorizedServiceInstanceToCompany(contextId, serviceInstance);
	}

	public Task FederatedServiceAddAuthorizedServiceInstanceToCompanyAsync(Guid contextId, string serviceInstance)
	{
		return base.Channel.FederatedServiceAddAuthorizedServiceInstanceToCompanyAsync(contextId, serviceInstance);
	}

	public void FederatedServiceCreateUpdateDeleteSubscription(Subscription subscription)
	{
		base.Channel.FederatedServiceCreateUpdateDeleteSubscription(subscription);
	}

	public Task FederatedServiceCreateUpdateDeleteSubscriptionAsync(Subscription subscription)
	{
		return base.Channel.FederatedServiceCreateUpdateDeleteSubscriptionAsync(subscription);
	}
}
