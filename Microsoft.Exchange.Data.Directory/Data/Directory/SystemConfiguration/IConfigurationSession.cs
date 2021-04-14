using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal interface IConfigurationSession : IDirectorySession, IConfigDataProvider
	{
		ADObjectId ConfigurationNamingContext { get; }

		ADObjectId DeletedObjectsContainer { get; }

		ADObjectId SchemaNamingContext { get; }

		bool CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName);

		bool CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName);

		bool CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName);

		bool CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName);

		void DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler);

		AcceptedDomain[] FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId);

		ADPagedReader<TResult> FindAllPaged<TResult>() where TResult : ADConfigurationObject, new();

		ExchangeRoleAssignment[] FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll);

		T FindMailboxPolicyByName<T>(string name) where T : MailboxPolicy, new();

		MicrosoftExchangeRecipient FindMicrosoftExchangeRecipient();

		OfflineAddressBook[] FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir);

		ThrottlingPolicy[] FindOrganizationThrottlingPolicies(OrganizationId organizationId);

		ADPagedReader<TResult> FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize) where TResult : ADConfigurationObject, new();

		Result<ExchangeRoleAssignment>[] FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode);

		Result<ExchangeRoleAssignment>[] FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter);

		ManagementScope[] FindSimilarManagementScope(ManagementScope managementScope);

		T FindSingletonConfigurationObject<T>() where T : ADConfigurationObject, new();

		AcceptedDomain GetAcceptedDomainByDomainName(string domainName);

		ADPagedReader<ManagementScope> GetAllExclusiveScopes();

		ADPagedReader<ManagementScope> GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType);

		AvailabilityAddressSpace GetAvailabilityAddressSpace(string domainName);

		AvailabilityConfig GetAvailabilityConfig();

		AcceptedDomain GetDefaultAcceptedDomain();

		ExchangeConfigurationContainer GetExchangeConfigurationContainer();

		ExchangeConfigurationContainerWithAddressLists GetExchangeConfigurationContainerWithAddressLists();

		FederatedOrganizationId GetFederatedOrganizationId();

		FederatedOrganizationId GetFederatedOrganizationId(OrganizationId organizationId);

		FederatedOrganizationId GetFederatedOrganizationIdByDomainName(string domainName);

		NspiRpcClientConnection GetNspiRpcClientConnection();

		ThrottlingPolicy GetOrganizationThrottlingPolicy(OrganizationId organizationId);

		ThrottlingPolicy GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup);

		Organization GetOrgContainer();

		OrganizationRelationship GetOrganizationRelationship(string domainName);

		ADObjectId GetOrgContainerId();

		RbacContainer GetRbacContainer();

		bool ManagementScopeIsInUse(ManagementScope managementScope);

		TResult FindByExchangeObjectId<TResult>(Guid exchangeObjectId) where TResult : ADConfigurationObject, new();

		TResult Read<TResult>(ADObjectId entryId) where TResult : ADConfigurationObject, new();

		Result<TResult>[] ReadMultiple<TResult>(ADObjectId[] identities) where TResult : ADConfigurationObject, new();

		MultiValuedProperty<ReplicationCursor> ReadReplicationCursors(ADObjectId id);

		void ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom);

		void Save(ADConfigurationObject instanceToSave);
	}
}
