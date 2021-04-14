using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class SyncSession
	{
		public SyncSession()
		{
			this.DataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);
		}

		public SyncSession(Guid caller) : this()
		{
			this.Caller = caller;
		}

		public Guid Caller { get; private set; }

		internal IConfigDataProvider DataProvider { get; set; }

		public bool AcquireServiceCookie(ServiceCookieFilter filter, out ServiceCookie cookie)
		{
			if (filter == null)
			{
				throw new ArgumentException("filter");
			}
			if (this.Caller == Guid.Empty)
			{
				throw new IllegalOperationNoCallerSpecifiedException();
			}
			IConfigurable[] source = this.DataProvider.Find<ServiceCookie>(SyncSession.BuildFindCookieQuery(this.Caller, filter, true), null, true, null);
			cookie = (source.FirstOrDefault<IConfigurable>() as ServiceCookie);
			return cookie != null;
		}

		public bool AcquireTenantCookie(TenantCookieFilter filter, out TenantCookie cookie)
		{
			if (filter == null)
			{
				throw new ArgumentException("filter");
			}
			if (this.Caller == Guid.Empty)
			{
				throw new IllegalOperationNoCallerSpecifiedException();
			}
			IConfigurable[] source = this.DataProvider.Find<TenantCookie>(SyncSession.BuildFindCookieQuery(this.Caller, filter, true), null, true, null);
			cookie = (source.FirstOrDefault<IConfigurable>() as TenantCookie);
			return cookie != null;
		}

		public T[] FindCookies<T>(BaseCookieFilter filter) where T : IConfigurable, new()
		{
			if (filter == null)
			{
				throw new ArgumentException("filter");
			}
			if (this.Caller == Guid.Empty)
			{
				throw new IllegalOperationNoCallerSpecifiedException();
			}
			return this.DataProvider.Find<T>(SyncSession.BuildFindCookieQuery(this.Caller, filter, false), null, true, null).Cast<T>().ToArray<T>();
		}

		public IEnumerable<UnsyncedObject> FindUnsyncedObjects(string serviceInstance, ADObjectId tenantId = null, TimeSpan? cooldown = null)
		{
			if (string.IsNullOrEmpty(serviceInstance))
			{
				throw new ArgumentNullException("serviceInstance");
			}
			return this.DataProvider.Find<UnsyncedObject>(SyncSession.BuildFindUnsyncedObjectQuery(serviceInstance, tenantId, cooldown ?? SyncSession.DefaultCooldown), null, true, null).Cast<UnsyncedObject>();
		}

		public IEnumerable<AssignedPlan> FindUnpublishedPlans(string serviceInstance)
		{
			if (string.IsNullOrEmpty(serviceInstance))
			{
				throw new ArgumentNullException("serviceInstance");
			}
			return this.DataProvider.Find<AssignedPlan>(SyncSession.BuildFindUnpublishedObjectQuery(serviceInstance), null, true, null).Cast<AssignedPlan>();
		}

		public IEnumerable<UnpublishedObject> FindUnpublishedObjects(string serviceInstance)
		{
			if (string.IsNullOrEmpty(serviceInstance))
			{
				throw new ArgumentNullException("serviceInstance");
			}
			return this.DataProvider.Find<UnpublishedObject>(SyncSession.BuildFindUnpublishedObjectQuery(serviceInstance), null, true, null).Cast<UnpublishedObject>();
		}

		public IEnumerable<AcceptedDomain> FindUnprovisionedDomains(string serviceInstance)
		{
			if (string.IsNullOrEmpty(serviceInstance))
			{
				throw new ArgumentNullException("serviceInstance");
			}
			QueryFilter filter = new AndFilter(new ComparisonFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, CommonSyncProperties.PublishedProp, false),
				new ComparisonFilter(ComparisonOperator.Equal, CommonSyncProperties.ServiceInstanceProp, serviceInstance)
			});
			IEnumerable<AcceptedDomain> enumerable = this.DataProvider.Find<AcceptedDomain>(filter, null, true, null).Cast<AcceptedDomain>();
			foreach (AcceptedDomain acceptedDomain in enumerable)
			{
				acceptedDomain[CommonSyncProperties.PublishedProp] = true;
				if ((bool)acceptedDomain[SyncSession.IsDeletedProp])
				{
					acceptedDomain[ADObjectSchema.ObjectState] = ObjectState.Deleted;
				}
			}
			return enumerable;
		}

		public bool SyncStreamShouldRefreshProperties(string serviceInstance)
		{
			return this.FindSyncPropertyRefresh(serviceInstance).Any((SyncPropertyRefresh r) => r.Status == SyncPropertyRefreshStatus.Requested);
		}

		internal IEnumerable<SyncPropertyRefresh> FindSyncPropertyRefresh(string serviceInstance)
		{
			if (string.IsNullOrEmpty(serviceInstance))
			{
				throw new ArgumentNullException("serviceInstance");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serviceInstance);
			return this.DataProvider.Find<SyncPropertyRefresh>(filter, null, false, null).Cast<SyncPropertyRefresh>();
		}

		public void CompletePropertyRefresh(string serviceInstance)
		{
			foreach (SyncPropertyRefresh syncPropertyRefresh in from r in this.FindSyncPropertyRefresh(serviceInstance)
			where r.Status == SyncPropertyRefreshStatus.InProgress
			select r)
			{
				syncPropertyRefresh.Status = SyncPropertyRefreshStatus.Completed;
				this.Save(syncPropertyRefresh);
			}
		}

		public void SetSyncPropertyRefreshInProgress(string serviceInstance)
		{
			IEnumerable<SyncPropertyRefresh> enumerable = from r in this.FindSyncPropertyRefresh(serviceInstance)
			where r.Status == SyncPropertyRefreshStatus.Requested
			select r;
			if (!enumerable.Any<SyncPropertyRefresh>())
			{
				throw new InvalidOperationException(string.Format("Refresh not requested for service instance {0}", serviceInstance));
			}
			foreach (SyncPropertyRefresh syncPropertyRefresh in enumerable)
			{
				syncPropertyRefresh.Status = SyncPropertyRefreshStatus.InProgress;
				this.Save(syncPropertyRefresh);
			}
		}

		public void Save(IConfigurable configurable)
		{
			if (configurable == null)
			{
				throw new ArgumentNullException("configurable");
			}
			BaseCookie baseCookie = configurable as BaseCookie;
			if (baseCookie != null)
			{
				if (this.Caller == Guid.Empty)
				{
					throw new IllegalOperationNoCallerSpecifiedException();
				}
				baseCookie.ActiveMachine = Environment.MachineName;
				baseCookie[BaseCookieSchema.CallerProp] = this.Caller;
				baseCookie[BaseCookieSchema.AllowNullCookieProp] = false;
			}
			this.DataProvider.Save(configurable);
		}

		public void ResetCookie(BaseCookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (this.Caller == Guid.Empty)
			{
				throw new IllegalOperationNoCallerSpecifiedException();
			}
			cookie[BaseCookieSchema.CallerProp] = this.Caller;
			cookie[BaseCookieSchema.AllowNullCookieProp] = true;
			this.DataProvider.Save(cookie);
		}

		internal static QueryFilter BuildFindCookieQuery(Guid caller, BaseCookieFilter cookieFilter, bool acquireCookie)
		{
			ComparisonFilter item = new ComparisonFilter(ComparisonOperator.Equal, BaseCookieSchema.CallerProp, caller);
			ComparisonFilter item2 = new ComparisonFilter(ComparisonOperator.Equal, BaseCookieSchema.AcquireCookieLockProp, acquireCookie);
			List<QueryFilter> queryFilters = cookieFilter.GetQueryFilters();
			queryFilters.Add(item);
			queryFilters.Add(item2);
			return new AndFilter(queryFilters.ToArray());
		}

		internal static QueryFilter BuildFindUnsyncedObjectQuery(string serviceInstance, ADObjectId tenantId, TimeSpan cooldown)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, UnpublishedObjectSchema.ServiceInstanceProp, serviceInstance));
			if (tenantId != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, UnpublishedObjectSchema.TenantIdProp, tenantId));
			}
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, SyncSession.CooldownProp, cooldown.Minutes));
			return new AndFilter(list.ToArray());
		}

		internal static QueryFilter BuildFindUnpublishedObjectQuery(string serviceInstance)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, UnpublishedObjectSchema.ServiceInstanceProp, serviceInstance);
		}

		public static readonly HygienePropertyDefinition IsDeletedProp = new HygienePropertyDefinition("isDeleted", typeof(bool), false, ExchangeObjectVersion.Exchange2007, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CooldownProp = new HygienePropertyDefinition("Cooldown", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		private static readonly TimeSpan DefaultCooldown = TimeSpan.FromSeconds(0.0);
	}
}
