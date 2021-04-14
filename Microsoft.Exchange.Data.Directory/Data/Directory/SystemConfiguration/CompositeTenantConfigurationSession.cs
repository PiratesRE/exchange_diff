using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Cache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class CompositeTenantConfigurationSession : CompositeDirectorySession<ITenantConfigurationSession>, ITenantConfigurationSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		protected override string Implementor
		{
			get
			{
				return "CompositeTenantConfigurationSession";
			}
		}

		internal CompositeTenantConfigurationSession(ITenantConfigurationSession cacheSession, ITenantConfigurationSession directorySession, bool cacheSessionForDeletingOnly = false) : base(cacheSession, directorySession, cacheSessionForDeletingOnly)
		{
		}

		ADObjectId IConfigurationSession.ConfigurationNamingContext
		{
			get
			{
				return base.GetSession().ConfigurationNamingContext;
			}
		}

		ADObjectId IConfigurationSession.DeletedObjectsContainer
		{
			get
			{
				return base.GetSession().DeletedObjectsContainer;
			}
		}

		ADObjectId IConfigurationSession.SchemaNamingContext
		{
			get
			{
				return base.GetSession().SchemaNamingContext;
			}
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			return base.GetSession().CheckForRetentionPolicyWithConflictingRetentionId(retentionId, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			return base.GetSession().CheckForRetentionPolicyWithConflictingRetentionId(retentionId, identity, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			return base.GetSession().CheckForRetentionPolicyWithConflictingRetentionId(retentionId, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			return base.GetSession().CheckForRetentionPolicyWithConflictingRetentionId(retentionId, identity, out duplicateName);
		}

		void IConfigurationSession.DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetSession().DeleteTree(instanceToDelete, handler);
				return true;
			}, "DeleteTree");
		}

		AcceptedDomain[] IConfigurationSession.FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId)
		{
			return base.InvokeWithAPILogging<AcceptedDomain[]>(() => this.GetSession().FindAcceptedDomainsByFederatedOrgId(federatedOrganizationId), "FindAcceptedDomainsByFederatedOrgId");
		}

		ADPagedReader<TResult> IConfigurationSession.FindAllPaged<TResult>()
		{
			return base.InvokeWithAPILogging<ADPagedReader<TResult>>(() => base.GetSession().FindAllPaged<TResult>(), "FindAllPaged");
		}

		ExchangeRoleAssignment[] IConfigurationSession.FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll)
		{
			return base.InvokeWithAPILogging<ExchangeRoleAssignment[]>(() => this.GetSession().FindAssignmentsForManagementScope(managementScope, returnAll), "FindAssignmentsForManagementScope");
		}

		T IConfigurationSession.FindMailboxPolicyByName<T>(string name)
		{
			return base.InvokeGetObjectWithAPILogging<T>(() => this.GetSession().FindMailboxPolicyByName<T>(name), "FindMailboxPolicyByName");
		}

		MicrosoftExchangeRecipient IConfigurationSession.FindMicrosoftExchangeRecipient()
		{
			return base.InvokeGetObjectWithAPILogging<MicrosoftExchangeRecipient>(() => base.GetSession().FindMicrosoftExchangeRecipient(), "FindMicrosoftExchangeRecipient");
		}

		OfflineAddressBook[] IConfigurationSession.FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir)
		{
			return base.InvokeWithAPILogging<OfflineAddressBook[]>(() => this.GetSession().FindOABsForWebDistributionPoint(vDir), "FindOABsForWebDistributionPoint");
		}

		ThrottlingPolicy[] IConfigurationSession.FindOrganizationThrottlingPolicies(OrganizationId organizationId)
		{
			return base.InvokeWithAPILogging<ThrottlingPolicy[]>(() => this.GetSession().FindOrganizationThrottlingPolicies(organizationId), "FindOrganizationThrottlingPolicies");
		}

		ADPagedReader<TResult> IConfigurationSession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			return base.InvokeWithAPILogging<ADPagedReader<TResult>>(() => this.GetSession().FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize), "FindPaged");
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode)
		{
			return base.InvokeWithAPILogging<Result<ExchangeRoleAssignment>[]>(() => this.GetSession().FindRoleAssignmentsByUserIds(securityPrincipalIds, partnerMode), "FindRoleAssignmentsByUserIds");
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter)
		{
			return base.InvokeWithAPILogging<Result<ExchangeRoleAssignment>[]>(() => this.GetSession().FindRoleAssignmentsByUserIds(securityPrincipalIds, additionalFilter), "FindRoleAssignmentsByUserIds");
		}

		ManagementScope[] IConfigurationSession.FindSimilarManagementScope(ManagementScope managementScope)
		{
			return base.InvokeWithAPILogging<ManagementScope[]>(() => this.GetSession().FindSimilarManagementScope(managementScope), "FindSimilarManagementScope");
		}

		T IConfigurationSession.FindSingletonConfigurationObject<T>()
		{
			return base.InvokeGetObjectWithAPILogging<T>(() => base.GetSession().FindSingletonConfigurationObject<T>(), "FindSingletonConfigurationObject");
		}

		AcceptedDomain IConfigurationSession.GetAcceptedDomainByDomainName(string domainName)
		{
			return base.InvokeGetObjectWithAPILogging<AcceptedDomain>(() => this.ExecuteSingleObjectQueryWithFallback<AcceptedDomain>((ITenantConfigurationSession session) => session.GetAcceptedDomainByDomainName(domainName), null, null), "GetAcceptedDomainByDomainName");
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllExclusiveScopes()
		{
			return base.InvokeWithAPILogging<ADPagedReader<ManagementScope>>(() => base.GetSession().GetAllExclusiveScopes(), "GetAllExclusiveScopes");
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType)
		{
			return base.InvokeWithAPILogging<ADPagedReader<ManagementScope>>(() => this.GetSession().GetAllScopes(organizationId, restrictionType), "GetAllScopes");
		}

		AvailabilityAddressSpace IConfigurationSession.GetAvailabilityAddressSpace(string domainName)
		{
			return base.InvokeGetObjectWithAPILogging<AvailabilityAddressSpace>(() => this.GetSession().GetAvailabilityAddressSpace(domainName), "GetAvailabilityAddressSpace");
		}

		AvailabilityConfig IConfigurationSession.GetAvailabilityConfig()
		{
			return base.InvokeGetObjectWithAPILogging<AvailabilityConfig>(() => base.GetSession().GetAvailabilityConfig(), "GetAvailabilityConfig");
		}

		AcceptedDomain IConfigurationSession.GetDefaultAcceptedDomain()
		{
			return base.InvokeGetObjectWithAPILogging<AcceptedDomain>(() => base.GetSession().GetDefaultAcceptedDomain(), "GetDefaultAcceptedDomain");
		}

		ExchangeConfigurationContainer IConfigurationSession.GetExchangeConfigurationContainer()
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationContainer>(() => base.GetSession().GetExchangeConfigurationContainer(), "GetExchangeConfigurationContainer");
		}

		ExchangeConfigurationContainerWithAddressLists IConfigurationSession.GetExchangeConfigurationContainerWithAddressLists()
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationContainerWithAddressLists>(() => base.GetSession().GetExchangeConfigurationContainerWithAddressLists(), "GetExchangeConfigurationContainerWithAddressLists");
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId()
		{
			return base.InvokeGetObjectWithAPILogging<FederatedOrganizationId>(() => base.GetSession().GetFederatedOrganizationId(), "GetFederatedOrganizationId");
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId(OrganizationId organizationId)
		{
			return base.InvokeGetObjectWithAPILogging<FederatedOrganizationId>(() => this.ExecuteSingleObjectQueryWithFallback<FederatedOrganizationId>((ITenantConfigurationSession session) => session.GetFederatedOrganizationId(organizationId), null, null), "GetFederatedOrganizationId");
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationIdByDomainName(string domainName)
		{
			return base.InvokeGetObjectWithAPILogging<FederatedOrganizationId>(() => this.GetSession().GetFederatedOrganizationIdByDomainName(domainName), "GetFederatedOrganizationIdByDomainName");
		}

		NspiRpcClientConnection IConfigurationSession.GetNspiRpcClientConnection()
		{
			return base.InvokeWithAPILogging<NspiRpcClientConnection>(() => base.GetSession().GetNspiRpcClientConnection(), "GetNspiRpcClientConnection");
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId)
		{
			return base.InvokeGetObjectWithAPILogging<ThrottlingPolicy>(() => this.GetSession().GetOrganizationThrottlingPolicy(organizationId), "GetOrganizationThrottlingPolicy");
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup)
		{
			return base.InvokeGetObjectWithAPILogging<ThrottlingPolicy>(() => this.GetSession().GetOrganizationThrottlingPolicy(organizationId, logFailedLookup), "GetOrganizationThrottlingPolicy");
		}

		Organization IConfigurationSession.GetOrgContainer()
		{
			return base.InvokeGetObjectWithAPILogging<Organization>(() => base.ExecuteSingleObjectQueryWithFallback<Organization>((ITenantConfigurationSession session) => session.GetOrgContainer(), null, null), "GetOrgContainer");
		}

		OrganizationRelationship IConfigurationSession.GetOrganizationRelationship(string domainName)
		{
			return base.InvokeGetObjectWithAPILogging<OrganizationRelationship>(() => this.GetSession().GetOrganizationRelationship(domainName), "GetOrganizationRelationship");
		}

		ADObjectId IConfigurationSession.GetOrgContainerId()
		{
			return base.InvokeWithAPILogging<ADObjectId>(delegate
			{
				Organization organization = base.ExecuteSingleObjectQueryWithFallback<Organization>((ITenantConfigurationSession session) => session.GetOrgContainer(), null, null);
				if (organization != null)
				{
					return organization.Id;
				}
				return null;
			}, "GetOrgContainerId");
		}

		RbacContainer IConfigurationSession.GetRbacContainer()
		{
			return base.InvokeGetObjectWithAPILogging<RbacContainer>(() => base.GetSession().GetRbacContainer(), "GetRbacContainer");
		}

		bool IConfigurationSession.ManagementScopeIsInUse(ManagementScope managementScope)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetSession().ManagementScopeIsInUse(managementScope), "ManagementScopeIsInUse");
		}

		public TResult FindByExchangeObjectId<TResult>(Guid exchangeObjectId) where TResult : ADConfigurationObject, new()
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.GetSession().FindByExchangeObjectId<TResult>(exchangeObjectId), "FindByExchangeObjectId");
		}

		TResult IConfigurationSession.Read<TResult>(ADObjectId entryId)
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.ExecuteSingleObjectQueryWithFallback<TResult>((ITenantConfigurationSession session) => session.Read<TResult>(entryId), null, null), "Read");
		}

		Result<TResult>[] IConfigurationSession.ReadMultiple<TResult>(ADObjectId[] identities)
		{
			return base.InvokeWithAPILogging<Result<TResult>[]>(() => this.GetSession().ReadMultiple<TResult>(identities), "ReadMultiple");
		}

		MultiValuedProperty<ReplicationCursor> IConfigurationSession.ReadReplicationCursors(ADObjectId id)
		{
			return base.InvokeWithAPILogging<MultiValuedProperty<ReplicationCursor>>(() => this.GetSession().ReadReplicationCursors(id), "ReadReplicationCursors");
		}

		void IConfigurationSession.ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom)
		{
			base.GetSession().ReadReplicationData(id, out replicationCursors, out repsFrom);
		}

		void IConfigurationSession.Save(ADConfigurationObject instanceToSave)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.InternalSave(instanceToSave);
				return true;
			}, "Save");
		}

		AcceptedDomain[] ITenantConfigurationSession.FindAllAcceptedDomainsInOrg(ADObjectId organizationCU)
		{
			return base.InvokeWithAPILogging<AcceptedDomain[]>(() => this.GetSession().FindAllAcceptedDomainsInOrg(organizationCU), "FindAllAcceptedDomainsInOrg");
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindAllOpenConfigurationUnits(bool excludeFull)
		{
			return base.InvokeWithAPILogging<ExchangeConfigurationUnit[]>(() => this.GetSession().FindAllOpenConfigurationUnits(excludeFull), "FindAllOpenConfigurationUnits");
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfiguration(SharedConfigurationInfo sharedConfigInfo, bool enabledSharedOrgOnly)
		{
			return base.InvokeWithAPILogging<ExchangeConfigurationUnit[]>(() => this.GetSession().FindSharedConfiguration(sharedConfigInfo, enabledSharedOrgOnly), "FindSharedConfiguration");
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfigurationByOrganizationId(OrganizationId tinyTenantId)
		{
			return base.InvokeWithAPILogging<ExchangeConfigurationUnit[]>(() => this.GetSession().FindSharedConfigurationByOrganizationId(tinyTenantId), "FindSharedConfigurationByOrganizationId");
		}

		ADRawEntry[] ITenantConfigurationSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindDeletedADRawEntryByUsnRange(lastKnownParentId, startUsn, sizeLimit, properties), "FindDeletedADRawEntryByUsnRange");
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(string externalId)
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationUnit>(() => this.ExecuteSingleObjectQueryWithFallback<ExchangeConfigurationUnit>((ITenantConfigurationSession session) => session.GetExchangeConfigurationUnitByExternalId(externalId), null, null), "GetExchangeConfigurationUnitByExternalId");
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(Guid externalDirectoryOrganizationId)
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationUnit>(() => this.ExecuteSingleObjectQueryWithFallback<ExchangeConfigurationUnit>((ITenantConfigurationSession session) => session.GetExchangeConfigurationUnitByExternalId(externalDirectoryOrganizationId), null, null), "GetExchangeConfigurationUnitByExternalId");
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByName(string organizationName)
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationUnit>(() => this.ExecuteSingleObjectQueryWithFallback<ExchangeConfigurationUnit>((ITenantConfigurationSession session) => session.GetExchangeConfigurationUnitByName(organizationName), null, null), "GetExchangeConfigurationUnitByName");
		}

		ADObjectId ITenantConfigurationSession.GetExchangeConfigurationUnitIdByName(string organizationName)
		{
			return base.InvokeWithAPILogging<ADObjectId>(() => this.GetSession().GetExchangeConfigurationUnitIdByName(organizationName), "GetExchangeConfigurationUnitIdByName");
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organizationName)
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationUnit>(() => this.ExecuteSingleObjectQueryWithFallback<ExchangeConfigurationUnit>((ITenantConfigurationSession session) => session.GetExchangeConfigurationUnitByNameOrAcceptedDomain(organizationName), delegate(ExchangeConfigurationUnit exocu)
			{
				if (!exocu.Id.Parent.Name.Equals(organizationName, StringComparison.OrdinalIgnoreCase))
				{
					return new List<Tuple<string, KeyType>>
					{
						new Tuple<string, KeyType>(organizationName, KeyType.DomainName)
					};
				}
				return null;
			}, null), "GetExchangeConfigurationUnitByNameOrAcceptedDomain");
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByUserNetID(string userNetID)
		{
			return base.InvokeGetObjectWithAPILogging<ExchangeConfigurationUnit>(() => this.ExecuteSingleObjectQueryWithFallback<ExchangeConfigurationUnit>((ITenantConfigurationSession session) => session.GetExchangeConfigurationUnitByUserNetID(userNetID), null, null), "GetExchangeConfigurationUnitByUserNetID");
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromOrgNameOrAcceptedDomain(string domainName)
		{
			return base.InvokeWithAPILogging<OrganizationId>(() => this.GetSession().GetOrganizationIdFromOrgNameOrAcceptedDomain(domainName), "GetOrganizationIdFromOrgNameOrAcceptedDomain");
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromExternalDirectoryOrgId(Guid externalDirectoryOrgId)
		{
			return base.InvokeWithAPILogging<OrganizationId>(() => this.GetSession().GetOrganizationIdFromExternalDirectoryOrgId(externalDirectoryOrgId), "GetOrganizationIdFromExternalDirectoryOrgId");
		}

		MsoTenantCookieContainer ITenantConfigurationSession.GetMsoTenantCookieContainer(Guid contextId)
		{
			return base.InvokeWithAPILogging<MsoTenantCookieContainer>(() => this.GetSession().GetMsoTenantCookieContainer(contextId), "GetMsoTenantCookieContainer");
		}

		Result<ADRawEntry>[] ITenantConfigurationSession.ReadMultipleOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().ReadMultipleOrganizationProperties(organizationOUIds, properties), "ReadMultipleOrganizationProperties");
		}

		T ITenantConfigurationSession.GetDefaultFilteringConfiguration<T>()
		{
			return base.InvokeGetObjectWithAPILogging<T>(() => base.GetSession().GetDefaultFilteringConfiguration<T>(), "GetDefaultFilteringConfiguration");
		}

		public bool IsTenantLockedOut()
		{
			return base.InvokeWithAPILogging<bool>(() => base.GetSession().IsTenantLockedOut(), "IsTenantLockedOut");
		}
	}
}
