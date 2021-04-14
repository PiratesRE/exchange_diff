using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal interface ITopologyConfigurationSession : IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		ADCrossRef[] FindADCrossRefByDomainId(ADObjectId domainNc);

		ADCrossRef[] FindADCrossRefByNetBiosName(string domain);

		AccountPartition[] FindAllAccountPartitions();

		ADSite[] FindAllADSites();

		IList<PublicFolderDatabase> FindAllPublicFolderDatabaseOfCurrentVersion();

		ADPagedReader<Server> FindAllServersWithExactVersionNumber(int versionNumber);

		ADPagedReader<Server> FindAllServersWithVersionNumber(int versionNumber);

		ADPagedReader<MiniServer> FindAllServersWithExactVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties);

		ADPagedReader<MiniServer> FindAllServersWithVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties);

		CmdletExtensionAgent[] FindCmdletExtensionAgents(bool enabledOnly, bool sortByPriority);

		ADComputer FindComputerByHostName(string hostName);

		ADComputer FindComputerByHostName(ADObjectId domainId, string hostName);

		ADComputer FindComputerBySid(SecurityIdentifier sid);

		TDatabase FindDatabaseByGuid<TDatabase>(Guid dbGuid) where TDatabase : Database, new();

		ADServer FindDCByFqdn(string dnsHostName);

		ADServer FindDCByInvocationId(Guid invocationId);

		UMDialPlan[] FindDialPlansForServer(Server server);

		ELCFolder FindElcFolderByName(string name);

		ADComputer FindLocalComputer();

		Server FindLocalServer();

		MailboxDatabase FindMailboxDatabaseByNameAndServer(string databaseName, Server server);

		MesoContainer FindMesoContainer(ADDomain dom);

		MiniClientAccessServerOrArray[] FindMiniClientAccessServerOrArray(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties);

		MiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties);

		MiniServer[] FindMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties);

		TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new();

		MiniServer FindMiniServerByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties);

		MiniServer FindMiniServerByName(string serverName, IEnumerable<PropertyDefinition> properties);

		ADOabVirtualDirectory[] FindOABVirtualDirectoriesForLocalServer();

		ADOwaVirtualDirectory[] FindOWAVirtualDirectoriesForLocalServer();

		ADO365SuiteServiceVirtualDirectory[] FindO365SuiteServiceVirtualDirectoriesForLocalServer();

		ADSnackyServiceVirtualDirectory[] FindSnackyServiceVirtualDirectoriesForLocalServer();

		MiniVirtualDirectory[] FindMiniVirtualDirectories(ADObjectId serverId);

		ADPagedReader<MiniServer> FindPagedMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties);

		MiniServer FindMiniServerByFqdn(string serverFqdn);

		Server FindServerByFqdn(string serverFqdn);

		Server FindServerByLegacyDN(string legacyExchangeDN);

		Server FindServerByName(string serverName);

		ReadOnlyCollection<ADServer> FindServerWithNtdsdsa(string domainDN, bool gcOnly, bool includingRodc);

		TResult FindUnique<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter) where TResult : ADConfigurationObject, new();

		TPolicy[] FindWorkloadManagementChildPolicies<TPolicy>(ADObjectId wlmPolicy) where TPolicy : ADConfigurationObject, new();

		AdministrativeGroup GetAdministrativeGroup();

		ADObjectId GetAdministrativeGroupId();

		ADPagedReader<ExtendedRight> GetAllExtendedRights();

		ADObjectId GetAutoDiscoverGlobalContainerId();

		string[] GetAutodiscoverTrustedHosters();

		ADObjectId GetClientAccessContainerId();

		DatabaseAvailabilityGroupContainer GetDatabaseAvailabilityGroupContainer();

		ADObjectId GetDatabaseAvailabilityGroupContainerId();

		DatabasesContainer GetDatabasesContainer();

		ADObjectId GetDatabasesContainerId();

		ServiceEndpointContainer GetEndpointContainer();

		ThrottlingPolicy GetGlobalThrottlingPolicy();

		ThrottlingPolicy GetGlobalThrottlingPolicy(bool throwError);

		Guid GetInvocationIdByDC(ADServer dc);

		Guid GetInvocationIdByFqdn(string serverFqdn);

		ADSite GetLocalSite();

		MsoMainStreamCookieContainer GetMsoMainStreamCookieContainer(string serviceInstanceName);

		Server GetParentServer(ADObjectId entryId, ADObjectId originalId);

		ProvisioningReconciliationConfig GetProvisioningReconciliationConfig();

		string GetRootDomainNamingContextFromCurrentReadConnection();

		RootDse GetRootDse();

		RoutingGroup GetRoutingGroup();

		ADObjectId GetRoutingGroupId();

		string GetSchemaMasterDC();

		ServicesContainer GetServicesContainer();

		SitesContainer GetSitesContainer();

		StampGroupContainer GetStampGroupContainer();

		ADObjectId GetStampGroupContainerId();

		bool HasAnyServer();

		bool IsInE12InteropMode();

		bool IsInPreE12InteropMode();

		bool IsInPreE14InteropMode();

		Server ReadLocalServer();

		MiniClientAccessServerOrArray ReadMiniClientAccessServerOrArray(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		MiniServer ReadMiniServer(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		Result<TResult>[] ReadMultipleLegacyObjects<TResult>(string[] objectNames) where TResult : ADLegacyVersionableObject, new();

		Result<Server>[] ReadMultipleServers(string[] serverNames);

		ManagementScope ReadRootOrgManagementScopeByName(string scopeName);

		bool TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniServer miniServer);

		bool TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniClientAccessServerOrArray miniClientAccessServerOrArray);

		void UpdateGwartLastModified();

		bool TryGetDefaultAdQueryPolicy(out ADQueryPolicy queryPolicy);
	}
}
