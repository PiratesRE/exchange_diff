using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal interface IGlobalLocatorServiceWriter
	{
		void SaveTenant(Guid tenantId, KeyValuePair<TenantProperty, PropertyValue>[] properties);

		void SaveDomain(SmtpDomain domain, string domainKey, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties);

		void SaveDomain(SmtpDomain domain, bool isInitialDomain, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties);

		void DeleteTenant(Guid tenantId, Namespace ns);

		void DeleteDomain(SmtpDomain domain, Guid tenantId, Namespace ns);

		IAsyncResult BeginSaveTenant(Guid tenantId, KeyValuePair<TenantProperty, PropertyValue>[] properties, AsyncCallback callback, object asyncState);

		IAsyncResult BeginSaveDomain(SmtpDomain domain, string domainKey, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties, AsyncCallback callback, object asyncState);

		IAsyncResult BeginDeleteTenant(Guid tenantId, Namespace ns, AsyncCallback callback, object asyncState);

		IAsyncResult BeginDeleteDomain(SmtpDomain domain, Guid tenantId, Namespace ns, AsyncCallback callback, object asyncState);

		void EndSaveTenant(IAsyncResult asyncResult);

		void EndSaveDomain(IAsyncResult asyncResult);

		void EndDeleteTenant(IAsyncResult asyncResult);

		void EndDeleteDomain(IAsyncResult asyncResult);
	}
}
