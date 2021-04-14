using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal abstract class FfoConfigurationSession : FfoDirectorySession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		protected FfoConfigurationSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		protected FfoConfigurationSession(ADObjectId tenantId) : base(tenantId)
		{
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			IConfigurable configurable = this.ReadImpl<T>(identity);
			ConfigurableObject configurableObject = configurable as ConfigurableObject;
			if (configurableObject != null)
			{
				configurableObject.ResetChangeTracking();
			}
			return configurable;
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return (IConfigurable[])((IConfigDataProvider)this).FindPaged<T>(filter, rootId, deepSearch, sortBy, int.MaxValue).ToArray<T>();
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			foreach (T t in this.FindImpl<T>(base.AddTenantIdFilter(filter), rootId, deepSearch, sortBy, pageSize))
			{
				ConfigurableObject configurableObject = t as ConfigurableObject;
				if (configurableObject != null)
				{
					configurableObject.ResetChangeTracking();
				}
				yield return t;
			}
			yield break;
		}

		void IConfigDataProvider.Save(IConfigurable configurable)
		{
			base.FixOrganizationalUnitRoot(configurable);
			base.GenerateIdForObject(configurable);
			base.ApplyAuditProperties(configurable);
			base.DataProvider.Save(configurable);
		}

		void IConfigDataProvider.Delete(IConfigurable configurable)
		{
			base.FixOrganizationalUnitRoot(configurable);
			base.GenerateIdForObject(configurable);
			base.ApplyAuditProperties(configurable);
			if (configurable is BindingStorage)
			{
				this.DeleteBindingStorage((BindingStorage)configurable);
				return;
			}
			base.DataProvider.Delete(configurable);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return "FfoConfigurationSession";
			}
		}

		OfflineAddressBook[] IConfigurationSession.FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new OfflineAddressBook[0];
		}

		public AvailabilityAddressSpace GetAvailabilityAddressSpace(string domainName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		public AvailabilityConfig GetAvailabilityConfig()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			duplicateName = null;
			return true;
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			duplicateName = null;
			return true;
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			duplicateName = null;
			return true;
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			duplicateName = null;
			return true;
		}

		void IConfigurationSession.DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			((IConfigDataProvider)this).Delete(instanceToDelete);
		}

		AcceptedDomain[] IConfigurationSession.FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new AcceptedDomain[0];
		}

		ADPagedReader<TResult> IConfigurationSession.FindAllPaged<TResult>()
		{
			return new FfoPagedReader<TResult>(this, null, null);
		}

		ExchangeRoleAssignment[] IConfigurationSession.FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ExchangeRoleAssignment[0];
		}

		T IConfigurationSession.FindMailboxPolicyByName<T>(string name)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(T);
		}

		MicrosoftExchangeRecipient IConfigurationSession.FindMicrosoftExchangeRecipient()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ThrottlingPolicy[] IConfigurationSession.FindOrganizationThrottlingPolicies(OrganizationId organizationId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADPagedReader<TResult> IConfigurationSession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			return new FfoPagedReader<TResult>(this, filter, rootId, pageSize);
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ExchangeRoleAssignment>[0];
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ExchangeRoleAssignment>[0];
		}

		ManagementScope[] IConfigurationSession.FindSimilarManagementScope(ManagementScope managementScope)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ManagementScope[0];
		}

		T IConfigurationSession.FindSingletonConfigurationObject<T>()
		{
			IConfigurable[] array = ((IConfigDataProvider)this).Find<T>(null, null, false, null);
			if (array == null || array.Length == 0)
			{
				return default(T);
			}
			return (T)((object)array[0]);
		}

		AcceptedDomain IConfigurationSession.GetAcceptedDomainByDomainName(string domainName)
		{
			return base.FindTenantObject<AcceptedDomain>(new object[]
			{
				ADObjectSchema.Name,
				domainName
			}).Cast<AcceptedDomain>().FirstOrDefault<AcceptedDomain>();
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllExclusiveScopes()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new FfoPagedReader<ManagementScope>(this, null, null);
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationId.OrganizationalUnit);
			return new FfoPagedReader<ManagementScope>(this, filter, null);
		}

		AcceptedDomain IConfigurationSession.GetDefaultAcceptedDomain()
		{
			IConfigurable[] source = ((IConfigDataProvider)this).Find<AcceptedDomain>(null, null, false, null);
			return source.Cast<AcceptedDomain>().FirstOrDefault((AcceptedDomain acceptedDomain) => acceptedDomain.Default);
		}

		ExchangeConfigurationContainer IConfigurationSession.GetExchangeConfigurationContainer()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ExchangeConfigurationContainerWithAddressLists IConfigurationSession.GetExchangeConfigurationContainerWithAddressLists()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId(OrganizationId organizationId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationIdByDomainName(string domainName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		NspiRpcClientConnection IConfigurationSession.GetNspiRpcClientConnection()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		OrganizationRelationship IConfigurationSession.GetOrganizationRelationship(string domainName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		Organization IConfigurationSession.GetOrgContainer()
		{
			return ((IConfigurationSession)this).Read<Organization>(base.TenantId);
		}

		ADObjectId IConfigurationSession.GetOrgContainerId()
		{
			return base.TenantId;
		}

		RbacContainer IConfigurationSession.GetRbacContainer()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		bool IConfigurationSession.ManagementScopeIsInUse(ManagementScope managementScope)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		public TResult FindByExchangeObjectId<TResult>(Guid exchangeObjectId) where TResult : ADConfigurationObject, new()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(TResult);
		}

		TResult IConfigurationSession.Read<TResult>(ADObjectId entryId)
		{
			return (TResult)((object)((IConfigDataProvider)this).Read<TResult>(entryId));
		}

		Result<TResult>[] IConfigurationSession.ReadMultiple<TResult>(ADObjectId[] identities)
		{
			int num = 0;
			Result<TResult>[] array = new Result<TResult>[identities.Length];
			foreach (ADObjectId entryId in identities)
			{
				TResult data = ((IConfigurationSession)this).Read<TResult>(entryId);
				array[num++] = new Result<TResult>(data, null);
			}
			return array;
		}

		MultiValuedProperty<ReplicationCursor> IConfigurationSession.ReadReplicationCursors(ADObjectId id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		void IConfigurationSession.ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom)
		{
			replicationCursors = null;
			repsFrom = null;
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IConfigurationSession.Save(ADConfigurationObject instanceToSave)
		{
			if (instanceToSave is BindingStorage)
			{
				this.SaveBindingStorage((BindingStorage)instanceToSave);
				return;
			}
			((IConfigDataProvider)this).Save(instanceToSave);
		}

		ADObjectId IConfigurationSession.ConfigurationNamingContext
		{
			get
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return null;
			}
		}

		ADObjectId IConfigurationSession.DeletedObjectsContainer
		{
			get
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return null;
			}
		}

		ADObjectId IConfigurationSession.SchemaNamingContext
		{
			get
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return null;
			}
		}

		internal static ExchangeConfigurationUnit GetExchangeConfigurationUnit(FfoTenant ffoTenant)
		{
			if (ffoTenant == null)
			{
				return null;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = new ExchangeConfigurationUnit();
			FfoConfigurationSession.SetTenantIds(ffoTenant, exchangeConfigurationUnit);
			exchangeConfigurationUnit.ExternalDirectoryOrganizationId = ffoTenant.TenantId.ObjectGuid.ToString();
			exchangeConfigurationUnit.CompanyTags = ffoTenant.CompanyTags;
			exchangeConfigurationUnit.DirSyncServiceInstance = ffoTenant.ServiceInstance;
			return exchangeConfigurationUnit;
		}

		protected static ADOrganizationalUnit GetADOrganizationalUnit(FfoTenant ffoTenant)
		{
			if (ffoTenant == null)
			{
				return null;
			}
			ADOrganizationalUnit adorganizationalUnit = new ADOrganizationalUnit();
			FfoConfigurationSession.SetTenantIds(ffoTenant, adorganizationalUnit);
			return adorganizationalUnit;
		}

		protected static Organization GetOrganization(FfoTenant ffoTenant)
		{
			if (ffoTenant == null)
			{
				return null;
			}
			Organization organization = new Organization();
			FfoConfigurationSession.SetTenantIds(ffoTenant, organization);
			return organization;
		}

		private static void SetTenantIds(FfoTenant ffoTenant, ADObject adTenantObject)
		{
			if (ffoTenant == null)
			{
				return;
			}
			adTenantObject.Name = ffoTenant.TenantName;
			FfoDirectorySession.FixDistinguishedName(adTenantObject, DalHelper.GetTenantDistinguishedName(ffoTenant.TenantName), ffoTenant.TenantId.ObjectGuid, ffoTenant.TenantId.ObjectGuid, null);
		}

		private IConfigurable ReadImpl<T>(ObjectId id) where T : IConfigurable, new()
		{
			if (typeof(T) == typeof(ExchangeConfigurationUnit))
			{
				FfoTenant ffoTenant = base.ReadAndHandleException<FfoTenant>(base.AddTenantIdFilter(null));
				return FfoConfigurationSession.GetExchangeConfigurationUnit(ffoTenant);
			}
			if (typeof(T) == typeof(ADOrganizationalUnit))
			{
				FfoTenant ffoTenant2 = base.ReadAndHandleException<FfoTenant>(base.AddTenantIdFilter(null));
				return FfoConfigurationSession.GetADOrganizationalUnit(ffoTenant2);
			}
			if (typeof(T) == typeof(Organization))
			{
				FfoTenant ffoTenant3 = base.ReadAndHandleException<FfoTenant>(base.AddTenantIdFilter(null));
				return FfoConfigurationSession.GetOrganization(ffoTenant3);
			}
			T t = base.ReadAndHandleException<T>(base.AddTenantIdFilter(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id)));
			ADObject adobject = t as ADObject;
			if (adobject != null)
			{
				FfoDirectorySession.FixDistinguishedName(adobject, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)adobject.Identity).ObjectGuid, null);
			}
			return t;
		}

		private IEnumerable<T> FindImpl<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (base.TenantId == null)
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
			}
			else if (typeof(T) == typeof(ExchangeConfigurationUnit))
			{
				IEnumerable<FfoTenant> ffoTenants = base.FindAndHandleException<FfoTenant>(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (FfoTenant ffoTenant in ffoTenants)
				{
					yield return (T)((object)FfoConfigurationSession.GetExchangeConfigurationUnit(ffoTenant));
				}
			}
			else if (typeof(T) == typeof(ADOrganizationalUnit))
			{
				IEnumerable<FfoTenant> ffoTenants2 = base.FindAndHandleException<FfoTenant>(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (FfoTenant ffoTenant2 in ffoTenants2)
				{
					yield return (T)((object)FfoConfigurationSession.GetADOrganizationalUnit(ffoTenant2));
				}
			}
			else if (typeof(T) == typeof(Organization))
			{
				IEnumerable<FfoTenant> ffoTenants3 = base.FindAndHandleException<FfoTenant>(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (FfoTenant ffoTenant3 in ffoTenants3)
				{
					yield return (T)((object)FfoConfigurationSession.GetOrganization(ffoTenant3));
				}
			}
			else if (typeof(T) == typeof(TransportRuleCollection))
			{
				IEnumerable<TransportRuleCollection> collections = this.FindTransportRuleCollections(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (TransportRuleCollection coll in collections)
				{
					yield return (T)((object)coll);
				}
			}
			else if (typeof(T) == typeof(BindingStorage))
			{
				IEnumerable<BindingStorage> bindings = this.FindBindingStorage(filter, rootId, deepSearch, sortBy, pageSize, true);
				foreach (BindingStorage storage in bindings)
				{
					yield return (T)((object)storage);
				}
			}
			else if (typeof(T) == typeof(ExchangeRoleAssignment))
			{
				IEnumerable<ExchangeRoleAssignment> roleAssignments = this.FindExchangeRoleAssignments(filter, rootId, deepSearch, sortBy, pageSize, true);
				foreach (ExchangeRoleAssignment roleAssignment in roleAssignments)
				{
					yield return (T)((object)roleAssignment);
				}
			}
			else if (typeof(T) == typeof(ExchangeRole))
			{
				IEnumerable<ExchangeRole> exchangeRoles = base.FindAndHandleException<ExchangeRole>(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (ExchangeRole exchangeRole in exchangeRoles)
				{
					FfoDirectorySession.FixDistinguishedName(exchangeRole, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)exchangeRole.Identity).ObjectGuid, ExchangeRole.RdnContainer);
					this.UpdateImplictScope(exchangeRole);
					yield return (T)((object)exchangeRole);
				}
			}
			else
			{
				IEnumerable<T> configurables = null;
				try
				{
					configurables = base.FindAndHandleException<T>(filter, rootId, deepSearch, sortBy, pageSize);
				}
				catch (DataProviderMappingException ex)
				{
					FfoDirectorySession.LogNotSupportedInFFO(ex);
				}
				if (configurables == null || configurables.Count<T>() == 0)
				{
					configurables = base.GetDefaultArray<T>();
				}
				configurables = this.DoPostQueryFilter<T>(filter, configurables);
				foreach (T configurable in configurables)
				{
					ADObject adObject = configurable as ADObject;
					if (adObject != null)
					{
						FfoDirectorySession.FixDistinguishedName(adObject, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)adObject.Identity).ObjectGuid, null);
					}
					yield return configurable;
				}
			}
			yield break;
		}

		private IEnumerable<T> DoPostQueryFilter<T>(QueryFilter filter, IEnumerable<T> configurables) where T : IConfigurable, new()
		{
			var func = null;
			var func2 = null;
			if (typeof(T) != typeof(DomainContentConfig) || filter == null)
			{
				return configurables;
			}
			string input = filter.ToString();
			Match match = FfoConfigurationSession.domainContentConfigDupCheckFilterRegex.Match(input);
			if (match.Success)
			{
				ReadOnlyCollection<QueryFilter> filters = ((AndFilter)filter).Filters;
				filters = ((AndFilter)filters[0]).Filters;
				Guid idGuid = (Guid)((ComparisonFilter)filters[0]).PropertyValue;
				ReadOnlyCollection<QueryFilter> filters2 = ((OrFilter)filters[1]).Filters;
				string domainName1 = (string)((ComparisonFilter)filters2[0]).PropertyValue;
				string domainName2 = (string)((ComparisonFilter)filters2[1]).PropertyValue;
				IEnumerable<DomainContentConfig> source = configurables.Cast<DomainContentConfig>();
				if (func == null)
				{
					func = ((DomainContentConfig dcc) => new
					{
						dcc = dcc,
						dccDomainName = ((dcc.DomainName != null) ? dcc.DomainName.Domain : null)
					});
				}
				var source2 = from <>h__TransparentIdentifier4b in source.Select(func)
				where <>h__TransparentIdentifier4b.dcc.Guid != idGuid && (string.Equals(<>h__TransparentIdentifier4b.dccDomainName, domainName1, StringComparison.InvariantCultureIgnoreCase) || string.Equals(<>h__TransparentIdentifier4b.dccDomainName, domainName2, StringComparison.InvariantCultureIgnoreCase))
				select <>h__TransparentIdentifier4b;
				if (func2 == null)
				{
					func2 = (<>h__TransparentIdentifier4b => (T)((object)<>h__TransparentIdentifier4b.dcc));
				}
				return source2.Select(func2);
			}
			return configurables;
		}

		private IEnumerable<TransportRuleCollection> FindTransportRuleCollections(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize = 2147483647)
		{
			object obj;
			Guid objectGuid;
			IEnumerable<TransportRuleCollection> enumerable;
			if (DalHelper.TryFindPropertyValueByName(filter, ADObjectSchema.Name.Name, out obj) && obj is string && FfoConfigurationSession.builtInTransportRuleContainers.TryGetValue((string)obj, out objectGuid))
			{
				TransportRuleCollection transportRuleCollection = new TransportRuleCollection
				{
					Name = (string)obj
				};
				FfoDirectorySession.FixDistinguishedName(transportRuleCollection, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, objectGuid, null);
				enumerable = new TransportRuleCollection[]
				{
					transportRuleCollection
				};
			}
			else
			{
				enumerable = base.FindAndHandleException<TransportRuleCollection>(filter, rootId, deepSearch, sortBy, pageSize);
				foreach (TransportRuleCollection transportRuleCollection2 in enumerable)
				{
					FfoDirectorySession.FixDistinguishedName(transportRuleCollection2, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)transportRuleCollection2.Identity).ObjectGuid, null);
				}
			}
			return enumerable;
		}

		private IEnumerable<ExchangeRoleAssignment> FindExchangeRoleAssignments(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize = 2147483647, bool includeScopes = true)
		{
			IEnumerable<ExchangeRoleAssignment> roleAssignments = base.FindAndHandleException<ExchangeRoleAssignment>(filter, rootId, deepSearch, sortBy, pageSize);
			IEnumerable<string> roleNames = (from roleAssignment in roleAssignments
			select roleAssignment.Role.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToArray<string>();
			Dictionary<string, ExchangeRole> cannedRoles = new Dictionary<string, ExchangeRole>(StringComparer.OrdinalIgnoreCase);
			foreach (string text in roleNames)
			{
				ExchangeRole exchangeRole = new ExchangeRole
				{
					Name = text
				};
				if (this.UpdateImplictScope(exchangeRole))
				{
					cannedRoles.Add(text, exchangeRole);
				}
			}
			foreach (ExchangeRoleAssignment roleAssignment2 in roleAssignments)
			{
				FfoDirectorySession.FixDistinguishedName(roleAssignment2, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)roleAssignment2.Identity).ObjectGuid, ExchangeRoleAssignment.RdnContainer);
				roleAssignment2.Role = FfoDirectorySession.GetUpdatedADObjectIdWithDN(roleAssignment2.Role, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ExchangeRole.RdnContainer);
				if (roleAssignment2.RoleAssigneeType == RoleAssigneeType.RoleGroup && cannedRoles.ContainsKey(roleAssignment2.Role.Name))
				{
					ExchangeRole exchangeRole2 = cannedRoles[roleAssignment2.Role.Name];
					roleAssignment2.RecipientReadScope = exchangeRole2.ImplicitRecipientReadScope;
					roleAssignment2.ConfigReadScope = exchangeRole2.ImplicitConfigReadScope;
					roleAssignment2.RecipientWriteScope = (RecipientWriteScopeType)exchangeRole2.ImplicitRecipientWriteScope;
					roleAssignment2.ConfigWriteScope = (ConfigWriteScopeType)exchangeRole2.ImplicitConfigWriteScope;
				}
				yield return roleAssignment2;
			}
			yield break;
		}

		private IEnumerable<BindingStorage> FindBindingStorage(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize = 2147483647, bool includeScopes = true)
		{
			IEnumerable<BindingStorage> configurables = base.FindAndHandleException<BindingStorage>(filter, rootId, deepSearch, sortBy, pageSize);
			if (configurables == null || configurables.Count<BindingStorage>() == 0)
			{
				configurables = base.GetDefaultArray<BindingStorage>();
			}
			configurables = this.DoPostQueryFilter<BindingStorage>(filter, configurables);
			foreach (BindingStorage configurable in configurables)
			{
				FfoDirectorySession.FixDistinguishedName(configurable, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)configurable.Identity).ObjectGuid, null);
				IEnumerable<ScopeStorage> childScopes = base.FindAndHandleException<ScopeStorage>(QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, base.TenantId),
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.ContainerProp, configurable.Id.ObjectGuid)
				}), rootId, deepSearch, sortBy, pageSize);
				foreach (ScopeStorage scopeStorage in childScopes)
				{
					scopeStorage.ResetChangeTracking();
				}
				configurable.AppliedScopes = new MultiValuedProperty<ScopeStorage>(childScopes);
				yield return configurable;
			}
			yield break;
		}

		private void SaveBindingStorage(BindingStorage bindingInstance)
		{
			BindingStorage existingStorage = this.FindBindingStorage(QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, base.TenantId),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, bindingInstance.Id.ObjectGuid)
			}), null, false, null, int.MaxValue, true).Cast<BindingStorage>().FirstOrDefault<BindingStorage>();
			if (existingStorage == null)
			{
				using (MultiValuedProperty<ScopeStorage>.Enumerator enumerator = bindingInstance.AppliedScopes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ScopeStorage scopeStorage = enumerator.Current;
						scopeStorage[UnifiedPolicyStorageBaseSchema.ContainerProp] = bindingInstance.Id.ObjectGuid.ToString();
						scopeStorage[UnifiedPolicyStorageBaseSchema.WorkloadProp] = bindingInstance.Workload;
						((IConfigDataProvider)this).Save(scopeStorage);
					}
					goto IL_20A;
				}
			}
			IEnumerable<ScopeStorage> enumerable = from s in bindingInstance.AppliedScopes
			where !existingStorage.AppliedScopes.Any((ScopeStorage e) => e.Id.ObjectGuid == s.Id.ObjectGuid) || bindingInstance.AppliedScopes.Any((ScopeStorage e) => e.Id.ObjectGuid == s.Id.ObjectGuid && s.GetChangedPropertyDefinitions().Any<PropertyDefinition>())
			select s;
			IEnumerable<ScopeStorage> enumerable2 = from e in existingStorage.AppliedScopes
			where !bindingInstance.AppliedScopes.Any((ScopeStorage n) => n.Id.ObjectGuid == e.Id.ObjectGuid)
			select e;
			foreach (ScopeStorage instance in enumerable2)
			{
				((IConfigDataProvider)this).Delete(instance);
			}
			foreach (ScopeStorage scopeStorage2 in enumerable)
			{
				scopeStorage2[UnifiedPolicyStorageBaseSchema.ContainerProp] = bindingInstance.Id.ObjectGuid.ToString();
				scopeStorage2[UnifiedPolicyStorageBaseSchema.WorkloadProp] = bindingInstance.Workload;
				((IConfigDataProvider)this).Save(scopeStorage2);
			}
			IL_20A:
			bindingInstance[UnifiedPolicyStorageBaseSchema.ContainerProp] = bindingInstance.PolicyId.ToString();
			((IConfigDataProvider)this).Save(bindingInstance);
		}

		private void DeleteBindingStorage(BindingStorage bindingInstance)
		{
			foreach (ScopeStorage instance in bindingInstance.AppliedScopes)
			{
				((IConfigDataProvider)this).Delete(instance);
			}
			base.DataProvider.Delete(bindingInstance);
		}

		private bool UpdateImplictScope(ExchangeRole exchangeRole)
		{
			string value = exchangeRole.Name.Replace(" ", string.Empty).Replace("-", string.Empty);
			RoleType roleType;
			if (Enum.TryParse<RoleType>(value, true, out roleType))
			{
				exchangeRole.RoleType = roleType;
				exchangeRole.StampImplicitScopes();
				exchangeRole.StampIsEndUserRole();
				return true;
			}
			return false;
		}

		private static readonly Dictionary<string, Guid> builtInTransportRuleContainers = new Dictionary<string, Guid>
		{
			{
				"MalwareFilterVersioned",
				Guid.Parse("66BF36AA-ECD6-404e-9CD8-F2A9C1037154")
			},
			{
				"HostedContentFilterVersioned",
				Guid.Parse("42A3E6E5-1048-4c22-8990-CFFE8ACDCFC1")
			}
		};

		private static Regex domainContentConfigDupCheckFilterRegex = new Regex("\\(\\&\\(\\(\\&\\(\\(Guid NotEqual [^\\)]+\\)\\(\\|\\(\\(DomainName Equal [^\\)]+\\)\\(DomainName Equal [^\\)]+\\)\\)\\)\\)\\)\\(OrganizationalUnitRoot Equal [^\\)]+\\)\\)\\)");
	}
}
