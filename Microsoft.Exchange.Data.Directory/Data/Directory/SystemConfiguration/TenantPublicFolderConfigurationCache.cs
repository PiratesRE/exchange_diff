using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantPublicFolderConfigurationCache : TenantConfigurationCache<TenantPublicFolderConfiguration>
	{
		public static TenantPublicFolderConfigurationCache Instance
		{
			get
			{
				return TenantPublicFolderConfigurationCache.instance;
			}
		}

		private TenantPublicFolderConfigurationCache() : base(TenantPublicFolderConfigurationCache.CacheSizeInBytes, CacheTimeToLive.OrgPropertyCacheTimeToLive, TimeSpan.Zero, null, null)
		{
		}

		private static readonly long CacheSizeInBytes = (long)ByteQuantifiedSize.FromMB(10UL).ToBytes();

		private static TenantPublicFolderConfigurationCache instance = new TenantPublicFolderConfigurationCache();
	}
}
