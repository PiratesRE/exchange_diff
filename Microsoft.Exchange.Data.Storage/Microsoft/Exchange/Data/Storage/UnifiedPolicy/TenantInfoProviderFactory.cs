using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantInfoProviderFactory : ITenantInfoProviderFactory
	{
		public TenantInfoProviderFactory(TimeSpan cacheExpiry, int cacheBucket = 10, int cacheBucketSize = 1000)
		{
			this.tenantInfoProviderCache = new TenantInfoProviderFactory.TenantInfoProviderCache(cacheExpiry, cacheBucket, cacheBucketSize);
		}

		public ITenantInfoProvider CreateTenantInfoProvider(TenantContext tenantContext)
		{
			ITenantInfoProvider result;
			try
			{
				result = this.tenantInfoProviderCache.Get(tenantContext.TenantId);
			}
			catch (ADTransientException innerException)
			{
				throw new SyncAgentTransientException("CreateTenantInfoProvider failed with ADTransientException", innerException);
			}
			catch (DataSourceOperationException innerException2)
			{
				throw new SyncAgentPermanentException("CreateTenantInfoProvider failed with SyncAgentPermanentException", innerException2);
			}
			catch (DataValidationException innerException3)
			{
				throw new SyncAgentPermanentException("CreateTenantInfoProvider failed with DataValidationException", innerException3);
			}
			catch (StoragePermanentException innerException4)
			{
				throw new SyncAgentPermanentException("CreateTenantInfoProvider failed with StoragePermanentException", innerException4);
			}
			catch (StorageTransientException innerException5)
			{
				throw new SyncAgentTransientException("CreateTenantInfoProvider failed with StorageTransientException", innerException5);
			}
			return result;
		}

		private readonly TenantInfoProviderFactory.TenantInfoProviderCache tenantInfoProviderCache;

		private sealed class TenantInfoProviderCache : LazyLookupTimeoutCache<Guid, TenantInfoProvider>
		{
			public TenantInfoProviderCache(TimeSpan cacheExpiry, int cacheBucket = 10, int cacheBucketSize = 1001) : base(cacheBucket, cacheBucketSize, false, cacheExpiry)
			{
			}

			protected override TenantInfoProvider CreateOnCacheMiss(Guid key, ref bool shouldAdd)
			{
				OrganizationId scopingOrganizationId = OrganizationId.FromExternalDirectoryOrganizationId(key);
				ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 99, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\TenantInfoProviderFactory.cs");
				ADUser discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession);
				ExchangePrincipal syncMailboxPrincipal = ExchangePrincipal.FromADUser(adsessionSettings, discoveryMailbox);
				return new TenantInfoProvider(syncMailboxPrincipal);
			}
		}
	}
}
