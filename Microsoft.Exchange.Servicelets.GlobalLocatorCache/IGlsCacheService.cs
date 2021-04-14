using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService", Name = "LocatorService")]
	public interface IGlsCacheService
	{
		[FaultContract(typeof(LocatorServiceFault))]
		[OperationContract]
		FindTenantResponse FindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest);

		[OperationContract]
		[FaultContract(typeof(LocatorServiceFault))]
		FindDomainResponse FindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest);

		[OperationContract]
		[FaultContract(typeof(LocatorServiceFault))]
		FindDomainsResponse FindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest);
	}
}
