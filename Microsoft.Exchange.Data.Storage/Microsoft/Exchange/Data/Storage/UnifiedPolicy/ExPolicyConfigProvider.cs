using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExPolicyConfigProvider : PolicyConfigProvider, IConfigurationSession, IDirectorySession, IConfigDataProvider, IDisposeTrackable, IDisposable
	{
		private ExPolicyConfigProvider(ExecutionLog logProvider) : base(logProvider)
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public ExPolicyConfigProvider(IConfigurationSession configurationSession, ExecutionLog logProvider = null) : this(logProvider)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			this.configurationSession = configurationSession;
			this.InitializeUnifiedPoliciesContainerIfNeeded();
		}

		public ExPolicyConfigProvider(Guid externalOrganizationId, bool readOnly = true, string domainController = "", ExecutionLog logProvider = null) : this(logProvider)
		{
			ADSessionSettings sessionSettings = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled ? ADSessionSettings.FromExternalDirectoryOrganizationId(externalOrganizationId) : ADSessionSettings.FromRootOrgScopeSet();
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, readOnly, ConsistencyMode.PartiallyConsistent, sessionSettings, 116, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\ExPolicyConfigProvider.cs");
			this.InitializeUnifiedPoliciesContainerIfNeeded();
		}

		public ExPolicyConfigProvider(OrganizationId organizationId, bool readOnly = true, string domainController = "", ExecutionLog logProvider = null) : this(logProvider)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 141, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\ExPolicyConfigProvider.cs");
			this.InitializeUnifiedPoliciesContainerIfNeeded();
		}

		public ADObjectId GetPolicyConfigContainer(Guid? underPolicyId)
		{
			base.CheckDispose();
			if (ExPolicyConfigProvider.IsFFOOnline)
			{
				return null;
			}
			ADObjectId adobjectId = this.configurationSession.GetOrgContainerId().GetDescendantId(PolicyStorage.PoliciesContainer);
			if (underPolicyId != null)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicyStorageBaseSchema.MasterIdentity, underPolicyId.Value.ToString());
				IConfigurable[] array = ((IConfigDataProvider)this).Find<PolicyStorage>(filter, adobjectId, false, null);
				if (array != null && array.Any<IConfigurable>())
				{
					adobjectId = (array[0].Identity as ADObjectId);
				}
			}
			return adobjectId;
		}

		public OrganizationId GetOrganizationId()
		{
			base.CheckDispose();
			return this.configurationSession.GetOrgContainer().OrganizationId;
		}

		public bool ReadOnly
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ReadOnly;
			}
		}

		public string LastUsedDc
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.LastUsedDc;
			}
		}

		private List<T> FindByQueryFilter<T>(QueryFilter queryFilter, Guid? underPolicyId) where T : PolicyConfigBase
		{
			PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(typeof(T), true);
			List<T> list = new List<T>();
			bool arg = !ExPolicyConfigProvider.IsFFOOnline && underPolicyId == null && typeof(PolicyRuleConfig).Equals(typeof(T));
			IConfigurable[] array = converterByType.GetFindStorageObjectsDelegate(this)(queryFilter, this.GetPolicyConfigContainer(underPolicyId), arg, null);
			foreach (IConfigurable configurable in array)
			{
				list.Add(converterByType.ConvertFromStorage(this, configurable as UnifiedPolicyStorageBase) as T);
			}
			return list;
		}

		private void ManageStorageObjects(Type storageObjectType, Action fromMailboxDelegate, Action fromOtherStorageDelegate)
		{
			if (ExPolicyConfigProvider.IsFFOOnline || !(storageObjectType == typeof(BindingStorage)))
			{
				fromOtherStorageDelegate();
				return;
			}
			if (fromMailboxDelegate == null)
			{
				throw new InvalidOperationException("fromMailboxDelegate is null. BindingStorage in mailbox cannot be managed.");
			}
			fromMailboxDelegate();
		}

		private ExBindingStoreObjectProvider GetExBindingStoreObjectProvider()
		{
			if (this.exBindingStoreProvider == null && !ExPolicyConfigProvider.IsFFOOnline)
			{
				this.exBindingStoreProvider = new ExBindingStoreObjectProvider(this);
			}
			return this.exBindingStoreProvider;
		}

		private void InitializeUnifiedPoliciesContainerIfNeeded()
		{
			if (!this.ReadOnly)
			{
				ADObjectId policyConfigContainer = this.GetPolicyConfigContainer(null);
				if (policyConfigContainer == null)
				{
					return;
				}
				if (this.configurationSession.Read<Container>(policyConfigContainer) == null)
				{
					Container container = new Container();
					container.SetId(policyConfigContainer);
					container.OrganizationId = this.GetOrganizationId();
					this.configurationSession.Save(container);
					base.LogOneEntry(ExecutionLog.EventType.Information, string.Format("Container '{0}' is created for unified policy.", policyConfigContainer), null);
				}
			}
			this.configurationSession.DomainController = this.configurationSession.LastUsedDc;
			base.LogOneEntry(ExecutionLog.EventType.Information, string.Format("Use domain controller: '{0}'.", this.configurationSession.DomainController), null);
		}

		private static QueryFilter CreateNameQueryFilter(ADPropertyDefinition nameProperty, string nameString)
		{
			nameString = (nameString ?? string.Empty);
			if (!nameString.StartsWith("*") && !nameString.EndsWith("*"))
			{
				return new ComparisonFilter(ComparisonOperator.Equal, nameProperty, nameString);
			}
			MatchOptions matchOptions = MatchOptions.FullString;
			if (nameString.StartsWith("*") && nameString.EndsWith("*"))
			{
				if (nameString.Length <= 2)
				{
					return null;
				}
				nameString = nameString.Substring(1, nameString.Length - 2);
				matchOptions = MatchOptions.SubString;
			}
			else if (nameString.EndsWith("*"))
			{
				nameString = nameString.Substring(0, nameString.Length - 1);
				matchOptions = MatchOptions.Prefix;
			}
			else if (nameString.StartsWith("*"))
			{
				nameString = nameString.Substring(1);
				matchOptions = MatchOptions.Suffix;
			}
			return new TextFilter(nameProperty, nameString, matchOptions, MatchFlags.IgnoreCase);
		}

		private static void MarkAsUnchanged(IEnumerable<IConfigurable> instances)
		{
			if (ExPolicyConfigProvider.IsFFOOnline)
			{
				foreach (IConfigurable configurable in instances)
				{
					ConfigurableObject configurableObject = configurable as ConfigurableObject;
					if (configurableObject != null)
					{
						configurableObject.ResetChangeTracking(true);
					}
				}
			}
		}

		protected override bool IsPermanentException(Exception exception)
		{
			return exception is DataSourceOperationException || exception is DataValidationException || exception is StoragePermanentException || base.IsPermanentException(exception);
		}

		protected override bool IsTransientException(Exception exception)
		{
			return exception is DataSourceTransientException || exception is StorageTransientException || base.IsTransientException(exception);
		}

		protected override bool IsPerObjectException(Exception exception)
		{
			return exception is DataValidationException || base.IsPerObjectException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			this.configurationSession = null;
			this.exBindingStoreProvider = null;
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			base.Dispose(disposing);
		}

		protected override string InternalGetLocalOrganizationId()
		{
			return Convert.ToBase64String(this.configurationSession.GetOrgContainer().OrganizationId.GetBytes(Encoding.UTF8));
		}

		protected override T InternalFindByIdentity<T>(Guid identity)
		{
			QueryFilter queryFilter;
			if (ExPolicyConfigProvider.IsFFOOnline)
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, identity);
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicyStorageBaseSchema.MasterIdentity, identity.ToString());
			}
			List<T> list = this.FindByQueryFilter<T>(queryFilter, null);
			if (list.Count <= 0)
			{
				return default(T);
			}
			return list[0];
		}

		protected override IEnumerable<T> InternalFindByName<T>(string name)
		{
			QueryFilter queryFilter = ExPolicyConfigProvider.CreateNameQueryFilter(ADObjectSchema.Name, name);
			return this.FindByQueryFilter<T>(queryFilter, null);
		}

		protected override IEnumerable<T> InternalFindByPolicyDefinitionConfigId<T>(Guid policyDefinitionConfigId)
		{
			ArgumentValidator.ThrowIfInvalidValue<Guid>("policyDefinitionConfigId", policyDefinitionConfigId, (Guid id) => Guid.Empty != id);
			if (!ExPolicyConfigProvider.IsFFOOnline && typeof(T).Equals(typeof(PolicyRuleConfig)))
			{
				return this.FindByQueryFilter<T>(null, new Guid?(policyDefinitionConfigId));
			}
			PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(typeof(T), true);
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, converterByType.PolicyIdProperty, policyDefinitionConfigId);
			return this.FindByQueryFilter<T>(queryFilter, null);
		}

		protected override IEnumerable<PolicyBindingConfig> InternalFindPolicyBindingConfigsByScopes(IEnumerable<string> scopes)
		{
			throw new NotImplementedException();
		}

		protected override PolicyAssociationConfig InternalFindPolicyAssociationConfigByScope(string scope)
		{
			throw new NotImplementedException();
		}

		protected override void InternalSave(PolicyConfigBase instance)
		{
			ArgumentValidator.ThrowIfNull("instance", instance);
			if (instance.ObjectState == ChangeType.Update || instance.ObjectState == ChangeType.Add)
			{
				PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(instance.GetType(), true);
				IConfigurable storageObject = converterByType.ConvertToStorage(this, instance);
				this.ManageStorageObjects(storageObject.GetType(), delegate
				{
					ExBindingStoreObjectProvider exBindingStoreObjectProvider = this.GetExBindingStoreObjectProvider();
					if (exBindingStoreObjectProvider == null)
					{
						throw new InvalidOperationException("ExBindingStoreObjectProvider shouldn't be null.");
					}
					exBindingStoreObjectProvider.SaveBindingStorage(storageObject as BindingStorage);
				}, delegate
				{
					if (storageObject is ADConfigurationObject)
					{
						this.configurationSession.Save(storageObject as ADConfigurationObject);
						return;
					}
					this.configurationSession.Save(storageObject);
				});
			}
		}

		protected override void InternalDelete(PolicyConfigBase instance)
		{
			ArgumentValidator.ThrowIfNull("instance", instance);
			PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(instance.GetType(), true);
			IConfigurable storageObject = converterByType.ConvertToStorage(this, instance);
			this.ManageStorageObjects(storageObject.GetType(), delegate
			{
				this.GetExBindingStoreObjectProvider().DeleteBindingStorage(storageObject as BindingStorage);
			}, delegate
			{
				this.configurationSession.Delete(storageObject);
			});
		}

		protected override void InternalPublishStatus(IEnumerable<UnifiedPolicyStatus> statusUpdateNotifications)
		{
			string text = this.GetOrganizationId().ToExternalDirectoryOrganizationId();
			Guid tenantId;
			if (!Guid.TryParse(text, out tenantId))
			{
				throw new PolicyConfigProviderPermanentException(string.Format("Cannot publish status because ExternalDirectoryOrganizationId is not valid guid: {0}, OrganizationId: {1}", text, this.GetOrganizationId().ToString()));
			}
			using (ITenantInfoProvider tenantInfoProvider = ExPolicyConfigProvider.tenantInfoProviderFactory.CreateTenantInfoProvider(new TenantContext(tenantId, null)))
			{
				TenantInfo tenantInfo = tenantInfoProvider.Load();
				if (tenantInfo == null || !(tenantInfo.TenantId != Guid.Empty) || string.IsNullOrEmpty(tenantInfo.SyncSvcUrl))
				{
					throw new PolicyConfigProviderPermanentException(string.Format("Cannot publish status because tenant info is wrong. TenantId: '{0}'; SyncSvcUrl: '{1}'.", (tenantInfo == null) ? string.Empty : tenantInfo.TenantId.ToString(), (tenantInfo == null) ? string.Empty : tenantInfo.SyncSvcUrl));
				}
				foreach (UnifiedPolicyStatus unifiedPolicyStatus in statusUpdateNotifications)
				{
					unifiedPolicyStatus.TenantId = tenantInfo.TenantId;
					unifiedPolicyStatus.Workload = Workload.Exchange;
				}
				string text2 = "PublishStatus_" + Guid.NewGuid().ToString();
				SyncNotificationResult syncNotificationResult = RpcClientWrapper.NotifyStatusChanges(text2, null, tenantInfo.TenantId, tenantInfo.SyncSvcUrl, false, statusUpdateNotifications.ToList<UnifiedPolicyStatus>());
				if (!syncNotificationResult.Success)
				{
					throw new PolicyConfigProviderTransientException(string.Format("Cannot publish status because of '{0}'.", syncNotificationResult.Error));
				}
				base.LogOneEntry(ExecutionLog.EventType.Information, string.Format("Status publish with notification '{0}' succeeded.", text2), null);
			}
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			base.CheckDispose();
			IConfigurable instance = null;
			this.ManageStorageObjects(typeof(T), null, delegate
			{
				instance = this.configurationSession.Read<T>(identity);
			});
			return instance;
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			base.CheckDispose();
			IConfigurable[] instances = Array<IConfigurable>.Empty;
			this.ManageStorageObjects(typeof(T), delegate
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter == null || comparisonFilter.ComparisonOperator != ComparisonOperator.Equal || (comparisonFilter.Property != BindingStorageSchema.PolicyId && comparisonFilter.Property != UnifiedPolicyStorageBaseSchema.MasterIdentity))
				{
					throw new NotImplementedException("Exchange only supports query BindingStorage by policy definition id and MasterIdentity.");
				}
				BindingStorage bindingStorage;
				if (comparisonFilter.Property == BindingStorageSchema.PolicyId)
				{
					bindingStorage = this.GetExBindingStoreObjectProvider().FindBindingStorageByPolicyId((Guid)comparisonFilter.PropertyValue);
				}
				else
				{
					bindingStorage = this.GetExBindingStoreObjectProvider().FindBindingStorageById(comparisonFilter.PropertyValue.ToString());
				}
				if (bindingStorage != null)
				{
					instances = new IConfigurable[]
					{
						bindingStorage
					};
				}
			}, delegate
			{
				instances = this.configurationSession.Find<T>(filter, rootId, deepSearch, sortBy);
			});
			ExPolicyConfigProvider.MarkAsUnchanged(instances);
			return instances;
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			base.CheckDispose();
			IEnumerable<T> instances = null;
			this.ManageStorageObjects(typeof(T), null, delegate
			{
				instances = this.configurationSession.FindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize);
			});
			return instances;
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			base.CheckDispose();
			if (instance.ObjectState != ObjectState.Unchanged)
			{
				UnifiedPolicyStorageBase unifiedPolicyStorageBase = instance as UnifiedPolicyStorageBase;
				if (unifiedPolicyStorageBase != null)
				{
					unifiedPolicyStorageBase.PolicyVersion = CombGuidGenerator.NewGuid();
					if (ExPolicyConfigProvider.IsFFOOnline && unifiedPolicyStorageBase.ObjectState == ObjectState.New && unifiedPolicyStorageBase.Guid == Guid.Empty && unifiedPolicyStorageBase.MasterIdentity != Guid.Empty)
					{
						unifiedPolicyStorageBase.SetId(new ADObjectId(unifiedPolicyStorageBase.Id.DistinguishedName, unifiedPolicyStorageBase.MasterIdentity));
					}
				}
				this.ManageStorageObjects(instance.GetType(), delegate
				{
					this.GetExBindingStoreObjectProvider().SaveBindingStorage(instance as BindingStorage);
				}, delegate
				{
					if (instance is ADConfigurationObject)
					{
						this.configurationSession.Save(instance as ADConfigurationObject);
						return;
					}
					this.configurationSession.Save(instance);
				});
				PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(instance.GetType(), false);
				if (converterByType != null)
				{
					PolicyConfigBase policyConfig = converterByType.ConvertFromStorage(this, instance as UnifiedPolicyStorageBase);
					base.OnPolicyConfigChanged(new PolicyConfigChangeEventArgs(this, policyConfig, (instance.ObjectState == ObjectState.New) ? ChangeType.Add : ChangeType.Update));
				}
			}
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			base.CheckDispose();
			UnifiedPolicyStorageBase policyStorage = instance as UnifiedPolicyStorageBase;
			this.ManageStorageObjects(instance.GetType(), delegate
			{
				this.GetExBindingStoreObjectProvider().DeleteBindingStorage(instance as BindingStorage);
			}, delegate
			{
				if (ExPolicyConfigProvider.IsFFOOnline && policyStorage != null)
				{
					policyStorage.PolicyVersion = CombGuidGenerator.NewGuid();
					this.configurationSession.Save(policyStorage);
				}
				this.configurationSession.Delete(instance);
			});
			PolicyConfigConverterBase converterByType = PolicyConfigConverterTable.GetConverterByType(instance.GetType(), false);
			if (converterByType != null)
			{
				if (policyStorage.ObjectState != ObjectState.Deleted)
				{
					policyStorage.MarkAsDeleted();
				}
				PolicyConfigBase policyConfig = converterByType.ConvertFromStorage(this, policyStorage);
				base.OnPolicyConfigChanged(new PolicyConfigChangeEventArgs(this, policyConfig, ChangeType.Delete));
			}
		}

		string IConfigDataProvider.Source
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.Source;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExPolicyConfigProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		ADObjectId IConfigurationSession.ConfigurationNamingContext
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ConfigurationNamingContext;
			}
		}

		ADObjectId IConfigurationSession.DeletedObjectsContainer
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.DeletedObjectsContainer;
			}
		}

		ADObjectId IConfigurationSession.SchemaNamingContext
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.SchemaNamingContext;
			}
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			base.CheckDispose();
			return this.configurationSession.CheckForRetentionPolicyWithConflictingRetentionId(retentionId, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionPolicyWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			base.CheckDispose();
			return this.configurationSession.CheckForRetentionPolicyWithConflictingRetentionId(retentionId, identity, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, out string duplicateName)
		{
			base.CheckDispose();
			return this.configurationSession.CheckForRetentionTagWithConflictingRetentionId(retentionId, out duplicateName);
		}

		bool IConfigurationSession.CheckForRetentionTagWithConflictingRetentionId(Guid retentionId, string identity, out string duplicateName)
		{
			base.CheckDispose();
			return this.configurationSession.CheckForRetentionTagWithConflictingRetentionId(retentionId, identity, out duplicateName);
		}

		void IConfigurationSession.DeleteTree(ADConfigurationObject instanceToDelete, TreeDeleteNotFinishedHandler handler)
		{
			base.CheckDispose();
			this.configurationSession.DeleteTree(instanceToDelete, handler);
		}

		AcceptedDomain[] IConfigurationSession.FindAcceptedDomainsByFederatedOrgId(FederatedOrganizationId federatedOrganizationId)
		{
			base.CheckDispose();
			return this.configurationSession.FindAcceptedDomainsByFederatedOrgId(federatedOrganizationId);
		}

		ADPagedReader<TResult> IConfigurationSession.FindAllPaged<TResult>()
		{
			base.CheckDispose();
			return this.configurationSession.FindAllPaged<TResult>();
		}

		ExchangeRoleAssignment[] IConfigurationSession.FindAssignmentsForManagementScope(ManagementScope managementScope, bool returnAll)
		{
			base.CheckDispose();
			return this.configurationSession.FindAssignmentsForManagementScope(managementScope, returnAll);
		}

		T IConfigurationSession.FindMailboxPolicyByName<T>(string name)
		{
			base.CheckDispose();
			return this.configurationSession.FindMailboxPolicyByName<T>(name);
		}

		MicrosoftExchangeRecipient IConfigurationSession.FindMicrosoftExchangeRecipient()
		{
			base.CheckDispose();
			return this.configurationSession.FindMicrosoftExchangeRecipient();
		}

		OfflineAddressBook[] IConfigurationSession.FindOABsForWebDistributionPoint(ADOabVirtualDirectory vDir)
		{
			base.CheckDispose();
			return this.configurationSession.FindOABsForWebDistributionPoint(vDir);
		}

		ThrottlingPolicy[] IConfigurationSession.FindOrganizationThrottlingPolicies(OrganizationId organizationId)
		{
			base.CheckDispose();
			return this.configurationSession.FindOrganizationThrottlingPolicies(organizationId);
		}

		ADPagedReader<TResult> IConfigurationSession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			base.CheckDispose();
			return this.configurationSession.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize);
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, bool partnerMode)
		{
			base.CheckDispose();
			return this.configurationSession.FindRoleAssignmentsByUserIds(securityPrincipalIds, partnerMode);
		}

		Result<ExchangeRoleAssignment>[] IConfigurationSession.FindRoleAssignmentsByUserIds(ADObjectId[] securityPrincipalIds, QueryFilter additionalFilter)
		{
			base.CheckDispose();
			return this.configurationSession.FindRoleAssignmentsByUserIds(securityPrincipalIds, additionalFilter);
		}

		ManagementScope[] IConfigurationSession.FindSimilarManagementScope(ManagementScope managementScope)
		{
			base.CheckDispose();
			return this.configurationSession.FindSimilarManagementScope(managementScope);
		}

		T IConfigurationSession.FindSingletonConfigurationObject<T>()
		{
			base.CheckDispose();
			return this.configurationSession.FindSingletonConfigurationObject<T>();
		}

		AcceptedDomain IConfigurationSession.GetAcceptedDomainByDomainName(string domainName)
		{
			base.CheckDispose();
			return this.configurationSession.GetAcceptedDomainByDomainName(domainName);
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllExclusiveScopes()
		{
			base.CheckDispose();
			return this.configurationSession.GetAllExclusiveScopes();
		}

		ADPagedReader<ManagementScope> IConfigurationSession.GetAllScopes(OrganizationId organizationId, ScopeRestrictionType restrictionType)
		{
			base.CheckDispose();
			return this.configurationSession.GetAllScopes(organizationId, restrictionType);
		}

		AvailabilityAddressSpace IConfigurationSession.GetAvailabilityAddressSpace(string domainName)
		{
			base.CheckDispose();
			return this.configurationSession.GetAvailabilityAddressSpace(domainName);
		}

		AvailabilityConfig IConfigurationSession.GetAvailabilityConfig()
		{
			base.CheckDispose();
			return this.configurationSession.GetAvailabilityConfig();
		}

		AcceptedDomain IConfigurationSession.GetDefaultAcceptedDomain()
		{
			base.CheckDispose();
			return this.configurationSession.GetDefaultAcceptedDomain();
		}

		ExchangeConfigurationContainer IConfigurationSession.GetExchangeConfigurationContainer()
		{
			base.CheckDispose();
			return this.configurationSession.GetExchangeConfigurationContainer();
		}

		ExchangeConfigurationContainerWithAddressLists IConfigurationSession.GetExchangeConfigurationContainerWithAddressLists()
		{
			base.CheckDispose();
			return this.configurationSession.GetExchangeConfigurationContainerWithAddressLists();
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId()
		{
			base.CheckDispose();
			return this.configurationSession.GetFederatedOrganizationId();
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationId(OrganizationId organizationId)
		{
			base.CheckDispose();
			return this.configurationSession.GetFederatedOrganizationId(organizationId);
		}

		FederatedOrganizationId IConfigurationSession.GetFederatedOrganizationIdByDomainName(string domainName)
		{
			base.CheckDispose();
			return this.configurationSession.GetFederatedOrganizationIdByDomainName(domainName);
		}

		NspiRpcClientConnection IConfigurationSession.GetNspiRpcClientConnection()
		{
			base.CheckDispose();
			return this.configurationSession.GetNspiRpcClientConnection();
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId)
		{
			base.CheckDispose();
			return this.configurationSession.GetOrganizationThrottlingPolicy(organizationId);
		}

		ThrottlingPolicy IConfigurationSession.GetOrganizationThrottlingPolicy(OrganizationId organizationId, bool logFailedLookup)
		{
			base.CheckDispose();
			return this.configurationSession.GetOrganizationThrottlingPolicy(organizationId, logFailedLookup);
		}

		Organization IConfigurationSession.GetOrgContainer()
		{
			base.CheckDispose();
			return this.configurationSession.GetOrgContainer();
		}

		OrganizationRelationship IConfigurationSession.GetOrganizationRelationship(string domainName)
		{
			base.CheckDispose();
			return this.configurationSession.GetOrganizationRelationship(domainName);
		}

		ADObjectId IConfigurationSession.GetOrgContainerId()
		{
			base.CheckDispose();
			return this.configurationSession.GetOrgContainerId();
		}

		RbacContainer IConfigurationSession.GetRbacContainer()
		{
			base.CheckDispose();
			return this.configurationSession.GetRbacContainer();
		}

		bool IConfigurationSession.ManagementScopeIsInUse(ManagementScope managementScope)
		{
			base.CheckDispose();
			return this.configurationSession.ManagementScopeIsInUse(managementScope);
		}

		TResult IConfigurationSession.FindByExchangeObjectId<TResult>(Guid exchangeObjectId)
		{
			base.CheckDispose();
			return this.configurationSession.FindByExchangeObjectId<TResult>(exchangeObjectId);
		}

		TResult IConfigurationSession.Read<TResult>(ADObjectId entryId)
		{
			base.CheckDispose();
			return this.configurationSession.Read<TResult>(entryId);
		}

		Result<TResult>[] IConfigurationSession.ReadMultiple<TResult>(ADObjectId[] identities)
		{
			base.CheckDispose();
			return this.configurationSession.ReadMultiple<TResult>(identities);
		}

		MultiValuedProperty<ReplicationCursor> IConfigurationSession.ReadReplicationCursors(ADObjectId id)
		{
			base.CheckDispose();
			return this.configurationSession.ReadReplicationCursors(id);
		}

		void IConfigurationSession.ReadReplicationData(ADObjectId id, out MultiValuedProperty<ReplicationCursor> replicationCursors, out MultiValuedProperty<ReplicationNeighbor> repsFrom)
		{
			base.CheckDispose();
			this.configurationSession.ReadReplicationData(id, out replicationCursors, out repsFrom);
		}

		void IConfigurationSession.Save(ADConfigurationObject instanceToSave)
		{
			base.CheckDispose();
			this.configurationSession.Save(instanceToSave);
		}

		TimeSpan? IDirectorySession.ClientSideSearchTimeout
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ClientSideSearchTimeout;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.ClientSideSearchTimeout = value;
			}
		}

		ConfigScopes IDirectorySession.ConfigScope
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ConfigScope;
			}
		}

		ConsistencyMode IDirectorySession.ConsistencyMode
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ConsistencyMode;
			}
		}

		string IDirectorySession.DomainController
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.DomainController;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.DomainController = value;
			}
		}

		bool IDirectorySession.EnforceContainerizedScoping
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.EnforceContainerizedScoping;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.EnforceContainerizedScoping = value;
			}
		}

		bool IDirectorySession.EnforceDefaultScope
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.EnforceDefaultScope;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.EnforceDefaultScope = value;
			}
		}

		string IDirectorySession.LastUsedDc
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.LastUsedDc;
			}
		}

		int IDirectorySession.Lcid
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.Lcid;
			}
		}

		string IDirectorySession.LinkResolutionServer
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.LinkResolutionServer;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.LinkResolutionServer = value;
			}
		}

		bool IDirectorySession.LogSizeLimitExceededEvent
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.LogSizeLimitExceededEvent;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.LogSizeLimitExceededEvent = value;
			}
		}

		NetworkCredential IDirectorySession.NetworkCredential
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.NetworkCredential;
			}
		}

		bool IDirectorySession.ReadOnly
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ReadOnly;
			}
		}

		ADServerSettings IDirectorySession.ServerSettings
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ServerSettings;
			}
		}

		TimeSpan? IDirectorySession.ServerTimeout
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ServerTimeout;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.ServerTimeout = value;
			}
		}

		ADSessionSettings IDirectorySession.SessionSettings
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.SessionSettings;
			}
		}

		bool IDirectorySession.SkipRangedAttributes
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.SkipRangedAttributes;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.SkipRangedAttributes = value;
			}
		}

		public string[] ExclusiveLdapAttributes
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ExclusiveLdapAttributes;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.ExclusiveLdapAttributes = value;
			}
		}

		bool IDirectorySession.UseConfigNC
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.UseConfigNC;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.UseConfigNC = value;
			}
		}

		bool IDirectorySession.UseGlobalCatalog
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.UseGlobalCatalog;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.UseGlobalCatalog = value;
			}
		}

		IActivityScope IDirectorySession.ActivityScope
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.ActivityScope;
			}
			set
			{
				base.CheckDispose();
				this.configurationSession.ActivityScope = value;
			}
		}

		string IDirectorySession.CallerInfo
		{
			get
			{
				base.CheckDispose();
				return this.configurationSession.CallerInfo;
			}
		}

		void IDirectorySession.AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			base.CheckDispose();
			this.configurationSession.AnalyzeDirectoryError(connection, request, de, totalRetries, retriesOnServer);
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter)
		{
			base.CheckDispose();
			return this.configurationSession.ApplyDefaultFilters(filter, rootId, dummyObject, applyImplicitFilter);
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			base.CheckDispose();
			return this.configurationSession.ApplyDefaultFilters(filter, scope, dummyObject, applyImplicitFilter);
		}

		void IDirectorySession.CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			base.CheckDispose();
			this.configurationSession.CheckFilterForUnsafeIdentity(filter);
		}

		void IDirectorySession.UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			base.CheckDispose();
			this.configurationSession.UnsafeExecuteModificationRequest(request, rootId);
		}

		ADRawEntry[] IDirectorySession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.Find(rootId, scope, filter, sortBy, maxResults, properties);
		}

		TResult[] IDirectorySession.Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			base.CheckDispose();
			return this.configurationSession.Find<TResult>(rootId, scope, filter, sortBy, maxResults);
		}

		ADRawEntry[] IDirectorySession.FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindAllADRawEntriesByUsnRange(root, startUsn, endUsn, sizeLimit, useAtomicFilter, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindByADObjectIds(ids, properties);
		}

		Result<TData>[] IDirectorySession.FindByADObjectIds<TData>(ADObjectId[] ids)
		{
			base.CheckDispose();
			return this.configurationSession.FindByADObjectIds<TData>(ids);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindByCorrelationIds(correlationIds, configUnit, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindByExchangeLegacyDNs(exchangeLegacyDNs, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindByObjectGuids(objectGuids, properties);
		}

		ADRawEntry[] IDirectorySession.FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindDeletedTenantSyncObjectByUsnRange(tenantOuRoot, startUsn, sizeLimit, properties);
		}

		ADPagedReader<TResult> IDirectorySession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindPagedADRawEntry(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.FindPagedADRawEntryWithDefaultFilters<TResult>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<TResult> IDirectorySession.FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			base.CheckDispose();
			return this.configurationSession.FindPagedDeletedObject<TResult>(rootId, scope, filter, sortBy, pageSize);
		}

		ADObjectId IDirectorySession.GetConfigurationNamingContext()
		{
			base.CheckDispose();
			return this.configurationSession.GetConfigurationNamingContext();
		}

		ADObjectId IDirectorySession.GetConfigurationUnitsRoot()
		{
			base.CheckDispose();
			return this.configurationSession.GetConfigurationUnitsRoot();
		}

		ADObjectId IDirectorySession.GetDomainNamingContext()
		{
			base.CheckDispose();
			return this.configurationSession.GetDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetHostedOrganizationsRoot()
		{
			base.CheckDispose();
			return this.configurationSession.GetHostedOrganizationsRoot();
		}

		ADObjectId IDirectorySession.GetRootDomainNamingContext()
		{
			base.CheckDispose();
			return this.configurationSession.GetRootDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetSchemaNamingContext()
		{
			base.CheckDispose();
			return this.configurationSession.GetSchemaNamingContext();
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			base.CheckDispose();
			return this.configurationSession.GetReadConnection(preferredServer, ref rootId);
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			base.CheckDispose();
			return this.configurationSession.GetReadConnection(preferredServer, optionalBaseDN, ref rootId, scopeDeteriminingObject);
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			base.CheckDispose();
			return this.configurationSession.GetReadScope(rootId, scopeDeterminingObject);
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			base.CheckDispose();
			return this.configurationSession.GetReadScope(rootId, scopeDeterminingObject, isWellKnownGuidSearch, out applicableScope);
		}

		bool IDirectorySession.GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.GetSchemaAndApplyFilter(adRawEntry, scope, out dummyObject, out ldapAttributes, ref filter, ref properties);
		}

		bool IDirectorySession.IsReadConnectionAvailable()
		{
			base.CheckDispose();
			return this.configurationSession.IsReadConnectionAvailable();
		}

		bool IDirectorySession.IsRootIdWithinScope<TObject>(ADObjectId rootId)
		{
			base.CheckDispose();
			return this.configurationSession.IsRootIdWithinScope<TObject>(rootId);
		}

		bool IDirectorySession.IsTenantIdentity(ADObjectId id)
		{
			base.CheckDispose();
			return this.configurationSession.IsTenantIdentity(id);
		}

		TResult[] IDirectorySession.ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance)
		{
			base.CheckDispose();
			return this.configurationSession.ObjectsFromEntries<TResult>(entries, originatingServerName, properties, dummyInstance);
		}

		ADRawEntry IDirectorySession.ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.ReadADRawEntry(entryId, properties);
		}

		RawSecurityDescriptor IDirectorySession.ReadSecurityDescriptor(ADObjectId id)
		{
			base.CheckDispose();
			return this.configurationSession.ReadSecurityDescriptor(id);
		}

		SecurityDescriptor IDirectorySession.ReadSecurityDescriptorBlob(ADObjectId id)
		{
			base.CheckDispose();
			return this.configurationSession.ReadSecurityDescriptorBlob(id);
		}

		string[] IDirectorySession.ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			base.CheckDispose();
			return this.configurationSession.ReplicateSingleObject(instanceToReplicate, sites);
		}

		bool IDirectorySession.ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			base.CheckDispose();
			return this.configurationSession.ReplicateSingleObjectToTargetDC(instanceToReplicate, targetServerName);
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId)
		{
			base.CheckDispose();
			return this.configurationSession.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerId);
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN)
		{
			base.CheckDispose();
			return this.configurationSession.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerDN);
		}

		TenantRelocationSyncObject IDirectorySession.RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			base.CheckDispose();
			return this.configurationSession.RetrieveTenantRelocationSyncObject(entryId, properties);
		}

		ADOperationResultWithData<TResult>[] IDirectorySession.RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall)
		{
			base.CheckDispose();
			return this.configurationSession.RunAgainstAllDCsInSite<TResult>(siteId, methodToCall);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			base.CheckDispose();
			this.configurationSession.SaveSecurityDescriptor(id, sd);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			base.CheckDispose();
			this.configurationSession.SaveSecurityDescriptor(id, sd, modifyOwner);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			base.CheckDispose();
			this.configurationSession.SaveSecurityDescriptor(obj, sd);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			base.CheckDispose();
			this.configurationSession.SaveSecurityDescriptor(obj, sd, modifyOwner);
		}

		bool IDirectorySession.TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			base.CheckDispose();
			return this.configurationSession.TryVerifyIsWithinScopes(entry, isModification, out exception);
		}

		void IDirectorySession.UpdateServerSettings(PooledLdapConnection connection)
		{
			base.CheckDispose();
			this.configurationSession.UpdateServerSettings(connection);
		}

		void IDirectorySession.VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			base.CheckDispose();
			this.configurationSession.VerifyIsWithinScopes(entry, isModification);
		}

		private static TenantInfoProviderFactory tenantInfoProviderFactory = new TenantInfoProviderFactory(TimeSpan.FromHours(4.0), 10, 1000);

		internal static readonly bool IsFFOOnline = Datacenter.IsForefrontForOfficeDatacenter();

		private DisposeTracker disposeTracker;

		private IConfigurationSession configurationSession;

		private ExBindingStoreObjectProvider exBindingStoreProvider;
	}
}
