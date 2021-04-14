using System;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal interface IGlobalLocatorServiceReader
	{
		bool TenantExists(Guid tenantId, Namespace[] ns);

		bool DomainExists(SmtpDomain domain, Namespace[] ns);

		FindTenantResult FindTenant(Guid tenantId, TenantProperty[] tenantProperties);

		FindDomainResult FindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties);

		FindDomainsResult FindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties);

		IAsyncResult BeginTenantExists(Guid tenantId, Namespace[] ns, AsyncCallback callback, object asyncState);

		IAsyncResult BeginDomainExists(SmtpDomain domain, Namespace[] ns, AsyncCallback callback, object asyncState);

		IAsyncResult BeginFindTenant(Guid tenantId, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState);

		IAsyncResult BeginFindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState);

		IAsyncResult BeginFindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState);

		bool EndTenantExists(IAsyncResult asyncResult);

		bool EndDomainExists(IAsyncResult asyncResult);

		FindTenantResult EndFindTenant(IAsyncResult asyncResult);

		FindDomainResult EndFindDomain(IAsyncResult asyncResult);

		FindDomainsResult EndFindDomains(IAsyncResult asyncResult);
	}
}
