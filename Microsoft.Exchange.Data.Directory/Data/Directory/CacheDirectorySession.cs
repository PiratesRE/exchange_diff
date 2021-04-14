using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Cache;
using Microsoft.Exchange.Data.Directory.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	internal class CacheDirectorySession : ITenantConfigurationSession, IConfigurationSession, ITenantRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider, ICacheDirectorySession
	{
		public CacheDirectorySession(ADSessionSettings sessionSettings)
		{
			ArgumentValidator.ThrowIfNull("sessionSettings", sessionSettings);
			this.sessionSettings = sessionSettings;
		}

		public ADCacheResultState ResultState { get; private set; }

		public bool IsNewProxyObject { get; private set; }

		public int RetryCount { get; private set; }

		TimeSpan? IDirectorySession.ClientSideSearchTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		ConfigScopes IDirectorySession.ConfigScope
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ConsistencyMode IDirectorySession.ConsistencyMode
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		string IDirectorySession.DomainController
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.EnforceContainerizedScoping
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.EnforceDefaultScope
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		string IDirectorySession.LastUsedDc
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		int IDirectorySession.Lcid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		string IDirectorySession.LinkResolutionServer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.LogSizeLimitExceededEvent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		NetworkCredential IDirectorySession.NetworkCredential
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.ReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ADServerSettings IDirectorySession.ServerSettings
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		TimeSpan? IDirectorySession.ServerTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		ADSessionSettings IDirectorySession.SessionSettings
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.SkipRangedAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string[] ExclusiveLdapAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.UseConfigNC
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool IDirectorySession.UseGlobalCatalog
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IActivityScope ActivityScope
		{
			get
			{
				return this.logContext.ActivityScope;
			}
			set
			{
				this.logContext.ActivityScope = value;
			}
		}

		public string CallerInfo
		{
			get
			{
				return this.logContext.GetCallerInformation();
			}
		}

		void IDirectorySession.AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			throw new NotImplementedException();
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter)
		{
			throw new NotImplementedException();
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IDirectorySession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		TResult[] IDirectorySession.Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IDirectorySession.FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IDirectorySession.FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<TData>[] IDirectorySession.FindByADObjectIds<TData>(ADObjectId[] ids)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IDirectorySession.FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IDirectorySession.FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IDirectorySession.FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IDirectorySession.FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<TResult> IDirectorySession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<TResult> IDirectorySession.FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetConfigurationNamingContext()
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetConfigurationUnitsRoot()
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetDomainNamingContext()
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetHostedOrganizationsRoot()
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetRootDomainNamingContext()
		{
			throw new NotImplementedException();
		}

		ADObjectId IDirectorySession.GetSchemaNamingContext()
		{
			throw new NotImplementedException();
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			throw new NotImplementedException();
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			throw new NotImplementedException();
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			throw new NotImplementedException();
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.IsReadConnectionAvailable()
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.IsRootIdWithinScope<TObject>(ADObjectId rootId)
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.IsTenantIdentity(ADObjectId id)
		{
			throw new NotImplementedException();
		}

		ADRawEntry IDirectorySession.ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			if (properties == null)
			{
				return null;
			}
			PropertyDefinition[] second = new PropertyDefinition[]
			{
				ADObjectSchema.Id
			};
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(entryId), ObjectType.ADRawEntry, properties.Concat(second)));
		}

		RawSecurityDescriptor IDirectorySession.ReadSecurityDescriptor(ADObjectId id)
		{
			throw new NotImplementedException();
		}

		SecurityDescriptor IDirectorySession.ReadSecurityDescriptorBlob(ADObjectId id)
		{
			throw new NotImplementedException();
		}

		string[] IDirectorySession.ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			throw new NotImplementedException();
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId)
		{
			throw new NotImplementedException();
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN)
		{
			throw new NotImplementedException();
		}

		TenantRelocationSyncObject IDirectorySession.RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADOperationResultWithData<TResult>[] IDirectorySession.RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			throw new NotImplementedException();
		}

		bool IDirectorySession.TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.UpdateServerSettings(PooledLdapConnection connection)
		{
			throw new NotImplementedException();
		}

		void IDirectorySession.VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			throw new NotImplementedException();
		}

		TResult[] IDirectorySession.ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance)
		{
			throw new NotImplementedException();
		}

		ADObjectId IConfigurationSession.ConfigurationNamingContext
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ADObjectId IConfigurationSession.DeletedObjectsContainer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ADObjectId IConfigurationSession.SchemaNamingContext
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			throw new NotImplementedException();
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			throw new NotImplementedException();
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			throw new NotImplementedException();
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			throw new NotImplementedException();
		}

		void IConfigurationSession.DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler)
		{
			throw new NotImplementedException();
		}

		AcceptedDomain[] IConfigurationSession.FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<TResult> IConfigurationSession.FindAllPaged<TResult>()
		{
			throw new NotImplementedException();
		}

		ExchangeRoleAssignment[] IConfigurationSession.FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll)
		{
			throw new NotImplementedException();
		}

		T IConfigurationSession.FindMailboxPolicyByName<T>(string name)
		{
			throw new NotImplementedException();
		}

		MicrosoftExchangeRecipient IConfigurationSession.FindMicrosoftExchangeRecipient()
		{
			throw new NotImplementedException();
		}

		OfflineAddressBook[] IConfigurationSession.FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir)
		{
			throw new NotImplementedException();
		}

		ThrottlingPolicy[] IConfigurationSession.FindOrganizationThrottlingPolicies(OrganizationId organizationId)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<TResult> IConfigurationSession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			throw new NotImplementedException();
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode)
		{
			throw new NotImplementedException();
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter)
		{
			throw new NotImplementedException();
		}

		ManagementScope[] IConfigurationSession.FindSimilarManagementScope(ManagementScope managementScope)
		{
			throw new NotImplementedException();
		}

		T IConfigurationSession.FindSingletonConfigurationObject<T>()
		{
			throw new NotImplementedException();
		}

		AcceptedDomain IConfigurationSession.GetAcceptedDomainByDomainName(string domainName)
		{
			return this.InternalGet<AcceptedDomain>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, domainName, KeyType.Name, ObjectType.AcceptedDomain, null));
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllExclusiveScopes()
		{
			throw new NotImplementedException();
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType)
		{
			throw new NotImplementedException();
		}

		AvailabilityAddressSpace IConfigurationSession.GetAvailabilityAddressSpace(string domainName)
		{
			throw new NotImplementedException();
		}

		AvailabilityConfig IConfigurationSession.GetAvailabilityConfig()
		{
			throw new NotImplementedException();
		}

		AcceptedDomain IConfigurationSession.GetDefaultAcceptedDomain()
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationContainer IConfigurationSession.GetExchangeConfigurationContainer()
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationContainerWithAddressLists IConfigurationSession.GetExchangeConfigurationContainerWithAddressLists()
		{
			throw new NotImplementedException();
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId()
		{
			throw new NotImplementedException();
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId(OrganizationId organizationId)
		{
			return this.InternalGet<FederatedOrganizationId>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, organizationId.ConfigurationUnit.DistinguishedName, KeyType.OrgCUDN, ObjectType.FederatedOrganizationId, null));
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationIdByDomainName(string domainName)
		{
			throw new NotImplementedException();
		}

		NspiRpcClientConnection IConfigurationSession.GetNspiRpcClientConnection()
		{
			throw new NotImplementedException();
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId)
		{
			throw new NotImplementedException();
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup)
		{
			throw new NotImplementedException();
		}

		Organization IConfigurationSession.GetOrgContainer()
		{
			if (null == this.sessionSettings.CurrentOrganizationId || OrganizationId.ForestWideOrgId.Equals(this.sessionSettings.CurrentOrganizationId))
			{
				return null;
			}
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(this.sessionSettings.CurrentOrganizationId.ConfigurationUnit), ObjectType.ExchangeConfigurationUnit, null));
		}

		OrganizationRelationship IConfigurationSession.GetOrganizationRelationship(string domainName)
		{
			throw new NotImplementedException();
		}

		ADObjectId IConfigurationSession.GetOrgContainerId()
		{
			throw new NotImplementedException();
		}

		RbacContainer IConfigurationSession.GetRbacContainer()
		{
			throw new NotImplementedException();
		}

		bool IConfigurationSession.ManagementScopeIsInUse(ManagementScope managementScope)
		{
			throw new NotImplementedException();
		}

		public TResult FindByExchangeObjectId<TResult>(Guid exchangeObjectId) where TResult : ADConfigurationObject, new()
		{
			throw new NotImplementedException();
		}

		TResult IConfigurationSession.Read<TResult>(ADObjectId entryId)
		{
			ObjectType objectTypeFor = CacheUtils.GetObjectTypeFor(typeof(TResult), false);
			if (objectTypeFor == ObjectType.Unknown)
			{
				return default(TResult);
			}
			return this.InternalGet<TResult>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(entryId), objectTypeFor, null));
		}

		Result<TResult>[] IConfigurationSession.ReadMultiple<TResult>(ADObjectId[] identities)
		{
			throw new NotImplementedException();
		}

		MultiValuedProperty<ReplicationCursor> IConfigurationSession.ReadReplicationCursors(ADObjectId id)
		{
			throw new NotImplementedException();
		}

		void IConfigurationSession.ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom)
		{
			throw new NotImplementedException();
		}

		void IConfigurationSession.Save(ADConfigurationObject instanceToSave)
		{
			this.InternalSave(instanceToSave, null, null, 2147483646, CacheItemPriority.Default);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.InternalDelete((ADRawEntry)instance);
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			throw new NotImplementedException();
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			throw new NotImplementedException();
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			throw new NotImplementedException();
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			this.InternalSave((ADRawEntry)instance, null, null, 2147483646, CacheItemPriority.Default);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		AcceptedDomain[] ITenantConfigurationSession.FindAllAcceptedDomainsInOrg(ADObjectId organizationCU)
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindAllOpenConfigurationUnits(bool excludeFull)
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfiguration(SharedConfigurationInfo sharedConfigInfo, bool enabledSharedOrgOnly)
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfigurationByOrganizationId(OrganizationId tinyTenantId)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] ITenantConfigurationSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(string externalId)
		{
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, externalId, KeyType.ExternalDirectoryOrganizationId, ObjectType.ExchangeConfigurationUnit, null));
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(Guid externalDirectoryOrganizationId)
		{
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, externalDirectoryOrganizationId.ToString(), KeyType.ExternalDirectoryOrganizationId, ObjectType.ExchangeConfigurationUnit, null));
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByName(string organizationName)
		{
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, organizationName.ToString(), KeyType.Name, ObjectType.ExchangeConfigurationUnit, null));
		}

		ADObjectId ITenantConfigurationSession.GetExchangeConfigurationUnitIdByName(string organizationName)
		{
			throw new NotImplementedException();
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organizationName)
		{
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, organizationName.ToString(), KeyType.Name | KeyType.DomainName, ObjectType.ExchangeConfigurationUnit, null));
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByUserNetID(string userNetID)
		{
			return this.InternalGet<ExchangeConfigurationUnit>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, userNetID, KeyType.NetId, ObjectType.ExchangeConfigurationUnit, null));
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromOrgNameOrAcceptedDomain(string domainName)
		{
			throw new NotImplementedException();
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromExternalDirectoryOrgId(Guid externalDirectoryOrgId)
		{
			throw new NotImplementedException();
		}

		MsoTenantCookieContainer ITenantConfigurationSession.GetMsoTenantCookieContainer(Guid contextId)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] ITenantConfigurationSession.ReadMultipleOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		T ITenantConfigurationSession.GetDefaultFilteringConfiguration<T>()
		{
			throw new NotImplementedException();
		}

		public bool IsTenantLockedOut()
		{
			throw new NotImplementedException();
		}

		ADObjectId IRecipientSession.SearchRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ITableView IRecipientSession.Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		void IRecipientSession.Delete(ADRecipient instanceToDelete)
		{
			this.InternalDelete(instanceToDelete);
		}

		ADRecipient[] IRecipientSession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw new NotImplementedException();
		}

		ADRawEntry IRecipientSession.FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromSid(sId), ObjectType.ADRawEntry, properties));
		}

		ADRawEntry[] IRecipientSession.FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			throw new NotImplementedException();
		}

		Result<ADRecipient>[] IRecipientSession.FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			throw new NotImplementedException();
		}

		ADUser[] IRecipientSession.FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw new NotImplementedException();
		}

		ADUser IRecipientSession.FindADUserByObjectId(ADObjectId adObjectId)
		{
			return this.InternalGet<ADUser>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(adObjectId), ObjectType.Recipient, null));
		}

		ADUser IRecipientSession.FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			throw new NotImplementedException();
		}

		ADObject IRecipientSession.FindByAccountName<T>(string domainName, string accountName)
		{
			throw new NotImplementedException();
		}

		IEnumerable<T> IRecipientSession.FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter)
		{
			throw new NotImplementedException();
		}

		ADRecipient[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindByCertificate(X509Identifier identifier)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IRecipientSession.FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADRawEntry IRecipientSession.FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		TEntry IRecipientSession.FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindByExchangeObjectId(Guid exchangeObjectId)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindByExchangeGuid(Guid exchangeGuid)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromExchangeGuid(exchangeGuid, false, false), ObjectType.Recipient, null));
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromExchangeGuid(exchangeGuid, true, true), ObjectType.Recipient, null));
		}

		ADRawEntry IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromExchangeGuid(exchangeGuid, true, true), ObjectType.ADRawEntry, properties));
		}

		TObject IRecipientSession.FindByExchangeGuidIncludingAlternate<TObject>(Guid exchangeGuid)
		{
			ObjectType objectTypeFor = CacheUtils.GetObjectTypeFor(typeof(TObject), false);
			if (objectTypeFor == ObjectType.Unknown)
			{
				return default(TObject);
			}
			return this.InternalGet<TObject>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromExchangeGuid(exchangeGuid, true, true), objectTypeFor, null));
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromExchangeGuid(exchangeGuid, false, true), ObjectType.Recipient, null));
		}

		Result<ADRecipient>[] IRecipientSession.FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromLegacyExchangeDNs(legacyExchangeDN), ObjectType.Recipient, null));
		}

		Result<ADRawEntry>[] IRecipientSession.FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindByObjectGuid(Guid guid)
		{
			ADObjectId objectId = new ADObjectId(null, guid);
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(objectId), ObjectType.Recipient, null));
		}

		ADRecipient IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromProxyAddress(proxyAddress), ObjectType.Recipient, null));
		}

		ADRawEntry IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromProxyAddress(proxyAddress), ObjectType.ADRawEntry, properties));
		}

		TEntry IRecipientSession.FindByProxyAddress<TEntry>(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<TEntry>[] IRecipientSession.FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses)
		{
			throw new NotImplementedException();
		}

		Result<ADRecipient>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.FindBySid(SecurityIdentifier sId)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromSid(sId), ObjectType.Recipient, null));
		}

		ADRawEntry IRecipientSession.FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromSid(sId), ObjectType.ADRawEntry, properties));
		}

		ADRawEntry[] IRecipientSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			throw new NotImplementedException();
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		TResult IRecipientSession.FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			ObjectType objectTypeFor = CacheUtils.GetObjectTypeFor(typeof(TResult), false);
			if (objectTypeFor == ObjectType.Unknown)
			{
				return default(TResult);
			}
			return this.InternalGet<TResult>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromProxyAddress(proxyAddress), objectTypeFor, properties));
		}

		TResult IRecipientSession.FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			ObjectType objectTypeFor = CacheUtils.GetObjectTypeFor(typeof(TResult), false);
			if (objectTypeFor == ObjectType.Unknown)
			{
				return default(TResult);
			}
			return this.InternalGet<TResult>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromSid(sId), objectTypeFor, properties));
		}

		ADRecipient[] IRecipientSession.FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			throw new NotImplementedException();
		}

		object[][] IRecipientSession.FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<OWAMiniRecipient>[] IRecipientSession.FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<ADRecipient> IRecipientSession.FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			throw new NotImplementedException();
		}

		ADPagedReader<TEntry> IRecipientSession.FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] IRecipientSession.FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw new NotImplementedException();
		}

		IEnumerable<ADGroup> IRecipientSession.FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId)
		{
			throw new NotImplementedException();
		}

		List<string> IRecipientSession.GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			throw new NotImplementedException();
		}

		List<string> IRecipientSession.GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			throw new NotImplementedException();
		}

		SecurityIdentifier IRecipientSession.GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			throw new NotImplementedException();
		}

		bool IRecipientSession.IsLegacyDNInUse(string legacyDN)
		{
			throw new NotImplementedException();
		}

		bool IRecipientSession.IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			throw new NotImplementedException();
		}

		bool IRecipientSession.IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		bool IRecipientSession.IsReducedRecipientSession()
		{
			throw new NotImplementedException();
		}

		bool IRecipientSession.IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			throw new NotImplementedException();
		}

		ADRecipient IRecipientSession.Read(ADObjectId entryId)
		{
			return this.InternalGet<ADRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(entryId), ObjectType.Recipient, null));
		}

		TMiniRecipient IRecipientSession.ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			ObjectType objectTypeFor = CacheUtils.GetObjectTypeFor(typeof(TMiniRecipient), false);
			if (objectTypeFor != ObjectType.MiniRecipient && objectTypeFor != ObjectType.TransportMiniRecipient && objectTypeFor != ObjectType.LoadBalancingMiniRecipient && objectTypeFor != ObjectType.OWAMiniRecipient && objectTypeFor != ObjectType.ActiveSyncMiniRecipient && objectTypeFor != ObjectType.StorageMiniRecipient)
			{
				return default(TMiniRecipient);
			}
			return this.InternalGet<TMiniRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(entryId), objectTypeFor, properties));
		}

		MiniRecipient IRecipientSession.ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalGet<MiniRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(entryId), ObjectType.MiniRecipient, properties));
		}

		Result<ADRecipient>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<ADGroup>[] IRecipientSession.ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			throw new NotImplementedException();
		}

		Result<ADUser>[] IRecipientSession.ReadMultipleADUsers(ADObjectId[] userIds)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADObjectId[] IRecipientSession.ResolveSidsToADObjectIds(string[] sids)
		{
			throw new NotImplementedException();
		}

		void IRecipientSession.Save(ADRecipient instanceToSave)
		{
			this.InternalSave(instanceToSave, null, null, 2147483646, CacheItemPriority.Default);
		}

		void IRecipientSession.Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			this.InternalSave(instanceToSave, null, null, 2147483646, CacheItemPriority.Default);
		}

		void IRecipientSession.SetPassword(ADObject obj, SecureString password)
		{
			throw new NotImplementedException();
		}

		void IRecipientSession.SetPassword(ADObjectId id, SecureString password)
		{
			throw new NotImplementedException();
		}

		ADRawEntry ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADRawEntry[] entries)
		{
			throw new NotImplementedException();
		}

		ADObjectId ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id)
		{
			throw new NotImplementedException();
		}

		DirectoryBackendType ITenantRecipientSession.DirectoryBackendType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		MiniRecipient ITenantRecipientSession.FindRecipientByNetID(NetID netId)
		{
			return this.InternalGet<MiniRecipient>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromNetId(netId.ToString()), ObjectType.MiniRecipient, null));
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromNetId(netID), ObjectType.ADRawEntry, properties));
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties)
		{
			return this.InternalGet<ADRawEntry>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromNetId(netID), ObjectType.ADRawEntry, properties));
		}

		ADRawEntry ITenantRecipientSession.FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		Result<ADRawEntry>[] ITenantRecipientSession.ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties)
		{
			throw new NotImplementedException();
		}

		MiniRecipientWithTokenGroups IRecipientSession.ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			return this.InternalGet<MiniRecipientWithTokenGroups>(new DirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, KeyBuilder.LookupKeysFromADObjectId(id), ObjectType.MiniRecipientWithTokenGroups, null));
		}

		void ICacheDirectorySession.Insert(IConfigurable objectToSave, IEnumerable<PropertyDefinition> properties, List<Tuple<string, KeyType>> keys, int secondsTimeout, CacheItemPriority priority)
		{
			ArgumentValidator.ThrowIfNull("objectToSave", objectToSave);
			this.InternalSave((ADRawEntry)objectToSave, properties, keys, secondsTimeout, priority);
		}

		internal void SetCallerInfo(string callerFilePath, string memberName, int callerFileLine)
		{
			this.logContext.FilePath = callerFilePath;
			this.logContext.FileLine = callerFileLine;
			this.logContext.MemberName = memberName;
		}

		private void InternalSave(ADRawEntry entryToSave, IEnumerable<PropertyDefinition> properties = null, List<Tuple<string, KeyType>> otherKeys = null, int secondsTimeout = 2147483646, CacheItemPriority priority = CacheItemPriority.Default)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("secondsTimeout", secondsTimeout, 1, 2147483646);
			if (CacheUtils.GetObjectTypeFor(entryToSave.GetType(), false) == ObjectType.Unknown)
			{
				return;
			}
			List<Tuple<string, KeyType>> list = null;
			if (entryToSave is ADObject)
			{
				list = KeyBuilder.GetAddKeysFromObject(entryToSave as ADObject);
			}
			else
			{
				list = KeyBuilder.GetAddKeysFromADRawEntry(entryToSave);
			}
			if (otherKeys != null)
			{
				list.AddRange(otherKeys);
			}
			if (ExTraceGlobals.CacheSessionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CacheSessionTracer.TraceDebug((long)this.GetHashCode(), "InternalSave. DN {0}. Timeout {1}. Priority={2}. Keys [{3}]", new object[]
				{
					entryToSave.GetDistinguishedNameOrName(),
					secondsTimeout,
					priority,
					string.Join<Tuple<string, KeyType>>("|", list)
				});
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				AddDirectoryCacheRequest cacheRequest;
				if (entryToSave is ADObject)
				{
					cacheRequest = new AddDirectoryCacheRequest(list, entryToSave, this.sessionSettings.PartitionId.ForestFQDN, this.sessionSettings.CurrentOrganizationId, properties, secondsTimeout, priority);
				}
				else
				{
					List<PropertyDefinition> list2 = new List<PropertyDefinition>(entryToSave.propertyBag.Keys.Count);
					foreach (object obj in entryToSave.propertyBag.Keys)
					{
						ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)obj;
						if (entryToSave.propertyBag[adpropertyDefinition] == null || (adpropertyDefinition.IsMultivalued && ((MultiValuedPropertyBase)entryToSave.propertyBag[adpropertyDefinition]).Count == 0))
						{
							using (IEnumerator<PropertyDefinition> enumerator2 = properties.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									PropertyDefinition propertyDefinition = enumerator2.Current;
									if (adpropertyDefinition.Name.Equals(propertyDefinition.Name))
									{
										list2.Add(adpropertyDefinition);
										break;
									}
								}
								continue;
							}
						}
						list2.Add(adpropertyDefinition);
					}
					cacheRequest = new AddDirectoryCacheRequest(list, entryToSave, this.sessionSettings.PartitionId.ForestFQDN, this.sessionSettings.CurrentOrganizationId, list2, secondsTimeout, priority);
				}
				this.GetCacheClient().Put(cacheRequest);
				this.IsNewProxyObject = this.GetCacheClient().IsNewProxyObject;
				this.RetryCount = this.GetCacheClient().RetryCount;
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.CacheSessionTracer.TraceError<Exception>((long)this.GetHashCode(), "InternalGet. Exception {0}", ex);
				CachePerformanceTracker.AddException(Operation.PutOperation, ex);
			}
			finally
			{
				stopwatch.Stop();
				CachePerformanceTracker.AddPerfData(Operation.TotalWCFPutOperation, stopwatch.ElapsedMilliseconds);
			}
		}

		private void InternalDelete(ADRawEntry objectToDelete)
		{
			ArgumentValidator.ThrowIfNull("objectToDelete", objectToDelete);
			if (CacheUtils.GetObjectTypeFor(objectToDelete.GetType(), false) == ObjectType.Unknown)
			{
				return;
			}
			ExTraceGlobals.CacheSessionTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "InternalDelete. Removing Object from cache {0}", objectToDelete.Id);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				this.GetCacheClient().Remove(new RemoveDirectoryCacheRequest(this.sessionSettings.PartitionId.ForestFQDN, this.sessionSettings.CurrentOrganizationId, KeyBuilder.LookupKeysFromADObjectId(objectToDelete.Id), CacheUtils.GetObjectTypeFor(objectToDelete.GetType(), true)));
				this.IsNewProxyObject = this.GetCacheClient().IsNewProxyObject;
				this.RetryCount = this.GetCacheClient().RetryCount;
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.CacheSessionTracer.TraceError<Exception>((long)this.GetHashCode(), "InternalDelete. Exception {0}", ex);
				CachePerformanceTracker.AddException(Operation.RemoveOperation, ex);
			}
			finally
			{
				stopwatch.Stop();
				CachePerformanceTracker.AddPerfData(Operation.TotalWCFRemoveOperation, stopwatch.ElapsedMilliseconds);
			}
		}

		private TObject InternalGet<TObject>(DirectoryCacheRequest request) where TObject : ADRawEntry, new()
		{
			TObject tobject = default(TObject);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				request.SetOrganizationId(this.sessionSettings.CurrentOrganizationId);
				tobject = this.GetCacheClient().Get<TObject>(request);
				this.ResultState = this.GetCacheClient().ResultState;
				this.IsNewProxyObject = this.GetCacheClient().IsNewProxyObject;
				this.RetryCount = this.GetCacheClient().RetryCount;
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.CacheSessionTracer.TraceError<DirectoryCacheRequest, ADTransientException>((long)this.GetHashCode(), "InternalGet. Request {0} . Exception {1}", request, ex);
				CachePerformanceTracker.AddException(Operation.GetOperation, ex);
			}
			finally
			{
				stopwatch.Stop();
				CachePerformanceTracker.AddPerfData(Operation.TotalWCFGetOperation, stopwatch.ElapsedMilliseconds);
			}
			ExTraceGlobals.CacheSessionTracer.TraceDebug<DirectoryCacheRequest, string>((long)this.GetHashCode(), "InternalGet. Request {0}. Cache {1}", request, (tobject != null) ? "HIT" : "MISS");
			return tobject;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IDirectoryCacheProvider GetCacheClient()
		{
			if (this.cacheProvider == null)
			{
				this.cacheProvider = DirectoryCacheProviderFactory.Default.CreateNewDirectoryCacheProvider();
			}
			return this.cacheProvider;
		}

		[NonSerialized]
		private IDirectoryCacheProvider cacheProvider;

		private ADSessionSettings sessionSettings;

		private ADLogContext logContext = new ADLogContext();
	}
}
