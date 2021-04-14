using System;
using System.Threading;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class RefCount
	{
		public RefCount()
		{
			this.releaseOnDispose = new RefCount.ReleaseOnDispose(this);
		}

		public bool IsDisabled
		{
			get
			{
				return (Thread.VolatileRead(ref this.referenceCount) & int.MinValue) != 0;
			}
		}

		public int Count
		{
			get
			{
				return Thread.VolatileRead(ref this.referenceCount) & int.MaxValue;
			}
		}

		public IDisposable AcquireReference()
		{
			if (this.TryAddRef())
			{
				return this.releaseOnDispose;
			}
			return null;
		}

		public bool TryAddRef()
		{
			int num = Thread.VolatileRead(ref this.referenceCount);
			while ((num & -2147483648) == 0)
			{
				int num2 = Interlocked.CompareExchange(ref this.referenceCount, num + 1, num);
				if (num == num2)
				{
					return true;
				}
				num = num2;
			}
			return false;
		}

		public void AddRef()
		{
			if (!this.TryAddRef())
			{
				throw new InvalidOperationException("RefCount has been disabled");
			}
		}

		public void Release()
		{
			int num = Interlocked.Decrement(ref this.referenceCount);
			if (num == -2147483648)
			{
				this.disabledAndZeroEvent.Set();
			}
		}

		public bool DisableAddRef()
		{
			this.disabledAndZeroEvent = new ManualResetEvent(false);
			int num = Thread.VolatileRead(ref this.referenceCount);
			int num2;
			while ((num2 = Interlocked.CompareExchange(ref this.referenceCount, num | -2147483648, num)) != num)
			{
				num = num2;
			}
			if ((num2 & 2147483647) == 0)
			{
				this.disabledAndZeroEvent.Dispose();
				this.disabledAndZeroEvent = null;
				return true;
			}
			return false;
		}

		public bool TryWaitForZero(TimeSpan waitTime)
		{
			if (this.DisableAddRef())
			{
				return true;
			}
			if (!this.disabledAndZeroEvent.WaitOne(waitTime))
			{
				return false;
			}
			this.disabledAndZeroEvent.Dispose();
			this.disabledAndZeroEvent = null;
			return true;
		}

		private const int DisabledFlag = -2147483648;

		private const int LastReferenceAndDisabled = -2147483648;

		private readonly RefCount.ReleaseOnDispose releaseOnDispose;

		private int referenceCount;

		private ManualResetEvent disabledAndZeroEvent;

		private class ReleaseOnDispose : IDisposable
		{
			public ReleaseOnDispose(RefCount refCount)
			{
				this.refCount = refCount;
			}

			public void Dispose()
			{
				this.refCount.Release();
			}

			private readonly RefCount refCount;
		}
	}
}
