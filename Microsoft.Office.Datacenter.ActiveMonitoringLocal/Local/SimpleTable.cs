using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal abstract class SimpleTable<TItem, TClusteredIndexKey, TSegment> : ITable<TItem>, ITable
	{
		public SimpleTable(IIndexDescriptor<TItem, TClusteredIndexKey> primaryKeyDescriptor)
		{
			int maxRunningTasks = Settings.MaxRunningTasks;
			this.dictionary = new ConcurrentDictionary<TClusteredIndexKey, TSegment>(maxRunningTasks, 1024, SimpleTable<TItem, TClusteredIndexKey, TSegment>.keyComparer);
			this.indexes = new Dictionary<Type, IIndex<TSegment>>();
			this.primaryIndexDescriptor = primaryKeyDescriptor;
		}

		public IEnumerable<TItem> GetItems<TKey>(IIndexDescriptor<TItem, TKey> indexDescriptor)
		{
			Func<KeyValuePair<TClusteredIndexKey, TSegment>, TSegment> func = null;
			IEnumerable<TItem> query;
			IIndex<TSegment> index;
			if (indexDescriptor is IIndexDescriptor<TItem, TClusteredIndexKey>)
			{
				IIndexDescriptor<TItem, TClusteredIndexKey> indexDescriptor2 = indexDescriptor as IIndexDescriptor<TItem, TClusteredIndexKey>;
				TSegment segment;
				if (this.dictionary.TryGetValue(indexDescriptor2.Key, out segment))
				{
					query = this.GetItemsFromSegment<TClusteredIndexKey>(segment, indexDescriptor2);
				}
				else
				{
					query = Enumerable.Empty<TItem>();
				}
			}
			else if (this.indexes.TryGetValue(indexDescriptor.GetType(), out index))
			{
				SimpleIndex<TSegment, TKey> simpleIndex = (SimpleIndex<TSegment, TKey>)index;
				IEnumerable<TSegment> items = simpleIndex.GetItems(indexDescriptor.Key);
				query = this.GetItemsFromSegments<TKey>(items, indexDescriptor);
			}
			else
			{
				IEnumerable<KeyValuePair<TClusteredIndexKey, TSegment>> source = this.dictionary;
				if (func == null)
				{
					func = ((KeyValuePair<TClusteredIndexKey, TSegment> pair) => pair.Value);
				}
				IEnumerable<TSegment> segments = source.Select(func);
				query = this.GetItemsFromSegments<TKey>(segments, indexDescriptor);
			}
			return indexDescriptor.ApplyIndexRestriction(SimpleTable<TItem, TClusteredIndexKey, TSegment>.dataAccess.AsDataAccessQuery<TItem>(query));
		}

		public void Insert(TItem item, TracingContext traceContext)
		{
			TClusteredIndexKey key = this.primaryIndexDescriptor.GetKeyValues(item).First<TClusteredIndexKey>();
			TSegment orAdd = this.dictionary.GetOrAdd(key, (TClusteredIndexKey k) => this.CreateSegment(item));
			bool flag = this.AddToSegment(orAdd, item);
			if (flag && this.indexes != null)
			{
				foreach (Type key2 in this.indexes.Keys)
				{
					this.indexes[key2].Add(orAdd, traceContext);
				}
			}
		}

		public void AddIndex<TKey>(IIndexDescriptor<TItem, TKey> indexDescriptor)
		{
			if (this.dictionary.Count != 0)
			{
				throw new NotSupportedException("Indexes cannot be added once rows have been added to the table");
			}
			this.indexes[indexDescriptor.GetType()] = new SimpleIndex<TSegment, TKey>((TSegment segment) => indexDescriptor.GetKeyValues(this.GetItemsFromSegment<TKey>(segment, indexDescriptor).First<TItem>()));
		}

		protected abstract IEnumerable<TItem> GetItemsFromSegment<TKey>(TSegment segment, IIndexDescriptor<TItem, TKey> indexDescriptor);

		protected abstract IEnumerable<TItem> GetItemsFromSegments<TKey>(IEnumerable<TSegment> segments, IIndexDescriptor<TItem, TKey> indexDescriptor);

		protected abstract TSegment CreateSegment(TItem item);

		protected abstract bool AddToSegment(TSegment segment, TItem item);

		private const int DictionarySize = 1024;

		private static LocalDataAccess dataAccess = new LocalDataAccess();

		private static KeyComparer<TClusteredIndexKey> keyComparer = new KeyComparer<TClusteredIndexKey>();

		private ConcurrentDictionary<TClusteredIndexKey, TSegment> dictionary;

		private Dictionary<Type, IIndex<TSegment>> indexes;

		private IIndexDescriptor<TItem, TClusteredIndexKey> primaryIndexDescriptor;
	}
}
