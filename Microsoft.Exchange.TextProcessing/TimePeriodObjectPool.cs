using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class TimePeriodObjectPool<TValue> where TValue : IInitialize, new()
	{
		public TimePeriodObjectPool()
		{
			this.internalPools = new ConcurrentBag<TValue>[2];
			this.internalPools[0] = new ConcurrentBag<TValue>();
			this.internalPools[1] = new ConcurrentBag<TValue>();
			this.TimeStamps = new DateTime[2];
			this.TimeStamps[0] = DateTime.UtcNow;
			this.TimeStamps[1] = DateTime.UtcNow;
		}

		public DateTime[] TimeStamps { get; set; }

		public void Clear(int i, DateTime dateTime)
		{
			if (i != 0 && i != 1)
			{
				throw new ArgumentException("Parameter i allow only 0 or 1.", "i");
			}
			ConcurrentBag<TValue> value = new ConcurrentBag<TValue>();
			Interlocked.CompareExchange<ConcurrentBag<TValue>>(ref this.internalPools[i], value, this.internalPools[i]);
			this.TimeStamps[i] = dateTime;
		}

		public TValue GetObject(int i)
		{
			ConcurrentBag<TValue> concurrentBag = this.internalPools[i];
			ConcurrentBag<TValue> concurrentBag2 = this.internalPools[1 - i];
			TValue tvalue;
			if (concurrentBag2.TryTake(out tvalue))
			{
				tvalue.Initialize();
				concurrentBag.Add(tvalue);
				return tvalue;
			}
			if (default(TValue) != null)
			{
				return default(TValue);
			}
			return Activator.CreateInstance<TValue>();
		}

		public void Add(int i, TValue value)
		{
			if (i != 0 && i != 1)
			{
				throw new ArgumentException("Parameter i allow only 0 or 1.", "i");
			}
			this.internalPools[i].Add(value);
		}

		internal int GetInternalPoolSize(int i)
		{
			return this.internalPools[i].Count;
		}

		private ConcurrentBag<TValue>[] internalPools;
	}
}
