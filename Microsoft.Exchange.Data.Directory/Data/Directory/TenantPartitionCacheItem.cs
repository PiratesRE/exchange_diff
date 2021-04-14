using System;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantPartitionCacheItem : CachableItem
	{
		internal TenantPartitionCacheItem(Guid accountPartitionGuid, string accountPartitionFqdn, string resourceForestFqdn, Guid externalOrgId, string tenantName, bool dataFromOfflineService = false)
		{
			ArgumentValidator.ThrowIfNullOrEmpty(accountPartitionFqdn, "accountPartitionFqdn");
			ArgumentValidator.ThrowIfNullOrEmpty(resourceForestFqdn, "resourceForestFqdn");
			if (PartitionId.IsLocalForestPartition(accountPartitionFqdn))
			{
				this.accountPartitionId = PartitionId.LocalForest;
			}
			else
			{
				this.accountPartitionId = ((accountPartitionGuid == Guid.Empty) ? new PartitionId(accountPartitionFqdn) : new PartitionId(accountPartitionFqdn, accountPartitionGuid));
			}
			this.resourceForestFqdn = resourceForestFqdn;
			this.externalOrgId = externalOrgId;
			this.tenantName = tenantName;
			this.expirationTime = DateTime.UtcNow + TenantPartitionCacheItem.CalculateCacheItemExpirationWindow(dataFromOfflineService, tenantName, this.externalOrgId, this.accountPartitionId);
		}

		private static TimeSpan CalculateCacheItemExpirationWindow(bool dataFromOfflineService, string tenantName, Guid externalOrgId, PartitionId accountPartitionId)
		{
			if (dataFromOfflineService)
			{
				return TimeSpan.FromMinutes((double)ConfigBase<AdDriverConfigSchema>.GetConfig<int>("OfflineDataCacheExpirationTimeInMinutes"));
			}
			if (!ForestTenantRelocationsCache.IsTenantRelocationAllowed(accountPartitionId.ForestFQDN))
			{
				return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.DefaultExpirationWindow;
			}
			if (!string.IsNullOrEmpty(tenantName))
			{
				return TenantRelocationStateCache.GetTenantCacheExpirationWindow(tenantName, accountPartitionId);
			}
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(accountPartitionId), 110, "CalculateCacheItemExpirationWindow", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\TenantPartitionCacheItem.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByExternalId = tenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(externalOrgId);
			if (exchangeConfigurationUnitByExternalId != null)
			{
				tenantName = ((ADObjectId)exchangeConfigurationUnitByExternalId.Identity).Parent.Name;
				return TenantRelocationStateCache.GetTenantCacheExpirationWindow(tenantName, accountPartitionId);
			}
			return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.DefaultExpirationWindow;
		}

		public override long ItemSize
		{
			get
			{
				return (long)(32 + ((this.tenantName == null) ? 0 : (this.tenantName.Length * 2)) + this.resourceForestFqdn.Length * 2);
			}
		}

		public override bool IsExpired(DateTime currentTime)
		{
			return this.expirationTime < currentTime || base.IsExpired(currentTime);
		}

		internal PartitionId AccountPartitionId
		{
			get
			{
				return this.accountPartitionId;
			}
		}

		internal string ResourceForestFqdn
		{
			get
			{
				return this.resourceForestFqdn;
			}
		}

		internal Guid ExternalOrgId
		{
			get
			{
				return this.externalOrgId;
			}
		}

		internal string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		internal bool IsRegisteredAccountPartition
		{
			get
			{
				return this.AccountPartitionId == PartitionId.LocalForest || (this.AccountPartitionId.PartitionObjectId != null && this.AccountPartitionId.PartitionObjectId.Value != Guid.Empty);
			}
		}

		private readonly PartitionId accountPartitionId;

		private readonly string resourceForestFqdn;

		private readonly Guid externalOrgId;

		private readonly string tenantName;

		private readonly DateTime expirationTime;
	}
}
