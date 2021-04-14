using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IGlobalDirectorySession
	{
		string GetRedirectServer(string memberName);

		bool TryGetRedirectServer(string memberName, out string fqdn);

		string GetRedirectServer(Guid orgGuid);

		bool TryGetMSAUserMemberName(string msaUserNetID, out string msaUserMemberName);

		bool TryGetRedirectServer(Guid orgGuid, out string fqdn);

		bool TryGetDomainFlag(string domainFqdn, GlsDomainFlags flag, out bool value);

		void SetDomainFlag(string domainFqdn, GlsDomainFlags flag, bool value);

		bool TryGetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, out bool value);

		void SetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, bool value);

		void AddTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN);

		void AddTenant(Guid externalDirectoryOrganizationId, CustomerType tenantType, string ffoRegion, string ffoVersion);

		void AddMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId);

		void UpdateTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN);

		void UpdateMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId);

		void RemoveTenant(Guid externalDirectoryOrganizationId);

		void RemoveMSAUser(string msaUserNetID);

		bool TryGetTenantType(Guid externalDirectoryOrganizationId, out CustomerType tenantType);

		bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN, out bool dataFromOfflineService);

		bool TryGetTenantForestsByOrgGuid(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN, out bool dataFromOfflineService);

		bool TryGetTenantForestsByMSAUserNetID(string msaUserNetID, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN);

		void SetAccountForest(Guid externalDirectoryOrganizationId, string value, string tenantContainerCN = null);

		void SetResourceForest(Guid externalDirectoryOrganizationId, string value);

		void SetTenantVersion(Guid externalDirectoryOrganizationId, string newTenantVersion);

		bool TryGetTenantDomains(Guid externalDirectoryOrganizationId, out string[] acceptedDomainFqdns);

		void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain);

		void UpdateAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn);

		void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, bool nego2Enabled, bool oauth2ClientProfileEnabled);

		void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, string ffoRegion, string ffoServiceVersion);

		void RemoveAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn);

		void SetDomainVersion(Guid externalDirectoryOrganizationId, string domainFqdn, string newDomainVersion);

		IEnumerable<string> GetDomainNamesProvisionedByEXO(IEnumerable<SmtpDomain> domains);

		IAsyncResult BeginGetFfoTenantAttributionPropertiesByDomain(SmtpDomain domain, object clientAsyncState, AsyncCallback clientCallback);

		bool TryEndGetFfoTenantAttributionPropertiesByDomain(IAsyncResult asyncResult, out string ffoRegion, out string ffoVersion, out Guid externalDirectoryOrganizationId, out string exoNextHop, out CustomerType tenantType, out DomainIPv6State ipv6Enabled, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer);

		IAsyncResult BeginGetFfoTenantAttributionPropertiesByOrgId(Guid externalDirectoryOrganizationId, object clientAsyncState, AsyncCallback clientCallback);

		bool TryEndGetFfoTenantAttributionPropertiesByOrgId(IAsyncResult asyncResult, out string ffoRegion, out string exoNextHop, out CustomerType tenantType, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer);

		bool TryGetFfoTenantProvisioningProperties(Guid externalDirectoryOrganizationId, out string version, out CustomerType tenantType, out string region);

		bool TenantExists(Guid externalDirectoryOrganizationId, Namespace namespaceToCheck);

		bool MSAUserExists(string msaUserNetID);
	}
}
