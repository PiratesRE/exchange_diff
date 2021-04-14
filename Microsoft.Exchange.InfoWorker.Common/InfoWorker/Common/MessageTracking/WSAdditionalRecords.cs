using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class WSAdditionalRecords<K, T> where K : class where T : class
	{
		internal WSAdditionalRecords(QueryMethod<K, T> queryMethod)
		{
			this.queryMethod = queryMethod;
		}

		internal T FindAndCache(K key, bool bypassCache)
		{
			T t = default(T);
			if (this.recordCache != null && this.recordCache.TryGetValue(key.ToString(), out t) && !bypassCache)
			{
				return t;
			}
			if (this.recordCache == null)
			{
				this.recordCache = new Dictionary<string, T>(20, StringComparer.OrdinalIgnoreCase);
			}
			KeyValuePair<K, T>[] array;
			t = this.queryMethod(key, t, out array);
			if (this.recordCache.Count + array.Length > 32)
			{
				this.recordCache.Clear();
			}
			this.recordCache[key.ToString()] = t;
			foreach (KeyValuePair<K, T> keyValuePair in array)
			{
				Dictionary<string, T> dictionary = this.recordCache;
				K key2 = keyValuePair.Key;
				dictionary[key2.ToString()] = keyValuePair.Value;
			}
			return t;
		}

		private const int MaxSize = 32;

		private QueryMethod<K, T> queryMethod;

		private Dictionary<string, T> recordCache;
	}
}
