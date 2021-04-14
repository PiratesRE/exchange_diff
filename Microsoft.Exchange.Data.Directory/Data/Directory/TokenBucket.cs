using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TokenBucket : ITokenBucket
	{
		public static ITokenBucket Create(ITokenBucket tokenBucket, Unlimited<uint> maxBalance, Unlimited<uint> rechargeRate, Unlimited<uint> minBalance, BudgetKey budgetKey)
		{
			if (rechargeRate == 0U)
			{
				return new FullyThrottledTokenBucket(tokenBucket);
			}
			if (rechargeRate == 2147483647U || rechargeRate.IsUnlimited)
			{
				return new UnthrottledTokenBucket(tokenBucket);
			}
			TokenBucket tokenBucket2 = tokenBucket as TokenBucket;
			if (tokenBucket2 != null && tokenBucket2.BudgetKey == budgetKey)
			{
				tokenBucket2.UpdateSettings(maxBalance, rechargeRate, minBalance);
				return tokenBucket;
			}
			return new TokenBucket(tokenBucket, maxBalance, rechargeRate, minBalance, budgetKey);
		}

		public static ITokenBucket Create(Unlimited<uint> maxBalance, Unlimited<uint> rechargeRate, Unlimited<uint> minBalance, BudgetKey budgetKey)
		{
			return TokenBucket.Create(null, maxBalance, rechargeRate, minBalance, budgetKey);
		}

		private TokenBucket(ITokenBucket oldBucket, Unlimited<uint> maxBalance, Unlimited<uint> rechargeRate, Unlimited<uint> minBalance, BudgetKey budgetKey)
		{
			this.BudgetKey = budgetKey;
			this.LastUpdateUtc = TimeProvider.UtcNow;
			this.UpdateSettings(maxBalance, rechargeRate, minBalance);
			this.balance = (maxBalance.IsUnlimited ? 2147483647U : maxBalance.Value);
			if (oldBucket != null)
			{
				this.PendingCharges = oldBucket.PendingCharges;
			}
		}

		public BudgetKey BudgetKey { get; private set; }

		public int PendingCharges { get; private set; }

		public DateTime? LockedUntilUtc
		{
			get
			{
				DateTime? lockedUntilUtcNonUpdating;
				lock (this.instanceLock)
				{
					this.Update(default(TimeSpan));
					lockedUntilUtcNonUpdating = this.LockedUntilUtcNonUpdating;
				}
				return lockedUntilUtcNonUpdating;
			}
		}

		public DateTime? LockedAt
		{
			get
			{
				DateTime? result;
				lock (this.instanceLock)
				{
					this.Update(default(TimeSpan));
					result = (this.locked ? new DateTime?(this.lockedAt) : null);
				}
				return result;
			}
		}

		private DateTime? LockedUntilUtcNonUpdating
		{
			get
			{
				DateTime? result;
				lock (this.instanceLock)
				{
					if (this.locked)
					{
						DateTime dateTime = this.lockedAt + TokenBucket.MinimumLockoutTime;
						float num = (float)this.MinimumBalance - this.balance;
						DateTime dateTime2 = (num > 0f) ? TimeProvider.UtcNow.AddMilliseconds((double)((float)(3600000 / this.RechargeRate) * num)) : DateTime.MinValue;
						result = new DateTime?((dateTime > dateTime2) ? dateTime : dateTime2);
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
		}

		public bool Locked
		{
			get
			{
				bool result;
				lock (this.instanceLock)
				{
					this.Update(default(TimeSpan));
					result = this.locked;
				}
				return result;
			}
		}

		public int MaximumBalance { get; private set; }

		public int MinimumBalance { get; private set; }

		public int RechargeRate { get; private set; }

		public float GetBalance()
		{
			float result;
			lock (this.instanceLock)
			{
				this.Update(default(TimeSpan));
				result = this.balance;
			}
			return result;
		}

		public DateTime LastUpdateUtc { get; private set; }

		public void Increment()
		{
			lock (this.instanceLock)
			{
				this.Update(default(TimeSpan));
				this.PendingCharges++;
			}
		}

		public void Decrement(TimeSpan extraDuration = default(TimeSpan), bool reverseBudgetCharge = false)
		{
			lock (this.instanceLock)
			{
				if (reverseBudgetCharge)
				{
					this.Update(-extraDuration);
				}
				else
				{
					this.Update(extraDuration);
				}
				this.PendingCharges--;
			}
		}

		private void UpdateSettings(Unlimited<uint> maxBalance, Unlimited<uint> rechargeRate, Unlimited<uint> minBalance)
		{
			if (rechargeRate == 0U)
			{
				throw new ArgumentOutOfRangeException("rechargeRate", rechargeRate, "rechargeRate must be greater than zero.");
			}
			lock (this.instanceLock)
			{
				this.Update(default(TimeSpan));
				this.MaximumBalance = (int)(maxBalance.IsUnlimited ? 2147483647U : maxBalance.Value);
				this.MinimumBalance = (int)(minBalance.IsUnlimited ? 2147483648U : (uint.MaxValue * minBalance.Value));
				this.RechargeRate = (int)(rechargeRate.IsUnlimited ? 2147483647U : rechargeRate.Value);
				this.rechargeRateMsec = (double)this.RechargeRate / 3600000.0;
				if (!minBalance.IsUnlimited && this.balance < (float)this.MinimumBalance)
				{
					this.LockBucket();
				}
				else if (this.balance > (float)this.MaximumBalance)
				{
					this.balance = (float)this.MaximumBalance;
				}
				if (this.locked && this.balance > (float)this.MinimumBalance)
				{
					this.UnlockBucket();
				}
			}
		}

		internal void SetBalanceForTest(float newBalance)
		{
			lock (this.instanceLock)
			{
				this.locked = false;
				this.balance = newBalance;
				this.LastUpdateUtc = TimeProvider.UtcNow;
				this.Update(default(TimeSpan));
			}
		}

		private void Update(TimeSpan extraDuration = default(TimeSpan))
		{
			DateTime utcNow = TimeProvider.UtcNow;
			TimeSpan t = utcNow - this.LastUpdateUtc;
			if (t <= TimeSpan.Zero)
			{
				return;
			}
			this.LastUpdateUtc = utcNow;
			double num = this.rechargeRateMsec - (double)this.PendingCharges;
			this.balance += (float)(num * t.TotalMilliseconds - extraDuration.TotalMilliseconds);
			if (this.balance > (float)this.MaximumBalance)
			{
				this.balance = (float)this.MaximumBalance;
			}
			if (this.MinimumBalance != -2147483648 && this.balance < (float)this.MinimumBalance && !this.locked)
			{
				this.LockBucket();
				return;
			}
			if (this.locked && this.LockedUntilUtcNonUpdating <= TimeProvider.UtcNow)
			{
				this.UnlockBucket();
			}
		}

		private void UnlockBucket()
		{
			lock (this.instanceLock)
			{
				this.locked = false;
				this.lockedAt = DateTime.MinValue;
			}
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[TokenBucket.UnlockBucket] Bucket is now unlocked. Resetting state.");
			if (Globals.ProcessInstanceType != InstanceType.NotInitialized)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_UserNoLongerLockedOutThrottling, string.Empty, new object[]
				{
					this.BudgetKey
				});
			}
		}

		private void LockBucket()
		{
			lock (this.instanceLock)
			{
				this.locked = true;
				this.lockedAt = TimeProvider.UtcNow;
			}
			DateTime value = this.LockedUntilUtcNonUpdating.Value;
			ThrottlingPerfCounterWrapper.IncrementBudgetsLockedOut(this.BudgetKey, value - TimeProvider.UtcNow);
			if (Globals.ProcessInstanceType != InstanceType.NotInitialized)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_UserLockedOutThrottling, string.Empty, new object[]
				{
					this.BudgetKey,
					value,
					this.GetTraceInt(this.MaximumBalance),
					this.GetTraceInt(this.RechargeRate),
					this.GetTraceInt(this.MinimumBalance)
				});
			}
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<DateTime, int>((long)this.GetHashCode(), "[TokenBucket.LockBucket] Bucket locked until {0}.  Current Pending charges: {1}", value, this.PendingCharges);
		}

		private string GetTraceInt(int valueToTrace)
		{
			if (valueToTrace != 2147483647)
			{
				return valueToTrace.ToString();
			}
			return "$null";
		}

		private const int MsecPerHour = 3600000;

		public static readonly TimeSpan MinimumLockoutTime = TimeSpan.FromMinutes(5.0);

		private double rechargeRateMsec;

		private float balance;

		private bool locked;

		private DateTime lockedAt;

		private object instanceLock = new object();
	}
}
