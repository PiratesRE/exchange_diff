using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal class SimpleIndex<TItem, TKey> : IIndex<TItem>
	{
		public SimpleIndex(Func<TItem, IEnumerable<TKey>> keySelector)
		{
			int maxRunningTasks = Settings.MaxRunningTasks;
			this.dictionary = new ConcurrentDictionary<TKey, ConcurrentQueue<TItem>>(maxRunningTasks, 1024, SimpleIndex<TItem, TKey>.keyComparer);
			this.keySelector = keySelector;
		}

		public void Add(TItem item, TracingContext traceContext)
		{
			foreach (TKey tkey in this.keySelector(item))
			{
				if (tkey != null)
				{
					ConcurrentQueue<TItem> orAdd = this.dictionary.GetOrAdd(tkey, (TKey k) => new ConcurrentQueue<TItem>());
					orAdd.Enqueue(item);
				}
			}
		}

		public IEnumerable<TItem> GetItems(TKey key)
		{
			ConcurrentQueue<TItem> result;
			if (!this.dictionary.TryGetValue(key, out result))
			{
				return Enumerable.Empty<TItem>();
			}
			return result;
		}

		private const int DictionarySize = 1024;

		private static KeyComparer<TKey> keyComparer = new KeyComparer<TKey>();

		private ConcurrentDictionary<TKey, ConcurrentQueue<TItem>> dictionary;

		private Func<TItem, IEnumerable<TKey>> keySelector;
	}
}
