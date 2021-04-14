using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.UnifiedPolicy;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Hygiene.Data.Directory.UnifiedPolicy
{
	internal class UnifiedPolicySession : IUnifiedPolicyStatusPublisher
	{
		public static void Authorize(IEnumerable<Guid> claimedTenantIds, IEnumerable<Guid> requestedTenantIds)
		{
			if (claimedTenantIds == null)
			{
				throw new ArgumentNullException("claimedTenantIds");
			}
			if (requestedTenantIds == null)
			{
				throw new ArgumentNullException("claimedTenantIds");
			}
			if (claimedTenantIds.Any((Guid t) => !requestedTenantIds.Contains(t)))
			{
				throw new UnauthorizedAccessException("Tenant claims(s) do not match API-specified tenants");
			}
			if (requestedTenantIds.Any((Guid t) => !claimedTenantIds.Contains(t)))
			{
				throw new UnauthorizedAccessException("Tenant claims(s) do not match API-specified tenants");
			}
		}

		public static void Authorize(IEnumerable<Guid> claimedTenantIds, Guid requestedTenantId)
		{
			UnifiedPolicySession.Authorize(claimedTenantIds, new Guid[]
			{
				requestedTenantId
			});
		}

		public PolicyConfigurationBase GetObject(Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects = false)
		{
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				return this.GetObjectInternal<PolicyStorage, PolicyConfiguration>(tenantId, objectId, includeDeletedObjects, new Func<PolicyStorage, PolicyConfiguration>(UnifiedPolicyStorageFactory.FromPolicyStorage));
			case ConfigurationObjectType.Rule:
				return this.GetObjectInternal<RuleStorage, RuleConfiguration>(tenantId, objectId, includeDeletedObjects, new Func<RuleStorage, RuleConfiguration>(UnifiedPolicyStorageFactory.FromRuleStorage));
			case ConfigurationObjectType.Association:
				return this.GetObjectInternal<AssociationStorage, AssociationConfiguration>(tenantId, objectId, includeDeletedObjects, new Func<AssociationStorage, AssociationConfiguration>(UnifiedPolicyStorageFactory.FromAssociationStorage));
			case ConfigurationObjectType.Binding:
				return this.GetObjectInternal<BindingStorage, BindingConfiguration>(tenantId, objectId, includeDeletedObjects, new Func<BindingStorage, BindingConfiguration>(UnifiedPolicyStorageFactory.FromBindingStorage));
			default:
				throw new NotSupportedException();
			}
		}

		public PolicyChangeBatch GetChanges(TenantCookieCollection cookies)
		{
			switch (cookies.ObjectType)
			{
			case ConfigurationObjectType.Policy:
				return this.GetChangesInternal<PolicyStorage, PolicyConfiguration>(cookies, new Func<PolicyStorage, PolicyConfiguration>(UnifiedPolicyStorageFactory.FromPolicyStorage), false);
			case ConfigurationObjectType.Rule:
				return this.GetChangesInternal<RuleStorage, RuleConfiguration>(cookies, new Func<RuleStorage, RuleConfiguration>(UnifiedPolicyStorageFactory.FromRuleStorage), false);
			case ConfigurationObjectType.Association:
				return this.GetChangesInternal<AssociationStorage, AssociationConfiguration>(cookies, new Func<AssociationStorage, AssociationConfiguration>(UnifiedPolicyStorageFactory.FromAssociationStorage), true);
			case ConfigurationObjectType.Binding:
				return this.GetBindingChanges(cookies);
			default:
				throw new NotSupportedException();
			}
		}

		public void PublishStatus(IEnumerable<object> statuses, bool deleteConfiguration)
		{
			this.PublishStatus(statuses.Cast<UnifiedPolicyStatus>(), deleteConfiguration);
		}

		public void PublishStatus(IEnumerable<UnifiedPolicyStatus> statuses, bool deleteConfiguration = true)
		{
			Dictionary<Guid, List<UnifiedPolicyStatus>> dictionary = UnifiedPolicySession.PartitionByTenant<UnifiedPolicyStatus>(statuses, (UnifiedPolicyStatus status) => status.TenantId);
			foreach (Guid guid in dictionary.Keys)
			{
				ITenantConfigurationSession tenantSession = UnifiedPolicySession.GetTenantSession(guid);
				ConfigurationSettingStatusBatch configurationSettingStatusBatch = new ConfigurationSettingStatusBatch(guid);
				IEnumerable<UnifiedPolicySettingStatus> enumerable = (from s in dictionary[guid]
				select UnifiedPolicyStorageFactory.ToStatusStorage(s)).Cache<UnifiedPolicySettingStatus>();
				foreach (UnifiedPolicySettingStatus configurable in enumerable)
				{
					configurationSettingStatusBatch.Add(new TenantSettingFacade<UnifiedPolicySettingStatus>(configurable));
				}
				tenantSession.Save(configurationSettingStatusBatch);
				if (deleteConfiguration)
				{
					foreach (UnifiedPolicySettingStatus unifiedPolicySettingStatus in from s in enumerable
					where s.ObjectStatus == StatusMode.Deleted
					select s)
					{
						IEnumerable<UnifiedPolicySettingStatus> source = tenantSession.Find<UnifiedPolicySettingStatus>(QueryFilter.AndTogether(new QueryFilter[]
						{
							new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySettingStatusSchema.SettingType, unifiedPolicySettingStatus.SettingType),
							new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySettingStatusSchema.ObjectId, unifiedPolicySettingStatus.Id.ObjectGuid)
						}), null, false, null).Cast<UnifiedPolicySettingStatus>();
						if (source.All((UnifiedPolicySettingStatus s) => s.ObjectStatus == StatusMode.Deleted))
						{
							UnifiedPolicySession.DeleteReferencedObject(tenantSession, unifiedPolicySettingStatus);
						}
					}
				}
			}
		}

		private static PolicyChangeBatch GetNewBatch<TResult>(TenantCookieCollection previousCookies) where TResult : PolicyConfigurationBase
		{
			PolicyChangeBatch policyChangeBatch = new PolicyChangeBatch
			{
				Changes = new List<TResult>(),
				NewCookies = new TenantCookieCollection(previousCookies.Workload, previousCookies.ObjectType)
			};
			foreach (TenantCookie tenantCookie in ((IEnumerable<TenantCookie>)previousCookies))
			{
				policyChangeBatch.NewCookies[tenantCookie.TenantId] = new TenantCookie(tenantCookie.TenantId, tenantCookie.Cookie, previousCookies.Workload, previousCookies.ObjectType, tenantCookie.DeletedObjectTimeThreshold)
				{
					MoreData = true
				};
			}
			return policyChangeBatch;
		}

		private static Dictionary<Guid, List<T>> PartitionByTenant<T>(IEnumerable<T> collection, Func<T, Guid> getTenantFunc)
		{
			Dictionary<Guid, List<T>> dictionary = new Dictionary<Guid, List<T>>();
			foreach (T t in collection)
			{
				Guid key = getTenantFunc(t);
				List<T> list;
				if (!dictionary.TryGetValue(key, out list))
				{
					list = new List<T>();
					dictionary.Add(key, list);
				}
				list.Add(t);
			}
			return dictionary;
		}

		private static string GetPagingWatermark<T>(PolicySyncCookie cookie)
		{
			string text = null;
			if (!cookie.TryGetValue(typeof(T).Name, out text) || text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		private static void SetPagingWatermark<T>(PolicySyncCookie cookie, string newEntityCookie)
		{
			cookie[typeof(T).Name] = newEntityCookie;
		}

		private static QueryFilter CreateGetChangesQueryFilter(TenantCookie cookie, string pagingWatermark, Workload workload, bool scopeToWorkload)
		{
			object propertyValue = scopeToWorkload ? workload : null;
			return PagingHelper.GetPagingQueryFilter(QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, cookie.TenantId),
				new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySession.FlagsProperty, propertyValue),
				new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySession.DeletedObjectThresholdQueryProperty, cookie.DeletedObjectTimeThreshold),
				new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySession.DeltaChangesQueryProperty, true)
			}), pagingWatermark);
		}

		private static QueryFilter CreateGetObjectQueryFilter(Guid tenantId, Guid objectId, bool includeDeletedObjects)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, objectId),
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.IncludeTombstonesProperty, includeDeletedObjects)
			});
		}

		private static void DeleteReferencedObject(ITenantConfigurationSession session, UnifiedPolicySettingStatus deleteStatus)
		{
			IConfigurable configurable = null;
			bool flag = false;
			QueryFilter filter = UnifiedPolicySession.CreateGetObjectQueryFilter(deleteStatus.OrganizationalUnitRoot.ObjectGuid, deleteStatus.Id.ObjectGuid, true);
			string settingType;
			if ((settingType = deleteStatus.SettingType) != null)
			{
				if (!(settingType == "AssociationStorage"))
				{
					if (!(settingType == "BindingStorage"))
					{
						if (!(settingType == "PolicyStorage"))
						{
							if (!(settingType == "RuleStorage"))
							{
								if (!(settingType == "ScopeStorage"))
								{
									goto IL_F7;
								}
								configurable = session.Find<ScopeStorage>(filter, null, false, null).FirstOrDefault<IConfigurable>();
							}
							else
							{
								configurable = session.Find<RuleStorage>(filter, null, false, null).FirstOrDefault<IConfigurable>();
							}
						}
						else
						{
							configurable = session.Find<PolicyStorage>(filter, null, false, null).FirstOrDefault<IConfigurable>();
							flag = true;
						}
					}
					else
					{
						configurable = session.Find<BindingStorage>(filter, null, false, null).FirstOrDefault<IConfigurable>();
					}
				}
				else
				{
					configurable = session.Find<AssociationStorage>(filter, null, false, null).FirstOrDefault<IConfigurable>();
				}
				if (flag)
				{
					IEnumerable<RuleStorage> enumerable = from RuleStorage r in session.Find<RuleStorage>(new ComparisonFilter(ComparisonOperator.Equal, DalHelper.IncludeTombstonesProperty, true), null, false, null)
					where r.ParentPolicyId == deleteStatus.Id.ObjectGuid && r.ObjectState != ObjectState.Deleted
					select r;
					foreach (RuleStorage instance in enumerable)
					{
						session.Delete(instance);
					}
					IEnumerable<BindingStorage> enumerable2 = session.Find<BindingStorage>(QueryFilter.AndTogether(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicyStorageBaseSchema.ContainerProp, deleteStatus.Id.ObjectGuid),
						new ComparisonFilter(ComparisonOperator.Equal, DalHelper.IncludeTombstonesProperty, true)
					}), null, false, null).Cast<BindingStorage>();
					foreach (BindingStorage instance2 in enumerable2)
					{
						session.Delete(instance2);
					}
				}
				if (configurable != null && configurable.ObjectState != ObjectState.Deleted)
				{
					session.Delete(configurable);
				}
				return;
			}
			IL_F7:
			throw new NotSupportedException(string.Format("Unable to delete object of type '{0}'", deleteStatus.SettingType ?? "<null>"));
		}

		private static ITenantConfigurationSession GetTenantSession(Guid tenantId)
		{
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromExternalDirectoryOrganizationId(tenantId), 420, "GetTenantSession", "f:\\15.00.1497\\sources\\dev\\Hygiene\\src\\Data\\Directory\\UnifiedPolicy\\UnifiedPolicySession.cs");
		}

		private TResult GetObjectInternal<TStorage, TResult>(Guid tenantId, Guid objectId, bool includeDeletedObjects, Func<TStorage, TResult> convertFunc) where TStorage : UnifiedPolicyStorageBase, new() where TResult : PolicyConfigurationBase
		{
			TResult result = default(TResult);
			ITenantConfigurationSession tenantSession = UnifiedPolicySession.GetTenantSession(tenantId);
			QueryFilter filter = UnifiedPolicySession.CreateGetObjectQueryFilter(tenantId, objectId, includeDeletedObjects);
			TStorage tstorage = tenantSession.Find<TStorage>(filter, null, false, null).Cast<TStorage>().FirstOrDefault<TStorage>();
			if (tstorage != null)
			{
				result = convertFunc(tstorage);
			}
			return result;
		}

		private PolicyChangeBatch GetChangesInternal<TStorage, TResult>(TenantCookieCollection tenantCookies, Func<TStorage, TResult> convertFunc, bool scopeToWorkload) where TStorage : UnifiedPolicyStorageBase, new() where TResult : PolicyConfigurationBase
		{
			PolicyChangeBatch newBatch = UnifiedPolicySession.GetNewBatch<TResult>(tenantCookies);
			List<TResult> list = new List<TResult>();
			foreach (TenantCookie tenantCookie in ((IEnumerable<TenantCookie>)tenantCookies))
			{
				PolicySyncCookie policySyncCookie = PolicySyncCookie.Deserialize(tenantCookie.Cookie);
				string pagingWatermark = UnifiedPolicySession.GetPagingWatermark<TResult>(policySyncCookie);
				QueryFilter queryFilter = UnifiedPolicySession.CreateGetChangesQueryFilter(tenantCookie, pagingWatermark, tenantCookies.Workload, scopeToWorkload);
				list.AddRange(from TStorage s in UnifiedPolicySession.GetTenantSession(tenantCookie.TenantId).FindPaged<TStorage>(queryFilter, null, false, null, 1000)
				select convertFunc(s));
				bool flag;
				UnifiedPolicySession.SetPagingWatermark<TResult>(policySyncCookie, PagingHelper.GetProcessedCookie(queryFilter, out flag));
				TenantCookie tenantCookie2 = newBatch.NewCookies[tenantCookie.TenantId];
				tenantCookie2.Cookie = policySyncCookie.Serialize();
				tenantCookie2.MoreData = !flag;
			}
			newBatch.Changes = list;
			return newBatch;
		}

		private PolicyChangeBatch GetBindingChanges(TenantCookieCollection tenantCookies)
		{
			IConfigDataProvider configDataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);
			PolicyChangeBatch newBatch = UnifiedPolicySession.GetNewBatch<BindingConfiguration>(tenantCookies);
			List<BindingStorage> list = new List<BindingStorage>();
			List<ScopeStorage> list2 = new List<ScopeStorage>();
			new List<BindingConfiguration>();
			foreach (TenantCookie tenantCookie in ((IEnumerable<TenantCookie>)tenantCookies))
			{
				PolicySyncCookie policySyncCookie = PolicySyncCookie.Deserialize(tenantCookie.Cookie);
				string pagingWatermark = UnifiedPolicySession.GetPagingWatermark<BindingConfiguration>(policySyncCookie);
				string pagingWatermark2 = UnifiedPolicySession.GetPagingWatermark<ScopeConfiguration>(policySyncCookie);
				QueryFilter queryFilter = UnifiedPolicySession.CreateGetChangesQueryFilter(tenantCookie, pagingWatermark, tenantCookies.Workload, true);
				QueryFilter queryFilter2 = UnifiedPolicySession.CreateGetChangesQueryFilter(tenantCookie, pagingWatermark2, tenantCookies.Workload, true);
				list.AddRange(configDataProvider.FindPaged<BindingStorage>(queryFilter, null, false, null, 1000).Cast<BindingStorage>());
				list2.AddRange(configDataProvider.FindPaged<ScopeStorage>(queryFilter2, null, false, null, 1000).Cast<ScopeStorage>());
				bool flag;
				UnifiedPolicySession.SetPagingWatermark<BindingConfiguration>(policySyncCookie, PagingHelper.GetProcessedCookie(queryFilter, out flag));
				bool flag2;
				UnifiedPolicySession.SetPagingWatermark<ScopeConfiguration>(policySyncCookie, PagingHelper.GetProcessedCookie(queryFilter2, out flag2));
				TenantCookie tenantCookie2 = newBatch.NewCookies[tenantCookie.TenantId];
				tenantCookie2.Cookie = policySyncCookie.Serialize();
				tenantCookie2.MoreData = (!flag || !flag2);
			}
			using (List<ScopeStorage>.Enumerator enumerator2 = list2.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					UnifiedPolicySession.<>c__DisplayClass19 CS$<>8__locals1 = new UnifiedPolicySession.<>c__DisplayClass19();
					CS$<>8__locals1.scope = enumerator2.Current;
					Guid parentBindingId = Guid.Parse((string)CS$<>8__locals1.scope[UnifiedPolicyStorageBaseSchema.ContainerProp]);
					BindingStorage bindingStorage = list.FirstOrDefault((BindingStorage b) => b.Id.ObjectGuid == parentBindingId && b.OrganizationalUnitRoot.ObjectGuid == CS$<>8__locals1.scope.OrganizationalUnitRoot.ObjectGuid);
					if (bindingStorage == null)
					{
						bindingStorage = configDataProvider.Find<BindingStorage>(UnifiedPolicySession.CreateGetObjectQueryFilter(CS$<>8__locals1.scope.OrganizationalUnitRoot.ObjectGuid, parentBindingId, true), null, false, null).Cast<BindingStorage>().FirstOrDefault<BindingStorage>();
						list.Add(bindingStorage);
					}
					if (CS$<>8__locals1.scope.ObjectState == ObjectState.Deleted)
					{
						bindingStorage.RemovedScopes.Add(CS$<>8__locals1.scope);
					}
					else
					{
						bindingStorage.AppliedScopes.Add(CS$<>8__locals1.scope);
					}
				}
			}
			newBatch.Changes = from r in list
			select UnifiedPolicyStorageFactory.FromBindingStorage(r);
			return newBatch;
		}

		private const int PerTenantPageSize = 1000;

		public static HygienePropertyDefinition DeletedObjectThresholdQueryProperty = new HygienePropertyDefinition("DeletedObjectThreshold", typeof(DateTime?));

		public static HygienePropertyDefinition DeltaChangesQueryProperty = new HygienePropertyDefinition("DeltaChanges", typeof(bool?));

		public static HygienePropertyDefinition FlagsProperty = new HygienePropertyDefinition("Flags", typeof(int?));

		public static HygienePropertyDefinition ForceChangeProperty = new HygienePropertyDefinition("ForceChange", typeof(DateTime?));
	}
}
