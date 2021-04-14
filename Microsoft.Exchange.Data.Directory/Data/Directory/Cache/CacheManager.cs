using System;
using System.Runtime.Caching;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal sealed class CacheManager
	{
		private CacheManager()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.keyTable = new MemoryCache("KeyTable", null);
			this.adObjectCache = new MemoryCache("ADObjectCache", null);
		}

		public static CacheManager Instance
		{
			get
			{
				if (CacheManager.singleton == null)
				{
					CacheManager value = new CacheManager();
					Interlocked.CompareExchange<CacheManager>(ref CacheManager.singleton, value, null);
				}
				return CacheManager.singleton;
			}
		}

		public MemoryCache KeyTable
		{
			get
			{
				return this.keyTable;
			}
		}

		public MemoryCache ADObjectCache
		{
			get
			{
				return this.adObjectCache;
			}
		}

		internal void ResetAllCaches()
		{
			MemoryCache memoryCache = this.KeyTable;
			MemoryCache adobjectCache = this.ADObjectCache;
			this.Initialize();
			memoryCache.Dispose();
			adobjectCache.Dispose();
		}

		private static CacheManager singleton;

		private MemoryCache keyTable;

		private MemoryCache adObjectCache;
	}
}
