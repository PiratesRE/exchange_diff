using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationCache
	{
		internal OrganizationCache()
		{
			this.cache = new Dictionary<object, OrganizationCache.MiniPropertyBag>();
			this.cacheLock = new ReaderWriterLockSlim();
			this.concurrencyLocks = new Dictionary<string, object>();
			this.concurrencyLocksGuard = new ReaderWriterLockSlim();
		}

		internal bool TryLookupCache<T>(ICacheConsumer consumer, string attribute, out T result)
		{
			try
			{
				this.cacheLock.EnterReadLock();
				OrganizationCache.MiniPropertyBag miniPropertyBag;
				if (this.cache.TryGetValue(consumer.Id, out miniPropertyBag) && miniPropertyBag.TryGet<T>(attribute, out result))
				{
					return true;
				}
			}
			finally
			{
				try
				{
					this.cacheLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			result = default(T);
			return false;
		}

		internal T Get<T>(ICacheConsumer consumer, string attribute, Func<T> invokeQuery)
		{
			T t;
			if (this.TryLookupCache<T>(consumer, attribute, out t))
			{
				return t;
			}
			T result;
			lock (this.GetConcurrencyLock(attribute))
			{
				if (this.TryLookupCache<T>(consumer, attribute, out t))
				{
					result = t;
				}
				else
				{
					t = StorageGlobals.ProtectedADCall<T>(invokeQuery);
					this.cacheLock.EnterWriteLock();
					try
					{
						OrganizationCache.MiniPropertyBag miniPropertyBag;
						if (!this.cache.TryGetValue(consumer.Id, out miniPropertyBag))
						{
							miniPropertyBag = new OrganizationCache.MiniPropertyBag();
							this.cache[consumer.Id] = miniPropertyBag;
						}
						miniPropertyBag[attribute] = t;
						result = t;
					}
					finally
					{
						this.cacheLock.ExitWriteLock();
					}
				}
			}
			return result;
		}

		internal void InvalidateCache(object id)
		{
			try
			{
				this.cacheLock.EnterWriteLock();
				if (this.testBridge != null)
				{
					this.testBridge(id);
				}
				this.cache.Remove(id);
			}
			finally
			{
				try
				{
					this.cacheLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		private object GetConcurrencyLock(string attribute)
		{
			try
			{
				this.concurrencyLocksGuard.EnterReadLock();
				object obj;
				if (this.concurrencyLocks.TryGetValue(attribute, out obj))
				{
					return obj;
				}
			}
			finally
			{
				try
				{
					this.concurrencyLocksGuard.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			object result;
			try
			{
				this.concurrencyLocksGuard.EnterWriteLock();
				object obj;
				if (this.concurrencyLocks.TryGetValue(attribute, out obj))
				{
					result = obj;
				}
				else
				{
					this.concurrencyLocks[attribute] = new object();
					result = this.concurrencyLocks[attribute];
				}
			}
			finally
			{
				try
				{
					this.concurrencyLocksGuard.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		internal Action<object> TestBridge
		{
			get
			{
				return this.testBridge;
			}
			set
			{
				this.testBridge = value;
			}
		}

		private readonly ReaderWriterLockSlim cacheLock;

		private readonly Dictionary<object, OrganizationCache.MiniPropertyBag> cache;

		private readonly Dictionary<string, object> concurrencyLocks;

		private readonly ReaderWriterLockSlim concurrencyLocksGuard;

		private Action<object> testBridge;

		internal class MiniPropertyBag
		{
			internal MiniPropertyBag()
			{
				this.cache = new Dictionary<string, object>();
			}

			internal object this[string attribute]
			{
				set
				{
					this.cache[attribute] = value;
				}
			}

			internal bool TryGet<T>(string attribute, out T value)
			{
				object obj;
				if (this.cache.TryGetValue(attribute, out obj))
				{
					value = (T)((object)obj);
					return true;
				}
				value = default(T);
				return false;
			}

			private readonly Dictionary<string, object> cache;
		}
	}
}
