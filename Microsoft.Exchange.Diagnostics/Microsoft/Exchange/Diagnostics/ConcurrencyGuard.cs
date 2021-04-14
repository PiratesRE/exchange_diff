using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics
{
	public class ConcurrencyGuard : IConcurrencyGuard
	{
		public string GuardName
		{
			get
			{
				return this.guardName;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				return this.maxConcurrency;
			}
		}

		public bool TrainingMode
		{
			get
			{
				return this.trainingMode;
			}
		}

		public ConcurrencyGuard(string guardName, int maxConcurrency, bool useTrainingMode = false, Action<ConcurrencyGuard, string, object> onIncrementDelegate = null, Action<ConcurrencyGuard, string, object> onDecrementDelegate = null, Action<ConcurrencyGuard, string, object> onNearThresholdDelegate = null, Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException> onRejectDelegate = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("guardName", guardName);
			this.guardName = guardName;
			this.maxConcurrency = maxConcurrency;
			this.concurrencyWarningThreshold = maxConcurrency - maxConcurrency / 20;
			this.onDecrementDelegate = onDecrementDelegate;
			this.onIncrementDelegate = onIncrementDelegate;
			this.onNearThresholdDelegate = onNearThresholdDelegate;
			this.onRejectDelegate = onRejectDelegate;
			this.trainingMode = useTrainingMode;
			this.buckets.Add("_Default", new ConcurrencyGuard.RefCount());
		}

		public static string FormatGuardBucketName(IConcurrencyGuard guard, string bucketName)
		{
			ArgumentValidator.ThrowIfNull("guard", guard);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("bucketName", bucketName);
			return guard.GuardName + "(\"" + bucketName + "\")";
		}

		public long GetCurrentValue()
		{
			return this.GetCurrentValue("_Default");
		}

		public long GetCurrentValue(string bucketName)
		{
			ConcurrencyGuard.RefCount refCount;
			if (this.TryGetRefCount(bucketName, out refCount))
			{
				return refCount.Count;
			}
			return 0L;
		}

		private bool TryGetRefCount(string bucketName, out ConcurrencyGuard.RefCount refCount)
		{
			try
			{
				this.instanceLock.EnterReadLock();
				this.buckets.TryGetValue(bucketName, out refCount);
			}
			finally
			{
				try
				{
					this.instanceLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return refCount != null;
		}

		public long Increment(object stateObject = null)
		{
			return this.Increment("_Default", null);
		}

		public long Increment(string bucketName, object stateObject = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("bucketName", bucketName);
			ConcurrencyGuard.RefCount refCount;
			if (!this.TryGetRefCount(bucketName, out refCount))
			{
				try
				{
					this.instanceLock.EnterWriteLock();
					if (!this.buckets.TryGetValue(bucketName, out refCount) && refCount == null)
					{
						refCount = new ConcurrencyGuard.RefCount();
						this.buckets.Add(bucketName, refCount);
					}
				}
				finally
				{
					try
					{
						this.instanceLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			long count = refCount.Count;
			ExTraceGlobals.ConcurrencyGuardTracer.TraceDebug<string, long, int>((long)this.GetHashCode(), "[ConcurrencyGuard::Increment]: Guard {0} is at {1}/{2}.", ConcurrencyGuard.FormatGuardBucketName(this, bucketName), count, this.MaxConcurrency);
			if (this.onNearThresholdDelegate != null && count > (long)this.concurrencyWarningThreshold)
			{
				this.onNearThresholdDelegate(this, bucketName, stateObject);
			}
			if (count + 1L > (long)this.MaxConcurrency)
			{
				ExTraceGlobals.ConcurrencyGuardTracer.TraceError<string, long, int>((long)this.GetHashCode(), "[ConcurrencyGuard::Increment]: Guard {0} is at concurrency limit {1}/{2} - REJECTING.", ConcurrencyGuard.FormatGuardBucketName(this, bucketName), count, this.MaxConcurrency);
				MaxConcurrencyReachedException ex = new MaxConcurrencyReachedException(this, bucketName);
				if (this.onRejectDelegate != null)
				{
					this.onRejectDelegate(this, bucketName, stateObject, ex);
				}
				if (!this.TrainingMode)
				{
					throw ex;
				}
			}
			refCount.Increment();
			if (this.onIncrementDelegate != null)
			{
				this.onIncrementDelegate(this, bucketName, stateObject);
			}
			return count;
		}

		public long Decrement(object stateObject = null)
		{
			return this.Decrement("_Default", null);
		}

		public long Decrement(string bucketName, object stateObject = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("bucketName", bucketName);
			ConcurrencyGuard.RefCount refCount = null;
			if (!this.TryGetRefCount(bucketName, out refCount))
			{
				return 0L;
			}
			if (refCount.Count > 0L)
			{
				long num = refCount.Decrement();
				ExTraceGlobals.ConcurrencyGuardTracer.TraceDebug<string, long, int>((long)this.GetHashCode(), "[ConcurrencyGuard::Decrement]: Guard {0} is at {1}/{2}.", ConcurrencyGuard.FormatGuardBucketName(this, bucketName), num, this.MaxConcurrency);
				if (this.onDecrementDelegate != null)
				{
					this.onDecrementDelegate(this, bucketName, stateObject);
				}
				return num;
			}
			bool flag = false;
			refCount.Reset();
			if (flag)
			{
				throw new ApplicationException("Cannot decrement guard " + ConcurrencyGuard.FormatGuardBucketName(this, bucketName) + " below 0. This usually indicates a bug in your code.");
			}
			return 0L;
		}

		public const string DefaultBucketName = "_Default";

		private readonly string guardName;

		private readonly int maxConcurrency;

		private readonly bool trainingMode;

		private readonly int concurrencyWarningThreshold;

		private readonly Action<ConcurrencyGuard, string, object> onIncrementDelegate;

		private readonly Action<ConcurrencyGuard, string, object> onDecrementDelegate;

		private readonly Action<ConcurrencyGuard, string, object> onNearThresholdDelegate;

		private readonly Action<ConcurrencyGuard, string, object, MaxConcurrencyReachedException> onRejectDelegate;

		private Dictionary<string, ConcurrencyGuard.RefCount> buckets = new Dictionary<string, ConcurrencyGuard.RefCount>(1);

		private readonly ReaderWriterLockSlim instanceLock = new ReaderWriterLockSlim();

		private class RefCount
		{
			public long Count
			{
				get
				{
					return this.count;
				}
			}

			public long Increment()
			{
				return Interlocked.Increment(ref this.count);
			}

			public long Decrement()
			{
				return Interlocked.Decrement(ref this.count);
			}

			public void Reset()
			{
				lock (this)
				{
					this.count = 0L;
				}
			}

			private long count;
		}
	}
}
