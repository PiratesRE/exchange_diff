using System;
using System.Threading;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	public class CachedObject<T>
	{
		public CachedObject(CachedObject<T>.UpdateMethod<T> updateMethod, TimeSpan expirationTime, bool treatNullAsInvalid = false)
		{
			this.updateMethod = updateMethod;
			this.expirationTime = expirationTime;
			this.lastUpdate = DateTime.MinValue;
			this.treatNullAsInvalid = treatNullAsInvalid;
			this.rwLock = new ReaderWriterLockSlim();
		}

		public T GetValue
		{
			get
			{
				this.rwLock.EnterUpgradeableReadLock();
				T result;
				try
				{
					if (DateTime.UtcNow - this.lastUpdate > this.expirationTime || (this.cachedValue == null && this.treatNullAsInvalid))
					{
						this.rwLock.EnterWriteLock();
						try
						{
							this.cachedValue = this.updateMethod();
							this.lastUpdate = DateTime.UtcNow;
						}
						finally
						{
							this.rwLock.ExitWriteLock();
						}
					}
					result = this.cachedValue;
				}
				finally
				{
					this.rwLock.ExitUpgradeableReadLock();
				}
				return result;
			}
		}

		public DateTime LastUpdate
		{
			get
			{
				this.rwLock.EnterReadLock();
				DateTime result;
				try
				{
					result = this.lastUpdate;
				}
				finally
				{
					this.rwLock.ExitReadLock();
				}
				return result;
			}
		}

		internal void ForceUpdate()
		{
			this.rwLock.EnterWriteLock();
			try
			{
				this.cachedValue = this.updateMethod();
				this.lastUpdate = DateTime.UtcNow;
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		private readonly TimeSpan expirationTime;

		private readonly bool treatNullAsInvalid;

		private CachedObject<T>.UpdateMethod<T> updateMethod;

		private DateTime lastUpdate;

		private ReaderWriterLockSlim rwLock;

		private T cachedValue;

		public delegate U UpdateMethod<U>();
	}
}
