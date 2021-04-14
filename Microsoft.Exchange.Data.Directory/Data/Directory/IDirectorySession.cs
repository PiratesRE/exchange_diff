using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IDirectorySession
	{
		TimeSpan? ClientSideSearchTimeout { get; set; }

		ConfigScopes ConfigScope { get; }

		ConsistencyMode ConsistencyMode { get; }

		string DomainController { get; set; }

		bool EnforceContainerizedScoping { get; set; }

		bool EnforceDefaultScope { get; set; }

		string LastUsedDc { get; }

		int Lcid { get; }

		string LinkResolutionServer { get; set; }

		bool LogSizeLimitExceededEvent { get; set; }

		NetworkCredential NetworkCredential { get; }

		bool ReadOnly { get; }

		ADServerSettings ServerSettings { get; }

		TimeSpan? ServerTimeout { get; set; }

		ADSessionSettings SessionSettings { get; }

		bool SkipRangedAttributes { get; set; }

		string[] ExclusiveLdapAttributes { get; set; }

		bool UseConfigNC { get; set; }

		bool UseGlobalCatalog { get; set; }

		IActivityScope ActivityScope { get; set; }

		string CallerInfo { get; }

		void AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer);

		QueryFilter ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter);

		QueryFilter ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter);

		void CheckFilterForUnsafeIdentity(QueryFilter filter);

		void UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId);

		ADRawEntry[] Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties);

		TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults) where TResult : ADObject, new();

		ADRawEntry[] FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties);

		Result<ADRawEntry>[] FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties);

		Result<TData>[] FindByADObjectIds<TData>(ADObjectId[] ids) where TData : ADObject, new();

		Result<ADRawEntry>[] FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties);

		Result<ADRawEntry>[] FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties);

		Result<ADRawEntry>[] FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties);

		ADRawEntry[] FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties);

		ADPagedReader<TResult> FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TResult : IConfigurable, new();

		ADPagedReader<ADRawEntry> FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties);

		ADPagedReader<ADRawEntry> FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new();

		ADPagedReader<TResult> FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize) where TResult : DeletedObject, new();

		ADObjectId GetConfigurationNamingContext();

		ADObjectId GetConfigurationUnitsRoot();

		ADObjectId GetDomainNamingContext();

		ADObjectId GetHostedOrganizationsRoot();

		ADObjectId GetRootDomainNamingContext();

		ADObjectId GetSchemaNamingContext();

		PooledLdapConnection GetReadConnection(string preferredServer, ref ADObjectId rootId);

		PooledLdapConnection GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject);

		ADScope GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject);

		ADScope GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope);

		bool GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties);

		bool IsReadConnectionAvailable();

		bool IsRootIdWithinScope<TObject>(ADObjectId rootId) where TObject : IConfigurable, new();

		bool IsTenantIdentity(ADObjectId id);

		TResult[] ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance) where TResult : IConfigurable, new();

		ADRawEntry ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		RawSecurityDescriptor ReadSecurityDescriptor(ADObjectId id);

		SecurityDescriptor ReadSecurityDescriptorBlob(ADObjectId id);

		string[] ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites);

		bool ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName);

		TResult ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId) where TResult : ADObject, new();

		TResult ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN) where TResult : ADObject, new();

		TenantRelocationSyncObject RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		ADOperationResultWithData<TResult>[] RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall) where TResult : class;

		void SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd);

		void SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner);

		void SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd);

		void SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner);

		bool TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception);

		void UpdateServerSettings(PooledLdapConnection connection);

		void VerifyIsWithinScopes(ADObject entry, bool isModification);
	}
}
