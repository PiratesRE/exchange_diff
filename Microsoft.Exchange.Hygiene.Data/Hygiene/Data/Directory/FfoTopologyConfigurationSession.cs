using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class FfoTopologyConfigurationSession : FfoConfigurationSession, ITopologyConfigurationSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		public FfoTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, true, consistencyMode, null, sessionSettings)
		{
		}

		public FfoTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, null, sessionSettings)
		{
		}

		public FfoTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
		}

		public FfoTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.ConfigScope = configScope;
		}

		bool ITopologyConfigurationSession.TryGetDefaultAdQueryPolicy(out ADQueryPolicy queryPolicy)
		{
			throw new NotImplementedException();
		}

		ADCrossRef[] ITopologyConfigurationSession.FindADCrossRefByDomainId(ADObjectId domainNc)
		{
			throw new NotImplementedException();
		}

		ADCrossRef[] ITopologyConfigurationSession.FindADCrossRefByNetBiosName(string domain)
		{
			throw new NotImplementedException();
		}

		AccountPartition[] ITopologyConfigurationSession.FindAllAccountPartitions()
		{
			throw new NotImplementedException();
		}

		ADSite[] ITopologyConfigurationSession.FindAllADSites()
		{
			throw new NotImplementedException();
		}

		IList<PublicFolderDatabase> ITopologyConfigurationSession.FindAllPublicFolderDatabaseOfCurrentVersion()
		{
			throw new NotImplementedException();
		}

		ADPagedReader<Server> ITopologyConfigurationSession.FindAllServersWithExactVersionNumber(int versionNumber)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<Server> ITopologyConfigurationSession.FindAllServersWithVersionNumber(int versionNumber)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<MiniServer> ITopologyConfigurationSession.FindAllServersWithExactVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<MiniServer> ITopologyConfigurationSession.FindAllServersWithVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		CmdletExtensionAgent[] ITopologyConfigurationSession.FindCmdletExtensionAgents(bool enabledOnly, bool sortByPriority)
		{
			throw new NotImplementedException();
		}

		ADComputer ITopologyConfigurationSession.FindComputerByHostName(string hostName)
		{
			throw new NotImplementedException();
		}

		ADComputer ITopologyConfigurationSession.FindComputerByHostName(ADObjectId domainId, string hostName)
		{
			throw new NotImplementedException();
		}

		ADComputer ITopologyConfigurationSession.FindComputerBySid(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		TDatabase ITopologyConfigurationSession.FindDatabaseByGuid<TDatabase>(Guid dbGuid)
		{
			throw new NotImplementedException();
		}

		ADServer ITopologyConfigurationSession.FindDCByFqdn(string dnsHostName)
		{
			throw new NotImplementedException();
		}

		ADServer ITopologyConfigurationSession.FindDCByInvocationId(Guid invocationId)
		{
			throw new NotImplementedException();
		}

		UMDialPlan[] ITopologyConfigurationSession.FindDialPlansForServer(Server server)
		{
			throw new NotImplementedException();
		}

		ELCFolder ITopologyConfigurationSession.FindElcFolderByName(string name)
		{
			throw new NotImplementedException();
		}

		ADComputer ITopologyConfigurationSession.FindLocalComputer()
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.FindLocalServer()
		{
			throw new NotImplementedException();
		}

		MailboxDatabase ITopologyConfigurationSession.FindMailboxDatabaseByNameAndServer(string databaseName, Server server)
		{
			throw new NotImplementedException();
		}

		MesoContainer ITopologyConfigurationSession.FindMesoContainer(ADDomain dom)
		{
			throw new NotImplementedException();
		}

		MiniClientAccessServerOrArray[] ITopologyConfigurationSession.FindMiniClientAccessServerOrArray(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniClientAccessServerOrArray ITopologyConfigurationSession.FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniServer[] ITopologyConfigurationSession.FindMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniServer ITopologyConfigurationSession.FindMiniServerByFqdn(string serverFqdn)
		{
			throw new NotImplementedException();
		}

		MiniServer ITopologyConfigurationSession.FindMiniServerByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniServer ITopologyConfigurationSession.FindMiniServerByName(string serverName, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADOabVirtualDirectory[] ITopologyConfigurationSession.FindOABVirtualDirectoriesForLocalServer()
		{
			throw new NotImplementedException();
		}

		ADOwaVirtualDirectory[] ITopologyConfigurationSession.FindOWAVirtualDirectoriesForLocalServer()
		{
			throw new NotImplementedException();
		}

		ADSnackyServiceVirtualDirectory[] ITopologyConfigurationSession.FindSnackyServiceVirtualDirectoriesForLocalServer()
		{
			throw new NotImplementedException();
		}

		ADO365SuiteServiceVirtualDirectory[] ITopologyConfigurationSession.FindO365SuiteServiceVirtualDirectoriesForLocalServer()
		{
			throw new NotImplementedException();
		}

		public MiniVirtualDirectory[] FindMiniVirtualDirectories(ADObjectId serverId)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<MiniServer> ITopologyConfigurationSession.FindPagedMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		public TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.FindServerByFqdn(string serverFqdn)
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.FindServerByLegacyDN(string legacyExchangeDN)
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.FindServerByName(string serverName)
		{
			throw new NotImplementedException();
		}

		ReadOnlyCollection<ADServer> ITopologyConfigurationSession.FindServerWithNtdsdsa(string domainDN, bool gcOnly, bool includingRodc)
		{
			throw new NotImplementedException();
		}

		TResult ITopologyConfigurationSession.FindUnique<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter)
		{
			throw new NotImplementedException();
		}

		TPolicy[] ITopologyConfigurationSession.FindWorkloadManagementChildPolicies<TPolicy>(ADObjectId wlmPolicy)
		{
			throw new NotImplementedException();
		}

		AdministrativeGroup ITopologyConfigurationSession.GetAdministrativeGroup()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetAdministrativeGroupId()
		{
			throw new NotImplementedException();
		}

		ADPagedReader<ExtendedRight> ITopologyConfigurationSession.GetAllExtendedRights()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetAutoDiscoverGlobalContainerId()
		{
			throw new NotImplementedException();
		}

		string[] ITopologyConfigurationSession.GetAutodiscoverTrustedHosters()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetClientAccessContainerId()
		{
			throw new NotImplementedException();
		}

		DatabaseAvailabilityGroupContainer ITopologyConfigurationSession.GetDatabaseAvailabilityGroupContainer()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetDatabaseAvailabilityGroupContainerId()
		{
			throw new NotImplementedException();
		}

		DatabasesContainer ITopologyConfigurationSession.GetDatabasesContainer()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetDatabasesContainerId()
		{
			throw new NotImplementedException();
		}

		ServiceEndpointContainer ITopologyConfigurationSession.GetEndpointContainer()
		{
			throw new NotImplementedException();
		}

		ThrottlingPolicy ITopologyConfigurationSession.GetGlobalThrottlingPolicy()
		{
			throw new NotImplementedException();
		}

		ThrottlingPolicy ITopologyConfigurationSession.GetGlobalThrottlingPolicy(bool throwError)
		{
			throw new NotImplementedException();
		}

		Guid ITopologyConfigurationSession.GetInvocationIdByDC(ADServer dc)
		{
			throw new NotImplementedException();
		}

		Guid ITopologyConfigurationSession.GetInvocationIdByFqdn(string serverFqdn)
		{
			throw new NotImplementedException();
		}

		ADSite ITopologyConfigurationSession.GetLocalSite()
		{
			throw new NotImplementedException();
		}

		MsoMainStreamCookieContainer ITopologyConfigurationSession.GetMsoMainStreamCookieContainer(string serviceInstanceName)
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.GetParentServer(ADObjectId entryId, ADObjectId originalId)
		{
			throw new NotImplementedException();
		}

		ProvisioningReconciliationConfig ITopologyConfigurationSession.GetProvisioningReconciliationConfig()
		{
			throw new NotImplementedException();
		}

		string ITopologyConfigurationSession.GetRootDomainNamingContextFromCurrentReadConnection()
		{
			throw new NotImplementedException();
		}

		RootDse ITopologyConfigurationSession.GetRootDse()
		{
			throw new NotImplementedException();
		}

		RoutingGroup ITopologyConfigurationSession.GetRoutingGroup()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetRoutingGroupId()
		{
			throw new NotImplementedException();
		}

		string ITopologyConfigurationSession.GetSchemaMasterDC()
		{
			throw new NotImplementedException();
		}

		ServicesContainer ITopologyConfigurationSession.GetServicesContainer()
		{
			throw new NotImplementedException();
		}

		SitesContainer ITopologyConfigurationSession.GetSitesContainer()
		{
			throw new NotImplementedException();
		}

		StampGroupContainer ITopologyConfigurationSession.GetStampGroupContainer()
		{
			throw new NotImplementedException();
		}

		ADObjectId ITopologyConfigurationSession.GetStampGroupContainerId()
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.HasAnyServer()
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.IsInE12InteropMode()
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.IsInPreE12InteropMode()
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.IsInPreE14InteropMode()
		{
			throw new NotImplementedException();
		}

		Server ITopologyConfigurationSession.ReadLocalServer()
		{
			throw new NotImplementedException();
		}

		MiniClientAccessServerOrArray ITopologyConfigurationSession.ReadMiniClientAccessServerOrArray(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniServer ITopologyConfigurationSession.ReadMiniServer(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		Result<TResult>[] ITopologyConfigurationSession.ReadMultipleLegacyObjects<TResult>(string[] objectNames)
		{
			throw new NotImplementedException();
		}

		Result<Server>[] ITopologyConfigurationSession.ReadMultipleServers(string[] serverNames)
		{
			throw new NotImplementedException();
		}

		ManagementScope ITopologyConfigurationSession.ReadRootOrgManagementScopeByName(string scopeName)
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniServer miniServer)
		{
			throw new NotImplementedException();
		}

		bool ITopologyConfigurationSession.TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniClientAccessServerOrArray miniClientAccessServerOrArray)
		{
			throw new NotImplementedException();
		}

		void ITopologyConfigurationSession.UpdateGwartLastModified()
		{
			throw new NotImplementedException();
		}
	}
}
