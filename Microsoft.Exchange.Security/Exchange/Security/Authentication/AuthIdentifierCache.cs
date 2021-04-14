using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuthIdentifierCache
	{
		public AuthIdentifierCache(int partitions, int buckets, TimeSpan lifetime)
		{
			if (partitions < 1)
			{
				throw new ArgumentException("Number of partitions must be at least one.");
			}
			this.partitions = new AuthIdentifierCache.CachePartition[partitions];
			for (int i = 0; i < partitions; i++)
			{
				this.partitions[i] = new AuthIdentifierCache.CachePartition(buckets, lifetime);
			}
		}

		public string Lookup(string identityKey)
		{
			int num = this.ComputePartitionIndex(identityKey);
			return this.partitions[num].Lookup(identityKey);
		}

		public void Add(string identityKey, string authIdentifierInfo)
		{
			int num = this.ComputePartitionIndex(identityKey);
			this.partitions[num].Add(identityKey, authIdentifierInfo);
		}

		private int ComputePartitionIndex(string identityKey)
		{
			return Math.Abs(identityKey.GetHashCode()) % this.partitions.Length;
		}

		private readonly AuthIdentifierCache.CachePartition[] partitions;

		private class CachePartition
		{
			public CachePartition(int numBuckets, TimeSpan lifetime)
			{
				if (numBuckets < 2)
				{
					throw new ArgumentException("Number of buckets must be two or more.");
				}
				if (lifetime < TimeSpan.FromMinutes(1.0))
				{
					throw new ArgumentException("Lifetime must be at least one minute.");
				}
				this.buckets = new ConcurrentDictionary<string, string>[numBuckets];
				for (int i = 0; i < numBuckets; i++)
				{
					this.buckets[i] = new ConcurrentDictionary<string, string>();
				}
				this.bucketLifetime = (uint)lifetime.TotalMilliseconds / (uint)numBuckets;
			}

			public string Lookup(string identityKey)
			{
				this.FlushCheck();
				string result = null;
				ConcurrentDictionary<string, string>[] array = this.buckets;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].TryGetValue(identityKey, out result))
					{
						return result;
					}
				}
				return null;
			}

			public void Add(string identityKey, string authIdentifierInfo)
			{
				this.FlushCheck();
				ConcurrentDictionary<string, string>[] array = this.buckets;
				ConcurrentDictionary<string, string> concurrentDictionary = array[0];
				concurrentDictionary[identityKey] = authIdentifierInfo;
			}

			private void FlushCheck()
			{
				if (this.flushInProgress)
				{
					return;
				}
				uint num = (uint)(Environment.TickCount - (int)this.lastFlush);
				if (num <= this.bucketLifetime)
				{
					return;
				}
				lock (this.flushCheckLock)
				{
					if (this.flushInProgress)
					{
						return;
					}
					num = (uint)(Environment.TickCount - (int)this.lastFlush);
					if (num <= this.bucketLifetime)
					{
						return;
					}
					this.flushInProgress = true;
				}
				try
				{
					int num2 = (int)Math.Max(Math.Min(num / this.bucketLifetime, 1U), (uint)this.buckets.Length);
					ConcurrentDictionary<string, string>[] array = new ConcurrentDictionary<string, string>[this.buckets.Length];
					if (num2 < this.buckets.Length)
					{
						for (int i = this.buckets.Length - 1; i >= num2; i--)
						{
							array[i] = this.buckets[i - num2];
						}
					}
					for (int j = 0; j < num2; j++)
					{
						array[j] = new ConcurrentDictionary<string, string>();
					}
					this.buckets = array;
					this.lastFlush = (uint)Environment.TickCount;
				}
				finally
				{
					this.flushInProgress = false;
				}
			}

			private readonly object flushCheckLock = new object();

			private readonly uint bucketLifetime;

			private ConcurrentDictionary<string, string>[] buckets;

			private uint lastFlush = (uint)Environment.TickCount;

			private bool flushInProgress;
		}
	}
}
