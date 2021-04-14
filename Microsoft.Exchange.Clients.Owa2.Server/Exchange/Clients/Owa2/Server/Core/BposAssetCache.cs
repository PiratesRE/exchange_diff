using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class BposAssetCache<T> : LazyLookupTimeoutCache<string, T> where T : class
	{
		internal BposAssetCache() : base(5, 100, false, TimeSpan.FromDays(1.0), TimeSpan.FromDays(7.0))
		{
		}

		internal static BposAssetCache<T> Instance
		{
			get
			{
				return BposAssetCache<T>.instance;
			}
		}

		internal void Add(string key, T value)
		{
			base.Remove(key);
			this.TryPerformAdd(key, value);
		}

		protected override T CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			shouldAdd = false;
			return default(T);
		}

		private static BposAssetCache<T> instance = new BposAssetCache<T>();
	}
}
