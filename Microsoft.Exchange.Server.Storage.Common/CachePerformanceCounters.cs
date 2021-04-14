using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class CachePerformanceCounters<T> : ICachePerformanceCounters where T : class
	{
		public ExPerformanceCounter CacheSize
		{
			get
			{
				return this.sizeCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheLookups
		{
			get
			{
				return this.lookupsCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheMisses
		{
			get
			{
				return this.missesCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheHits
		{
			get
			{
				return this.hitsCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheInserts
		{
			get
			{
				return this.insertsCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheRemoves
		{
			get
			{
				return this.removesCounter(this.instanceAccessor());
			}
		}

		public ExPerformanceCounter CacheExpirationQueueLength
		{
			get
			{
				return this.expirationQueueLengthCounter(this.instanceAccessor());
			}
		}

		public CachePerformanceCounters(Func<T> instanceAccessor, Func<T, ExPerformanceCounter> sizeCounter, Func<T, ExPerformanceCounter> lookupsCounter, Func<T, ExPerformanceCounter> missesCounter, Func<T, ExPerformanceCounter> hitsCounter, Func<T, ExPerformanceCounter> insertsCounter, Func<T, ExPerformanceCounter> removesCounter, Func<T, ExPerformanceCounter> expirationQueueLengthCounter)
		{
			this.instanceAccessor = instanceAccessor;
			this.sizeCounter = sizeCounter;
			this.lookupsCounter = lookupsCounter;
			this.missesCounter = missesCounter;
			this.hitsCounter = hitsCounter;
			this.insertsCounter = insertsCounter;
			this.removesCounter = removesCounter;
			this.expirationQueueLengthCounter = expirationQueueLengthCounter;
		}

		private Func<T> instanceAccessor;

		private Func<T, ExPerformanceCounter> sizeCounter;

		private Func<T, ExPerformanceCounter> lookupsCounter;

		private Func<T, ExPerformanceCounter> missesCounter;

		private Func<T, ExPerformanceCounter> hitsCounter;

		private Func<T, ExPerformanceCounter> insertsCounter;

		private Func<T, ExPerformanceCounter> removesCounter;

		private Func<T, ExPerformanceCounter> expirationQueueLengthCounter;
	}
}
