using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class UnthrottledTokenBucket : ITokenBucket
	{
		public UnthrottledTokenBucket(ITokenBucket oldBucket)
		{
			this.pendingCharges = ((oldBucket == null) ? 0 : oldBucket.PendingCharges);
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
				return null;
			}
		}

		public DateTime? LockedAt
		{
			get
			{
				return null;
			}
		}

		public bool Locked
		{
			get
			{
				return false;
			}
		}

		public int MaximumBalance
		{
			get
			{
				return int.MaxValue;
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
				return int.MaxValue;
			}
		}

		public float GetBalance()
		{
			return (float)this.MaximumBalance;
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
