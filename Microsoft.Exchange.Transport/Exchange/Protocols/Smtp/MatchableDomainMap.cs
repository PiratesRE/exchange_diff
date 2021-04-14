using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MatchableDomainMap<T> : IEnumerable<KeyValuePair<MatchableDomain, T>>, IEnumerable
	{
		public MatchableDomainMap()
		{
			this.list = new List<KeyValuePair<MatchableDomain, T>>();
		}

		public MatchableDomainMap(int capacity)
		{
			this.list = new List<KeyValuePair<MatchableDomain, T>>(capacity);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void Add(MatchableDomain domain, T value)
		{
			ArgumentValidator.ThrowIfNull("domain", domain);
			KeyValuePair<MatchableDomain, T> item = new KeyValuePair<MatchableDomain, T>(domain, value);
			int num = this.list.BinarySearch(item, MatchableDomainMap<T>.MatchableDomainComparer.Comparer);
			if (num < 0)
			{
				num = ~num;
			}
			this.list.Insert(num, item);
		}

		public IEnumerator<KeyValuePair<MatchableDomain, T>> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private readonly List<KeyValuePair<MatchableDomain, T>> list;

		private class MatchableDomainComparer : IComparer<KeyValuePair<MatchableDomain, T>>
		{
			public static MatchableDomainMap<T>.MatchableDomainComparer Comparer
			{
				get
				{
					return MatchableDomainMap<T>.MatchableDomainComparer.comparer;
				}
			}

			public int Compare(KeyValuePair<MatchableDomain, T> x, KeyValuePair<MatchableDomain, T> y)
			{
				if (x.Key.Domain.IncludeSubDomains == y.Key.Domain.IncludeSubDomains)
				{
					return y.Key.DotCount - x.Key.DotCount;
				}
				if (!x.Key.Domain.IncludeSubDomains)
				{
					return -1;
				}
				return 1;
			}

			private static MatchableDomainMap<T>.MatchableDomainComparer comparer = new MatchableDomainMap<T>.MatchableDomainComparer();
		}
	}
}
