using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Collections
{
	internal sealed class SimpleTimeoutCache<T> : IDisposable
	{
		public SimpleTimeoutCache(TimeSpan expiration, TimeSpan purgeFrequency)
		{
			this.expiration = expiration;
			this.items = new Dictionary<T, DateTime>();
			this.timer = new Timer(new TimerCallback(this.Expire), null, purgeFrequency, purgeFrequency);
		}

		public void Dispose()
		{
			this.timer.Dispose();
		}

		public event Action<int> CountChanged;

		public void Add(T key)
		{
			int count;
			int count2;
			lock (this.items)
			{
				count = this.items.Count;
				this.items[key] = DateTime.UtcNow + this.expiration;
				count2 = this.items.Count;
			}
			this.RaiseCountChanged(count, count2);
		}

		public bool Contains(T key)
		{
			bool result;
			lock (this.items)
			{
				DateTime t;
				if (this.items.TryGetValue(key, out t))
				{
					result = (t > DateTime.UtcNow);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void Expire(object notUsed)
		{
			int count;
			int count2;
			lock (this.items)
			{
				count = this.items.Count;
				List<T> list = new List<T>(10);
				foreach (KeyValuePair<T, DateTime> keyValuePair in this.items)
				{
					if (keyValuePair.Value < DateTime.UtcNow)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (T key in list)
				{
					this.items.Remove(key);
				}
				count2 = this.items.Count;
			}
			this.RaiseCountChanged(count, count2);
		}

		private void RaiseCountChanged(int oldCount, int newCount)
		{
			if (newCount != oldCount && this.CountChanged != null)
			{
				this.CountChanged(newCount);
			}
		}

		private readonly TimeSpan expiration;

		private readonly Dictionary<T, DateTime> items;

		private readonly Timer timer;
	}
}
