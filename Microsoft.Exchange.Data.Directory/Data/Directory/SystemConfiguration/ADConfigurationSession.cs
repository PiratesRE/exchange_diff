using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class ADConfigurationSession : ADDataSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		public ADConfigurationSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.Delete((ADConfigurationObject)instance);
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			if (!typeof(ADConfigurationObject).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(DirectoryStrings.ExceptionADConfigurationObjectRequired(typeof(T).Name, "IConfigDataProvider.Find<T>"), "T");
			}
			return (IConfigurable[])base.Find<T>((ADObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, sortBy, 0, null, false);
		}

		public AcceptedDomain[] FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.FederatedOrganizationLink, federatedOrganizationId.Id)
			});
			return base.Find<AcceptedDomain>(federatedOrganizationId.ConfigurationUnit, QueryScope.SubTree, filter, null, 0);
		}

		public OfflineAddressBook[] FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir)
		{
			if (vDir == null)
			{
				throw new ArgumentNullException("vDir");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, OfflineAddressBookSchema.VirtualDirectories, vDir.Id);
			ADPagedReader<OfflineAddressBook> adpagedReader = this.FindPaged<OfflineAddressBook>(null, QueryScope.SubTree, filter, null, 0);
			List<OfflineAddressBook> list = new List<OfflineAddressBook>();
			foreach (OfflineAddressBook item in adpagedReader)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return base.FindPaged<T>((ADObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, sortBy, pageSize, null);
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			return base.InternalRead<T>((ADObjectId)identity, null);
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			this.Save((ADConfigurationObject)instance);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return base.LastUsedDc;
			}
		}

		public bool CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			return this.CheckForRetentionPolicyWithConflictingRetentionId(retentionId, null, out duplicateName);
		}

		public bool CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			duplicateName = string.Empty;
			ADPagedReader<RetentionPolicy> adpagedReader = this.FindPaged<RetentionPolicy>(this.GetOrgContainerId(), QueryScope.SubTree, null, null, 0);
			foreach (RetentionPolicy retentionPolicy in adpagedReader)
			{
				if (retentionPolicy.RetentionId == retentionId)
				{
					if (!string.IsNullOrEmpty(identity) && retentionPolicy.Identity.Equals(identity))
					{
						return true;
					}
					duplicateName = retentionPolicy.Name;
					return false;
				}
			}
			return true;
		}

		public bool CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			return this.CheckForRetentionTagWithConflictingRetentionId(retentionId, null, out duplicateName);
		}

		public bool CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			duplicateName = string.Empty;
			ADPagedReader<RetentionPolicyTag> adpagedReader = this.FindPaged<RetentionPolicyTag>(this.GetOrgContainerId(), QueryScope.SubTree, null, null, 0);
			foreach (RetentionPolicyTag retentionPolicyTag in adpagedReader)
			{
				if (retentionPolicyTag.RetentionId == retentionId)
				{
					if (!string.IsNullOrEmpty(identity) && retentionPolicyTag.Identity.Equals(identity))
					{
						return true;
					}
					duplicateName = retentionPolicyTag.Name;
					return false;
				}
			}
			return true;
		}

		protected override ADObject CreateAndInitializeObject<TResult>(ADPropertyBag propertyBag, ADRawEntry dummyObject)
		{
			return ADObjectFactory.CreateAndInitializeConfigObject<TResult>(propertyBag, dummyObject, this);
		}

		public ADPagedReader<TResult> FindAllPaged<TResult>() where TResult : ADConfigurationObject, new()
		{
			return this.FindPaged<TResult>(null, QueryScope.SubTree, null, null, 0);
		}

		public TResult FindByExchangeObjectId<TResult>(Guid exchangeObjectId) where TResult : ADConfigurationObject, new()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeObjectId, exchangeObjectId);
			TResult[] array = base.Find<TResult>(null, QueryScope.SubTree, filter, null, 2);
			if (array == null || array.Length == 0)
			{
				return default(TResult);
			}
			return array[0];
		}

		public TResult Read<TResult>(ADObjectId entryId) where TResult : ADConfigurationObject, new()
		{
			return base.InternalRead<TResult>(entryId, null);
		}

		public void Save(ADConfigurationObject instanceToSave)
		{
			if (instanceToSave == null)
			{
				throw new ArgumentNullException("instanceToSave");
			}
			base.Save(instanceToSave, instanceToSave.Schema.AllProperties);
		}

		public void Delete(ADConfigurationObject instanceToDelete)
		{
			base.Delete(instanceToDelete);
		}

		public void DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler)
		{
			if (instanceToDelete == null)
			{
				throw new ArgumentNullException("instanceToDelete");
			}
			ADConfigurationSession adconfigurationSession = this;
			IDirectorySession session = instanceToDelete.Session;
			try
			{
				for (;;)
				{
					try
					{
						adconfigurationSession.Delete(instanceToDelete, true);
					}
					catch (ADTreeDeleteNotFinishedException ex)
					{
						ExTraceGlobals.ADTopologyTracer.TraceWarning<string, string>((long)this.GetHashCode(), "ADTreeDeleteNotFinishedException is caught while deleting the whole tree '{0}': {1}", instanceToDelete.Identity.ToString(), ex.Message);
						if (handler != null)
						{
							handler(ex);
						}
						if (string.IsNullOrEmpty(adconfigurationSession.DomainController))
						{
							adconfigurationSession = (ADConfigurationSession)this.Clone();
							adconfigurationSession.DomainController = ex.Server;
							instanceToDelete.m_Session = null;
						}
						continue;
					}
					break;
				}
			}
			finally
			{
				instanceToDelete.m_Session = session;
			}
		}

		private object Clone()
		{
			return base.MemberwiseClone();
		}

		public T FindSingletonConfigurationObject<T>() where T : ADConfigurationObject, new()
		{
			T[] array = base.Find<T>(null, QueryScope.SubTree, null, null, 1);
			if (array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		public AvailabilityAddressSpace GetAvailabilityAddressSpace(string domainName)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, AvailabilityAddressSpaceSchema.ForestName, domainName);
			AvailabilityAddressSpace[] array = base.Find<AvailabilityAddressSpace>(this.GetOrgContainerId(), QueryScope.SubTree, filter, null, 1);
			if (array.Length == 1)
			{
				return array[0];
			}
			return null;
		}

		public AvailabilityConfig GetAvailabilityConfig()
		{
			AvailabilityConfig[] array = base.Find<AvailabilityConfig>(this.GetOrgContainerId(), QueryScope.SubTree, null, null, 1);
			if (array.Length == 1)
			{
				return array[0];
			}
			return null;
		}

		public ExchangeConfigurationContainer GetExchangeConfigurationContainer()
		{
			return this.InternalGetExchangeConfigurationContainer<ExchangeConfigurationContainer>();
		}

		public ExchangeConfigurationContainerWithAddressLists GetExchangeConfigurationContainerWithAddressLists()
		{
			return this.InternalGetExchangeConfigurationContainer<ExchangeConfigurationContainerWithAddressLists>();
		}

		public ThrottlingPolicy GetOrganizationThrottlingPolicy(OrganizationId organizationId)
		{
			return this.GetOrganizationThrottlingPolicy(organizationId, true);
		}

		public ThrottlingPolicy GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup)
		{
			string text = (OrganizationId.ForestWideOrgId.Equals(organizationId) || organizationId == null) ? this.GetOrgContainerId().DistinguishedName : organizationId.ConfigurationUnit.DistinguishedName;
			ThrottlingPolicy[] array = this.FindOrganizationThrottlingPolicies(organizationId);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[ADConfigurationSession::GetOrganizationThrottlingPolicy] No organization policy found in org '{0}'.", text);
				return null;
			}
			if (array.Length != 1)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError<string>((long)this.GetHashCode(), "[ADConfigurationSession::GetOrganizationThrottlingPolicy] Multiple organization policies found in org '{0}'.", text);
				if (logFailedLookup)
				{
					Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_MoreThanOneOrganizationThrottlingPolicy, text, new object[]
					{
						text
					});
				}
				return null;
			}
			return array[0];
		}

		public ADObjectId GetOrgContainerId()
		{
			if (this.orgContainerId != null)
			{
				return this.orgContainerId;
			}
			if (Globals.IsDatacenter && (base.SessionSettings.ConfigReadScope == null || base.SessionSettings.ConfigReadScope.Root == null))
			{
				this.orgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(base.SessionSettings.GetAccountOrResourceForestFqdn(), base.DomainController, base.NetworkCredential);
			}
			if (this.orgContainerId == null)
			{
				Organization orgContainer = this.GetOrgContainer();
				this.orgContainerId = orgContainer.Id;
			}
			return this.orgContainerId;
		}

		public Organization GetOrgContainer()
		{
			ADObjectId adobjectId = null;
			if (base.SessionSettings.ConfigReadScope != null)
			{
				adobjectId = base.SessionSettings.ConfigReadScope.Root;
			}
			Organization[] array;
			if (adobjectId == null)
			{
				if (Globals.IsDatacenter)
				{
					Organization organization = (Organization)ADSystemConfigurationSession.GetRootOrgContainer(base.SessionSettings.GetAccountOrResourceForestFqdn(), base.DomainController, base.NetworkCredential).Clone();
					organization.SetIsReadOnly(false);
					organization.m_Session = this;
					return organization;
				}
				array = base.Find<Organization>(null, QueryScope.SubTree, null, null, 2);
			}
			else if (adobjectId.Parent.Parent.Equals(ADSession.GetConfigurationUnitsRoot(base.SessionSettings.GetAccountOrResourceForestFqdn())))
			{
				array = base.Find<ExchangeConfigurationUnit>(adobjectId, QueryScope.Base, null, null, 1);
			}
			else
			{
				array = base.Find<ExchangeConfigurationUnit>(adobjectId, QueryScope.SubTree, null, null, 2);
			}
			if (array == null || array.Length == 0)
			{
				if (adobjectId == null)
				{
					throw new OrgContainerNotFoundException();
				}
				throw new TenantOrgContainerNotFoundException(adobjectId.ToString());
			}
			else
			{
				if (array.Length > 1)
				{
					throw new OrgContainerAmbiguousException();
				}
				return array[0];
			}
		}

		public RbacContainer GetRbacContainer()
		{
			ADObjectId adobjectId = this.GetOrgContainerId();
			return this.Read<RbacContainer>(adobjectId.GetDescendantId(new ADObjectId("CN=RBAC")));
		}

		public ManagementScope[] FindSimilarManagementScope(ManagementScope managementScope)
		{
			if (managementScope == null)
			{
				throw new ArgumentNullException("managementScope");
			}
			QueryFilter queryFilter;
			if (managementScope.RecipientRoot == null)
			{
				queryFilter = new NotFilter(new ExistsFilter(ManagementScopeSchema.RecipientRoot));
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.RecipientRoot, managementScope.RecipientRoot);
			}
			QueryFilter queryFilter2;
			if (string.IsNullOrEmpty(managementScope.Filter))
			{
				queryFilter2 = new NotFilter(new ExistsFilter(ManagementScopeSchema.Filter));
			}
			else
			{
				queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.Filter, managementScope.Filter);
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2,
				new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.Exclusive, managementScope.Exclusive),
				new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.ScopeRestrictionType, managementScope.ScopeRestrictionType),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, managementScope.Id)
			});
			List<ManagementScope> source = new List<ManagementScope>(base.Find<ManagementScope>(managementScope.Id.Parent, QueryScope.OneLevel, filter, null, 0));
			return (from scope in source
			where scope.ScopeRestrictionType == managementScope.ScopeRestrictionType
			select scope).ToArray<ManagementScope>();
		}

		public bool ManagementScopeIsInUse(ManagementScope managementScope)
		{
			ExchangeRoleAssignment[] array = this.FindAssignmentsForManagementScope(managementScope, false);
			return array.Length != 0;
		}

		public ExchangeRoleAssignment[] FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll)
		{
			if (managementScope == null)
			{
				throw new ArgumentNullException("managementScope");
			}
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomRecipientWriteScope, managementScope.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.CustomConfigWriteScope, managementScope.Id)
			});
			ADPagedReader<ExchangeRoleAssignment> adpagedReader = this.FindPaged<ExchangeRoleAssignment>(null, QueryScope.SubTree, filter, null, returnAll ? 0 : 1);
			List<ExchangeRoleAssignment> list = new List<ExchangeRoleAssignment>();
			foreach (ExchangeRoleAssignment item in adpagedReader)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		public NspiRpcClientConnection GetNspiRpcClientConnection()
		{
			string text = base.ServerSettings.PreferredGlobalCatalog(base.SessionSettings.GetAccountOrResourceForestFqdn());
			string domainController;
			if (!string.IsNullOrEmpty(text))
			{
				domainController = text;
			}
			else
			{
				PooledLdapConnection pooledLdapConnection = null;
				try
				{
					pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.GlobalCatalog, base.SessionSettings.GetAccountOrResourceForestFqdn());
					domainController = pooledLdapConnection.ServerName;
				}
				finally
				{
					if (pooledLdapConnection != null)
					{
						pooledLdapConnection.ReturnToPool();
					}
				}
			}
			return NspiRpcClientConnection.GetNspiRpcClientConnection(domainController);
		}

		public T FindMailboxPolicyByName<T>(string name) where T : MailboxPolicy, new()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, name);
			T[] array = base.Find<T>(null, QueryScope.SubTree, filter, null, 2);
			if (array == null || array.Length <= 0)
			{
				return default(T);
			}
			return array[0];
		}

		public MicrosoftExchangeRecipient FindMicrosoftExchangeRecipient()
		{
			return this.Read<MicrosoftExchangeRecipient>(ADMicrosoftExchangeRecipient.GetDefaultId(this));
		}

		public ThrottlingPolicy[] FindOrganizationThrottlingPolicies(OrganizationId organizationId)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ThrottlingPolicySchema.ThrottlingPolicyScope, ThrottlingPolicyScopeType.Organization);
			if (organizationId == null && (base.ConfigScope == ConfigScopes.TenantSubTree || base.ConfigScope == ConfigScopes.None))
			{
				return null;
			}
			ADObjectId rootId = (OrganizationId.ForestWideOrgId.Equals(organizationId) || organizationId == null) ? this.GetOrgContainerId() : organizationId.ConfigurationUnit;
			return base.Find<ThrottlingPolicy>(rootId, QueryScope.SubTree, filter, null, 2);
		}

		public ADPagedReader<TResult> FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize) where TResult : ADConfigurationObject, new()
		{
			return base.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize, null);
		}

		public FederatedOrganizationId GetFederatedOrganizationId(OrganizationId organizationId)
		{
			return this.GetFederatedOrganizationId(organizationId.ConfigurationUnit);
		}

		public FederatedOrganizationId GetFederatedOrganizationId()
		{
			return this.GetFederatedOrganizationId(this.GetOrgContainerId());
		}

		private FederatedOrganizationId GetFederatedOrganizationId(ADObjectId rootId)
		{
			FederatedOrganizationId[] array = base.Find<FederatedOrganizationId>(rootId, QueryScope.SubTree, null, null, 1);
			if (array.Length == 1)
			{
				return array[0];
			}
			return null;
		}

		public FederatedOrganizationId GetFederatedOrganizationIdByDomainName(string domainName)
		{
			AcceptedDomain acceptedDomainByDomainName = this.GetAcceptedDomainByDomainName(domainName);
			if (acceptedDomainByDomainName == null || acceptedDomainByDomainName.FederatedOrganizationLink == null)
			{
				return null;
			}
			return this.Read<FederatedOrganizationId>(acceptedDomainByDomainName.FederatedOrganizationLink);
		}

		public AcceptedDomain GetDefaultAcceptedDomain()
		{
			BitMaskAndFilter filter = new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 4UL);
			AcceptedDomain[] array = base.Find<AcceptedDomain>(this.GetOrgContainerId(), QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length > 1)
			{
				Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_MULTIPLE_DEFAULT_ACCEPTED_DOMAIN, array[0].DistinguishedName, new object[]
				{
					array.Length.ToString()
				});
			}
			return array[0];
		}

		public AcceptedDomain GetAcceptedDomainByDomainName(string domainName)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter("msExchAcceptedDomain"),
				new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, domainName)
			});
			AcceptedDomain[] array = base.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 2);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length == 2)
			{
				throw new ADOperationException(DirectoryStrings.DuplicatedAcceptedDomain(domainName, array[0].Id.ToDNString(), array[1].Id.ToDNString()));
			}
			return array[0];
		}

		public OrganizationRelationship GetOrganizationRelationship(string domainName)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, OrganizationRelationshipSchema.DomainNames, domainName);
			OrganizationRelationship[] array = base.Find<OrganizationRelationship>(this.GetOrgContainerId(), QueryScope.SubTree, filter, null, 1);
			if (array.Length == 1)
			{
				return array[0];
			}
			return null;
		}

		public Result<ExchangeRoleAssignment>[] FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode)
		{
			QueryFilter partnerFilter = RoleAssignmentFlagsFormat.GetPartnerFilter(partnerMode);
			return this.FindRoleAssignmentsByUserIds(securityPrincipalIds, partnerFilter);
		}

		public Result<ExchangeRoleAssignment>[] FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter)
		{
			if (securityPrincipalIds == null)
			{
				throw new ArgumentNullException("securityPrincipalIds");
			}
			if (securityPrincipalIds.Length == 0)
			{
				throw new ArgumentException("securityPrincipalIds");
			}
			Converter<ADObjectId, QueryFilter> filterBuilder = delegate(ADObjectId id)
			{
				if (id == null)
				{
					throw new ArgumentNullException("id");
				}
				return new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, id);
			};
			return base.ReadMultiple<ADObjectId, ExchangeRoleAssignment>(securityPrincipalIds, null, filterBuilder, additionalFilter, null, null, null);
		}

		public Result<TResult>[] ReadMultiple<TResult>(ADObjectId[] identities) where TResult : ADConfigurationObject, new()
		{
			if (identities == null)
			{
				throw new ArgumentNullException("identities");
			}
			if (identities.Length == 0)
			{
				return new Result<TResult>[0];
			}
			return base.ReadMultiple<ADObjectId, TResult>(identities, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), new ADDataSession.HashInserter<TResult>(ADRecipientObjectSession.ADObjectIdHashInserter<TResult>), new ADDataSession.HashLookup<ADObjectId, TResult>(ADRecipientObjectSession.ADObjectIdHashLookup<TResult>), null);
		}

		public ADPagedReader<ManagementScope> GetAllExclusiveScopes()
		{
			return this.FindPaged<ManagementScope>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.Exclusive, true), null, 0);
		}

		public ADPagedReader<ManagementScope> GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType)
		{
			ADObjectId rootId = (OrganizationId.ForestWideOrgId.Equals(organizationId) || organizationId == null) ? this.GetOrgContainerId() : organizationId.ConfigurationUnit;
			return this.FindPaged<ManagementScope>(rootId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.ScopeRestrictionType, restrictionType), null, 0);
		}

		public MultiValuedProperty<ReplicationCursor> ReadReplicationCursors(ADObjectId id)
		{
			if (string.IsNullOrEmpty(base.DomainController))
			{
				throw new NotSupportedException("The session has to be bound to a specific Domain Controller.");
			}
			ADRawEntry adrawEntry = base.InternalRead<ADRawEntry>(id, new ADPropertyDefinition[]
			{
				ADDomainSchema.ReplicationCursors
			});
			if (adrawEntry == null)
			{
				return new MultiValuedProperty<ReplicationCursor>();
			}
			return adrawEntry.propertyBag[ADDomainSchema.ReplicationCursors] as MultiValuedProperty<ReplicationCursor>;
		}

		public void ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom)
		{
			if (string.IsNullOrEmpty(base.DomainController))
			{
				throw new NotSupportedException("The session has to be bound to a specific Domain Controller.");
			}
			ADRawEntry adrawEntry = base.InternalRead<ADRawEntry>(id, new ADPropertyDefinition[]
			{
				ADDomainSchema.ReplicationCursors,
				ADDomainSchema.RepsFrom
			});
			if (adrawEntry == null)
			{
				replicationCursors = new MultiValuedProperty<ReplicationCursor>();
				repsFrom = new MultiValuedProperty<ReplicationNeighbor>();
				return;
			}
			replicationCursors = (adrawEntry.propertyBag[ADDomainSchema.ReplicationCursors] as MultiValuedProperty<ReplicationCursor>);
			repsFrom = (adrawEntry.propertyBag[ADDomainSchema.RepsFrom] as MultiValuedProperty<ReplicationNeighbor>);
		}

		public ADObjectId ConfigurationNamingContext
		{
			get
			{
				return base.GetConfigurationNamingContext();
			}
		}

		public virtual ADObjectId DeletedObjectsContainer
		{
			get
			{
				return this.ConfigurationNamingContext.GetChildId("Deleted Objects");
			}
		}

		public ADObjectId SchemaNamingContext
		{
			get
			{
				return base.GetSchemaNamingContext();
			}
		}

		private T InternalGetExchangeConfigurationContainer<T>() where T : ExchangeConfigurationContainer, new()
		{
			T[] array = base.Find<T>(null, QueryScope.SubTree, null, null, 1);
			if (array == null || array.Length == 0)
			{
				throw new ExchangeConfigurationContainerNotFoundException();
			}
			return array[0];
		}

		[NonSerialized]
		private ADObjectId orgContainerId;
	}
}
