using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class ThreadSafeCache<TKey, TValue> : DisposeTrackableBase
	{
		public ThreadSafeCache(ThreadSafeCache<TKey, TValue>.ValueCreatorDelegate valCreator, ThreadSafeCache<TKey, TValue>.ValueDestroyerDelegate valDestroyer) : this(valCreator, valDestroyer, null)
		{
		}

		public ThreadSafeCache(ThreadSafeCache<TKey, TValue>.ValueCreatorDelegate valCreator, ThreadSafeCache<TKey, TValue>.ValueDestroyerDelegate valDestroyer, IEqualityComparer<TKey> comparer)
		{
			if (valCreator == null)
			{
				throw new ArgumentNullException("valCreator");
			}
			this.ValueCreator = valCreator;
			this.ValueDestroyer = valDestroyer;
			this.Dictionary = ((comparer == null) ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(comparer));
			this.Lock = new ReaderWriterLockSlim();
		}

		private Dictionary<TKey, TValue> Dictionary { get; set; }

		private ThreadSafeCache<TKey, TValue>.ValueCreatorDelegate ValueCreator { get; set; }

		private ThreadSafeCache<TKey, TValue>.ValueDestroyerDelegate ValueDestroyer { get; set; }

		private ReaderWriterLockSlim Lock { get; set; }

		public bool NonCached { get; set; }

		public TValue this[TKey key]
		{
			get
			{
				if (this.NonCached)
				{
					this.Remove(key);
				}
				base.CheckDisposed();
				try
				{
					this.Lock.EnterReadLock();
					if (this.Dictionary.ContainsKey(key))
					{
						return this.Dictionary[key];
					}
				}
				finally
				{
					try
					{
						this.Lock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				TValue tvalue = this.ValueCreator(key);
				bool flag = true;
				TValue result;
				try
				{
					try
					{
						this.Lock.EnterUpgradeableReadLock();
						if (this.Dictionary.ContainsKey(key))
						{
							return this.Dictionary[key];
						}
						try
						{
							this.Lock.EnterWriteLock();
							this.Dictionary[key] = tvalue;
						}
						finally
						{
							try
							{
								this.Lock.ExitWriteLock();
							}
							catch (SynchronizationLockException)
							{
							}
						}
					}
					finally
					{
						try
						{
							this.Lock.ExitUpgradeableReadLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
					flag = false;
					result = tvalue;
				}
				finally
				{
					if (flag && this.ValueDestroyer != null)
					{
						this.ValueDestroyer(tvalue);
					}
				}
				return result;
			}
		}

		public void Remove(TKey key)
		{
			base.CheckDisposed();
			try
			{
				this.Lock.EnterUpgradeableReadLock();
				if (this.Dictionary.ContainsKey(key))
				{
					TValue tvalue = this.Dictionary[key];
					try
					{
						this.Lock.EnterWriteLock();
						IDisposable disposable = tvalue as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
						else if (this.ValueDestroyer != null)
						{
							this.ValueDestroyer(tvalue);
						}
						this.Dictionary.Remove(key);
					}
					finally
					{
						try
						{
							this.Lock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			finally
			{
				try
				{
					this.Lock.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ThreadSafeCache<TKey, TValue>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				foreach (TValue tvalue in this.Dictionary.Values)
				{
					IDisposable disposable = tvalue as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					else if (this.ValueDestroyer != null)
					{
						this.ValueDestroyer(tvalue);
					}
				}
				this.Dictionary.Clear();
				this.Dictionary = null;
				this.Lock.Dispose();
			}
		}

		public delegate TValue ValueCreatorDelegate(TKey key);

		public delegate void ValueDestroyerDelegate(TValue val);
	}
}
