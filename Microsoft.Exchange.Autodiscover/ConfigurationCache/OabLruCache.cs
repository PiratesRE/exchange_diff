using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	public class OabLruCache<TKey, TValue> : LRUCache<TKey, TValue> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		public OabLruCache(int capacity, Func<TKey, TValue> loadItem, Func<TValue, IEnumerable<TKey>> loadExtraKeys, Predicate<TValue> forceReload) : base(capacity, loadItem, null, null, null, null, null, null)
		{
			this.loadExtraKeys = (loadExtraKeys ?? OabLruCache<TKey, TValue>.LoadExtraKeysDefault);
			this.forceReload = (forceReload ?? OabLruCache<TKey, TValue>.forceReloadDefault);
		}

		protected override bool TryLoadFromCache(TKey key, out TValue value)
		{
			return base.TryLoadFromCache(key, out value) && !this.forceReload(value);
		}

		protected override TValue AddNewItem(TKey key, TValue value, ref bool elementEvicted)
		{
			TValue tvalue = base.AddNewItem(key, value, ref elementEvicted);
			foreach (TKey key2 in this.loadExtraKeys(tvalue))
			{
				base.AddNewItem(key2, tvalue, ref elementEvicted);
			}
			return tvalue;
		}

		private static readonly IEnumerable<TKey> emptyList = new List<TKey>();

		private static readonly Func<TValue, IEnumerable<TKey>> LoadExtraKeysDefault = (TValue value) => OabLruCache<TKey, TValue>.emptyList;

		private static readonly Predicate<TValue> forceReloadDefault = (TValue value) => false;

		private readonly Func<TValue, IEnumerable<TKey>> loadExtraKeys;

		private readonly Predicate<TValue> forceReload;
	}
}
