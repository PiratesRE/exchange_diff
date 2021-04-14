using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HygieneData;
using Microsoft.Exchange.Hygiene.Cache.Data;
using Microsoft.Exchange.Hygiene.Common.Diagnosis;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class DomainSession
	{
		public DomainSession() : this(CombGuidGenerator.NewGuid(), "Unknown", CacheFailoverMode.DatabaseOnly, null)
		{
		}

		public DomainSession(CacheFailoverMode dataAccessType) : this(CombGuidGenerator.NewGuid(), "Unknown", dataAccessType, null)
		{
		}

		public DomainSession(CacheFailoverMode dataAccessType, Tracking profiler) : this(CombGuidGenerator.NewGuid(), "Unknown", dataAccessType, profiler)
		{
		}

		public DomainSession(Guid transactionId) : this(transactionId, "Unknown", CacheFailoverMode.DatabaseOnly, null)
		{
		}

		public DomainSession(Guid transactionId, string callerId = "Unknown", CacheFailoverMode dataAccessType = CacheFailoverMode.DatabaseOnly, Tracking profiler = null)
		{
			if (callerId == null)
			{
				throw new ArgumentNullException("callerId");
			}
			this.transactionId = transactionId;
			this.callerId = callerId;
			this.DataAcccess = dataAccessType;
			this.Profiler = profiler;
			if (CacheFailoverMode.DatabaseOnly == this.DataAcccess)
			{
				this.dataProvider = this.WebStoreDataProvider;
			}
			else
			{
				this.dataProvider = this.CacheFallbackDataProvider;
			}
			this.TraceInformation("DomainSession.DataProvider type={0}, dataAccessType={1}", new object[]
			{
				this.dataProvider.GetType().Name,
				this.DataAcccess
			});
		}

		public Tracking Profiler { get; private set; }

		public CacheFailoverMode DataAcccess { get; private set; }

		public string ContextPrefix
		{
			get
			{
				return string.Format("ThreadId: {0}, TransactionId: {1}, ", Environment.CurrentManagedThreadId, this.transactionId);
			}
		}

		internal IConfigDataProvider DefaultDataProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		private IConfigDataProvider CacheDataProvider
		{
			get
			{
				if (this.cacheDataProvider == null)
				{
					this.cacheDataProvider = ConfigDataProviderFactory.CacheDefault.Create(DatabaseType.Domain);
				}
				return this.cacheDataProvider;
			}
		}

		private IConfigDataProvider CacheFallbackDataProvider
		{
			get
			{
				if (this.cacheFallbackDataProvider == null)
				{
					this.cacheFallbackDataProvider = ConfigDataProviderFactory.CacheFallbackDefault.Create(DatabaseType.Domain);
				}
				return this.cacheFallbackDataProvider;
			}
		}

		private IConfigDataProvider WebStoreDataProvider
		{
			get
			{
				if (this.webStoreDataProvider == null)
				{
					this.webStoreDataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Domain);
				}
				return this.webStoreDataProvider;
			}
		}

		public static string TrackingDictionaryKey(DomainSession.CacheTrackingCategory category, DomainSession.CacheTrackingTypes type)
		{
			return string.Format("{0}_{1}", category, type);
		}

		public static SqlPropertyDefinition[] FindPropertyDefinition(string entityName = null, string propertyName = null, int? entityId = null, int? propertyId = null)
		{
			ObjectCache @default = MemoryCache.Default;
			string key = string.Format("DBs={0};Entity={1};Prop={2};EntityID={3};PropID={4}", new object[]
			{
				DatabaseType.Domain,
				entityName,
				propertyName,
				entityId,
				propertyId
			});
			SqlPropertyDefinition[] array = @default[key] as SqlPropertyDefinition[];
			if (array == null)
			{
				array = HygieneSession.FindPropertyDefinition(DatabaseType.Domain, entityName, propertyName, entityId, propertyId);
				if (array != null)
				{
					@default.Set(key, array, DomainSession.propDefinitioncachItemPolicy, null);
				}
				ExTraceGlobals.DomainSessionTracer.TraceInformation<string, string>(0, 0L, "DomainSession.FindPropertyDefinition EntityName={0}, PropName={1}: Bag read from DB", entityName, propertyName);
			}
			else
			{
				ExTraceGlobals.DomainSessionTracer.TraceInformation<string, string>(0, 0L, "DomainSession.FindPropertyDefinition EntityName={0}, PropName={1}: Bag read from Cache", entityName, propertyName);
			}
			return array;
		}

		public static void SavePropertyDefinition(SqlPropertyDefinition propertyDefinition)
		{
			HygieneSession.SavePropertyDefinition(DatabaseType.Domain, propertyDefinition);
		}

		public static void DeletePropertyDefinition(SqlPropertyDefinition propertyDefinition)
		{
			HygieneSession.DeletePropertyDefinition(DatabaseType.Domain, propertyDefinition);
		}

		public void Save(IConfigurable obj)
		{
			this.CheckInputType(obj);
			this.Track(this.GetTrackingTag(true, "Sav", new Type[]
			{
				obj.GetType()
			}));
			DomainSession.AddIdentifier(obj as IPropertyBag);
			this.ApplyAuditProperties(obj);
			this.TraceDebug("Calling Save on {0}", new object[]
			{
				obj
			});
			this.DefaultDataProvider.Save(obj);
			this.Track(this.GetTrackingTag(false, "Sav", new Type[]
			{
				obj.GetType()
			}));
		}

		public void SaveDomainAndTargetService(DomainTargetEnvironment domainTargetEnvironment, TargetService targetService)
		{
			this.TraceDebug("Calling SaveDomainAndTargetService");
			this.Track(this.GetTrackingTag(true, "Sav", new Type[]
			{
				typeof(DomainTargetEnvironment),
				typeof(TargetService)
			}));
			if (domainTargetEnvironment == null)
			{
				throw new ArgumentNullException("domainTargetEnvironment");
			}
			if (targetService == null)
			{
				throw new ArgumentNullException("targetService");
			}
			this.ValidateDomainRecordsMatch(domainTargetEnvironment, targetService);
			this.Run(false, "SaveDomainAndTargetService", delegate
			{
				this.Save(domainTargetEnvironment);
				targetService.DomainKey = domainTargetEnvironment.DomainKey;
				this.Save(targetService);
			});
			this.Track(this.GetTrackingTag(false, "Sav", new Type[]
			{
				typeof(DomainTargetEnvironment),
				typeof(TargetService)
			}));
		}

		public void Delete(IConfigurable obj)
		{
			this.CheckInputType(obj);
			this.Track(this.GetTrackingTag(true, "Del", new Type[]
			{
				obj.GetType()
			}));
			this.ApplyAuditProperties(obj);
			this.TraceDebug("Calling Delete on {0}", new object[]
			{
				obj
			});
			this.DefaultDataProvider.Delete(obj);
			this.Track(this.GetTrackingTag(false, "Del", new Type[]
			{
				obj.GetType()
			}));
		}

		public void DeleteTenantAndDomains(Guid tenantId, IEnumerable<int> tenantPropEntityIds, IEnumerable<int> domainPropEntityIds)
		{
			this.Track(this.GetTrackingTag(true, "Del", new Type[]
			{
				typeof(TenantTargetEnvironment),
				typeof(DomainTargetEnvironment)
			}));
			if (tenantPropEntityIds == null)
			{
				throw new ArgumentNullException("tenantPropEntityIds");
			}
			if (domainPropEntityIds == null)
			{
				throw new ArgumentNullException("domainPropEntityIds");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling DeleteTenantAndDomains with tenantId={0}, tenantPropEntityIds={1}, domainPropEntityIds={2}  ", new object[]
				{
					tenantId,
					tenantPropEntityIds.ConvertToString<int>(),
					domainPropEntityIds.ConvertToString<int>()
				});
			}
			TenantTargetEnvironment tenant = new TenantTargetEnvironment
			{
				TenantId = tenantId
			};
			tenant.Properties = new Dictionary<int, Dictionary<int, string>>();
			foreach (int key in tenantPropEntityIds)
			{
				if (!tenant.Properties.ContainsKey(key))
				{
					tenant.Properties.Add(key, null);
				}
			}
			Dictionary<int, Dictionary<int, string>> domainProperties = new Dictionary<int, Dictionary<int, string>>();
			foreach (int key2 in domainPropEntityIds)
			{
				if (!domainProperties.ContainsKey(key2))
				{
					domainProperties.Add(key2, null);
				}
			}
			this.Run(false, "DeleteTenantAndDomains", delegate
			{
				object[] allPhysicalPartitions = ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions();
				Task[] array = new Task[allPhysicalPartitions.Length * 2 + 1];
				int num = 0;
				array[num++] = Task.Factory.StartNew(delegate()
				{
					this.Delete(tenant);
				});
				object[] array2 = allPhysicalPartitions;
				for (int i = 0; i < array2.Length; i++)
				{
					object value = array2[i];
					DomainTargetEnvironment domain = new DomainTargetEnvironment
					{
						TenantId = tenant.TenantId,
						Properties = domainProperties
					};
					domain[DalHelper.PhysicalInstanceKeyProp] = value;
					array[num++] = Task.Factory.StartNew(delegate()
					{
						this.Delete(domain);
					});
					TargetService targetService = new TargetService
					{
						TenantId = tenant.TenantId,
						Properties = domainProperties
					};
					targetService[DalHelper.PhysicalInstanceKeyProp] = value;
					array[num++] = Task.Factory.StartNew(delegate()
					{
						this.Delete(targetService);
					});
				}
				Task.WaitAll(array);
			});
			this.Track(this.GetTrackingTag(false, "Del", new Type[]
			{
				typeof(TenantTargetEnvironment),
				typeof(DomainTargetEnvironment)
			}));
		}

		public void DeleteDomainAndTargetService(DomainTargetEnvironment domainTargetEnvironment, TargetService targetService)
		{
			this.Track(this.GetTrackingTag(true, "Del", new Type[]
			{
				typeof(DomainTargetEnvironment),
				typeof(TargetService)
			}));
			this.TraceDebug("Calling DeleteDomainAndTargetService");
			if (domainTargetEnvironment == null && targetService == null)
			{
				throw new ArgumentNullException("domainTargetEnvironment && targetService");
			}
			if (domainTargetEnvironment == null)
			{
				this.TraceDebug("domainTargetEnvironment is null");
				if (string.IsNullOrWhiteSpace(targetService.DomainName))
				{
					TargetService targetService2 = this.FindTargetService(targetService.DomainKey);
					targetService.DomainName = ((targetService2 != null) ? targetService2.DomainName : null);
					this.TraceDebug("DomainName= {0} fetched from targetservice in DB", new object[]
					{
						targetService.DomainName
					});
				}
				else
				{
					this.TraceDebug("using DomainName= {0} specified in targetservice", new object[]
					{
						targetService.DomainName
					});
				}
				if (!string.IsNullOrWhiteSpace(targetService.DomainName))
				{
					domainTargetEnvironment = new DomainTargetEnvironment
					{
						DomainName = targetService.DomainName,
						TenantId = targetService.TenantId,
						Properties = targetService.Properties
					};
				}
				else
				{
					this.TraceDebug("could not get DomainName from targetservice, skipping DomainTargetEnvironment delete");
				}
			}
			else if (targetService == null)
			{
				this.TraceDebug("targetService is null");
				if (string.IsNullOrWhiteSpace(domainTargetEnvironment.DomainKey))
				{
					DomainTargetEnvironment domainTargetEnvironment2 = this.FindDomainTargetEnvironment(domainTargetEnvironment.DomainName);
					domainTargetEnvironment.DomainKey = ((domainTargetEnvironment2 != null) ? domainTargetEnvironment2.DomainKey : null);
					this.TraceDebug("DomainKey= {0} fetched from domainTargetEnvironment in DB", new object[]
					{
						domainTargetEnvironment.DomainKey
					});
				}
				else
				{
					this.TraceDebug("using DomainKey= {0} specified in domainTargetEnvironment", new object[]
					{
						domainTargetEnvironment.DomainKey
					});
				}
				if (!string.IsNullOrWhiteSpace(domainTargetEnvironment.DomainKey))
				{
					targetService = new TargetService
					{
						DomainKey = domainTargetEnvironment.DomainKey,
						DomainName = domainTargetEnvironment.DomainName,
						TenantId = domainTargetEnvironment.TenantId,
						Properties = domainTargetEnvironment.Properties
					};
				}
				else
				{
					this.TraceDebug("could not get DomainKey from domainTargetEnvironment, skipping targetService delete");
				}
			}
			else
			{
				this.ValidateDomainRecordsMatch(domainTargetEnvironment, targetService);
			}
			this.Run(false, "DeleteDomainAndTargetService", delegate
			{
				if (targetService != null)
				{
					this.Delete(targetService);
				}
				if (domainTargetEnvironment != null)
				{
					this.Delete(domainTargetEnvironment);
				}
			});
			this.Track(this.GetTrackingTag(false, "Del", new Type[]
			{
				typeof(DomainTargetEnvironment),
				typeof(TargetService)
			}));
		}

		public IEnumerable<DomainTargetEnvironment> FindDomainTargetEnvironments(IEnumerable<string> domainNames)
		{
			if (domainNames == null)
			{
				throw new ArgumentNullException("domainNames");
			}
			IEnumerable<string> enumerable = from domainName in domainNames.Distinct(StringComparer.OrdinalIgnoreCase)
			where !string.IsNullOrWhiteSpace(domainName)
			select domainName;
			if (enumerable.Count<string>() == 0)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "domainNames");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling FindDomainTargetEnvironments, domainNames={0}", new object[]
				{
					enumerable.ConvertToString<string>()
				});
			}
			return this.Find<DomainTargetEnvironment, string>(enumerable, DomainSchema.DomainNames, DomainSchema.DomainName);
		}

		public IEnumerable<TargetService> FindTargetServices(IEnumerable<string> domainKeys)
		{
			if (domainKeys == null)
			{
				throw new ArgumentNullException("domainKeys");
			}
			IEnumerable<string> enumerable = from domainKey in domainKeys.Distinct(StringComparer.OrdinalIgnoreCase)
			where !string.IsNullOrWhiteSpace(domainKey)
			select domainKey;
			if (enumerable.Count<string>() == 0)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "domainKeys");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling FindTargetServices, domainKeys={0}", new object[]
				{
					enumerable.ConvertToString<string>()
				});
			}
			return this.Find<TargetService, string>(enumerable, DomainSchema.DomainKeys, DomainSchema.DomainKey);
		}

		public IEnumerable<TenantTargetEnvironment> FindTenantTargetEnvironments(IEnumerable<Guid> tenantIds)
		{
			if (tenantIds == null)
			{
				throw new ArgumentNullException("tenantIds");
			}
			IEnumerable<Guid> enumerable = from tenantId in tenantIds.Distinct<Guid>()
			where tenantId != Guid.Empty
			select tenantId;
			if (enumerable.Count<Guid>() == 0)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "tenantIds");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling FindTenantTargetEnvironments, tenantIds={0}", new object[]
				{
					enumerable.ConvertToString<Guid>()
				});
			}
			return this.Find<TenantTargetEnvironment, Guid>(enumerable, DomainSchema.TenantIds, DomainSchema.TenantId);
		}

		public IEnumerable<DomainTargetEnvironment> FindDomainTargetEnvironmentsByTenantIds(IEnumerable<Guid> tenantIds)
		{
			if (tenantIds == null)
			{
				throw new ArgumentNullException("tenantIds");
			}
			IEnumerable<Guid> enumerable = from tenantId in tenantIds.Distinct<Guid>()
			where tenantId != Guid.Empty
			select tenantId;
			if (!enumerable.Any<Guid>())
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "tenantIds");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling FindDomainTargetEnvironmentsByTenantIds, tenantIds={0}", new object[]
				{
					enumerable.ConvertToString<Guid>()
				});
			}
			List<DomainTargetEnvironment> list = new List<DomainTargetEnvironment>();
			foreach (int num in this.GetAllPhysicalPartitions())
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.TenantIds, new MultiValuedProperty<Guid>(enumerable));
				list.AddRange(this.Find<DomainTargetEnvironment>(filter, this.WebStoreDataProvider, num));
			}
			return list;
		}

		public IEnumerable<TargetService> FindTargetServicesByTenantIds(IEnumerable<Guid> tenantIds)
		{
			if (tenantIds == null)
			{
				throw new ArgumentNullException("tenantIds");
			}
			IEnumerable<Guid> enumerable = from tenantId in tenantIds.Distinct<Guid>()
			where tenantId != Guid.Empty
			select tenantId;
			if (!enumerable.Any<Guid>())
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "tenantIds");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling FindTargetServicesByTenantIds, tenantIds={0}", new object[]
				{
					enumerable.ConvertToString<Guid>()
				});
			}
			List<TargetService> list = new List<TargetService>();
			foreach (int num in this.GetAllPhysicalPartitions())
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.TenantIds, new MultiValuedProperty<Guid>(enumerable));
				list.AddRange(this.Find<TargetService>(filter, this.WebStoreDataProvider, num));
			}
			return list;
		}

		public DomainTargetEnvironment FindDomainTargetEnvironment(string domainName)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			this.TraceDebug("Calling FindDomainTargetEnvironment, domainName={0}", new object[]
			{
				domainName
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainName, domainName);
			return this.Find<DomainTargetEnvironment>(filter, null, null).FirstOrDefault<DomainTargetEnvironment>();
		}

		public IEnumerable<DIDomainTargetEnvironment> DIFindDomainTargetEnvironment(string domainName)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			this.TraceDebug("Calling DIFindDomainTargetEnvironment, domainName={0}", new object[]
			{
				domainName
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainName, domainName);
			return this.Find<DIDomainTargetEnvironment>(filter, null, null);
		}

		public IEnumerable<DomainTargetEnvironment> FindDomainTargetEnvironmentsByTenantId(Guid tenantId)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException("TenantId cannot be Guid.Empty", "tenantId");
			}
			this.TraceDebug("Calling FindDomainTargetEnvironmentsByTenantId, tenantId={0}", new object[]
			{
				tenantId
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId);
			return this.FindFromAllPartitions<DomainTargetEnvironment>(filter);
		}

		public IEnumerable<DIDomainTargetEnvironment> DIFindDomainTargetEnvironmentsByTenantId(Guid tenantId)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException("TenantId cannot be Guid.Empty", "tenantId");
			}
			this.TraceDebug("Calling DIFindDomainTargetEnvironmentsByTenantId, tenantId={0}", new object[]
			{
				tenantId
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId);
			return this.FindFromAllPartitions<DIDomainTargetEnvironment>(filter);
		}

		public TargetService FindTargetService(string domainKey)
		{
			if (string.IsNullOrWhiteSpace(domainKey))
			{
				throw new ArgumentNullException("domainKey");
			}
			this.TraceDebug("Calling FindTargetService, domainKey={0}", new object[]
			{
				domainKey
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainKey, domainKey);
			return this.Find<TargetService>(filter, null, null).FirstOrDefault<TargetService>();
		}

		public IEnumerable<DITargetService> DIFindTargetService(string domainKey)
		{
			if (string.IsNullOrWhiteSpace(domainKey))
			{
				throw new ArgumentNullException("domainKey");
			}
			this.TraceDebug("Calling DIFindTargetService, domainKey={0}", new object[]
			{
				domainKey
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainKey, domainKey);
			return this.Find<DITargetService>(filter, null, null);
		}

		public IEnumerable<TargetService> FindTargetServicesByTenantId(Guid tenantId)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException("TenantId cannot be Guid.Empty", "tenantId");
			}
			this.TraceDebug("Calling FindTargetServicesByTenantId, tenantId={0}", new object[]
			{
				tenantId
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId);
			return this.FindFromAllPartitions<TargetService>(filter);
		}

		public UserTargetEnvironment FindUserEnvironment(string userKey)
		{
			if (string.IsNullOrWhiteSpace(userKey))
			{
				throw new ArgumentNullException("userKey");
			}
			this.TraceDebug("Calling FindUserTargetEnvironment, userKey={0}", new object[]
			{
				userKey
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.UserKey, userKey);
			return this.Find<UserTargetEnvironment>(filter, null, null).FirstOrDefault<UserTargetEnvironment>();
		}

		public TenantTargetEnvironment FindTenantTargetEnvironment(Guid tenantId)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyGuid, "tenantId");
			}
			this.TraceDebug("Calling FindTenantTargetEnvironment, tenantId={0}", new object[]
			{
				tenantId
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.TenantId, tenantId);
			return this.Find<TenantTargetEnvironment>(filter, null, null).FirstOrDefault<TenantTargetEnvironment>();
		}

		public IEnumerable<TenantTargetEnvironment> FindTenantTargetEnvironmentCopies(Guid tenantId, int copiesToRead = 2)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyGuid, "tenantId");
			}
			this.TraceDebug("Calling FindTenantTargetEnvironmentCopies, tenantId={0}, copiesToRead={1}", new object[]
			{
				tenantId,
				copiesToRead
			});
			int physicalPartitionCopyCount = this.GetPhysicalPartitionCopyCount(this.GetPhysicalInstanceId(tenantId));
			if (copiesToRead < 1 || (physicalPartitionCopyCount > 1 && physicalPartitionCopyCount < copiesToRead))
			{
				throw new ArgumentOutOfRangeException("copiesToRead", string.Format("The minimum allowed value is 1 and maximum allowed value is {0}", physicalPartitionCopyCount));
			}
			List<TransientDALException> list = null;
			List<TenantTargetEnvironment> list2 = new List<TenantTargetEnvironment>();
			int num = 0;
			int num2 = 0;
			int num3 = DomainSession.randomGenerator.Next(0, physicalPartitionCopyCount);
			do
			{
				try
				{
					num3 = ++num3 % physicalPartitionCopyCount;
					QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId),
						new ComparisonFilter(ComparisonOperator.Equal, DalHelper.FssCopyIdProp, num3)
					});
					list2.AddRange(this.Find<TenantTargetEnvironment>(filter, this.WebStoreDataProvider, null));
					num++;
				}
				catch (TransientDALException item)
				{
					if (list == null)
					{
						list = new List<TransientDALException>();
					}
					list.Add(item);
				}
			}
			while (++num2 < physicalPartitionCopyCount && num < copiesToRead);
			if (list != null && list.Count == physicalPartitionCopyCount)
			{
				throw new AggregateException(list);
			}
			return list2;
		}

		public IEnumerable<DITenantTargetEnvironment> DIFindTenantTargetEnvironment(Guid tenantId)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyGuid, "tenantId");
			}
			this.TraceDebug("Calling DIFindTenantTargetEnvironment, tenantId={0}", new object[]
			{
				tenantId
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.TenantId, tenantId);
			return this.Find<DITenantTargetEnvironment>(filter, null, null);
		}

		public IEnumerable<NsResourceRecord> FindNsResourceRecord(string domainName)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			this.TraceDebug("Calling FindNsResourceRecord, domainName={0}", new object[]
			{
				domainName
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainName, domainName);
			return this.Find<NsResourceRecord>(filter, null, null);
		}

		public SoaResourceRecord FindSoaResourceRecord(string domainName)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			this.TraceDebug("Calling FindSoaResourceRecord, domainName={0}", new object[]
			{
				domainName
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainName, domainName);
			return this.Find<SoaResourceRecord>(filter, null, null).FirstOrDefault<SoaResourceRecord>();
		}

		public Zone FindZone(string domainName)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			this.TraceDebug("Calling FindZone, domainName={0}", new object[]
			{
				domainName
			});
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DomainSchema.DomainName, domainName);
			return this.Find<Zone>(filter, null, null).FirstOrDefault<Zone>();
		}

		public IEnumerable<Zone> FindZoneAll()
		{
			this.TraceDebug("Calling FindZoneAll");
			return this.Find<Zone>(null, null, null);
		}

		public bool UpdateTargetServiceByDomainKey(string domainKey, Dictionary<int, Dictionary<int, string>> properties)
		{
			this.Track(this.GetTrackingTag(true, "Upd", new Type[]
			{
				typeof(TargetService)
			}));
			if (string.IsNullOrWhiteSpace(domainKey))
			{
				throw new ArgumentNullException("domainKey");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling UpdateTargetServiceByDomainKey, domainKey={0}, properties(EntityId:PropertyId:PropertyValue)={1}", new object[]
				{
					domainKey,
					properties.ConvertToString()
				});
			}
			TargetService targetService = this.FindTargetService(domainKey);
			if (targetService != null)
			{
				targetService.Properties = properties;
				this.Save(targetService);
				this.Track(this.GetTrackingTag(false, "Upd", new Type[]
				{
					typeof(TargetService)
				}));
				return true;
			}
			this.Track(this.GetTrackingTag(false, "Upd", new Type[]
			{
				typeof(TargetService)
			}));
			this.TraceDebug("TargetService not found");
			return false;
		}

		public bool UpdateDomainKey(string domainName, string newDomainKey)
		{
			if (string.IsNullOrWhiteSpace(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			if (string.IsNullOrWhiteSpace(newDomainKey))
			{
				throw new ArgumentNullException("newDomainKey");
			}
			this.Track(this.GetTrackingTag(true, "Upd", new Type[]
			{
				typeof(DomainTargetEnvironment),
				typeof(TargetService)
			}));
			try
			{
				this.TraceDebug("Calling UpdateDomainKey, domainName={0}, newDomainKey={1}", new object[]
				{
					domainName,
					newDomainKey
				});
				DomainTargetEnvironment domainTargetEnvironment = this.FindDomainTargetEnvironment(domainName);
				if (domainTargetEnvironment == null)
				{
					this.TraceDebug("DomainTargetEnvironment not found, domainName:{0}", new object[]
					{
						domainName
					});
					return false;
				}
				string domainKey = domainTargetEnvironment.DomainKey;
				TargetService oldTargetService = this.FindTargetService(domainKey);
				if (oldTargetService == null)
				{
					this.TraceDebug("TargetService not found, domainKey:{0}", new object[]
					{
						domainKey
					});
					return false;
				}
				if (newDomainKey.Equals(domainKey, StringComparison.OrdinalIgnoreCase))
				{
					this.TraceDebug("Domain:{0} already has the new domainKey:{1}", new object[]
					{
						domainName,
						newDomainKey
					});
					return true;
				}
				TargetService newTargetService = new TargetService
				{
					DomainName = oldTargetService.DomainName,
					DomainKey = newDomainKey,
					TenantId = oldTargetService.TenantId,
					Properties = oldTargetService.Properties
				};
				domainTargetEnvironment.DomainKey = newDomainKey;
				domainTargetEnvironment[DomainSchema.UpdateDomainKey] = true;
				this.Run(false, "UpdateDomainKey", delegate
				{
					this.Save(newTargetService);
					this.Save(domainTargetEnvironment);
					this.Delete(oldTargetService);
				});
			}
			finally
			{
				this.Track(this.GetTrackingTag(false, "Upd", new Type[]
				{
					typeof(DomainTargetEnvironment),
					typeof(TargetService)
				}));
			}
			return true;
		}

		public IEnumerable<string> UpdateTargetServiceByTenantId(Guid tenantId, Dictionary<int, Dictionary<int, string>> properties)
		{
			this.Track(this.GetTrackingTag(true, "Upd", new Type[]
			{
				typeof(TargetService)
			}));
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyGuid, "tenantId");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Calling UpdateTargetServiceByTenantId, tenantId={0}, properties(EntityId:PropertyId:PropertyValue)={1}", new object[]
				{
					tenantId,
					properties.ConvertToString()
				});
			}
			TargetServiceByTenantId targetService = new TargetServiceByTenantId
			{
				TenantId = tenantId,
				Properties = properties
			};
			this.Run(false, "UpdateTargetServiceByTenantId", delegate
			{
				this.Save(targetService);
			});
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("Updated domainkeys = {0}", new object[]
				{
					targetService.UpdatedDomainKeys.ConvertToString<string>()
				});
			}
			this.Track(this.GetTrackingTag(false, "Upd", new Type[]
			{
				typeof(TargetService)
			}));
			return targetService.UpdatedDomainKeys;
		}

		public void UndeleteTenant(Guid tenantId, DateTime deletedDatetime)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException("The tenantId must not be empty.");
			}
			this.DefaultDataProvider.Save(new TenantUndeleteRequest
			{
				TenantId = tenantId,
				DeletedDatetime = deletedDatetime
			});
		}

		private static void AddIdentifier(IPropertyBag obj)
		{
			HygienePropertyDefinition propertyDefinition = null;
			if (DomainSession.IdentifierMap.TryGetValue(obj.GetType(), out propertyDefinition))
			{
				Guid a = (Guid)obj[propertyDefinition];
				if (a == Guid.Empty)
				{
					obj[propertyDefinition] = DomainSession.GenerateIdentifier();
				}
			}
		}

		private static Guid GenerateIdentifier()
		{
			return CombGuidGenerator.NewGuid();
		}

		private void ApplyAuditProperties(IConfigurable configurable)
		{
			IPropertyBag propertyBag = configurable as IPropertyBag;
			AuditHelper.ApplyAuditProperties(propertyBag, this.transactionId, this.callerId);
		}

		private void CheckInputType(IConfigurable obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("DomainSession input");
			}
			if (!(obj is Zone) && !(obj is NsResourceRecord) && !(obj is SoaResourceRecord) && !(obj is TargetService) && !(obj is TargetServiceByTenantId) && !(obj is TenantTargetEnvironment) && !(obj is UserTargetEnvironment) && !(obj is DomainTargetEnvironment))
			{
				string name = obj.GetType().Name;
				this.TraceWarning("Invalid object type = {0}", new object[]
				{
					name
				});
				throw new InvalidObjectTypeForSessionException(HygieneDataStrings.ErrorInvalidObjectTypeForSession("DomainSession", name));
			}
		}

		private void ValidateDomainRecordsMatch(DomainTargetEnvironment domainTargetEnvironment, TargetService targetService)
		{
			if (!string.IsNullOrWhiteSpace(domainTargetEnvironment.DomainKey) && !string.IsNullOrWhiteSpace(targetService.DomainKey) && string.Compare(domainTargetEnvironment.DomainKey, targetService.DomainKey, StringComparison.OrdinalIgnoreCase) != 0)
			{
				this.TraceWarning("domainKeys don't match domainTargetEnvironment.DomainKey = {0}, targetService.DomainKey = {1}", new object[]
				{
					domainTargetEnvironment.DomainKey,
					targetService.DomainKey
				});
				throw new ArgumentException(HygieneDataStrings.ErrorInvalidArgumentDomainKeyMismatch(domainTargetEnvironment.DomainKey, targetService.DomainKey));
			}
			if (!string.IsNullOrWhiteSpace(domainTargetEnvironment.DomainName) && !string.IsNullOrWhiteSpace(targetService.DomainName) && string.Compare(domainTargetEnvironment.DomainName, targetService.DomainName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				this.TraceWarning("domainNames don't match domainTargetEnvironment.DomainName = {0}, targetService.DomainName = {1}", new object[]
				{
					domainTargetEnvironment.DomainName,
					targetService.DomainName
				});
				throw new ArgumentException(HygieneDataStrings.ErrorInvalidArgumentDomainNameMismatch(domainTargetEnvironment.DomainName, targetService.DomainName));
			}
			if (domainTargetEnvironment.TenantId != targetService.TenantId)
			{
				this.TraceWarning("tenantIds don't match domainTargetEnvironment.TenantId = {0}, targetService.TenantId = {1}", new object[]
				{
					domainTargetEnvironment.TenantId,
					targetService.TenantId
				});
				throw new ArgumentException(HygieneDataStrings.ErrorInvalidArgumentTenantIdMismatch(domainTargetEnvironment.TenantId, targetService.TenantId));
			}
		}

		private void GetCacheStatus(Dictionary<PropertyDefinition, object> cacheOutputBag, IConfigurable[] results, out int cacheHit, out CachePrimingState primingState, out CacheFailoverMode failoverMode, out int bloomFilterHit, out int inMemoryHit)
		{
			primingState = CachePrimingState.Unknown;
			failoverMode = CacheFailoverMode.Default;
			bloomFilterHit = -1;
			cacheHit = -1;
			inMemoryHit = -1;
			if (cacheOutputBag.ContainsKey(DalHelper.CachePrimingStateProp))
			{
				primingState = (CachePrimingState)cacheOutputBag[DalHelper.CachePrimingStateProp];
			}
			if (cacheOutputBag.ContainsKey(DalHelper.CacheFailoverModeProp))
			{
				failoverMode = (CacheFailoverMode)cacheOutputBag[DalHelper.CacheFailoverModeProp];
			}
			if (cacheOutputBag.ContainsKey(DalHelper.BloomHitProp))
			{
				bloomFilterHit = (((bool)cacheOutputBag[DalHelper.BloomHitProp]) ? 1 : 0);
			}
			if (cacheOutputBag.ContainsKey(DalHelper.InMemoryCacheHitProp))
			{
				inMemoryHit = (((bool)cacheOutputBag[DalHelper.InMemoryCacheHitProp]) ? 1 : 0);
			}
			if (results != null && results.Length > 0 && results[0] != null)
			{
				IPropertyBag propertyBag = results[0] as IPropertyBag;
				if (propertyBag != null)
				{
					cacheHit = (((bool)propertyBag[DalHelper.CacheHitProp]) ? 1 : 0);
				}
			}
		}

		private IEnumerable<T> Find<T>(QueryFilter filter, IConfigDataProvider alternateDataProvider = null, object physicalInstanceId = null) where T : IConfigurable, new()
		{
			Dictionary<PropertyDefinition, object> dictionary = null;
			this.Track(this.GetTrackingTag(true, "Fnd", physicalInstanceId, new Type[]
			{
				typeof(T)
			}));
			if (physicalInstanceId != null)
			{
				filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, physicalInstanceId)
				});
			}
			IConfigDataProvider configDataProvider = (alternateDataProvider != null) ? alternateDataProvider : this.DefaultDataProvider;
			if (configDataProvider is ICachePrimingInfo)
			{
				dictionary = new Dictionary<PropertyDefinition, object>();
				filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.CacheFailoverModeProp, this.DataAcccess),
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.CacheOutputBagProp, dictionary)
				});
			}
			IConfigurable[] array = configDataProvider.Find<T>(filter, null, true, null);
			if (configDataProvider is ICachePrimingInfo)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				CachePrimingState cachePrimingState;
				CacheFailoverMode cacheFailoverMode;
				this.GetCacheStatus(dictionary, array, out num, out cachePrimingState, out cacheFailoverMode, out num2, out num3);
				if (TraceHelper.IsDebugTraceEnabled())
				{
					this.TraceDebug("Find type={0} result={1}, cacheStatus={2}, hitCache={3} mode={4} bloom={5} mem={6}", new object[]
					{
						typeof(T).Name,
						array.ConvertToString<IConfigurable>(),
						cachePrimingState,
						num,
						cacheFailoverMode,
						num2,
						num3
					});
				}
				this.Track(this.GetCacheTrackingType(typeof(T)), this.GetTrackingTag(false, "Fnd", physicalInstanceId, new Type[]
				{
					typeof(T)
				}), num, cachePrimingState, cacheFailoverMode, num2, num3, false);
			}
			else
			{
				if (TraceHelper.IsDebugTraceEnabled())
				{
					this.TraceDebug("Find type={0} result={1}", new object[]
					{
						typeof(T).Name,
						array.ConvertToString<IConfigurable>()
					});
				}
				this.Track(this.GetCacheTrackingType(typeof(T)), this.GetTrackingTag(false, "Fnd", physicalInstanceId, new Type[]
				{
					typeof(T)
				}), -1, CachePrimingState.Unknown, CacheFailoverMode.DatabaseOnly, -1, -1, this.DataAcccess == CacheFailoverMode.DatabaseOnly);
			}
			return array.Cast<T>();
		}

		private IEnumerable<TReturn> Find<TReturn, TInput>(IEnumerable<TInput> lookupValues, HygienePropertyDefinition lookupCollectionDefinition, HygienePropertyDefinition lookupValueDefinition) where TReturn : IConfigurable, IPropertyBag, new()
		{
			this.Track(this.GetTrackingTag(true, "FndB", new Type[]
			{
				typeof(TReturn)
			}));
			List<TReturn> list = new List<TReturn>();
			CacheFailoverMode cacheFailoverMode = this.DataAcccess;
			if (this.DataAcccess != CacheFailoverMode.DatabaseOnly && this.DefaultDataProvider is ICachePrimingInfo)
			{
				ICachePrimingInfo cachePrimingInfo = this.DefaultDataProvider as ICachePrimingInfo;
				Guid guid = Guid.NewGuid();
				this.TraceDebug("Find type={0}: Trying to get CurrentFailoverMode from CompositeDataProvider {1}, requestMode={2}", new object[]
				{
					typeof(TReturn),
					guid,
					this.DataAcccess
				});
				CachePrimingState currentPrimingState = cachePrimingInfo.GetCurrentPrimingState(typeof(TReturn));
				cacheFailoverMode = cachePrimingInfo.GetCurrentFailoverMode(typeof(TReturn), this.DataAcccess, currentPrimingState);
				this.TraceDebug("CurrentFailoverMode={0}, PrimingState={1}", new object[]
				{
					cacheFailoverMode,
					currentPrimingState
				});
			}
			IEnumerable<TInput> enumerable = null;
			if (cacheFailoverMode != CacheFailoverMode.DatabaseOnly)
			{
				IEnumerable<TReturn> enumerable2 = this.FindFromCache<TReturn, TInput>(lookupValues, lookupValueDefinition);
				enumerable = from IPropertyBag r in enumerable2
				select (TInput)((object)r[lookupValueDefinition]);
				if (enumerable.Count<TInput>() > 0)
				{
					list.AddRange(enumerable2);
				}
			}
			if (cacheFailoverMode != CacheFailoverMode.CacheOnly)
			{
				IEnumerable<TInput> enumerable3 = (enumerable != null && enumerable.Count<TInput>() > 0) ? lookupValues.Except(enumerable, new DomainSession.InputEqualityComparer<TInput>()) : lookupValues;
				if (enumerable3.Count<TInput>() > 0)
				{
					IEnumerable<TReturn> enumerable4 = this.FindFromDB<TReturn, TInput>(enumerable3, lookupCollectionDefinition);
					IEnumerable<TInput> source = from IPropertyBag r in enumerable4
					select (TInput)((object)r[lookupValueDefinition]);
					if (source.Count<TInput>() > 0)
					{
						list.AddRange(enumerable4);
					}
				}
			}
			this.Track(this.GetTrackingTag(false, "FndB", new Type[]
			{
				typeof(TReturn)
			}));
			return list;
		}

		private IEnumerable<TReturn> FindFromDB<TReturn, TInput>(IEnumerable<TInput> lookupValues, HygienePropertyDefinition lookupCollectionDefinition) where TReturn : IConfigurable, new()
		{
			this.Track(this.GetTrackingTag(true, "FndBD", new Type[]
			{
				typeof(TReturn)
			}));
			Dictionary<object, List<TInput>> dictionary = DalHelper.SplitByPhysicalInstance<TInput>((IHashBucket)this.WebStoreDataProvider, lookupValues, (TInput i) => i.ToString());
			List<TReturn> list = new List<TReturn>();
			foreach (object obj in dictionary.Keys)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, lookupCollectionDefinition, new MultiValuedProperty<TInput>(dictionary[obj]));
				list.AddRange(this.Find<TReturn>(filter, this.WebStoreDataProvider, obj));
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("FindFromDB type={0},  result={1}", new object[]
				{
					typeof(TReturn),
					list.ConvertToString<TReturn>()
				});
			}
			this.Track(this.GetTrackingTag(false, "FndBD", new Type[]
			{
				typeof(TReturn)
			}));
			return list;
		}

		private IEnumerable<TReturn> FindFromAllPartitions<TReturn>(QueryFilter filter) where TReturn : IConfigurable, new()
		{
			this.Track(this.GetTrackingTag(true, "FndAllP", new Type[]
			{
				typeof(TReturn)
			}));
			object[] allPhysicalPartitions = ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions();
			List<TReturn> list = new List<TReturn>();
			foreach (object physicalInstanceId in allPhysicalPartitions)
			{
				list.AddRange(this.Find<TReturn>(filter, this.WebStoreDataProvider, physicalInstanceId));
			}
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("FindFromDB type={0},  result={1}", new object[]
				{
					typeof(TReturn),
					list.ConvertToString<TReturn>()
				});
			}
			this.Track(this.GetTrackingTag(false, "FndAllP", new Type[]
			{
				typeof(TReturn)
			}));
			return list;
		}

		private IEnumerable<TReturn> FindFromCache<TReturn, TInput>(IEnumerable<TInput> lookupValues, HygienePropertyDefinition lookupValueDefinition) where TReturn : IConfigurable, new()
		{
			this.Track(this.GetTrackingTag(true, "FndBC", new Type[]
			{
				typeof(TReturn)
			}));
			ConcurrentBag<TReturn> results = new ConcurrentBag<TReturn>();
			Parallel.ForEach<TInput>(lookupValues, delegate(TInput lookupValue)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, lookupValueDefinition, lookupValue);
				TReturn treturn = this.Find<TReturn>(filter, this.CacheDataProvider, null).FirstOrDefault<TReturn>();
				if (treturn != null)
				{
					results.Add(treturn);
				}
			});
			if (TraceHelper.IsDebugTraceEnabled())
			{
				this.TraceDebug("FindFromCache type={0},  result={1}", new object[]
				{
					typeof(TReturn),
					results.ConvertToString<TReturn>()
				});
			}
			this.Track(this.GetTrackingTag(false, "FndBC", new Type[]
			{
				typeof(TReturn)
			}));
			return results;
		}

		private int GetPhysicalPartitionCopyCount(int physicalInstanceId)
		{
			IPartitionedDataProvider partitionedDataProvider = this.webStoreDataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.webStoreDataProvider.GetType()));
			}
			return partitionedDataProvider.GetNumberOfPersistentCopiesPerPartition(physicalInstanceId);
		}

		private int GetPhysicalInstanceId(Guid objectId)
		{
			IHashBucket hashBucket = this.WebStoreDataProvider as IHashBucket;
			if (hashBucket == null)
			{
				throw new NotSupportedException(string.Format("Not supported for DataProvider type: {0}", this.webStoreDataProvider.GetType()));
			}
			int logicalHash = hashBucket.GetLogicalHash(objectId.ToString());
			return (int)hashBucket.GetPhysicalInstanceIdByHashValue(logicalHash);
		}

		private int[] GetAllPhysicalPartitions()
		{
			return ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions().Cast<int>().ToArray<int>();
		}

		private void Track(string trackInfo)
		{
			if (this.Profiler != null)
			{
				this.Profiler.SnapShot(trackInfo, -1, true, null);
			}
		}

		private void Track(DomainSession.CacheTrackingTypes type, string profilerHeader, int cacheHit, CachePrimingState primingState, CacheFailoverMode failoverMode, int bloomFilterHit, int inMemoryHit, bool cacheExplicitlyBypassed)
		{
			if (this.Profiler != null)
			{
				string text = string.Format("{0}:Health={1}/CacheHit={2}/Mode={3}/Bloom={4}/Mem={5}#", new object[]
				{
					profilerHeader,
					(int)primingState,
					cacheHit,
					(int)failoverMode,
					bloomFilterHit,
					inMemoryHit
				});
				if (!cacheExplicitlyBypassed)
				{
					if (primingState == CachePrimingState.Healthy)
					{
						if (!this.Profiler.AddDictionary(DomainSession.TrackingDictionaryKey(DomainSession.CacheTrackingCategory.CacheHealthy, type), "1", 2))
						{
							EventLogger.LogDomainCacheTrackingError(new object[]
							{
								"CacheHealthy",
								text
							});
						}
					}
					else if (!this.Profiler.AddDictionary(DomainSession.TrackingDictionaryKey(DomainSession.CacheTrackingCategory.CacheUnHealthy, type), "1", 2))
					{
						EventLogger.LogDomainCacheTrackingError(new object[]
						{
							"CacheUnHealthy",
							text
						});
					}
					if (cacheHit == 1)
					{
						if (!this.Profiler.AddDictionary(DomainSession.TrackingDictionaryKey(DomainSession.CacheTrackingCategory.CacheHit, type), "1", 2))
						{
							EventLogger.LogDomainCacheTrackingError(new object[]
							{
								"CacheHit",
								text
							});
						}
					}
					else if (!this.Profiler.AddDictionary(DomainSession.TrackingDictionaryKey(DomainSession.CacheTrackingCategory.CacheMiss, type), "1", 2))
					{
						EventLogger.LogDomainCacheTrackingError(new object[]
						{
							"CacheMiss",
							text
						});
					}
				}
				this.Profiler.SnapShot(text, -1, true, null);
			}
		}

		private DomainSession.CacheTrackingTypes GetCacheTrackingType(Type objectType)
		{
			DomainSession.CacheTrackingTypes result;
			if (!DomainSession.ObjectTypeToCacheTrackingTypeMap.TryGetValue(objectType, out result))
			{
				result = DomainSession.CacheTrackingTypes.TypeMinIndex;
			}
			return result;
		}

		private string GetTrackingTag(bool isStart, string operationTag, object physicalInstanceId, params Type[] objectTypes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DAL");
			stringBuilder.Append("_");
			stringBuilder.Append(operationTag);
			if (physicalInstanceId != null)
			{
				stringBuilder.Append("P" + physicalInstanceId.ToString());
			}
			if (objectTypes != null && objectTypes.Length > 0)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(string.Join("_", objectTypes.Select(delegate(Type objectType)
				{
					string result = null;
					if (!DomainSession.ObjectTypeToTrackingTagMap.TryGetValue(objectType, out result))
					{
						result = "Unkown";
					}
					return result;
				})));
			}
			stringBuilder.Append("_");
			stringBuilder.Append(isStart ? "S" : "E");
			return stringBuilder.ToString();
		}

		private string GetTrackingTag(bool isStart, string operationTag, params Type[] objectTypes)
		{
			return this.GetTrackingTag(isStart, operationTag, null, objectTypes);
		}

		public const int ConstUseExistingDomainKeyFromDB = 1;

		private const string TrackingTagSeparator = "_";

		private const string FindTrackingTag = "Fnd";

		private const string FindBulkTrackingTag = "FndB";

		private const string FindBulkDBTrackingTag = "FndBD";

		private const string FindAllPartitionsDBTrackingTag = "FndAllP";

		private const string FindBulkCacheTrackingTag = "FndBC";

		private const string SaveTrackingTag = "Sav";

		private const string DeleteTrackingTag = "Del";

		private const string UpdateTrackingTag = "Upd";

		private const string DalTrackingTag = "DAL";

		private const string PhysicalInstanceTag = "P";

		private const string UnknownTag = "Unkown";

		private const string StartTag = "S";

		private const string EndTag = "E";

		private const string UnknownCallerId = "Unknown";

		public static readonly Dictionary<Type, HygienePropertyDefinition> IdentifierMap = new Dictionary<Type, HygienePropertyDefinition>
		{
			{
				typeof(TargetService),
				DomainSchema.TargetServiceId
			},
			{
				typeof(DomainTargetEnvironment),
				DomainSchema.DomainTargetEnvironmentId
			},
			{
				typeof(TenantTargetEnvironment),
				DomainSchema.TenantTargetEnvironmentId
			},
			{
				typeof(UserTargetEnvironment),
				DomainSchema.UserTargetEnvironmentId
			},
			{
				typeof(Zone),
				DomainSchema.ZoneId
			},
			{
				typeof(NsResourceRecord),
				DomainSchema.ResourceRecordId
			},
			{
				typeof(SoaResourceRecord),
				DomainSchema.ResourceRecordId
			}
		};

		private static readonly Dictionary<Type, string> ObjectTypeToTrackingTagMap = new Dictionary<Type, string>
		{
			{
				typeof(TargetService),
				"TS"
			},
			{
				typeof(DomainTargetEnvironment),
				"D"
			},
			{
				typeof(TenantTargetEnvironment),
				"T"
			},
			{
				typeof(UserTargetEnvironment),
				"U"
			},
			{
				typeof(Zone),
				"ZN"
			},
			{
				typeof(NsResourceRecord),
				"NS"
			},
			{
				typeof(SoaResourceRecord),
				"SOA"
			}
		};

		private static readonly Dictionary<Type, DomainSession.CacheTrackingTypes> ObjectTypeToCacheTrackingTypeMap = new Dictionary<Type, DomainSession.CacheTrackingTypes>
		{
			{
				typeof(TargetService),
				DomainSession.CacheTrackingTypes.TargetService
			},
			{
				typeof(DomainTargetEnvironment),
				DomainSession.CacheTrackingTypes.Domain
			},
			{
				typeof(TenantTargetEnvironment),
				DomainSession.CacheTrackingTypes.Tenant
			}
		};

		private IConfigDataProvider dataProvider;

		private IConfigDataProvider webStoreDataProvider;

		private static readonly Random randomGenerator = new Random();

		private static readonly CacheItemPolicy propDefinitioncachItemPolicy = new CacheItemPolicy();

		private readonly Guid transactionId;

		private readonly string callerId;

		private IConfigDataProvider cacheDataProvider;

		private IConfigDataProvider cacheFallbackDataProvider;

		public enum CacheTrackingTypes
		{
			TypeMinIndex,
			Tenant,
			Domain,
			TargetService,
			TypeMaxIndex
		}

		public enum CacheTrackingCategory
		{
			CategoryMinIndex,
			CacheHealthy,
			CacheUnHealthy,
			CacheHit,
			CacheMiss,
			CategoryMaxIndex
		}

		private class InputEqualityComparer<TInput> : IEqualityComparer<TInput>
		{
			public bool Equals(TInput x, TInput y)
			{
				if (typeof(TInput) == typeof(string))
				{
					return string.Equals(Convert.ToString(x), Convert.ToString(y), StringComparison.OrdinalIgnoreCase);
				}
				return x.Equals(y);
			}

			public int GetHashCode(TInput obj)
			{
				if (typeof(TInput) == typeof(string))
				{
					return Convert.ToString(obj).ToLower().GetHashCode();
				}
				return obj.GetHashCode();
			}
		}
	}
}
