using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TenantConfigurationCache<TSettings> : IDisposable where TSettings : TenantConfigurationCacheableItemBase, new()
	{
		public TenantConfigurationCache(long cacheSize, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval, ICacheTracer<OrganizationId> tracer, ICachePerformanceCounters perfCounters) : this(cacheSize, cacheExpirationInterval, cacheCleanupInterval, Cache<OrganizationId, TSettings>.DefaultPurgeInterval, tracer, perfCounters)
		{
		}

		public TenantConfigurationCache(long cacheSize, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval, TimeSpan cachePurgeInterval, ICacheTracer<OrganizationId> tracer, ICachePerformanceCounters perfCounters)
		{
			this.cache = new Cache<OrganizationId, TSettings>(cacheSize, cacheExpirationInterval, cacheCleanupInterval, cachePurgeInterval, tracer, perfCounters);
			this.cache.OnRemoved += this.HandleCacheItemRemoved;
		}

		public bool TryGetValue(OrganizationId orgId, out TSettings perTenantSettings)
		{
			bool flag;
			return this.TryGetValue(orgId, out perTenantSettings, out flag);
		}

		public bool TryGetValue(OrganizationId orgId, out TSettings perTenantSettings, object state)
		{
			bool flag;
			return this.TryGetValue(orgId, false, out perTenantSettings, out flag, state);
		}

		public bool TryGetValue(OrganizationId orgId, out TSettings perTenantSettings, out bool hasExpired)
		{
			return this.TryGetValue(orgId, false, out perTenantSettings, out hasExpired, null);
		}

		public bool TryGetValue(IConfigurationSession adSession, out TSettings perTenantSettings)
		{
			return this.TryGetPerTenantSettingsWithoutRegistrationAndCaching(adSession, false, out perTenantSettings);
		}

		public virtual TSettings GetValue(OrganizationId orgId)
		{
			TSettings result;
			bool flag;
			this.TryGetValue(orgId, true, out result, out flag, null);
			return result;
		}

		public TSettings GetValue(IConfigurationSession adSession)
		{
			TSettings result;
			this.TryGetPerTenantSettingsWithoutRegistrationAndCaching(adSession, true, out result);
			return result;
		}

		public TSettings GetValue(OrganizationId orgId, object state)
		{
			TSettings result;
			bool flag;
			this.TryGetValue(orgId, true, out result, out flag, state);
			return result;
		}

		public bool ContainsInCache(OrganizationId organizationId)
		{
			TSettings tsettings;
			bool flag2;
			bool flag = this.cache.TryGetValue(organizationId, out tsettings, out flag2);
			return flag && !flag2;
		}

		public void Clear()
		{
			this.cache.Clear();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void RemoveValue(OrganizationId orgId)
		{
			this.cache.Remove(orgId);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.cache.Dispose();
				}
				this.disposed = true;
			}
		}

		private bool TryGetValue(OrganizationId orgId, bool allowExceptions, out TSettings perTenantSettings, out bool hasExpired, object state)
		{
			perTenantSettings = default(TSettings);
			if (this.cache.TryGetValue(orgId, out perTenantSettings, out hasExpired))
			{
				TSettings tsettings;
				if (hasExpired && this.InitializeAndAddPerTenantSettings(orgId, allowExceptions, out tsettings, state))
				{
					perTenantSettings = tsettings;
				}
			}
			else if (!this.InitializeAndAddPerTenantSettings(orgId, allowExceptions, out perTenantSettings, state))
			{
				return false;
			}
			return true;
		}

		private bool InitializeAndAddPerTenantSettings(OrganizationId orgId, bool allowExceptions, out TSettings perTenantSettings, object state)
		{
			perTenantSettings = Activator.CreateInstance<TSettings>();
			bool flag = allowExceptions ? perTenantSettings.Initialize(orgId, new CacheNotificationHandler(this.RemoveValue), state) : perTenantSettings.TryInitialize(orgId, new CacheNotificationHandler(this.RemoveValue), state);
			if (flag)
			{
				if (!this.cache.TryAdd(orgId, perTenantSettings))
				{
					perTenantSettings.UnregisterChangeNotification();
				}
				return true;
			}
			return false;
		}

		private bool TryGetPerTenantSettingsWithoutRegistrationAndCaching(IConfigurationSession adSession, bool allowExceptions, out TSettings perTenantSettings)
		{
			perTenantSettings = Activator.CreateInstance<TSettings>();
			return perTenantSettings.InitializeWithoutRegistration(adSession, allowExceptions);
		}

		private void HandleCacheItemRemoved(object sender, OnRemovedEventArgs<OrganizationId, TSettings> e)
		{
			TSettings value = e.Value;
			value.UnregisterChangeNotification();
		}

		private Cache<OrganizationId, TSettings> cache;

		private bool disposed;
	}
}
