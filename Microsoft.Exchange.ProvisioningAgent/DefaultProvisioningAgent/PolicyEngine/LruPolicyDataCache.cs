using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	internal class LruPolicyDataCache : IDictionary<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>, ICollection<KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>>, IEnumerable<KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>>, IEnumerable
	{
		public LruPolicyDataCache(int bucketSize)
		{
			if (bucketSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bucketSize");
			}
			this.bucketSize = bucketSize;
			int capacity = Math.Min(16, bucketSize);
			this.bucket = new Dictionary<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>(capacity);
			this.lruList = new LinkedList<PolicyDataCacheKey>();
		}

		public int BucketSize
		{
			get
			{
				return this.bucketSize;
			}
		}

		private void MarkAsMostRecentlyUsed(PolicyDataCacheKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			lock (this.syncRoot)
			{
				LinkedListNode<PolicyDataCacheKey> linkedListNode = this.lruList.Find(key);
				if (linkedListNode != null && linkedListNode.Next != null)
				{
					this.lruList.Remove(linkedListNode);
					this.lruList.AddLast(linkedListNode);
				}
			}
		}

		public void Add(PolicyDataCacheKey key, IEnumerable<ADProvisioningPolicy> value)
		{
			lock (this.syncRoot)
			{
				this.bucket.Add(key, value);
				this.lruList.AddLast(key);
				if (this.BucketSize < this.bucket.Count)
				{
					this.bucket.Remove(this.lruList.First.Value);
					this.lruList.RemoveFirst();
				}
			}
		}

		public bool ContainsKey(PolicyDataCacheKey key)
		{
			return this.bucket.ContainsKey(key);
		}

		public ICollection<PolicyDataCacheKey> Keys
		{
			get
			{
				return this.bucket.Keys;
			}
		}

		public bool Remove(PolicyDataCacheKey key)
		{
			bool result;
			lock (this.syncRoot)
			{
				bool flag2 = this.bucket.Remove(key);
				this.lruList.Remove(key);
				result = flag2;
			}
			return result;
		}

		public bool TryGetValue(PolicyDataCacheKey key, out IEnumerable<ADProvisioningPolicy> value)
		{
			this.MarkAsMostRecentlyUsed(key);
			return this.bucket.TryGetValue(key, out value);
		}

		public ICollection<IEnumerable<ADProvisioningPolicy>> Values
		{
			get
			{
				return this.bucket.Values;
			}
		}

		public IEnumerable<ADProvisioningPolicy> this[PolicyDataCacheKey key]
		{
			get
			{
				this.MarkAsMostRecentlyUsed(key);
				return this.bucket[key];
			}
			set
			{
				lock (this.syncRoot)
				{
					this.bucket[key] = value;
				}
			}
		}

		public void Add(KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			lock (this.syncRoot)
			{
				this.bucket.Clear();
				this.lruList.Clear();
			}
		}

		public bool Contains(KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get
			{
				return this.bucket.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>> item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private int bucketSize;

		private Dictionary<PolicyDataCacheKey, IEnumerable<ADProvisioningPolicy>> bucket;

		private LinkedList<PolicyDataCacheKey> lruList;

		private object syncRoot = new object();
	}
}
