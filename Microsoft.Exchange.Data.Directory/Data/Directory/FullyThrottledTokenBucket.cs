using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class FullyThrottledTokenBucket : ITokenBucket
	{
		public FullyThrottledTokenBucket(ITokenBucket oldBucket)
		{
			if (oldBucket != null)
			{
				this.pendingCharges = oldBucket.PendingCharges;
			}
		}

		public int PendingCharges
		{
			get
			{
				return this.pendingCharges;
			}
		}

		public DateTime? LockedUntilUtc
		{
			get
			{
				return new DateTime?(DateTime.MaxValue);
			}
		}

		public DateTime? LockedAt
		{
			get
			{
				return new DateTime?(TimeProvider.UtcNow);
			}
		}

		public bool Locked
		{
			get
			{
				return true;
			}
		}

		public int MaximumBalance
		{
			get
			{
				return 0;
			}
		}

		public int MinimumBalance
		{
			get
			{
				return int.MinValue;
			}
		}

		public int RechargeRate
		{
			get
			{
				return 0;
			}
		}

		public float GetBalance()
		{
			return (float)this.MinimumBalance;
		}

		public DateTime LastUpdateUtc
		{
			get
			{
				return TimeProvider.UtcNow;
			}
		}

		public void Increment()
		{
			Interlocked.Increment(ref this.pendingCharges);
		}

		public void Decrement(TimeSpan extraDuration = default(TimeSpan), bool reverseBudgetCharge = false)
		{
			Interlocked.Decrement(ref this.pendingCharges);
		}

		private int pendingCharges;
	}
}
