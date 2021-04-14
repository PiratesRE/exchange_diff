using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class ListLoadBalancer<T>
	{
		public ListLoadBalancer(bool randomLoadBalancingOffsetEnabled) : this(null, randomLoadBalancingOffsetEnabled)
		{
		}

		public ListLoadBalancer(IList<T> list, bool randomLoadBalancingOffsetEnabled)
		{
			if (list != null)
			{
				this.list = new List<T>(list);
			}
			if (randomLoadBalancingOffsetEnabled)
			{
				this.roundRobinBase = RoutingUtils.GetRandomNumber(10000);
				return;
			}
			this.roundRobinBase = -1;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public int Count
		{
			get
			{
				if (this.list != null)
				{
					return this.list.Count;
				}
				return 0;
			}
		}

		public ICollection<T> NonLoadBalancedCollection
		{
			get
			{
				return this.NonLoadBalancedList;
			}
		}

		public List<T> NonLoadBalancedList
		{
			get
			{
				if (this.list == null)
				{
					return ListLoadBalancer<T>.emptyList;
				}
				return this.list;
			}
		}

		public LoadBalancedCollection<T> LoadBalancedCollection
		{
			get
			{
				return new LoadBalancedCollection<T>(this.NonLoadBalancedList, Interlocked.Increment(ref this.roundRobinBase));
			}
		}

		public void AddItem(T item)
		{
			RoutingUtils.AddItemToLazyList<T>(item, ref this.list);
		}

		public void RemoveItem(T item)
		{
			if (this.list != null)
			{
				this.list.Remove(item);
			}
		}

		private static readonly List<T> emptyList = new List<T>();

		private List<T> list;

		private int roundRobinBase;
	}
}
