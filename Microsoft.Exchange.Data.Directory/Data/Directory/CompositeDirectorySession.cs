using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Cache;
using Microsoft.Exchange.Data.Directory.Diagnostics;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class CompositeDirectorySession<TSession> : IDirectorySession, IConfigDataProvider where TSession : IDirectorySession, IConfigDataProvider
	{
		protected CompositeDirectorySession(TSession cacheSession, TSession directorySession, bool cacheSessionForDeletingOnly = false)
		{
			ArgumentValidator.ThrowIfNull("cacheSession", cacheSession);
			ArgumentValidator.ThrowIfNull("directorySession", directorySession);
			this.cacheSession = cacheSession;
			this.directorySession = directorySession;
			this.CacheSessionForDeletingOnly = cacheSessionForDeletingOnly;
		}

		internal bool CacheSessionForDeletingOnly { get; set; }

		protected abstract string Implementor { get; }

		TimeSpan? IDirectorySession.ClientSideSearchTimeout
		{
			get
			{
				TSession session = this.GetSession();
				return session.ClientSideSearchTimeout;
			}
			set
			{
				TSession session = this.GetSession();
				session.ClientSideSearchTimeout = value;
			}
		}

		ConfigScopes IDirectorySession.ConfigScope
		{
			get
			{
				TSession session = this.GetSession();
				return session.ConfigScope;
			}
		}

		ConsistencyMode IDirectorySession.ConsistencyMode
		{
			get
			{
				TSession session = this.GetSession();
				return session.ConsistencyMode;
			}
		}

		string IDirectorySession.DomainController
		{
			get
			{
				TSession session = this.GetSession();
				return session.DomainController;
			}
			set
			{
				TSession session = this.GetSession();
				session.DomainController = value;
			}
		}

		bool IDirectorySession.EnforceContainerizedScoping
		{
			get
			{
				TSession session = this.GetSession();
				return session.EnforceContainerizedScoping;
			}
			set
			{
				TSession session = this.GetSession();
				session.EnforceContainerizedScoping = value;
			}
		}

		bool IDirectorySession.EnforceDefaultScope
		{
			get
			{
				TSession session = this.GetSession();
				return session.EnforceDefaultScope;
			}
			set
			{
				TSession session = this.GetSession();
				session.EnforceDefaultScope = value;
			}
		}

		string IDirectorySession.LastUsedDc
		{
			get
			{
				TSession session = this.GetSession();
				return session.LastUsedDc;
			}
		}

		int IDirectorySession.Lcid
		{
			get
			{
				TSession session = this.GetSession();
				return session.Lcid;
			}
		}

		string IDirectorySession.LinkResolutionServer
		{
			get
			{
				TSession session = this.GetSession();
				return session.LinkResolutionServer;
			}
			set
			{
				TSession session = this.GetSession();
				session.LinkResolutionServer = value;
			}
		}

		bool IDirectorySession.LogSizeLimitExceededEvent
		{
			get
			{
				TSession session = this.GetSession();
				return session.LogSizeLimitExceededEvent;
			}
			set
			{
				TSession session = this.GetSession();
				session.LogSizeLimitExceededEvent = value;
			}
		}

		NetworkCredential IDirectorySession.NetworkCredential
		{
			get
			{
				TSession session = this.GetSession();
				return session.NetworkCredential;
			}
		}

		bool IDirectorySession.ReadOnly
		{
			get
			{
				TSession session = this.GetSession();
				return session.ReadOnly;
			}
		}

		ADServerSettings IDirectorySession.ServerSettings
		{
			get
			{
				TSession session = this.GetSession();
				return session.ServerSettings;
			}
		}

		TimeSpan? IDirectorySession.ServerTimeout
		{
			get
			{
				TSession session = this.GetSession();
				return session.ServerTimeout;
			}
			set
			{
				TSession session = this.GetSession();
				session.ServerTimeout = value;
			}
		}

		ADSessionSettings IDirectorySession.SessionSettings
		{
			get
			{
				TSession session = this.GetSession();
				return session.SessionSettings;
			}
		}

		bool IDirectorySession.SkipRangedAttributes
		{
			get
			{
				TSession session = this.GetSession();
				return session.SkipRangedAttributes;
			}
			set
			{
				TSession session = this.GetSession();
				session.SkipRangedAttributes = value;
			}
		}

		public string[] ExclusiveLdapAttributes
		{
			get
			{
				TSession session = this.GetSession();
				return session.ExclusiveLdapAttributes;
			}
			set
			{
				TSession session = this.GetSession();
				session.ExclusiveLdapAttributes = value;
			}
		}

		bool IDirectorySession.UseConfigNC
		{
			get
			{
				TSession session = this.GetSession();
				return session.UseConfigNC;
			}
			set
			{
				TSession session = this.GetSession();
				session.UseConfigNC = value;
			}
		}

		bool IDirectorySession.UseGlobalCatalog
		{
			get
			{
				TSession session = this.GetSession();
				return session.UseGlobalCatalog;
			}
			set
			{
				TSession session = this.GetSession();
				session.UseGlobalCatalog = value;
			}
		}

		IActivityScope IDirectorySession.ActivityScope
		{
			get
			{
				TSession session = this.GetSession();
				return session.ActivityScope;
			}
			set
			{
				TSession session = this.GetSession();
				session.ActivityScope = value;
			}
		}

		string IDirectorySession.CallerInfo
		{
			get
			{
				TSession session = this.GetSession();
				return session.CallerInfo;
			}
		}

		void IDirectorySession.AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			TSession session = this.GetSession();
			session.AnalyzeDirectoryError(connection, request, de, totalRetries, retriesOnServer);
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter)
		{
			TSession session = this.GetSession();
			return session.ApplyDefaultFilters(filter, rootId, dummyObject, applyImplicitFilter);
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			TSession session = this.GetSession();
			return session.ApplyDefaultFilters(filter, scope, dummyObject, applyImplicitFilter);
		}

		void IDirectorySession.CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			TSession session = this.GetSession();
			session.CheckFilterForUnsafeIdentity(filter);
		}

		void IDirectorySession.UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			TSession session = this.GetSession();
			session.UnsafeExecuteModificationRequest(request, rootId);
		}

		ADRawEntry[] IDirectorySession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.Find(rootId, scope, filter, sortBy, maxResults, properties);
		}

		TResult[] IDirectorySession.Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			TSession session = this.GetSession();
			return session.Find<TResult>(rootId, scope, filter, sortBy, maxResults);
		}

		ADRawEntry[] IDirectorySession.FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.FindAllADRawEntriesByUsnRange(root, startUsn, endUsn, sizeLimit, useAtomicFilter, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			TSession session = this.GetSession();
			return session.FindByADObjectIds(ids, properties);
		}

		Result<TData>[] IDirectorySession.FindByADObjectIds<TData>(ADObjectId[] ids)
		{
			TSession session = this.GetSession();
			return session.FindByADObjectIds<TData>(ids);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			TSession session = this.GetSession();
			return session.FindByCorrelationIds(correlationIds, configUnit, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			TSession session = this.GetSession();
			return session.FindByExchangeLegacyDNs(exchangeLegacyDNs, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			TSession session = this.GetSession();
			return session.FindByObjectGuids(objectGuids, properties);
		}

		ADRawEntry[] IDirectorySession.FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.FindDeletedTenantSyncObjectByUsnRange(tenantOuRoot, startUsn, sizeLimit, properties);
		}

		ADPagedReader<TResult> IDirectorySession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.FindPagedADRawEntry(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.FindPagedADRawEntryWithDefaultFilters<TResult>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		ADPagedReader<TResult> IDirectorySession.FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			TSession session = this.GetSession();
			return session.FindPagedDeletedObject<TResult>(rootId, scope, filter, sortBy, pageSize);
		}

		ADObjectId IDirectorySession.GetConfigurationNamingContext()
		{
			TSession session = this.GetSession();
			return session.GetConfigurationNamingContext();
		}

		ADObjectId IDirectorySession.GetConfigurationUnitsRoot()
		{
			TSession session = this.GetSession();
			return session.GetConfigurationUnitsRoot();
		}

		ADObjectId IDirectorySession.GetDomainNamingContext()
		{
			TSession session = this.GetSession();
			return session.GetDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetHostedOrganizationsRoot()
		{
			TSession session = this.GetSession();
			return session.GetHostedOrganizationsRoot();
		}

		ADObjectId IDirectorySession.GetRootDomainNamingContext()
		{
			TSession session = this.GetSession();
			return session.GetRootDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetSchemaNamingContext()
		{
			TSession session = this.GetSession();
			return session.GetSchemaNamingContext();
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			TSession session = this.GetSession();
			return session.GetReadConnection(preferredServer, ref rootId);
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			TSession session = this.GetSession();
			return session.GetReadConnection(preferredServer, optionalBaseDN, ref rootId, scopeDeteriminingObject);
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			TSession session = this.GetSession();
			return session.GetReadScope(rootId, scopeDeterminingObject);
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			TSession session = this.GetSession();
			return session.GetReadScope(rootId, scopeDeterminingObject, isWellKnownGuidSearch, out applicableScope);
		}

		bool IDirectorySession.GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.GetSchemaAndApplyFilter(adRawEntry, scope, out dummyObject, out ldapAttributes, ref filter, ref properties);
		}

		bool IDirectorySession.IsReadConnectionAvailable()
		{
			TSession session = this.GetSession();
			return session.IsReadConnectionAvailable();
		}

		bool IDirectorySession.IsRootIdWithinScope<TObject>(ADObjectId rootId)
		{
			TSession session = this.GetSession();
			return session.IsRootIdWithinScope<TObject>(rootId);
		}

		bool IDirectorySession.IsTenantIdentity(ADObjectId id)
		{
			TSession session = this.GetSession();
			return session.IsTenantIdentity(id);
		}

		ADRawEntry IDirectorySession.ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((TSession session) => session.ReadADRawEntry(entryId, properties), null, properties);
		}

		RawSecurityDescriptor IDirectorySession.ReadSecurityDescriptor(ADObjectId id)
		{
			TSession session = this.GetSession();
			return session.ReadSecurityDescriptor(id);
		}

		SecurityDescriptor IDirectorySession.ReadSecurityDescriptorBlob(ADObjectId id)
		{
			TSession session = this.GetSession();
			return session.ReadSecurityDescriptorBlob(id);
		}

		string[] IDirectorySession.ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			TSession session = this.GetSession();
			return session.ReplicateSingleObject(instanceToReplicate, sites);
		}

		bool IDirectorySession.ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			TSession session = this.GetSession();
			return session.ReplicateSingleObjectToTargetDC(instanceToReplicate, targetServerName);
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId)
		{
			TSession session = this.GetSession();
			return session.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerId);
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN)
		{
			TSession session = this.GetSession();
			return session.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerDN);
		}

		TenantRelocationSyncObject IDirectorySession.RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			TSession session = this.GetSession();
			return session.RetrieveTenantRelocationSyncObject(entryId, properties);
		}

		ADOperationResultWithData<TResult>[] IDirectorySession.RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall)
		{
			TSession session = this.GetSession();
			return session.RunAgainstAllDCsInSite<TResult>(siteId, methodToCall);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			TSession session = this.GetSession();
			session.SaveSecurityDescriptor(id, sd);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			TSession session = this.GetSession();
			session.SaveSecurityDescriptor(id, sd, modifyOwner);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			obj.m_Session = this.GetSession();
			TSession session = this.GetSession();
			session.SaveSecurityDescriptor(obj, sd);
			obj.m_Session = this;
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			obj.m_Session = this.GetSession();
			TSession session = this.GetSession();
			session.SaveSecurityDescriptor(obj, sd, modifyOwner);
			obj.m_Session = this;
		}

		bool IDirectorySession.TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			TSession session = this.GetSession();
			return session.TryVerifyIsWithinScopes(entry, isModification, out exception);
		}

		void IDirectorySession.UpdateServerSettings(PooledLdapConnection connection)
		{
			TSession session = this.GetSession();
			session.UpdateServerSettings(connection);
		}

		void IDirectorySession.VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			TSession session = this.GetSession();
			session.VerifyIsWithinScopes(entry, isModification);
		}

		TResult[] IDirectorySession.ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance)
		{
			TSession session = this.GetSession();
			return session.ObjectsFromEntries<TResult>(entries, originatingServerName, properties, dummyInstance);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.InternalDelete(instance);
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			TSession session = this.GetSession();
			return session.Find<T>(filter, rootId, deepSearch, sortBy);
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			TSession session = this.GetSession();
			return session.FindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize);
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			TSession session = this.GetSession();
			return session.Read<T>(identity);
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			this.InternalSave(instance);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				TSession session = this.GetSession();
				return session.Source;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected TSession GetCacheSession()
		{
			return this.cacheSession;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected TSession GetSession()
		{
			return this.directorySession;
		}

		protected TResult ExecuteSingleObjectQueryWithFallback<TResult>(Func<TSession, TResult> query, Func<TResult, List<Tuple<string, KeyType>>> getAdditionalKeys = null, IEnumerable<PropertyDefinition> properties = null) where TResult : ADRawEntry, new()
		{
			ArgumentValidator.ThrowIfNull("query", query);
			if (!Configuration.IsCacheEnabled(typeof(TResult)))
			{
				return query(this.GetSession());
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			CachePerformanceTracker.StartLogging();
			CompositeDirectorySession<TSession>.TraceState traceState = CompositeDirectorySession<TSession>.TraceState.None;
			CacheMode cacheModeForCurrentProcess = Configuration.GetCacheModeForCurrentProcess();
			double num = -1.0;
			double num2 = -1.0;
			double num3 = -1.0;
			double num4 = -1.0;
			TResult tresult = default(TResult);
			string text = string.Empty;
			Guid guid = Guid.Empty;
			ADCacheResultState adcacheResultState = ADCacheResultState.Succeed;
			bool flag = true;
			int num5 = 0;
			try
			{
				Stopwatch stopwatch2 = null;
				ADObject adobject = null;
				try
				{
					TSession session = this.GetSession();
					if (session.ActivityScope != null)
					{
						TSession session2 = this.GetSession();
						if (session2.ActivityScope.Status == ActivityContextStatus.ActivityStarted)
						{
							TSession session3 = this.GetSession();
							guid = session3.ActivityScope.ActivityId;
						}
					}
					if (!this.CacheSessionForDeletingOnly && (CacheMode.Read & cacheModeForCurrentProcess) != CacheMode.Disabled)
					{
						TSession session4 = this.GetSession();
						if (!session4.SessionSettings.IncludeSoftDeletedObjectLinks)
						{
							if (properties != null || !typeof(TResult).Equals(typeof(ADRawEntry)))
							{
								traceState |= CompositeDirectorySession<TSession>.TraceState.CacheRead;
								stopwatch2 = Stopwatch.StartNew();
								tresult = query(this.GetCacheSession());
								CacheDirectorySession cacheDirectorySession = this.GetCacheSession() as CacheDirectorySession;
								if (cacheDirectorySession != null)
								{
									adcacheResultState = cacheDirectorySession.ResultState;
									flag = cacheDirectorySession.IsNewProxyObject;
									num5 = cacheDirectorySession.RetryCount;
								}
								stopwatch2.Stop();
								num = (double)stopwatch2.ElapsedMilliseconds;
								goto IL_19A;
							}
							goto IL_19A;
						}
					}
					if ((CacheMode.Read & cacheModeForCurrentProcess) == CacheMode.Disabled || this.CacheSessionForDeletingOnly)
					{
						adcacheResultState = ADCacheResultState.CacheModeIsNotRead;
					}
					else
					{
						adcacheResultState = ADCacheResultState.SoftDeletedObject;
					}
					IL_19A:
					if (tresult != null && tresult.Id != null)
					{
						TSession session5 = this.GetSession();
						if (ADSession.ShouldFilterCNFObject(session5.SessionSettings, tresult.Id))
						{
							tresult = default(TResult);
							adcacheResultState = ADCacheResultState.CNFedObject;
						}
						else
						{
							TSession session6 = this.GetSession();
							if (ADSession.ShouldFilterSoftDeleteObject(session6.SessionSettings, tresult.Id))
							{
								tresult = default(TResult);
								adcacheResultState = ADCacheResultState.SoftDeletedObject;
							}
							else
							{
								if (tresult.Id != null && tresult.Id.DomainId != null)
								{
									TSession session7 = this.GetSession();
									if (!session7.SessionSettings.PartitionId.Equals(tresult.Id.GetPartitionId()))
									{
										if (ExEnvironment.IsTest)
										{
											TSession session8 = this.GetSession();
											if (session8.SessionSettings.PartitionId.ForestFQDN.EndsWith(tresult.Id.GetPartitionId().ForestFQDN, StringComparison.OrdinalIgnoreCase))
											{
												goto IL_341;
											}
										}
										ExEventLog.EventTuple tuple_WrongObjectReturned = DirectoryEventLogConstants.Tuple_WrongObjectReturned;
										string name = typeof(TResult).Name;
										object[] array = new object[2];
										object[] array2 = array;
										int num6 = 0;
										TSession session9 = this.GetSession();
										array2[num6] = session9.SessionSettings.PartitionId.ForestFQDN;
										array[1] = tresult.Id.GetPartitionId().ForestFQDN;
										Globals.LogEvent(tuple_WrongObjectReturned, name, array);
										tresult = default(TResult);
										adcacheResultState = ADCacheResultState.WrongForest;
										goto IL_3E1;
									}
								}
								IL_341:
								TSession session10 = this.GetSession();
								if (session10.SessionSettings.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
								{
									TSession session11 = this.GetSession();
									OrganizationId currentOrganizationId = session11.SessionSettings.CurrentOrganizationId;
									if (!tresult.Id.IsDescendantOf(currentOrganizationId.OrganizationalUnit) && !tresult.Id.IsDescendantOf(currentOrganizationId.ConfigurationUnit.Parent))
									{
										tresult = default(TResult);
										adcacheResultState = ADCacheResultState.OranizationIdMismatch;
									}
								}
							}
						}
					}
					else if (tresult != null)
					{
						tresult = default(TResult);
					}
					else
					{
						adcacheResultState = ADCacheResultState.NotFound;
					}
					IL_3E1:
					adobject = (tresult as ADObject);
				}
				catch (Exception ex)
				{
					tresult = default(TResult);
					adcacheResultState = ADCacheResultState.ExceptionHappened;
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_CallADCacheServiceFailed, typeof(TResult).Name, new object[]
					{
						ex.ToString()
					});
					text = ex.ToString();
				}
				if (tresult == null)
				{
					traceState |= CompositeDirectorySession<TSession>.TraceState.ADRead;
					stopwatch2 = Stopwatch.StartNew();
					tresult = query(this.GetSession());
					adobject = (tresult as ADObject);
					stopwatch2.Stop();
					num4 = (double)stopwatch2.ElapsedMilliseconds;
					stopwatch2.Restart();
					bool flag2 = true;
					if (adobject != null)
					{
						TSession session12 = this.GetSession();
						if (!session12.ReadOnly && CompositeDirectorySession<TSession>.ExchangeConfigUnitCUType.Equals(adobject.GetType()) && OrganizationId.ForestWideOrgId.Equals(adobject.OrganizationId))
						{
							ExTraceGlobals.SessionTracer.TraceWarning<string>((long)this.GetHashCode(), "Newly created ExchangeCU with organizationId equals to RootOrgId, ignored till is fully populated. DN {0}", tresult.GetDistinguishedNameOrName());
							flag2 = false;
						}
					}
					if (tresult != null && CacheUtils.GetObjectTypeFor(tresult.GetType(), false) == ObjectType.Unknown)
					{
						flag2 = false;
					}
					if (tresult != null && ((CacheMode.SyncWrite | CacheMode.AsyncWrite) & cacheModeForCurrentProcess) != CacheMode.Disabled && tresult.Id != null && !tresult.Id.IsDeleted && flag2)
					{
						traceState |= CompositeDirectorySession<TSession>.TraceState.CacheInsert;
						if ((CacheMode.SyncWrite & cacheModeForCurrentProcess) != CacheMode.Disabled)
						{
							try
							{
								this.CacheInsert<TResult>(tresult, getAdditionalKeys, properties);
								goto IL_5C8;
							}
							catch (Exception ex2)
							{
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_CallADCacheServiceFailed, "CacheInsert", new object[]
								{
									ex2.ToString()
								});
								text += ex2.Message;
								goto IL_5C8;
							}
						}
						TSession session13 = this.GetSession();
						if (session13.ReadOnly)
						{
							this.AsyncCacheInsert<TResult>(tresult, getAdditionalKeys, properties);
						}
						else
						{
							adcacheResultState |= ADCacheResultState.WritableSession;
						}
					}
					IL_5C8:
					stopwatch2.Stop();
					num3 = (double)stopwatch2.ElapsedMilliseconds;
				}
				if (adobject != null)
				{
					adobject.m_Session = this;
				}
			}
			finally
			{
				stopwatch.Stop();
				string text2 = CachePerformanceTracker.StopLogging();
				string operation = "Read";
				string dn = (tresult != null) ? tresult.GetDistinguishedNameOrName() : "<NULL>";
				DateTime whenReadUTC = (tresult != null) ? ((tresult.WhenReadUTC != null) ? tresult.WhenReadUTC.Value : DateTime.MinValue) : DateTime.MinValue;
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				long wcfGetProcessingTime = (long)num;
				long wcfRemoveProcessingTime = (long)num2;
				long wcfPutProcessingTime = (long)num3;
				long adProcessingTime = (long)num4;
				bool isNewProxyObject = flag;
				int retryCount = num5;
				string objectType = (tresult != null) ? tresult.GetType().Name : typeof(TResult).Name;
				string cachePerformanceTracker = text2;
				Guid activityId = guid;
				TSession session14 = this.GetSession();
				CacheProtocolLog.BeginAppend(operation, dn, whenReadUTC, elapsedMilliseconds, wcfGetProcessingTime, wcfRemoveProcessingTime, wcfPutProcessingTime, adProcessingTime, isNewProxyObject, retryCount, objectType, cachePerformanceTracker, activityId, session14.CallerInfo, string.Format("ResultState:{0};{1}", (int)adcacheResultState, text));
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "ExecuteSingleObjectQueryWithFallback. Cache Mode {0}. TraceState {1}. DN {2}. WhenRead {3}. IsCached {4}.", new object[]
				{
					cacheModeForCurrentProcess,
					traceState,
					(tresult != null) ? tresult.GetDistinguishedNameOrName() : "<NULL>",
					(tresult != null) ? ((tresult.WhenReadUTC != null) ? tresult.WhenReadUTC.Value.ToString() : "<NULL>") : "<NULL>",
					tresult != null && tresult.IsCached
				});
				ExTraceGlobals.SessionTracer.TracePerformance((long)this.GetHashCode(), "ExecuteSingleObjectQueryWithFallback.  Cache Mode {0}. TraceState {1}. DN {2}. TotalTime {3}. GetCacheTime {4}. RemoveCacheTime {5}. PutCacheTime {6}. ADTime {7}. WCFDetails [{8}]", new object[]
				{
					cacheModeForCurrentProcess,
					traceState,
					(tresult != null) ? tresult.GetDistinguishedNameOrName() : "<NULL>",
					stopwatch.ElapsedMilliseconds,
					num,
					num2,
					num3,
					num4,
					text2
				});
				if (tresult != null)
				{
					ADProviderPerf.UpdateADDriverCacheHitRate(tresult.IsCached);
				}
			}
			return tresult;
		}

		protected void InternalSave(IConfigurable instance)
		{
			ObjectState objectState = instance.ObjectState;
			TSession session = this.GetSession();
			session.Save(instance);
			this.CacheUpdateFromSavedObject(instance, objectState);
		}

		protected void InternalDelete(IConfigurable instance)
		{
			TSession session = this.GetSession();
			session.Delete(instance);
			this.CacheDelete(instance);
		}

		protected void CacheUpdateFromSavedObject(IConfigurable instance, ObjectState objectStateBeforeSave)
		{
			ADRawEntry adrawEntry = instance as ADRawEntry;
			bool flag = Configuration.IsCacheEnabledForInsertOnSave(adrawEntry);
			ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "UpdateOrRemoveFromCache. Identity={0}, ObjectStateBeforeSave={1}, DistinguishedName={2}, WhenCreatedUtc={3}, InsertInCache={4}", new object[]
			{
				instance.Identity,
				objectStateBeforeSave,
				(adrawEntry != null) ? adrawEntry.GetDistinguishedNameOrName() : "<NULL>",
				(adrawEntry != null) ? adrawEntry[ADObjectSchema.WhenCreatedUTC] : "<NULL>",
				flag
			});
			if (!flag)
			{
				if (objectStateBeforeSave != ObjectState.New)
				{
					this.CacheDelete(instance);
				}
				return;
			}
			IEnumerable<PropertyDefinition> objectProperties = null;
			if (!(adrawEntry is ADObject))
			{
				objectProperties = new List<PropertyDefinition>(0);
			}
			this.CacheInsert<ADRawEntry>(adrawEntry, null, objectProperties);
		}

		protected void CacheDelete(IConfigurable instance)
		{
			CacheMode cacheModeForCurrentProcess = Configuration.GetCacheModeForCurrentProcess();
			if (((CacheMode.SyncWrite | CacheMode.AsyncWrite) & cacheModeForCurrentProcess) != CacheMode.Disabled)
			{
				if ((CacheMode.SyncWrite & cacheModeForCurrentProcess) != CacheMode.Disabled)
				{
					this.CacheDeleteInternal(instance);
				}
				else
				{
					ThreadPool.QueueUserWorkItem(delegate(object x)
					{
						this.CacheDeleteInternal((IConfigurable)x);
					}, instance);
				}
			}
			ExTraceGlobals.SessionTracer.TraceDebug<CacheMode, string>((long)this.GetHashCode(), "CacheDelete. Cache Mode {0}. ID {1}", cacheModeForCurrentProcess, instance.Identity.ToString());
		}

		protected T InvokeWithAPILogging<T>(Func<T> action, [CallerMemberName] string memberName = null)
		{
			return ADScenarioLog.InvokeWithAPILog<T>(DateTime.UtcNow, memberName, default(Guid), this.Implementor, "", () => action(), delegate
			{
				TSession session = this.GetSession();
				return session.LastUsedDc;
			});
		}

		protected T InvokeGetObjectWithAPILogging<T>(Func<T> action, [CallerMemberName] string memberName = null) where T : ADRawEntry
		{
			return ADScenarioLog.InvokeGetObjectAPIAndLog<T>(DateTime.UtcNow, memberName, default(Guid), this.Implementor, "", () => action(), delegate
			{
				TSession session = this.GetSession();
				return session.LastUsedDc;
			});
		}

		private void CacheDeleteInternal(IConfigurable instance)
		{
			string error = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			CachePerformanceTracker.StartLogging();
			bool isNewProxyObject = false;
			int retryCount = 0;
			string callerInfo = null;
			Guid activityId = Guid.Empty;
			try
			{
				TSession tsession = this.GetCacheSession();
				tsession.Delete(instance);
				CacheDirectorySession cacheDirectorySession = this.GetCacheSession() as CacheDirectorySession;
				if (cacheDirectorySession != null)
				{
					isNewProxyObject = cacheDirectorySession.IsNewProxyObject;
					retryCount = cacheDirectorySession.RetryCount;
				}
				TSession session = this.GetSession();
				if (session.ActivityScope != null)
				{
					TSession session2 = this.GetSession();
					if (session2.ActivityScope.Status == ActivityContextStatus.ActivityStarted)
					{
						TSession session3 = this.GetSession();
						activityId = session3.ActivityScope.ActivityId;
					}
				}
				TSession session4 = this.GetSession();
				callerInfo = session4.CallerInfo;
			}
			catch (Exception ex)
			{
				error = ex.ToString();
				throw;
			}
			finally
			{
				stopwatch.Stop();
				string cachePerformanceTracker = CachePerformanceTracker.StopLogging();
				ADRawEntry adrawEntry = instance as ADRawEntry;
				CacheProtocolLog.BeginAppend("Remove", (adrawEntry != null) ? adrawEntry.GetDistinguishedNameOrName() : "<NULL>", DateTime.MinValue, stopwatch.ElapsedMilliseconds, -1L, stopwatch.ElapsedMilliseconds, -1L, -1L, isNewProxyObject, retryCount, instance.GetType().Name, cachePerformanceTracker, activityId, callerInfo, error);
			}
		}

		private void AsyncCacheInsert<TResult>(TResult instance, Func<TResult, List<Tuple<string, KeyType>>> getAdditionalKeys = null, IEnumerable<PropertyDefinition> objectProperties = null) where TResult : IConfigurable, new()
		{
			ThreadPool.QueueUserWorkItem(delegate(object x)
			{
				try
				{
					Tuple<TResult, Func<TResult, List<Tuple<string, KeyType>>>, IEnumerable<PropertyDefinition>> tuple = (Tuple<TResult, Func<TResult, List<Tuple<string, KeyType>>>, IEnumerable<PropertyDefinition>>)x;
					this.CacheInsert<TResult>(tuple.Item1, tuple.Item2, tuple.Item3);
				}
				catch
				{
				}
			}, new Tuple<TResult, Func<TResult, List<Tuple<string, KeyType>>>, IEnumerable<PropertyDefinition>>(instance, getAdditionalKeys, objectProperties));
		}

		private void CacheInsert<TResult>(TResult instance, Func<TResult, List<Tuple<string, KeyType>>> getAdditionalKeys = null, IEnumerable<PropertyDefinition> objectProperties = null) where TResult : IConfigurable, new()
		{
			ArgumentValidator.ThrowIfNull("query", instance);
			Stopwatch stopwatch = Stopwatch.StartNew();
			CachePerformanceTracker.StartLogging();
			bool isNewProxyObject = false;
			int retryCount = 0;
			Guid activityId = Guid.Empty;
			string callerInfo = null;
			string error = null;
			try
			{
				List<Tuple<string, KeyType>> keys = null;
				TSession session = this.GetSession();
				if (session.ActivityScope != null)
				{
					TSession session2 = this.GetSession();
					if (session2.ActivityScope.Status == ActivityContextStatus.ActivityStarted)
					{
						TSession session3 = this.GetSession();
						activityId = session3.ActivityScope.ActivityId;
					}
				}
				TSession session4 = this.GetSession();
				callerInfo = session4.CallerInfo;
				if (getAdditionalKeys != null)
				{
					keys = getAdditionalKeys(instance);
				}
				ICacheDirectorySession cacheDirectorySession = this.GetCacheSession() as ICacheDirectorySession;
				bool flag = false;
				if (((IDirectorySession)this).SessionSettings.TenantConsistencyMode == TenantConsistencyMode.IgnoreRetiredTenants && (TenantRelocationStateCache.IsTenantRetired((ADObjectId)instance.Identity) || TenantRelocationStateCache.IsTenantArriving((ADObjectId)instance.Identity)))
				{
					ExTraceGlobals.SessionTracer.TraceWarning<ObjectId>((long)this.GetHashCode(), "CacheInsert. DN {0}. Tenant Is Retired or Arriving, skipping.", instance.Identity);
				}
				else
				{
					if (!instance.GetType().Equals(CompositeDirectorySession<TSession>.ExchangeConfigUnitCUType) && (TenantRelocationStateCache.IsTenantLockedDown((ADObjectId)instance.Identity) || TenantRelocationStateCache.IsTenantRetired((ADObjectId)instance.Identity)))
					{
						flag = true;
					}
					int num = flag ? 30 : Configuration.GetCacheExpirationForObject(instance as ADRawEntry);
					CacheItemPriority cacheItemPriority = flag ? CacheItemPriority.Default : Configuration.GetCachePriorityForObject(instance as ADRawEntry);
					if (cacheDirectorySession != null)
					{
						cacheDirectorySession.Insert(instance, objectProperties, keys, num, cacheItemPriority);
					}
					else
					{
						TSession tsession = this.GetCacheSession();
						tsession.Save(instance);
					}
					CacheDirectorySession cacheDirectorySession2 = this.GetCacheSession() as CacheDirectorySession;
					if (cacheDirectorySession2 != null)
					{
						isNewProxyObject = cacheDirectorySession2.IsNewProxyObject;
						retryCount = cacheDirectorySession2.RetryCount;
					}
					ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "CacheInsert. DN={0}, IsTenantLockedDownOrRetired={1}, CacheExpiration={2}, Priority={3}", new object[]
					{
						instance.Identity,
						flag,
						num,
						cacheItemPriority
					});
				}
			}
			catch (TransientException ex)
			{
				ExTraceGlobals.SessionTracer.TraceError<ObjectId, TransientException>((long)this.GetHashCode(), "CacheInsert. DN {0}. Exception {1}", instance.Identity, ex);
				error = ex.ToString();
			}
			catch (Exception ex2)
			{
				error = ex2.ToString();
				throw;
			}
			finally
			{
				stopwatch.Stop();
				string cachePerformanceTracker = CachePerformanceTracker.StopLogging();
				ADRawEntry adrawEntry = instance as ADRawEntry;
				CacheProtocolLog.BeginAppend("Save", (adrawEntry != null) ? adrawEntry.GetDistinguishedNameOrName() : "<NULL>", DateTime.MinValue, stopwatch.ElapsedMilliseconds, -1L, -1L, stopwatch.ElapsedMilliseconds, -1L, isNewProxyObject, retryCount, instance.GetType().Name, cachePerformanceTracker, activityId, callerInfo, error);
			}
		}

		private static readonly Type ExchangeConfigUnitCUType = typeof(ExchangeConfigurationUnit);

		private TSession cacheSession;

		private TSession directorySession;

		[Flags]
		private enum TraceState : byte
		{
			None = 0,
			CacheRead = 1,
			CacheRemoval = 2,
			CacheInsert = 4,
			ADRead = 8
		}
	}
}
