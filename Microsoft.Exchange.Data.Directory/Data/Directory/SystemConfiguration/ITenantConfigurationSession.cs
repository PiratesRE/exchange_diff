using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal interface ITenantConfigurationSession : IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		AcceptedDomain[] FindAllAcceptedDomainsInOrg(ADObjectId organizationCU);

		ExchangeConfigurationUnit[] FindAllOpenConfigurationUnits(bool excludeFull);

		ExchangeConfigurationUnit[] FindSharedConfiguration(SharedConfigurationInfo sharedConfigInfo, bool enabledSharedOrgOnly);

		ExchangeConfigurationUnit[] FindSharedConfigurationByOrganizationId(OrganizationId tinyTenantId);

		ADRawEntry[] FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties);

		ExchangeConfigurationUnit GetExchangeConfigurationUnitByExternalId(string externalDirectoryOrganizationId);

		ExchangeConfigurationUnit GetExchangeConfigurationUnitByExternalId(Guid externalDirectoryOrganizationId);

		ExchangeConfigurationUnit GetExchangeConfigurationUnitByName(string organizationName);

		ADObjectId GetExchangeConfigurationUnitIdByName(string organizationName);

		ExchangeConfigurationUnit GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organizationName);

		ExchangeConfigurationUnit GetExchangeConfigurationUnitByUserNetID(string userNetID);

		OrganizationId GetOrganizationIdFromOrgNameOrAcceptedDomain(string domainName);

		OrganizationId GetOrganizationIdFromExternalDirectoryOrgId(Guid externalDirectoryOrgId);

		MsoTenantCookieContainer GetMsoTenantCookieContainer(Guid contextId);

		Result<ADRawEntry>[] ReadMultipleOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties);

		T GetDefaultFilteringConfiguration<T>() where T : ADConfigurationObject, new();

		bool IsTenantLockedOut();
	}
}
